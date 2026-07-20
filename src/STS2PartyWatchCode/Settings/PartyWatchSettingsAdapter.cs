using STS2PartyWatch.UI;

namespace STS2PartyWatch.Settings;

internal static class PartyWatchSettingsAdapter
{
    private static PartyWatchBaseLibConfig? _config;

    public static void Bind(PartyWatchBaseLibConfig config)
    {
        if (_config is not null)
        {
            _config.ConfigChanged -= ConfigChanged;
        }

        _config = config;
        _config.ConfigChanged += ConfigChanged;
    }

    public static void ApplyCurrent()
    {
        PartyWatchUiSettings.Apply(
            hudEnabled: PartyWatchBaseLibConfig.EnablePartyWatchHud,
            showLocalHudInMultiplayer: PartyWatchBaseLibConfig.ShowLocalPlayerHudInMultiplayer,
            showBreakdownDetails: PartyWatchBaseLibConfig.ShowAdvancedShieldHeartDetails,
            freezeHudWithinPlayerTurn: PartyWatchBaseLibConfig.FreezeHudNumbersAfterTurnEnd,
            damageDisplayMode: PartyWatchBaseLibConfig.DamageDisplayMode,
            incomingDamagePlacement: PartyWatchBaseLibConfig.IncomingDamagePlacement,
            includeCurrentBlockInIncomingDamage: PartyWatchBaseLibConfig.IncludeCurrentBlockInIncomingDamage,
            includePowerBlockInIncomingDamage: PartyWatchBaseLibConfig.IncludePowerBlockInIncomingDamage,
            includeRelicBlockInIncomingDamage: PartyWatchBaseLibConfig.IncludeRelicBlockInIncomingDamage,
            includePowerHpLossModifiersInIncomingDamage: PartyWatchBaseLibConfig.IncludePowerHpLossModifiersInIncomingDamage,
            includeRelicHpLossModifiersInIncomingDamage: PartyWatchBaseLibConfig.IncludeRelicHpLossModifiersInIncomingDamage,
            hudAnchor: PartyWatchBaseLibConfig.HudAnchorPreset,
            offsetX: PartyWatchBaseLibConfig.HorizontalOffset,
            offsetY: PartyWatchBaseLibConfig.VerticalOffset,
            totalLossColor: PartyWatchBaseLibConfig.TotalExpectedLossColor,
            blockableDetailColor: PartyWatchBaseLibConfig.ShieldDetailColor,
            directHpLossDetailColor: PartyWatchBaseLibConfig.HeartDetailColor);
    }

    private static void ConfigChanged(object? sender, EventArgs e)
    {
        ApplyCurrent();
    }
}
