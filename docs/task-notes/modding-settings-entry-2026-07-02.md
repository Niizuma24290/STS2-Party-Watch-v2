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
- `git diff --check`: passed; only CRLF conversion warnings from Git.

## Runtime Verification

Not verified in Steam in this coding pass.

Required manual Steam checks:

1. Settings -> Modding still opens the native Modding screen.
2. Other mods remain listed and selectable.
3. `Party Watch HUD` appears once in the native mod list.
4. Selecting `Party Watch HUD` shows exactly one Party Watch settings button in the right info panel.
5. Back from the Party Watch settings panel returns to the native mod info panel; native back returns to settings.
6. Combat HUD still displays and respects changed settings.

## Commit Hash

To be reported after the final commit for this task.

## Next Single Task

Run Steam runtime validation for the native Modding flow and the combat HUD settings persistence within the current session.
