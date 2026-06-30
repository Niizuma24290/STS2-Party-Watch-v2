# STS2 Party Watch v2

STS2 Party Watch v2 is a read-only single-player combat forecast HUD for Slay the Spire 2.

It answers one narrow question:

```text
If I end the turn from the current single-player combat state, how much verified damage or HP loss should I expect?
```

## HUD

The production HUD emphasizes total expected HP loss:

```text
-18
```

- `-N`: the sum of trusted final blockable prediction and trusted direct HP loss.
- Advanced details can optionally show `🛡 N` and `♥ N` as source breakdowns.
- `🛡 N`: verified blockable incoming damage after verified block and HP-loss result modifiers.
- `♥ N`: verified direct HP loss that does not go through Block.

When a value cannot be predicted precisely, Party Watch hides that value instead of showing a guess.

## Supported Single-Player Scope

- Enemy AttackIntent / DeathBlow intent damage.
- Burn-style hand turn-end blockable `DamageVar`.
- Verified end-turn block sources: Frost, PlatingPower, Orichalcum, FakeOrichalcum, RippleBasin, CloakClasp.
- Direct HP loss from Beckon, Bad Luck, and Regret.
- HP loss result modifiers from Tungsten Rod and Beating Remnant within the verified normal-game scope.
- Diamond Diadem enemy attack reduction within the verified single-hit / multi-hit forecast path.

Read-only safety guarantees:

- v2 reads local combat state and native attack intent damage.
- v2 does not modify combat state, player state, enemy state, cards, powers, relics, or room state.
- v2 does not call real damage, command, RNG, save, or network entry points.
- Unknown is safer than an incorrect prediction.

Explicitly out of scope:

- Multiplayer.
- Remote player state.
- Formal multiplayer damage HUD.
- Generic turn simulation, generic damage engines, or generic HP loss engines.
- Unsupported or unverified relic, power, card, or enemy mechanisms.
- Guessing partial blockable damage when required event order or hit granularity is unknown.
- Config menus and generic diagnostics systems.

Current status: Phase 9 code and documentation cleanup is complete; final single-player runtime regression is pending.
