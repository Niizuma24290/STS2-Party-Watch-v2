# Architecture

v2 keeps a narrow read-only chain:

```text
game state readers
=> LocalIncomingDamageReader
=> LocalDamageForecast / IncomingDamageDisplayRead
=> ForecastResult / ForecastHudSnapshot
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

The expected-HP-loss path returns `IncomingDamageRead.Known`, `Hidden`, or
`Unknown`. Unsupported mechanics return Unknown or a trusted direct-only result
when that boundary is explicitly supported.

Phase 13A adds `ReadIncomingDamageForLocalCreature(...)`, which builds the
optional positive incoming-damage value `N` from the same trusted source
readers. It applies only the user-selected current Block, Power/Orb Block,
relic Block, Power modifier, and relic modifier categories. It does not derive
`N` by reversing `-N`, and an unsupported selected path makes `N` Unknown
rather than exposing a partial total.

## Forecast Calculation

`Forecast/LocalDamageForecast` is the only module that combines raw blockable damage with effective Block.

```text
OutDamage = max(0, RawDamage - EffectiveBlock)
Total display loss = OutDamage + DirectHpLoss
```

If blockable damage is unknown but direct HP loss is trusted, the forecast can return a direct-only known result. If both are untrusted or zero, the HUD hides.

`ForecastHudSnapshot` carries the existing expected HP-loss result and the
separate incoming-damage display result through one display/freezing snapshot.
The default remains `ExpectedHpLossOnly`; `IncomingDamageOnly` and `Both` are
optional Phase 13A projections and do not change `ForecastResult` semantics.

## HP-Loss Event Modifiers

`VerifiedHpLossResultModifier` applies only to verified HP-loss events with known order and granularity.

Current order:

```text
IntangiblePower -> TungstenRod -> BeatingRemnant
```

`TungstenRod` requires single-event granularity. `BeatingRemnant` uses `ObservedHpLossBudgetTracker` plus forecast event order to avoid trusting private native state blindly.

## Enemy Damage Modifiers

`VerifiedEnemyDamageModifier` capability-routes Diamond Diadem without binding the shared DLL to removed legacy APIs:

- legacy `CardsPlayedThisTurn` plus `DiamondDiademPower` selects `LegacyDiamondDiademDamageForecast`;
- the v0.109 first-turn Block + Blur mechanism trusts native attack and Block state;
- unknown mechanisms and local read failures keep native attack damage instead of invalidating the complete HUD read;
- the pending Stampede adjustment exists only inside the legacy crown strategy. Ripple Basin keeps its separate Stampede logic in `VerifiedPreAttackBlockReader`.

## Poison Pre-Action Survival

`EnemyPreActionSurvivalPreview` and `PoisonTickPreview` conservatively preview enemy Poison before the current intent:

- enemy identity uses `Creature.CombatId` when available, otherwise the native object reference plus snapshot index;
- current Poison and opponent `AccelerantPower` are read-only inputs;
- ordinary Poison damage is previewed before the current enemy action;
- verified Intangible, Slippery, Hardened Shell, Hard to Kill, and native Poison-preview paths are used when available;
- when a specialized path cannot establish a lethal pre-action result, the preview keeps the current intent instead of exposing an inert supported/unsupported result channel.

This preview only decides whether a specific enemy instance's current Attack Intent should be included. It does not execute poison, death, removal, or enemy actions.

## HUD Layer

`Patches/ForecastRefreshPatch` owns the health-bar label lifecycle:

- creates one expected-loss `Label`, one incoming-damage `Label`, and one
  advanced-detail `RichTextLabel`;
- hooks `NHealthBar.SetCreature`, `NHealthBar.RefreshValues`, and health-bar container resize;
- refreshes on hand pile changes, relic changes, native covering screen lifecycle, turn lifecycle, and settings changes;
- commits final snapshots through compatible stable/beta turn-end hooks:
  `Hook.BeforeTurnEnd` on stable v0.107.1 or `Hook.BeforeSideTurnEnd` on the
  current frozen beta v0.109.0 capability surface;
- clears snapshots at player turn start, combat end, or hidden UI states as appropriate.

`ForecastRefreshPatch` may call the reader and forecast calculator. It must not calculate mechanics itself.

## Display Helpers

- `DamageForecastHudDisplay` builds text, applies font/color styling, and positions
  the expected-loss, incoming-damage, and detail labels.
- Expected HP loss is `-N`; optional incoming damage is positive `N`.
- Display modes are `ExpectedHpLossOnly`, `IncomingDamageOnly`, and `Both`.
- When both values are visible, `-N` remains the anchor and incoming `N` may be
  placed to its left or right. Incoming-only mode uses the same anchor point.
- Advanced detail display is optional `🛡 N` / `♥ N`, drawn from the same trusted `ForecastResult` fields.
- In multiplayer combat, positioning forces the HUD below the local health bar so it does not imply teammate HUD support.

## Visibility and Freezing

- `DamageForecastHudVisibilityPolicy.ShouldRenderHud(...)` decides whether the HUD should render.
- `DamageForecastNativeCoveringScreenTracker` tracks native screens that cover combat.
- `DamageForecastHudSnapshotStore` controls display-only freezing. It chooses which already-calculated `ForecastResult` is shown; it never changes how the result is calculated.

Refresh timing summary:

- `NHealthBar` lifecycle and hand/relic changes refresh live values.
- `Hook.BeforeSideTurnStart` clears the player-turn display snapshot.
- First trusted player-turn result can become the current snapshot.
- `Hook.BeforeTurnEnd` on stable or `Hook.BeforeSideTurnEnd` on beta reads the final current state and commits a snapshot for enemy action time.
- `Hook.AfterCombatEnd` clears snapshots and hides stale values.

## Settings Entry

Damage Forecast settings are registered through BaseLib:

```text
DamageForecastBaseLibConfig : SimpleModConfig
=> ModConfigRegistry.Register("damage-forecast", config)
=> DamageForecastSettingsAdapter
=> DamageForecastUiSettings
```

`DamageForecastBaseLibConfig` owns BaseLib-facing persisted values and the generated configuration page. BaseLib reads and writes the current `DamageForecast.cfg` file. The isolated compatibility subsystem runs before construction of this type and handles supported legacy-source migration without exposing a legacy DTO to ordinary settings code.

`DamageForecastSettingsAdapter` copies persisted BaseLib values into `DamageForecastUiSettings`, which remains the business-facing settings API read by HUD, visibility, positioning, and display code.

Phase 13A settings add the damage display mode, incoming-value placement, and
five independently persisted inclusion switches for current Block, Power/Orb
Block, relic Block, Power HP-loss modifiers, and relic HP-loss modifiers. Their
defaults preserve the pre-Phase-13A behavior: expected HP loss only, incoming
placement on the right, and all incoming-defense switches off.

Phase 12B keeps BaseLib's automatic rows and controls. It overrides `SetupConfigUI(...)` only to order rows and apply Damage Forecast-local English / Simplified Chinese text after BaseLib creates standard rows. It does not replace the BaseLib fullscreen shell, left mod list, scrolling, spacing, or controls.

The code-only Mod has no BaseLib `settings_ui` localization PCK. `DamageForecastBaseLibTitlePatch` therefore keeps BaseLib's shared list/button lookup identity stable as English `Damage Forecast`, while the page title is updated separately after BaseLib completes `LoadModConfig(...)` and whenever `ConfigLanguage` changes. This preserves left-list selection, highlighting, and controller focus while allowing the page title and setting text to follow English / Simplified Chinese immediately and across restart.

The former custom settings route was removed. The supported settings route is the main-menu BaseLib Mod Configuration page. The in-combat built-in BaseLib configuration route is visible but currently unusable and remains a known limitation.

## Boundaries

- HUD code must not call damage preview APIs except through the reader/forecast path.
- Combat readers must not create or mutate Godot UI nodes.
- Patches must coordinate lifecycle, not invent mechanics formulas.
- No v2 module depends on Minty.
- No v2 module reads remote player state for formal teammate forecasts.
- No temporary diagnostics or decompiled game source enters production modules.
- Combat, forecast, and HUD calculation modules must not reference BaseLib UI types directly.
