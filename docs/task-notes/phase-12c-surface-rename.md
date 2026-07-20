# Phase 12C-B G3 — Surface Rename

Date: 2026-07-21

## Status

Completed / StaticVerified / BuildVerified / ContractVerified / RuntimeUnverified.

G2 and AUD-0007 were already closed before this gate. The user separately approved G3. No game installation, launch, commit, push, or remote update was performed.

## Player-Facing Result

- English product name: `Damage Forecast`.
- Chinese product name: `伤害预测`.
- English enable label: `Enable Damage Forecast HUD`.
- Chinese enable label: `启用伤害预测 HUD`.
- English multiplayer label: `Show local-player Damage Forecast HUD in Multiplayer`.
- Chinese multiplayer label: `在多人模式中显示本机伤害预测 HUD`.
- Manifest description: `Read-only local-player combat damage forecast HUD.`

## Technical and Historical Identity Retained

- Mod ID, BaseLib registration key, and Harmony owner: `sts2-party-watch-v2`.
- Assembly/DLL identity and manifest filename: `sts2-party-watch-v2`.
- Root namespace: `STS2PartyWatch`.
- All `PartyWatch*` types, members, properties, node names, source filenames, and config keys.
- Project/directory identity: `STS2PartyWatchCode`.
- Build, publish, install, repository, and Workshop paths/IDs.
- Diagnostic prefix: `[STS2 Party Watch]`.
- Dated phase notes, screenshots, logs, hashes, commands, commits, and other historical evidence.

No source file or document was renamed. No forecast, persistence, layout, lifecycle, performance, or multiplayer behavior changed.

## Files Changed for G3

Player surface and contract:

- `src/STS2PartyWatchCode/sts2-party-watch-v2.json`
- `src/STS2PartyWatchCode/Settings/PartyWatchConfigText.cs`
- `tests/STS2PartyWatchCode.ContractTests/Program.cs`

Current/canonical or hybrid prose:

- `README.md`
- `docs/architecture.md`
- `docs/decisions.md`
- `docs/interface-map.md`
- `docs/mechanics-evidence.md`
- `docs/project-state.md`
- `docs/v2-roadmap.md`
- `docs/task-notes/README.md`
- `docs/task-notes/project-total-note.md`
- `docs/task-notes/phase-11-supplements.md`
- `docs/task-notes/phase-12b-config-localization-language-color.md`

Audit closure:

- `docs/task-notes/phase-12c-audit/name-migration-inventory.md`
- `docs/task-notes/phase-12c-audit/document-disposition-register.md`
- `docs/task-notes/phase-12c-audit/audit-progress.md`
- `docs/task-notes/phase-12c-audit/audit-final-report.md`
- this task note.

Reviewed without a G3 text change because their matching literals are technical or historical:

- `docs/build-environment.md`
- `docs/task-notes/phase-13a-damage-display.md`

## Residual Classification

Remaining `Party Watch`, `PartyWatch*`, `STS2PartyWatch*`, and `sts2-party-watch-v2` hits were classified as one of:

1. technical/loading/persistence identity;
2. source symbol, project, file, or path identity;
3. stable diagnostic prefix;
4. dated historical evidence or a completed-phase statement;
5. an explicit former-name compatibility note.

No unresolved old player-facing label remains in the manifest or current config text. Historical UI labels remain deliberately visible in dated notes and screenshots.

## Verification

Stable reference: v0.107.1. Beta reference: v0.109.0.

- Stable Release restore/build/publish: passed, 0 warnings, 0 errors.
- Beta Release restore/build/publish: passed, 0 warnings, 0 errors.
- Stable contract harness: 21/21 passed.
- Beta contract harness: 21/21 passed.
- Artifact whitelist: each target contains only `sts2-party-watch-v2.dll` and `sts2-party-watch-v2.json`.
- Stable/beta DLL: byte-identical, 109,568 bytes, SHA256 `C7FC2B9FEC076CE4389146FD4469E78474EFDD3951DAA5BA27818C28898A2F37`.
- Stable/beta manifest: byte-identical, 375 bytes, SHA256 `A0CCDE08C2C7C0DB1C5D3BFC374C7523DFF0ECF492D00E6F154BF1AE1DAC5E11`.
- Assembly identity: `sts2-party-watch-v2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null`.
- Manifest ID: unchanged as `sts2-party-watch-v2`.
- `git diff --check`: passed; existing LF/CRLF working-copy warnings are informational.

The DLL hash changed from the G2 production hash because the compiled player-visible config strings changed. The identity, file name, behavior contracts, and target parity did not change.

## G4 Runtime Closure

The user separately approved G4, and it is now closed as `RuntimeVerified`. Full evidence is recorded in `docs/task-notes/phase-12c-g4-runtime-verification.md`.

Runtime result:

1. the Mod list shows `Damage Forecast` exactly once;
2. startup/load succeeds and the retained `[STS2 Party Watch]` diagnostic prefix remains usable;
3. `Main Menu -> Mod Configuration -> Damage Forecast` opens;
4. the left list keeps the stable English identity while the page title and setting labels correctly use English or Simplified Chinese, including the first open after launch;
5. the multiplayer label explicitly says local player in both languages;
6. the selected config language survives a complete game restart;
7. the core single-player HUD remains normal, while the previously verified local-player-only multiplayer boundary remains unchanged;
8. no duplicate legacy/new Mod entry or manifest appears.

The BaseLib title required a code-only compatibility refinement because the Mod has no localization PCK: keep the shared list/button lookup title stable as `Damage Forecast`, then update only this Mod's page-title control after BaseLib finishes `LoadModConfig` and on later language changes. The final installed DLL is 112,128 bytes with SHA256 `1BAC85248FD83B62BE9867FD3A2F8F3B54D676D96D3F6B25F71B720A62D544F0`; stable/beta builds pass with 0 warnings/errors and 28/28 contract assertions on both targets.

No G5 action was authorized: no commit, push, Workshop update, or remote-documentation update was performed.

AUD-0008, AUD-0009, the accepted future test expansion in AUD-0010, and the separate `NCardPileScreen` coverage gap remain outside G3.
