# Phase 12C-A Document Disposition Register

> Authority: complete document/note inventory and proposed disposition.
> Product-document modification authorization: Yes for the G3 current-prose scope completed on 2026-07-21.

## Baseline

- Tracked documentation baseline: 24 files (`README.md` plus 23 files under `docs`).
- Untracked documentation/asset candidates: 18 files under `docs` (15 Markdown files and 3 PNG assets).
- Formal document coverage denominator: 24 tracked documents.
- Candidate coverage denominator: 18 untracked documents/assets, reported separately.

## Register

| ID | Path | Tracked | Role | Authority | Old-name hits | Disposition | Literals preserved | Rename file? | Link impact | Approval | Status |
|---|---|---|---|---|---:|---|---|---|---|---|---|
| DOC-0001 | `README.md` | Yes | D1 ACTIVE_CANONICAL | Primary user entry | 12 lines | Rewrite current product/config/scope prose | Technical IDs/paths in commands | No | None | G3 for rename prose | Audited; stale |
| DOC-0002 | `docs/architecture.md` | Yes | D5 TECHNICAL_REFERENCE | Current architecture reference | 13 lines | Update current Poison/config/Phase 13A descriptions | Symbols, namespaces, paths | No | None | G2/G3 by subject | Audited; stale sections |
| DOC-0003 | `docs/build-environment.md` | Yes | D5 TECHNICAL_REFERENCE | Current build reference | 24 lines | Update visible product prose only | Project, DLL, manifest, Mod IDs and paths | No | None | G3 | Audited |
| DOC-0004 | `docs/compatibility.md` | Yes | D5 TECHNICAL_REFERENCE | Current compatibility reference | 0 | Keep; update only if a finding is fixed | All technical literals | No | None | G2 if needed | Audited |
| DOC-0005 | `docs/decisions.md` | Yes | D4 HYBRID | Decision log with current implications | 3 lines | Add supersession markers for D015/D021; do not rewrite decisions | Historical quotations and technical IDs | No | None | G2/G3 | Audited; stale implications |
| DOC-0006 | `docs/interface-map.md` | Yes | D5 TECHNICAL_REFERENCE | Current interface map | 13 lines | Correct current Poison/native-call map; update surface prose | Symbols, type names, IDs | No | None | G2/G3 | Audited; stale row |
| DOC-0007 | `docs/mechanics-evidence.md` | Yes | D4 HYBRID | Evidence plus current mechanics conclusions | 11 lines | Preserve evidence; update current conclusions only | Evidence names, symbols and paths | No | None | G2/G3 | Audited |
| DOC-0008 | `docs/project-state.md` | Yes | D4 HYBRID | Current state plus chronology | 29 lines | Reconcile Phase 12A/12B/13A and next-task sections | Historical events; technical IDs/paths | No | Technical URL review | G2/G3 | Audited; internally contradictory |
| DOC-0009 | `docs/task-notes/README.md` | Yes | D4 HYBRID | Task-note index/current task pointer | 5 lines | Replace stale Phase 12B current-task pointer; preserve chronology | Note filenames and technical literals | No | None | G2/G3 | Audited; internally contradictory |
| DOC-0010 | `docs/task-notes/modding-settings-entry-2026-07-02.md` | Yes | D3 HISTORICAL_RECORD | Dated runtime/design record | 20 lines | Preserve verbatim | All historical/technical literals | No | None | None | Audited; retain |
| DOC-0011 | `docs/task-notes/phase-0-repository-bootstrap.md` | Yes | D3 HISTORICAL_RECORD | Completed phase record | 1 line | Preserve verbatim | All historical/technical literals | No | None | None | Audited; retain |
| DOC-0012 | `docs/task-notes/phase-1-4-singleplayer-baseline.md` | Yes | D3 HISTORICAL_RECORD | Completed phase record | 6 lines | Preserve verbatim | All historical/technical literals | No | None | None | Audited; retain |
| DOC-0013 | `docs/task-notes/phase-11-supplements.md` | Yes | D4 HYBRID | Supplement index plus current guidance | 32 lines | Preserve dated results; reconcile current guidance | Historical evidence and technical literals | No | None | G2/G3 | Audited |
| DOC-0014 | `docs/task-notes/phase-5-blockable-incoming-damage.md` | Yes | D3 HISTORICAL_RECORD | Completed phase record | 8 lines | Preserve verbatim | All historical/technical literals | No | None | None | Audited; retain |
| DOC-0015 | `docs/task-notes/phase-6-direct-hp-loss.md` | Yes | D3 HISTORICAL_RECORD | Completed phase record | 6 lines | Preserve verbatim | All historical/technical literals | No | None | None | Audited; retain |
| DOC-0016 | `docs/task-notes/phase-7-hp-loss-result-modifiers.md` | Yes | D3 HISTORICAL_RECORD | Completed phase record | 0 | Preserve verbatim | All historical/technical literals | No | None | None | Audited; retain |
| DOC-0017 | `docs/task-notes/phase-8-non-block-damage-modifiers.md` | Yes | D3 HISTORICAL_RECORD | Completed phase record | 4 lines | Preserve verbatim | All historical/technical literals | No | None | None | Audited; retain |
| DOC-0018 | `docs/task-notes/phase-9-singleplayer-validation.md` | Yes | D3 HISTORICAL_RECORD | Completed validation record | 15 lines | Preserve verbatim | All historical/technical literals | No | None | None | Audited; retain |
| DOC-0019 | `docs/task-notes/phase-9a-followup-2026-07-01.md` | Yes | D3 HISTORICAL_RECORD | Dated follow-up | 7 lines | Preserve verbatim | All historical/technical literals | No | None | None | Audited; retain |
| DOC-0020 | `docs/task-notes/phase-9a-ui-settings-lifecycle.md` | Yes | D3 HISTORICAL_RECORD | Completed phase record | 8 lines | Preserve verbatim | All historical/technical literals | No | None | None | Audited; retain |
| DOC-0021 | `docs/task-notes/phase-9b-poison-pre-action-survival.md` | Yes | D3 HISTORICAL_RECORD | Completed phase record | 10 lines | Preserve verbatim | All historical/technical literals | No | None | None | Audited; retain |
| DOC-0022 | `docs/task-notes/project-total-note.md` | Yes | D4 HYBRID | Project summary with chronology | 10 lines | Reconcile config/Poison/current status; preserve history | Historical and technical literals | No | None | G2/G3 | Audited; stale sections |
| DOC-0023 | `docs/task-notes/workshop-private-rc-2026-07-01.md` | Yes | D3 HISTORICAL_RECORD | Dated release record | 16 lines | Preserve verbatim | All historical/technical literals | No | Technical URL review | None | Audited; retain |
| DOC-0024 | `docs/v2-roadmap.md` | Yes | D4 HYBRID | Roadmap with completed history | 6 lines | Reconcile Phase 11C/12B/13A status and next work | Completed-phase history and technical literals | No | None | G2/G3 | Audited; stale |
| DOC-0025 | `docs/task-notes/phase-11a-v108-quick-compatibility.md` | No | D3 HISTORICAL_RECORD | Candidate completed phase record | 23 lines | Preserve verbatim; decide tracking separately | All historical/technical literals | No | None | Acceptance outside 12C-B | Audited; retain candidate |
| DOC-0026 | `docs/task-notes/phase-11b-stable-dll-initialization.md` | No | D3 HISTORICAL_RECORD | Candidate completed phase record | 14 lines | Preserve verbatim; decide tracking separately | All historical/technical literals | No | None | Acceptance outside 12C-B | Audited; retain candidate |
| DOC-0027 | `docs/task-notes/phase-11c-poison-runtime-verification-safe-expansion.md` | No | D3 HISTORICAL_RECORD | Candidate runtime evidence record | 19 lines | Preserve verbatim with referenced PNG | All historical/technical literals | No | None | Acceptance outside 12C-B | Audited; retain candidate |
| DOC-0028 | `docs/task-notes/phase-11d-diamond-diadem-mechanism-compatibility.md` | No | D3 HISTORICAL_RECORD | Candidate completed phase record | 3 lines | Preserve verbatim; decide tracking separately | All historical/technical literals | No | None | Acceptance outside 12C-B | Audited; retain candidate |
| DOC-0029 | `docs/task-notes/phase-11e-dual-version-build-baseline.md` | No | D3 HISTORICAL_RECORD | Candidate completed phase record | 6 lines | Preserve verbatim; decide tracking separately | All historical/technical literals | No | None | Acceptance outside 12C-B | Audited; retain candidate |
| DOC-0030 | `docs/task-notes/phase-12a-baselib-auto-config-evaluation.md` | No | D3 HISTORICAL_RECORD | Candidate evaluation record | 36 lines | Preserve verbatim | All historical/technical literals | No | None | Acceptance outside 12C-B | Audited; retain candidate |
| DOC-0031 | `docs/task-notes/phase-12a-completion-handoff-2026-07-03.md` | No | D3 HISTORICAL_RECORD | Candidate dated handoff | 8 lines | Preserve verbatim with two referenced PNGs | All historical/technical literals | No | None | Acceptance outside 12C-B | Audited; retain candidate |
| DOC-0032 | `docs/task-notes/phase-12a-runtime-verification-2026-07-03.md` | No | D3 HISTORICAL_RECORD | Candidate runtime evidence record | 5 lines | Preserve verbatim with two embedded PNGs | All historical/technical literals | No | None | Acceptance outside 12C-B | Audited; retain candidate |
| DOC-0033 | `docs/task-notes/phase-12a-session-handoff-2026-07-03.md` | No | D3 HISTORICAL_RECORD | Candidate dated handoff | 67 lines | Preserve verbatim | All historical/technical literals | No | Technical URL review | Acceptance outside 12C-B | Audited; retain candidate |
| DOC-0034 | `docs/task-notes/phase-12b-config-localization-language-color.md` | No | D4 HYBRID | Candidate implementation record with current limitations | 53 lines | Preserve results; keep current limitation/status section authoritative | Historical evidence, symbols and IDs | No | None | G2/G3 plus acceptance | Audited |
| DOC-0035 | `docs/task-notes/phase-13a-damage-display.md` | No | D4 HYBRID | Candidate implementation/current validation record | 17 lines | Preserve implementation history; update unresolved validation/finding links | Historical evidence, symbols and IDs | No | None | G1/G2/G3 plus acceptance | Audited |
| DOC-0036 | `docs/task-notes/session-handoff-dual-target-build-baseline-2026-07-18.md` | No | D3 HISTORICAL_RECORD | Candidate dated handoff | 6 lines | Preserve verbatim | All historical/technical literals | No | None | Acceptance outside 12C-B | Audited; retain candidate |
| DOC-0037 | `docs/task-notes/session-handoff-dual-target-build-complete-2026-07-18.md` | No | D3 HISTORICAL_RECORD | Candidate dated handoff | 5 lines | Preserve verbatim | All historical/technical literals | No | None | Acceptance outside 12C-B | Audited; retain candidate |
| DOC-0038 | `docs/task-notes/session-handoff-dual-version-compatibility-2026-07-17.md` | No | D3 HISTORICAL_RECORD | Candidate dated handoff | 18 lines | Preserve verbatim | All historical/technical literals | No | Technical URL review | Acceptance outside 12C-B | Audited; retain candidate |
| DOC-0039 | `docs/task-notes/session-handoff-poison-testsubject-intangible-2026-07-18.md` | No | D3 HISTORICAL_RECORD | Candidate runtime handoff/evidence | 14 lines | Preserve verbatim with referenced PNG | All historical/technical literals | No | None | Acceptance outside 12C-B | Audited; retain candidate |
| DOC-0040 | `docs/task-notes/assets/phase-11c-testsubject-intangible-tsi-04-2026-07-18.png` | No | D3 HISTORICAL_RECORD | Candidate runtime screenshot evidence | 0 | Preserve binary unchanged | Visible historical product name if any | No | Referenced by DOC-0027/0039 | Acceptance outside 12C-B | Visually audited; retain |
| DOC-0041 | `docs/task-notes/assets/phase-12a-runtime/in-combat-built-in-entry.png` | No | D3 HISTORICAL_RECORD | Candidate runtime screenshot evidence | 0 | Preserve binary unchanged | Visible historical product/config state | No | Referenced by DOC-0031/0032 | Acceptance outside 12C-B | Visually audited; retain |
| DOC-0042 | `docs/task-notes/assets/phase-12a-runtime/main-menu-mod-config.png` | No | D3 HISTORICAL_RECORD | Candidate runtime screenshot evidence | 0 | Preserve binary unchanged | Visible historical raw property labels/name | No | Referenced by DOC-0031/0032 | Acceptance outside 12C-B | Visually audited; retain |

## Summary

- Coverage: 42/42 pre-G0 items (24 tracked documents plus 15 candidate Markdown files and 3 candidate PNG assets).
- Roles: D1 = 1, D3 = 28, D4 = 9, D5 = 4; D2/D6/D7/D8 = 0.
- Proposed current-prose work: 14 D1/D4/D5 documents; 9 D4 documents require section-level separation of current guidance from history.
- Historical preservation: 28 D3 documents/assets should remain unchanged by Surface Rename.
- Old-name corpus: 37 Markdown files contain matching lines; 5 items contain none. Counts above are matching-line counts, not regex occurrence counts.
- Filename rename recommendation: none. Existing names are technical identity, phase identity, or historical evidence rather than player-facing surface.
- Surface-rename link impact: none because repository, Mod, project, DLL, manifest, and note paths remain unchanged. Four documents contain technical repository URLs and must be rechecked only during a future technical migration: DOC-0008, DOC-0023, DOC-0033, DOC-0038.
- Pending content decisions are routed to G1/G2/G3; whether the 18 untracked candidates become tracked documents is outside Surface Rename scope.

## Authority Conflicts and Drift

1. `README.md` still describes the obsolete own Mod-info settings button and session-only settings, omits the current BaseLib main-menu configuration/persistence behavior, Phase 11C Poison expansion, and Phase 13A incoming-damage mode.
2. `docs/project-state.md` contains both implemented Phase 12B/13A statements and older sections naming Phase 12B as the next task.
3. `docs/v2-roadmap.md` is reconciled only through 2026-07-03 and still presents Phase 12B as pending and special Poison cases as unsupported.
4. `docs/task-notes/README.md` names Phase 12B as the current sole task but later records Phase 13A as implemented.
5. `docs/task-notes/project-total-note.md` retains pre-BaseLib/session-only configuration and pre-Phase-11C Poison scope statements.
6. `docs/interface-map.md` says the Poison path does not call native `CalculateTotalDamageNextTurn`; current code does.
7. `docs/architecture.md` describes special Poison lifecycle as unsupported; current narrow Phase 11C branches use supported conservative/native paths.
8. `docs/decisions.md` D015 and D021 are historical decisions superseded by BaseLib persistence/configuration and need explicit supersession markers rather than rewritten history.
9. The master-plan readiness wording predates the current dirty worktree: Phase 13A is implemented, but its full combat HUD matrix is still incomplete. Readiness review is therefore retrospective; Phase 13B remains future work.

## G1 Document Impact

- G1 modified no existing product document or historical evidence asset.
- Build outputs are ignored/generated and do not change the 42-item document baseline or any DOC disposition.
- Surface-rename document/link validation remains pending G3; the G1 identity check found no technical name or artifact drift that would alter this register.

## G2 Document Impact

- DOC-0003 `docs/build-environment.md` received a current technical section documenting the new stable/beta contract-test commands and explicitly preserving the game-runtime boundary.
- No historical record or player-facing rename prose changed. All other DOC dispositions remain unchanged.

## G3 Document Impact

The `Status` column above records the pre-G3 audit baseline. This closure overrides the stale/pending status for the authorized current-prose set without changing the historical-disposition rows.

- Updated current player/product prose: DOC-0001, DOC-0002, DOC-0005 through DOC-0009, DOC-0013, DOC-0022, DOC-0024, and DOC-0034.
- Reviewed with no player-visible prose change required: DOC-0003 and DOC-0035; their legacy hits are technical identities or historical implementation evidence.
- Preserved unchanged as D3 history: DOC-0010 through DOC-0012, DOC-0014 through DOC-0021, DOC-0023, DOC-0025 through DOC-0033, and DOC-0036 through DOC-0042.
- New G3 execution record: `docs/task-notes/phase-12c-surface-rename.md` (created after the pre-G0 denominator, so it is not retroactively assigned a DOC ID).
- No document or source filename moved. All repository links, commands, paths, artifact names, hashes, screenshot references, and technical URLs remain attributable.
- G3 document status: completed and StaticVerified. Player-visible runtime confirmation remains G4.
