namespace DamageForecast.Combat;

internal enum TurnEndDamageBodyResolution
{
    NoTurnEndEffect,
    ParsedSync,
    ParsedAsync,
    Unreadable
}

internal enum TurnEndDamageOverloadKind
{
    None,
    DamageVarStable,
    DamageVarBeta,
    Decimal,
    Other
}

internal enum TurnEndDamageDecision
{
    NoDamage,
    VerifiedBlockable,
    UnsupportedDamage
}

internal enum TurnEndDamageSupportReason
{
    NoTurnEndEffect,
    NoDamageCall,
    ExternalDamagingType,
    InheritedOrForeignTurnEndMethod,
    UnreadableMethodBody,
    DamageCallCountMismatch,
    UnsupportedDamageOverload,
    UnknownTargetProvenance,
    UnknownAmountProvenance,
    UnknownSourceCardProvenance,
    UnexpectedBetaCardPlay,
    UnsupportedControlFlow,
    DamageVarCountMismatch,
    UnsupportedDamageVarValue,
    UnsupportedDamageVarProps,
    VerifiedSingleOfficialBlockableDamage
}

internal readonly record struct TurnEndDamageShapeDescriptor(
    bool IsOfficialCurrentGameAssembly,
    bool MethodDeclaredOnConcreteType,
    TurnEndDamageBodyResolution BodyResolution,
    int DamageCallCount,
    TurnEndDamageOverloadKind OverloadKind,
    bool TargetIsSameCardOwnerCreature,
    bool AmountIsSameCardDynamicVarsDamage,
    bool SourceCardIsSameCard,
    bool BetaCardPlayIsNull,
    bool ControlFlowIsSupported);

internal readonly record struct TurnEndDamageInstanceShape(
    int DamageVarCount,
    bool DamageVarBaseValueIsSupported,
    bool DamageVarPropsAreExactUnpoweredMove);

internal readonly record struct TurnEndDamagePolicyResult(
    TurnEndDamageDecision Decision,
    TurnEndDamageSupportReason Reason);
