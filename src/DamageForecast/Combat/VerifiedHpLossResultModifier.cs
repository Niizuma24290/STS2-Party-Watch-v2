using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;

namespace DamageForecast.Combat;

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
            var hasIntangible = includePowerModifiers
                && localCreature.GetPower<IntangiblePower>()?.Amount > 0;
            var relicModifiers = includeRelicModifiers
                ? player.Relics
                    .Where(relic => !relic.IsMelted && (relic is TungstenRod || relic is BeatingRemnant))
                    .Select(ToPolicyInput)
                    .ToArray()
                : [];

            return ApplyPolicy(
                events,
                new HpLossModifierPolicyInput(
                    hasIntangible,
                    relicModifiers,
                    observedDamageReceivedThisTurn));
        }
        catch
        {
            return HpLossResultModificationResult.Unsupported(
                HpLossResultModificationState.UnsupportedBecauseEventGranularityUnknown,
                0,
                0);
        }
    }

    internal static HpLossResultModificationResult ApplyPolicy(
        IReadOnlyList<UpcomingHpLossEvent> events,
        HpLossModifierPolicyInput input)
    {
        try
        {
            decimal predictedDamageReceivedThisTurn = Math.Max(
                0,
                input.ObservedDamageReceivedThisTurn);
            var blockable = 0;
            var direct = 0;
            var relicModifiers = (input.RelicModifiers
                    ?? Array.Empty<HpLossRelicModifierInput>())
                .Select((modifier, index) => (Modifier: modifier, Index: index))
                .OrderBy(entry => ModifierOrder(entry.Modifier.Kind))
                .ThenBy(entry => entry.Index)
                .Select(entry => entry.Modifier)
                .ToArray();

            foreach (var entry in events
                         .Select((hpLossEvent, index) => (Event: hpLossEvent, Index: index))
                         .OrderBy(entry => entry.Event.NativeExecutionOrder)
                         .ThenBy(entry => entry.Index))
            {
                var hpLossEvent = entry.Event;
                decimal amount = Math.Max(0, hpLossEvent.VerifiedHpLoss);

                if (input.HasIntangible
                    && hpLossEvent.DisplayLane == HpLossDisplayLane.DirectHpLoss
                    && amount > decimal.Zero)
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

                foreach (var modifier in relicModifiers)
                {
                    switch (modifier.Kind)
                    {
                        case HpLossRelicModifierKind.TungstenRod:
                            if (!hpLossEvent.IsSingleVerifiedEvent && amount > 0)
                            {
                                return HpLossResultModificationResult.Unsupported(
                                    HpLossResultModificationState.UnsupportedBecauseAggregateEnemyHpLossWithTungstenRod,
                                    blockable,
                                    direct);
                            }

                            amount = Math.Max(decimal.Zero, amount - modifier.Value);
                            break;
                        case HpLossRelicModifierKind.BeatingRemnant:
                            amount = Math.Min(
                                amount,
                                modifier.Value - predictedDamageReceivedThisTurn);
                            break;
                        default:
                            return HpLossResultModificationResult.Unsupported(
                                HpLossResultModificationState.UnsupportedBecauseEventGranularityUnknown,
                                blockable,
                                direct);
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

    private static HpLossRelicModifierInput ToPolicyInput(RelicModel relic)
    {
        return relic switch
        {
            TungstenRod tungstenRod => new HpLossRelicModifierInput(
                HpLossRelicModifierKind.TungstenRod,
                ReadDynamicVar(tungstenRod, "HpLossReduction", decimal.One)),
            BeatingRemnant beatingRemnant => new HpLossRelicModifierInput(
                HpLossRelicModifierKind.BeatingRemnant,
                ReadDynamicVar(beatingRemnant, "MaxHpLoss", 20m)),
            _ => throw new InvalidOperationException($"Unsupported HP-loss relic: {relic.GetType().FullName}")
        };
    }

    private static int ModifierOrder(HpLossRelicModifierKind kind)
    {
        return kind switch
        {
            HpLossRelicModifierKind.TungstenRod => 0,
            HpLossRelicModifierKind.BeatingRemnant => 1,
            _ => int.MaxValue
        };
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

internal readonly record struct HpLossModifierPolicyInput(
    bool HasIntangible,
    IReadOnlyList<HpLossRelicModifierInput>? RelicModifiers,
    int ObservedDamageReceivedThisTurn);

internal readonly record struct HpLossRelicModifierInput(
    HpLossRelicModifierKind Kind,
    decimal Value);

internal enum HpLossRelicModifierKind
{
    TungstenRod,
    BeatingRemnant
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
