using STS2PartyWatch.UI;

namespace STS2PartyWatch.Settings;

internal enum PartyWatchConfigLanguage
{
    English,
    SimplifiedChinese
}

internal static class PartyWatchConfigText
{
    public const string EnglishProductName = "Damage Forecast";
    public const string SimplifiedChineseProductName = "伤害预测";

    public static string ProductName(PartyWatchConfigLanguage language)
    {
        return language == PartyWatchConfigLanguage.SimplifiedChinese
            ? SimplifiedChineseProductName
            : EnglishProductName;
    }

    public static string LanguageName(PartyWatchConfigLanguage language)
    {
        return language == PartyWatchConfigLanguage.SimplifiedChinese ? "简体中文" : "English";
    }

    public static string Section(string key, PartyWatchConfigLanguage language)
    {
        var zh = language == PartyWatchConfigLanguage.SimplifiedChinese;
        return key switch
        {
            "Display" => zh ? "显示" : "Display",
            "IncomingDamage" => zh ? "来袭总伤害 N 的计算方式" : "Incoming Damage N Calculation",
            "Multiplayer" => zh ? "多人模式" : "Multiplayer",
            "PositionAndAppearance" => zh ? "位置与外观" : "Position & Appearance",
            _ => key
        };
    }

    public static string Setting(string propertyName, PartyWatchConfigLanguage language)
    {
        var zh = language == PartyWatchConfigLanguage.SimplifiedChinese;
        return propertyName switch
        {
            nameof(PartyWatchBaseLibConfig.ConfigLanguage) => zh ? "语言" : "Language",
            nameof(PartyWatchBaseLibConfig.EnablePartyWatchHud) => zh ? "启用伤害预测 HUD" : "Enable Damage Forecast HUD",
            nameof(PartyWatchBaseLibConfig.ShowAdvancedShieldHeartDetails) => zh ? "显示护盾与生命损失详情" : "Show Block & HP Loss Details",
            nameof(PartyWatchBaseLibConfig.FreezeHudNumbersAfterTurnEnd) => zh ? "回合结束后冻结 HUD 数字" : "Freeze HUD Numbers After Turn End",
            nameof(PartyWatchBaseLibConfig.DamageDisplayMode) => zh ? "伤害显示模式" : "Damage Display Mode",
            nameof(PartyWatchBaseLibConfig.IncomingDamagePlacement) => zh ? "来袭总伤害 N 的位置" : "Incoming Damage N Position",
            nameof(PartyWatchBaseLibConfig.IncludeCurrentBlockInIncomingDamage) => zh ? "计入当前护盾" : "Apply Current Block",
            nameof(PartyWatchBaseLibConfig.IncludePowerBlockInIncomingDamage) => zh ? "计入能力护盾" : "Apply Power Block",
            nameof(PartyWatchBaseLibConfig.IncludeRelicBlockInIncomingDamage) => zh ? "计入遗物护盾" : "Apply Relic Block",
            nameof(PartyWatchBaseLibConfig.IncludePowerHpLossModifiersInIncomingDamage) => zh ? "计入能力减伤" : "Apply Power Damage Reduction",
            nameof(PartyWatchBaseLibConfig.IncludeRelicHpLossModifiersInIncomingDamage) => zh ? "计入遗物减伤" : "Apply Relic Damage Reduction",
            nameof(PartyWatchBaseLibConfig.ShowLocalPlayerHudInMultiplayer) => zh ? "在多人模式中显示本机伤害预测 HUD" : "Show local-player Damage Forecast HUD in Multiplayer",
            nameof(PartyWatchBaseLibConfig.HudAnchorPreset) => zh ? "HUD 锚点位置" : "HUD Position",
            nameof(PartyWatchBaseLibConfig.HorizontalOffset) => zh ? "水平偏移" : "Horizontal Offset",
            nameof(PartyWatchBaseLibConfig.VerticalOffset) => zh ? "垂直偏移" : "Vertical Offset",
            nameof(PartyWatchBaseLibConfig.TotalExpectedLossColor) => zh ? "HUD 颜色" : "HUD Color",
            nameof(PartyWatchBaseLibConfig.ShieldDetailColor) => zh ? "护盾详情颜色" : "Block Detail Color",
            nameof(PartyWatchBaseLibConfig.HeartDetailColor) => zh ? "生命损失详情颜色" : "HP Loss Detail Color",
            _ => propertyName
        };
    }

    public static string EnumValue(string propertyName, object? value, PartyWatchConfigLanguage language)
    {
        if (value is PartyWatchConfigLanguage configLanguage)
        {
            return LanguageName(configLanguage);
        }

        var zh = language == PartyWatchConfigLanguage.SimplifiedChinese;
        if (value is DamageDisplayMode damageDisplayMode)
        {
            return damageDisplayMode switch
            {
                DamageDisplayMode.ExpectedHpLossOnly => zh ? "预计掉血（默认）" : "Expected HP Loss (Default)",
                DamageDisplayMode.IncomingDamageOnly => zh ? "来袭总伤害" : "Incoming Damage",
                DamageDisplayMode.Both => zh ? "同时显示" : "Show Both",
                _ => damageDisplayMode.ToString()
            };
        }

        if (value is IncomingDamagePlacement incomingDamagePlacement)
        {
            return incomingDamagePlacement switch
            {
                IncomingDamagePlacement.LeftOfExpectedHpLoss => zh ? "显示在预计掉血左侧" : "Left of Expected Loss",
                IncomingDamagePlacement.RightOfExpectedHpLoss => zh ? "显示在预计掉血右侧" : "Right of Expected Loss",
                _ => incomingDamagePlacement.ToString()
            };
        }

        if (value is PartyWatchHudAnchor anchor)
        {
            return anchor switch
            {
                PartyWatchHudAnchor.HealthBarRight => zh ? "血条右侧" : "Right of Health Bar",
                PartyWatchHudAnchor.HealthBarLeft => zh ? "血条左侧" : "Left of Health Bar",
                PartyWatchHudAnchor.HealthBarAbove => zh ? "血条上方" : "Above Health Bar",
                PartyWatchHudAnchor.HealthBarBelow => zh ? "血条下方" : "Below Health Bar",
                _ => anchor.ToString()
            };
        }

        return value?.ToString() ?? propertyName;
    }
}
