# Architecture

v2.0 keeps each module narrow and read-only.

## MainFile

- Mod bootstrap and dependency composition only.
- No combat calculation.
- No UI rendering logic.

## Combat/LocalIncomingDamageReader

- Reads local combat state and enemy AttackIntent objects.
- Calls game-native preview APIs only for read-only damage forecasting.
- Produces blockable damage, effective block, direct HP loss, or an explicit unknown state.
- Does not touch UI nodes.

## Forecast/LocalDamageForecast

- Receives blockable damage, effective block, and direct HP loss.
- Calculates `🛡 = max(0, blockable damage - effective block)` and passes through verified direct HP loss.
- Produces a small immutable result.
- Does not inspect intents or UI.

## Forecast/ForecastResult

- Small immutable result model.
- States: Hidden, KnownDamage, Unknown.
- No Godot, UI, or combat access.

## Patches/ForecastRefreshPatch

- Hooks the local `NHealthBar` lifecycle to create, position, update, and hide one small label near the player health bar.
- Uses `LocalIncomingDamageReader` and `LocalDamageForecast` for all mechanics; the patch only coordinates refresh and display.
- Displays total expected HP loss as the default HUD line.
- Keeps trusted `🛡` / `♥` source details behind a UI-layer advanced toggle.
- Hides the label for hidden/unknown/zero-output states.
- Does not create or execute combat commands.

No future v2 module may bypass this chain:

```text
Combat reader
=> forecast calculator
=> result
=> health-bar HUD label
```

Architecture rules:

- No HUD code may call damage preview APIs.
- No combat reader may access Godot UI nodes.
- No patch may calculate mechanics values.
- No module may depend on Minty.
- No v2.0 module may read remote player state.
- No temporary diagnostics code enters production modules without a separate scoped task.

Single-player production notes:

- `Combat/LocalIncomingDamageReader` is the only module that reads combat mechanics.
- `Forecast/LocalDamageForecast` is the only module that combines blockable damage with effective block.
- `Patches/ForecastRefreshPatch` is the only module that owns the final HUD label and its display text.
- `Patches/ForecastRefreshPatch` may add trusted final `OutDamage + DirectHpLoss` for display only; it must not reread game state or recalculate mechanics.
- Breakdown details, when enabled, remain display-only and use the same trusted final values.
- UI reflection is restricted to reading `NHealthBar._creature` and `HpBarContainer` placement data; it does not read combat mechanics.
