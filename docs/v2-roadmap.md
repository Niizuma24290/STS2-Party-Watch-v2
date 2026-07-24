# v2 Roadmap

Last reconciled: 2026-07-24

Current player-facing and active technical identity: `Damage Forecast` / `伤害预测` / `damage-forecast`. Current settings use `DamageForecast.Settings.DamageForecastBaseLibConfig` / `DamageForecast.cfg`; the isolated compatibility subsystem recognizes the former config only as a legacy migration source. Completed-phase wording below is preserved as historical evidence where it used the former product name.

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
| G6 - Full Technical Identity Migration | Complete / stable+beta verified | Assembly/DLL, manifest, Mod ID, BaseLib key, Harmony owner, namespaces, project/test paths, publish/install identity, and diagnostics migrated to `damage-forecast` / `DamageForecast`; 18-setting persistence stayed on `STS2PartyWatch.cfg`; beta smoke and the full stable migration matrix passed. |
| Post-G6 C1-C4 - Full name unification | Complete / stable+beta verified | Ordinary code, current config identity, Godot nodes, active-scope guardrails, compatibility token inventory, current authority, stable full migration matrix, beta smoke, and approved cleanup are closed; Workshop remains unchanged. |

## Current Initiative

### Documentation Authority Consolidation

Status: `Closed — DC0-DC6 Complete / DC4 Skipped`.

Current authority and exact scope:

[`docs/task-notes/documentation-authority-consolidation-master-task-card.md`](task-notes/documentation-authority-consolidation-master-task-card.md)

This task establishes one current authority per subject and routes Phase/audit/
handoff documents as historical evidence. It does not authorize code, tests,
scripts, manifest, Workshop, install, game runtime, Forecast Engine, G6,
commit, push, tag, file moves, or deletions.

### Forecast Engine Test Guardrails

Status: `Closed — TG0-TG7 Complete`.

Current task card:

[`docs/task-notes/forecast-engine-test-guardrails-master-task-card.md`](task-notes/forecast-engine-test-guardrails-master-task-card.md)

This closed task established behavior, lifecycle, identity-coherence, publish,
and dual-target guardrails reused throughout G6. Historical Gate evidence and
case counts remain in its task card.

### G6 Full Technical Identity Migration

Status: `Closed — G6-0 through G6-7 Complete`.

The active identity is `damage-forecast` / `DamageForecast`; current settings
use `DamageForecast.Settings.DamageForecastBaseLibConfig` /
`DamageForecast.cfg`. The compatibility subsystem retains only the bounded
legacy-source descriptors needed for supported direct upgrade and rollback.
Stable migration/rollback/re-upgrade and beta matching-artifact/config smoke
passed. C4 closes the active-scope guards and current authority; Workshop was
not modified.

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

### CodeGraph index rebuild

Status: deferred maintenance, not the current task.

After Forecast Engine architecture stabilization is complete, fully rebuild the ignored local `.codegraph`
index. Verify that the rebuilt index covers the intended current C# files,
contains no deleted paths, and is not treated as authority unless its contents
match the repository. Do not spend a separate implementation Gate refreshing
the index before those code and path changes are finished.

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

There is no active approved task. Documentation Authority Consolidation is
closed: DC5 validation passed, minimal DC3 affected only six superseded
handoffs, and DC4 physical archive was skipped.

The Forecast Engine test-guardrails card is closed through TG7, and G6 Full
Technical Identity Migration is closed through G6-7. The next recommended
behavior candidate is a separate, not-yet-approved diagnosis/fix task for the
current v0.3.0 user report that any damage-dealing Status or Curse card in hand
hides the complete HUD while other scenes behave normally. This supersedes the
earlier Regret/Bad Luck-only scope; the exact matrix and root cause are pending.
The Forecast Engine architecture card remains Proposed/Queued. Workshop changes
and formal multiplayer HUD stay separately gated and frozen unless explicitly
reopened.
