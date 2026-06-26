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

- Hooks `NCombatUi.Activate` and `NCombatUi.Deactivate` to attach, update, and clear the HUD controller.
- Does not calculate damage.
- Does not own HUD state.

## UI/ForecastHudController

- Owns refresh cadence and coordinates the reader, calculator, and HUD view.
- Reads no combat data directly except the local creature node required for HUD placement.
- Places the HUD near the right side of the local player's health bar, with fallback spacing if the health bar node cannot be located.
- Does not create or execute combat commands.

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

Phase 1–4 implementation notes:

- `Combat/LocalIncomingDamageReader` is the only module that calls native `AttackIntent.GetTotalDamage`.
- `Forecast/LocalDamageForecast` is the only module that calculates OUT.
- `UI/ForecastHudView` only shows or hides the final `🛡 -N` text.
- `UI/HealthBarLocator` uses narrow UI reflection only to place the HUD near the local health bar; it does not read combat mechanics.
