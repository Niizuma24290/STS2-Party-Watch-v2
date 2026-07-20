namespace STS2PartyWatch.Combat;

internal readonly record struct IncomingDamageDisplayRead(
    IncomingDamageDisplayReadState State,
    int Damage)
{
    public static IncomingDamageDisplayRead Hidden => new(IncomingDamageDisplayReadState.Hidden, 0);

    public static IncomingDamageDisplayRead Unknown => new(IncomingDamageDisplayReadState.Unknown, 0);

    public static IncomingDamageDisplayRead Known(int damage) =>
        damage > 0
            ? new(IncomingDamageDisplayReadState.Known, damage)
            : Hidden;
}

internal enum IncomingDamageDisplayReadState
{
    Hidden,
    Known,
    Unknown
}
