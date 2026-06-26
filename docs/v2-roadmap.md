# v2 Roadmap

## Phase 0 - Bootstrap

Create clean repository, scope, architecture rules, and roadmap.

## Phase 1 - Empty Mod load proof

Create the smallest possible independent Mod project.
Build, publish, load in game, and prove one initialization log line.
No combat logic.

## Phase 2 - Local combat access proof

Read only local Player, local Creature, HP, Block, enemy list, and intent presence.
No damage output and no HUD.

## Phase 3 - Native RAW reader

Implement LocalIncomingDamageReader.
Validate normal attack, multi-hit attack, and non-attack intent.
No final forecast HUD.

## Phase 4 - Local forecast calculation

Implement LocalDamageForecast and ForecastResult.
Use RAW and Creature.Block exactly once.
Validate Block 0, partial Block, and full Block.

## Phase 5 - HUD integration

Create ForecastHudView at a proven local HUD anchor.
Show only 🛡 -OUT.
Hide for OUT=0, unknown, and no attack.

## Phase 6 - Controlled runtime validation

Run a small written test matrix:
normal attack,
multi-hit,
0 Block,
partial Block,
full Block,
non-attack,
one tested special-case scene.
Record actual result versus forecast.

## Phase 7 - v2.0 cleanup

Remove temporary diagnostics.
Confirm only production responsibilities remain.
Document known limitations.
Tag v2.0.0 only after manual runtime validation.

## Deferred

- v2.1 direct HP-loss pipeline.
- v2.2 multiplayer state reading.
- v2.3 actual-target resolution.
- v2.4 teammate HUD.
