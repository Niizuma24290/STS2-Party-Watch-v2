# Phase 11A - STS2 v1.08 Beta Quick Compatibility Smoke

Date: 2026-07-03

Scope: lightweight local development compatibility smoke only. This is not full v1.08 certification, not a Poison verification replacement, not BaseLib work, and not a Workshop release task.

## Target

Installed game target:

- Game root: `C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2`
- `release_info.json`: version `v0.108.0`, branch `v0.108.0`, commit `58694f64`, date `2026-07-02T20:04:20-07:00`
- Local `sts2.dll`: `data_sts2_windows_x86_64\sts2.dll`, length `9571328`, timestamp `2026-07-03 16:16:47`
- Local `sts2.xml`: `data_sts2_windows_x86_64\sts2.xml`, length `5469103`, timestamp `2026-07-03 16:16:47`

## Focused CodeGraph Inspection

Relevant compatibility surfaces inspected:

- Project reference setup: `src/STS2PartyWatchCode/STS2PartyWatchCode.csproj` references `$(Sts2GameRoot)\data_sts2_windows_x86_64\sts2.dll`, `GodotSharp.dll`, and `0Harmony.dll`; default `Sts2GameRoot` already points at the local Steam game install.
- Harmony patch classes:
  - `ForecastRefreshPatch` patches `NHealthBar.SetCreature`, `NHealthBar.RefreshValues`, `NHealthBar.SetHpBarContainerSizeWithOffsets`, hand pile changes, relic changes, Beating Remnant side-turn reset, hook side-turn lifecycle, and combat end.
  - `NativeCoveringScreenLifecyclePatch` resolves native covering screen lifecycle methods by type name via `AccessTools`.
  - `PartyWatchModdingSettingsPatch` patches `NModInfoContainer.Fill(Mod)` for Party Watch's own mod info panel.
- `LocalIncomingDamageReader` remains the core read-only forecast input path. It reads local player/creature state, enemy `AttackIntent`, hand turn-end damage, turn-end power damage, verified direct HP loss, supported HP-loss modifiers, and ordinary Poison pre-action survival.
- `EnemyPreActionSurvivalPreview` / `PoisonTickPreview` are present in `Combat/EnemyPreActionSurvivalPreview.cs`; ordinary Poison logic is implemented but the formal Poison Steam matrix remains deferred.
- `PartyWatchHudDisplay` still builds the main `-N` output and optional detail line, and positions HUD labels relative to the local health bar.
- `PartyWatchHudVisibilityPolicy` still limits rendering to valid local-player combat state and hides during native covering screens.
- Direct reflection / cached native members:
  - `NHealthBar._creature` is read through a cached `FieldInfo`.
  - `CardTurnEndDamageInspector` inspects card `OnTurnEndInHand` IL.
  - `NativeCoveringScreenLifecyclePatch` resolves native covering screen methods with Harmony `AccessTools`.

Findings:

- `Hook.BeforeTurnEnd` was removed or renamed in v0.108.0. The equivalent side lifecycle target present in the local API is `Hook.BeforeSideTurnEnd(ICombatState, CombatSide, IEnumerable<Creature>)`.
- `Hook.ModifyDamage(...)` now includes a nullable `CardPlay` argument before `ModifyDamageHookType` and `CardPreviewMode`.
- The first v1.08 local load after the compile fixes did not show the HUD in a simple player-turn scene. `NHealthBar` targets still existed in v0.108.0, so a minimal lifecycle refresh was added after `Hook.AfterPlayerTurnStart` for the local player.
- Map/settings covering screens were visible without hiding the HUD because these native screen nodes can remain in the tree and toggle `CanvasItem` visibility without re-running `_Ready`, `_EnterTree`, or `_ExitTree`. A minimal visibility patch now refreshes registered HUD bars when known covering screen instances call `Show`, `Hide`, or `set_Visible`.
- No BaseLib references or settings migration were added.

## Retarget Table

| Item | Previous baseline | Current target | Result |
| --- | --- | --- | --- |
| Game version | v1.07 test baseline | v1.08 beta / `v0.108.0` local install | Retargeted to currently installed beta files. |
| Project reference | Local Steam game install `data_sts2_windows_x86_64\sts2.dll` | Same local path, now containing v0.108.0 `sts2.dll` | No game DLL/XML copied into repo. |
| Build result | Previously built against v1.07 baseline | Built against v1.08 beta using `C:\sts2\dotnet\dotnet.exe` | Success, 0 warnings, 0 errors. |
| Required code changes | n/a | Minimal API compatibility fixes | `Hook.BeforeTurnEnd` -> `Hook.BeforeSideTurnEnd`; add `null` `CardPlay` argument to read-only `Hook.ModifyDamage` preview calls; refresh registered HUD bars after local `Hook.AfterPlayerTurnStart`; refresh after known covering screen `Show` / `Hide` / `set_Visible`. |

## Commands

Used only `C:\sts2\dotnet\dotnet.exe`.

Restore initially hit an inaccessible machine temp lock, then passed with `NUGET_SCRATCH` redirected to ignored `work\nuget-scratch`:

```powershell
C:\sts2\dotnet\dotnet.exe restore .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj --configfile NuGet.Config
```

Build initially surfaced the v1.08 API changes above. After the minimal compatibility fixes:

```powershell
C:\sts2\dotnet\dotnet.exe build .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj --no-restore -c Release
```

Result: success, 0 warnings, 0 errors.

Publish:

```powershell
C:\sts2\dotnet\dotnet.exe publish .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-build -o .\src\STS2PartyWatchCode\bin\Release\net9.0\publish
```

Result: success.

## Local Install And Load Smoke

Installed local development artifact:

- Install directory: `C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\sts2-party-watch-v2`
- First v1.08 API-fix DLL timestamp/hash: `2026-07-03 19:02:21`, SHA256 `02EA4B2FA0A04E2B5968DDFA62A33AABDD94E616CA03CCC0363C4EAC24A9CBF7`; this loaded but did not display the core HUD in the user's simple combat screenshot.
- Final HUD-refresh-fix DLL timestamp/hash: `2026-07-03 19:08:07`, SHA256 `53DA93A7D94447BEA387399171A900546CFC45DA89212F4115C9454B274D962C`
- Final covering-screen-hide-fix DLL timestamp/hash: `2026-07-04 01:30:54`, SHA256 `781B555A5161BBEFDAB19A74A20D3301211B297880CECB854DBB1486BC1A85D0`
- JSON SHA256: `F93CED0594EC8EE7D49A4DF616BB4373251F08B12FFAC730B68262A2D567ED4C`

Load smoke:

- Steam-launched game process `SlayTheSpire2.exe` started at `2026-07-03 19:03:54`.
- Process module inspection showed `sts2-party-watch-v2.dll` loaded from the intended local mods directory.
- No fresh `godot.log` was found under the previously known app-userdata path during this run, so load evidence for this smoke is process-module based plus user visual confirmation, not a fresh log file.

## Runtime Smoke Result

Runtime sequence:

- First in-game screenshot after the compile-only v1.08 fixes showed no Party Watch HUD in a simple supported scene.
- After adding the local-player `Hook.AfterPlayerTurnStart` refresh and reinstalling the local DLL, the user screenshot showed Party Watch `-13` next to the local health bar while the enemy displayed a 13 attack intent.
- Follow-up screenshots showed the damage display itself working, including a reduced `-8` forecast after block.
- User confirmed the damage display lifecycle is working: it refreshes normally at turn start, and the end-turn freeze behavior is good.
- Covering-screen hide behavior initially failed in this v1.08 smoke: the Party Watch `-8` HUD remained visible over the map screen and over the native settings screen. This was fixed later in the task by refreshing registered HUD bars after known covering screen visibility changes.
- After the covering-screen visibility refresh fix, user confirmed the HUD now hides successfully when opening the deck, settings, and map screens.

## Key Files And Successful Pattern

Key files touched or relied on:

- `src/STS2PartyWatchCode/STS2PartyWatchCode.csproj`: keeps references pointed at the local game install instead of copying `sts2.dll` or XML into the repo.
- `src/STS2PartyWatchCode/Patches/ForecastRefreshPatch.cs`: v1.08 hook compatibility and HUD lifecycle refreshes:
  - `Hook.BeforeTurnEnd` retargeted to `Hook.BeforeSideTurnEnd`.
  - local `Hook.AfterPlayerTurnStart` refresh added so the HUD appears after v1.08 has finalized the player-turn combat view.
- `src/STS2PartyWatchCode/Combat/LocalIncomingDamageReader.cs`
- `src/STS2PartyWatchCode/Combat/VerifiedEnemyDamageModifier.cs`
- `src/STS2PartyWatchCode/Combat/VerifiedTurnEndPowerDamageReader.cs`: read-only `Hook.ModifyDamage(...)` calls updated for the v0.108.0 nullable `CardPlay` argument.
- `src/STS2PartyWatchCode/Patches/NativeCoveringScreenLifecyclePatch.cs`: retained `_Ready` / `_EnterTree` / `_ExitTree` tracking and added known covering screen visibility refreshes for `Show`, `Hide`, and `set_Visible`.
- `src/STS2PartyWatchCode/UI/PartyWatchNativeCoveringScreenTracker.cs`: central list and predicate for native covering screens; now filters tracked nodes by known covering-screen type before recording visibility state.
- `src/STS2PartyWatchCode/UI/PartyWatchHudVisibilityPolicy.cs`: remains the final render gate that hides HUD output while a native covering screen is open.

Successful pattern / lessons:

- Prove the loaded artifact first. Process-module inspection confirmed the running game loaded `sts2-party-watch-v2.dll` from the intended local mods directory; this avoided confusing Workshop and local builds.
- Let the installed v1.08 assembly define the API truth. Reflection showed `Hook.BeforeTurnEnd` was gone, `Hook.BeforeSideTurnEnd` existed, `Hook.ModifyDamage(...)` gained `CardPlay`, and `NHealthBar` targets still existed.
- Separate compile compatibility from runtime lifecycle compatibility. The first build loaded but did not show HUD; the fix was not a forecast change, but a v1.08 lifecycle refresh after local `AfterPlayerTurnStart`.
- Treat overlay hiding as a visibility-refresh problem, not a damage problem. Map/settings/deck screens can stay inside the scene tree and only toggle `CanvasItem` visibility, so `_Ready` / `_EnterTree` / `_ExitTree` alone is not enough.
- Keep fixes minimal and read-only. No BaseLib package, Workshop upload, game-file modification, settings migration, Poison expansion, or forecast-mechanic expansion was needed for this smoke.

Status:

- Built against v1.08: verified.
- Local development artifact installed: verified.
- Load smoke verified: verified by running process module list and user confirmation.
- Core HUD smoke verified: user screenshot confirmed `-13` in a simple supported runtime smoke after the `AfterPlayerTurnStart` refresh fix.
- Core damage update smoke verified: user screenshot confirmed the forecast can update to `-8` after block.
- HUD lifecycle smoke verified: user confirmed turn-start refresh and end-turn freeze both behave correctly.
- Covering-screen hide smoke: verified after the visibility refresh fix; deck, settings, and map screens hide the HUD.
- Existing Party Watch settings entry opens: not reverified from the HUD screenshot in this final pass; the v0.108.0 `NModInfoContainer.Fill(Mod)` target is still present and the patch still builds.
- Patch-target/runtime errors: no compile-time target break remains; no fresh log was available for log-based runtime error scanning in this run.

## Not Reverified

- Not fully compatible with v1.08 is not claimed.
- Ordinary Poison Steam matrix remains deferred.
- Phase 11 ordinary Poison Steam matrix remains deferred.
- Workshop item was not uploaded or altered.
- BaseLib package/UI/config work was not performed.
- Formal multiplayer HUD, teammate HUD, and shared party HUD remain frozen.

## Outcome

Party Watch is partially reverified for the v1.08 beta local development baseline:

- Built against v1.08.
- Load smoke verified.
- Core HUD smoke verified.
- Core damage update smoke verified.
- HUD lifecycle smoke verified.
- Covering-screen hide verified after fix.
- Settings entry smoke not reverified in the final HUD screenshot pass.

Phase 11A is closed as a partial v1.08 smoke. Do not describe the build as fully compatible with v1.08.
