# Phase 12C-B G4 — Install and Runtime Verification

Date: 2026-07-21

## Status

Closed / RuntimeVerified / HashVerified / BuildVerified / ContractVerified. G4 is complete; G5 is not approved.

G4 was separately approved by the user after G3 closed. No commit, push, or remote update is authorized or performed.

## Installed Artifact

Target directory:

```text
C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\sts2-party-watch-v2
```

The game process was closed before installation. Exactly two files were overwritten:

- `sts2-party-watch-v2.dll`: 109,568 bytes; SHA256 `C7FC2B9FEC076CE4389146FD4469E78474EFDD3951DAA5BA27818C28898A2F37`.
- `sts2-party-watch-v2.json`: 375 bytes; SHA256 `A0CCDE08C2C7C0DB1C5D3BFC374C7523DFF0ECF492D00E6F154BF1AE1DAC5E11`.

Post-install static checks:

- manifest ID: `sts2-party-watch-v2`;
- manifest player name: `Damage Forecast`;
- matching manifest count under the game `mods` tree: 1;
- installed hashes exactly match the G3 beta build artifact.

Pre-launch log baseline:

```text
C:\Users\ROG\AppData\Roaming\SlayTheSpire2\logs\godot.log
Last write: 2026-07-20 23:59:15 +08:00
Length: 48,732 bytes
```

## Runtime Matrix

| Check | Required evidence | Status |
| --- | --- | --- |
| Launch/load | Steam launch, intended DLL loaded, `[STS2 Party Watch] Loaded`, no related exception | Passed |
| Loader Mod list/log identity | Loader reports `Damage Forecast` exactly once | Passed |
| BaseLib config list/title | Left list: stable `Damage Forecast`; page title: English `Damage Forecast`, Simplified Chinese `伤害预测` | Passed after the initial-load timing fix; user confirmed correct fresh-open title and both language states |
| Config route | Main-menu Mod Configuration opens the intended config | Passed under the corrected `Damage Forecast` list identity |
| English labels | `Enable Damage Forecast HUD`; explicit local-player multiplayer label | Passed during user-confirmed language-switch retest |
| Chinese labels | `启用伤害预测 HUD`; `在多人模式中显示本机伤害预测 HUD` | Passed during user-confirmed language-switch retest |
| Persistence | Selected language or another harmless BaseLib value survives full restart | Passed: user switched to English, fully restarted the game, and confirmed English remained selected; Simplified Chinese restoration was also observed during the prior cycle |
| Single-player HUD smoke | Supported combat shows and refreshes the existing forecast normally | Passed: user reported combat display normal after fresh launch |
| Multiplayer boundary | Local-player HUD behavior remains local-only when available | Preserved; existing Workshop subscription runtime evidence covers the local-player HUD boundary. No new formal multiplayer session was claimed in G4 |
| Duplicate identity | No legacy/new duplicate Mod entry or duplicate manifest | Passed: one installed manifest and one initialized Mod entry |

Known out-of-scope limitations are not G4 rename failures: AUD-0008 opened dropdown items, AUD-0009 in-combat BaseLib route, AUD-0010 future test expansion, and the separate `NCardPileScreen` covering gap.

## First Runtime Result

Steam launch on beta v0.109.0 loaded the intended installed assembly and logged:

```text
Loading assembly DLL ...\mods\sts2-party-watch-v2\sts2-party-watch-v2.dll
Calling initializer method of type STS2PartyWatch.MainFile for sts2-party-watch-v2, Version=1.0.0.0
[BaseLib] Registered config for mod sts2-party-watch-v2
[STS2 Party Watch] Loaded
Finished mod initialization for 'Damage Forecast' (sts2-party-watch-v2)
* Damage Forecast [sts2-party-watch-v2] (v0.1.0)
```

The user screenshot shows that the Chinese enable label is correctly `启用伤害预测 HUD`, proving the G3 config-text artifact is active. It also shows a missed player surface: BaseLib's left list and page title both remain `STS2PartyWatch`.

Screenshot evidence:

```text
C:\Users\ROG\AppData\Local\Temp\codex-clipboard-cd9aaf92-9bda-4262-9cb2-8d7d1cd07622.png
SHA256: 64E21D9EDFDDC95DA31F68D632944A09B2CF86318A2FF3CD7BA9BDB3AFE8E2DA
```

## Diagnosed Cause and Boundary

BaseLib does not use the Mod manifest display name for its configuration title. `NModConfigSubmenu.GetModTitle(...)` first looks up the `settings_ui` localization key `<ModPrefix>.mod_title`; if missing, it falls back to the config type's root namespace. This code-only mod has no `settings_ui` localization resource, so the retained namespace `STS2PartyWatch` becomes the visible fallback.

Changing the namespace would violate the G3 retained-identity boundary and would still omit the desired space. The clean BaseLib-supported route is a `settings_ui` localization entry, but the current project intentionally has no PCK/localization pipeline. A minimal code-only fallback would be a narrowly gated title override for only `ModId == "sts2-party-watch-v2"`; it requires explicit fix authorization and a fresh build/install/runtime cycle.

## Approved Title Fix

The user approved the minimal code-only title fix. `PartyWatchBaseLibTitlePatch` patches BaseLib's existing private title resolver and changes the result only when `ModId == "sts2-party-watch-v2"`:

- English config language: `Damage Forecast`;
- Simplified Chinese config language: `伤害预测`;
- every other Mod ID: preserve BaseLib's original title result.

The patch target is discovered optionally; technical and persistence identities remain unchanged. No BaseLib source or installed BaseLib artifact was modified.

Verification before installation:

- stable v0.107.1 Release build: 0 warnings / 0 errors;
- beta v0.109.0 Release build: 0 warnings / 0 errors;
- stable contract tests: 25/25 passed;
- beta contract tests: 25/25 passed;
- contract coverage confirms BaseLib 3.3.4 exposes the expected resolver, both localized titles resolve correctly, and other Mod IDs preserve their fallback title;
- stable/beta artifacts are byte-identical and contain only the DLL and manifest;
- `git diff --check` passed with existing informational LF/CRLF warnings.

Installed fix artifact:

- DLL: 110,592 bytes; SHA256 `D27726D767588575E0F722858B0855373836C5D21BC9A473AD9F4576B2C658E0`;
- manifest: 375 bytes; SHA256 `A0CCDE08C2C7C0DB1C5D3BFC374C7523DFF0ECF492D00E6F154BF1AE1DAC5E11`;
- matching installed manifest count: 1;
- game process was closed during installation.

## BaseLib List/Page Title Refinement

The first approved title fix revealed a BaseLib lifecycle constraint during runtime retest: the left Mod list is created once, while the page title and this Mod's setting labels can be refreshed after the config language changes. Returning a language-dependent value from BaseLib's shared `GetModTitle` resolver would also change the string used to find and highlight the existing left-side button.

The user accepted the following player-facing contract:

- left Mod list: stable English `Damage Forecast`;
- page title: `Damage Forecast` in English and `伤害预测` in Simplified Chinese;
- setting labels and values: continue following this Mod's selected config language.

Implementation boundary:

- `PartyWatchBaseLibTitlePatch` now returns only the stable English list identity for this Mod ID and preserves every other Mod's original result;
- `PartyWatchBaseLibConfig.ApplyLocalizedText` separately refreshes BaseLib's current page-title control whenever the page is created or the config language changes;
- no technical ID, namespace, persistence key, BaseLib source, or installed BaseLib artifact changed.

Verification before installation:

- stable v0.107.1 and beta v0.109.0 Release builds: 0 warnings / 0 errors;
- stable and beta contract tests: 28/28 passed;
- stable/beta publish outputs are byte-identical and contain only the DLL and manifest;
- `git diff --check` passed with only existing informational LF/CRLF warnings.

Current installed retest artifact:

- DLL: 111,104 bytes; SHA256 `289EF6EC4BD0FF6A5AEDBC98933D5F4D8653752DA2BFD5592C77479012FF25DB`;
- manifest: 375 bytes; SHA256 `A0CCDE08C2C7C0DB1C5D3BFC374C7523DFF0ECF492D00E6F154BF1AE1DAC5E11`;
- installed files exactly match the beta publish output;
- matching installed manifest count remains 1;
- game process was closed during installation;
- title/language, persistence reload, and HUD smoke all passed across the subsequent runtime cycles.

## Refined Title Runtime Result

The user launched the corrected artifact, exercised the language switch, and reported no issue:

- left list remains `Damage Forecast`;
- page title changes between `Damage Forecast` and `伤害预测`;
- setting text follows the selected config language.

The same launch log confirms:

```text
Loading assembly DLL ...\mods\sts2-party-watch-v2\sts2-party-watch-v2.dll
[BaseLib] Registered config for mod sts2-party-watch-v2
[STS2 Party Watch] Loaded
Finished mod initialization for 'Damage Forecast' (sts2-party-watch-v2)
* Damage Forecast [sts2-party-watch-v2] (v0.1.0)
```

No Party Watch or Harmony patch failure appears in the run. Runtime Workshop BaseLib is v3.3.7; the title refinement therefore has positive runtime evidence on v3.3.7 in addition to the frozen v3.3.4 build/contract baseline. BaseLib's missing-localization fallback warnings are expected because this code-only Mod intentionally has no `settings_ui` PCK resource and the narrowly gated patch replaces only this Mod's visible result.

On clean game exit BaseLib logged `NGame.Quit(): saving all ModConfigs`. The persisted file `mod_configs/STS2PartyWatch.cfg` contains `ConfigLanguage = SimplifiedChinese`, enabled HUD, advanced details enabled, and `DamageDisplayMode = Both`. The next fresh launch restored the Chinese setting state, closing the persistence check while exposing the separate initial page-title timing issue below.

## Fresh-Open Title Timing Finding and Fix

On the next fresh launch, the user confirmed that Simplified Chinese persisted: the Hub/config choices and setting labels opened in Chinese, and the combat HUD displayed normally. The page title alone initially remained `Damage Forecast`; changing English and then Simplified Chinese changed it to `伤害预测`.

This isolates the issue to initial UI write ordering rather than persistence. `SetupConfigUI` applied the localized title, then BaseLib's remaining `LoadModConfig` work overwrote the page title with its stable English list identity. The approved code-only title boundary was refined again:

- keep the existing stable-English `GetModTitle` result for the left list, button lookup, highlighting, and controller focus;
- keep the existing config-language callback for live title/setting changes;
- add a narrowly gated postfix after BaseLib `LoadModConfig(ModConfig)` completes, refreshing the page title only when `ModId == sts2-party-watch-v2`.

Post-fix verification:

- stable v0.107.1 and beta v0.109.0 Release builds: 0 warnings / 0 errors;
- stable and beta contract tests: 28/28 passed;
- the contract target check now covers the title resolver, page-load hook, and page-title field on frozen BaseLib 3.3.4;
- stable/beta artifacts remain byte-identical and whitelist-compliant;
- `git diff --check` passed with only existing informational LF/CRLF warnings.

Installed timing-fix artifact:

- DLL: 112,128 bytes; SHA256 `1BAC85248FD83B62BE9867FD3A2F8F3B54D676D96D3F6B25F71B720A62D544F0`;
- manifest: 375 bytes; SHA256 `A0CCDE08C2C7C0DB1C5D3BFC374C7523DFF0ECF492D00E6F154BF1AE1DAC5E11`;
- installed DLL and manifest exactly match the beta publish output;
- game process was closed during installation;
- fresh-open runtime retest passed: with Simplified Chinese already selected, the page opened directly with `伤害预测`; after switching to English and fully restarting, the page retained English.

## G4 Closure

The user explicitly reported the final runtime cycle successful and requested closure. Phase 12C-B now meets the master plan's `RuntimeVerified` criteria:

1. Mod list: `Damage Forecast` appears once.
2. BaseLib page: left list remains stable English; the page title and setting text use the selected English or Simplified Chinese language on first open and after live switching.
3. Config persistence: the selected language survives a full game restart.
4. Single load identity: one installed manifest and one initialized `sts2-party-watch-v2` entry; retained technical identity and `[STS2 Party Watch]` diagnostic prefix remain usable.
5. HUD regression: the user confirmed the single-player combat display remains normal.

Final installed artifact:

- DLL: 112,128 bytes; SHA256 `1BAC85248FD83B62BE9867FD3A2F8F3B54D676D96D3F6B25F71B720A62D544F0`;
- manifest: 375 bytes; SHA256 `A0CCDE08C2C7C0DB1C5D3BFC374C7523DFF0ECF492D00E6F154BF1AE1DAC5E11`;
- stable and beta publish artifacts are byte-identical;
- stable/beta Release builds passed with 0 warnings and 0 errors;
- stable/beta contract harness passed 28/28 assertions;
- runtime Workshop BaseLib v3.3.7 loaded the artifact successfully, while frozen BaseLib v3.3.4 remains the build/contract baseline.

Key files:

- `src/STS2PartyWatchCode/Patches/PartyWatchBaseLibTitlePatch.cs`;
- `src/STS2PartyWatchCode/Settings/PartyWatchBaseLibConfig.cs`;
- `src/STS2PartyWatchCode/Settings/PartyWatchConfigText.cs`;
- `tests/STS2PartyWatchCode.ContractTests/Program.cs`;
- `docs/task-notes/phase-12c-g4-runtime-verification.md`.

Successful pattern:

- keep BaseLib's list/button identity stable instead of changing the shared lookup string after list creation;
- localize the page title separately after `LoadModConfig` completes, and refresh it again on language changes;
- distinguish persistence failures from UI write-order failures by comparing restored setting labels, persisted values, and the first-open title independently;
- verify the exact installed artifact hash after every install rather than trusting a default publish path.

No commit, push, Workshop update, or remote documentation update was performed. Those actions require separate G5 approval. Formal teammate/shared multiplayer HUD remains frozen, and AUD-0008, AUD-0009, AUD-0010 expansion, and the `NCardPileScreen` coverage gap remain separately tracked rather than blocking this rename closure.
