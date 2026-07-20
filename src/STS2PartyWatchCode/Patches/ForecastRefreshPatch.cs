using System;
using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2PartyWatch.Combat;
#if PARTY_WATCH_VISIBILITY_PROFILING
using STS2PartyWatch.Diagnostics;
#endif
using STS2PartyWatch.Forecast;
using STS2PartyWatch.UI;

namespace STS2PartyWatch.Patches;

[HarmonyPatch(typeof(NHealthBar))]
internal static class ForecastRefreshPatch
{
    private const string MainLabelName = "STS2PartyWatchForecastLabel";
    private const string IncomingLabelName = "STS2PartyWatchIncomingDamageLabel";
    private const string DetailLabelName = "STS2PartyWatchForecastDetailsLabel";
    private static readonly FieldInfo? CreatureField = typeof(NHealthBar).GetField("_creature", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly LocalIncomingDamageReader Reader = new();
    private static readonly LocalDamageForecast Forecast = new();
    private static readonly List<WeakReference<NHealthBar>> RegisteredBars = new();
#if PARTY_WATCH_VISIBILITY_PROFILING
    [ThreadStatic]
    private static int _lastRegisteredBarsVisited;

    internal static int LastRegisteredBarsVisitedForVisibilityProfiling => _lastRegisteredBarsVisited;
#endif

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
        var incomingLabel = GetOrCreateIncomingLabel(bar);
        var detailLabel = GetOrCreateDetailLabel(bar);
        if (mainLabel is null || incomingLabel is null || detailLabel is null)
        {
            return;
        }

        PartyWatchHudDisplay.ApplyMainHudStyle(mainLabel);
        PartyWatchHudDisplay.ApplyIncomingHudStyle(incomingLabel);
        PartyWatchHudDisplay.ApplyDetailHudStyle(detailLabel);
        ObservedHpLossBudgetTracker.Observe(creature);

        if (!PartyWatchHudVisibilityPolicy.ShouldRenderHud(bar, creature, out var temporarilyCovered))
        {
            if (!temporarilyCovered)
            {
                PartyWatchHudSnapshotStore.Clear();
            }

            Hide(mainLabel, incomingLabel, detailLabel);
            return;
        }

        var snapshot = PartyWatchHudSnapshotStore.TryGetCommitted(creature, out var committed)
            ? committed
            : PartyWatchHudSnapshotStore.ResolveDisplayResult(
                creature,
                BuildForecastHudSnapshot(creature));
        if (!PartyWatchHudDisplay.HasDisplayableSnapshot(snapshot))
        {
            Hide(mainLabel, incomingLabel, detailLabel);
            return;
        }

        mainLabel.Text = PartyWatchHudDisplay.ShouldShowExpectedHpLoss(snapshot)
            ? PartyWatchHudDisplay.BuildMainHudDisplay(snapshot.ExpectedHpLoss)
            : string.Empty;
        incomingLabel.Text = PartyWatchHudDisplay.ShouldShowIncomingDamage(snapshot)
            ? PartyWatchHudDisplay.BuildIncomingHudDisplay(snapshot.IncomingDamage)
            : string.Empty;
        var details = PartyWatchHudDisplay.BuildHudDetails(snapshot.ExpectedHpLoss);
        PartyWatchHudDisplay.ApplyMainHudTextBounds(mainLabel);
        PartyWatchHudDisplay.ApplyHudTextBounds(incomingLabel);
        Reposition(bar, mainLabel, incomingLabel, detailLabel, containerSize);
        ShowOrHide(mainLabel);
        ShowOrHide(incomingLabel);
        if (string.IsNullOrEmpty(details))
        {
            Hide(detailLabel);
        }
        else
        {
            detailLabel.Text = details;
            detailLabel.Show();
        }
    }

    private static ForecastHudSnapshot BuildForecastHudSnapshot(Creature creature)
    {
        var expected = Forecast.Calculate(Reader.ReadForLocalCreature(creature));
        var incoming = Reader.ReadIncomingDamageForLocalCreature(creature, new IncomingDamageDisplayOptions(
            PartyWatchUiSettings.IncludeCurrentBlockInIncomingDamage,
            PartyWatchUiSettings.IncludePowerBlockInIncomingDamage,
            PartyWatchUiSettings.IncludeRelicBlockInIncomingDamage,
            PartyWatchUiSettings.IncludePowerHpLossModifiersInIncomingDamage,
            PartyWatchUiSettings.IncludeRelicHpLossModifiersInIncomingDamage));
        return new ForecastHudSnapshot(expected, incoming);
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

    private static Label? GetOrCreateIncomingLabel(NHealthBar bar)
    {
        var parent = GetLabelParent(bar);
        if (parent is null)
        {
            return null;
        }

        var existing = parent.GetNodeOrNull<Label>(IncomingLabelName);
        if (existing is not null)
        {
            return existing;
        }

        var stale = parent.GetNodeOrNull<Control>(IncomingLabelName);
        if (stale is not null)
        {
            stale.Name = $"{IncomingLabelName}Stale";
            stale.QueueFree();
        }

        var label = new Label
        {
            Name = IncomingLabelName,
            MouseFilter = Control.MouseFilterEnum.Ignore,
            ZIndex = 50,
            Text = string.Empty,
            Visible = false,
        };
        PartyWatchHudDisplay.ApplyIncomingHudStyle(label);

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

    private static Control? GetLabelParent(NHealthBar bar)
    {
        return bar.HpBarContainer?.GetParent() as Control ?? bar;
    }

    private static void Reposition(
        NHealthBar bar,
        Label mainLabel,
        Label incomingLabel,
        RichTextLabel detailLabel,
        Vector2? containerSize)
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
            incomingLabel,
            detailLabel,
            containerSize,
            isMultiplayer);
    }

    private static void HideExisting(NHealthBar bar)
    {
        var parent = GetLabelParent(bar);
        var mainLabel = parent?.GetNodeOrNull<Label>(MainLabelName);
        var staleMainLabel = parent?.GetNodeOrNull<Control>(MainLabelName);
        var incomingLabel = parent?.GetNodeOrNull<Label>(IncomingLabelName);
        var staleIncomingLabel = parent?.GetNodeOrNull<Control>(IncomingLabelName);
        var detailLabel = parent?.GetNodeOrNull<RichTextLabel>(DetailLabelName);
        if (mainLabel is not null)
        {
            Hide(mainLabel);
        }
        else if (staleMainLabel is not null)
        {
            staleMainLabel.Name = $"{MainLabelName}Stale";
            staleMainLabel.QueueFree();
        }

        if (incomingLabel is not null)
        {
            Hide(incomingLabel);
        }
        else if (staleIncomingLabel is not null)
        {
            staleIncomingLabel.Name = $"{IncomingLabelName}Stale";
            staleIncomingLabel.QueueFree();
        }

        if (detailLabel is not null)
        {
            Hide(detailLabel);
        }
    }

    private static void Hide(Label mainLabel, Label incomingLabel, RichTextLabel detailLabel)
    {
        Hide(mainLabel);
        Hide(incomingLabel);
        Hide(detailLabel);
    }

    private static void ShowOrHide(Label label)
    {
        if (string.IsNullOrEmpty(label.Text))
        {
            Hide(label);
        }
        else
        {
            label.Show();
        }
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
#if PARTY_WATCH_VISIBILITY_PROFILING
        var barsVisited = 0;
#endif
        for (var i = RegisteredBars.Count - 1; i >= 0; i--)
        {
            if (!RegisteredBars[i].TryGetTarget(out var bar) || !GodotObject.IsInstanceValid(bar))
            {
                RegisteredBars.RemoveAt(i);
                continue;
            }

#if PARTY_WATCH_VISIBILITY_PROFILING
            barsVisited++;
#endif
            Refresh(bar, null);
        }
#if PARTY_WATCH_VISIBILITY_PROFILING
        _lastRegisteredBarsVisited = barsVisited;
#endif
    }

    internal static void CommitFinalSnapshot(Creature creature)
    {
        var player = creature.Player;
        if (player is null)
        {
            return;
        }

        ObservedHpLossBudgetTracker.Observe(creature);
        var snapshot = BuildForecastHudSnapshot(creature);
        PartyWatchHudSnapshotStore.OnPlayerTurnEnding(player, creature, snapshot);
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
    [HarmonyPatch(nameof(Hook.AfterPlayerTurnStart))]
    private static void AfterPlayerTurnStartPostfix(
        ICombatState combatState,
        PlayerChoiceContext choiceContext,
        Player player)
    {
        try
        {
            var localPlayer = LocalContext.GetMe(combatState);
            var creature = player.Creature;
            if (localPlayer is null
                || creature is null
                || player.NetId != localPlayer.NetId)
            {
                return;
            }

            PartyWatchHudSnapshotStore.OnPlayerSideTurnStarted(player, creature);
            ForecastRefreshPatch.RefreshRegisteredBars();
        }
        catch
        {
        }
    }

    internal static void CommitTurnEndSnapshot(
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
        ObservedHpLossBudgetTracker.Clear();
        ForecastRefreshPatch.RefreshRegisteredBars();
#if PARTY_WATCH_VISIBILITY_PROFILING
        Aud0007VisibilityProfiler.Dump("combat-end", reset: true);
#endif
    }

    internal static bool HasHookMethod(string methodName)
    {
        return AccessTools.Method(typeof(Hook), methodName) is not null;
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

[HarmonyPatch(typeof(Hook))]
internal static class ForecastStableTurnEndPatch
{
    [HarmonyPrepare]
    private static bool Prepare()
    {
        return ForecastTurnLifecyclePatch.HasHookMethod("BeforeTurnEnd");
    }

    [HarmonyPostfix]
    [HarmonyPatch("BeforeTurnEnd")]
    private static void BeforeTurnEndPostfix(
        ICombatState combatState,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        ForecastTurnLifecyclePatch.CommitTurnEndSnapshot(combatState, side, participants);
    }
}

[HarmonyPatch(typeof(Hook))]
internal static class ForecastBetaTurnEndPatch
{
    [HarmonyPrepare]
    private static bool Prepare()
    {
        return ForecastTurnLifecyclePatch.HasHookMethod("BeforeSideTurnEnd");
    }

    [HarmonyPostfix]
    [HarmonyPatch("BeforeSideTurnEnd")]
    private static void BeforeSideTurnEndPostfix(
        ICombatState combatState,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        ForecastTurnLifecyclePatch.CommitTurnEndSnapshot(combatState, side, participants);
    }
}
