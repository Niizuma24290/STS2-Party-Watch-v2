using DamageForecast.Combat;

namespace DamageForecast.Forecast;

internal readonly record struct ForecastHudSnapshot(
    ForecastResult ExpectedHpLoss,
    IncomingDamageDisplayRead IncomingDamage)
{
    public static ForecastHudSnapshot Hidden => new(ForecastResult.Hidden, IncomingDamageDisplayRead.Hidden);
}
