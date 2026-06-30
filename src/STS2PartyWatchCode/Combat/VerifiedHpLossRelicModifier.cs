using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace STS2PartyWatch.Combat;

internal static class VerifiedHpLossRelicModifier
{
    public static HpLossRelicModificationResult Apply(
        Player player,
        IReadOnlyList<UpcomingHpLossEvent> events,
        int observedDamageReceivedThisTurn)
    {
        try
        {
            var relics = player.Relics
                .Where(relic => !relic.IsMelted && (relic is TungstenRod || relic is BeatingRemnant))
                .ToList();

            if (relics.Count == 0)
            {
                return HpLossRelicModificationResult.Supported(
                    events.Where(e => e.DisplayLane == HpLossDisplayLane.Blockable).Sum(e => Math.Max(0, e.VerifiedHpLoss)),
                    events.Where(e => e.DisplayLane == HpLossDisplayLane.DirectHpLoss).Sum(e => Math.Max(0, e.VerifiedHpLoss)));
            }

            decimal predictedDamageReceivedThisTurn = Math.Max(0, observedDamageReceivedThisTurn);
            var blockable = 0;
            var direct = 0;

            foreach (var hpLossEvent in events.OrderBy(e => e.NativeExecutionOrder))
            {
                decimal amount = Math.Max(0, hpLossEvent.VerifiedHpLoss);

                foreach (var relic in relics)
                {
                    if (relic is TungstenRod tungstenRod)
                    {
                        if (!hpLossEvent.IsSingleVerifiedEvent && amount > 0)
                        {
                            return HpLossRelicModificationResult.Unsupported(
                                HpLossRelicModificationState.UnsupportedBecauseAggregateEnemyHpLossWithTungstenRod,
                                blockable,
                                direct);
                        }

                        amount = Math.Max(decimal.Zero, amount - ReadDynamicVar(tungstenRod, "HpLossReduction", decimal.One));
                    }
                    else if (relic is BeatingRemnant beatingRemnant)
                    {
                        amount = Math.Min(amount, ReadDynamicVar(beatingRemnant, "MaxHpLoss", 20m) - predictedDamageReceivedThisTurn);
                    }
                }

                if (amount < decimal.Zero)
                {
                    return HpLossRelicModificationResult.Unsupported(
                        HpLossRelicModificationState.UnsupportedBecauseInvalidRemainingBudget,
                        blockable,
                        direct);
                }

                var finalHpLoss = (int)decimal.Truncate(amount);
                predictedDamageReceivedThisTurn += finalHpLoss;

                if (hpLossEvent.DisplayLane == HpLossDisplayLane.Blockable)
                {
                    blockable += finalHpLoss;
                }
                else
                {
                    direct += finalHpLoss;
                }
            }

            return HpLossRelicModificationResult.Supported(blockable, direct);
        }
        catch
        {
            return HpLossRelicModificationResult.Unsupported(
                HpLossRelicModificationState.UnsupportedBecauseEventGranularityUnknown,
                0,
                0);
        }
    }

    private static decimal ReadDynamicVar(RelicModel relic, string key, decimal fallback)
    {
        try
        {
            return relic.DynamicVars[key].BaseValue;
        }
        catch
        {
            return fallback;
        }
    }
}

internal readonly record struct HpLossRelicModificationResult(
    HpLossRelicModificationState State,
    int BlockableHpLoss,
    int DirectHpLoss)
{
    public static HpLossRelicModificationResult Supported(int blockableHpLoss, int directHpLoss) =>
        new(HpLossRelicModificationState.Supported, Math.Max(0, blockableHpLoss), Math.Max(0, directHpLoss));

    public static HpLossRelicModificationResult Unsupported(
        HpLossRelicModificationState state,
        int blockableHpLoss,
        int directHpLoss) =>
        new(state, Math.Max(0, blockableHpLoss), Math.Max(0, directHpLoss));
}

internal enum HpLossRelicModificationState
{
    Supported,
    UnsupportedBecauseAggregateEnemyHpLossWithTungstenRod,
    UnsupportedBecauseEventGranularityUnknown,
    UnsupportedBecauseEventOrderUnknown,
    UnsupportedBecauseInvalidRemainingBudget
}
