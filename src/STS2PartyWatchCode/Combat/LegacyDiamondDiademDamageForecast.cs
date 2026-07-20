using System.Reflection;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;
using STS2PartyWatch.Diagnostics;

namespace STS2PartyWatch.Combat;

internal static class LegacyDiamondDiademDamageForecast
{
    private const string DiamondDiademTypeName = "MegaCrit.Sts2.Core.Models.Relics.DiamondDiadem";
    private const string DiamondDiademPowerTypeName = "MegaCrit.Sts2.Core.Models.Powers.DiamondDiademPower";
    private const string StampedePowerTypeName = "MegaCrit.Sts2.Core.Models.Powers.StampedePower";

    public static EnemyDamageModificationResult Apply(
        Player player,
        Creature localCreature,
        AttackIntent attackIntent,
        Creature enemy,
        int nativeAmount,
        bool isSingleVerifiedEvent)
    {
        try
        {
            if (HasPowerNamed(localCreature, DiamondDiademPowerTypeName)
                || !TryFindDiamondDiadem(player, out var relic)
                || !TryReadCardThreshold(relic, out var threshold)
                || !TryReadCardsPlayedThisTurn(relic, out var cardsPlayed))
            {
                return EnemyDamageModificationResult.Supported(nativeAmount);
            }

            if (cardsPlayed == threshold && HasPendingStampedeAttack(player, localCreature))
            {
                cardsPlayed++;
            }

            if (cardsPlayed > threshold || (!isSingleVerifiedEvent && nativeAmount > 0))
            {
                return EnemyDamageModificationResult.Supported(nativeAmount);
            }

            return EnemyDamageModificationResult.Supported(
                ReadFutureDiamondDiademSingleDamage(player, localCreature, attackIntent, enemy));
        }
        catch (Exception exception)
        {
            PartyWatchDiagnostics.ReportOnce("diamond.legacy-apply", exception);
            return EnemyDamageModificationResult.Supported(nativeAmount);
        }
    }

    private static bool TryFindDiamondDiadem(Player player, out RelicModel relic)
    {
        relic = player.Relics.FirstOrDefault(candidate =>
            !candidate.IsMelted && candidate.GetType().FullName == DiamondDiademTypeName)!;
        return relic is not null;
    }

    private static bool TryReadCardThreshold(RelicModel relic, out int threshold)
    {
        threshold = 0;
        try
        {
            threshold = Math.Max(0, (int)relic.DynamicVars["CardThreshold"].BaseValue);
            return true;
        }
        catch (Exception exception)
        {
            PartyWatchDiagnostics.ReportOnce("diamond.card-threshold", exception);
            return false;
        }
    }

    private static bool TryReadCardsPlayedThisTurn(RelicModel relic, out int cardsPlayed)
    {
        cardsPlayed = 0;
        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        try
        {
            var property = relic.GetType().GetProperty("CardsPlayedThisTurn", flags);
            if (property?.GetValue(relic) is not { } value)
            {
                return false;
            }

            cardsPlayed = Math.Max(0, Convert.ToInt32(value));
            return true;
        }
        catch (Exception exception)
        {
            PartyWatchDiagnostics.ReportOnce("diamond.cards-played", exception);
            return false;
        }
    }

    private static bool HasPendingStampedeAttack(Player player, Creature localCreature)
    {
        if (!HasPositivePowerNamed(localCreature, StampedePowerTypeName))
        {
            return false;
        }

        var handPile = CardPile.Get(PileType.Hand, player);
        return handPile is not null && handPile.Cards.Any(card => card.Owner == player && card.Type == CardType.Attack);
    }

    private static bool HasPowerNamed(Creature creature, string powerTypeName) =>
        creature.Powers.Any(power => power.GetType().FullName == powerTypeName);

    private static bool HasPositivePowerNamed(Creature creature, string powerTypeName) =>
        creature.Powers.Any(power => power.GetType().FullName == powerTypeName && power.Amount > 0);

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

        var beforeCap = HookDamageCompat.ModifyDamage(
            player.RunState,
            localCreature.CombatState!,
            localCreature,
            enemy,
            baseDamage,
            props,
            null,
            additiveAndMultiplicative,
            CardPreviewMode.None);

        var afterDiamondDiadem = beforeCap * 0.5m;
        var afterCap = HookDamageCompat.ModifyDamage(
            player.RunState,
            localCreature.CombatState!,
            localCreature,
            enemy,
            afterDiamondDiadem,
            props,
            null,
            ModifyDamageHookType.Cap,
            CardPreviewMode.None);

        return Math.Max(0, (int)afterCap);
    }
}
