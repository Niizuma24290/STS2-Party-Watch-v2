namespace DamageForecast.Combat;

internal static class PoisonTickPolicy
{
    public static PoisonTickPreviewResult Preview(PoisonTickPolicyInput input)
    {
        if (input.CurrentPoison <= 0)
        {
            return Survives();
        }

        var triggers = PreviewTriggerCount(input.CurrentPoison, input.OpponentAccelerant);
        if (input.HasIntangible)
        {
            if (input.HasHardToKill || input.HasSlippery || input.HasHardenedShell)
            {
                return Survives();
            }

            var damage = Math.Min(Math.Max(0, input.CurrentHp), triggers);
            return FromDamage(damage, input.CurrentHp);
        }

        if (input.HasHardenedShell)
        {
            if (input.HardenedShellRemainingBudget is not int budget
                || budget <= 0
                || budget < input.CurrentHp)
            {
                return Survives();
            }

            var damage = input.NativePreviewedDamage;
            if (damage is null && input.HasHardToKill)
            {
                return Survives();
            }

            damage ??= PreviewOrdinaryDamage(input.CurrentPoison, input.OpponentAccelerant);
            return FromDamage(Math.Min(Math.Max(0, damage.Value), budget), input.CurrentHp);
        }

        if (input.HasSlippery
            && input.ModifiedTickDamages is { } tickDamages)
        {
            var remainingSlippery = Math.Max(0, input.SlipperyAmount);
            var previewedDamage = 0;
            var availableTriggers = Math.Min(triggers, tickDamages.Count);
            for (var i = 0; i < availableTriggers; i++)
            {
                var hpLoss = Math.Max(0, tickDamages[i]);
                if (remainingSlippery > 0 && hpLoss > 0)
                {
                    hpLoss = Math.Min(hpLoss, 1);
                    remainingSlippery--;
                }

                previewedDamage += hpLoss;
                if (previewedDamage >= input.CurrentHp)
                {
                    return FromDamage(previewedDamage, input.CurrentHp);
                }
            }

            if (tickDamages.Count >= triggers)
            {
                return FromDamage(previewedDamage, input.CurrentHp);
            }
        }

        if (input.NativePreviewedDamage is int nativeDamage)
        {
            return FromDamage(nativeDamage, input.CurrentHp);
        }

        if (input.HasHardToKill)
        {
            return Survives();
        }

        return FromDamage(
            PreviewOrdinaryDamage(input.CurrentPoison, input.OpponentAccelerant),
            input.CurrentHp);
    }

    public static int PreviewTriggerCount(int currentPoison, int opponentAccelerant) =>
        Math.Min(Math.Max(0, currentPoison), 1 + Math.Max(0, opponentAccelerant));

    public static int PreviewOrdinaryDamage(int currentPoison, int opponentAccelerant)
    {
        var poison = Math.Max(0, currentPoison);
        var triggers = PreviewTriggerCount(poison, opponentAccelerant);
        var previewedDamage = 0;
        for (var i = 0; i < triggers; i++)
        {
            previewedDamage += poison;
            poison = Math.Max(0, poison - 1);
        }

        return previewedDamage;
    }

    private static PoisonTickPreviewResult FromDamage(int damage, int currentHp) =>
        PoisonTickPreviewResult.Create(damage, Math.Max(0, damage) < currentHp);

    private static PoisonTickPreviewResult Survives() =>
        PoisonTickPreviewResult.Create(0, willRemainAliveBeforeAction: true);
}

internal readonly record struct PoisonTickPolicyInput(
    int CurrentHp,
    int CurrentPoison,
    int OpponentAccelerant,
    bool HasIntangible = false,
    bool HasHardToKill = false,
    bool HasSlippery = false,
    int SlipperyAmount = 0,
    bool HasHardenedShell = false,
    int? HardenedShellRemainingBudget = null,
    int? NativePreviewedDamage = null,
    IReadOnlyList<int>? ModifiedTickDamages = null);

internal static class PoisonIntentIdentityPolicy
{
    public static bool ShouldRetainCurrentIntent(
        string candidateStableIdentity,
        string previewedStableIdentity,
        bool previewedIntentWillExecute) =>
        !string.Equals(candidateStableIdentity, previewedStableIdentity, StringComparison.Ordinal)
        || previewedIntentWillExecute;
}
