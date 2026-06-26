namespace STS2PartyWatch.Forecast;

public readonly record struct ForecastResult(ForecastResultState State, int OutDamage, int DirectHpLoss)
{
    public static ForecastResult Hidden => new(ForecastResultState.Hidden, 0, 0);

    public static ForecastResult Unknown => new(ForecastResultState.Unknown, 0, 0);

    public static ForecastResult KnownDamage(int outDamage, int directHpLoss) =>
        new(ForecastResultState.KnownDamage, Math.Max(0, outDamage), Math.Max(0, directHpLoss));
}

public enum ForecastResultState
{
    Hidden,
    KnownDamage,
    Unknown
}
