# Mechanics Evidence

## Carried observations from the v1 research prototype - must be revalidated in v2

- AttackIntent.GetTotalDamage(IEnumerable<Creature> targets, Creature attacker) was observed in v1.
- Single attack example: 8 => total 8.
- Multi-hit example: 4x3 => total 12.
- Single-player examples observed:
- RAW 13, Block 0 => OUT 13.
- RAW 13, Block 5 => OUT 8.
- RAW 13, Block 10 => OUT 3.
- A tested Intangible scene produced native intent values 1x2 plus 1, and RAW 3.

## Phase 1–4 code-confirmed mechanics

- The v2 code calls native `AttackIntent.GetTotalDamage(new[] { localCreature }, enemy)` for every readable enemy `AttackIntent`.
- The v2 code sums readable enemy attack RAW values before subtracting Block.
- The v2 code reads current local `Creature.Block` exactly once per refresh.
- The v2 code calculates `OUT = max(0, RAW - Block)`.
- The v2 HUD only displays `🛡 -N` when OUT is greater than 0.
- Non-attack intents, missing combat state, missing local player, missing local creature, dead local creature, no attack intent, and OUT <= 0 hide the HUD.

## Phase 1–4 runtime validation

- Not completed.
- Direct `SlayTheSpire2.exe` launch reached Steam initialization error: `No appID found`.
- Because the direct launch did not reach Mod loading or combat, no Phase 1–4 combat mechanics are considered runtime-proven yet.

## v2 validation requirement

No carried observation or code-confirmed mechanic is considered a v2 runtime proof until revalidated by a scoped v2 runtime task.
