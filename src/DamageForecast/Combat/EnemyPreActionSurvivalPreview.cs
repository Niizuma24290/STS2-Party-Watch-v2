using System.Runtime.CompilerServices;
using System.Reflection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;
using DamageForecast.Diagnostics;

namespace DamageForecast.Combat;

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
    private static readonly MethodInfo? CalculateTotalDamageNextTurnMethod =
        typeof(PoisonPower).GetMethod(
            "CalculateTotalDamageNextTurn",
            BindingFlags.Public | BindingFlags.Instance,
            Type.EmptyTypes);

    public static PoisonTickPreviewResult Preview(EnemyPreActionState state)
    {
        if (state.CurrentPoison <= 0)
        {
            return PoisonTickPolicy.Preview(ToPolicyInput(state));
        }

        var hasIntangible = state.NativeEnemy.GetPower<IntangiblePower>()?.Amount > 0;
        var hasHardToKill = state.NativeEnemy.GetPower<HardToKillPower>()?.Amount > 0;
        var hasHardenedShell = HasHardenedShellBudget(state.NativeEnemy);
        var slipperyAmount = state.NativeEnemy.GetPower<SlipperyPower>()?.Amount ?? 0;
        int? hardenedShellBudget = null;
        int? nativePreviewedDamage = null;
        IReadOnlyList<int>? modifiedTickDamages = null;

        if (!hasIntangible && hasHardenedShell)
        {
            if (TryGetHardenedShellRemainingBudget(state.NativeEnemy, out var budget))
            {
                hardenedShellBudget = budget;
                if (budget > 0 && budget >= state.CurrentHp)
                {
                    nativePreviewedDamage = TryReadNativePoisonDamage(state.NativeEnemy);
                }
            }
        }
        else if (!hasIntangible && slipperyAmount > 0)
        {
            if (!TryReadModifiedPoisonTickDamages(state, slipperyAmount, out modifiedTickDamages))
            {
                nativePreviewedDamage = TryReadNativePoisonDamage(state.NativeEnemy);
            }
        }
        else if (!hasIntangible)
        {
            nativePreviewedDamage = TryReadNativePoisonDamage(state.NativeEnemy);
        }

        return PoisonTickPolicy.Preview(new PoisonTickPolicyInput(
            state.CurrentHp,
            state.CurrentPoison,
            state.OpponentAccelerant,
            hasIntangible,
            hasHardToKill,
            slipperyAmount > 0,
            slipperyAmount,
            hasHardenedShell,
            hardenedShellBudget,
            nativePreviewedDamage,
            modifiedTickDamages));
    }

    private static PoisonTickPolicyInput ToPolicyInput(EnemyPreActionState state) =>
        new(state.CurrentHp, state.CurrentPoison, state.OpponentAccelerant);

    private static bool TryGetHardenedShellRemainingBudget(Creature enemy, out int remainingBudget)
    {
        remainingBudget = 0;
        var power = enemy.Powers.FirstOrDefault(power => power.GetType().Name == "HardenedShellPower");
        if (power is null)
        {
            return false;
        }

        return TryReadIntMember(power, "DisplayAmount", out remainingBudget);
    }

    private static bool TryReadIntMember(object instance, string memberName, out int value)
    {
        value = 0;
        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        try
        {
            var property = instance.GetType().GetProperty(memberName, flags);
            if (property is not null)
            {
                value = Math.Max(0, Convert.ToInt32(property.GetValue(instance)));
                return true;
            }

            var field = instance.GetType().GetField(memberName, flags);
            if (field is not null)
            {
                value = Math.Max(0, Convert.ToInt32(field.GetValue(instance)));
                return true;
            }
        }
        catch (Exception exception)
        {
            DamageForecastDiagnostics.ReportOnce("poison.hardened-shell-member", exception);
            value = 0;
        }

        return false;
    }

    private static bool TryReadModifiedPoisonTickDamages(
        EnemyPreActionState state,
        int slipperyAmount,
        out IReadOnlyList<int>? tickDamages)
    {
        tickDamages = null;
        var combatState = state.NativeEnemy.CombatState;
        if (combatState is null)
        {
            return false;
        }

        try
        {
            var poison = Math.Max(0, state.CurrentPoison);
            var triggers = PoisonTickPolicy.PreviewTriggerCount(poison, state.OpponentAccelerant);
            var values = new int[triggers];
            var remainingSlippery = Math.Max(0, slipperyAmount);
            var previewedHpLoss = 0;
            for (var i = 0; i < triggers; i++)
            {
                var damage = PreviewModifiedPoisonTickDamage(state.NativeEnemy, poison);
                values[i] = damage;
                var hpLoss = damage;
                if (remainingSlippery > 0 && hpLoss > 0)
                {
                    hpLoss = Math.Min(hpLoss, 1);
                    remainingSlippery--;
                }

                previewedHpLoss += hpLoss;
                if (previewedHpLoss >= state.CurrentHp)
                {
                    tickDamages = values[..(i + 1)];
                    return true;
                }

                poison = Math.Max(0, poison - 1);
            }

            tickDamages = values;
            return true;
        }
        catch (Exception exception)
        {
            DamageForecastDiagnostics.ReportOnce("poison.slippery-preview", exception);
            tickDamages = null;
            return false;
        }
    }

    private static int PreviewModifiedPoisonTickDamage(Creature enemy, int baseDamage)
    {
        var modified = HookDamageCompat.ModifyDamage(
            enemy.CombatState!.RunState,
            enemy.CombatState,
            enemy,
            enemy,
            Math.Max(0, baseDamage),
            ValueProp.Unblockable | ValueProp.Unpowered,
            null,
            ModifyDamageHookType.All,
            CardPreviewMode.None);

        return Math.Max(0, (int)modified);
    }

    private static int? TryReadNativePoisonDamage(Creature enemy)
    {
        var poisonPower = enemy.GetPower<PoisonPower>();
        if (poisonPower is null)
        {
            return null;
        }

        if (CalculateTotalDamageNextTurnMethod is null)
        {
            return null;
        }

        try
        {
            var value = CalculateTotalDamageNextTurnMethod.Invoke(poisonPower, null);
            if (value is null)
            {
                return null;
            }

            return Math.Max(0, Convert.ToInt32(value));
        }
        catch (Exception exception)
        {
            DamageForecastDiagnostics.ReportOnce("poison.native-preview", exception);
            return null;
        }
    }

    private static bool HasPowerNamed(Creature enemy, string powerTypeName) =>
        enemy.Powers.Any(power => power.Amount > 0 && power.GetType().Name == powerTypeName);

    private static bool HasHardenedShellBudget(Creature enemy) =>
        HasPowerNamed(enemy, "HardenedShellPower") || enemy.Monster?.GetType().Name == "SewerClam";
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
    string LifecycleState)
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
                or NemesisPower
                || power.GetType().Name == "HardenedShellPower")
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
            lifecycleState);
    }
}

internal sealed record EnemyPreActionPreviewResult(
    EnemyPreActionState State,
    PoisonTickPreviewResult PoisonTick)
{
    public bool WillExecuteCurrentIntent => PoisonTick.WillExecuteCurrentIntent;

    public bool WillExecuteIntentFor(string stableIdentity) =>
        PoisonIntentIdentityPolicy.ShouldRetainCurrentIntent(
            stableIdentity,
            State.Identity.StableIdentity,
            WillExecuteCurrentIntent);
}

internal readonly record struct PoisonTickPreviewResult(
    int PreviewedPoisonDamage,
    bool WillRemainAliveBeforeAction,
    bool WillExecuteCurrentIntent)
{
    public static PoisonTickPreviewResult Create(int previewedDamage, bool willRemainAliveBeforeAction) =>
        new(
            Math.Max(0, previewedDamage),
            willRemainAliveBeforeAction,
            willRemainAliveBeforeAction);
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
