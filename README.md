# STS2 Party Watch v2

STS2 Party Watch v2 is a read-only single-player combat damage forecast HUD for Slay the Spire 2.

v2.0 display:

```text
🛡 -OUT
```

Core formula:

```text
RAW = native AttackIntent.GetTotalDamage([local Creature], enemy)
OUT = max(0, RAW - local Creature.Block)
```

Read-only safety guarantees:

- v2 reads local combat state and native attack intent damage.
- v2 does not modify combat state, player state, enemy state, cards, powers, relics, or room state.
- v2 displays only a forecast derived from native damage and current local Block.
- Unknown is safer than an incorrect prediction.

Explicitly out of scope for v2.0:

- Multiplayer.
- Remote player state.
- TargetProbe or target inference.
- Teammate health-bar UI.
- Candidate damage matrices.
- Minty dependencies or UI scraping.
- Direct HP loss or damage.
- Custom Strength, Weak, Vulnerable, Intangible, sequential hit, relic, or power formulas.
- Config menus and generic diagnostics systems.

Current status: bootstrap complete; no gameplay code yet.
