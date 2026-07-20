using STS2PartyWatch.Combat;

namespace STS2PartyWatch.Forecast;

internal readonly record struct ForecastHudSnapshot(
    ForecastResult ExpectedHpLoss,
    IncomingDamageDisplayRead IncomingDamage)
{
    public static ForecastHudSnapshot Hidden => new(ForecastResult.Hidden, IncomingDamageDisplayRead.Hidden);
}
