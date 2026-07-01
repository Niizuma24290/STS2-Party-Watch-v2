using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Nodes.Screens.ModdingScreen;
using STS2PartyWatch.UI;

namespace STS2PartyWatch.Patches;

[HarmonyPatch(typeof(NModInfoContainer))]
internal static class PartyWatchModdingSettingsPatch
{
    private const string ModId = "sts2-party-watch-v2";
    private const string EntryButtonName = "STS2PartyWatchSettingsEntryButton";
    private const string PanelName = "STS2PartyWatchSettingsPanel";

    [HarmonyPostfix]
    [HarmonyPatch(nameof(NModInfoContainer.Fill))]
    private static void FillPostfix(NModInfoContainer __instance, Mod mod)
    {
        UpdateSettingsEntry(__instance, mod.manifest?.id == ModId);
    }

    private static void UpdateSettingsEntry(NModInfoContainer container, bool isPartyWatch)
    {
        var panel = container.GetNodeOrNull<PanelContainer>(PanelName);
        var entry = container.GetNodeOrNull<Button>(EntryButtonName);
        if (!isPartyWatch)
        {
            panel?.Hide();
            entry?.Hide();
            return;
        }

        if (panel is null)
        {
            panel = BuildSettingsPanel();
            container.AddChild(panel);
        }

        if (entry is null)
        {
            entry = new Button
            {
                Name = EntryButtonName,
                Text = "Party Watch HUD",
                MouseFilter = Control.MouseFilterEnum.Stop,
                CustomMinimumSize = new Vector2(280f, 56f)
            };
            entry.SetAnchorsPreset(Control.LayoutPreset.BottomRight);
            entry.OffsetLeft = -360f;
            entry.OffsetTop = -92f;
            entry.OffsetRight = -40f;
            entry.OffsetBottom = -32f;
            entry.Pressed += () =>
            {
                container.GetNodeOrNull<PanelContainer>(PanelName)?.Show();
                ForecastRefreshPatch.RefreshRegisteredBars();
            };
            container.AddChild(entry);
        }

        panel.Hide();
        entry.Show();
    }

    private static PanelContainer BuildSettingsPanel()
    {
        var panel = new PanelContainer
        {
            Name = PanelName,
            MouseFilter = Control.MouseFilterEnum.Stop,
            ZIndex = 200,
            Visible = false
        };
        panel.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        panel.OffsetLeft = 20f;
        panel.OffsetTop = 20f;
        panel.OffsetRight = -20f;
        panel.OffsetBottom = -20f;

        var content = new VBoxContainer
        {
            CustomMinimumSize = new Vector2(560f, 680f)
        };
        content.AddThemeConstantOverride("separation", 10);

        var header = new HBoxContainer();
        header.AddThemeConstantOverride("separation", 16);
        header.AddChild(MakeHeader("Party Watch HUD"));
        var spacer = new Control { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
        header.AddChild(spacer);
        var back = new Button
        {
            Text = "Back",
            CustomMinimumSize = new Vector2(120f, 44f)
        };
        back.Pressed += () =>
        {
            panel.Hide();
            ForecastRefreshPatch.RefreshRegisteredBars();
        };
        header.AddChild(back);
        content.AddChild(header);

        content.AddChild(MakeSeparator());
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
        var reset = new Button
        {
            Text = "Restore default settings",
            CustomMinimumSize = new Vector2(0f, 48f)
        };
        reset.Pressed += () =>
        {
            PartyWatchUiSettings.ResetDefaults();
            var parent = panel.GetParent();
            parent.RemoveChild(panel);
            panel.QueueFree();
            var replacement = BuildSettingsPanel();
            parent.AddChild(replacement);
            replacement.Show();
        };
        content.AddChild(reset);

        panel.AddChild(content);
        return panel;
    }

    private static Label MakeHeader(string text)
    {
        var label = new Label
        {
            Text = text,
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        label.AddThemeFontSizeOverride("font_size", 26);
        return label;
    }

    private static HSeparator MakeSeparator() => new();

    private static CheckBox MakeCheckBox(string text, bool value, Action<bool> setter)
    {
        var checkBox = new CheckBox
        {
            Text = text,
            ButtonPressed = value,
            MouseFilter = Control.MouseFilterEnum.Stop,
            CustomMinimumSize = new Vector2(0f, 44f)
        };
        checkBox.Toggled += pressed => setter(pressed);
        return checkBox;
    }

    private static Control MakeAnchorOption()
    {
        var row = MakeSettingsRow("Anchor preset");
        var option = new OptionButton
        {
            CustomMinimumSize = new Vector2(280f, 44f)
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
        box.AddThemeConstantOverride("separation", 4);
        var label = new Label { Text = $"{text}: {value:0}" };
        var slider = new HSlider
        {
            MinValue = min,
            MaxValue = max,
            Step = 1,
            Value = value,
            CustomMinimumSize = new Vector2(500f, 32f)
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
        var row = MakeSettingsRow(text);
        var button = new ColorPickerButton
        {
            Color = value,
            CustomMinimumSize = new Vector2(180f, 44f)
        };
        button.ColorChanged += color => setter(color);
        row.AddChild(button);
        return row;
    }

    private static HBoxContainer MakeSettingsRow(string text)
    {
        var row = new HBoxContainer();
        row.AddThemeConstantOverride("separation", 16);
        row.AddChild(new Label
        {
            Text = text,
            CustomMinimumSize = new Vector2(230f, 44f),
            VerticalAlignment = VerticalAlignment.Center,
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        });
        return row;
    }
}
