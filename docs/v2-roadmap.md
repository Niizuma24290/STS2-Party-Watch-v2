# v2 Roadmap

Last reconciled: 2026-07-21

Current player-facing name: `Damage Forecast` / `伤害预测`. Completed-phase wording below is preserved as historical evidence where it used the former `Party Watch` name; technical identifiers are unchanged.

## Completed Phases

| Phase | Status | Outcome |
| --- | --- | --- |
| Phase 0 - Bootstrap | Complete | Clean v2 repository, initial docs, remote `main`, and project skeleton established. |
| Phase 1 - Empty mod load proof | Complete | Steam-launched mod load and startup log proved. |
| Phase 2 - Local combat access proof | Complete | Local player, local creature, enemy list, HP/Block, and intent presence access established. |
| Phase 3 - Native RAW reader | Complete | Native `AttackIntent` preview read path established. |
| Phase 4 - Local forecast calculation | Complete | `LocalDamageForecast` and `ForecastResult` established. |
| Phase 5 - HUD integration and blockable damage | Complete | Health-bar HUD, enemy attack, hand turn-end blockable damage, and first EffectiveBlock set verified. |
| Phase 6 - Direct HP loss | Complete | Beckon, Bad Luck, and Regret direct HP loss implemented and Steam verified. |
| Phase 7 - HP-loss result modifiers | Complete within documented scope | TungstenRod and BeatingRemnant forecast path implemented with conservative granularity rules. |
| Phase 8 - Non-Block damage modifiers | Complete within documented scope | Diamond Diadem / DiamondDiademPower implemented and runtime verified in supported single-player cases. |
| Phase 9 - Single-player release cleanup | Complete | Default total `-N`, optional advanced `🛡/♥`, documentation, and single-player scope cleanup completed. |
| Phase 9A - HUD lifecycle and settings | Complete | HUD visibility policy, turn freezing, settings panel, position, and colors implemented. |
| Modding settings entry fix | Complete, historical route | Party Watch stopped occupying the native Settings/Modding main entry. The later Phase 12A/12B BaseLib route supersedes the former own-mod-info-panel route. |
| Local HUD in multiplayer | Complete for local-player HUD only | Workshop subscription runtime evidence shows local player's Party Watch HUD in multiplayer combat. |
| Phase 10 - Workshop upload / subscription milestone | Complete as private/subscription milestone | Workshop item `3755598583`, private visibility, cover/preview uploads, uploaded DLL hash, and subscription local-HUD validation are recorded in `workshop-private-rc-2026-07-01.md`. |
| Phase 11C - Poison safe expansion | Complete within documented scope | HardToKill, Slippery, HardenedShell, and the representative TestSubject phase-3 Intangible exact-lethal boundary are RuntimeVerified; broader matrices remain deferred. |
| Phase 11E - Dual-version build baseline | Complete / BuildVerified | Stable v0.107.1 and beta v0.109.0 frozen-reference builds are reproducible; this phase did not claim complete dual-branch runtime coverage. |
| Phase 12A - BaseLib automatic config baseline | Complete | Party Watch settings migrated to BaseLib automatic `SimpleModConfig`; main-menu Mod Configuration route user-verified usable; in-combat BaseLib route remains a known limitation. |
| Phase 12B - BaseLib config polish | Complete / RuntimeVerified | Damage Forecast English/Simplified Chinese labels, language switching, restart persistence, colors, and BaseLib-compatible layout are verified. |
| Phase 13A - Incoming damage display | Implemented / partially RuntimeVerified | Optional incoming damage `N`, display modes, placement, and calculation switches are implemented; the full combat/lifecycle matrix is not fully backfilled. |
| Phase 12C G0-G5 - Audit, rename, runtime and repository closure | Complete | Audit, Surface Rename, G4 runtime verification, full-history push, GitHub rename, and annotated tags are closed; Workshop was not updated. |

## Current Initiative

### Documentation Authority Consolidation

Status: `DC2 Complete / DC3 Pending Separate Approval`.

Current authority and exact scope:

[`docs/task-notes/documentation-authority-consolidation-master-task-card.md`](task-notes/documentation-authority-consolidation-master-task-card.md)

This task establishes one current authority per subject and routes Phase/audit/
handoff documents as historical evidence. It does not authorize code, tests,
scripts, manifest, Workshop, install, game runtime, Forecast Engine, G6,
commit, push, tag, file moves, or deletions.

## Deferred Validation Candidates

### Poison matrix expansion

Status: optional future validation, not the current task.

Current supported scope:

- ordinary Poison pre-action survival remains implemented;
- HardToKill, Slippery, HardenedShell, and the representative TestSubject
  phase-3 Intangible exact-lethal boundary have narrow RuntimeVerified support;
- unsupported combinations conservatively keep base Intent.

Deferred evidence:

- full ordinary Poison, multi-enemy, and same-name matrix;
- family-specific Nemesis and ToughEgg/Hatch captures;
- special-power combination matrices outside the documented narrow support.

## Workshop State

Status: private/subscription milestone recorded. Do not describe the mod as publicly published unless a public publish record is added.

Current boundaries:

- no DLL, PDB, PCK, cover, uploader, `mod_id.txt`, logs, `bin`, `obj`, `publish`, or `work` content should be committed;
- private/subscription testing is not the same as public release.

## Frozen Future Work

Formal multiplayer HUD remains frozen.

Allowed future research direction:

- investigate real multiplayer targets and native target-aware previews;
- keep local-player HUD distinct from formal teammate/party HUD.

Not implemented:

- teammate top-bar forecasts;
- teammate HUD;
- shared party HUD;
- network messages;
- network state modification;
- guessed multiplayer targets.

## Next Single Direction

Review the completed Documentation Authority Consolidation DC2 authority layer,
then separately decide whether to approve DC3 historical routing/banners. Do
not automatically continue into DC3, physical archive, deletion, commit, push,
or tag.

After documentation closure, the Forecast Engine architecture card remains
Proposed/Queued and must restart from its read-only AR0 revalidation before any
AR1 implementation approval. G6 Full Technical Identity Migration remains an
independent optional decision. Workshop changes and formal multiplayer HUD stay
separately gated and frozen unless explicitly reopened.
