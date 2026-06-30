using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using STS2PartyWatch.UI;

namespace STS2PartyWatch.Patches;

[HarmonyPatch(typeof(NMainMenu))]
internal static class PartyWatchSettingsPatch
{
    private const string PanelName = "STS2PartyWatchSettingsPanel";

    [HarmonyPostfix]
    [HarmonyPatch("_Ready")]
    private static void ReadyPostfix(NMainMenu __instance)
    {
        AddSettingsPanel(__instance);
    }

    private static void AddSettingsPanel(Control screen)
    {
        if (screen.GetNodeOrNull<PanelContainer>(PanelName) is not null)
        {
            return;
        }

        var panel = new PanelContainer
        {
            Name = PanelName,
            MouseFilter = Control.MouseFilterEnum.Stop,
            ZIndex = 100
        };
        panel.SetAnchorsPreset(Control.LayoutPreset.TopRight);
        panel.OffsetLeft = -580f;
        panel.OffsetTop = 90f;
        panel.OffsetRight = -56f;
        panel.OffsetBottom = 760f;

        var content = new VBoxContainer
        {
            CustomMinimumSize = new Vector2(500f, 640f)
        };
        content.AddThemeConstantOverride("separation", 8);

        content.AddChild(MakeHeader("Party Watch HUD"));
        content.AddChild(MakeCheckBox(
            "Enable Party Watch HUD",
            PartyWatchUiSettings.HudEnabled,
            PartyWatchUiSettings.SetHudEnabled));
        content.AddChild(MakeCheckBox(
            "Show advanced shield / heart details",
            PartyWatchUiSettings.ShowBreakdownDetails,
            PartyWatchUiSettings.SetShowBreakdownDetails));
        content.AddChild(MakeCheckBox(
            "Freeze HUD numbers after turn end",
            PartyWatchUiSettings.FreezeHudWithinPlayerTurn,
            PartyWatchUiSettings.SetFreezeHudWithinPlayerTurn));

        content.AddChild(MakeSeparator());
        content.AddChild(MakeHeader("Position"));
        content.AddChild(MakeAnchorOption());
        content.AddChild(MakeSlider("X offset", -320, 320, PartyWatchUiSettings.OffsetX, PartyWatchUiSettings.SetOffsetX));
        content.AddChild(MakeSlider("Y offset", -240, 240, PartyWatchUiSettings.OffsetY, PartyWatchUiSettings.SetOffsetY));

        content.AddChild(MakeSeparator());
        content.AddChild(MakeHeader("Colors"));
        content.AddChild(MakeColorButton("Total expected loss", PartyWatchUiSettings.TotalLossColor, PartyWatchUiSettings.SetTotalLossColor));
        content.AddChild(MakeColorButton("Shield detail", PartyWatchUiSettings.BlockableDetailColor, PartyWatchUiSettings.SetBlockableDetailColor));
        content.AddChild(MakeColorButton("Heart detail", PartyWatchUiSettings.DirectHpLossDetailColor, PartyWatchUiSettings.SetDirectHpLossDetailColor));

        content.AddChild(MakeSeparator());
        var reset = new Button { Text = "Restore default settings" };
        reset.Pressed += () =>
        {
            PartyWatchUiSettings.ResetDefaults();
            panel.GetParent()?.RemoveChild(panel);
            panel.QueueFree();
            AddSettingsPanel(screen);
        };
        content.AddChild(reset);

        panel.AddChild(content);
        screen.AddChild(panel);
    }

    private static Label MakeHeader(string text)
    {
        var label = new Label { Text = text };
        label.AddThemeFontSizeOverride("font_size", 22);
        return label;
    }

    private static HSeparator MakeSeparator() => new();

    private static CheckBox MakeCheckBox(string text, bool value, Action<bool> setter)
    {
        var checkBox = new CheckBox
        {
            Text = text,
            ButtonPressed = value,
            MouseFilter = Control.MouseFilterEnum.Stop
        };
        checkBox.Toggled += pressed => setter(pressed);
        return checkBox;
    }

    private static Control MakeAnchorOption()
    {
        var row = new HBoxContainer();
        row.AddThemeConstantOverride("separation", 12);
        row.AddChild(new Label
        {
            Text = "Anchor preset",
            CustomMinimumSize = new Vector2(180f, 0f)
        });

        var option = new OptionButton
        {
            CustomMinimumSize = new Vector2(260f, 0f)
        };
        option.AddItem("Health bar right");
        option.AddItem("Health bar left");
        option.AddItem("Health bar above");
        option.AddItem("Health bar below");
        option.Select((int)PartyWatchUiSettings.HudAnchor);
        option.ItemSelected += index => PartyWatchUiSettings.SetHudAnchor((PartyWatchHudAnchor)index);
        row.AddChild(option);
        return row;
    }

    private static Control MakeSlider(string text, double min, double max, float value, Action<float> setter)
    {
        var box = new VBoxContainer();
        var label = new Label { Text = $"{text}: {value:0}" };
        var slider = new HSlider
        {
            MinValue = min,
            MaxValue = max,
            Step = 1,
            Value = value,
            CustomMinimumSize = new Vector2(450f, 0f)
        };
        slider.ValueChanged += newValue =>
        {
            label.Text = $"{text}: {newValue:0}";
            setter((float)newValue);
        };
        box.AddChild(label);
        box.AddChild(slider);
        return box;
    }

    private static Control MakeColorButton(string text, Color value, Action<Color> setter)
    {
        var row = new HBoxContainer();
        row.AddThemeConstantOverride("separation", 12);
        row.AddChild(new Label
        {
            Text = text,
            CustomMinimumSize = new Vector2(220f, 0f)
        });
        var button = new ColorPickerButton
        {
            Color = value,
            CustomMinimumSize = new Vector2(160f, 0f)
        };
        button.ColorChanged += color => setter(color);
        row.AddChild(button);
        return row;
    }
}
