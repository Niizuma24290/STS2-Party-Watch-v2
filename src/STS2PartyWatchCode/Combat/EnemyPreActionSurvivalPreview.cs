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
using STS2PartyWatch.Diagnostics;

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
    private static readonly MethodInfo? CalculateTotalDamageNextTurnMethod =
        typeof(PoisonPower).GetMethod(
            "CalculateTotalDamageNextTurn",
            BindingFlags.Public | BindingFlags.Instance,
            Type.EmptyTypes);

    public static PoisonTickPreviewResult Preview(EnemyPreActionState state)
    {
        if (state.CurrentPoison <= 0)
        {
            return PoisonTickPreviewResult.Create(0, willRemainAliveBeforeAction: true);
        }

        if (state.NativeEnemy.GetPower<IntangiblePower>()?.Amount > 0
            && TryPreviewIntangiblePoisonDamage(state, out var intangiblePreviewedDamage))
        {
            return PoisonTickPreviewResult.Create(
                intangiblePreviewedDamage,
                willRemainAliveBeforeAction: intangiblePreviewedDamage < state.CurrentHp);
        }

        if (HasHardenedShellBudget(state.NativeEnemy))
        {
            if (TryGetHardenedShellRemainingBudget(state.NativeEnemy, out var hardenedShellBudget)
                && TryPreviewHardenedShellPoisonDamage(state, hardenedShellBudget, out var hardenedShellPreviewedDamage))
            {
                return PoisonTickPreviewResult.Create(
                    hardenedShellPreviewedDamage,
                    willRemainAliveBeforeAction: hardenedShellPreviewedDamage < state.CurrentHp);
            }

            return PoisonTickPreviewResult.Create(0, willRemainAliveBeforeAction: true);
        }

        if (state.NativeEnemy.GetPower<SlipperyPower>()?.Amount is > 0 and var slipperyAmount
            && TryPreviewSlipperyPoisonDamage(state, slipperyAmount, out var slipperyPreviewedDamage))
        {
            return PoisonTickPreviewResult.Create(
                slipperyPreviewedDamage,
                willRemainAliveBeforeAction: slipperyPreviewedDamage < state.CurrentHp);
        }

        var poisonPower = state.NativeEnemy.GetPower<PoisonPower>();
        if (poisonPower is not null && TryReadNativePoisonDamage(poisonPower, out var nativePreviewedDamage))
        {
            return PoisonTickPreviewResult.Create(
                nativePreviewedDamage,
                willRemainAliveBeforeAction: nativePreviewedDamage < state.CurrentHp);
        }

        if (state.NativeEnemy.GetPower<HardToKillPower>()?.Amount > 0)
        {
            return PoisonTickPreviewResult.Create(0, willRemainAliveBeforeAction: true);
        }

        var previewedDamage = PreviewOrdinaryPoisonDamage(state.CurrentPoison, state.OpponentAccelerant);
        return PoisonTickPreviewResult.Create(
            previewedDamage,
            willRemainAliveBeforeAction: previewedDamage < state.CurrentHp);
    }

    private static bool TryPreviewIntangiblePoisonDamage(
        EnemyPreActionState state,
        out int previewedHpLoss)
    {
        previewedHpLoss = 0;
        if (state.NativeEnemy.GetPower<HardToKillPower>()?.Amount > 0
            || state.NativeEnemy.GetPower<SlipperyPower>()?.Amount > 0
            || HasHardenedShellBudget(state.NativeEnemy))
        {
            return true;
        }

        var triggers = PreviewPoisonTriggerCount(state.CurrentPoison, state.OpponentAccelerant);
        previewedHpLoss = Math.Min(Math.Max(0, state.CurrentHp), triggers);
        return true;
    }

    private static bool TryPreviewHardenedShellPoisonDamage(
        EnemyPreActionState state,
        int remainingBudget,
        out int previewedHpLoss)
    {
        previewedHpLoss = 0;
        if (remainingBudget <= 0 || remainingBudget < state.CurrentHp)
        {
            return true;
        }

        var poisonPower = state.NativeEnemy.GetPower<PoisonPower>();
        if (poisonPower is not null && TryReadNativePoisonDamage(poisonPower, out var nativePreviewedDamage))
        {
            previewedHpLoss = Math.Min(nativePreviewedDamage, remainingBudget);
            return true;
        }

        if (state.NativeEnemy.GetPower<HardToKillPower>()?.Amount > 0)
        {
            return true;
        }

        previewedHpLoss = Math.Min(
            PreviewOrdinaryPoisonDamage(state.CurrentPoison, state.OpponentAccelerant),
            remainingBudget);
        return true;
    }

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
            PartyWatchDiagnostics.ReportOnce("poison.hardened-shell-member", exception);
            value = 0;
        }

        return false;
    }

    private static bool TryPreviewSlipperyPoisonDamage(
        EnemyPreActionState state,
        int slipperyAmount,
        out int previewedHpLoss)
    {
        previewedHpLoss = 0;
        var combatState = state.NativeEnemy.CombatState;
        if (combatState is null)
        {
            return false;
        }

        try
        {
            var poison = Math.Max(0, state.CurrentPoison);
            var triggers = Math.Min(poison, 1 + Math.Max(0, state.OpponentAccelerant));
            var remainingSlippery = Math.Max(0, slipperyAmount);
            for (var i = 0; i < triggers; i++)
            {
                var damage = PreviewModifiedPoisonTickDamage(state.NativeEnemy, poison);
                var hpLoss = damage;
                if (remainingSlippery > 0 && hpLoss > 0)
                {
                    hpLoss = Math.Min(hpLoss, 1);
                    remainingSlippery--;
                }

                previewedHpLoss += hpLoss;
                if (previewedHpLoss >= state.CurrentHp)
                {
                    return true;
                }

                poison = Math.Max(0, poison - 1);
            }

            return true;
        }
        catch (Exception exception)
        {
            PartyWatchDiagnostics.ReportOnce("poison.slippery-preview", exception);
            previewedHpLoss = 0;
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

    private static bool TryReadNativePoisonDamage(PoisonPower poisonPower, out int previewedDamage)
    {
        previewedDamage = 0;
        if (CalculateTotalDamageNextTurnMethod is null)
        {
            return false;
        }

        try
        {
            var value = CalculateTotalDamageNextTurnMethod.Invoke(poisonPower, null);
            if (value is null)
            {
                return false;
            }

            previewedDamage = Math.Max(0, Convert.ToInt32(value));
            return true;
        }
        catch (Exception exception)
        {
            PartyWatchDiagnostics.ReportOnce("poison.native-preview", exception);
            previewedDamage = 0;
            return false;
        }
    }

    private static int PreviewOrdinaryPoisonDamage(int currentPoison, int opponentAccelerant)
    {
        var poison = Math.Max(0, currentPoison);
        var triggers = PreviewPoisonTriggerCount(poison, opponentAccelerant);
        var previewedDamage = 0;
        for (var i = 0; i < triggers; i++)
        {
            previewedDamage += Math.Max(0, poison);
            poison = Math.Max(0, poison - 1);
        }

        return previewedDamage;
    }

    private static int PreviewPoisonTriggerCount(int currentPoison, int opponentAccelerant) =>
        Math.Min(Math.Max(0, currentPoison), 1 + Math.Max(0, opponentAccelerant));

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
