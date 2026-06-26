using STS2PartyWatch.Combat;

namespace STS2PartyWatch.Forecast;

public sealed class LocalDamageForecast
{
    public ForecastResult Calculate(IncomingDamageRead read)
    {
        return read.State switch
        {
            IncomingDamageReadState.Hidden => ForecastResult.Hidden,
            IncomingDamageReadState.Unknown => ForecastResult.Unknown,
            IncomingDamageReadState.Known => CalculateKnown(read.RawDamage, read.EffectiveBlock),
            _ => ForecastResult.Unknown
        };
    }

    private static ForecastResult CalculateKnown(int rawDamage, int localBlock)
    {
        var outDamage = Math.Max(0, rawDamage - localBlock);
        return outDamage > 0
            ? ForecastResult.KnownDamage(outDamage)
            : ForecastResult.Hidden;
    }
}
