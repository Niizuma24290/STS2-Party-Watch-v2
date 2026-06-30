using Godot;
using STS2PartyWatch.Forecast;

namespace STS2PartyWatch.UI;

internal static class PartyWatchHudDisplay
{
    private const float DetailFontRatio = 0.62f;

    public static string BuildHudDisplay(ForecastResult result)
    {
        var total = result.OutDamage + result.DirectHpLoss;
        if (total <= 0)
        {
            return string.Empty;
        }

        if (!PartyWatchUiSettings.ShowBreakdownDetails)
        {
            return $"[center]-{total}[/center]";
        }

        var details = BuildForecastDetails(result.OutDamage, result.DirectHpLoss);
        return string.IsNullOrEmpty(details)
            ? $"[center]-{total}[/center]"
            : $"[center]-{total}\n[font_size={GetDetailFontSize()}]{details}[/font_size][/center]";
    }

    public static void ApplyHudStyle(RichTextLabel label)
    {
        var showDetails = PartyWatchUiSettings.ShowBreakdownDetails;
        label.BbcodeEnabled = true;
        label.FitContent = false;
        label.ScrollActive = false;
        label.MouseFilter = Control.MouseFilterEnum.Ignore;
        label.CustomMinimumSize = new Vector2(GetWidth(showDetails), GetHeight(showDetails));
        label.Size = label.CustomMinimumSize;
        label.AddThemeFontSizeOverride("normal_font_size", 32);
        label.AddThemeColorOverride("default_color", PartyWatchUiSettings.TotalLossColor);
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
                healthBar.Position.X + size.X + 42f,
                healthBar.Position.Y + MathF.Max(0f, (size.Y - labelSize.Y) * 0.5f))
        };

        position += new Vector2(PartyWatchUiSettings.OffsetX, PartyWatchUiSettings.OffsetY);
        label.Position = new Vector2(MathF.Max(0f, position.X), MathF.Max(0f, position.Y));
    }

    private static string BuildForecastDetails(int blockablePrediction, int directHpLossPrediction)
    {
        var details = new List<string>(2);
        if (blockablePrediction > 0)
        {
            details.Add($"[color=#{PartyWatchUiSettings.BlockableDetailColor.ToHtml(false)}]🛡 {blockablePrediction}[/color]");
        }

        if (directHpLossPrediction > 0)
        {
            details.Add($"[color=#{PartyWatchUiSettings.DirectHpLossDetailColor.ToHtml(false)}]♥ {directHpLossPrediction}[/color]");
        }

        return string.Join("   ", details);
    }

    private static int GetDetailFontSize() => Math.Max(18, (int)(32 * DetailFontRatio));

    private static float GetWidth(bool showDetails) => showDetails ? 240f : 108f;

    private static float GetHeight(bool showDetails) => showDetails ? 78f : 44f;
}
