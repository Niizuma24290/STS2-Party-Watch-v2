using Godot;
using STS2PartyWatch.Forecast;

namespace STS2PartyWatch.UI;

public sealed partial class ForecastHudView : Control
{
    private readonly Label _label = new();

    public override void _Ready()
    {
        MouseFilter = MouseFilterEnum.Ignore;
        CustomMinimumSize = new Vector2(140, 42);

        _label.MouseFilter = MouseFilterEnum.Ignore;
        _label.AddThemeFontSizeOverride("font_size", 30);
        _label.AddThemeColorOverride("font_color", Colors.White);
        _label.AddThemeColorOverride("font_shadow_color", Colors.Black);
        _label.AddThemeConstantOverride("shadow_offset_x", 2);
        _label.AddThemeConstantOverride("shadow_offset_y", 2);
        _label.Position = Vector2.Zero;
        _label.Size = CustomMinimumSize;

        AddChild(_label);
        Hide();
    }

    public void Apply(ForecastResult result)
    {
        if (result.State != ForecastResultState.KnownDamage || result.OutDamage <= 0)
        {
            _label.Text = string.Empty;
            Hide();
            return;
        }

        _label.Text = $"🛡 -{result.OutDamage}";
        Show();
    }
}
