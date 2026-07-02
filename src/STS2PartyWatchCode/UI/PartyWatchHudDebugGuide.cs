using Godot;

namespace STS2PartyWatch.UI;

internal static class PartyWatchHudDebugGuide
{
    private const float HealthBarCenterGuideHeight = 2f;
    private const float HealthBarCenterGuideMinWidth = 360f;
    private const float HealthBarCenterGuideRightPadding = 36f;
    private const float MainHudTextCenterGuideHeight = 2f;
    private static readonly Color HealthBarCenterGuideColor = new(0.1f, 0.95f, 1f, 0.9f);
    private static readonly Color MainHudTextCenterGuideColor = new(1f, 0.1f, 0.95f, 0.95f);

    public static void ApplyHealthBarCenterGuide(
        ColorRect guide,
        Control healthBar,
        Control mainLabel,
        Control? detailLabel,
        Vector2? containerSize)
    {
        var size = containerSize ?? healthBar.Size;
        if (size.X <= 0f || size.Y <= 0f)
        {
            guide.Hide();
            return;
        }

        var center = PartyWatchHudDisplay.GetOffsetHealthBarCenter(healthBar, size);
        var guideStartX = PartyWatchHudDisplay.GetHealthBarCenter(healthBar, size).X;
        var guideEndX = MathF.Max(
            guideStartX + HealthBarCenterGuideMinWidth,
            mainLabel.Position.X + mainLabel.Size.X + HealthBarCenterGuideRightPadding);

        if (detailLabel is not null)
        {
            guideEndX = MathF.Max(
                guideEndX,
                detailLabel.Position.X + detailLabel.Size.X + HealthBarCenterGuideRightPadding);
        }

        guide.MouseFilter = Control.MouseFilterEnum.Ignore;
        guide.Color = HealthBarCenterGuideColor;
        guide.Position = new Vector2(
            MathF.Max(0f, guideStartX),
            MathF.Max(0f, center.Y - (HealthBarCenterGuideHeight * 0.5f)));
        guide.Size = new Vector2(MathF.Max(1f, guideEndX - guideStartX), HealthBarCenterGuideHeight);
        guide.Show();
    }

    public static void ApplyMainHudTextCenterGuide(ColorRect guide, Control mainLabel)
    {
        if (mainLabel.Size.X <= 0f || mainLabel.Size.Y <= 0f)
        {
            guide.Hide();
            return;
        }

        var centerY = GetControlCenter(mainLabel).Y;
        guide.MouseFilter = Control.MouseFilterEnum.Ignore;
        guide.Color = MainHudTextCenterGuideColor;
        guide.Position = new Vector2(
            MathF.Max(0f, mainLabel.Position.X),
            MathF.Max(0f, centerY - (MainHudTextCenterGuideHeight * 0.5f)));
        guide.Size = new Vector2(MathF.Max(1f, mainLabel.Size.X), MainHudTextCenterGuideHeight);
        guide.Show();
    }

    public static string BuildAlignmentDebugLine(
        Control healthBar,
        Label mainLabel,
        ColorRect? healthBarCenterGuide,
        ColorRect? mainHudTextCenterGuide,
        Vector2? containerSize,
        bool forceBelowHealthBar)
    {
        var healthBarSize = containerSize ?? healthBar.Size;
        var rawHealthBarCenter = PartyWatchHudDisplay.GetHealthBarCenter(healthBar, healthBarSize);
        var targetCenter = PartyWatchHudDisplay.GetOffsetHealthBarCenter(healthBar, healthBarSize);
        var mainHudCenter = GetControlCenter(mainLabel);
        var measuredTextSize = PartyWatchHudDisplay.GetMeasuredTextSize(mainLabel);
        var healthGuideCenter = healthBarCenterGuide is null ? (float?)null : GetControlCenter(healthBarCenterGuide).Y;
        var textGuideCenter = mainHudTextCenterGuide is null ? (float?)null : GetControlCenter(mainHudTextCenterGuide).Y;
        var configuredAnchor = forceBelowHealthBar ? PartyWatchHudAnchor.HealthBarBelow : PartyWatchUiSettings.HudAnchor;

        return "[STS2 Party Watch][HUD Align] "
            + $"anchor={configuredAnchor} "
            + $"offset={FormatVector(new Vector2(PartyWatchUiSettings.OffsetX, PartyWatchUiSettings.OffsetY))} "
            + $"healthBar.pos={FormatVector(healthBar.Position)} "
            + $"healthBar.size={FormatVector(healthBarSize)} "
            + $"healthBar.rawCenter={FormatVector(rawHealthBarCenter)} "
            + $"targetCenter={FormatVector(targetCenter)} "
            + $"main.pos={FormatVector(mainLabel.Position)} "
            + $"main.size={FormatVector(mainLabel.Size)} "
            + $"main.center={FormatVector(mainHudCenter)} "
            + $"main.deltaY={FormatFloat(mainHudCenter.Y - targetCenter.Y)} "
            + $"text='{mainLabel.Text}' "
            + $"textSize={FormatVector(measuredTextSize)} "
            + $"cyanGuide.centerY={FormatNullableFloat(healthGuideCenter)} "
            + $"magentaGuide.centerY={FormatNullableFloat(textGuideCenter)} "
            + $"guide.deltaY={FormatNullableDelta(textGuideCenter, healthGuideCenter)} "
            + $"healthBar.global={FormatRect(healthBar.GetGlobalRect())} "
            + $"main.global={FormatRect(mainLabel.GetGlobalRect())}";
    }

    private static Vector2 GetControlCenter(Control control)
    {
        return control.Position + (control.Size * 0.5f);
    }

    private static string FormatVector(Vector2 value)
    {
        return $"({FormatFloat(value.X)},{FormatFloat(value.Y)})";
    }

    private static string FormatRect(Rect2 value)
    {
        return $"pos={FormatVector(value.Position)},size={FormatVector(value.Size)}";
    }

    private static string FormatNullableFloat(float? value)
    {
        return value.HasValue ? FormatFloat(value.Value) : "null";
    }

    private static string FormatNullableDelta(float? first, float? second)
    {
        return first.HasValue && second.HasValue ? FormatFloat(first.Value - second.Value) : "null";
    }

    private static string FormatFloat(float value)
    {
        return value.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
    }
}
