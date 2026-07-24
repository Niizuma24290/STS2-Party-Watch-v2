namespace DamageForecast.Combat;

internal static class VerifiedTurnEndDamagePolicy
{
    public static TurnEndDamagePolicyResult ClassifyDescriptor(
        TurnEndDamageShapeDescriptor descriptor,
        TurnEndDamageInstanceShape instance)
    {
        if (descriptor.BodyResolution == TurnEndDamageBodyResolution.NoTurnEndEffect)
        {
            return new TurnEndDamagePolicyResult(
                TurnEndDamageDecision.NoDamage,
                TurnEndDamageSupportReason.NoTurnEndEffect);
        }

        if (descriptor.BodyResolution == TurnEndDamageBodyResolution.Unreadable)
        {
            return Unsupported(TurnEndDamageSupportReason.UnreadableMethodBody);
        }

        if (descriptor.BodyResolution is not (
                TurnEndDamageBodyResolution.ParsedSync
                or TurnEndDamageBodyResolution.ParsedAsync))
        {
            return Unsupported(TurnEndDamageSupportReason.UnreadableMethodBody);
        }

        if (descriptor.DamageCallCount == 0)
        {
            return new TurnEndDamagePolicyResult(
                TurnEndDamageDecision.NoDamage,
                TurnEndDamageSupportReason.NoDamageCall);
        }

        if (descriptor.DamageCallCount != 1)
        {
            return Unsupported(TurnEndDamageSupportReason.DamageCallCountMismatch);
        }

        if (!descriptor.IsOfficialCurrentGameAssembly)
        {
            return Unsupported(TurnEndDamageSupportReason.ExternalDamagingType);
        }

        if (!descriptor.MethodDeclaredOnConcreteType)
        {
            return Unsupported(TurnEndDamageSupportReason.InheritedOrForeignTurnEndMethod);
        }

        var supportedDamageVarOverload =
            descriptor.OverloadKind == TurnEndDamageOverloadKind.DamageVarStable
            || descriptor.OverloadKind == TurnEndDamageOverloadKind.DamageVarBeta;
        if (!supportedDamageVarOverload)
        {
            return Unsupported(TurnEndDamageSupportReason.UnsupportedDamageOverload);
        }

        if (!descriptor.TargetIsSameCardOwnerCreature)
        {
            return Unsupported(TurnEndDamageSupportReason.UnknownTargetProvenance);
        }

        if (!descriptor.AmountIsSameCardDynamicVarsDamage)
        {
            return Unsupported(TurnEndDamageSupportReason.UnknownAmountProvenance);
        }

        if (!descriptor.SourceCardIsSameCard)
        {
            return Unsupported(TurnEndDamageSupportReason.UnknownSourceCardProvenance);
        }

        if (descriptor.OverloadKind == TurnEndDamageOverloadKind.DamageVarBeta
            && !descriptor.BetaCardPlayIsNull)
        {
            return Unsupported(TurnEndDamageSupportReason.UnexpectedBetaCardPlay);
        }

        if (!descriptor.ControlFlowIsSupported)
        {
            return Unsupported(TurnEndDamageSupportReason.UnsupportedControlFlow);
        }

        if (instance.DamageVarCount != 1)
        {
            return Unsupported(TurnEndDamageSupportReason.DamageVarCountMismatch);
        }

        if (!instance.DamageVarBaseValueIsSupported)
        {
            return Unsupported(TurnEndDamageSupportReason.UnsupportedDamageVarValue);
        }

        if (!instance.DamageVarPropsAreExactUnpoweredMove)
        {
            return Unsupported(TurnEndDamageSupportReason.UnsupportedDamageVarProps);
        }

        return new TurnEndDamagePolicyResult(
            TurnEndDamageDecision.VerifiedBlockable,
            TurnEndDamageSupportReason.VerifiedSingleOfficialBlockableDamage);
    }

    public static bool IsVerifiedSingleBlockableDamageShape(
        Type runtimeCardType,
        Type verifiedCardType,
        int damageVarCount,
        bool singleDamageVarIsUnblockable)
    {
        return runtimeCardType == verifiedCardType
            && damageVarCount == 1
            && !singleDamageVarIsUnblockable;
    }

    private static TurnEndDamagePolicyResult Unsupported(TurnEndDamageSupportReason reason)
    {
        return new TurnEndDamagePolicyResult(TurnEndDamageDecision.UnsupportedDamage, reason);
    }
}
