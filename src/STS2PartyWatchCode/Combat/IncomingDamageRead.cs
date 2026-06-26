namespace STS2PartyWatch.Combat;

public readonly record struct IncomingDamageRead(
    IncomingDamageReadState State,
    int RawDamage,
    int LocalBlock)
{
    public static IncomingDamageRead Hidden => new(IncomingDamageReadState.Hidden, 0, 0);

    public static IncomingDamageRead Unknown => new(IncomingDamageReadState.Unknown, 0, 0);

    public static IncomingDamageRead Known(int rawDamage, int localBlock) =>
        new(IncomingDamageReadState.Known, rawDamage, localBlock);
}

public enum IncomingDamageReadState
{
    Hidden,
    Known,
    Unknown
}
