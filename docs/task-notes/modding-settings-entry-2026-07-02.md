# Modding Settings Entry Fix - 2026-07-02

## Root Cause

`PartyWatchSettingsPatch` patched `NSettingsScreen` directly and reflected the native `_moddingScreenButton`.
It then added a `Party Watch HUD` button to the native Modding button's parent container, with a fallback button directly on the settings screen.
That made Party Watch look like a sibling or replacement for the native Modding entry instead of a normal mod-owned settings surface.

## Removed Native UI Occupation

- Removed the `NSettingsScreen._Ready` postfix entry injection.
- Removed `_moddingScreenButton` reflection.
- Removed Party Watch button creation in the native settings screen and the fallback top-right settings-screen entry.
- Kept the native Modding row, native Modding button text, and native button click flow untouched.

## Current Settings Entry

The patch now targets `NModInfoContainer.Fill(Mod)` in the game's native Modding screen.
When the selected mod id is `sts2-party-watch-v2`, the right-side native mod info panel shows one `Party Watch HUD` button.
Pressing it opens the existing Party Watch settings panel inside that same mod info area.

This does not add a BaseLib compile-time dependency and does not use BaseLib's config registry.
The native `Modding` text, native settings button, and native `NSettingsScreen.OpenModdingScreen` flow are no longer modified or intercepted.

## Changed Files

- `src/STS2PartyWatchCode/Patches/PartyWatchSettingsPatch.cs`: moved the settings entry from `NSettingsScreen` injection to an `NModInfoContainer.Fill(Mod)` postfix that only activates for `sts2-party-watch-v2`.
- `src/STS2PartyWatchCode/sts2-party-watch-v2.json`: changed the native mod-list display name to `Party Watch HUD`.
- `docs/task-notes/modding-settings-entry-2026-07-02.md`: recorded the root cause, removed native UI occupation, verification status, and follow-up plan.

## Preserved Settings Page Logic

The existing session settings controls are preserved:

- Enable Party Watch HUD
- Show advanced shield / heart details
- Freeze HUD numbers after turn end
- Anchor preset
- X/Y offset
- Total, shield, and heart colors
- Restore default settings

The HUD prediction, damage, relic, power, and forecast paths were not changed.

## Build Verification

- `C:\sts2\dotnet\dotnet.exe build .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore`: passed, 0 warnings, 0 errors.
- `C:\sts2\dotnet\dotnet.exe publish .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore`: passed.
- `powershell -ExecutionPolicy Bypass -File .\scripts\Install-LocalMod.ps1`: passed and installed the local mod to `C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\sts2-party-watch-v2`.
- `git diff --check`: passed; only CRLF conversion warnings from Git.
- `git status`: one unrelated pre-existing working-tree change remained in `docs/task-notes/workshop-private-rc-2026-07-01.md`.

## Runtime Verification

Not verified in Steam in this coding pass.

Shortest Steam verification path:

1. Settings -> native Modding.
2. Select `Party Watch HUD` in the native mod list.
3. Click the `Party Watch HUD` settings button in the right-side native mod info area.
4. Toggle HUD, advanced details, freeze, position, colors, and restore defaults.
5. Back returns to the native mod info area, then native back returns to Settings.
6. Enter combat and confirm HUD display plus changed settings.

Full required manual Steam checks:

1. Settings -> Modding still opens the native Modding screen.
2. Other mods remain listed and selectable.
3. `Party Watch HUD` appears once in the native mod list.
4. Selecting `Party Watch HUD` shows exactly one Party Watch settings button in the right info panel.
5. Back from the Party Watch settings panel returns to the native mod info panel; native back returns to settings.
6. Combat HUD still displays and respects changed settings.

## Commit Hash

`8df6d0a9c8f10eb9b1fedb8f5b1f61dd8f1cf49c`

## Next Single Task

Run Steam runtime validation for the native Modding flow and the combat HUD settings persistence within the current session.

## Web GPT Summary

Party Watch v2 previously occupied the native Modding settings entry because `PartyWatchSettingsPatch.cs` patched `NSettingsScreen`, reflected `_moddingScreenButton`, and inserted a `Party Watch HUD` button into the native Modding button's parent container. This made the mod look like a replacement or sibling of the game's native Modding entry.

The fix removes the `NSettingsScreen` injection entirely. The mod now leaves the native Settings and Modding button flow untouched, and only adds Party Watch's own settings button when the native Modding screen is already displaying the `sts2-party-watch-v2` mod info panel. The entry is implemented by patching `NModInfoContainer.Fill(Mod)` and checking the selected mod id.

No BaseLib dependency or BaseLib config registration is used in this version. The current route is a game-native Modding-screen info-panel entry for Party Watch itself. A future cleanup may switch to a formal BaseLib dependency and `ModConfigRegistry.Register(...)` if the project decides that shared settings UI and persistence are worth depending on BaseLib.
