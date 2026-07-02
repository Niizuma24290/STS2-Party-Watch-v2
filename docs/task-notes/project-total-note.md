# Project Total Note

Date: 2026-07-02

This is the current total task note for the project. It backfills the documentation state after several implementation and release-prep commits landed faster than the full docs could be kept synchronized.

## Audit Sources

- `git status`, `git log --oneline --decorate --all -80`, `git log --stat --oneline -40`, `git diff --check`.
- Current source files under `src/STS2PartyWatchCode`.
- Existing docs under `docs/`, `docs/task-notes/`, and `README.md`.
- Existing uncommitted documentation notes for native poison bar follow-up and modding settings entry follow-up.

## Historical Gap Found

The last broad documentation baseline was `3dcdb44 docs: update single-player support and release state`.

After that baseline, the following important work existed but was not fully reflected across `project-state`, `architecture`, `interface-map`, `mechanics-evidence`, roadmap, task notes, and README:

| Topic | Actual change | Modules / files | Verification state | Unverified items | Known limits | Related commits |
| --- | --- | --- | --- | --- | --- | --- |
| Party Watch settings entry no longer occupies native Modding entry | Settings entry moved to Party Watch's own native Mod info panel | `Patches/PartyWatchSettingsPatch.cs`, `sts2-party-watch-v2.json`, task note | Built and publish/install evidence recorded in task note; Steam runtime not fully recorded in repo | Full native settings navigation matrix | No BaseLib config registry; settings remain session-only | `8df6d0a` |
| Poison pre-action survival preview | Ordinary Poison can remove a specific enemy instance's current Attack Intent when the enemy is predicted to die before acting | `Combat/EnemyPreActionSurvivalPreview.cs`, `Combat/LocalIncomingDamageReader.cs`, `Patches/ForecastRefreshPatch.cs`, `UI/PartyWatchHudSnapshotStore.cs` | Code integrated, build/publish evidence recorded | Dedicated Steam matrix for ordinary Poison, multi-enemy, same-name enemy, unsupported specials | HardToKill, Slippery, enemy Intangible, TestSubject/Adaptable, Hatch/ToughEgg, HardenedShell/SewerClam unsupported | `81e06a3`, `0130eff`, `21a960b` |
| Native poison bar follow-up | Native poison preview evidence documented for future small-step Poison improvements | `docs/mechanics-evidence.md`, `docs/task-notes/phase-9b-poison-pre-action-survival.md` | DocumentedOnly; based on local `sts2.dll` / `sts2.xml` inspection and user screenshots | No Party Watch code change in this reconciliation | Do not treat `_poisonForeground` as a data source | uncommitted docs before this reconciliation |
| Local HUD in multiplayer | Existing local HUD can display for the local player during multiplayer combat | `PartyWatchUiSettings`, `PartyWatchHudVisibilityPolicy`, `ForecastRefreshPatch`, `PartyWatchHudDisplay` | Workshop subscription runtime screenshot recorded local `-6` under local character health bar | Formal multiplayer target-aware forecast is not implemented | No teammate HUD, no shared HUD, no network behavior | `90fece2`, `ed50d7b`, `47703ec` |
| Default total HUD and advanced details | Default display is total `-N`; `🛡/♥` details are optional and off by default | `PartyWatchHudDisplay`, `PartyWatchUiSettings`, `ForecastResult` | Existing single-player runtime history plus UI implementation evidence | Some settings/lifecycle visual matrix remains partially manual | Details are display-only and do not change prediction | Phase 9 and Phase 9A commits |
| Tungsten Rod + Beating Remnant order | Party Watch forecast applies Tungsten Rod before Beating Remnant | `VerifiedHpLossResultModifier` | Code evidence and task note history | Subscription/runtime validation for this exact combined order remains noted as pending | Requires verified event granularity | `a52de0b` and later notes |

## Phase Numbering Correction

Several files named `phase-9*` actually record work completed after the Workshop upload / subscription-test milestone. Their filenames are kept as historical evidence, but they should not be used as the naming pattern for new work.

Current numbering policy:

- Phase 9: single-player release cleanup and historical `phase-9*` evidence files.
- Phase 10: Workshop upload / private subscription-test milestone.
- Phase 11: new supplements, maintenance fixes, runtime backfills, and documentation updates after Workshop upload.

`phase-10-multiplayer-research-frozen.md` was policy-only and conflicted with the corrected Phase 10 meaning, so the frozen multiplayer rule now lives in the total index / roadmap / Phase 11 notes rather than as a Phase 10 task note.

## Phase 9B Poison Total Note

The final scattered Poison notes have been consolidated here and in `docs/task-notes/README.md`.

Implemented / code-evidenced state:

- `EnemyPreActionSurvivalPreview` and `PoisonTickPreview` are integrated into `LocalIncomingDamageReader`.
- The preview runs per native enemy instance before that instance's current `AttackIntent` is included in the forecast.
- Enemy identity uses `Creature.CombatId` when available; otherwise it uses native object reference plus same-snapshot enemy index. Display name is not a key.
- Ordinary supported input state is current enemy HP, current `PoisonPower.Amount`, current intent contribution, and opponent `AccelerantPower.Amount`.
- `AccelerantPower` is read only when already active. Party Watch does not replay `Accelerant`, card effects, command queues, or native Poison commands.
- If ordinary Poison is predicted to kill the enemy before it acts, only that enemy instance's current Intent contribution is removed from final `-N`.

Native poison-bar evidence now carried in the total note:

- `PoisonPower.CalculateTotalDamageNextTurn()` is the native model-layer total poison preview used by the native poison health-bar foreground.
- Native trigger count is `min(PoisonPower.Amount, 1 + alive opponents AccelerantPower amount sum)`.
- Native preview calls `Hook.ModifyDamage(...)` per Poison tick, which makes it relevant future evidence for damage-cap cases such as `HardToKillPower` and damage-cap side of enemy `IntangiblePower`.
- `NHealthBar.IsPoisonLethal(...)` and `_poisonForeground` are UI preview signals. They do not prove enemy Intent cancellation, phase/revive behavior, death removal timing, or HP-loss budget behavior.

Unsupported / research-only state:

- Ordinary Poison kill / no-kill, multiple enemies, same-name enemies, and unsupported special enemies still need a dedicated Steam runtime matrix before being marked RuntimeVerified.
- `HardToKillPower` / Exoskeleton has native poison-bar evidence but is not implemented in Party Watch.
- `SlipperyPower` should keep that enemy's current Intent contribution while active; future work should disable only that enemy instance's Poison survival correction until Slippery is gone.
- Enemy `IntangiblePower` needs separate validation for per-tick Poison cap and current Intent cancellation.
- `TestSubject` / `AdaptablePower` phase and revive behavior is not implemented.
- `ToughEgg` / `HatchPower` and `SewerClam` / `HardenedShellPower` remain unsupported / research-only.

Verification classification:

- Implemented: ordinary Poison pre-action survival code path.
- Built / publish evidenced: recorded in the Phase 9B task note.
- RuntimeVerified: not yet fully backfilled for the ordinary Poison matrix.
- DocumentedOnly: native poison-bar evidence and special enemy strategy notes.

## Current Supported Scope

- Local-player HUD in single-player combat.
- Local-player HUD in multiplayer combat when enabled, with no teammate or shared HUD claims.
- Default total expected HP loss `-N`.
- Optional advanced `🛡 N` / `♥ N` details.
- Enemy AttackIntent, hand turn-end blockable damage, verified pre-attack Block, fixed direct HP loss, verified turn-end Power damage, supported HP-loss result modifiers, Diamond Diadem supported path, and ordinary Poison pre-action survival under documented constraints.
- Phase 11 is the forward path for new supplements after Workshop upload.

## Current Known Limits

- Phase 11 local HUD alignment cleanup is closed at the code level: the temporary cyan health-bar guide, magenta HUD text guide, `[HUD Align]` runtime log, and `PartyWatchHudDebugGuide` helper were removed in commit `070774b70ef07a5ead50f3e82ad60f1a6a3c6c0f`.
- The main `-N` alignment was RuntimeVerified by pre-removal runtime logs (`main.deltaY=0`, `guide.deltaY=0`). The debug-guide-removal build is Built and Installed only until a final Steam smoke check confirms no guide lines or debug log noise.
- Formal multiplayer HUD remains frozen.
- Poison special enemies and special HP-loss budgets remain unsupported or research-only.
- Settings are session-only.
- Public Workshop release is not claimed.
- Unsupported or untrusted mechanics hide rather than displaying guessed numbers.

## Documents Updated In This Reconciliation

- `README.md`
- `docs/project-state.md`
- `docs/architecture.md`
- `docs/interface-map.md`
- `docs/mechanics-evidence.md`
- `docs/decisions.md`
- `docs/v2-roadmap.md`
- `docs/task-notes/README.md`
- `docs/task-notes/project-total-note.md`
- `docs/task-notes/phase-11-supplements.md`

## Verification State For This Documentation Task

- No code was changed.
- No build or publish was run, because this task is documentation-only.
- Final verification should use `git diff -- docs README.md`, `git diff --check`, and `git status`.

## Next Single Task

Record new supplements under Phase 11. The first candidate is the ordinary Poison Steam runtime matrix. Keep formal multiplayer HUD frozen.
