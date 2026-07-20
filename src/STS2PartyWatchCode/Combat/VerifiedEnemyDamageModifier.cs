using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using STS2PartyWatch.Diagnostics;

namespace STS2PartyWatch.Combat;

internal static class VerifiedEnemyDamageModifier
{
    private const string DiamondDiademTypeName = "MegaCrit.Sts2.Core.Models.Relics.DiamondDiadem";
    private const string DiamondDiademPowerTypeName = "MegaCrit.Sts2.Core.Models.Powers.DiamondDiademPower";
    private static readonly DiamondDiademMechanismKind DiamondDiademMechanism = DetectDiamondDiademMechanism();

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
            return DiamondDiademMechanism == DiamondDiademMechanismKind.LegacyCardCountDamageReduction
                ? LegacyDiamondDiademDamageForecast.Apply(
                    player,
                    localCreature,
                    attackIntent,
                    enemy,
                    nativeAmount,
                    isSingleVerifiedEvent)
                : EnemyDamageModificationResult.Supported(nativeAmount);
        }
        catch (Exception exception)
        {
            PartyWatchDiagnostics.ReportOnce("diamond.apply", exception);
            return EnemyDamageModificationResult.Supported(nativeAmount);
        }
    }

    private static DiamondDiademMechanismKind DetectDiamondDiademMechanism()
    {
        try
        {
            var gameAssembly = typeof(RelicModel).Assembly;
            var relicType = gameAssembly.GetType(DiamondDiademTypeName, throwOnError: false);
            if (relicType is null)
            {
                return DiamondDiademMechanismKind.NativeOnlyFallback;
            }

            var hasLegacyCounter = relicType.GetProperty("CardsPlayedThisTurn") is not null;
            var hasLegacyPower = gameAssembly.GetType(DiamondDiademPowerTypeName, throwOnError: false) is not null;
            if (hasLegacyCounter && hasLegacyPower)
            {
                return DiamondDiademMechanismKind.LegacyCardCountDamageReduction;
            }

            var hasFirstTurnStartHook = relicType.GetMethods()
                .Any(method => method.Name == "AfterSideTurnStart");
            return hasFirstTurnStartHook
                ? DiamondDiademMechanismKind.FirstTurnBlockAndBlur
                : DiamondDiademMechanismKind.NativeOnlyFallback;
        }
        catch (Exception exception)
        {
            PartyWatchDiagnostics.ReportOnce("diamond.detect-mechanism", exception);
            return DiamondDiademMechanismKind.NativeOnlyFallback;
        }
    }

    private enum DiamondDiademMechanismKind
    {
        LegacyCardCountDamageReduction,
        FirstTurnBlockAndBlur,
        NativeOnlyFallback
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
