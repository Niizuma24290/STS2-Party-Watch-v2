using System;
using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2PartyWatch.Combat;
using STS2PartyWatch.Forecast;
using STS2PartyWatch.UI;

namespace STS2PartyWatch.Patches;

[HarmonyPatch(typeof(NHealthBar))]
internal static class ForecastRefreshPatch
{
    private const string MainLabelName = "STS2PartyWatchForecastLabel";
    private const string DetailLabelName = "STS2PartyWatchForecastDetailsLabel";
    private const string HealthBarCenterGuideName = "STS2PartyWatchHealthBarCenterGuide";
    private const string MainHudTextCenterGuideName = "STS2PartyWatchHudTextCenterGuide";
    private static readonly FieldInfo? CreatureField = typeof(NHealthBar).GetField("_creature", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly LocalIncomingDamageReader Reader = new();
    private static readonly LocalDamageForecast Forecast = new();
    private static readonly List<WeakReference<NHealthBar>> RegisteredBars = new();
    private static string? LastAlignmentDebugLine;

    static ForecastRefreshPatch()
    {
        PartyWatchUiSettings.Changed += RefreshRegisteredBars;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(NHealthBar.SetCreature))]
    private static void SetCreaturePostfix(NHealthBar __instance)
    {
        Refresh(__instance, null);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(NHealthBar.RefreshValues))]
    private static void RefreshValuesPostfix(NHealthBar __instance)
    {
        Refresh(__instance, null);
    }

    [HarmonyPostfix]
    [HarmonyPatch("SetHpBarContainerSizeWithOffsets")]
    private static void ResizePostfix(NHealthBar __instance, Vector2 size)
    {
        Refresh(__instance, size);
    }

    private static void Refresh(NHealthBar bar, Vector2? containerSize)
    {
        if (!TryGetLocalCreature(bar, out var creature) || creature is null)
        {
            HideExisting(bar);
            return;
        }

        RegisterBar(bar);

        var mainLabel = GetOrCreateMainLabel(bar);
        var detailLabel = GetOrCreateDetailLabel(bar);
        var healthBarCenterGuide = GetOrCreateHealthBarCenterGuide(bar);
        var mainHudTextCenterGuide = GetOrCreateMainHudTextCenterGuide(bar);
        if (mainLabel is null || detailLabel is null)
        {
            HideOptional(healthBarCenterGuide);
            HideOptional(mainHudTextCenterGuide);
            return;
        }

        PartyWatchHudDisplay.ApplyMainHudStyle(mainLabel);
        PartyWatchHudDisplay.ApplyDetailHudStyle(detailLabel);
        ObservedHpLossBudgetTracker.Observe(creature);

        if (!PartyWatchHudVisibilityPolicy.ShouldRenderHud(bar, creature))
        {
            PartyWatchHudSnapshotStore.Clear();
            Hide(mainLabel, detailLabel);
            HideOptional(healthBarCenterGuide);
            HideOptional(mainHudTextCenterGuide);
            return;
        }

        var result = PartyWatchHudSnapshotStore.TryGetCommitted(creature, out var committed)
            ? committed
            : PartyWatchHudSnapshotStore.ResolveDisplayResult(
                creature,
                Forecast.Calculate(Reader.ReadForLocalCreature(creature)));
        if (result.State != ForecastResultState.KnownDamage || (result.OutDamage <= 0 && result.DirectHpLoss <= 0))
        {
            Hide(mainLabel, detailLabel);
            HideOptional(healthBarCenterGuide);
            HideOptional(mainHudTextCenterGuide);
            return;
        }

        mainLabel.Text = PartyWatchHudDisplay.BuildMainHudDisplay(result);
        PartyWatchHudDisplay.ApplyMainHudTextBounds(mainLabel);
        Reposition(bar, mainLabel, detailLabel, containerSize);
        mainLabel.Show();
        ShowHealthBarCenterGuide(bar, mainLabel, detailLabel, healthBarCenterGuide, containerSize);
        ShowMainHudTextCenterGuide(mainLabel, mainHudTextCenterGuide);
        LogAlignmentIfChanged(bar, mainLabel, healthBarCenterGuide, mainHudTextCenterGuide, containerSize);

        var details = PartyWatchHudDisplay.BuildHudDetails(result);
        if (string.IsNullOrEmpty(details))
        {
            Hide(detailLabel);
            return;
        }

        detailLabel.Text = details;
        detailLabel.Show();
    }

    private static bool TryGetLocalCreature(NHealthBar bar, out Creature? creature)
    {
        creature = GetCreature(bar);
        if (creature?.Player is null)
        {
            return false;
        }

        try
        {
            var localPlayer = LocalContext.GetMe(creature.CombatState);
            return localPlayer is not null && creature.Player.NetId == localPlayer.NetId;
        }
        catch
        {
            return false;
        }
    }

    private static Creature? GetCreature(NHealthBar bar)
    {
        return CreatureField?.GetValue(bar) as Creature;
    }

    private static Label? GetOrCreateMainLabel(NHealthBar bar)
    {
        var parent = GetLabelParent(bar);
        if (parent is null)
        {
            return null;
        }

        var existing = parent.GetNodeOrNull<Label>(MainLabelName);
        if (existing is not null)
        {
            return existing;
        }

        var stale = parent.GetNodeOrNull<Control>(MainLabelName);
        if (stale is not null)
        {
            stale.Name = $"{MainLabelName}Stale";
            stale.QueueFree();
        }

        var label = new Label
        {
            Name = MainLabelName,
            MouseFilter = Control.MouseFilterEnum.Ignore,
            ZIndex = 50,
            Text = string.Empty,
            Visible = false,
        };
        PartyWatchHudDisplay.ApplyMainHudStyle(label);

        parent.AddChild(label);
        return label;
    }

    private static RichTextLabel? GetOrCreateDetailLabel(NHealthBar bar)
    {
        var parent = GetLabelParent(bar);
        if (parent is null)
        {
            return null;
        }

        var existing = parent.GetNodeOrNull<RichTextLabel>(DetailLabelName);
        if (existing is not null)
        {
            return existing;
        }

        var label = new RichTextLabel
        {
            Name = DetailLabelName,
            MouseFilter = Control.MouseFilterEnum.Ignore,
            ZIndex = 50,
            Text = string.Empty,
            Visible = false,
        };
        PartyWatchHudDisplay.ApplyDetailHudStyle(label);

        parent.AddChild(label);
        return label;
    }

    private static ColorRect? GetOrCreateHealthBarCenterGuide(NHealthBar bar)
    {
        var parent = GetLabelParent(bar);
        if (parent is null)
        {
            return null;
        }

        var existing = parent.GetNodeOrNull<ColorRect>(HealthBarCenterGuideName);
        if (existing is not null)
        {
            return existing;
        }

        var stale = parent.GetNodeOrNull<Control>(HealthBarCenterGuideName);
        if (stale is not null)
        {
            stale.Name = $"{HealthBarCenterGuideName}Stale";
            stale.QueueFree();
        }

        var guide = new ColorRect
        {
            Name = HealthBarCenterGuideName,
            MouseFilter = Control.MouseFilterEnum.Ignore,
            ZIndex = 49,
            Visible = false,
        };

        parent.AddChild(guide);
        return guide;
    }

    private static ColorRect? GetOrCreateMainHudTextCenterGuide(NHealthBar bar)
    {
        var parent = GetLabelParent(bar);
        if (parent is null)
        {
            return null;
        }

        var existing = parent.GetNodeOrNull<ColorRect>(MainHudTextCenterGuideName);
        if (existing is not null)
        {
            return existing;
        }

        var stale = parent.GetNodeOrNull<Control>(MainHudTextCenterGuideName);
        if (stale is not null)
        {
            stale.Name = $"{MainHudTextCenterGuideName}Stale";
            stale.QueueFree();
        }

        var guide = new ColorRect
        {
            Name = MainHudTextCenterGuideName,
            MouseFilter = Control.MouseFilterEnum.Ignore,
            ZIndex = 60,
            Visible = false,
        };

        parent.AddChild(guide);
        return guide;
    }

    private static Control? GetLabelParent(NHealthBar bar)
    {
        return bar.HpBarContainer?.GetParent() as Control ?? bar;
    }

    private static void Reposition(NHealthBar bar, Label mainLabel, RichTextLabel detailLabel, Vector2? containerSize)
    {
        var container = bar.HpBarContainer;
        if (container is null)
        {
            return;
        }

        var isMultiplayer = GetCreature(bar)?.CombatState?.Players.Count > 1;
        PartyWatchHudDisplay.ApplyHudPosition(
            container,
            mainLabel,
            detailLabel,
            containerSize,
            isMultiplayer);
    }

    private static void HideExisting(NHealthBar bar)
    {
        var parent = GetLabelParent(bar);
        var mainLabel = parent?.GetNodeOrNull<Label>(MainLabelName);
        var staleMainLabel = parent?.GetNodeOrNull<Control>(MainLabelName);
        var detailLabel = parent?.GetNodeOrNull<RichTextLabel>(DetailLabelName);
        var healthBarCenterGuide = parent?.GetNodeOrNull<ColorRect>(HealthBarCenterGuideName);
        var mainHudTextCenterGuide = parent?.GetNodeOrNull<ColorRect>(MainHudTextCenterGuideName);
        if (mainLabel is not null)
        {
            Hide(mainLabel);
        }
        else if (staleMainLabel is not null)
        {
            staleMainLabel.Name = $"{MainLabelName}Stale";
            staleMainLabel.QueueFree();
        }

        if (detailLabel is not null)
        {
            Hide(detailLabel);
        }

        HideOptional(healthBarCenterGuide);
        HideOptional(mainHudTextCenterGuide);
    }

    private static void Hide(Label mainLabel, RichTextLabel detailLabel)
    {
        Hide(mainLabel);
        Hide(detailLabel);
    }

    private static void Hide(Control label)
    {
        if (label is Label simpleLabel)
        {
            simpleLabel.Text = string.Empty;
        }
        else if (label is RichTextLabel richTextLabel)
        {
            richTextLabel.Text = string.Empty;
        }

        label.Hide();
    }

    private static void HideOptional(Control? control)
    {
        control?.Hide();
    }

    private static void ShowHealthBarCenterGuide(
        NHealthBar bar,
        Label mainLabel,
        RichTextLabel detailLabel,
        ColorRect? healthBarCenterGuide,
        Vector2? containerSize)
    {
        var container = bar.HpBarContainer;
        if (container is null || healthBarCenterGuide is null)
        {
            HideOptional(healthBarCenterGuide);
            return;
        }

        PartyWatchHudDebugGuide.ApplyHealthBarCenterGuide(
            healthBarCenterGuide,
            container,
            mainLabel,
            detailLabel,
            containerSize);
    }

    private static void ShowMainHudTextCenterGuide(Label mainLabel, ColorRect? mainHudTextCenterGuide)
    {
        if (mainHudTextCenterGuide is null)
        {
            return;
        }

        PartyWatchHudDebugGuide.ApplyMainHudTextCenterGuide(mainHudTextCenterGuide, mainLabel);
    }

    private static void LogAlignmentIfChanged(
        NHealthBar bar,
        Label mainLabel,
        ColorRect? healthBarCenterGuide,
        ColorRect? mainHudTextCenterGuide,
        Vector2? containerSize)
    {
        var container = bar.HpBarContainer;
        if (container is null)
        {
            return;
        }

        var isMultiplayer = GetCreature(bar)?.CombatState?.Players.Count > 1;
        var line = PartyWatchHudDebugGuide.BuildAlignmentDebugLine(
            container,
            mainLabel,
            healthBarCenterGuide,
            mainHudTextCenterGuide,
            containerSize,
            isMultiplayer);
        if (line == LastAlignmentDebugLine)
        {
            return;
        }

        LastAlignmentDebugLine = line;
        GD.Print(line);
    }

    private static void RegisterBar(NHealthBar bar)
    {
        for (var i = RegisteredBars.Count - 1; i >= 0; i--)
        {
            if (!RegisteredBars[i].TryGetTarget(out var existing) || !GodotObject.IsInstanceValid(existing))
            {
                RegisteredBars.RemoveAt(i);
                continue;
            }

            if (ReferenceEquals(existing, bar))
            {
                return;
            }
        }

        RegisteredBars.Add(new WeakReference<NHealthBar>(bar));
    }

    internal static void RefreshRegisteredBars()
    {
        for (var i = RegisteredBars.Count - 1; i >= 0; i--)
        {
            if (!RegisteredBars[i].TryGetTarget(out var bar) || !GodotObject.IsInstanceValid(bar))
            {
                RegisteredBars.RemoveAt(i);
                continue;
            }

            Refresh(bar, null);
        }
    }

    internal static void CommitFinalSnapshot(Creature creature)
    {
        var player = creature.Player;
        if (player is null)
        {
            return;
        }

        ObservedHpLossBudgetTracker.Observe(creature);
        var result = Forecast.Calculate(Reader.ReadForLocalCreature(creature));
        PartyWatchHudSnapshotStore.OnPlayerTurnEnding(player, creature, result);
    }
}

[HarmonyPatch(typeof(CardPile))]
internal static class ForecastHandChangePatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(CardPile.InvokeContentsChanged))]
    private static void InvokeContentsChangedPostfix(CardPile __instance)
    {
        if (__instance.Type == PileType.Hand)
        {
            ForecastRefreshPatch.RefreshRegisteredBars();
        }
    }
}

[HarmonyPatch(typeof(Player))]
internal static class ForecastRelicChangePatch
{
    [HarmonyPostfix]
    [HarmonyPatch("AddRelicInternal")]
    private static void AddRelicInternalPostfix()
    {
        ForecastRefreshPatch.RefreshRegisteredBars();
    }

    [HarmonyPostfix]
    [HarmonyPatch("RemoveRelicInternal")]
    private static void RemoveRelicInternalPostfix()
    {
        ForecastRefreshPatch.RefreshRegisteredBars();
    }

    [HarmonyPostfix]
    [HarmonyPatch("MeltRelicInternal")]
    private static void MeltRelicInternalPostfix()
    {
        ForecastRefreshPatch.RefreshRegisteredBars();
    }
}

[HarmonyPatch(typeof(BeatingRemnant))]
internal static class ForecastBeatingRemnantBudgetPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(BeatingRemnant.BeforeSideTurnStart))]
    private static void BeforeSideTurnStartPostfix(BeatingRemnant __instance, object[] __args)
    {
        if (__args.Length < 3 || __args[2] is not IReadOnlyList<Creature> creaturesStartingTurn)
        {
            return;
        }

        var owner = __instance.Owner;
        var ownerCreature = owner?.Creature;
        if (owner is null || ownerCreature is null || !creaturesStartingTurn.Contains(ownerCreature))
        {
            return;
        }

        ObservedHpLossBudgetTracker.ResetWindow(owner);
        ForecastRefreshPatch.RefreshRegisteredBars();
    }
}

[HarmonyPatch(typeof(Hook))]
internal static class ForecastTurnLifecyclePatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Hook.BeforeSideTurnStart))]
    private static void BeforeSideTurnStartPostfix(
        ICombatState combatState,
        CombatSide side,
        IReadOnlyList<Creature> participants)
    {
        if (!IsPlayerSide(side)
            || !TryGetLocalParticipant(combatState, participants, out var creature)
            || creature is null)
        {
            return;
        }

        var player = creature.Player;
        if (player is null)
        {
            return;
        }

        PartyWatchHudSnapshotStore.OnPlayerSideTurnStarted(player, creature);
        ForecastRefreshPatch.RefreshRegisteredBars();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Hook.BeforeTurnEnd))]
    private static void BeforeTurnEndPostfix(
        ICombatState combatState,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!IsPlayerSide(side)
            || !TryGetLocalParticipant(combatState, participants, out var creature)
            || creature is null)
        {
            return;
        }

        var player = creature.Player;
        if (player is null)
        {
            return;
        }

        ForecastRefreshPatch.CommitFinalSnapshot(creature);
        ForecastRefreshPatch.RefreshRegisteredBars();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Hook.AfterCombatEnd))]
    private static void AfterCombatEndPostfix()
    {
        PartyWatchHudSnapshotStore.Clear();
        ForecastRefreshPatch.RefreshRegisteredBars();
    }

    private static bool TryGetLocalParticipant(
        ICombatState combatState,
        IEnumerable<Creature> participants,
        out Creature? creature)
    {
        creature = null;
        try
        {
            var localPlayer = LocalContext.GetMe(combatState);
            var localCreature = localPlayer?.Creature;
            if (localPlayer is null || localCreature is null)
            {
                return false;
            }

            foreach (var participant in participants)
            {
                if (ReferenceEquals(participant, localCreature)
                    || participant.Player?.NetId == localPlayer.NetId)
                {
                    creature = participant;
                    return participant.Player is not null;
                }
            }
        }
        catch
        {
            return false;
        }

        return false;
    }

    private static bool IsPlayerSide(CombatSide side)
    {
        return side.ToString().Contains("Player", StringComparison.OrdinalIgnoreCase);
    }
}
