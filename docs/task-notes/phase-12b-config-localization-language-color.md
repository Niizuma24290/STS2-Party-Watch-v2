# Phase 12B - BaseLib Config Localization, Language Selector, and HUD Color

> Historical Phase 12B record: `Party Watch` was the player-facing name during this work. The current name is `Damage Forecast` / `伤害预测`; technical identifiers and this historical evidence remain unchanged. The later Phase 12C audit ledger is authoritative for confirmed open limitations.

## Status

Implemented / Built / Published / Installed for local user verification.

Runtime verification is not completed yet. Per user instruction, Codex should stop before entering the game and ask the user to verify in game.

Phase 12A is already complete at the BaseLib automatic UI baseline level. Phase 12B is the next step after Phase 12A.

No existing `phase-12b*` task note was found when this file was created, so no existing Phase 12B needed to be pushed back. If older wording describes "Phase 12B" as a fully custom right-side UI, treat that old custom-UI idea as postponed behind this task.

## User Direction

User direction on 2026-07-03:

```text
这个列为phase 12 b 如果之前有phase 12b就往后延。
具体排版还是走baselib，因为文本的问题我给你的排版是会出现错行，不对齐的问题。
```

This means:

- The downloaded `phase-12a1-config-localization-language-color.md` plan is now Phase 12B.
- Do not keep it as Phase 12A.1.
- If another Phase 12B exists later, move that other work after this task.
- Layout must continue to use BaseLib's automatic configuration page and controls.
- Do not implement the hand-drawn/custom text layout from the planning mockup, because it can produce line wrapping, row mismatch, and alignment problems.

## Product Goal

Turn the working Phase 12A BaseLib automatic config page into a more player-facing Party Watch settings page while preserving BaseLib's native layout.

Deliver exactly these improvements:

- Friendly player-facing text for Party Watch config groups, setting labels, descriptions, and enum values.
- A Party Watch-only language choice for English and Simplified Chinese, if this can be added without replacing BaseLib's page layout.
- One persisted HUD color setting in the existing BaseLib-generated appearance settings, only if the current visible color setting is not already sufficient.

## Layout Boundary

Keep BaseLib responsible for:

- fullscreen configuration shell;
- left mod list;
- Party Watch content panel;
- section/group rendering;
- setting rows;
- checkboxes;
- dropdowns;
- sliders;
- color picker;
- scrolling;
- spacing;
- focus/navigation;
- persistence and reset behavior.

Do not:

- manually rebuild the Display, Multiplayer, or Position & Appearance rows;
- copy the text-table mockup as a custom layout;
- create a custom fullscreen settings screen;
- recreate BaseLib's panel, title, scrollbar, fonts, row spacing, background, or left mod list;
- patch BaseLib globally;
- modify BaseLib source;
- affect another mod's config page.

Allowed narrow customization:

- Add a Party Watch-only language selector if BaseLib's actual API allows it cleanly.
- If top-right/header placement would require a global patch, BaseLib source modification, or manual reconstruction of all rows, stop and document the limitation instead of forcing a fragile layout.

## Required API Investigation Before Coding

Before implementation, inspect the exact BaseLib package/runtime currently used by the project, not old examples.

Confirm:

- automatic `SimpleModConfig` generation flow;
- supported attributes or metadata for group names;
- supported attributes or metadata for setting display names;
- supported description / hover text mechanism;
- enum display text or localization support;
- `ConfigColorPicker` storage type and behavior;
- hidden-from-auto-generation support, if the language value must be persisted but not shown as a normal row;
- whether `SetupConfigUI(...)` can be extended while still calling BaseLib's automatic row generation;
- whether the current Party Watch page can be refreshed after a language change without duplicating rows or affecting other mods.

If an API is missing, do not guess. Record the limitation and choose the smallest BaseLib-native alternative.

## API Findings

BaseLib version:

```text
NuGet package: Alchyr.Sts2.BaseLib 3.3.4
Runtime assembly: BaseLib, Version=3.3.4.0
```

Relevant APIs confirmed from `BaseLib.xml` and reflection against the installed runtime:

- `SimpleModConfig.SetupConfigUI(Godot.Control)` is public virtual.
- `SimpleModConfig.GenerateOptionFromProperty(PropertyInfo)` is protected and returns a standard `NConfigOptionRow`.
- `SimpleModConfig.CreateSectionHeader(string,bool)` is protected and returns a standard BaseLib section header.
- `SimpleModConfig.AddRestoreDefaultsButton(Control)` is protected.
- `SimpleModConfig.SetupFocusNeighbors(Control)` is public static.
- `NConfigOptionRow.SettingControl` is public, allowing code to distinguish the row label area from the control area.
- `ConfigColorPickerAttribute` supports `Godot.Color` and `string` HTML color values; Party Watch continues to use `Godot.Color`.
- `ConfigHideInUI` can persist values without auto-generating rows, but Phase 12B did not need it.
- `ConfigDropdownOverrideLocalizationAttribute` and `settings_ui.json` localization exist, but Party Watch remains a code-only mod with no pck localization pipeline in this task.

Implementation decision:

- Keep BaseLib row/control generation.
- Override `SetupConfigUI(...)` only to control row order and apply Party Watch-local text after BaseLib creates standard rows.
- Do not manually recreate BaseLib row layout, controls, scrollbar, panel, or mod list.
- Do not force a top-right header selector because that would require manipulating BaseLib's outer content-panel/header structure; the implemented selector is a normal BaseLib-generated row to preserve alignment.

## Language Scope

Support exactly:

- `English`
- `简体中文`

Default:

- `English`

The selected language affects only Party Watch's configuration-page text. It must not change:

- game language;
- BaseLib global language;
- another mod's configuration page;
- forecast logic;
- HUD damage calculations.

Do not add, stub, reserve, or document Japanese or Korean support in this task.

## Text Scope

Replace visible raw identifiers such as:

```text
EnablePartyWatchHud
ShowAdvancedShieldHeartDetails
FreezeHudNumbersAfterTurnEnd
ShowLocalPlayerHudInMultiplayer
HudAnchorPreset
HealthBarRight
HorizontalOffset
VerticalOffset
PositionAndAppearance
```

with friendly English / Simplified Chinese text.

Use BaseLib-supported localization or metadata mechanisms discovered during the API investigation. Do not scatter hardcoded UI strings through unrelated combat, forecast, or HUD calculation code.

## HUD Color Scope

Add or localize one real HUD color setting only.

Rules:

- Use BaseLib's supported color picker/control.
- Keep it in the BaseLib-generated Position & Appearance group.
- Preserve the current default Party Watch HUD color.
- Persist through BaseLib config, not a second JSON store.
- Changing color should update the HUD immediately or on the next normal HUD refresh.
- Reset/default restores the original color.
- Do not add themes, palettes, gradients, or unrelated appearance options.

## Known Limitation To Keep

Phase 12A verified route:

```text
Main Menu -> Mod Configuration -> STS2PartyWatch
```

Known limitation:

```text
In-combat native Settings -> Modding -> Mod Configuration / Open Configuration is visible but currently unusable.
```

Do not fix that route in Phase 12B unless the user explicitly changes scope.

## Out Of Scope

- Full custom right-side UI.
- Manual replacement of BaseLib automatic rows.
- Poison logic.
- Poison runtime matrix.
- Formal multiplayer HUD.
- Teammate HUD.
- Shared party HUD.
- Stable-release DLL initialization investigation.
- Workshop upload / public release.
- BaseLib source changes.

## Completion Standard

Phase 12B can be considered complete only after:

- API investigation is documented.
- All visible Party Watch config labels use friendly English / Simplified Chinese text where BaseLib allows it.
- No supported route shows raw C# property names or enum identifiers unless a BaseLib limitation is explicitly recorded.
- Language selection behavior is implemented or a documented BaseLib limitation explains why it cannot be cleanly placed.
- HUD color is implemented or the existing color setting is localized without duplication.
- Build passes.
- Main-menu BaseLib Mod Configuration route is runtime checked.
- The in-combat route limitation remains recorded and is not falsely claimed fixed.

## Implementation Result - 2026-07-03

Changed files:

```text
src/STS2PartyWatchCode/Settings/PartyWatchBaseLibConfig.cs
src/STS2PartyWatchCode/Settings/PartyWatchConfigText.cs
```

Implemented:

- Added persisted `ConfigLanguage` with values `English` and `SimplifiedChinese`.
- Added centralized `PartyWatchConfigText` table for English and Simplified Chinese text.
- Overrode `PartyWatchBaseLibConfig.SetupConfigUI(...)`.
- Continued to use BaseLib `GenerateOptionFromProperty(...)` for setting rows.
- Continued to use BaseLib `CreateSectionHeader(...)` for section headers.
- Continued to use BaseLib restore-defaults button and focus-neighbor setup.
- Replaced visible row/section/dropdown closed-state text after BaseLib creates the standard controls.
- Re-applies localized text after `ConfigChanged`, so changing language or HUD position updates the currently visible labels.
- Reused the existing `TotalExpectedLossColor` setting as player-facing `HUD Color`; no duplicate color setting was added.

Known implementation limitation:

- The language selector is a BaseLib standard row rather than a top-right header control. This intentionally avoids fragile manual header/panel layout work.
- Dropdown popup item text still needs runtime verification. The closed dropdown text is relabeled by Party Watch after BaseLib row generation and config changes.
- Hover descriptions are not implemented in this pass because BaseLib's documented hover path expects `settings_ui.json` localization resources; Party Watch remains code-only in this task.

Build / publish / install:

```text
C:\sts2\dotnet\dotnet.exe build .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore
Result: 0 warnings, 0 errors

C:\sts2\dotnet\dotnet.exe publish .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore -o .\work\publish\sts2-party-watch-v2
Result: publish succeeded
```

Installed locally to:

```text
C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\sts2-party-watch-v2
```

Installed files:

```text
sts2-party-watch-v2.dll
sts2-party-watch-v2.json
```

No BaseLib DLL/PCK/JSON was copied into Party Watch's mod directory.

## Runtime Verification Needed

User should verify from the already-supported route:

```text
Main Menu -> Mod Configuration -> STS2PartyWatch
```

Check:

- English labels replace raw property names.
- Change Language to `简体中文`; visible labels switch immediately.
- Change Language back to `English`; visible labels switch immediately.
- HUD Position closed dropdown text shows friendly text.
- `HUD Color` appears and controls the existing main HUD color setting.
- Existing toggles/sliders/dropdowns/color controls still operate.
- No duplicate language selector or duplicate HUD color setting appears.
- In-combat built-in BaseLib config route remains a known limitation and is not claimed fixed.

## Closeout Update - 2026-07-03

User runtime observation after the first Phase 12B install:

- Main English page labels were localized.
- Language dropdown closed value could show `简体中文`, but the opened dropdown still showed raw enum text `SimplifiedChinese`.
- In Chinese mode, the HUD position dropdown options still needed localization.

Small follow-up fix:

- Updated only `src/STS2PartyWatchCode/Settings/PartyWatchBaseLibConfig.cs`.
- Added a lightweight dropdown popup text updater under the Party Watch BaseLib config page.
- The updater replaces raw dropdown text while the page is open:
  - language dropdown: `English` / `简体中文`;
  - English HUD position dropdown: `Right of Health Bar`, `Left of Health Bar`, `Above Health Bar`, `Below Health Bar`;
  - Simplified Chinese HUD position dropdown: `血条右侧`, `血条左侧`, `血条上方`, `血条下方`.
- This keeps BaseLib's dropdown layout and controls; it does not rebuild the dropdown UI manually.

Verification after the small fix:

```text
C:\sts2\dotnet\dotnet.exe build .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore
Result: 0 warnings, 0 errors

C:\sts2\dotnet\dotnet.exe publish .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore -o .\work\publish\sts2-party-watch-v2
Result: publish succeeded

git diff --check -- src/STS2PartyWatchCode/Settings/PartyWatchBaseLibConfig.cs
Result: passed
```

Local install after the small fix:

```text
C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\sts2-party-watch-v2\sts2-party-watch-v2.dll
Timestamp: 2026-07-03 23:28:18
Size: 85504 bytes
```

BaseLib runtime source for local verification:

- The manually copied local game `mods\BaseLib` directory was removed.
- Runtime verification should now use the user's Steam Workshop subscribed BaseLib at:

```text
C:\Program Files (x86)\Steam\steamapps\workshop\content\2868840\3737335127
```

Current status:

- Implemented: yes.
- Built: yes.
- Published: yes.
- Installed locally: yes.
- RuntimeVerified for the final dropdown popup text fix: not yet. User should re-open the main-menu Mod Configuration route and check the language and HUD position dropdown popup text.

Do not claim the in-combat BaseLib configuration route is fixed. It remains a known limitation.

## Closeout Update - 2026-07-04: Dropdown Popup Localization Still Failing

User runtime verification on 2026-07-04 confirmed the dropdown popup localization is still not successful.

Observed in-game state:

- Party Watch config page opens from the main-menu BaseLib Mod Configuration route.
- Page body localization works in Simplified Chinese.
- Closed dropdown values are localized:
  - language closed value shows `简体中文`;
  - HUD position closed value shows `血条右侧`.
- Opened dropdown popup items still show raw enum identifiers:
  - language popup still shows `English` and `SimplifiedChinese`;
  - HUD position popup still shows `HealthBarRight`, `HealthBarLeft`, `HealthBarAbove`, `HealthBarBelow`.

Current conclusion:

- This is not solved.
- Do not mark Phase 12B runtime verification complete.
- Do not claim dropdown popup option text localization is fixed.
- Per user instruction, stop changing implementation for now and record findings / investigation paths only.

### Failed Fix Attempts To Preserve

Attempt 1: runtime popup text scanner.

- File touched: `src/STS2PartyWatchCode/Settings/PartyWatchBaseLibConfig.cs`.
- Added a `DropdownTextUpdater` node under the Party Watch config page.
- The updater scanned `GetTree().Root` each frame and tried to relabel:
  - `NDropdownItem.Text`;
  - `Label.Text`;
  - `RichTextLabel.Text`;
  - private `_label` / `_richLabel` text fields.
- Build and install succeeded.
- User runtime result: failed. Opened popup items still showed raw enum names.

Attempt 2: BaseLib dropdown `_items` source rewrite.

- File touched: `src/STS2PartyWatchCode/Settings/PartyWatchBaseLibConfig.cs`.
- Removed the per-frame scanner.
- Reflected into `BaseLib.Config.UI.NConfigDropdown._items`.
- Replaced each internal `NConfigDropdownItem.ItemData` with a new instance using localized `Text`, preserving `Value` and `OnSet`.
- Build and install succeeded.
- User runtime result: failed. Opened popup items still showed raw enum names.

Important note:

- The installed DLL after Attempt 2 contains the unsuccessful `_items` rewrite attempt.
- Do not assume the current source state is a working fix.
- Do not continue stacking another guess on top without first inspecting runtime behavior more directly.

### Build / Install State After Failed Attempt 2

Last locally installed files:

```text
Install directory:
C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\sts2-party-watch-v2

DLL:
C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\sts2-party-watch-v2\sts2-party-watch-v2.dll
SHA256: D3BE541A1C8C0B7F57E7AA2D4D5F48413DEA521BA2149BA7660A5E34AC8F615D
Timestamp: 2026-07-04 01:58:14
Size: 87552 bytes

JSON:
C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\sts2-party-watch-v2\sts2-party-watch-v2.json
SHA256: A12AE8A5FD44292DFF347350CEB64B005C54DE82D5155F5CE1B50A2F6C6C96BA
```

Verification commands run:

```text
work\.dotnet\dotnet.exe build src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore
Result: 0 warnings, 0 errors

work\.dotnet\dotnet.exe publish src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-build -o work\publish\sts2-party-watch-v2
Result: publish succeeded

git diff --check -- src\STS2PartyWatchCode\Settings\PartyWatchBaseLibConfig.cs
Result: passed
```

### Investigation Paths Already Checked

Project source:

```text
C:\Users\ROG\Documents\Codex\STS2-Party-Watch-v2\src\STS2PartyWatchCode\Settings\PartyWatchBaseLibConfig.cs
C:\Users\ROG\Documents\Codex\STS2-Party-Watch-v2\src\STS2PartyWatchCode\Settings\PartyWatchConfigText.cs
```

BaseLib package / runtime paths:

```text
NuGet package:
C:\Users\ROG\.nuget\packages\alchyr.sts2.baselib\3.3.4

NuGet runtime DLL:
C:\Users\ROG\.nuget\packages\alchyr.sts2.baselib\3.3.4\lib\net9.0\BaseLib.dll

Steam Workshop BaseLib used for local runtime:
C:\Program Files (x86)\Steam\steamapps\workshop\content\2868840\3737335127
```

Temporary reflection helper:

```text
C:\Users\ROG\Documents\Codex\STS2-Party-Watch-v2\work\reflect\Program.cs
C:\Users\ROG\Documents\Codex\STS2-Party-Watch-v2\work\reflect\Reflect.csproj
```

Build environment note:

```text
System dotnet at C:\Program Files\dotnet\dotnet.exe had no SDK.
Usable SDK was:
C:\Users\ROG\Documents\Codex\STS2-Party-Watch-v2\work\.dotnet\dotnet.exe
SDK: 9.0.315

NuGet.Config references `.local-nuget`.
The missing `.local-nuget` directory was recreated for restore/build.
The BaseLib 3.3.4 nupkg was copied from:
C:\Users\ROG\.nuget\packages\alchyr.sts2.baselib\3.3.4\alchyr.sts2.baselib.3.3.4.nupkg
to:
C:\Users\ROG\Documents\Codex\STS2-Party-Watch-v2\.local-nuget\alchyr.sts2.baselib.3.3.4.nupkg
```

### Reflection Findings

Reflection against `BaseLib.dll` 3.3.4 and `sts2.dll` showed:

```text
BaseLib.Config.UI.NConfigDropdown
Base type: MegaCrit.Sts2.Core.Nodes.Screens.Settings.NSettingsDropdown
Important fields:
- private BaseLib.Config.ModConfig _config
- private System.Reflection.PropertyInfo _property
- private List<NConfigDropdownItem.ItemData> _items
- private int _currentDisplayIndex
Important methods:
- Initialize(ModConfig config, PropertyInfo property, string modPrefix, Action onChanged)
- SetFromProperty()
- OnDropdownItemSelected(NDropdownItem nDropdownItem)
- _Ready()
- _Process(double delta)

BaseLib.Config.UI.NConfigDropdownItem
Base type: MegaCrit.Sts2.Core.Nodes.CommonUi.NDropdownItem
Important fields:
- public ItemData Data
- public int DisplayIndex
Important methods:
- static Create(ItemData data)
- Init(int setIndex)

BaseLib.Config.UI.NConfigDropdownItem.ItemData
Constructor:
- ItemData(string text, object value, Action onSet)
Properties:
- string Text, getter only
- object Value, getter only
- Action OnSet, getter only

MegaCrit.Sts2.Core.Nodes.CommonUi.NDropdownItem
Important fields:
- private MegaLabel _label
- private MegaRichTextLabel _richLabel
Important property:
- public string Text { get; set; }

MegaCrit.Sts2.Core.Nodes.GodotExtensions.NDropdown
Important fields:
- private MegaLabel _currentOptionLabel
- private Control _dropdownContainer
- private Control _dropdownItems
- private bool _isOpen
Important methods:
- OpenDropdown()
- ClearDropdownItems()
- CloseDropdown()
```

Likely implication:

- Closed value text is being localized because Damage Forecast can reach the already-created closed dropdown label/control.
- Opened popup text is generated or refreshed through BaseLib / game dropdown internals after Damage Forecast's current relabeling attempts.
- The popup may be using cloned scenes or a later call path that reuses enum names after `_items` was rewritten, or the rewrite may be happening too early/against a different dropdown instance than the one that opens.
- The next investigation should not guess another text setter. It should inspect/log the actual runtime nodes and call timing when the dropdown opens.

### Suggested Next Investigation, Not Yet Done

Only if the user reopens this task:

- Add temporary diagnostic logging, not a permanent fix, around the Damage Forecast config page:
  - log each `NConfigDropdown` found under `SettingControl`;
  - log `_items[i].Text` before and after localization;
  - log whether `_items` is later recreated by `_Ready`, `SetFromProperty`, or dropdown open;
  - log actual popup child node types under `NDropdown._dropdownItems` when `_isOpen == true`.
- Consider Harmony patching `BaseLib.Config.UI.NConfigDropdownItem.Init(int)` or `NConfigDropdown.Initialize(...)` only after runtime evidence shows the exact timing.
- Prefer using BaseLib's official `settings_ui.json` localization route if a code-only workaround remains brittle.
- Keep this scoped to Damage Forecast; do not patch BaseLib globally unless there is no BaseLib-native option and the user explicitly approves.

### Current Stop Point

Stop here for now.

User instruction:

```text
先别改了，把目前发现的问题和检查路径都记录下来。然后先收口。
```

Current status:

```text
Phase 12B body / closed dropdown localization: partially working.
Phase 12B opened dropdown popup item localization: failing.
Runtime verified complete: no.
Implementation work: paused by user instruction.
```
