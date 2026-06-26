namespace STS2PartyWatch.Forecast;

public readonly record struct ForecastResult(ForecastResultState State, int OutDamage)
{
    public static ForecastResult Hidden => new(ForecastResultState.Hidden, 0);

    public static ForecastResult Unknown => new(ForecastResultState.Unknown, 0);

    public static ForecastResult KnownDamage(int outDamage) =>
        new(ForecastResultState.KnownDamage, outDamage);
}

public enum ForecastResultState
{
    Hidden,
    KnownDamage,
    Unknown
}
