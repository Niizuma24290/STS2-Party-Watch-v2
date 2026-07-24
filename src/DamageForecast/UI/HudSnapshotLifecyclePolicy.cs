using DamageForecast.Forecast;

namespace DamageForecast.UI;

internal static class HudSnapshotLifecyclePolicy
{
    public static HudSnapshotLifecycleState StartPlayerTurn(HudSnapshotOwnerIdentity owner) =>
        HudSnapshotLifecycleState.Empty with { Owner = owner };

    public static HudSnapshotLifecycleState CommitLatest(
        HudSnapshotLifecycleState state,
        HudSnapshotOwnerIdentity owner)
    {
        state = ForOwner(state, owner);
        return state with
        {
            CommittedSnapshot = state.LatestLiveSnapshot,
            HasPlayerEndedTurn = true
        };
    }

    public static HudSnapshotLifecycleState Commit(
        HudSnapshotLifecycleState state,
        HudSnapshotOwnerIdentity owner,
        ForecastHudSnapshot latest,
        bool isDisplayable)
    {
        state = ForOwner(state, owner);
        ForecastHudSnapshot? accepted = isDisplayable ? latest : null;
        return state with
        {
            LatestLiveSnapshot = accepted,
            CommittedSnapshot = accepted,
            HasPlayerEndedTurn = true
        };
    }

    public static HudSnapshotResolution ResolveDisplay(
        HudSnapshotLifecycleState state,
        HudSnapshotOwnerIdentity owner,
        ForecastHudSnapshot latest,
        bool isDisplayable,
        bool freezeEnabled)
    {
        if (!freezeEnabled)
        {
            return new(StartPlayerTurn(owner), latest);
        }

        state = ForOwner(state, owner);
        if (state.HasPlayerEndedTurn)
        {
            return new(state, state.CommittedSnapshot ?? ForecastHudSnapshot.Hidden);
        }

        state = state with { LatestLiveSnapshot = isDisplayable ? latest : null };
        return new(state, isDisplayable ? latest : ForecastHudSnapshot.Hidden);
    }

    public static bool TryGetCommitted(
        HudSnapshotLifecycleState state,
        HudSnapshotOwnerIdentity owner,
        bool freezeEnabled,
        out ForecastHudSnapshot snapshot)
    {
        if (freezeEnabled
            && state.Owner == owner
            && state.HasPlayerEndedTurn
            && state.CommittedSnapshot is { } committed)
        {
            snapshot = committed;
            return true;
        }

        snapshot = ForecastHudSnapshot.Hidden;
        return false;
    }

    public static HudSnapshotLifecycleState OnVisibilityEvent(
        HudSnapshotLifecycleState state,
        HudVisibilityLifecycleEvent visibilityEvent) =>
        visibilityEvent == HudVisibilityLifecycleEvent.TemporarilyCovered
            ? state
            : HudSnapshotLifecycleState.Empty;

    private static HudSnapshotLifecycleState ForOwner(
        HudSnapshotLifecycleState state,
        HudSnapshotOwnerIdentity owner) =>
        state.Owner == owner ? state : StartPlayerTurn(owner);
}

internal readonly record struct HudSnapshotOwnerIdentity(
    ulong PlayerNetId,
    string CreatureStableIdentity);

internal readonly record struct HudSnapshotLifecycleState(
    HudSnapshotOwnerIdentity? Owner,
    ForecastHudSnapshot? LatestLiveSnapshot,
    ForecastHudSnapshot? CommittedSnapshot,
    bool HasPlayerEndedTurn)
{
    public static HudSnapshotLifecycleState Empty => new(null, null, null, false);
}

internal readonly record struct HudSnapshotResolution(
    HudSnapshotLifecycleState State,
    ForecastHudSnapshot DisplaySnapshot);

internal enum HudVisibilityLifecycleEvent
{
    TemporarilyCovered,
    PermanentlyInvalidated
}
