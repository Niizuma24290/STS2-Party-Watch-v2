using Godot;
using STS2PartyWatch.Forecast;

namespace STS2PartyWatch.UI;

internal static class PartyWatchHudDisplay
{
    private const int MainFontSize = 24;
    private const int DetailFontSize = 18;
    private const int DetailShieldFontSize = 15;
    private const int DetailHeartFontSize = 22;
    private const float HealthBarRightPadding = 6f;
    private const float HealthBarRightVerticalNudge = -40f;
    private const float DetailHorizontalGap = 48f;
    private const float GuideThickness = 1f;

    public static string BuildMainHudDisplay(ForecastResult result)
    {
        var total = result.OutDamage + result.DirectHpLoss;
        return total <= 0 ? string.Empty : $"-{total}";
    }

    public static string BuildHudDetails(ForecastResult result)
    {
        return PartyWatchUiSettings.ShowBreakdownDetails
            ? BuildForecastDetails(result.OutDamage, result.DirectHpLoss)
            : string.Empty;
    }

    public static void ApplyMainHudStyle(Label label)
    {
        label.MouseFilter = Control.MouseFilterEnum.Ignore;
        label.CustomMinimumSize = new Vector2(GetMainWidth(), GetMainHeight());
        label.Size = label.CustomMinimumSize;
        label.HorizontalAlignment = HorizontalAlignment.Left;
        label.VerticalAlignment = VerticalAlignment.Center;
        label.AddThemeFontSizeOverride("font_size", MainFontSize);
        label.AddThemeColorOverride("font_color", PartyWatchUiSettings.TotalLossColor);
        label.AddThemeColorOverride("font_shadow_color", Colors.Black);
        label.AddThemeConstantOverride("shadow_offset_x", 2);
        label.AddThemeConstantOverride("shadow_offset_y", 2);
    }

    public static void ApplyDetailHudStyle(RichTextLabel label)
    {
        label.MouseFilter = Control.MouseFilterEnum.Ignore;
        label.BbcodeEnabled = true;
        label.ScrollActive = false;
        label.AutowrapMode = TextServer.AutowrapMode.Off;
        label.CustomMinimumSize = new Vector2(GetDetailWidth(), GetDetailHeight());
        label.Size = label.CustomMinimumSize;
        label.AddThemeFontSizeOverride("font_size", DetailFontSize);
        label.AddThemeColorOverride("font_shadow_color", Colors.Black);
        label.AddThemeConstantOverride("shadow_offset_x", 2);
        label.AddThemeConstantOverride("shadow_offset_y", 2);
    }

    public static void ApplyHudPosition(Control healthBar, Control mainLabel, Control? detailLabel, Vector2? containerSize)
    {
        var size = containerSize ?? healthBar.Size;
        var labelSize = mainLabel.Size;
        var position = PartyWatchUiSettings.HudAnchor switch
        {
            PartyWatchHudAnchor.HealthBarLeft => new Vector2(
                healthBar.Position.X - labelSize.X - 22f,
                healthBar.Position.Y + MathF.Max(0f, (size.Y - labelSize.Y) * 0.5f)),
            PartyWatchHudAnchor.HealthBarAbove => new Vector2(
                healthBar.Position.X + MathF.Max(0f, (size.X - labelSize.X) * 0.5f),
                healthBar.Position.Y - labelSize.Y - 14f),
            PartyWatchHudAnchor.HealthBarBelow => new Vector2(
                healthBar.Position.X + MathF.Max(0f, (size.X - labelSize.X) * 0.5f),
                healthBar.Position.Y + size.Y + 14f),
            _ => new Vector2(
                healthBar.Position.X + size.X + HealthBarRightPadding,
                healthBar.Position.Y + ((size.Y - labelSize.Y) * 0.5f) + HealthBarRightVerticalNudge)
        };

        position += new Vector2(PartyWatchUiSettings.OffsetX, PartyWatchUiSettings.OffsetY);
        mainLabel.Position = new Vector2(MathF.Max(0f, position.X), MathF.Max(0f, position.Y));

        if (detailLabel is not null)
        {
            detailLabel.Position = new Vector2(
                mainLabel.Position.X + DetailHorizontalGap,
                mainLabel.Position.Y + ((mainLabel.Size.Y - detailLabel.Size.Y) * 0.5f));
        }
    }

    public static void ApplyAlignmentGuide(
        Control healthBar,
        Control mainLabel,
        ColorRect barCenterGuide,
        ColorRect hudCenterGuide,
        Vector2? containerSize)
    {
        var size = containerSize ?? healthBar.Size;
        barCenterGuide.MouseFilter = Control.MouseFilterEnum.Ignore;
        barCenterGuide.Color = new Color(0f, 1f, 1f, 0.85f);
        barCenterGuide.Position = new Vector2(
            healthBar.Position.X,
            healthBar.Position.Y + (size.Y * 0.5f) - (GuideThickness * 0.5f));
        barCenterGuide.Size = new Vector2(size.X + HealthBarRightPadding + mainLabel.Size.X, GuideThickness);
        barCenterGuide.ZIndex = 80;

        hudCenterGuide.MouseFilter = Control.MouseFilterEnum.Ignore;
        hudCenterGuide.Color = new Color(1f, 0f, 1f, 0.85f);
        hudCenterGuide.Position = new Vector2(
            mainLabel.Position.X,
            mainLabel.Position.Y + (mainLabel.Size.Y * 0.5f) - (GuideThickness * 0.5f));
        hudCenterGuide.Size = new Vector2(mainLabel.Size.X, GuideThickness);
        hudCenterGuide.ZIndex = 81;
    }

    private static string BuildForecastDetails(int blockablePrediction, int directHpLossPrediction)
    {
        var details = new List<string>(2);
        if (blockablePrediction > 0)
        {
            details.Add($"[color={ToHtml(PartyWatchUiSettings.BlockableDetailColor)}][font_size={DetailShieldFontSize}]\U0001F6E1[/font_size] [font_size={DetailFontSize}]{blockablePrediction}[/font_size][/color]");
        }

        if (directHpLossPrediction > 0)
        {
            details.Add($"[color={ToHtml(PartyWatchUiSettings.DirectHpLossDetailColor)}][font_size={DetailHeartFontSize}]\u2665[/font_size] [font_size={DetailFontSize}]{directHpLossPrediction}[/font_size][/color]");
        }

        return string.Join("   ", details);
    }

    private static float GetMainWidth() => 72f;

    private static float GetMainHeight() => 34f;

    private static float GetDetailWidth() => 240f;

    private static float GetDetailHeight() => 28f;

    private static string ToHtml(Color color)
    {
        var r = ToByte(color.R);
        var g = ToByte(color.G);
        var b = ToByte(color.B);
        return $"#{r:X2}{g:X2}{b:X2}";
    }

    private static int ToByte(float value)
    {
        return (int)MathF.Round(Math.Clamp(value, 0f, 1f) * 255f);
    }
}
