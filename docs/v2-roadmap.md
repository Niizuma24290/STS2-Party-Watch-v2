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
| Modding settings entry fix | Complete | Party Watch no longer occupies native Settings/Modding main entry; settings now live inside Party Watch's own mod info panel. |
| Local HUD in multiplayer | Complete for local-player HUD only | Workshop subscription runtime evidence shows local player's Party Watch HUD in multiplayer combat. |
| Phase 10 - Workshop upload / subscription milestone | Complete as private/subscription milestone | Workshop item `3755598583`, private visibility, cover/preview uploads, uploaded DLL hash, and subscription local-HUD validation are recorded in `workshop-private-rc-2026-07-01.md`. |
| Phase 12A - BaseLib automatic config baseline | Complete | Party Watch settings migrated to BaseLib automatic `SimpleModConfig`; main-menu Mod Configuration route user-verified usable; in-combat BaseLib route remains a known limitation. |

## Current Phase

### Phase 12C-B G3/G4 - Surface Rename and Runtime Closure

Status: completed / StaticVerified / BuildVerified / ContractVerified / RuntimeVerified. G2 audit closure and AUD-0007 performance measurement were complete before G3; the separately approved G4 runtime gate is now closed.

Task note:

```text
docs/task-notes/phase-12c-surface-rename.md
docs/task-notes/phase-12c-g4-runtime-verification.md
```

Authorized scope:

- rename the player-visible product to `Damage Forecast` / `伤害预测`;
- update manifest display metadata, BaseLib labels, and current product documentation;
- explicitly label multiplayer visibility as local-player only;
- retain Mod ID, BaseLib persistence key, Harmony ID, namespaces, `PartyWatch*` symbols, project/assembly/file identities, paths, Workshop ID, diagnostic prefix, and historical evidence.

Hard boundary:

- no behavior, forecast, persistence, layout, or performance logic changes;
- no technical-identity or filename migration;
- no install or game runtime verification inside G3.

Runtime note:

- G3 ended at static verification and stable/beta build evidence;
- G4 installed the artifact and verified single-load identity, the Mod list, BaseLib English/Chinese titles and labels, restart persistence, and the core single-player HUD;
- the final installed DLL is 112,128 bytes with SHA256 `1BAC85248FD83B62BE9867FD3A2F8F3B54D676D96D3F6B25F71B720A62D544F0`;
- the in-combat BaseLib config route remains a known limitation.

Historical note: Phase 9B Poison and several `phase-9*` follow-ups were named before the Phase 11 numbering correction. Keep those files as evidence. Poison matrix work remains deferred unless explicitly reopened.

Deferred candidate:

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

G4 is closed. Request separate G5 approval before any commit, push, Workshop update, or remote-documentation update. Keep formal multiplayer HUD frozen unless explicitly reopened.
