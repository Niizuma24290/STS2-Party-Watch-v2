using DamageForecast.Combat;

namespace DamageForecast.Forecast;

public sealed class LocalDamageForecast
{
    public ForecastResult Calculate(IncomingDamageRead read)
    {
        return read.State switch
        {
            IncomingDamageReadState.Hidden => ForecastResult.Hidden,
            IncomingDamageReadState.Unknown => CalculateUnknown(read.DirectHpLoss),
            IncomingDamageReadState.Known => CalculateKnown(read.RawDamage, read.EffectiveBlock, read.DirectHpLoss),
            _ => ForecastResult.Unknown
        };
    }

    private static ForecastResult CalculateUnknown(int directHpLoss)
    {
        return directHpLoss > 0
            ? ForecastResult.KnownDamage(0, directHpLoss)
            : ForecastResult.Unknown;
    }

    private static ForecastResult CalculateKnown(int rawDamage, int localBlock, int directHpLoss)
    {
        var outDamage = Math.Max(0, rawDamage - localBlock);
        return outDamage > 0 || directHpLoss > 0
            ? ForecastResult.KnownDamage(outDamage, directHpLoss)
            : ForecastResult.Hidden;
    }
}
