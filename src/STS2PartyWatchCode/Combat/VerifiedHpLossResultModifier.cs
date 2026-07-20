using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;

namespace STS2PartyWatch.Combat;

internal static class VerifiedHpLossResultModifier
{
    public static HpLossResultModificationResult Apply(
        Player player,
        Creature localCreature,
        IReadOnlyList<UpcomingHpLossEvent> events,
        int observedDamageReceivedThisTurn,
        bool includePowerModifiers = true,
        bool includeRelicModifiers = true)
    {
        try
        {
            var hasIntangible = includePowerModifiers && localCreature.GetPower<IntangiblePower>()?.Amount > 0;
            var relics = includeRelicModifiers
                ? player.Relics
                    .Where(relic => !relic.IsMelted && (relic is TungstenRod || relic is BeatingRemnant))
                    .OrderBy(relic => relic is TungstenRod ? 0 : 1)
                    .ToList()
                : [];

            if (!hasIntangible && relics.Count == 0)
            {
                return HpLossResultModificationResult.Supported(
                    events.Where(e => e.DisplayLane == HpLossDisplayLane.Blockable).Sum(e => Math.Max(0, e.VerifiedHpLoss)),
                    events.Where(e => e.DisplayLane == HpLossDisplayLane.DirectHpLoss).Sum(e => Math.Max(0, e.VerifiedHpLoss)));
            }

            decimal predictedDamageReceivedThisTurn = Math.Max(0, observedDamageReceivedThisTurn);
            var blockable = 0;
            var direct = 0;

            foreach (var hpLossEvent in events.OrderBy(e => e.NativeExecutionOrder))
            {
                decimal amount = Math.Max(0, hpLossEvent.VerifiedHpLoss);

                if (hasIntangible && hpLossEvent.DisplayLane == HpLossDisplayLane.DirectHpLoss && amount > decimal.Zero)
                {
                    if (!hpLossEvent.IsSingleVerifiedEvent)
                    {
                        return HpLossResultModificationResult.Unsupported(
                            HpLossResultModificationState.UnsupportedBecauseAggregateDirectHpLossWithIntangible,
                            blockable,
                            direct);
                    }

                    amount = decimal.One;
                }

                foreach (var relic in relics)
                {
                    if (relic is TungstenRod tungstenRod)
                    {
                        if (!hpLossEvent.IsSingleVerifiedEvent && amount > 0)
                        {
                            return HpLossResultModificationResult.Unsupported(
                                HpLossResultModificationState.UnsupportedBecauseAggregateEnemyHpLossWithTungstenRod,
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
                    return HpLossResultModificationResult.Unsupported(
                        HpLossResultModificationState.UnsupportedBecauseInvalidRemainingBudget,
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

            return HpLossResultModificationResult.Supported(blockable, direct);
        }
        catch
        {
            return HpLossResultModificationResult.Unsupported(
                HpLossResultModificationState.UnsupportedBecauseEventGranularityUnknown,
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

internal readonly record struct HpLossResultModificationResult(
    HpLossResultModificationState State,
    int BlockableHpLoss,
    int DirectHpLoss)
{
    public static HpLossResultModificationResult Supported(int blockableHpLoss, int directHpLoss) =>
        new(HpLossResultModificationState.Supported, Math.Max(0, blockableHpLoss), Math.Max(0, directHpLoss));

    public static HpLossResultModificationResult Unsupported(
        HpLossResultModificationState state,
        int blockableHpLoss,
        int directHpLoss) =>
        new(state, Math.Max(0, blockableHpLoss), Math.Max(0, directHpLoss));
}

internal enum HpLossResultModificationState
{
    Supported,
    UnsupportedBecauseAggregateEnemyHpLossWithTungstenRod,
    UnsupportedBecauseAggregateDirectHpLossWithIntangible,
    UnsupportedBecauseEventGranularityUnknown,
    UnsupportedBecauseEventOrderUnknown,
    UnsupportedBecauseInvalidRemainingBudget
}
