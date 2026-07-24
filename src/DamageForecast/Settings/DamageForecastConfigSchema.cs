using System.Globalization;
using DamageForecast.UI;

namespace DamageForecast.Settings;

internal readonly record struct DamageForecastRgba(float R, float G, float B, float A)
{
    public IEnumerable<float> Components => [R, G, B, A];

    public string ToCanonicalString() => string.Join(",", Components.Select(Format));

    private static string Format(float value) => value.ToString("R", CultureInfo.InvariantCulture);
}

internal sealed record DamageForecastConfigSnapshot(
    DamageForecastConfigLanguage ConfigLanguage,
    bool EnableDamageForecastHud,
    bool ShowAdvancedShieldHeartDetails,
    bool FreezeHudNumbersAfterTurnEnd,
    DamageDisplayMode DamageDisplayMode,
    IncomingDamagePlacement IncomingDamagePlacement,
    bool IncludeCurrentBlockInIncomingDamage,
    bool IncludePowerBlockInIncomingDamage,
    bool IncludeRelicBlockInIncomingDamage,
    bool IncludePowerHpLossModifiersInIncomingDamage,
    bool IncludeRelicHpLossModifiersInIncomingDamage,
    bool ShowLocalPlayerHudInMultiplayer,
    DamageForecastHudAnchor HudAnchorPreset,
    float HorizontalOffset,
    float VerticalOffset,
    DamageForecastRgba TotalExpectedLossColor,
    DamageForecastRgba ShieldDetailColor,
    DamageForecastRgba HeartDetailColor)
{
    public string ToCanonicalString()
    {
        return string.Join("\n",
        [
            ConfigLanguage.ToString(),
            EnableDamageForecastHud ? "true" : "false",
            ShowAdvancedShieldHeartDetails ? "true" : "false",
            FreezeHudNumbersAfterTurnEnd ? "true" : "false",
            DamageDisplayMode.ToString(),
            IncomingDamagePlacement.ToString(),
            IncludeCurrentBlockInIncomingDamage ? "true" : "false",
            IncludePowerBlockInIncomingDamage ? "true" : "false",
            IncludeRelicBlockInIncomingDamage ? "true" : "false",
            IncludePowerHpLossModifiersInIncomingDamage ? "true" : "false",
            IncludeRelicHpLossModifiersInIncomingDamage ? "true" : "false",
            ShowLocalPlayerHudInMultiplayer ? "true" : "false",
            HudAnchorPreset.ToString(),
            HorizontalOffset.ToString("R", CultureInfo.InvariantCulture),
            VerticalOffset.ToString("R", CultureInfo.InvariantCulture),
            TotalExpectedLossColor.ToCanonicalString(),
            ShieldDetailColor.ToCanonicalString(),
            HeartDetailColor.ToCanonicalString()
        ]);
    }
}

internal static class DamageForecastConfigSchema
{
    public static readonly string[] PropertyOrder =
    [
        nameof(DamageForecastBaseLibConfig.ConfigLanguage),
        nameof(DamageForecastBaseLibConfig.EnableDamageForecastHud),
        nameof(DamageForecastBaseLibConfig.ShowAdvancedShieldHeartDetails),
        nameof(DamageForecastBaseLibConfig.FreezeHudNumbersAfterTurnEnd),
        nameof(DamageForecastBaseLibConfig.DamageDisplayMode),
        nameof(DamageForecastBaseLibConfig.IncomingDamagePlacement),
        nameof(DamageForecastBaseLibConfig.IncludeCurrentBlockInIncomingDamage),
        nameof(DamageForecastBaseLibConfig.IncludePowerBlockInIncomingDamage),
        nameof(DamageForecastBaseLibConfig.IncludeRelicBlockInIncomingDamage),
        nameof(DamageForecastBaseLibConfig.IncludePowerHpLossModifiersInIncomingDamage),
        nameof(DamageForecastBaseLibConfig.IncludeRelicHpLossModifiersInIncomingDamage),
        nameof(DamageForecastBaseLibConfig.ShowLocalPlayerHudInMultiplayer),
        nameof(DamageForecastBaseLibConfig.HudAnchorPreset),
        nameof(DamageForecastBaseLibConfig.HorizontalOffset),
        nameof(DamageForecastBaseLibConfig.VerticalOffset),
        nameof(DamageForecastBaseLibConfig.TotalExpectedLossColor),
        nameof(DamageForecastBaseLibConfig.ShieldDetailColor),
        nameof(DamageForecastBaseLibConfig.HeartDetailColor)
    ];
}
