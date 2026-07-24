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
using DamageForecast.Combat;
using DamageForecast.Diagnostics;
using DamageForecast.Forecast;
using DamageForecast.UI;

namespace DamageForecast.Patches;

[HarmonyPatch(typeof(NHealthBar))]
internal static class ForecastRefreshPatch
{
    internal const string MainLabelName = "DamageForecastExpectedLossLabel";
    internal const string IncomingLabelName = "DamageForecastIncomingDamageLabel";
    internal const string DetailLabelName = "DamageForecastDetailsLabel";
    private const string MainLabelOwnershipGroup = "damage-forecast-hud-expected-loss";
    private const string IncomingLabelOwnershipGroup = "damage-forecast-hud-incoming-damage";
    private const string DetailLabelOwnershipGroup = "damage-forecast-hud-details";
    private static readonly FieldInfo? CreatureField = typeof(NHealthBar).GetField("_creature", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly LocalIncomingDamageReader Reader = new();
    private static readonly LocalDamageForecast Forecast = new();
    private static readonly List<WeakReference<NHealthBar>> RegisteredBars = new();
#if DAMAGE_FORECAST_VISIBILITY_PROFILING
    [ThreadStatic]
    private static int _lastRegisteredBarsVisited;

    internal static int LastRegisteredBarsVisitedForVisibilityProfiling => _lastRegisteredBarsVisited;
#endif

    static ForecastRefreshPatch()
    {
        DamageForecastUiSettings.Changed += RefreshRegisteredBars;
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

        DamageForecastHudDisplay.ApplyMainHudStyle(mainLabel);
        DamageForecastHudDisplay.ApplyIncomingHudStyle(incomingLabel);
        DamageForecastHudDisplay.ApplyDetailHudStyle(detailLabel);
        ObservedHpLossBudgetTracker.Observe(creature);

        if (!DamageForecastHudVisibilityPolicy.ShouldRenderHud(bar, creature, out var temporarilyCovered))
        {
            DamageForecastHudSnapshotStore.OnVisibilityHidden(temporarilyCovered);

            Hide(mainLabel, incomingLabel, detailLabel);
            return;
        }

        var snapshot = DamageForecastHudSnapshotStore.TryGetCommitted(creature, out var committed)
            ? committed
            : DamageForecastHudSnapshotStore.ResolveDisplayResult(
                creature,
                BuildForecastHudSnapshot(creature));
        if (!DamageForecastHudDisplay.HasDisplayableSnapshot(snapshot))
        {
            Hide(mainLabel, incomingLabel, detailLabel);
            return;
        }

        mainLabel.Text = DamageForecastHudDisplay.ShouldShowExpectedHpLoss(snapshot)
            ? DamageForecastHudDisplay.BuildMainHudDisplay(snapshot.ExpectedHpLoss)
            : string.Empty;
        incomingLabel.Text = DamageForecastHudDisplay.ShouldShowIncomingDamage(snapshot)
            ? DamageForecastHudDisplay.BuildIncomingHudDisplay(snapshot.IncomingDamage)
            : string.Empty;
        var details = DamageForecastHudDisplay.BuildHudDetails(snapshot.ExpectedHpLoss);
        DamageForecastHudDisplay.ApplyMainHudTextBounds(mainLabel);
        DamageForecastHudDisplay.ApplyHudTextBounds(incomingLabel);
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
            DamageForecastUiSettings.IncludeCurrentBlockInIncomingDamage,
            DamageForecastUiSettings.IncludePowerBlockInIncomingDamage,
            DamageForecastUiSettings.IncludeRelicBlockInIncomingDamage,
            DamageForecastUiSettings.IncludePowerHpLossModifiersInIncomingDamage,
            DamageForecastUiSettings.IncludeRelicHpLossModifiersInIncomingDamage));
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

        return ResolveHudNode(
            parent,
            MainLabelName,
            MainLabelOwnershipGroup,
            static () => new Label
            {
                MouseFilter = Control.MouseFilterEnum.Ignore,
                ZIndex = 50,
                Text = string.Empty,
                Visible = false,
            },
            DamageForecastHudDisplay.ApplyMainHudStyle);
    }

    private static Label? GetOrCreateIncomingLabel(NHealthBar bar)
    {
        var parent = GetLabelParent(bar);
        if (parent is null)
        {
            return null;
        }

        return ResolveHudNode(
            parent,
            IncomingLabelName,
            IncomingLabelOwnershipGroup,
            static () => new Label
            {
                MouseFilter = Control.MouseFilterEnum.Ignore,
                ZIndex = 50,
                Text = string.Empty,
                Visible = false,
            },
            DamageForecastHudDisplay.ApplyIncomingHudStyle);
    }

    private static RichTextLabel? GetOrCreateDetailLabel(NHealthBar bar)
    {
        var parent = GetLabelParent(bar);
        if (parent is null)
        {
            return null;
        }

        return ResolveHudNode(
            parent,
            DetailLabelName,
            DetailLabelOwnershipGroup,
            static () => new RichTextLabel
            {
                MouseFilter = Control.MouseFilterEnum.Ignore,
                ZIndex = 50,
                Text = string.Empty,
                Visible = false,
            },
            DamageForecastHudDisplay.ApplyDetailHudStyle);
    }

    private static T? ResolveHudNode<T>(
        Control parent,
        string expectedName,
        string ownershipGroup,
        Func<T>? create,
        Action<T>? applyStyle)
        where T : Control
    {
        var children = parent.GetChildren().OfType<Node>().ToArray();
        var candidates = children.Select(child => new DamageForecastHudNodeCandidate(
            HasExpectedName: string.Equals(child.Name.ToString(), expectedName, StringComparison.Ordinal),
            HasExpectedType: child is T,
            IsOwned: child.IsInGroup(ownershipGroup))).ToArray();
        var resolution = DamageForecastHudNodeOwnershipPolicy.Resolve(candidates);
        if (resolution.FailClosed)
        {
            var conflictingTypes = children
                .Where(child => string.Equals(child.Name.ToString(), expectedName, StringComparison.Ordinal))
                .Select(child => child.GetType().FullName ?? child.GetType().Name);
            DamageForecastDiagnostics.ReportOnce(
                $"hud-node.type-conflict.{expectedName}",
                new InvalidOperationException(
                    $"Expected {typeof(T).FullName} for node '{expectedName}', found {string.Join(", ", conflictingTypes)}."));
            return null;
        }

        T? canonical;
        if (resolution.CreateNew)
        {
            if (create is null)
            {
                return null;
            }

            canonical = create();
            canonical.Name = expectedName;
            canonical.AddToGroup(ownershipGroup);
            applyStyle?.Invoke(canonical);
            parent.AddChild(canonical);
        }
        else
        {
            canonical = children[resolution.CanonicalIndex] as T;
            if (canonical is null)
            {
                return null;
            }

            canonical.Name = expectedName;
            if (!canonical.IsInGroup(ownershipGroup))
            {
                canonical.AddToGroup(ownershipGroup);
            }

            applyStyle?.Invoke(canonical);
        }

        foreach (var duplicateIndex in resolution.DuplicateOwnedIndexes)
        {
            var duplicate = children[duplicateIndex];
            if (GodotObject.IsInstanceValid(duplicate) && !duplicate.IsQueuedForDeletion())
            {
                duplicate.QueueFree();
            }
        }

        return canonical;
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
        DamageForecastHudDisplay.ApplyHudPosition(
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
        if (parent is null)
        {
            return;
        }

        var mainLabel = ResolveHudNode<Label>(
            parent,
            MainLabelName,
            MainLabelOwnershipGroup,
            create: null,
            applyStyle: null);
        var incomingLabel = ResolveHudNode<Label>(
            parent,
            IncomingLabelName,
            IncomingLabelOwnershipGroup,
            create: null,
            applyStyle: null);
        var detailLabel = ResolveHudNode<RichTextLabel>(
            parent,
            DetailLabelName,
            DetailLabelOwnershipGroup,
            create: null,
            applyStyle: null);
        if (mainLabel is not null)
        {
            Hide(mainLabel);
        }

        if (incomingLabel is not null)
        {
            Hide(incomingLabel);
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
#if DAMAGE_FORECAST_VISIBILITY_PROFILING
        var barsVisited = 0;
#endif
        for (var i = RegisteredBars.Count - 1; i >= 0; i--)
        {
            if (!RegisteredBars[i].TryGetTarget(out var bar) || !GodotObject.IsInstanceValid(bar))
            {
                RegisteredBars.RemoveAt(i);
                continue;
            }

#if DAMAGE_FORECAST_VISIBILITY_PROFILING
            barsVisited++;
#endif
            Refresh(bar, null);
        }
#if DAMAGE_FORECAST_VISIBILITY_PROFILING
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
        DamageForecastHudSnapshotStore.OnPlayerTurnEnding(player, creature, snapshot);
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

        DamageForecastHudSnapshotStore.OnPlayerSideTurnStarted(player, creature);
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

            DamageForecastHudSnapshotStore.OnPlayerSideTurnStarted(player, creature);
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
        DamageForecastHudSnapshotStore.Clear();
        ObservedHpLossBudgetTracker.Clear();
        ForecastRefreshPatch.RefreshRegisteredBars();
#if DAMAGE_FORECAST_VISIBILITY_PROFILING
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
