using DamageForecast.Combat;
using DamageForecast.Forecast;
using DamageForecast.UI;

internal static class LifecycleContractCases
{
    private static readonly HudSnapshotOwnerIdentity OwnerA = new(1, "creature-a");
    private static readonly HudSnapshotOwnerIdentity OwnerB = new(1, "creature-b");
    private static readonly HudSnapshotOwnerIdentity PlayerB = new(2, "creature-a");

    public static IEnumerable<ContractCase> Create()
    {
        yield return new(
            "LC-001",
            "Lifecycle",
            "Lifecycle.PlayerTurnStart_ClearsLiveAndCommittedSnapshots",
            assert =>
            {
                var state = HudSnapshotLifecyclePolicy.Commit(
                    HudSnapshotLifecycleState.Empty,
                    OwnerA,
                    Snapshot(4),
                    isDisplayable: true);
                state = HudSnapshotLifecyclePolicy.StartPlayerTurn(OwnerA);
                assert.True(
                    state.Owner == OwnerA
                    && state.LatestLiveSnapshot is null
                    && state.CommittedSnapshot is null
                    && !state.HasPlayerEndedTurn,
                    "owner=A; live=null; committed=null; ended=false",
                    state.ToString());
            });
        yield return new(
            "LC-002",
            "Lifecycle",
            "Lifecycle.LiveResult_UpdatesLatestAndDisplay",
            assert =>
            {
                var latest = Snapshot(5);
                var resolved = HudSnapshotLifecyclePolicy.ResolveDisplay(
                    HudSnapshotLifecyclePolicy.StartPlayerTurn(OwnerA),
                    OwnerA,
                    latest,
                    isDisplayable: true,
                    freezeEnabled: true);
                assert.True(
                    resolved.State.LatestLiveSnapshot == latest
                    && resolved.DisplaySnapshot == latest,
                    "latestLive=5; display=5",
                    resolved.ToString());
            });
        yield return new(
            "LC-003",
            "Lifecycle",
            "Lifecycle.TurnEnding_CommitsFinalSnapshot",
            assert =>
            {
                var final = Snapshot(7);
                var state = HudSnapshotLifecyclePolicy.Commit(
                    HudSnapshotLifecyclePolicy.StartPlayerTurn(OwnerA),
                    OwnerA,
                    final,
                    isDisplayable: true);
                var found = HudSnapshotLifecyclePolicy.TryGetCommitted(
                    state,
                    OwnerA,
                    freezeEnabled: true,
                    out var committed);
                assert.True(
                    found && committed == final && state.HasPlayerEndedTurn,
                    "found=true; committed=7; ended=true",
                    $"found={found}; committed={committed}; ended={state.HasPlayerEndedTurn}");
            });
        yield return new(
            "LC-004",
            "Lifecycle",
            "Lifecycle.TurnEndingWithoutExplicitResult_CommitsLatestLive",
            assert =>
            {
                var latest = Snapshot(6);
                var live = HudSnapshotLifecyclePolicy.ResolveDisplay(
                    HudSnapshotLifecycleState.Empty,
                    OwnerA,
                    latest,
                    isDisplayable: true,
                    freezeEnabled: true).State;
                var committed = HudSnapshotLifecyclePolicy.CommitLatest(live, OwnerA);
                assert.True(
                    committed.CommittedSnapshot == latest && committed.HasPlayerEndedTurn,
                    "committed=6; ended=true",
                    committed.ToString());
            });
        yield return new(
            "LC-005",
            "Lifecycle",
            "Lifecycle.FreezeDisabled_DisplaysLatestAndDropsFrozenState",
            assert =>
            {
                var committed = HudSnapshotLifecyclePolicy.Commit(
                    HudSnapshotLifecycleState.Empty,
                    OwnerA,
                    Snapshot(9),
                    isDisplayable: true);
                var latest = Snapshot(3);
                var resolved = HudSnapshotLifecyclePolicy.ResolveDisplay(
                    committed,
                    OwnerA,
                    latest,
                    isDisplayable: true,
                    freezeEnabled: false);
                assert.True(
                    resolved.DisplaySnapshot == latest
                    && resolved.State.CommittedSnapshot is null
                    && !resolved.State.HasPlayerEndedTurn,
                    "display=3; committed=null; ended=false",
                    resolved.ToString());
            });
        yield return new(
            "LC-006",
            "Lifecycle",
            "Lifecycle.CoveringScreenHide_PreservesCommittedSnapshot",
            assert =>
            {
                var state = HudSnapshotLifecyclePolicy.Commit(
                    HudSnapshotLifecycleState.Empty,
                    OwnerA,
                    Snapshot(8),
                    isDisplayable: true);
                var hidden = HudSnapshotLifecyclePolicy.OnVisibilityEvent(
                    state,
                    HudVisibilityLifecycleEvent.TemporarilyCovered);
                assert.Equal(state, hidden);
            });
        yield return new(
            "LC-007",
            "Lifecycle",
            "Lifecycle.CoveringScreenClose_RestoresSameCommittedSnapshot",
            assert =>
            {
                var expected = Snapshot(8);
                var state = HudSnapshotLifecyclePolicy.Commit(
                    HudSnapshotLifecycleState.Empty,
                    OwnerA,
                    expected,
                    isDisplayable: true);
                state = HudSnapshotLifecyclePolicy.OnVisibilityEvent(
                    state,
                    HudVisibilityLifecycleEvent.TemporarilyCovered);
                var found = HudSnapshotLifecyclePolicy.TryGetCommitted(
                    state,
                    OwnerA,
                    freezeEnabled: true,
                    out var restored);
                assert.True(
                    found && restored == expected,
                    "found=true; restored=8",
                    $"found={found}; restored={restored}",
                    "AUD-0005 temporary cover preserves and restores the frozen snapshot");
            });
        yield return new(
            "LC-008",
            "Lifecycle",
            "Lifecycle.PermanentInvalidation_ClearsSnapshotState",
            assert =>
            {
                var state = HudSnapshotLifecyclePolicy.Commit(
                    HudSnapshotLifecycleState.Empty,
                    OwnerA,
                    Snapshot(8),
                    isDisplayable: true);
                var cleared = HudSnapshotLifecyclePolicy.OnVisibilityEvent(
                    state,
                    HudVisibilityLifecycleEvent.PermanentlyInvalidated);
                assert.Equal(HudSnapshotLifecycleState.Empty, cleared);
            });
        yield return new(
            "LC-009",
            "Lifecycle",
            "Lifecycle.CreatureIdentitySwitch_DoesNotReuseCommittedSnapshot",
            assert => AssertIdentitySwitch(assert, OwnerB));
        yield return new(
            "LC-010",
            "Lifecycle",
            "Lifecycle.PlayerIdentitySwitch_DoesNotReuseCommittedSnapshot",
            assert => AssertIdentitySwitch(assert, PlayerB));
        yield return new(
            "LC-011",
            "Lifecycle",
            "Lifecycle.HiddenLiveResult_IsNotStoredOrDisplayed",
            assert =>
            {
                var resolved = HudSnapshotLifecyclePolicy.ResolveDisplay(
                    HudSnapshotLifecyclePolicy.StartPlayerTurn(OwnerA),
                    OwnerA,
                    ForecastHudSnapshot.Hidden,
                    isDisplayable: false,
                    freezeEnabled: true);
                assert.True(
                    resolved.State.LatestLiveSnapshot is null
                    && resolved.DisplaySnapshot == ForecastHudSnapshot.Hidden,
                    "latestLive=null; display=Hidden",
                    resolved.ToString());
            });
        yield return new(
            "LC-012",
            "Lifecycle",
            "Lifecycle.HiddenFinalResult_IsNotCommitted",
            assert =>
            {
                var state = HudSnapshotLifecyclePolicy.Commit(
                    HudSnapshotLifecycleState.Empty,
                    OwnerA,
                    ForecastHudSnapshot.Hidden,
                    isDisplayable: false);
                var found = HudSnapshotLifecyclePolicy.TryGetCommitted(
                    state,
                    OwnerA,
                    freezeEnabled: true,
                    out _);
                assert.Equal(false, found);
            });
        yield return new(
            "LC-013",
            "Lifecycle",
            "Lifecycle.HpLossBudget_AccumulatesObservedLoss",
            assert =>
            {
                var state = HpLossBudgetPolicy.Observe(null, 20);
                state = HpLossBudgetPolicy.Observe(state, 17);
                state = HpLossBudgetPolicy.Observe(state, 12);
                assert.Equal(8, HpLossBudgetPolicy.GetSpent(state));
            });
        yield return new(
            "LC-014",
            "Lifecycle",
            "Lifecycle.HpLossBudget_HealingDoesNotReduceSpent",
            assert =>
            {
                var state = HpLossBudgetPolicy.Observe(null, 20);
                state = HpLossBudgetPolicy.Observe(state, 15);
                state = HpLossBudgetPolicy.Observe(state, 18);
                assert.Equal(5, HpLossBudgetPolicy.GetSpent(state));
            });
        yield return new(
            "LC-015",
            "Lifecycle",
            "Lifecycle.HpLossBudget_ResetStartsNewWindow",
            assert =>
            {
                var reset = HpLossBudgetPolicy.Reset(14);
                assert.True(
                    reset.LastObservedHp == 14 && reset.SpentThisWindow == 0,
                    "lastHp=14; spent=0",
                    reset.ToString());
            });
        yield return new(
            "LC-016",
            "Lifecycle",
            "Lifecycle.CombatEnd_ClearsStaticHpLossBudgets",
            assert =>
            {
                ObservedHpLossBudgetTracker.Clear();
                try
                {
                    ObservedHpLossBudgetTracker.ObserveIdentity(11, 20);
                    ObservedHpLossBudgetTracker.ObserveIdentity(11, 15);
                    assert.Equal(5, ObservedHpLossBudgetTracker.GetSpentForIdentity(11));
                    ObservedHpLossBudgetTracker.Clear();
                    assert.Equal(0, ObservedHpLossBudgetTracker.GetSpentForIdentity(11));
                }
                finally
                {
                    ObservedHpLossBudgetTracker.Clear();
                }
            });
        yield return new(
            "LC-017",
            "Lifecycle",
            "Lifecycle.ConsecutiveCombats_DoNotReuseOldHpLossBudget",
            assert =>
            {
                ObservedHpLossBudgetTracker.Clear();
                try
                {
                    ObservedHpLossBudgetTracker.ObserveIdentity(11, 20);
                    ObservedHpLossBudgetTracker.ObserveIdentity(11, 15);
                    ObservedHpLossBudgetTracker.Clear();
                    ObservedHpLossBudgetTracker.ObserveIdentity(11, 30);
                    ObservedHpLossBudgetTracker.ObserveIdentity(11, 28);
                    assert.Equal(
                        2,
                        ObservedHpLossBudgetTracker.GetSpentForIdentity(11),
                        "AUD-0013 next combat starts from its own baseline");
                }
                finally
                {
                    ObservedHpLossBudgetTracker.Clear();
                }
            });
        yield return new(
            "LC-018",
            "Lifecycle",
            "Lifecycle.HpLossBudgets_AreOwnedPerPlayerIdentity",
            assert =>
            {
                ObservedHpLossBudgetTracker.Clear();
                try
                {
                    ObservedHpLossBudgetTracker.ObserveIdentity(11, 20);
                    ObservedHpLossBudgetTracker.ObserveIdentity(22, 30);
                    ObservedHpLossBudgetTracker.ObserveIdentity(11, 16);
                    ObservedHpLossBudgetTracker.ObserveIdentity(22, 29);
                    assert.True(
                        ObservedHpLossBudgetTracker.GetSpentForIdentity(11) == 4
                        && ObservedHpLossBudgetTracker.GetSpentForIdentity(22) == 1,
                        "player11=4; player22=1",
                        $"player11={ObservedHpLossBudgetTracker.GetSpentForIdentity(11)}; player22={ObservedHpLossBudgetTracker.GetSpentForIdentity(22)}");
                }
                finally
                {
                    ObservedHpLossBudgetTracker.Clear();
                }
            });
        yield return new(
            "LC-019",
            "Lifecycle",
            "Lifecycle.UnknownBudgetIdentity_DoesNotInventDebugInjectionHistory",
            assert =>
            {
                ObservedHpLossBudgetTracker.Clear();
                try
                {
                    assert.Equal(
                        0,
                        ObservedHpLossBudgetTracker.GetSpentForIdentity(99),
                        "mid-combat debug relic injection remains a non-production boundary");
                }
                finally
                {
                    ObservedHpLossBudgetTracker.Clear();
                }
            });
    }

    private static void AssertIdentitySwitch(
        ContractAssert assert,
        HudSnapshotOwnerIdentity nextOwner)
    {
        var old = HudSnapshotLifecyclePolicy.Commit(
            HudSnapshotLifecycleState.Empty,
            OwnerA,
            Snapshot(9),
            isDisplayable: true);
        var latest = Snapshot(2);
        var switched = HudSnapshotLifecyclePolicy.ResolveDisplay(
            old,
            nextOwner,
            latest,
            isDisplayable: true,
            freezeEnabled: true);
        assert.True(
            switched.State.Owner == nextOwner
            && switched.State.CommittedSnapshot is null
            && switched.DisplaySnapshot == latest,
            "owner=next; committed=null; display=2",
            switched.ToString());
    }

    private static ForecastHudSnapshot Snapshot(int damage) =>
        new(ForecastResult.KnownDamage(damage, 0), IncomingDamageDisplayRead.Hidden);
}
