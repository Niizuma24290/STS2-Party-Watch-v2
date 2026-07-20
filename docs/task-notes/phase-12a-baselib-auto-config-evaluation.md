# Phase 12A - BaseLib Auto-Generated Config UI Evaluation

Status: Paused / investigation only.

Date: 2026-07-03.

This note records the Phase 12A work that was started and then paused at the user's request. No BaseLib config implementation was completed in this pass, no runtime screenshots were captured, no install/runtime verification was performed, and no commit was made.

## User Instruction At Pause

The user asked to record progress and stop:

```text
把任务进度记下来 你要停下来了。
```

## Scope Boundary

- Phase 11 ordinary Poison Steam verification remains deferred.
- Poison logic was not intentionally modified for Phase 12A.
- Phase 11 must not be marked complete from this work.
- Phase 12B custom right-side UI was not started.
- The old Party Watch settings route was inspected but not removed yet.

## Existing Dirty Worktree Before Phase 12A

The repository already had unrelated dirty files before Phase 12A edits:

```text
 M docs/project-state.md
 M docs/task-notes/workshop-private-rc-2026-07-01.md
 M src/STS2PartyWatchCode/Combat/LocalIncomingDamageReader.cs
 M src/STS2PartyWatchCode/Combat/VerifiedEnemyDamageModifier.cs
 M src/STS2PartyWatchCode/Combat/VerifiedTurnEndPowerDamageReader.cs
 M src/STS2PartyWatchCode/Patches/ForecastRefreshPatch.cs
?? docs/task-notes/phase-11a-v108-quick-compatibility.md
```

These were treated as pre-existing user work and were not reverted.

## CodeGraph Workflow

CodeGraph was loaded and checked against:

```text
C:\Users\ROG\Documents\Codex\STS2-Party-Watch-v2
```

Status result:

```text
Files indexed: 21
Total nodes: 356
Total edges: 613
Database size: 0.88 MB
Backend: node:sqlite
Languages: csharp
```

CodeGraph searches/exploration identified the current settings and HUD refresh path:

- `PartyWatchUiSettings`
- `PartyWatchSettingsPatch`
- `ForecastRefreshPatch`
- `PartyWatchHudVisibilityPolicy`
- `PartyWatchHudDisplay`

## Current Settings Map

Current business-facing settings API:

```text
src/STS2PartyWatchCode/UI/PartyWatchUiSettings.cs
```

Existing settings and defaults:

| Setting | Default | Current setter |
| --- | --- | --- |
| HUD enabled | `true` | `SetHudEnabled(bool)` |
| Show local HUD in multiplayer | `true` | `SetShowLocalHudInMultiplayer(bool)` |
| Show advanced shield / heart details | `false` | `SetShowBreakdownDetails(bool)` |
| Freeze HUD numbers after turn end | `true` | `SetFreezeHudWithinPlayerTurn(bool)` |
| HUD anchor | `PartyWatchHudAnchor.HealthBarRight` | `SetHudAnchor(PartyWatchHudAnchor)` |
| X offset | `0f`, clamped `-320f..320f` | `SetOffsetX(float)` |
| Y offset | `0f`, clamped `-240f..240f` | `SetOffsetY(float)` |
| Total loss color | `Colors.White` | `SetTotalLossColor(Color)` |
| Blockable detail color | `new Color(0.55f, 0.85f, 1f)` | `SetBlockableDetailColor(Color)` |
| Direct HP loss detail color | `new Color(1f, 0.55f, 0.62f)` | `SetDirectHpLossDetailColor(Color)` |

Current visible settings route:

```text
src/STS2PartyWatchCode/Patches/PartyWatchSettingsPatch.cs
```

It patches `NModInfoContainer.Fill` and adds a Party Watch-owned button/panel to the Party Watch mod info entry. This route is session-only and hand-builds controls with Godot UI nodes. It is the route Phase 12A should disable/remove once the BaseLib route is working.

## Settings Readers

Identified readers:

- `PartyWatchHudVisibilityPolicy.ShouldRenderHud(...)`
  - reads `HudEnabled`
  - reads `ShowLocalHudInMultiplayer`
- `PartyWatchHudDisplay.BuildHudDetails(...)`
  - reads `ShowBreakdownDetails`
- `PartyWatchHudDisplay.ApplyMainHudStyle(...)`
  - reads `TotalLossColor`
- `PartyWatchHudDisplay.ApplyHudPosition(...)`
  - reads `OffsetX`, `OffsetY`, `HudAnchor`
  - forces `HealthBarBelow` in multiplayer layout through the passed `forceBelowHealthBar` argument
- `PartyWatchHudDisplay.BuildForecastDetails(...)`
  - reads `BlockableDetailColor`
  - reads `DirectHpLossDetailColor`
- `ForecastRefreshPatch.Refresh(...)`
  - reads through visibility/display helpers and calls style/position methods

`FreezeHudWithinPlayerTurn` exists in `PartyWatchUiSettings` and the old settings UI, but its current reader path still needs a focused follow-up check before migration.

## Settings Writers

Current writers are the Godot controls in:

```text
src/STS2PartyWatchCode/Patches/PartyWatchSettingsPatch.cs
```

Controls found:

- checkbox for HUD enabled
- checkbox for local multiplayer HUD
- checkbox for advanced shield / heart details
- checkbox for freeze after turn end
- option button for anchor preset
- sliders for X/Y offsets
- color pickers for total loss, shield detail, heart detail
- reset button calling `PartyWatchUiSettings.ResetDefaults()`

## HUD Refresh / Invalidation Path

`ForecastRefreshPatch` subscribes to:

```csharp
PartyWatchUiSettings.Changed += RefreshRegisteredBars;
```

`RefreshRegisteredBars()` re-runs refresh on registered health bars. This is the existing live update path that a BaseLib adapter should call indirectly by updating `PartyWatchUiSettings`.

`ForecastRefreshPatch.Refresh(...)` also clears `PartyWatchHudSnapshotStore` when visibility says the HUD should not render.

## Multiplayer HUD Visibility Path

`ForecastRefreshPatch.TryGetLocalCreature(...)` restricts HUD work to the local player's creature by comparing `creature.Player.NetId` with `LocalContext.GetMe(creature.CombatState).NetId`.

`PartyWatchHudVisibilityPolicy.ShouldRenderHud(...)` then rejects multiplayer render when:

```csharp
combatState.Players.Count > 1 && !PartyWatchUiSettings.ShowLocalHudInMultiplayer
```

No teammate HUD or shared party HUD path was found in the inspected code.

## Localization Structure

No Party Watch-specific localization resource was found in the initial file list. Current labels are hard-coded English strings in `PartyWatchSettingsPatch.cs`.

BaseLib XML docs indicate labels and hover tips are localization-key based, but the exact project localization file placement still needs implementation-time verification.

## Project / Manifest

Current project:

```text
src/STS2PartyWatchCode/STS2PartyWatchCode.csproj
```

Current manifest:

```text
src/STS2PartyWatchCode/sts2-party-watch-v2.json
```

At pause time:

- no BaseLib reference had been added to the project
- manifest `dependencies` was still `[]`
- no config class or adapter had been created

## BaseLib Runtime And Package Findings

Local BaseLib runtime inspected:

```text
C:\Program Files (x86)\Steam\steamapps\workshop\content\2868840\3737335127\BaseLib
```

Runtime files:

```text
BaseLib.dll
BaseLib.json
BaseLib.pck
```

Runtime manifest:

```json
{
  "id": "BaseLib",
  "name": "BaseLib",
  "author": "Alchyr",
  "description": "Modding utility for Slay the Spire 2",
  "version": "v3.3.4",
  "has_pck": true,
  "has_dll": true,
  "dependencies": [],
  "affects_gameplay": false
}
```

Reflection against the runtime DLL reported:

```text
Assembly: BaseLib, Version=3.3.4.0, Culture=neutral, PublicKeyToken=null
```

Local NuGet cache contains:

```text
C:\Users\ROG\.nuget\packages\alchyr.sts2.baselib\3.3.2
```

NuGet package:

```text
Alchyr.Sts2.BaseLib 3.3.2
```

The local NuGet package is older than the installed Workshop runtime (`3.3.2` vs `v3.3.4`). Phase 12A implementation should resolve this before finalizing the pinned dependency, ideally by acquiring/using an exact `Alchyr.Sts2.BaseLib` `3.3.4` package if available, or by documenting the compile/runtime mismatch only if the exact package is unavailable and API compatibility has been verified.

## BaseLib Config API Findings From Runtime Reflection

Runtime `BaseLib.dll` v3.3.4 includes:

- `BaseLib.Config.SimpleModConfig`
- `BaseLib.Config.ModConfig`
- `BaseLib.Config.ModConfigRegistry`
- `BaseLib.Config.ConfigSectionAttribute`
- `BaseLib.Config.ConfigSliderAttribute`
- `BaseLib.Config.ConfigHoverTipAttribute`
- `BaseLib.Config.ConfigHoverTipsByDefaultAttribute`
- `BaseLib.Config.ConfigIgnoreAttribute`
- `BaseLib.Config.ConfigIgnoreRestoreDefaultsAttribute`
- `BaseLib.Config.ConfigTextInputAttribute`
- `BaseLib.Config.ConfigButtonAttribute`
- `BaseLib.Config.ConfigVisibleIfAttribute`
- `BaseLib.Config.ConfigColorPickerAttribute`
- `BaseLib.Config.ConfigDropdownOverrideLocalizationAttribute`

Important signatures found:

```text
ModConfigRegistry.Register(string modId, ModConfig config)
ModConfigRegistry.Get(string modId)
ModConfigRegistry.GetAll()

SimpleModConfig.SetupConfigUI(Godot.Control optionContainer)
SimpleModConfig.ClearUIEventHandlers()
SimpleModConfig.ConfirmRestoreDefaults()

ModConfig.Load()
ModConfig.Save()
ModConfig.SaveDebounced(int delayMs)
ModConfig.ConfigReloaded()
ModConfig.Changed()
ModConfig.GetDefaultValue(string propertyName)
ModConfig.HasSettings()
ModConfig.HasVisibleSettings()
ModConfig.VisibleInModList()
```

Attribute/control support found:

```text
ConfigSliderAttribute(double min, double max, double step)
  Format get/set

ConfigColorPickerAttribute()
  EditAlpha get/set
  EditIntensity get/set

ConfigHoverTipAttribute(bool enabled)
ConfigHoverTipsByDefaultAttribute()
ConfigIgnoreAttribute()
ConfigIgnoreRestoreDefaultsAttribute()
ConfigTextInputAttribute()
ConfigButtonAttribute(string buttonLabelKey)
ConfigVisibleIfAttribute(string targetName, object[] args)
ConfigDropdownOverrideLocalizationAttribute(string overridePropertyName)
```

Reflection also showed that `SimpleModConfig` has internal/nonpublic generation helpers:

```text
GenerateOptionsForAllProperties(Control targetContainer)
GenerateOptionFromProperty(PropertyInfo property)
CreateToggleOption(PropertyInfo property, bool addHoverTip)
CreateSliderOption(PropertyInfo property, bool addHoverTip)
CreateDropdownOption(PropertyInfo property, bool addHoverTip)
CreateColorPickerOption(PropertyInfo property, bool addHoverTip)
CreateButton(...)
CreateSectionHeader(...)
SetupFocusNeighbors(Control optionContainer)
```

This supports the requested automatic config baseline, but implementation was not started before pause.

## Temporary Local Work

The ignored file below was temporarily changed to reflect BaseLib APIs:

```text
work/reflect/Program.cs
```

`work/` is ignored by git and should not be committed. It can be reused or discarded later.

## Not Yet Done

- Add pinned BaseLib dependency to `.csproj`.
- Resolve exact BaseLib NuGet package version vs installed runtime version.
- Verify known-working manifest dependency format from docs or another BaseLib-dependent mod.
- Create `PartyWatchBaseLibConfig : SimpleModConfig`.
- Create `PartyWatchSettingsAdapter`.
- Register config with `ModConfigRegistry.Register(...)`.
- Preserve all existing defaults through BaseLib persistence.
- Disable/remove the old Party Watch-specific settings route.
- Add localization keys / labels / hover descriptions.
- Build.
- Install.
- Runtime verify navigation through Settings -> Modding -> Mod Configuration -> Party Watch.
- Capture screenshots.
- Verify persistence, reset, live update, multiplayer behavior, and BaseLib-absent behavior.
- Decide Recommendation A or B.
- Commit.

## Recommended Resume Point

Resume with the version/dependency decision:

1. Determine whether `Alchyr.Sts2.BaseLib` `3.3.4` is available as a package and add that exact version if possible.
2. If only package `3.3.2` is available locally, decide whether to fetch the exact package or compile against 3.3.2 while testing against runtime v3.3.4.
3. Verify manifest dependency syntax for requiring BaseLib.
4. Implement the automatic `SimpleModConfig` baseline only after the dependency/version decision is settled.

Next single task remains:

```text
Implement and runtime-evaluate the automatic BaseLib Party Watch config page, then stop for screenshot review before any Phase 12B custom UI.
```

## Follow-up Runtime Result - 2026-07-03

Later in the same Phase 12A workstream, the BaseLib automatic config baseline was implemented, built, published, installed locally, and checked in game by the user.

Runtime note:

```text
docs/task-notes/phase-12a-runtime-verification-2026-07-03.md
```

Current user-verified result:

- Main menu Mod Configuration entry opens `STS2PartyWatch`.
- The generated config controls are visible and usable from the main menu route.
- The in-combat built-in `模组配置（BaseLib）` route is visible but currently unusable; this is recorded as a known limitation and is not being fixed in Phase 12A.
- Phase 12B custom UI and Poison logic remain out of scope.
