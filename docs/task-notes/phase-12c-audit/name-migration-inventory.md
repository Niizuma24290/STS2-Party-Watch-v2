# Phase 12C-A Name Migration Inventory

> Authority: classification and disposition of legacy-name hits.
> Surface rename authorization: Yes (G3 approved and executed on 2026-07-21).
> Technical identity migration authorization: No (future G6).

## Classification

- N1: Player Surface
- N2: Current/Future Product Prose
- N3: Source Symbol
- N4: Persistent/Loading Identity
- N5: Path/Release Identity
- N6: Compatibility Alias
- N7: Historical Evidence
- N8: Unknown or mixed responsibility

## Inventory

| ID | Location | Current name | Class | Current role | 12C-B action | Future action | Compatibility | Verification | Status |
|---|---|---|---|---|---|---|---|---|---|
| REN-0001 | `src/STS2PartyWatchCode/sts2-party-watch-v2.json:3` | Party Watch HUD | N1 | Manifest player-visible Mod-list name | Rename to `Damage Forecast` | None | Keep manifest ID/file unchanged | Mod-list runtime check | G3 StaticVerified; G4 runtime pending |
| REN-0002 | `src/STS2PartyWatchCode/Settings/PartyWatchConfigText.cs:37` | Party Watch HUD | N1 | BaseLib player-visible setting label (English and Chinese) | Rename player copy to Damage Forecast / 伤害预测 | None | Keep property name/key unchanged | BaseLib UI and setting persistence | G3 ContractVerified; G4 runtime pending |
| REN-0003 | `src/STS2PartyWatchCode/MainFile.cs:28-29` | `[STS2 Party Watch]` | N4 | Legacy internal diagnostic prefix | Retain | Decide only in full technical migration | Log parsers/support evidence may depend on prefix | Static diff and runtime log | Retain in 12C-B |
| REN-0004 | `MainFile.cs:12,25,30`; manifest ID; csproj assembly; manifest/DLL filename | `sts2-party-watch-v2` | N4 | Mod ID, BaseLib registration key, Harmony owner, assembly/DLL and manifest identity | Retain all occurrences | Compatibility-designed identity migration | Settings, load identity, duplicate-load and upgrade continuity | Static identity diff; build; runtime load/config | Retain in 12C-B |
| REN-0005 | `src/STS2PartyWatchCode/**/*.cs`; `STS2PartyWatchCode.csproj:5` | `STS2PartyWatch` | N3 | Root namespace and namespace/import references | Retain | Compiler-aware symbol migration | Binary/type references | Compile after future migration | Retain in 12C-B |
| REN-0006 | `src/STS2PartyWatchCode/**/*.cs` and PartyWatch-prefixed source filenames | `PartyWatch*` | N3 | Types, members, properties, node names and source filenames | Retain | Compiler-aware symbol/file migration | Config property names and Godot node lookup names must be assessed individually | Static symbol inventory and future build/runtime | Retain in 12C-B |
| REN-0007 | project directory, csproj path, build/install script `Project` defaults | `STS2PartyWatchCode` | N5 | Project/path identity | Retain | Path/project migration | Scripts, documentation and local tooling paths | Script/path rescan and future build | Retain in 12C-B |
| REN-0008 | build/install output paths and required artifact names | `sts2-party-watch-v2` | N5 | Publish/install directory, DLL and manifest paths | Retain | Coordinated artifact/path migration | Installed Mod continuity and duplicate artifact avoidance | Static script diff; future install verification | Retain in 12C-B |
| REN-0009 | `src/STS2PartyWatchCode/Settings/PartyWatchConfigText.cs` multiplayer description | Show HUD in Multiplayer / 多人模式显示 HUD | N1 | Player-facing boundary description | Rename to `Show local-player Damage Forecast HUD in Multiplayer` / `在多人模式显示本机伤害预测 HUD` | None | Property/key remains unchanged | English/Chinese BaseLib UI runtime check | G3 ContractVerified; G4 runtime pending |
| REN-0010 | `README.md` (12 matching lines) | Party Watch / STS2PartyWatch / IDs | N2+N3+N4+N5 | Current user entry mixed with technical instructions | Rename current player/product prose; retain all technical literals | Revisit only for full technical migration | Commands and paths must keep working | Surface diff plus link/command scan | G3 completed |
| REN-0011 | `docs/architecture.md` (13 matching lines) | Party Watch / STS2PartyWatch symbols | N2+N3+N4 | Current architecture prose and source identities | Rename explanatory product prose; retain symbols/IDs | Future symbol/identity migration | Source references must remain resolvable | Symbol/path scan | G3 completed |
| REN-0012 | `docs/build-environment.md` (24 matching lines) | Party Watch / project/artifact identities | N2+N4+N5 | Current build prose and exact build identities | Rename visible product prose only | Future coordinated path/artifact migration | Commands/artifacts must remain exact | Command/path scan | G3 reviewed; technical literals retained |
| REN-0013 | `docs/decisions.md` (3 matching lines) | Party Watch / STS2PartyWatch | N2+N7 | Decision history with current implications | Rename only current framing; preserve decision text | None unless technical literals migrate | Historical meaning must remain intact | Section-level diff | G3 completed; supersession markers added |
| REN-0014 | `docs/interface-map.md` (13 matching lines) | Party Watch / source symbols | N2+N3+N4 | Current interface prose and exact symbols | Rename product prose; retain symbols/IDs | Future symbol/identity migration | Code links must resolve | Symbol/path scan | G3 completed |
| REN-0015 | `docs/mechanics-evidence.md` (11 matching lines) | Party Watch / technical references | N2+N3+N7 | Current conclusions plus evidence | Rename current conclusions; preserve evidence and symbols | Future technical migration only | Evidence provenance must remain exact | Section-level evidence scan | G3 completed |
| REN-0016 | `docs/project-state.md` (29 matching lines) | Party Watch / technical identities | N2+N3+N4+N5+N7 | Current state interleaved with chronology | Rename current/future prose; preserve dated history and technical literals | Future coordinated technical migration | Repository URLs/commands remain unchanged | Section and URL scan | G3 completed |
| REN-0017 | `docs/task-notes/README.md` (5 matching lines) | Party Watch / note identities | N2+N5+N7 | Current index plus historical entries | Rename current task/product prose; retain note paths/history | Future path migration only if files move | Index links must resolve | Link scan | G3 completed |
| REN-0018 | `docs/task-notes/modding-settings-entry-2026-07-02.md` (20 matching lines) | Party Watch / STS2PartyWatch | N3+N4+N7 | Dated historical record | Preserve all hits | Future identity migration must not rewrite evidence by default | Historical UI/code evidence | Historical-retention diff | Retain in 12C-B |
| REN-0019 | `docs/task-notes/phase-0-repository-bootstrap.md` (1 matching line) | STS2PartyWatchCode | N5+N7 | Completed phase record | Preserve | Future path migration may add a note, not rewrite history | Historical command accuracy | Historical-retention diff | Retain in 12C-B |
| REN-0020 | `docs/task-notes/phase-1-4-singleplayer-baseline.md` (6 matching lines) | Party Watch / symbols | N3+N7 | Completed phase record | Preserve | Future technical migration may add mapping | Historical evidence | Historical-retention diff | Retain in 12C-B |
| REN-0021 | `docs/task-notes/phase-11-supplements.md` (32 matching lines) | Party Watch / IDs/symbols | N2+N3+N4+N5+N7 | Current supplement guidance plus phase history | Rename current guidance only; preserve phase records and technical literals | Future technical migration | Internal links and evidence continuity | Section/link scan | G3 completed; current-name boundary added |
| REN-0022 | `docs/task-notes/phase-5-blockable-incoming-damage.md` (8 matching lines) | Party Watch / symbols | N3+N7 | Completed phase record | Preserve | Future symbol mapping only | Historical evidence | Historical-retention diff | Retain in 12C-B |
| REN-0023 | `docs/task-notes/phase-6-direct-hp-loss.md` (6 matching lines) | Party Watch / symbols | N3+N7 | Completed phase record | Preserve | Future symbol mapping only | Historical evidence | Historical-retention diff | Retain in 12C-B |
| REN-0024 | `docs/task-notes/phase-8-non-block-damage-modifiers.md` (4 matching lines) | Party Watch / symbols | N3+N7 | Completed phase record | Preserve | Future symbol mapping only | Historical evidence | Historical-retention diff | Retain in 12C-B |
| REN-0025 | `docs/task-notes/phase-9-singleplayer-validation.md` (15 matching lines) | Party Watch / IDs/paths | N4+N5+N7 | Completed validation record | Preserve | Future migration may add compatibility mapping | Historical runtime evidence | Historical-retention diff | Retain in 12C-B |
| REN-0026 | `docs/task-notes/phase-9a-followup-2026-07-01.md` (7 matching lines) | Party Watch / IDs | N4+N7 | Dated follow-up | Preserve | Future identity mapping only | Historical evidence | Historical-retention diff | Retain in 12C-B |
| REN-0027 | `docs/task-notes/phase-9a-ui-settings-lifecycle.md` (8 matching lines) | Party Watch / symbols | N3+N7 | Completed phase record | Preserve | Future symbol mapping only | Historical behavior evidence | Historical-retention diff | Retain in 12C-B |
| REN-0028 | `docs/task-notes/phase-9b-poison-pre-action-survival.md` (10 matching lines) | Party Watch / symbols | N3+N7 | Completed phase record | Preserve | Future symbol mapping only | Historical behavior evidence | Historical-retention diff | Retain in 12C-B |
| REN-0029 | `docs/task-notes/project-total-note.md` (10 matching lines) | Party Watch / technical literals | N2+N3+N4+N5+N7 | Current summary plus history | Rename/reconcile current prose; preserve history and technical literals | Future technical migration | Commands and evidence continuity | Section/path scan | G3 completed; historical snapshot marked |
| REN-0030 | `docs/task-notes/workshop-private-rc-2026-07-01.md` (16 matching lines) | Party Watch / repository and artifact identities | N4+N5+N7 | Dated release record | Preserve | Future migration must assess repository URL and release continuity | Historical release evidence | URL/artifact scan | Retain in 12C-B |
| REN-0031 | `docs/v2-roadmap.md` (6 matching lines) | Party Watch / technical literals | N2+N5+N7 | Current roadmap plus completed history | Rename/reconcile current and future prose; preserve completed history | Future path migration only | Phase links remain valid | Section/link scan | G3 completed |
| REN-0032 | `docs/task-notes/phase-11a-v108-quick-compatibility.md` (23 matching lines) | Party Watch / IDs/paths | N3+N4+N5+N7 | Candidate completed phase record | Preserve | Future migration may add compatibility mapping | Historical compatibility evidence | Historical-retention diff | Retain in 12C-B |
| REN-0033 | `docs/task-notes/phase-11b-stable-dll-initialization.md` (14 matching lines) | Party Watch / IDs/paths | N3+N4+N5+N7 | Candidate completed phase record | Preserve | Future migration may add compatibility mapping | Historical load evidence | Historical-retention diff | Retain in 12C-B |
| REN-0034 | `docs/task-notes/phase-11c-poison-runtime-verification-safe-expansion.md` (19 matching lines) | Party Watch / symbols | N3+N7 | Candidate runtime evidence record | Preserve | Future symbol mapping only | Screenshot/runtime provenance | Historical-retention diff | Retain in 12C-B |
| REN-0035 | `docs/task-notes/phase-11d-diamond-diadem-mechanism-compatibility.md` (3 matching lines) | Party Watch / symbol | N3+N7 | Candidate completed phase record | Preserve | Future symbol mapping only | Historical mechanics evidence | Historical-retention diff | Retain in 12C-B |
| REN-0036 | `docs/task-notes/phase-11e-dual-version-build-baseline.md` (6 matching lines) | Party Watch / build identities | N4+N5+N7 | Candidate completed phase record | Preserve | Future build-identity mapping | Historical build evidence | Historical-retention diff | Retain in 12C-B |
| REN-0037 | `docs/task-notes/phase-12a-baselib-auto-config-evaluation.md` (36 matching lines) | Party Watch / keys/symbols | N3+N4+N7 | Candidate evaluation record | Preserve | Future identity mapping only | Historical evaluation conclusions | Historical-retention diff | Retain in 12C-B |
| REN-0038 | `docs/task-notes/phase-12a-completion-handoff-2026-07-03.md` (8 matching lines) | Party Watch / IDs/paths | N4+N5+N7 | Candidate dated handoff | Preserve | Future mapping only | Referenced evidence paths | Historical-retention/link scan | Retain in 12C-B |
| REN-0039 | `docs/task-notes/phase-12a-runtime-verification-2026-07-03.md` (5 matching lines) | Party Watch / UI state | N7 | Candidate runtime evidence record | Preserve | None by default | Embedded screenshots document old surface | Historical-retention/link scan | Retain in 12C-B |
| REN-0040 | `docs/task-notes/phase-12a-session-handoff-2026-07-03.md` (67 matching lines) | Party Watch / IDs/paths/URL | N3+N4+N5+N7 | Candidate dated handoff | Preserve | Future migration must assess repository URL and technical mapping | Historical commands and evidence | Historical-retention/URL scan | Retain in 12C-B |
| REN-0041 | `docs/task-notes/phase-12b-config-localization-language-color.md` (53 matching lines) | Party Watch / symbols/keys | N2+N3+N4+N7 | Candidate implementation record with current limitation | Rename only current/future product prose; preserve evidence and technical literals | Future technical migration | Runtime limitation evidence must remain attributable | Section-level diff | G3 completed; historical boundary and current follow-up wording added |
| REN-0042 | `docs/task-notes/phase-13a-damage-display.md` (17 matching lines) | Party Watch / symbols/IDs | N2+N3+N4+N7 | Candidate implementation/current validation record | Rename current/future product prose; preserve implementation evidence and technical literals | Future technical migration | Runtime matrix provenance | Section-level diff | G3 reviewed; hits are technical identities |
| REN-0043 | `docs/task-notes/session-handoff-dual-target-build-baseline-2026-07-18.md` (6 matching lines) | Party Watch / build identities | N4+N5+N7 | Candidate dated handoff | Preserve | Future build-identity mapping | Historical build evidence | Historical-retention diff | Retain in 12C-B |
| REN-0044 | `docs/task-notes/session-handoff-dual-target-build-complete-2026-07-18.md` (5 matching lines) | Party Watch / build identities | N4+N5+N7 | Candidate dated handoff | Preserve | Future build-identity mapping | Historical build evidence | Historical-retention diff | Retain in 12C-B |
| REN-0045 | `docs/task-notes/session-handoff-dual-version-compatibility-2026-07-17.md` (18 matching lines) | Party Watch / IDs/paths/URL | N3+N4+N5+N7 | Candidate dated handoff | Preserve | Future migration must assess repository URL and compatibility mapping | Historical compatibility evidence | Historical-retention/URL scan | Retain in 12C-B |
| REN-0046 | `docs/task-notes/session-handoff-poison-testsubject-intangible-2026-07-18.md` (14 matching lines) | Party Watch / symbols | N3+N7 | Candidate runtime handoff/evidence | Preserve | Future symbol mapping only | Screenshot/runtime provenance | Historical-retention diff | Retain in 12C-B |

## Coverage Summary

- Technical and player-surface identities: REN-0001 through REN-0009.
- Document corpus: REN-0010 through REN-0046 covers all 37 pre-G0 Markdown files with legacy-name matching lines. The other two Markdown files and three PNG assets have zero text hits.
- 12C-B player-surface changes are limited to N1 and current/future N2 prose. N3/N4/N5 technical literals and all N7 historical evidence are explicitly retained.
- No filename rename is proposed for 12C-B.
- Future technical migration must separately design compatibility for Mod ID/BaseLib key/Harmony owner, assembly/DLL/manifest identity, settings continuity, namespaces/symbols, paths/scripts, repository URLs, installed-artifact upgrades, and rollback.

## G1 Identity Verification

- Stable and beta Release artifacts both retain assembly/DLL identity `sts2-party-watch-v2` and manifest filename `sts2-party-watch-v2.json`.
- Source inspection reconfirmed Mod ID, BaseLib registration key, and Harmony owner as `sts2-party-watch-v2`; root namespace remains `STS2PartyWatch`.
- Stable/beta DLLs and manifests are byte-identical across targets. No rename was performed, so every REN disposition remained unchanged and G3 was still pending at G1.

## G3 Surface Rename Verification

- Player-visible manifest name is `Damage Forecast`; English/Chinese enable and local-player multiplayer labels are covered by contract assertions.
- Current product prose was reconciled section-by-section. Dated history, screenshots, commands, hashes, paths, diagnostic prefixes, and technical literals were preserved.
- Stable/beta Release builds passed with 0 warnings and 0 errors. Both targets produced byte-identical 109,568-byte DLLs with SHA256 `C7FC2B9FEC076CE4389146FD4469E78474EFDD3951DAA5BA27818C28898A2F37` and byte-identical manifests with SHA256 `A0CCDE08C2C7C0DB1C5D3BFC374C7523DFF0ECF492D00E6F154BF1AE1DAC5E11`.
- Assembly/DLL identity remains `sts2-party-watch-v2, Version=1.0.0.0`; manifest ID, BaseLib key, Harmony owner, root namespace, filenames, and paths remain unchanged.
- Contract harness expanded from 17 to 21 assertions; all 21 passed against stable v0.107.1 and beta v0.109.0.
- G4 remains responsible for installation, mod-list/config UI, persistence, load-log, and in-game runtime verification.
