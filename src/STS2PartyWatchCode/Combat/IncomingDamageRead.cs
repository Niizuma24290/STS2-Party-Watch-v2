namespace STS2PartyWatch.Combat;

public readonly record struct IncomingDamageRead(
    IncomingDamageReadState State,
    int RawDamage,
    int EffectiveBlock)
{
    public static IncomingDamageRead Hidden => new(IncomingDamageReadState.Hidden, 0, 0);

    public static IncomingDamageRead Unknown => new(IncomingDamageReadState.Unknown, 0, 0);

    public static IncomingDamageRead Known(int rawDamage, int effectiveBlock) =>
        new(IncomingDamageReadState.Known, rawDamage, effectiveBlock);
}

public enum IncomingDamageReadState
{
    Hidden,
    Known,
    Unknown
}
