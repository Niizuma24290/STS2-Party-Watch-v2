# v2 Roadmap

Last reconciled: 2026-07-02

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
| Modding settings entry fix | Complete | Party Watch no longer occupies native Settings/Modding main entry; settings now live inside Party Watch's own mod info panel. |
| Local HUD in multiplayer | Complete for local-player HUD only | Workshop subscription runtime evidence shows local player's Party Watch HUD in multiplayer combat. |
| Phase 10 - Workshop upload / subscription milestone | Complete as private/subscription milestone | Workshop item `3755598583`, private visibility, cover/preview uploads, uploaded DLL hash, and subscription local-HUD validation are recorded in `workshop-private-rc-2026-07-01.md`. |

## Current Phase

### Phase 11 - Supplements and maintenance

Status: active for new follow-up work after Workshop upload. Do not create new Phase 9 subphases.

Historical note: Phase 9B Poison and several `phase-9*` follow-ups were named before this numbering correction. Keep those files as evidence, but record new work as Phase 11.

First supplement candidate:

- ordinary Poison pre-action survival Steam runtime matrix.

## Legacy Follow-Up Needing Validation

### Phase 9B - Poison pre-action survival preview

Status: implemented in code and build/publish evidenced, but专项 Steam runtime matrix remains the next validation task.

Current supported scope:

- ordinary enemy instance with readable `PoisonPower`;
- readable opponent `AccelerantPower`;
- no unsupported special damage cap, HP-loss cap, revive, hatch, phase, or special lifecycle Power;
- only decides whether that enemy instance's current Attack Intent should contribute to the local forecast.

Current unsupported/research scope:

- `HardToKillPower` / Exoskeleton;
- `SlipperyPower`;
- enemy `IntangiblePower`;
- `TestSubject` / `AdaptablePower`;
- `ToughEgg` / `HatchPower`;
- `SewerClam` / `HardenedShellPower`;
- generic enemy HP-loss budgets and lifecycle simulation.

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

Create Phase 11 supplements for new work. First candidate: validate and record the ordinary Poison matrix in Steam, including ordinary Poison kill/no-kill, multiple enemies, same-name enemies, and unsupported special enemy boundaries. Keep formal multiplayer HUD frozen.
