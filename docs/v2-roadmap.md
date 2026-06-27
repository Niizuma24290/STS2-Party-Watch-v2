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

## Phase 7 - HP Loss result modifiers

接入 TungstenRod、BeatingRemnant 等会修正 HP Loss 结果的机制。

## Phase 8 - Non-Block damage modifiers

补 DiamondDiademPower 等“改变实际承伤、但不属于 Block”的伤害修正机制。

## Phase 9 - Single-player release cleanup

Remove temporary diagnostics.
Confirm only production responsibilities remain.
Document known limitations.
Tag v2.0.0 only after manual runtime validation.

## Phase 10 - Multiplayer research

仅研究多人真实目标与原生 target-aware 伤害预览，证据不足前不做正式多人 HUD。
