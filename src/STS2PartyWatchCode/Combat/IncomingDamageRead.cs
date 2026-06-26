namespace STS2PartyWatch.Combat;

public readonly record struct IncomingDamageRead(
    IncomingDamageReadState State,
    int RawDamage,
    int EffectiveBlock,
    int DirectHpLoss)
{
    public static IncomingDamageRead Hidden => new(IncomingDamageReadState.Hidden, 0, 0, 0);

    public static IncomingDamageRead Unknown => new(IncomingDamageReadState.Unknown, 0, 0, 0);

    public static IncomingDamageRead UnknownDirect(int directHpLoss) =>
        new(IncomingDamageReadState.Unknown, 0, 0, Math.Max(0, directHpLoss));

    public static IncomingDamageRead Known(int rawDamage, int effectiveBlock, int directHpLoss) =>
        new(IncomingDamageReadState.Known, rawDamage, effectiveBlock, Math.Max(0, directHpLoss));
}

public enum IncomingDamageReadState
{
    Hidden,
    Known,
    Unknown
}
