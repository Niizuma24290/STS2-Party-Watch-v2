namespace DamageForecast.Compatibility;

internal static class PreDamageForecastSchemaV1
{
    public static readonly ConfigSchemaDescriptor Descriptor = new(
        Id: LegacyIdentityDescriptor.SchemaId,
        ConfigFileName: LegacyIdentityDescriptor.ConfigFileName,
        OrderedKeys:
        [
            "ConfigLanguage",
            LegacyIdentityDescriptor.HudEnabledKey,
            "ShowAdvancedShieldHeartDetails",
            "FreezeHudNumbersAfterTurnEnd",
            "DamageDisplayMode",
            "IncomingDamagePlacement",
            "IncludeCurrentBlockInIncomingDamage",
            "IncludePowerBlockInIncomingDamage",
            "IncludeRelicBlockInIncomingDamage",
            "IncludePowerHpLossModifiersInIncomingDamage",
            "IncludeRelicHpLossModifiersInIncomingDamage",
            "ShowLocalPlayerHudInMultiplayer",
            "HudAnchorPreset",
            "HorizontalOffset",
            "VerticalOffset",
            "TotalExpectedLossColor",
            "ShieldDetailColor",
            "HeartDetailColor"
        ],
        HudEnabledKey: LegacyIdentityDescriptor.HudEnabledKey,
        IsCurrent: false);
}
