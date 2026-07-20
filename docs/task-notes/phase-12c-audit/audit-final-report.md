# Phase 12C-A Final Audit Report

> Status: G2 formally closed; G3 Surface Rename completed and statically/build/contract verified on 2026-07-21; G4 runtime verification not started.
> Baseline HEAD: `4febf61d68dd5bb0000955d75aa9fe6b99c3d692`.
> Product/source modification authorization: Yes, limited to the approved G2 fixes and G3 player-surface/current-prose scope recorded below.

## 1. Executive Summary

The audit covered the entire 79-file Git-visible baseline, including all 51 tracked files and all 28 pre-G0 untracked candidates. It found no Blocker or High-severity issue. After both G2 batches and targeted user runtime work through 2026-07-21, current disposition is 2 Confirmed, 0 Needs Runtime Verification, 1 Accepted for Future Fix, and 10 Resolved.

G1 and both post-G2 verifications passed stable `v0.107.1` and beta `v0.109.0` Release restore/build/publish with 0 warnings and 0 errors. The final two target artifacts are byte-identical and contain only the required DLL and manifest. The executable contract harness now has 17 assertions passing against both references. User-run runtime verification passed AUD-0003, AUD-0005, AUD-0013, and the Phase 13A core HUD scenes. AUD-0007 instrumentation measured a non-material global visibility callback cost and the patch-disabled B control produced no perceptible difference, so no optimization is justified. The Beating Remnant display symptom was isolated to non-normal mid-combat debug injection and did not reproduce when the relic was obtained before entering combat.

The current dirty worktree already contains Phase 13A implementation. Static review therefore treats Phase 13A readiness as retrospective closure, not pre-implementation approval. Phase 13A still needs its combat/lifecycle runtime matrix. Phase 13B should wait until the Medium correctness/lifecycle findings and test strategy receive explicit disposition.

Surface Rename was completed as a standalone copy/document change. Player-visible `Party Watch HUD` copy is now `Damage Forecast` / `伤害预测`; every loading, persistence, symbol, artifact, path, diagnostic, and historical identity listed below remains retained. G4 is the separate install/runtime gate.

## 2. Repository Baseline

- Branch: `main`, tracking `origin/main`, ahead 35 at P00 and final verification.
- HEAD: `4febf61d68dd5bb0000955d75aa9fe6b99c3d692`; unchanged throughout the audit.
- Tracked files: 51.
- Pre-existing tracked worktree changes: 26 modified/deleted files, including one deletion; 0 staged.
- Pre-existing tracked diff: 1,449 insertions and 546 deletions.
- Pre-G0 untracked candidates: 28 (18 docs/assets, 2 scripts, 8 source files).
- Git-visible audit universe: 79 files.
- CodeGraph: readable but stale (6 added, 12 modified, 1 removed); not refreshed. `rg`, project references, and direct static reading were used.
- Excluded as ignored/generated: `.git`, `.codegraph`, `.local-nuget`, `work`, package caches, `bin`, `obj`, and equivalent artifacts.

## 3. Audit Coverage and Progress

- P00 and A01-A12: 100/100 weighted points complete.
- Source: all 30 working-tree C# files reviewed.
- Project/loading/build: project, manifest, three scripts, ignore/source configuration, dependency and artifact paths reviewed.
- Documents: 24 tracked documents plus 15 pre-G0 untracked Markdown candidates reviewed.
- Evidence assets: 3 pre-G0 untracked PNGs visually reviewed and reference chains verified.
- Findings: AUD-0001 through AUD-0013 cross-validated and deduplicated.
- Rename inventory: REN-0001 through REN-0046.
- Document register: DOC-0001 through DOC-0042.

## 4. Confirmed Blockers

None.

No static evidence establishes a present load failure, destructive state path, or broadly wrong base-game forecast that warrants Blocker or High severity. This does not convert runtime-dependent findings into passes.

## 5. High / Medium / Low Findings

### High

None.

### Medium

1. AUD-0001 — Resolved under G2: tracked, hash-pinned official BaseLib bootstrap replaces the ignored/machine-local NuGet source dependency.
2. AUD-0002 — Resolved under G2: exact built-in Burn plus one blockable `DamageVar` is the only trusted hand-damage shape; other detected shapes return Unknown.
3. AUD-0003 — Resolved under G2: melted relics are filtered and the user verified forecast refresh after removal.
4. AUD-0005 — Resolved under G2: temporary covering screens preserve and restore the frozen snapshot.
5. AUD-0010 — initial ten-case contract harness added; broader automated coverage accepted for future expansion.

### Low

1. AUD-0004 — Resolved under G2: non-positive incoming totals now produce `Hidden` at the typed result boundary.
2. AUD-0006 — Resolved under G2: the success latch is assigned only after registration/patching completes.
3. AUD-0007 — Resolved by measurement: global `CanvasItem` visibility callbacks cost approximately 0.368 ms/second overall and showed no perceptible A/B difference; no optimization is justified.
4. AUD-0008 — opened BaseLib dropdown items remain raw/unlocalized.
5. AUD-0009 — the in-combat BaseLib configuration entry is visible but unusable.
6. AUD-0011 — Resolved under G2: representative compatibility fallbacks emit once-only stable diagnostic codes.
7. AUD-0012 — Resolved under G2: the inert Poison unsupported channel was removed and conservative semantics documented.
8. AUD-0013 — Resolved under G2: combat-end budget cleanup passes the normal next-combat flow; the observed display fallback occurs only when the relic is debug-injected mid-combat.

## 6. Findings Requiring Build or Runtime Verification

- No finding is classified `Needs Build Verification`. G1 stable/beta Release restore/build/publish passed with 0 warnings and 0 errors.
- AUD-0001 is resolved: the official BaseLib 3.3.4 asset is bootstrapped with a pinned SHA256 into ignored `work/`; the project has no NuGet package references, and dual restore/build/publish passed.
- AUD-0003 and AUD-0005 are resolved by code/build plus 2026-07-20 user runtime verification.
- AUD-0007 is resolved by beta A4 instrumentation plus the patch-disabled B control. The measured aggregate cost remained non-material; duplicate delivery was 2.0 refreshes/transition and below the task trigger for optimization.
- AUD-0013 is resolved: normal obtain-before-combat flow passes; mid-combat debug injection is recorded as a non-production boundary.
- AUD-0002, AUD-0004, AUD-0006, AUD-0011, and AUD-0012 are resolved by G2 code plus contract/build verification.
- Confirmed findings AUD-0008 and AUD-0009 remain unchanged and intentionally separate from this G2 batch.

## 7. Phase 13A / 13B Readiness

### Phase 13A

Implemented in the current uncommitted working tree. The user reported that the core Phase 13A HUD scenes pass. Remaining targeted work is narrower:

- AUD-0007 profiling is complete; Production was restored and all A/B test build directories were removed at the user's request;
- AUD-0010 now provides an initial pure contract seam, but lifecycle/Harmony/configuration coverage remains incomplete.

### Phase 13B

AUD-0013 and AUD-0007 are dispositioned and do not block later work. The separate `NCardPileScreen` HUD coverage gap discovered during AUD-0007 remains a compatibility item and does not block the separately bounded G3 surface-copy work.

## 8. Name Migration Summary

- Player surface (N1): manifest display name, BaseLib title/labels, and multiplayer description may change under G3.
- Current/future prose (N2): update only active/current sections in the 14 D1/D4/D5 documents.
- Source symbols (N3), persistent/loading identity (N4), and path/release identity (N5): retain in 12C-B.
- Historical evidence (N7): preserve verbatim in 28 D3 records/assets and the historical sections of D4 documents.
- All 37 legacy-name-bearing Markdown files have an explicit REN record; 5 corpus items have no text hit.
- No filename rename is proposed for 12C-B.

Recommended player copy:

- English product name: `Damage Forecast`.
- Chinese product name: `伤害预测`.
- English setting title: `Enable Damage Forecast HUD`.
- Chinese setting title: `启用伤害预测 HUD`.
- Multiplayer description: `Show local-player Damage Forecast HUD in Multiplayer` / `在多人模式显示本机伤害预测 HUD`.

## 9. Full Document and Note Disposition Summary

- D1 ACTIVE_CANONICAL: 1.
- D3 HISTORICAL_RECORD: 28.
- D4 HYBRID: 9.
- D5 TECHNICAL_REFERENCE: 4.
- D2/D6/D7/D8: 0.
- Current explanatory prose proposed for reconciliation: 14 documents.
- D4 documents requiring section-level current/history separation: 9.
- Historical documents/assets proposed for unchanged preservation: 28.
- Filename renames: 0.
- Four documents have technical repository-URL impact only in a future technical migration: DOC-0008, DOC-0023, DOC-0033, DOC-0038.
- Whether the 18 pre-G0 untracked doc/assets become tracked is a separate acceptance decision, not part of Surface Rename.

Material authority drift is recorded in `document-disposition-register.md`: README configuration/scope, project-state and task-index current-task contradictions, roadmap freshness, project-total-note scope, Poison interface/architecture descriptions, superseded decisions D015/D021, and master-plan Phase 13A timing.

## 10. Completed Phase 12C-B Surface Rename Scope

G3 was approved and completed with this exact boundary:

1. Change only the manifest player-visible `name`; keep manifest filename and `id` unchanged.
2. Change BaseLib player-visible English/Chinese title, enable label, and descriptions; make the multiplayer label explicitly local-player scoped.
3. Reconcile current product prose in README and D1/D4/D5 current sections with BaseLib persistence, Phase 11C Poison scope, and Phase 13A behavior.
4. Add supersession markers to historical decisions rather than rewriting their recorded content.
5. Preserve all dated phase notes, runtime handoffs, screenshots, evidence hashes, commands, technical literals, symbols, paths, and URLs.
6. Do not rename source files, namespaces, types, members, Godot node names, project directories, artifacts, manifest files, or note filenames.

## 11. Explicitly Retained Technical Identities

- Mod ID: `sts2-party-watch-v2`.
- BaseLib configuration registration key: `sts2-party-watch-v2`.
- Harmony owner: `sts2-party-watch-v2`.
- Assembly/DLL identity and manifest filename.
- Root namespace: `STS2PartyWatch`.
- `PartyWatch*` source symbols, filenames, config property names, and node lookup names.
- Project/directory identity: `STS2PartyWatchCode`.
- Build, publish, install, and artifact paths/names.
- Diagnostic prefix: `[STS2 Party Watch]`.
- Repository URLs and all historical evidence literals.

## 12. Future Full Technical Migration Scope

A later compatibility-designed migration, not 12C-B, must cover at least:

1. Mod ID, BaseLib key, and settings persistence continuity.
2. Harmony owner and duplicate-patch/load behavior.
3. Assembly, DLL, manifest filename, and loader identity.
4. Namespace/type/member/source-filename migration.
5. Godot node names and runtime lookup compatibility.
6. Project/directory and script default paths.
7. Build/publish/install directory and artifact transitions.
8. Repository URLs, documentation links, release/Workshop identity, and upgrade messaging.
9. Compatibility aliases or one-time migration where supported, plus old-artifact cleanup and rollback.
10. Clean build, dual-target load, configuration persistence, upgrade, duplicate-load, and uninstall/reinstall verification.

## 13. Risks and Open Decisions

- G1 build/static verification: completed.
- G2 first batch: completed for AUD-0002/0003/0005/0006/0013 and the initial AUD-0010 test seam. Later user runtime evidence resolved AUD-0003/AUD-0005/AUD-0013. AUD-0007 measurement also completed and found no material performance impact.
- G2 second batch: completed for AUD-0001/AUD-0004/AUD-0011/AUD-0012. Runtime matrices were deliberately not performed.
- G3: completed; the exact Surface Rename scope in section 10 did not imply or perform technical identity migration.
- Decide whether/when the 18 untracked document/assets become formal tracked repository content.
- Existing dirty worktree remains user-owned; future work must preserve and rebaseline it before each gate.

## 14. Recommended Next Gate

Stop here before G4. Both authorized G2 static batches, AUD-0007 targeted measurement, and G3 Surface Rename are complete.

- G1/post-G2 result: stable/beta build and artifact verification passed; 17 contract tests pass on both references.
- AUD-0007 result: measured non-material and resolved without optimization. Production SHA256 `3D558648BF9F359172773ECF44381528F34B75368B99F6B9C9FD9A7F4DAA5639` was restored; all A/A2/A3/A4/B/B-final test directories were deleted.
- G2 second batch complete: AUD-0001/AUD-0004/AUD-0011/AUD-0012 resolved. BaseLib popup/in-combat route work remains separate.
- Recommended G4: after separate approval, install the named G3 artifact and verify mod-list/config labels, language switching, settings persistence, load identity, no duplicate entry, and core HUD smoke.

## 15. Statement of Changes Made

- AUD-0007 measurement seam added behind default-off compile properties: reusable cumulative profiler, deterministic accumulator/formatter tests, and a patch-disabled control switch. Normal Production excludes the profiler and retains the global visibility patches.
- AUD-0007 A/A2/A3/A4/B/B-final build and intermediate artifacts were deleted after Production restoration; no diagnostic/control build remains under repository `work/`.
- G2 product/source/project/test files modified: first-batch files plus `IncomingDamageDisplayRead.cs`, `VerifiedEnemyDamageModifier.cs`, `LegacyDiamondDiademDamageForecast.cs`, `EnemyPreActionSurvivalPreview.cs`, new `Diagnostics/PartyWatchDiagnostics.cs`, `STS2PartyWatchCode.csproj`, and the contract harness.
- Existing technical documents updated: `docs/architecture.md`, `docs/interface-map.md`, `docs/project-state.md`, and `docs/build-environment.md`.
- Build scripts added/updated: `Restore-BaseLibDependency.ps1` and `Build-DualTargets.ps1`; no manifest or G3 player-facing rename was changed by the second batch.
- Authorized audit ledger files created/updated: exactly five under `docs/task-notes/phase-12c-audit/`.
- Build performed: Yes — stable and beta Release restore/build/publish, 0 warnings and 0 errors.
- Automated tests performed: Yes — G3 expanded the harness to 21 assertions by covering all four renamed English/Chinese settings labels; 21/21 passed against stable and beta references.
- Static validation performed: Yes — artifact whitelist/hash/assembly identity, technical identity scan, `git diff --check`, Git status/HEAD boundary.
- Static index refresh performed: No.
- Game runtime evidence: Yes, user-performed through 2026-07-21; Codex installed only explicitly approved A/B/Production artifacts and did not launch the game.
- Commit/push/remote update performed: No.

## 16. G2 Closure

G2 was formally accepted for closure by the user on 2026-07-21.

- Both authorized G2 fix batches are complete and verified.
- AUD-0007 is resolved by measurement as non-material; no performance optimization was applied.
- The normal Production artifact is installed and hash-verified.
- All A/A2/A3/A4/B/B-final test build and intermediate directories have been removed.
- AUD-0008, AUD-0009, and the accepted future expansion in AUD-0010 are explicitly outside this closure and do not keep G2 open.
- The draw/discard-pile `NCardPileScreen` HUD coverage observation is recorded for a separate compatibility decision.
- HEAD is unchanged, the staged set is empty, and no commit, push, G3 rename, or game launch was performed.
- User's pre-existing tracked and untracked work preserved: Yes.

## 17. G3 Closure

G3 was completed on 2026-07-21 under the separately approved Surface Rename boundary.

- Manifest player-visible name: `Damage Forecast`; Mod ID and filename unchanged.
- BaseLib player copy: `Enable Damage Forecast HUD` / `启用伤害预测 HUD` and explicit local-player multiplayer labels in both languages.
- Current product prose reconciled section-by-section; D3 records and historical sections preserved.
- Stable/beta Release restore/build/publish passed with 0 warnings and 0 errors.
- Stable/beta artifacts are byte-identical: DLL SHA256 `C7FC2B9FEC076CE4389146FD4469E78474EFDD3951DAA5BA27818C28898A2F37`; manifest SHA256 `A0CCDE08C2C7C0DB1C5D3BFC374C7523DFF0ECF492D00E6F154BF1AE1DAC5E11`.
- Contract tests: 21/21 passed against stable v0.107.1 and beta v0.109.0.
- Assembly identity, Mod ID, BaseLib key, Harmony owner, namespace, `PartyWatch*` symbols, paths, Workshop ID, and `[STS2 Party Watch]` diagnostic prefix retained.
- Install/game runtime/commit/push: none.
- G4 remains pending separate approval.
