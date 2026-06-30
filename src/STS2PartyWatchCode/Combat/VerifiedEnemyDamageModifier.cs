using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;

namespace STS2PartyWatch.Combat;

internal static class VerifiedEnemyDamageModifier
{
    public static EnemyDamageModificationResult ApplyDiamondDiadem(
        Player player,
        Creature localCreature,
        AttackIntent attackIntent,
        Creature enemy,
        int nativeAmount,
        bool isSingleVerifiedEvent)
    {
        try
        {
            var state = ReadDiamondDiademState(player, localCreature);
            if (state == DiamondDiademForecastState.NotActiveOrAlreadyNative)
            {
                return EnemyDamageModificationResult.Supported(nativeAmount);
            }

            if (state == DiamondDiademForecastState.Unknown)
            {
                return EnemyDamageModificationResult.Unsupported(
                    EnemyDamageModificationState.UnsupportedBecauseDiamondDiademStateUnknown);
            }

            if (!isSingleVerifiedEvent && nativeAmount > 0)
            {
                return EnemyDamageModificationResult.Unsupported(
                    EnemyDamageModificationState.UnsupportedBecauseAggregateEnemyDamageWithPerHitRounding);
            }

            return EnemyDamageModificationResult.Supported(
                ReadFutureDiamondDiademSingleDamage(player, localCreature, attackIntent, enemy));
        }
        catch
        {
            return EnemyDamageModificationResult.Unsupported(
                EnemyDamageModificationState.UnsupportedBecauseDiamondDiademStateUnknown);
        }
    }

    private static DiamondDiademForecastState ReadDiamondDiademState(Player player, Creature localCreature)
    {
        if (localCreature.GetPower<DiamondDiademPower>() is not null)
        {
            return DiamondDiademForecastState.NotActiveOrAlreadyNative;
        }

        var relic = player.Relics.OfType<DiamondDiadem>().FirstOrDefault(relic => !relic.IsMelted);
        if (relic is null)
        {
            return DiamondDiademForecastState.NotActiveOrAlreadyNative;
        }

        if (!TryReadCardThreshold(relic, out var threshold))
        {
            return DiamondDiademForecastState.Unknown;
        }

        var cardsPlayed = relic.CardsPlayedThisTurn;
        if (cardsPlayed == threshold && HasPendingStampedeAttack(player, localCreature))
        {
            cardsPlayed++;
        }

        return cardsPlayed <= threshold
            ? DiamondDiademForecastState.ShouldApplyFuturePower
            : DiamondDiademForecastState.NotActiveOrAlreadyNative;
    }

    private static bool TryReadCardThreshold(DiamondDiadem relic, out int threshold)
    {
        threshold = 0;
        try
        {
            threshold = (int)relic.DynamicVars["CardThreshold"].BaseValue;
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool HasPendingStampedeAttack(Player player, Creature localCreature)
    {
        var stampede = localCreature.GetPower<StampedePower>();
        if (stampede is null || stampede.Amount <= 0)
        {
            return false;
        }

        var handPile = CardPile.Get(PileType.Hand, player);
        return handPile is not null && handPile.Cards.Any(card => card.Owner == player && card.Type == CardType.Attack);
    }

    private static int ReadFutureDiamondDiademSingleDamage(
        Player player,
        Creature localCreature,
        AttackIntent attackIntent,
        Creature enemy)
    {
        var damageCalc = attackIntent.DamageCalc;
        if (damageCalc is null)
        {
            throw new InvalidOperationException("AttackIntent.DamageCalc is not readable.");
        }

        var baseDamage = damageCalc();
        var props = DamageProps.monsterMove;
        var additiveAndMultiplicative = ModifyDamageHookType.Additive | ModifyDamageHookType.Multiplicative;

        var beforeCap = Hook.ModifyDamage(
            player.RunState,
            localCreature.CombatState!,
            localCreature,
            enemy,
            baseDamage,
            props,
            null,
            additiveAndMultiplicative,
            CardPreviewMode.None,
            out _);

        var afterDiamondDiadem = beforeCap * 0.5m;
        var afterCap = Hook.ModifyDamage(
            player.RunState,
            localCreature.CombatState!,
            localCreature,
            enemy,
            afterDiamondDiadem,
            props,
            null,
            ModifyDamageHookType.Cap,
            CardPreviewMode.None,
            out _);

        return Math.Max(0, (int)afterCap);
    }

    private enum DiamondDiademForecastState
    {
        NotActiveOrAlreadyNative,
        ShouldApplyFuturePower,
        Unknown
    }
}

internal readonly record struct EnemyDamageModificationResult(
    EnemyDamageModificationState State,
    int Amount)
{
    public static EnemyDamageModificationResult Supported(int amount) =>
        new(EnemyDamageModificationState.Supported, Math.Max(0, amount));

    public static EnemyDamageModificationResult Unsupported(EnemyDamageModificationState state) =>
        new(state, 0);
}

internal enum EnemyDamageModificationState
{
    Supported,
    UnsupportedBecauseAggregateEnemyDamageWithPerHitRounding,
    UnsupportedBecauseDiamondDiademStateUnknown
}
