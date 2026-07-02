# Architecture

v2 keeps a narrow read-only chain:

```text
game state readers
=> LocalIncomingDamageReader
=> LocalDamageForecast
=> ForecastResult
=> health-bar HUD labels
```

No module in this chain executes real damage, card play, command queue, RNG, save, or network behavior.

## Bootstrap

`MainFile` composes the mod and applies Harmony patches. It does not calculate combat values and does not render HUD content.

## Game State Read Path

`Combat/LocalIncomingDamageReader` is the only production module that reads combat mechanics for the forecast.

It reads:

- live combat state and the local player via `LocalContext.GetMe(...)`;
- local player creature, current Block, current powers, relics, hand pile, and enemy list;
- enemy `AttackIntent` values through native preview APIs;
- verified hand turn-end damage, fixed direct HP loss, verified turn-end Power damage, verified pre-attack Block, and supported HP-loss result modifiers;
- ordinary enemy Poison pre-action survival state before deciding whether a specific enemy instance contributes its current Attack Intent.

It returns `IncomingDamageRead.Known`, `Hidden`, or `Unknown`. Unsupported mechanics return Unknown or a trusted direct-only result when that boundary is explicitly supported.

## Forecast Calculation

`Forecast/LocalDamageForecast` is the only module that combines raw blockable damage with effective Block.

```text
OutDamage = max(0, RawDamage - EffectiveBlock)
Total display loss = OutDamage + DirectHpLoss
```

If blockable damage is unknown but direct HP loss is trusted, the forecast can return a direct-only known result. If both are untrusted or zero, the HUD hides.

## HP-Loss Event Modifiers

`VerifiedHpLossResultModifier` applies only to verified HP-loss events with known order and granularity.

Current order:

```text
IntangiblePower -> TungstenRod -> BeatingRemnant
```

`TungstenRod` requires single-event granularity. `BeatingRemnant` uses `ObservedHpLossBudgetTracker` plus forecast event order to avoid trusting private native state blindly.

## Enemy Damage Modifiers

`VerifiedEnemyDamageModifier` handles the narrow Diamond Diadem path:

- if `DiamondDiademPower` already exists, trust native `AttackIntent` preview and avoid double-applying it;
- if the relic should grant future Power at turn end and per-hit damage is readable, preview the future multiplicative reduction;
- if only aggregate enemy damage is available and per-hit rounding matters, return unsupported.

## Poison Pre-Action Survival

`EnemyPreActionSurvivalPreview` and `PoisonTickPreview` preview only ordinary enemy Poison cases:

- enemy identity uses `Creature.CombatId` when available, otherwise the native object reference plus snapshot index;
- current Poison and opponent `AccelerantPower` are read-only inputs;
- ordinary Poison damage is previewed before current enemy action;
- unsupported damage caps, HP-loss caps, phase/revive/hatch lifecycle, or special enemy rules return unsupported rather than guessing.

This preview only decides whether a specific enemy instance's current Attack Intent should be included. It does not execute poison, death, removal, or enemy actions.

## HUD Layer

`Patches/ForecastRefreshPatch` owns the health-bar label lifecycle:

- creates one main `Label` and one detail `RichTextLabel`;
- hooks `NHealthBar.SetCreature`, `NHealthBar.RefreshValues`, and health-bar container resize;
- refreshes on hand pile changes, relic changes, native covering screen lifecycle, turn lifecycle, and settings changes;
- commits final snapshots at `Hook.BeforeTurnEnd`;
- clears snapshots at player turn start, combat end, or hidden UI states as appropriate.

`ForecastRefreshPatch` may call the reader and forecast calculator. It must not calculate mechanics itself.

## Display Helpers

- `PartyWatchHudDisplay` builds text, applies font/color styling, and positions the main/detail labels.
- Main display is `-N`.
- Advanced detail display is optional `🛡 N` / `♥ N`, drawn from the same trusted `ForecastResult` fields.
- In multiplayer combat, positioning forces the HUD below the local health bar so it does not imply teammate HUD support.

## Visibility and Freezing

- `PartyWatchHudVisibilityPolicy.ShouldRenderHud(...)` decides whether the HUD should render.
- `PartyWatchNativeCoveringScreenTracker` tracks native screens that cover combat.
- `PartyWatchHudSnapshotStore` controls display-only freezing. It chooses which already-calculated `ForecastResult` is shown; it never changes how the result is calculated.

Refresh timing summary:

- `NHealthBar` lifecycle and hand/relic changes refresh live values.
- `Hook.BeforeSideTurnStart` clears the player-turn display snapshot.
- First trusted player-turn result can become the current snapshot.
- `Hook.BeforeTurnEnd` reads the final current state and commits a snapshot for enemy action time.
- `Hook.AfterCombatEnd` clears snapshots and hides stale values.

## Settings Entry

`PartyWatchUiSettings` is the current-session settings source.

`PartyWatchModdingSettingsPatch` patches `NModInfoContainer.Fill(Mod)`. It adds a `Party Watch HUD` button only when the native Modding screen is displaying the `sts2-party-watch-v2` mod info panel.

This intentionally does not patch or replace the native Settings main menu entry, the native Modding button, or the native `NSettingsScreen.OpenModdingScreen` flow.

## Boundaries

- HUD code must not call damage preview APIs except through the reader/forecast path.
- Combat readers must not create or mutate Godot UI nodes.
- Patches must coordinate lifecycle, not invent mechanics formulas.
- No v2 module depends on Minty.
- No v2 module reads remote player state for formal teammate forecasts.
- No temporary diagnostics or decompiled game source enters production modules.
