# Phase 9A follow-up - 2026-07-01

## Actual Changes

- Narrowed `ShouldRenderHud()` again after live screenshots showed Party Watch still hidden in normal combat.
- The root cause is likely that always-present UI stack nodes, especially `NModalContainer` and `NOverlayStack`, can be visible even when no real modal/overlay page is open. Those empty containers are now ignored; their real popup/loading/settings descendants can still hide HUD.
- Reverted the combat HUD renderer from `RichTextLabel`/BBCode to the previously verified plain `Label` path.
- Moved the Party Watch settings panel out of in-run `NSettingsScreen` and onto `NMainMenu._Ready`, so combat/pause settings no longer contain the Party Watch panel.

## Defaults And Limitations

- Main HUD font size is still `32`.
- Default position remains `Health bar above` with `X offset = 24`, `Y offset = 0`.
- Advanced detail colors are still present in settings, but this immediate visibility fallback uses a single plain `Label`, so per-detail colors should be restored with a multi-label HUD view after the user confirms the HUD is visible again.
- Settings are still session-only; no verified official per-mod persistence API has been wired.

## Verification

- `C:\sts2\dotnet\dotnet.exe build .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore` passed with 0 warnings and 0 errors after the code changes.
- Steam runtime verification is still pending user test after publish/install.

## Next Step

Confirm in Steam that the Party Watch `-N` label appears in combat, then tune HUD position/spacing and replace the temporary single-label advanced detail renderer with a multi-label renderer for per-detail colors.

## Freeze Semantics Correction

- Live test feedback showed the original freeze behavior locked the first valid prediction at player turn start. Example: enemy attack `13`, then the player gains `5` Block, but HUD still shows `-13`.
- Freeze now means: update live during the player turn, remember the latest displayable result, and only freeze that latest result when the player turn is ending.
- Expected behavior after this change: enemy attack `13` displays `-13`; after gaining `5` Block it updates to `-8`; after pressing end turn it holds the last value until the next player turn starts.

## Settings Entry And Covering Screen Policy

- Chose the independent settings route instead of BaseLib/mod-config dependency.
- Removed the main-menu always-visible settings panel. `NSettingsScreen` now gets a `Party Watch HUD` entry button; pressing it opens a focused Party Watch settings sub-panel with a Back button.
- Replaced broad UI-tree scanning with `PartyWatchNativeCoveringScreenTracker`. HUD visibility now depends on basic combat/player/HUD conditions plus explicit native covering screen lifecycle.
- Explicit covering screens include native settings, settings popup, combat pile/deck/card selection screens, deck view, rewards, map, pause, merchant inventories, and game over.
- Those covering screen `_Ready` / `_EnterTree` / `_ExitTree` hooks call `RefreshRegisteredBars()` so HUD hides and restores immediately when the covering page opens or closes.
