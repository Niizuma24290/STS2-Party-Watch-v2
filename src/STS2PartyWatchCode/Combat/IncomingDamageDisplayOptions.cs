namespace STS2PartyWatch.Combat;

internal readonly record struct IncomingDamageDisplayOptions(
    bool IncludeCurrentBlock,
    bool IncludePowerBlock,
    bool IncludeRelicBlock,
    bool IncludePowerHpLossModifiers,
    bool IncludeRelicHpLossModifiers);
