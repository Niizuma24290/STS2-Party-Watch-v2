# Phase 12C-A Audit Progress

> Mode: Controlled audit
> Current branch: `main` (tracking `origin/main`, ahead 35)
> Baseline HEAD: `4febf61d68dd5bb0000955d75aa9fe6b99c3d692`
> Current HEAD: `4febf61d68dd5bb0000955d75aa9fe6b99c3d692`
> Last checkpoint: CP-021
> Product files modified: Yes, limited to the G2 batches and G3 Surface Rename recorded below

## Scope Freeze

- Repository: `C:\Users\ROG\Documents\Codex\STS2-Party-Watch-v2`
- Formal tracked baseline: 51 files.
- Untracked candidate scope: 28 files (18 under `docs`, 2 scripts, 8 source files).
- Git-visible audit universe: 79 files.
- Existing working tree at baseline: 26 modified/deleted tracked files, 28 untracked files, 0 staged files.
- Existing tracked diff: 1,449 insertions and 546 deletions; one tracked source file deleted.
- Included: tracked files plus all 28 Git-visible untracked candidates. Untracked materials remain candidates rather than formal documents until separately accepted by the user.
- Excluded: `.git`, `.codegraph`, `.local-nuget`, `work`, package caches, `bin`, `obj`, and other ignored/generated artifacts.
- CodeGraph: readable but stale (6 added, 12 modified, 1 removed); no sync or index write is authorized. Use `rg`, project references, and static reading.
- Scope adjustment: no unit or weight changes. A10 explicitly records untracked notes/assets separately.

## Overall Progress

Completed weight: 100 / 100
Current unit: None; G2 and G3 formally closed, stopped before G4
Status: G3 Closed / G4 Pending Separate Approval

[████████████████████] 100%

## Units

| ID | Unit | Weight | Status | Scope checked | Findings | Rename records | Document records | Checkpoint |
|---|---|---:|---|---:|---:|---:|---:|---|
| P00 | Baseline and Scope Freeze | 5 | Complete | 79/79 enumerated | 0 | 0 | 0 | CP-001 |
| A01 | Repository Structure | 5 | Complete | 7/7 | 1 | 3 | 0 | CP-002 |
| A02 | Loading and Technical Identity | 7 | Complete | 34/34 | 0 | 8 | 0 | CP-003 |
| A03 | Forecast Correctness | 8 | Complete | 18/18 | 4 | 0 | 0 | CP-004 |
| A04 | Combat Lifecycle and State | 12 | Complete | 6/6 | 2 | 0 | 0 | CP-005 |
| A05 | Harmony and Integration | 12 | Complete | 4/4 | 2 | 0 | 0 | CP-006 |
| A06 | Configuration and Persistence | 8 | Complete | 4/4 | 2 | 1 | 0 | CP-007 |
| A07 | HUD and Multiplayer Boundary | 10 | Complete | 8/8 | 2 | 1 | 0 | CP-008 |
| A08 | Performance and Reliability | 8 | Complete | 30/30 | 3 | 0 | 0 | CP-009 |
| A09 | Tests and Diagnostics | 10 | Complete | 79/79 inventoried | 2 | 1 | 0 | CP-010 |
| A10 | Documents, Notes and Rename Corpus | 5 | Complete | 42/42 | 0 | 38 | 42 | CP-011 |
| A11 | Cross-Validation and Deduplication | 5 | Complete | 13/13 | 13 | 46 | 42 | CP-012 |
| A12 | Final Audit Report | 5 | Complete | 5/5 ledgers | 13 | 46 | 42 | CP-013 |

## G1 Verification

- Status: Complete.
- Stable `v0.107.1`: Release restore/build/publish passed; 0 warnings, 0 errors.
- Beta `v0.109.0`: Release restore/build/publish passed; 0 warnings, 0 errors.
- Artifact policy: passed; each target contains exactly the required DLL and manifest.
- Clean-package check: forced restore to an isolated package directory passed on the current machine; tracked-only clean checkout remains unverified because package-source configuration is ignored/machine-local.
- Tests: unavailable; repository has no test project/source.
- Runtime: not performed; game not installed/launched.
- HEAD/staged state: unchanged HEAD; 0 staged files.

## G2 First-Batch Fixes

- Status: Complete for the authorized first batch.
- Resolved: AUD-0002 and AUD-0006.
- Fix applied; runtime regression retained: AUD-0003, AUD-0005, AUD-0013.
- Initial test seam: AUD-0010 now has 7 executable contract assertions passing against stable and beta references.
- Stable/beta post-fix Release restore/build/publish: passed, 0 warnings, 0 errors.
- Not modified: AUD-0001, AUD-0004, AUD-0007, AUD-0008, AUD-0009, AUD-0011, AUD-0012 and all G3 rename scope.

## G2 Second-Batch Fixes

- Status: Complete for the authorized static second batch.
- Resolved: AUD-0001, AUD-0004, AUD-0011, AUD-0012.
- Reproducibility: official BaseLib 3.3.4 asset bootstrap with pinned SHA256; no NuGet package reference remains.
- Diagnostics: stable once-only reason codes cover the audited compatibility/reflection fallbacks.
- Contracts: non-positive incoming totals hide; dead Poison unsupported fields/factory removed and current docs reconciled.
- Test seam: 10 executable contract assertions pass against stable and beta references.
- Stable/beta post-fix Release restore/build/publish: passed, 0 warnings, 0 errors; artifacts byte-identical and whitelist-compliant.
- Not modified: AUD-0007, AUD-0008, AUD-0009, all game-runtime matrices, and all G3 rename scope.

## Current Unit Checklist

- [x] Audit all 42 pre-G0 document/note/asset items
- [x] Classify all 37 old-name-bearing Markdown files
- [x] Cross-validate and deduplicate all 13 findings
- [x] Calibrate severity and verification status
- [x] Complete final report
- [x] Verify five-ledger-only change boundary and unchanged HEAD
- [x] Write CP-013 and stop at G1/G2/G3
- [x] Complete G1 stable/beta Release build and publish
- [x] Verify artifacts, identities, isolated package restore, static diff and worktree boundary
- [x] Write CP-014 and stop before G2/G3/runtime work
- [x] Apply G2 conservative hand-card, melted-relic, covering-screen, initialization, and combat-end cleanup fixes
- [x] Add and run the initial stable/beta contract harness
- [x] Complete post-G2 dual-target build, artifact and static verification
- [x] Write CP-015 and stop before G3/runtime work
- [x] Apply the authorized AUD-0001/AUD-0004/AUD-0011/AUD-0012 second batch
- [x] Run dependency bootstrap/hash, package-reference, stable/beta contract, dual-build, artifact, identity, and static checks
- [x] Write CP-016 and stop before G3/game runtime work
- [x] Record user runtime passes for AUD-0003, AUD-0005, and Phase 13A core HUD scenes
- [x] Record the unresolved Beating Remnant symptom under AUD-0013
- [x] Defer AUD-0007 profiling to a dedicated task card
- [x] Differentiate AUD-0013: normal obtain-before-combat flow passes; only mid-combat debug injection reproduces the fallback

## Open Questions

- G1: completed for build/static verification; runtime matrices were not included.
- G2: AUD-0003/AUD-0005/AUD-0013 runtime-verified and resolved; AUD-0007 is deferred to a dedicated task card.
- G3: whether to implement the separately proposed Surface Rename scope.

## Resume Point

- Last complete unit: A12 Final Audit Report.
- Current incomplete unit: none.
- Last exact file/symbol: AUD-0013 normal acquisition versus mid-combat debug-injection runtime differentiation.
- Next exact action: execute the separate AUD-0007 performance task or request G3 separately.
- Work not to repeat: P00 and A01–A12; all 79 Git-visible items, 42 document candidates, 13 findings, and 46 rename records are closed for static audit.
- Required user decision: choose the separate AUD-0007 task and/or request G3.

## Checkpoints

### CP-001

- Timestamp and timezone: 2026-07-18 23:40:42 +08:00.
- Current branch and HEAD: `main`; `4febf61d68dd5bb0000955d75aa9fe6b99c3d692`.
- Working-tree status summary: ahead 35; 26 modified/deleted tracked; 28 untracked; 0 staged.
- Completed audit unit/sub-unit: P00 Baseline and Scope Freeze.
- Files reviewed: repository root entries; Git tracked/untracked inventories; status and diff summaries; `.codegraph` status; five planned ledger paths.
- Symbols or sections reviewed: N/A (baseline only).
- Finding IDs created/updated: none.
- Rename IDs created/updated: none.
- Document IDs created/updated: none.
- Open questions: G1, G2, G3 decisions remain deferred.
- Exact last reviewed location: CodeGraph status (21 indexed C# files; pending 6 added, 12 modified, 1 removed).
- Exact next action: A01 project structure, dependency, script, and artifact-path inspection.
- Product files modified: No.
- Audit ledger files modified: `audit-progress.md`, `code-audit-findings.md`, `name-migration-inventory.md`, `document-disposition-register.md`, `audit-final-report.md`.

### CP-002

- Timestamp and timezone: 2026-07-18 23:45:00 +08:00.
- Current branch and HEAD: `main`; `4febf61d68dd5bb0000955d75aa9fe6b99c3d692`.
- Working-tree status summary: baseline user changes preserved; only authorized audit ledgers added by this audit.
- Completed audit unit/sub-unit: A01 Repository Structure.
- Files reviewed: `.gitignore`, ignored `NuGet.Config`, `STS2PartyWatchCode.csproj`, manifest, `Install-LocalMod.ps1`, `Build-DualTargets.ps1`, `Save-Sts2ReferenceSnapshot.ps1`; source/build/output layout.
- Symbols or sections reviewed: target framework, assembly/root namespace, package/reference items, output-copy rules, restore/build/publish/install/snapshot paths and guards.
- Finding IDs created/updated: AUD-0001.
- Rename IDs created/updated: REN-0004, REN-0007, REN-0008.
- Document IDs created/updated: none.
- Open questions: build reproducibility can only be verified after G1.
- Exact last reviewed location: `scripts/Save-Sts2ReferenceSnapshot.ps1:113`.
- Exact next action: A02 manifest, entrypoint, BaseLib registration, Harmony owner, namespace/symbol and artifact identity map.
- Product files modified: No.
- Audit ledger files modified: `audit-progress.md`, `code-audit-findings.md`, `name-migration-inventory.md`.

### CP-003

- Timestamp and timezone: 2026-07-18 23:46:20 +08:00.
- Current branch and HEAD: `main`; `4febf61d68dd5bb0000955d75aa9fe6b99c3d692`.
- Working-tree status summary: baseline user changes preserved; only authorized audit ledgers added by this audit.
- Completed audit unit/sub-unit: A02 Loading and Technical Identity.
- Files reviewed: 30 working-tree C# files, project file, manifest, and the two identity-bearing build/install scripts.
- Symbols or sections reviewed: `ModInitializer`, `MainFile.Initialize`, Mod ID, BaseLib registration key, Harmony owner, assembly/DLL/manifest names, root namespace, `PartyWatch*` symbols, project/publish/install paths, diagnostic prefix.
- Finding IDs created/updated: none.
- Rename IDs created/updated: REN-0001 through REN-0008.
- Document IDs created/updated: none.
- Open questions: none for identity classification; all mixed roles were split into separate records.
- Exact last reviewed location: `src/STS2PartyWatchCode/Settings/PartyWatchConfigText.cs:37`.
- Exact next action: A03 trace damage, block, direct HP loss, Poison, Unknown, event ordering, and modifier fallbacks.
- Product files modified: No.
- Audit ledger files modified: `audit-progress.md`, `name-migration-inventory.md`.

### CP-004

- Timestamp and timezone: 2026-07-18 23:48:00 +08:00.
- Branch / HEAD / worktree: `main`; baseline HEAD unchanged; user changes preserved.
- Completed unit: A03 Forecast Correctness.
- Files/symbols reviewed: all 15 Combat and 3 Forecast source files; damage/block/direct-loss/Poison/Unknown paths; event ordering and modifier fallbacks; mechanics evidence.
- Finding IDs: AUD-0002, AUD-0003, AUD-0004, AUD-0012.
- Rename/document IDs: none.
- Open questions: AUD-0003 requires runtime collection semantics; Phase 13A zero-display contract requires a product decision.
- Last location: `EnemyPreActionSurvivalPreview.GetUnsupportedReason` and result contracts.
- Next action: A04 lifecycle/state graph.
- Product files modified: No.
- Audit ledgers modified: `audit-progress.md`, `code-audit-findings.md`.

### CP-005

- Timestamp and timezone: 2026-07-18 23:49:00 +08:00.
- Branch / HEAD / worktree: `main`; baseline HEAD unchanged; user changes preserved.
- Completed unit: A04 Combat Lifecycle and State.
- Files/symbols reviewed: snapshot store, turn start/end commits, combat end, covering-screen transitions, observed HP-loss budget state.
- Finding IDs: AUD-0005, AUD-0013.
- Rename/document IDs: none.
- Open questions: combined covering-screen/end-turn sequence and cross-combat budget order require G1/runtime evidence.
- Last location: `ForecastTurnLifecyclePatch.AfterCombatEndPostfix`.
- Next action: A05 Harmony target and integration audit.
- Product files modified: No.
- Audit ledgers modified: `audit-progress.md`, `code-audit-findings.md`.

### CP-006

- Timestamp and timezone: 2026-07-18 23:49:30 +08:00.
- Branch / HEAD / worktree: `main`; baseline HEAD unchanged; user changes preserved.
- Completed unit: A05 Harmony and Integration.
- Files/symbols reviewed: initializer/PatchAll, health-bar/private targets, card/relic/turn hooks, stable/beta optional hooks, global CanvasItem visibility patches.
- Finding IDs: AUD-0006, AUD-0007; AUD-0011 cross-referenced.
- Rename/document IDs: REN-0003, REN-0004, REN-0006 already registered.
- Open questions: global visibility-patch frequency requires measurement.
- Last location: `ForecastBetaTurnEndPatch` and `NativeCoveringScreenVisibilityPatch`.
- Next action: A06 BaseLib configuration/persistence/localization.
- Product files modified: No.
- Audit ledgers modified: `audit-progress.md`, `code-audit-findings.md`.

### CP-007

- Timestamp and timezone: 2026-07-18 23:50:00 +08:00.
- Branch / HEAD / worktree: `main`; baseline HEAD unchanged; user changes preserved.
- Completed unit: A06 Configuration and Persistence.
- Files/symbols reviewed: BaseLib config properties/key, generated row setup, restore-default path, adapter, localization/reflection, defaults and setting mapping.
- Finding IDs: AUD-0008, AUD-0009.
- Rename IDs: REN-0002 updated by evidence; document IDs: none.
- Open questions: popup localization and in-combat route remain separate future fixes.
- Last location: `PartyWatchBaseLibConfig.RewriteDropdownItems` and Phase 12B runtime failure record.
- Next action: A07 HUD and multiplayer boundary.
- Product files modified: No.
- Audit ledgers modified: `audit-progress.md`, `code-audit-findings.md`.

### CP-008

- Timestamp and timezone: 2026-07-18 23:50:30 +08:00.
- Branch / HEAD / worktree: `main`; baseline HEAD unchanged; user changes preserved.
- Completed unit: A07 HUD and Multiplayer Boundary.
- Files/symbols reviewed: local-creature selection, render policy, text/style/position, expected/incoming modes, snapshot display, covering screens, local-only multiplayer forcing.
- Finding IDs: AUD-0004, AUD-0005.
- Rename IDs: REN-0002 player copy requires explicit local-player wording; document IDs: none.
- Open questions: Phase 13A combat HUD and covering/freeze matrix remains incomplete.
- Last location: `PartyWatchHudVisibilityPolicy.ShouldRenderHud` and `PartyWatchHudDisplay`.
- Next action: A08 performance/reliability audit.
- Product files modified: No.
- Audit ledgers modified: `audit-progress.md`, `code-audit-findings.md`, `name-migration-inventory.md`.

### CP-009

- Timestamp and timezone: 2026-07-18 23:51:00 +08:00.
- Branch / HEAD / worktree: `main`; baseline HEAD unchanged; user changes preserved.
- Completed unit: A08 Performance and Reliability.
- Files/symbols reviewed: all 30 working-tree C# files; static caches, weak references, reflection, allocations, global patches, exception fallbacks, state cleanup.
- Finding IDs: AUD-0007, AUD-0011, AUD-0013.
- Rename/document IDs: none.
- Open questions: no performance severity above Low without measurement.
- Last location: global CanvasItem patch and broad compatibility catches.
- Next action: A09 tests/diagnostics inventory.
- Product files modified: No.
- Audit ledgers modified: `audit-progress.md`, `code-audit-findings.md`.

### CP-010

- Timestamp and timezone: 2026-07-18 23:52:04 +08:00.
- Branch / HEAD / worktree: `main`; baseline HEAD unchanged; user changes preserved plus authorized ledger directory.
- Completed unit: A09 Tests and Diagnostics.
- Files/symbols reviewed: complete Git-visible test/project inventory, prior build/runtime evidence, error/Unknown/native fallback diagnostics, logging prefix.
- Finding IDs: AUD-0010, AUD-0011.
- Rename IDs: REN-0003 diagnostic prefix; document IDs: none.
- Open questions: G1 is required for any new build/test/static-tool verification.
- Last location: repository test inventory (no test project/source found).
- Next action: A10 classify 24 tracked documents plus 18 pre-G0 untracked candidates and all legacy-name hits.
- Product files modified: No.
- Audit ledgers modified: `audit-progress.md`, `code-audit-findings.md`.

### CP-011

- Timestamp and timezone: 2026-07-19 00:03:30 +08:00.
- Branch / HEAD / worktree: `main`; baseline HEAD unchanged; user changes preserved plus authorized ledger directory.
- Completed unit: A10 Documents, Notes and Rename Corpus.
- Files/sections reviewed: 24 tracked documents, 15 pre-G0 untracked Markdown candidates, 3 pre-G0 untracked PNG evidence assets; all legacy-name matching lines and four technical repository-URL locations.
- Finding IDs: none added.
- Rename IDs: REN-0009 through REN-0046 added; REN-0001 through REN-0008 cross-referenced.
- Document IDs: DOC-0001 through DOC-0042.
- Open questions: G3 controls surface prose; candidate acceptance/tracking is outside 12C-B.
- Last location: `docs/task-notes/assets/phase-12a-runtime/main-menu-mod-config.png` and its DOC-0042 reference chain.
- Next action: A11 cross-validate findings, severities, duplicates, and Phase 13 readiness.
- Product files modified: No.
- Audit ledgers modified: `audit-progress.md`, `name-migration-inventory.md`, `document-disposition-register.md`.

### CP-012

- Timestamp and timezone: 2026-07-19 00:05:22 +08:00.
- Branch / HEAD / worktree: `main`; `4febf61d68dd5bb0000955d75aa9fe6b99c3d692`; baseline user state preserved.
- Completed unit: A11 Cross-Validation and Deduplication.
- Files/sections reviewed: all AUD-0001 through AUD-0013 against code, current/historical documents, mechanics evidence, runtime notes, and rename/document ledgers.
- Finding IDs: 13 retained; 9 Confirmed, 4 Needs Runtime Verification; Blocker 0, High 0, Medium 5, Low 8; no duplicates.
- Rename IDs: REN-0001 through REN-0046 closed for static classification.
- Document IDs: DOC-0001 through DOC-0042 closed for static disposition.
- Open questions: G1/G2/G3 only.
- Last location: `code-audit-findings.md` A11 Cross-Validation and Deduplication.
- Next action: A12 final report and change-boundary verification.
- Product files modified: No.
- Audit ledgers modified: `audit-progress.md`, `code-audit-findings.md`.

### CP-013

- Timestamp and timezone: 2026-07-19 00:07:31 +08:00.
- Branch / HEAD / worktree: `main`; `4febf61d68dd5bb0000955d75aa9fe6b99c3d692`; HEAD unchanged; 0 staged files; pre-existing user changes preserved.
- Completed unit: A12 Final Audit Report; Phase 12C-A static audit complete.
- Files/sections reviewed: all five authorized ledgers, final count/placeholder/change-boundary checks, audit directory contents, HEAD and Git status.
- Finding IDs: AUD-0001 through AUD-0013 finalized (9 Confirmed, 4 Needs Runtime Verification; 5 Medium, 8 Low).
- Rename IDs: REN-0001 through REN-0046 finalized.
- Document IDs: DOC-0001 through DOC-0042 finalized.
- Open questions: G1 build/runtime verification, G2 finding fixes, G3 Surface Rename.
- Last location: `audit-final-report.md` section 15 and five-ledger consistency scan.
- Next action: stop; await separate gate approval.
- Product files modified: No.
- Audit ledgers modified: exactly the five files under `docs/task-notes/phase-12c-audit/`.

### CP-014

- Timestamp and timezone: 2026-07-19 00:28:02 +08:00.
- Branch / HEAD / worktree: `main`; `4febf61d68dd5bb0000955d75aa9fe6b99c3d692`; HEAD unchanged; 0 staged files; ignored build outputs only.
- Completed gate: G1 build and static verification.
- Environment: local .NET SDK 9.0.315; stable snapshot `v0.107.1`; beta snapshot `v0.109.0`.
- Build evidence: both Release restore/build/publish targets passed with 0 warnings and 0 errors.
- Artifact evidence: both DLLs 108,544 bytes and SHA256 `B8E5ACB84580BCE901CA51623522B86560CFF178A07CD0F33619E1E49FAB7E8A`; manifests SHA256 `A12AE8A5FD44292DFF347350CEB64B005C54DE82D5155F5CE1B50A2F6C6C96BA`; whitelist passed.
- Identity evidence: assembly `sts2-party-watch-v2` version `1.0.0.0`; Mod ID/BaseLib key/Harmony owner/root namespace/manifest filename unchanged.
- Restore evidence: isolated-package forced restore passed on this machine; AUD-0001 remains Confirmed because ignored local configuration/package payload enabled it.
- Tests/runtime: no test project/source; no game install/launch/runtime verification.
- Static evidence: `git diff --check` passed without whitespace errors; LF/CRLF working-copy warnings noted; Git-visible user state preserved.
- Finding IDs updated: AUD-0001 evidence promoted to E3; all statuses otherwise unchanged.
- Open questions: G2 fixes, G3 Surface Rename, optional targeted runtime matrices.
- Next action: stop; await a separate gate decision.
- Product files modified: No.
- Audit ledgers modified: authorized Phase 12C-A ledgers only.

### CP-015

- Timestamp and timezone: 2026-07-19 00:41:33 +08:00.
- Branch / HEAD / worktree: `main`; `4febf61d68dd5bb0000955d75aa9fe6b99c3d692`; HEAD unchanged; 0 staged files; user-owned baseline changes preserved.
- Completed gate/sub-unit: G2 first-batch fixes and verification.
- Product fixes: conservative exact-Burn/single-blockable-`DamageVar` policy; melted-relic filter; temporary covering-screen snapshot preservation; completion-only initialization latch; combat-end observed-budget cleanup.
- Test changes: new `STS2PartyWatchCode.ContractTests` executable project with 7 contract assertions; stable and beta runs passed.
- Build evidence: post-fix stable/beta Release restore/build/publish passed, 0 warnings and 0 errors; artifacts byte-identical and whitelist-compliant.
- Finding dispositions: AUD-0002/AUD-0006 Resolved; AUD-0003/AUD-0005/AUD-0013 fixed but retain runtime verification; AUD-0010 Accepted for Future Fix after initial harness.
- Environment correction: removed the exact ignored G1-generated `src/STS2PartyWatchCode/work/` directory after verifying it contained only G1 cache/output; this eliminated duplicate generated assembly attributes. No user file was removed.
- Static evidence: `git diff --check` returned no whitespace errors; existing LF/CRLF warnings remain informational.
- Open questions: G3 Surface Rename; optional G2 second batch; targeted game runtime matrices.
- Next action: stop; await a separate decision.
- G2 files modified: 11 product/source/project/test files plus `docs/build-environment.md`, recorded in `audit-final-report.md` section 15.
- Game/installation/commit/push: none.

### CP-016

- Timestamp and timezone: 2026-07-19 01:08:31 +08:00.
- Branch / HEAD / worktree: `main`; `4febf61d68dd5bb0000955d75aa9fe6b99c3d692`; HEAD unchanged; 0 staged files; user-owned baseline changes preserved.
- Completed gate/sub-unit: G2 second-batch static fixes and verification.
- Product fixes: official hash-pinned BaseLib bootstrap/direct compile reference; zero-hidden incoming result contract; once-only compatibility diagnostics; dead Poison unsupported-state contract removal with current-document reconciliation.
- Test evidence: 10 contract assertions passed against stable v0.107.1 and beta v0.109.0.
- Dependency evidence: BaseLib 3.3.4 existing-file SHA256 verification passed; `dotnet list package` reports no package references.
- Build evidence: stable/beta Release restore/build/publish passed with 0 warnings and 0 errors.
- Artifact evidence: each target contains only the 109,568-byte DLL and 376-byte manifest; DLL SHA256 `3D558648BF9F359172773ECF44381528F34B75368B99F6B9C9FD9A7F4DAA5639`; manifest SHA256 `A12AE8A5FD44292DFF347350CEB64B005C54DE82D5155F5CE1B50A2F6C6C96BA`; stable/beta byte-identical.
- Finding dispositions: AUD-0001/AUD-0004/AUD-0011/AUD-0012 Resolved; current totals 2 Confirmed, 4 Needs Runtime Verification, 1 Accepted for Future Fix, 6 Resolved.
- Open runtime items retained: AUD-0003/AUD-0005/AUD-0013 regression matrices and AUD-0007 profiling; AUD-0008/AUD-0009 remain separate confirmed BaseLib UI limitations.
- G3/player-visible rename scope: untouched.
- Game/installation/commit/push: none.

### CP-017

- Timestamp and timezone: 2026-07-20 13:59:40 +08:00.
- Evidence source: user-run game verification; Codex did not install or launch the game.
- AUD-0003: passed; melting/removing the relic refreshes the forecast correctly. Status changed to Resolved.
- AUD-0005: passed; covering-screen close restores the frozen snapshot correctly. Status changed to Resolved.
- Phase 13A: user reports all core HUD scenes are OK.
- AUD-0013: not closed. With Beating Remnant added, only incoming `N` remains while expected `-N` and both detail lanes disappear; removing the relic restores them. This confirms an expected-loss fallback symptom but does not yet identify cross-combat cleanup as its cause.
- AUD-0007: intentionally deferred to dedicated task card `phase-12c-aud-0007-visibility-performance-measurement.md`.
- Current finding totals: 2 Confirmed, 2 Needs Runtime Verification, 1 Accepted for Future Fix, 8 Resolved.
- Code/build/install/commit/push performed by Codex in this checkpoint: none; documentation ledgers only.

### CP-018

- Timestamp and timezone: 2026-07-20 20:10:14 +08:00.
- Evidence source: user-run Beating Remnant differentiation; Codex did not install or launch the game.
- Result: the expected-loss fallback occurs only when Beating Remnant is debug-injected during an active combat. Obtaining/equipping it before entering combat does not reproduce the problem.
- Disposition: AUD-0013 changed from Needs Runtime Verification to Resolved. The normal cross-combat cleanup hypothesis was not reproduced; no production code change is warranted for the mid-combat debug-only boundary.
- Current finding totals: 2 Confirmed, 1 Needs Runtime Verification, 1 Accepted for Future Fix, 9 Resolved.
- Remaining G2 measurement: AUD-0007 only, delegated to `phase-12c-aud-0007-visibility-performance-measurement.md`.
- Code/build/install/commit/push performed by Codex in this checkpoint: none; documentation ledgers only.

### CP-019

- Timestamp and timezone: 2026-07-21 00:04:37 +08:00.
- Completed gate/sub-unit: AUD-0007 diagnostic measurement and patch-disabled A/B control.
- A4 runtime evidence: all three global visibility targets were patched; 30,993 callbacks over 510.002 seconds; 0.368 callback ms/second overall; approximately 0.520 callback ms/second in the derived overlay interval; maximum callback 807.300 us; maximum refresh 611.300 us.
- Redundancy evidence: two real transitions produced four matched notifications and four refreshes, or 2.0 refreshes/transition. This is below the task's more-than-three-refreshes optimization trigger.
- B control: the user reported no perceptible performance difference. FPS and 1% low are N/A because no in-game measurement facility was available. B's covering-screen hiding differences were expected because the global postfixes were disabled.
- Disposition: AUD-0007 changed from Needs Runtime Verification to Resolved, measured non-material; no optimization is authorized or justified.
- Separate observation: `NCardPileScreen` draw/discard-pile viewing does not hide the HUD. This is retained as a compatibility/coverage gap, not a performance result.
- Production restoration: user-approved Production artifact installed with the game closed; installed DLL SHA256 `3D558648BF9F359172773ECF44381528F34B75368B99F6B9C9FD9A7F4DAA5639`; exactly one Party Watch manifest; Codex did not launch the game.
- Cleanup: all A/A2/A3/A4/B/B-final publish, bin, obj, NuGet-scratch, and MSBuild-temp test directories were deleted after path-boundary validation. Recursive verification found zero matching test directories. Production output, task documentation, gated source, screenshot reference, and the raw A4 AppData log remain.
- Current finding totals: 2 Confirmed, 0 Needs Runtime Verification, 1 Accepted for Future Fix, 10 Resolved.
- Stable/beta contract tests: 17/17 passed on both references. Production dual builds passed with 0 warnings and 0 errors and remained byte-identical.
- Branch / HEAD / worktree: `main`; `4febf61d68dd5bb0000955d75aa9fe6b99c3d692`; no commit or push; existing dirty worktree preserved.

### CP-020

- Timestamp and timezone: 2026-07-21 00:54:08 +08:00.
- Gate: formal G2 closure accepted by the user.
- Closure basis: both authorized G2 batches complete; all required build/contract checks passed; AUD-0003/AUD-0005/AUD-0013 runtime-verified; AUD-0007 measured and resolved as non-material.
- Production state: normal Production restored and SHA256-verified as `3D558648BF9F359172773ECF44381528F34B75368B99F6B9C9FD9A7F4DAA5639`.
- Diagnostic/control cleanup: recursive verification reports zero remaining A/A2/A3/A4/B/B-final test build or intermediate directories.
- Deferred/out-of-scope: AUD-0008 and AUD-0009 remain Confirmed; AUD-0010 remains Accepted for Future Fix; the separate `NCardPileScreen` coverage gap is recorded. None keeps G2 open.
- Boundary: G3 not started; no commit, push, remote update, or game launch; existing dirty worktree preserved.
- Final G2 status: Closed.

### CP-021

- Timestamp and timezone: 2026-07-21 +08:00.
- Gate: G3 Surface Rename completed under separate user approval.
- Player surface: manifest display name changed to `Damage Forecast`; config labels changed to `Enable Damage Forecast HUD` / `启用伤害预测 HUD`; multiplayer labels explicitly state the local-player boundary in English and Simplified Chinese.
- Retained identities: Mod ID, BaseLib key, Harmony owner, assembly/DLL/manifest filenames, root namespace, all `PartyWatch*` symbols/files/keys, project and build/install paths, Workshop ID, and `[STS2 Party Watch]` diagnostic prefix.
- Document disposition: current/canonical prose reconciled section-by-section; historical records, screenshots, paths, commands, hashes, and completed-phase statements preserved. No filename rename.
- Contract evidence: harness expanded from 17 to 21 assertions; 21/21 passed against stable v0.107.1 and beta v0.109.0.
- Build evidence: stable/beta Release restore/build/publish passed with 0 warnings and 0 errors. Outputs contain only the DLL and manifest.
- Artifact evidence: stable/beta byte-identical 109,568-byte DLL SHA256 `C7FC2B9FEC076CE4389146FD4469E78474EFDD3951DAA5BA27818C28898A2F37`; byte-identical 375-byte manifest SHA256 `A0CCDE08C2C7C0DB1C5D3BFC374C7523DFF0ECF492D00E6F154BF1AE1DAC5E11`.
- Static evidence: exact player-surface assertions passed; assembly identity remains `sts2-party-watch-v2, Version=1.0.0.0`; `git diff --check` passed with informational LF/CRLF warnings; HEAD unchanged; 0 staged files.
- Runtime/install/commit/push: none. G4 remains pending separate approval.
- Final G3 status: Closed as StaticVerified / BuildVerified / ContractVerified / RuntimeUnverified.
