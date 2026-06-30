using Godot;
using STS2PartyWatch.Forecast;

namespace STS2PartyWatch.UI;

internal static class PartyWatchHudDisplay
{
    private const int MainFontSize = 26;
    private const float HealthBarRightPadding = 6f;

    public static string BuildHudDisplay(ForecastResult result)
    {
        var total = result.OutDamage + result.DirectHpLoss;
        if (total <= 0)
        {
            return string.Empty;
        }

        if (!PartyWatchUiSettings.ShowBreakdownDetails)
        {
            return $"-{total}";
        }

        var details = BuildForecastDetails(result.OutDamage, result.DirectHpLoss);
        return string.IsNullOrEmpty(details)
            ? $"-{total}"
            : $"-{total}\n{details}";
    }

    public static void ApplyHudStyle(Label label)
    {
        var showDetails = PartyWatchUiSettings.ShowBreakdownDetails;
        label.MouseFilter = Control.MouseFilterEnum.Ignore;
        label.CustomMinimumSize = new Vector2(GetWidth(showDetails), GetHeight(showDetails));
        label.Size = label.CustomMinimumSize;
        label.HorizontalAlignment = HorizontalAlignment.Left;
        label.VerticalAlignment = VerticalAlignment.Center;
        label.AddThemeFontSizeOverride("font_size", MainFontSize);
        label.AddThemeColorOverride("font_color", PartyWatchUiSettings.TotalLossColor);
        label.AddThemeColorOverride("font_shadow_color", Colors.Black);
        label.AddThemeConstantOverride("shadow_offset_x", 2);
        label.AddThemeConstantOverride("shadow_offset_y", 2);
    }

    public static void ApplyHudPosition(Control healthBar, Control label, Vector2? containerSize)
    {
        var size = containerSize ?? healthBar.Size;
        var labelSize = label.Size;
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
                healthBar.Position.Y + ((size.Y - labelSize.Y) * 0.5f))
        };

        position += new Vector2(PartyWatchUiSettings.OffsetX, PartyWatchUiSettings.OffsetY);
        label.Position = new Vector2(MathF.Max(0f, position.X), MathF.Max(0f, position.Y));
    }

    private static string BuildForecastDetails(int blockablePrediction, int directHpLossPrediction)
    {
        var details = new List<string>(2);
        if (blockablePrediction > 0)
        {
            details.Add($"\U0001F6E1 {blockablePrediction}");
        }

        if (directHpLossPrediction > 0)
        {
            details.Add($"\u2665 {directHpLossPrediction}");
        }

        return string.Join("   ", details);
    }

    private static float GetWidth(bool showDetails) => showDetails ? 240f : 84f;

    private static float GetHeight(bool showDetails) => showDetails ? 78f : 36f;
}
