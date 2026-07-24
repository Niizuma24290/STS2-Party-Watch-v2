namespace DamageForecast.Combat;

internal enum HandCardDamageClassification
{
    NoDamage,
    VerifiedBlockable,
    VerifiedDirect,
    UnsupportedDamage
}

internal readonly record struct HandCardDamageClassificationInput(
    bool GenericDamageInspectionAccepted,
    bool HasVerifiedBlockableDamage,
    bool HasVerifiedDirectHpLoss);

internal static class HandCardDamageClassificationPolicy
{
    public static HandCardDamageClassification Classify(HandCardDamageClassificationInput input)
    {
        if (input.HasVerifiedDirectHpLoss)
        {
            return HandCardDamageClassification.VerifiedDirect;
        }

        if (!input.GenericDamageInspectionAccepted)
        {
            return HandCardDamageClassification.UnsupportedDamage;
        }

        if (input.HasVerifiedBlockableDamage)
        {
            return HandCardDamageClassification.VerifiedBlockable;
        }

        return HandCardDamageClassification.NoDamage;
    }
}
