using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using STS2PartyWatch.Forecast;

namespace STS2PartyWatch.UI;

internal static class PartyWatchHudSnapshotStore
{
    private static Player? _player;
    private static Creature? _creature;
    private static ForecastResult? _snapshot;
    private static ForecastResult? _latestLiveResult;
    private static bool _hasPlayerEndedTurn;

    public static void OnPlayerSideTurnStarted(Player player, Creature creature)
    {
        _player = player;
        _creature = creature;
        _snapshot = null;
        _latestLiveResult = null;
        _hasPlayerEndedTurn = false;
    }

    public static void OnPlayerTurnEnding(Player player, Creature creature)
    {
        if (ReferenceEquals(_player, player) || ReferenceEquals(_creature, creature))
        {
            _snapshot = _latestLiveResult is { } latest && HasDisplayableDamage(latest)
                ? latest
                : null;
            _hasPlayerEndedTurn = true;
        }
    }

    public static void Clear()
    {
        _player = null;
        _creature = null;
        _snapshot = null;
        _latestLiveResult = null;
        _hasPlayerEndedTurn = false;
    }

    public static bool TryGetCommitted(Creature creature, out ForecastResult result)
    {
        if (PartyWatchUiSettings.FreezeHudWithinPlayerTurn
            && ReferenceEquals(_creature, creature)
            && _hasPlayerEndedTurn
            && _snapshot is { } snapshot
            && HasDisplayableDamage(snapshot))
        {
            result = snapshot;
            return true;
        }

        result = ForecastResult.Hidden;
        return false;
    }

    public static ForecastResult ResolveDisplayResult(Creature creature, ForecastResult latest)
    {
        if (!PartyWatchUiSettings.FreezeHudWithinPlayerTurn)
        {
            _player = creature.Player;
            _creature = creature;
            _snapshot = null;
            _latestLiveResult = null;
            _hasPlayerEndedTurn = false;
            return latest;
        }

        if (!ReferenceEquals(_creature, creature))
        {
            _player = creature.Player;
            _creature = creature;
            _snapshot = null;
            _latestLiveResult = null;
            _hasPlayerEndedTurn = false;
        }

        if (_hasPlayerEndedTurn)
        {
            return _snapshot is { } snapshot && HasDisplayableDamage(snapshot)
                ? snapshot
                : ForecastResult.Hidden;
        }

        _latestLiveResult = HasDisplayableDamage(latest)
            ? latest
            : null;

        if (!HasDisplayableDamage(latest))
        {
            return ForecastResult.Hidden;
        }

        return latest;
    }

    private static bool HasDisplayableDamage(ForecastResult result)
    {
        return result.State == ForecastResultState.KnownDamage
            && (result.OutDamage > 0 || result.DirectHpLoss > 0);
    }
}
