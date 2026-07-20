using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using STS2PartyWatch.Forecast;

namespace STS2PartyWatch.UI;

internal static class PartyWatchHudSnapshotStore
{
    private static Player? _player;
    private static Creature? _creature;
    private static ForecastHudSnapshot? _snapshot;
    private static ForecastHudSnapshot? _latestLiveResult;
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

    public static void OnPlayerTurnEnding(Player player, Creature creature, ForecastHudSnapshot latest)
    {
        if (ReferenceEquals(_player, player)
            || ReferenceEquals(_creature, creature)
            || _player is null
            || _creature is null)
        {
            _player = player;
            _creature = creature;
            _latestLiveResult = HasDisplayableDamage(latest) ? latest : null;
            _snapshot = _latestLiveResult;
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

    public static bool TryGetCommitted(Creature creature, out ForecastHudSnapshot result)
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

        result = ForecastHudSnapshot.Hidden;
        return false;
    }

    public static ForecastHudSnapshot ResolveDisplayResult(Creature creature, ForecastHudSnapshot latest)
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
                : ForecastHudSnapshot.Hidden;
        }

        _latestLiveResult = HasDisplayableDamage(latest)
            ? latest
            : null;

        if (!HasDisplayableDamage(latest))
        {
            return ForecastHudSnapshot.Hidden;
        }

        return latest;
    }

    private static bool HasDisplayableDamage(ForecastHudSnapshot result)
    {
        return PartyWatchHudDisplay.HasDisplayableSnapshot(result);
    }
}
