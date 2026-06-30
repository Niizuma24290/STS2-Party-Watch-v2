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
    private const string LabelName = "STS2PartyWatchForecastLabel";
    private static readonly FieldInfo? CreatureField = typeof(NHealthBar).GetField("_creature", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly LocalIncomingDamageReader Reader = new();
    private static readonly LocalDamageForecast Forecast = new();
    private static readonly List<WeakReference<NHealthBar>> RegisteredBars = new();

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

        var label = GetOrCreateLabel(bar);
        if (label is null)
        {
            return;
        }

        PartyWatchHudDisplay.ApplyHudStyle(label);
        Reposition(bar, label, containerSize);
        ObservedHpLossBudgetTracker.Observe(creature);

        if (!PartyWatchHudVisibilityPolicy.ShouldRenderHud(bar, creature))
        {
            PartyWatchHudSnapshotStore.Clear();
            Hide(label);
            return;
        }

        var result = PartyWatchHudSnapshotStore.TryGetCommitted(creature, out var committed)
            ? committed
            : PartyWatchHudSnapshotStore.ResolveDisplayResult(
                creature,
                Forecast.Calculate(Reader.ReadForLocalCreature(creature)));
        if (result.State != ForecastResultState.KnownDamage || (result.OutDamage <= 0 && result.DirectHpLoss <= 0))
        {
            Hide(label);
            return;
        }

        label.Text = PartyWatchHudDisplay.BuildHudDisplay(result);
        label.Show();
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

    private static Label? GetOrCreateLabel(NHealthBar bar)
    {
        var parent = GetLabelParent(bar);
        if (parent is null)
        {
            return null;
        }

        var existing = parent.GetNodeOrNull<Label>(LabelName);
        if (existing is not null)
        {
            return existing;
        }

        var label = new Label
        {
            Name = LabelName,
            MouseFilter = Control.MouseFilterEnum.Ignore,
            ZIndex = 50,
            Text = string.Empty,
            Visible = false,
        };
        PartyWatchHudDisplay.ApplyHudStyle(label);

        parent.AddChild(label);
        return label;
    }

    private static Control? GetLabelParent(NHealthBar bar)
    {
        return bar.HpBarContainer?.GetParent() as Control ?? bar;
    }

    private static void Reposition(NHealthBar bar, Label label, Vector2? containerSize)
    {
        var container = bar.HpBarContainer;
        if (container is null)
        {
            return;
        }

        PartyWatchHudDisplay.ApplyHudPosition(container, label, containerSize);
    }

    private static void HideExisting(NHealthBar bar)
    {
        var existing = GetLabelParent(bar)?.GetNodeOrNull<Label>(LabelName);
        if (existing is not null)
        {
            Hide(existing);
        }
    }

    private static void Hide(Label label)
    {
        label.Text = string.Empty;
        label.Hide();
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

        PartyWatchHudSnapshotStore.OnPlayerTurnEnding(player, creature);
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
