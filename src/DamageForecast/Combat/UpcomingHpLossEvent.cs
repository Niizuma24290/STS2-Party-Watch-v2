namespace DamageForecast.Combat;

internal readonly record struct UpcomingHpLossEvent(
    string Source,
    int NativeExecutionOrder,
    HpLossDisplayLane DisplayLane,
    int VerifiedHpLoss,
    bool IsSingleVerifiedEvent);

internal enum HpLossDisplayLane
{
    Blockable,
    DirectHpLoss
}
