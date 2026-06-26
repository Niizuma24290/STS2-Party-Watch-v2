# Architecture

v2.0 keeps each module narrow and read-only.

## MainFile

- Mod bootstrap and dependency composition only.
- No combat calculation.
- No UI rendering logic.

## Combat/LocalIncomingDamageReader

- Reads local combat state and enemy AttackIntent objects.
- Calls the game-native GetTotalDamage API.
- Produces RAW or an explicit unknown state.
- Does not touch UI nodes.

## Forecast/LocalDamageForecast

- Receives RAW and local Creature.Block.
- Calculates OUT = max(0, RAW - Block).
- Produces a small immutable result.
- Does not inspect intents or UI.

## Forecast/ForecastResult

- Small immutable result model.
- States: Hidden, KnownDamage, Unknown.
- No Godot, UI, or combat access.

## UI/ForecastHudView

- Creates, updates, hides, and destroys only the 🛡 -OUT text.
- Does not calculate RAW or OUT.
- Does not inspect combat state directly.

## Patches/ForecastRefreshPatch

- Uses a proven combat/UI refresh point to request an update.
- Does not calculate damage.
- Does not own HUD state.

No future v2 module may bypass this chain:

```text
Combat reader
=> forecast calculator
=> result
=> HUD view
```

Architecture rules:

- No HUD code may call GetTotalDamage.
- No combat reader may access Godot UI nodes.
- No patch may calculate RAW or OUT.
- No module may depend on Minty.
- No v2.0 module may read remote player state.
- No temporary diagnostics code enters production modules without a separate scoped task.
