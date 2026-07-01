using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace STS2PartyWatch.Combat;

internal static class EnemyPreActionSurvivalPreview
{
    public static EnemyPreActionPreviewResult Preview(Creature enemy, int snapshotIndex, int currentIntentContribution)
    {
        var state = EnemyPreActionState.From(enemy, snapshotIndex, currentIntentContribution);
        var poisonResult = PoisonTickPreview.Preview(state);
        return new EnemyPreActionPreviewResult(state, poisonResult);
    }
}

internal static class PoisonTickPreview
{
    public static PoisonTickPreviewResult Preview(EnemyPreActionState state)
    {
        if (state.CurrentPoison <= 0)
        {
            return PoisonTickPreviewResult.CreateSupported(0, willRemainAliveBeforeAction: true);
        }

        var unsupportedReason = GetUnsupportedReason(state);
        if (unsupportedReason is not null)
        {
            return PoisonTickPreviewResult.Unsupported(unsupportedReason);
        }

        var remainingHp = state.CurrentHp;
        var poison = state.CurrentPoison;
        var previewedDamage = 0;
        var triggers = Math.Min(poison, 1 + state.OpponentAccelerant);
        for (var i = 0; i < triggers; i++)
        {
            var tickDamage = Math.Max(0, poison);
            previewedDamage += tickDamage;
            remainingHp -= tickDamage;
            if (remainingHp <= 0)
            {
                return PoisonTickPreviewResult.CreateSupported(previewedDamage, willRemainAliveBeforeAction: false);
            }

            poison = Math.Max(0, poison - 1);
        }

        return PoisonTickPreviewResult.CreateSupported(previewedDamage, willRemainAliveBeforeAction: true);
    }

    private static string? GetUnsupportedReason(EnemyPreActionState state)
    {
        var enemy = state.NativeEnemy;
        if (enemy.GetPower<HardToKillPower>()?.Amount > 0)
        {
            return "Poison tick damage is affected by HardToKillPower.";
        }

        if (enemy.GetPower<SlipperyPower>()?.Amount > 0)
        {
            return "Poison tick HP loss is affected by SlipperyPower.";
        }

        if (enemy.GetPower<IntangiblePower>()?.Amount > 0)
        {
            return "Poison tick damage or HP loss is affected by IntangiblePower.";
        }

        if (enemy.GetPower<AdaptablePower>()?.Amount > 0 || enemy.Monster is TestSubject)
        {
            return "Enemy lifecycle is affected by TestSubject/AdaptablePower.";
        }

        if (enemy.GetPower<HatchPower>()?.Amount > 0 || enemy.Monster is ToughEgg)
        {
            return "Enemy lifecycle is affected by ToughEgg/HatchPower.";
        }

        return null;
    }
}

internal sealed record EnemyPreActionState(
    EnemyInstanceIdentity Identity,
    Creature NativeEnemy,
    string DisplayName,
    string NativeClass,
    int CurrentHp,
    int CurrentPoison,
    int CurrentIntentContribution,
    int OpponentAccelerant,
    IReadOnlyList<string> RelevantPowers,
    string LifecycleState,
    bool Supported,
    string? UnsupportedReason)
{
    public static EnemyPreActionState From(Creature enemy, int snapshotIndex, int currentIntentContribution)
    {
        var poison = enemy.GetPower<PoisonPower>()?.Amount ?? 0;
        var accelerant = enemy.CombatState?
            .GetOpponentsOf(enemy)
            .Where(opponent => opponent.IsAlive)
            .Sum(opponent => opponent.GetPowerAmount<AccelerantPower>()) ?? 0;
        var powers = enemy.Powers
            .Where(power => power is PoisonPower
                or AccelerantPower
                or HardToKillPower
                or SlipperyPower
                or IntangiblePower
                or AdaptablePower
                or HatchPower
                or NemesisPower)
            .Select(power => $"{power.GetType().FullName}:{power.Amount}")
            .ToArray();
        var lifecycleState = enemy.IsAlive ? "Alive" : "Dead";

        return new EnemyPreActionState(
            EnemyInstanceIdentity.From(enemy, snapshotIndex),
            enemy,
            enemy.Name,
            enemy.Monster?.GetType().FullName ?? enemy.GetType().FullName ?? "unknown",
            enemy.CurrentHp,
            poison,
            Math.Max(0, currentIntentContribution),
            Math.Max(0, accelerant),
            powers,
            lifecycleState,
            true,
            null);
    }
}

internal sealed record EnemyPreActionPreviewResult(
    EnemyPreActionState State,
    PoisonTickPreviewResult PoisonTick)
{
    public bool Supported => PoisonTick.Supported;
    public string? UnsupportedReason => PoisonTick.UnsupportedReason;
    public bool WillExecuteCurrentIntent => PoisonTick.WillExecuteCurrentIntent;
}

internal readonly record struct PoisonTickPreviewResult(
    int PreviewedPoisonDamage,
    bool WillRemainAliveBeforeAction,
    bool WillExecuteCurrentIntent,
    bool Supported,
    string? UnsupportedReason)
{
    public static PoisonTickPreviewResult CreateSupported(int previewedDamage, bool willRemainAliveBeforeAction) =>
        new(
            Math.Max(0, previewedDamage),
            willRemainAliveBeforeAction,
            willRemainAliveBeforeAction,
            true,
            null);

    public static PoisonTickPreviewResult Unsupported(string unsupportedReason) =>
        new(0, true, false, false, unsupportedReason);
}

internal sealed record EnemyInstanceIdentity(
    string StableIdentity,
    Creature NativeReference,
    int SnapshotIndex)
{
    public static EnemyInstanceIdentity From(Creature enemy, int snapshotIndex)
    {
        var stableIdentity = enemy.CombatId.HasValue
            ? $"CombatId:{enemy.CombatId.Value}"
            : $"ObjectRef:{RuntimeHelpers.GetHashCode(enemy)}";
        return new EnemyInstanceIdentity(stableIdentity, enemy, snapshotIndex);
    }
}
