using DamageForecast.Combat;
using DamageForecast.Forecast;

namespace DamageForecast.UI;

internal readonly record struct ForecastHudProjection(
    bool ShowExpectedHpLoss,
    bool ShowIncomingDamage,
    int ExpectedTotalHpLoss,
    int ExpectedBlockableHpLoss,
    int ExpectedDirectHpLoss,
    int IncomingDamage,
    IncomingDamagePlacement IncomingPlacement)
{
    public bool HasDisplayableValue =>
        (ShowExpectedHpLoss && ExpectedTotalHpLoss > 0)
        || (ShowIncomingDamage && IncomingDamage > 0);
}

internal static class ForecastHudProjectionPolicy
{
    public static ForecastHudProjection Project(
        ForecastHudSnapshot snapshot,
        DamageDisplayMode displayMode,
        IncomingDamagePlacement incomingPlacement)
    {
        var (expectedTotal, blockable, direct) = ProjectExpectedHpLoss(snapshot.ExpectedHpLoss);
        var incoming = ProjectIncomingDamage(snapshot.IncomingDamage);
        var normalizedMode = NormalizeDisplayMode(displayMode);

        return new ForecastHudProjection(
            ShowExpectedHpLoss: normalizedMode != DamageDisplayMode.IncomingDamageOnly
                && expectedTotal > 0,
            ShowIncomingDamage: normalizedMode != DamageDisplayMode.ExpectedHpLossOnly
                && incoming > 0,
            ExpectedTotalHpLoss: expectedTotal,
            ExpectedBlockableHpLoss: blockable,
            ExpectedDirectHpLoss: direct,
            IncomingDamage: incoming,
            IncomingPlacement: NormalizePlacement(incomingPlacement));
    }

    public static IncomingDamagePlacement NormalizePlacement(IncomingDamagePlacement placement)
    {
        return placement == IncomingDamagePlacement.LeftOfExpectedHpLoss
            ? IncomingDamagePlacement.LeftOfExpectedHpLoss
            : IncomingDamagePlacement.RightOfExpectedHpLoss;
    }

    private static DamageDisplayMode NormalizeDisplayMode(DamageDisplayMode mode)
    {
        return mode is DamageDisplayMode.ExpectedHpLossOnly
            or DamageDisplayMode.IncomingDamageOnly
            or DamageDisplayMode.Both
            ? mode
            : DamageDisplayMode.ExpectedHpLossOnly;
    }

    private static (int Total, int Blockable, int Direct) ProjectExpectedHpLoss(ForecastResult result)
    {
        if (result.State != ForecastResultState.KnownDamage)
        {
            return (0, 0, 0);
        }

        var blockable = Math.Max(0, result.OutDamage);
        var direct = Math.Max(0, result.DirectHpLoss);
        var total = (long)blockable + direct;
        if (total <= 0 || total > int.MaxValue)
        {
            return (0, 0, 0);
        }

        return ((int)total, blockable, direct);
    }

    private static int ProjectIncomingDamage(IncomingDamageDisplayRead read)
    {
        return read.State == IncomingDamageDisplayReadState.Known && read.Damage > 0
            ? read.Damage
            : 0;
    }
}
