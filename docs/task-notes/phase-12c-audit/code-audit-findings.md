# Phase 12C-A Code Audit Findings

> Authority: detailed static code-audit findings for Phase 12C-A.
> Fix authorization: G2 first and second batches approved, applied, verified, and formally closed; remaining Confirmed/Future-Fix items are outside the closed G2 scope.
> Build evidence: G1 and both post-G2 dual-target Release builds completed 2026-07-19.
> Runtime evidence: user-run verification received through 2026-07-21; AUD-0007 instrumented A/B measurement completed and Production restored; Codex did not launch the game.

## Status Summary

- Confirmed: 2
- Needs build verification: 0
- Needs runtime verification: 0
- Accepted for future fix: 1
- Resolved under G2/measurement: 10
- Rejected: 0
- Severity: Blocker 0, High 0, Medium 5, Low 8.
- G2 fixes applied: AUD-0001 through AUD-0006 and AUD-0011 through AUD-0013 where authorized; AUD-0003/AUD-0005/AUD-0013 are runtime-verified and resolved, AUD-0007 is resolved as measured non-material without an optimization, and AUD-0010 has an initial automated contract harness.

## Findings

### AUD-0001 — Tracked repository cannot restore its declared BaseLib dependency by itself

- Status: Resolved
- Severity: Medium
- Confidence: High
- Evidence level: E3
- Category: Reliability / Maintainability
- Location: `src/STS2PartyWatchCode/STS2PartyWatchCode.csproj:13-23`; `scripts/Restore-BaseLibDependency.ps1`; `scripts/Build-DualTargets.ps1`
- Related audit unit: A01
- Evidence: the package reference was replaced by a direct compile-only reference. The tracked bootstrap downloads the official BaseLib 3.3.4 release asset, requires SHA256 `C593F14EAAB504FC1D31C89DA7C029116D269F65706D9612D6F71A048E504235`, and stores it only under ignored `work/`. `dotnet list package` reports no package references.
- G2 evidence: bootstrap download/hash validation passed, the existing-file hash-only path passed, and stable/beta Release restore/build/publish both passed with 0 warnings and 0 errors without the ignored local NuGet source or package payload.
- Reasoning: a tracked clean checkout now has a deterministic public bootstrap path while avoiding committed third-party/game binaries and machine-local source configuration.
- Player/developer impact: clean-machine, disaster-recovery, CI, and future-maintainer builds no longer depend on the old ignored NuGet source.
- Recommended fix: Completed under G2; keep the version and digest updated together when BaseLib is deliberately upgraded.
- Required build verification: Completed against stable v0.107.1 and beta v0.109.0.
- Required runtime verification: No.
- Blocks: Phase 13A / Phase 13B clean-environment work; does not block static Phase 12C-B copy changes.
- Fix authorized: Yes (G2 second batch); resolved.
- Related rename IDs: REN-0007, REN-0008.
- Notes: BaseLib remains a runtime manifest dependency and is not included in the published Party Watch folder.

### AUD-0002 — Generic turn-end card scan can claim Known for unverified conditional or multi-variable cards

- Status: Resolved
- Severity: Medium
- Confidence: High
- Evidence level: E3
- Category: Correctness / Compatibility
- Location: `Combat/CardTurnEndDamageInspector.cs:14-50`; `Combat/LocalIncomingDamageReader.cs:645-660,696-709`
- Related audit unit: A03
- Evidence: the IL scanner answers only whether a card's turn-end method contains any `CreatureCmd.Damage` call. The reader then sums every blockable `DamageVar` on that card; it does not map a specific call to a specific dynamic variable, evaluate branch conditions, or restrict the result to an allowlist of verified card types.
- G2 fix/evidence: `CardTurnEndDamageInspector.TryGetVerifiedSingleBlockableDamageVar` now accepts only exact built-in `Burn` with exactly one blockable `DamageVar`; any detected unverified/derived/multi-variable/unblockable shape propagates Unknown. Seven contract tests include exact type, derived type, unverified type, multiple-variable, and unblockable cases and pass for both stable and beta builds.
- Reasoning: the verified Burn path is compatible with this heuristic, but a base-game update or third-party card with conditional damage, multiple `DamageVar` values, or a non-turn-end helper call can produce a precise but incorrect Known result.
- Player/developer impact: supported base-game scenes can remain correct while modded/future cards silently overcount or include damage that will not execute.
- Recommended fix: separate verified built-in readers from a conservative generic fallback; require unambiguous call-to-variable mapping or return Unknown for unverified shapes.
- Required build verification: compile any refactor against both reference targets.
- Required runtime verification: historical Burn runtime evidence remains applicable; an optional modded conditional/multi-`DamageVar` runtime regression can further validate the Unknown presentation path.
- Blocks: Phase 13B expansion and any claim of generic mod-card compatibility; not a Phase 12C-B rename blocker.
- Fix authorized: Yes (G2); applied and verified by contract tests plus stable/beta builds.
- Related rename IDs: none
- Notes: the generic shape is no longer trusted. Existing Burn evidence plus the pure rejection-policy tests close the registered defect.

### AUD-0003 — Pre-attack relic lookup does not exclude melted relic instances

- Status: Resolved
- Severity: Medium
- Confidence: High
- Evidence level: E4
- Category: Correctness / Lifecycle
- Location: `Combat/VerifiedPreAttackBlockReader.cs:67-132`
- Related audit unit: A03
- Evidence: `FindRelic<TRelic>` returns the first typed relic without checking `IsMelted`, while the HP-loss and legacy Diamond readers explicitly filter `!relic.IsMelted`; HUD refresh explicitly patches `Player.MeltRelicInternal`.
- G2 fix/evidence: `FindRelic<TRelic>` now filters `!relic.IsMelted`; stable and beta Release builds pass with 0 warnings/errors. On 2026-07-20 the user verified that melting/removing the relic refreshes the forecast correctly with no stale contribution.
- Reasoning: if melted relics remain in `Player.Relics`, Orichalcum, FakeOrichalcum, Ripple Basin, or Cloak Clasp can continue contributing predicted Block after melt.
- Player/developer impact: expected loss and optional incoming-damage values may be understated after relic melt.
- Recommended fix: after confirming collection semantics, filter melted relics consistently.
- Required build verification: dual-target compile after any fix.
- Required runtime verification: Completed for the reported melt/remove-and-refresh scenario.
- Blocks: None.
- Fix authorized: Yes (G2); code fix and runtime verification complete.
- Related rename IDs: none
- Notes: user result: successful; forecast refresh after relic removal is correct.

### AUD-0004 — Incoming-damage Known zero is rendered despite the current zero-output-hidden contract

- Status: Resolved
- Severity: Low
- Confidence: High
- Evidence level: E2
- Category: Correctness / HUD
- Location: `Combat/LocalIncomingDamageReader.cs:371,375`; `UI/PartyWatchHudDisplay.cs:30-44`; `docs/project-state.md:31`
- Related audit unit: A03 / A07
- Evidence: `IncomingDamageDisplayRead.Known(...)` now converts zero and negative totals to `Hidden`; contract tests cover both non-positive and positive results.
- Reasoning: implementation now follows the existing zero-output-hidden product contract at the typed result boundary.
- Player/developer impact: incoming-only/both mode no longer emits a literal `0` after selected defenses reduce the total to zero.
- Recommended fix: Completed under G2.
- Required build verification: Completed against stable and beta with 10 contract tests on each target.
- Required runtime verification: No for this pure result contract; the broader Phase 13A HUD matrix remains pending separately.
- Blocks: Phase 13A semantic closure; not Phase 12C-B rename.
- Fix authorized: Yes (G2 second batch); resolved.
- Related rename IDs: none
- Notes: the Phase 13A combat HUD matrix is explicitly incomplete.

### AUD-0005 — Covering-screen hide clears the committed turn-end snapshot rather than only hiding it

- Status: Resolved
- Severity: Medium
- Confidence: High
- Evidence level: E4
- Category: Lifecycle / HUD
- Location: `Patches/ForecastRefreshPatch.cs:80-84`; `UI/PartyWatchHudVisibilityPolicy.cs:50`; `UI/PartyWatchHudSnapshotStore.cs:50-56,75-105`
- Related audit unit: A04 / A07
- Evidence: every false visibility decision calls the global snapshot `Clear()`. A covering screen is one such decision. `Clear()` resets `_hasPlayerEndedTurn`; after the screen closes, the next refresh can enter the live-result path instead of restoring the committed end-turn snapshot.
- G2 fix/evidence: visibility policy now reports `temporarilyCovered`; refresh hides labels without clearing the snapshot only for that temporary reason, while permanent invalidation still clears. Stable/beta builds pass. On 2026-07-20 the user verified covering-screen close restores the frozen snapshot correctly.
- Reasoning: ordinary open/hide behavior is documented, but the combined sequence “commit -> open covering screen -> close during enemy-side activity” is not closed by existing runtime evidence and is listed in the remaining Phase 13A matrix.
- Player/developer impact: a frozen number may disappear permanently or be replaced with a newly calculated value after closing an overlay.
- Recommended fix: first verify the exact sequence; if reproduced, distinguish temporary visibility suppression from lifecycle invalidation.
- Required build verification: compile after any state-policy change.
- Required runtime verification: Completed for the user-reported covering-screen/frozen-snapshot scenario.
- Blocks: None.
- Fix authorized: Yes (G2); lifecycle fix and runtime verification complete.
- Related rename IDs: none
- Notes: user result: OK.

### AUD-0006 — Initialization latch is set before fallible registration and patching complete

- Status: Resolved
- Severity: Low
- Confidence: High
- Evidence level: E3
- Category: Reliability / Harmony
- Location: `MainFile.cs:13-30`
- Related audit unit: A05
- Evidence: `_initialized = true` is assigned before config construction/binding/registration and before `Harmony.PatchAll`; there is no rollback or completed-state distinction.
- G2 fix/evidence: `_initializing` now prevents re-entrant initialization, `_initialized` is assigned only after config registration, settings application, Harmony patching, and load logging complete, and `_initializing` is reset in `finally`. Stable/beta builds pass.
- Reasoning: an exception leaves the process marked initialized even though only a prefix of initialization ran.
- Player/developer impact: if the loader retries initialization, the retry is suppressed and the Mod remains partially initialized; the original exception may still prevent load entirely.
- Recommended fix: in a separate task, latch only after successful completion or use explicit initializing/initialized states with safe cleanup.
- Required build verification: stable and beta builds.
- Required runtime verification: controlled failure injection at config registration and patching if a retry path exists.
- Blocks: None.
- Fix authorized: Yes (G2); applied and build-verified.
- Related rename IDs: REN-0004
- Notes: controlled failure injection remains useful hardening evidence but is no longer required to establish that the success latch is assigned after completion.

### AUD-0007 — Global CanvasItem visibility patches add unmeasured work to every UI Show/Hide operation

- Status: Resolved
- Severity: Low
- Confidence: High
- Evidence level: E4
- Category: Performance / Harmony
- Location: `Patches/NativeCoveringScreenLifecyclePatch.cs:57-98`; `UI/PartyWatchNativeCoveringScreenTracker.cs:70-81`
- Related audit unit: A05 / A08
- Evidence: Instrumented beta A4 patched all three targets and recorded 30,993 callbacks over 510.002 seconds: 60.770 calls/second, 0.368 callback ms/second overall, and approximately 0.520 callback ms/second in the derived overlay interval. Type checking averaged 5.934 us; maximum complete callback was 807.300 us and maximum triggered refresh was 611.300 us. Two real transitions produced four matched notifications and four refreshes (2.0 refreshes/transition). The patch-disabled B control produced no perceptible performance difference for the user.
- Reasoning: the integration surface is broad, but measured aggregate cost stayed below the task's approximately 1 ms/second non-material threshold, no callback/refresh maximum exceeded 1 ms, and the A/B control showed no repeatable subjective difference. Duplicate delivery exists but did not cross the more-than-three-refreshes-per-transition optimization trigger.
- Player/developer impact: no material performance impact was established. A small redundant-refresh opportunity remains documented, as does a separate `NCardPileScreen` covering-screen coverage gap.
- Recommended fix: none for performance. Retain the default-off compile-gated measurement path for future feature regression checks; handle `NCardPileScreen` only in a separate compatibility gate if desired.
- Required build verification: No for measurement; dual-target build after any patch change.
- Required runtime verification: Complete; A4 instrumented run plus patch-disabled B user control completed on beta `v0.109.0`.
- Blocks: None.
- Fix authorized: No optimization authorized or warranted.
- Related rename IDs: REN-0006
- Notes: Production SHA256 `3D558648BF9F359172773ECF44381528F34B75368B99F6B9C9FD9A7F4DAA5639` was restored after B. All A/A2/A3/A4/B/B-final test build directories were deleted at the user's request; the task card and raw A4 AppData log retain the evidence. FPS/1% low is N/A because no in-game facility was available.

### AUD-0008 — Open dropdown popup items remain unlocalized

- Status: Confirmed
- Severity: Low
- Confidence: High
- Evidence level: E4
- Category: Configuration / HUD
- Location: `Settings/PartyWatchBaseLibConfig.cs:340-384`; `docs/task-notes/phase-12b-config-localization-language-color.md:345-394`
- Related audit unit: A06
- Evidence: existing user runtime verification records that body and closed values localize, but opened popup items still show raw enum identifiers after both the scanner and `_items` rewrite attempts. Current source retains the unsuccessful `_items` rewrite.
- Reasoning: this is a confirmed current player-visible gap, not a hypothetical reflection risk.
- Player/developer impact: Chinese UI becomes partially English/code-like when dropdowns open.
- Recommended fix: separate evidence-led BaseLib popup lifecycle investigation; prefer supported localization resources if feasible.
- Required build verification: stable/beta build for any fix.
- Required runtime verification: open each language, damage-mode, placement, and anchor dropdown.
- Blocks: full localization closure; does not block basic Surface Rename if accurately disclosed.
- Fix authorized: No
- Related rename IDs: REN-0002, REN-0006
- Notes: do not stack another reflection guess without runtime timing evidence.

### AUD-0009 — In-combat BaseLib configuration route is visible but unusable

- Status: Confirmed
- Severity: Low
- Confidence: High
- Evidence level: E4
- Category: Configuration / Integration
- Location: `docs/project-state.md:142-145`; `docs/task-notes/phase-12b-config-localization-language-color.md:182-186,341-343`
- Related audit unit: A06
- Evidence: existing runtime records consistently identify the main-menu Mod Configuration route as supported and the in-combat built-in route as visible but unusable.
- Reasoning: a visible entry that cannot complete its action is a current product limitation even though it is already documented and deferred.
- Player/developer impact: players can reach a dead configuration entry during combat.
- Recommended fix: treat as an independent BaseLib integration task; do not mix with rename.
- Required build verification: after any integration fix.
- Required runtime verification: open from main menu and in-combat settings.
- Blocks: None.
- Fix authorized: No
- Related rename IDs: REN-0001, REN-0002
- Notes: status Accepted for Future Fix would also be reasonable; retained as Confirmed because it remains user-visible.

### AUD-0010 — Automated coverage remains limited to an initial contract harness

- Status: Accepted for Future Fix
- Severity: Medium
- Confidence: High
- Evidence level: E3
- Category: Testing / Maintainability
- Location: repository-wide project/file inventory; only `src/STS2PartyWatchCode/STS2PartyWatchCode.csproj` exists
- Related audit unit: A09
- Evidence: G2 added `tests/STS2PartyWatchCode.ContractTests`, a dependency-light executable contract harness. Seven assertions cover conservative turn-end card-shape acceptance/rejection and negative-result clamping, and pass against both stable and beta references. Complex calculations, lifecycle state, Harmony integration, configuration mapping, and display formatting remain largely manual/runtime-verified.
- Reasoning: the literal absence of automated tests is fixed, but the initial seam covers only a narrow pure contract and does not yet provide broad regression detection across the game-integrated surface.
- Player/developer impact: small changes can regress edge cases without a fast deterministic signal; Phase 13B will enlarge this risk.
- Recommended fix: incrementally add pure forecast/display contracts and extracted game-state adapters; keep Harmony/Godot/game integration in a separate runtime matrix.
- Required build verification: test project must compile/run in a clean documented environment.
- Required runtime verification: still required for Harmony/Godot/game integration.
- Blocks: Phase 13B readiness recommendation; not static Surface Rename.
- Fix authorized: Yes (G2) for the initial harness; broader coverage accepted as future work.
- Related rename IDs: none
- Notes: command used for each reference target: `dotnet run --project tests/STS2PartyWatchCode.ContractTests/STS2PartyWatchCode.ContractTests.csproj -c Release -p:Sts2ReferenceRoot=<target>`.

### AUD-0011 — Failure fallbacks are largely silent, obscuring why the HUD becomes Unknown or uses native values

- Status: Resolved
- Severity: Low
- Confidence: High
- Evidence level: E1
- Category: Reliability / Diagnostics
- Location: representative broad catches in `LocalIncomingDamageReader.cs:53-60,671-718`, `VerifiedEnemyDamageModifier.cs:15-37`, `LegacyDiamondDiademDamageForecast.cs:19-52`, `EnemyPreActionSurvivalPreview.cs:160-178,251-266`
- Related audit unit: A08 / A09
- Evidence: the representative compatibility/reflection catches now call `PartyWatchDiagnostics.ReportOnce(...)` with stable boundary codes before preserving their conservative fallback. A concurrent once-only code set prevents repeated hot-path output.
- Reasoning: conservative fallback behavior is unchanged, while a first failure now distinguishes the affected reader/compatibility boundary in logs.
- Player/developer impact: missing/Unknown/native fallback regressions have an actionable reason code without per-frame log spam.
- Recommended fix: Completed under G2; add new stable codes when future compatibility boundaries are introduced.
- Required build verification: Completed; stable/beta builds passed and the once-only contract passed on both targets.
- Required runtime verification: Optional forced-failure log capture; not required to close the rate-limit/static instrumentation finding.
- Blocks: efficient G1/runtime diagnosis; not product loading.
- Fix authorized: Yes (G2 second batch); resolved.
- Related rename IDs: REN-0003
- Notes: the legacy diagnostic prefix may remain unchanged.

### AUD-0012 — Poison unsupported-state contract is inert in current code

- Status: Resolved
- Severity: Low
- Confidence: High
- Evidence level: E2
- Category: Correctness / Maintainability
- Location: `Combat/EnemyPreActionSurvivalPreview.cs:39-45,93-96,339-375`; `docs/architecture.md:63-72`
- Related audit unit: A03
- Evidence: the always-true `Supported` fields, always-null reason field/gate, and unused `Unsupported(...)` factory were removed. Current architecture/interface documents now state the actual conservative keep-intent semantics.
- Reasoning: the result type now exposes only states that current code can produce, eliminating a misleading dormant contract without changing the intended Phase 11C fallback behavior.
- Player/developer impact: maintainers no longer mistake conservative keep-intent output for an activated unsupported channel.
- Recommended fix: Completed under G2; add an explicit typed state only if a future path genuinely needs and produces it.
- Required build verification: Completed against stable and beta.
- Required runtime verification: the broader Poison special-combination matrix remains pending, but it is separate from this dead-contract cleanup.
- Blocks: complete Poison support claims; not Phase 12C-B.
- Fix authorized: Yes (G2 second batch); resolved.
- Related rename IDs: none
- Notes: current runtime-verified narrow Phase 11C cases remain evidence and are not rejected by this finding.

### AUD-0013 — Observed HP-loss budgets have no combat-end cleanup path

- Status: Resolved
- Severity: Low
- Confidence: High
- Evidence level: E4
- Category: Lifecycle / Reliability
- Location: `Combat/ObservedHpLossBudgetTracker.cs:6-68`; `Patches/ForecastRefreshPatch.cs:535-541`
- Related audit unit: A04 / A08
- Evidence: the static dictionary is reset per owner window by the Beating Remnant patch, but exposes no Clear/remove-all method; `AfterCombatEnd` clears only HUD snapshots.
- G2 fix/evidence: `ObservedHpLossBudgetTracker.Clear()` was added and is called from `Hook.AfterCombatEnd` beside snapshot cleanup; stable/beta builds pass.
- Runtime observation (2026-07-20): adding Beating Remnant through a debug path during an active combat made expected `-N` and both expected-loss detail lanes disappear while incoming `N` remained; removing the relic restored them. The user then confirmed that obtaining/equipping the relic before entering the next combat does not reproduce the symptom.
- Reasoning: the normal game boundary initializes/resets the observed HP-loss window correctly. Mid-combat relic injection changes the modifier set inside an already active observation window and is not evidence of failed combat-end cleanup.
- Player/developer impact: no reproduced defect in the normal obtain-relic-then-enter-combat flow. The conservative display fallback is limited to the non-normal mid-combat debug injection boundary.
- Recommended fix: Completed under G2; do not change production behavior for the debug-only injection case.
- Required build verification: Completed against stable and beta after the lifecycle patch.
- Required runtime verification: Completed for the normal relic-acquisition/next-combat path; the cross-combat cleanup hypothesis was not reproduced.
- Blocks: None.
- Fix authorized: Yes (G2); cleanup fix and runtime verification complete.
- Related rename IDs: none
- Notes: mid-combat console/debug injection remains an explicitly non-production test boundary, not an open gameplay bug.

## A11 Cross-Validation and Deduplication

- All 13 findings were rechecked against source, current/historical documentation, and available runtime evidence. No Candidate status remains.
- No finding was promoted to Blocker or High. Static evidence does not show a current load failure, data-loss path, or broadly wrong base-game forecast that justifies either severity.
- AUD-0002 remains distinct from AUD-0012: the former is a generic card-shape precision defect; the latter is a dormant Poison support-state contract.
- AUD-0003, AUD-0005, AUD-0007, and AUD-0013 remain `Needs Runtime Verification`; static evidence establishes a plausible path but not its occurrence/frequency under current game semantics.
- AUD-0008 and AUD-0009 remain separate confirmed BaseLib limitations: popup localization and the in-combat route have different causes, verification matrices, and future fixes.
- AUD-0004 is a confirmed implementation/documentation contract conflict, not yet a mandated code change; G2 must choose whether zero is hidden or documented as visible.
- Historical mechanics evidence cross-validates the current turn-end ordering of Frost/Plating/Orichalcum-related block. The initially suspected “future block applied too early” issue was rejected before registration and is not a finding.
- Historical Intangible evidence cross-validates use of native blockable-damage modification and direct-HP-loss adjustment. No duplicate Intangible finding was registered.
- Phase 13A is already implemented in the current working tree. Its remaining closure is the runtime matrix plus AUD-0002 through AUD-0005; Phase 13B should not expand the surface before the Medium findings and test strategy receive explicit disposition.

## G1 Build and Static Verification Update

- Stable `v0.107.1` Release restore/build/publish: Passed, 0 warnings, 0 errors.
- Beta `v0.109.0` Release restore/build/publish: Passed, 0 warnings, 0 errors.
- Both published DLLs: 108,544 bytes; SHA256 `B8E5ACB84580BCE901CA51623522B86560CFF178A07CD0F33619E1E49FAB7E8A`.
- Both published manifests: 376 bytes; SHA256 `A12AE8A5FD44292DFF347350CEB64B005C54DE82D5155F5CE1B50A2F6C6C96BA`, identical to the source manifest.
- Output whitelist: exactly the DLL and manifest per target; no `.deps.json`, `.pdb`, `.pck`, or `.log` files.
- Assembly identity: `sts2-party-watch-v2`, version `1.0.0.0`, unchanged for both targets.
- Static check: `git diff --check` produced no whitespace errors. Existing LF-to-CRLF notices are Git working-copy warnings, not diff-check failures.
- Tests: no test project/source exists, so no automated test suite could be executed; AUD-0010 remains Confirmed.
- Runtime: not performed. AUD-0003, AUD-0005, AUD-0007, and AUD-0013 remain `Needs Runtime Verification`.
- Finding statuses after G1: unchanged except AUD-0001 evidence promoted to E3. No finding was resolved because no product fix was authorized or applied.

## G2 First-Batch Fix Update

- Resolved: AUD-0002 (conservative verified Burn shape) and AUD-0006 (completion-only initialization latch).
- Fix applied; runtime verification retained: AUD-0003 (melted relic filter), AUD-0005 (temporary covering-screen snapshot preservation), AUD-0013 (combat-end HP-loss-budget cleanup).
- Accepted for future expansion: AUD-0010 now has a seven-case contract harness passing against stable and beta; broader lifecycle/Harmony/configuration coverage remains open.
- Not changed: AUD-0001, AUD-0004, AUD-0007, AUD-0008, AUD-0009, AUD-0011, AUD-0012.
- G2 build evidence: stable and beta Release restore/build/publish pass with 0 warnings and 0 errors; published target artifacts remain byte-identical.
- Runtime/game evidence: none added; no Mod installation or game launch occurred.

## G2 Second-Batch Fix Update

- Resolved: AUD-0001 (official hash-pinned BaseLib bootstrap/direct reference), AUD-0004 (zero-hidden result boundary), AUD-0011 (once-only stable diagnostics), and AUD-0012 (dead Poison unsupported contract removed).
- Contract evidence: 10 assertions pass against stable and beta, including non-positive/positive incoming totals and once-only diagnostic gating.
- Dependency evidence: verified BaseLib 3.3.4 bootstrap asset; `dotnet list package` reports no project package references.
- Build evidence: stable and beta Release restore/build/publish pass with 0 warnings and 0 errors; each publish folder contains only the required DLL and manifest, and target artifacts are byte-identical.
- Current statuses after 2026-07-20 user runtime evidence: 2 Confirmed, 1 Needs Runtime Verification, 1 Accepted for Future Fix, 9 Resolved.
- Runtime/game evidence: AUD-0003, AUD-0005, AUD-0013, and the Phase 13A core HUD scenes passed. AUD-0007 is deferred to its dedicated performance task card.
