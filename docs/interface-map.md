# Interface Map

Last reconciled: 2026-07-21

## Production Modules

| File / type | Responsibility | Key dependencies | Verification status | Limits |
| --- | --- | --- | --- | --- |
| `MainFile` | Mod bootstrap and Harmony patch setup | Mod loader / Harmony | Implemented | No mechanics or HUD rendering logic. |
| `Combat/LocalIncomingDamageReader` | Reads local combat state and returns `IncomingDamageRead` | `LocalContext`, `ICombatState`, `AttackIntent`, hand pile, powers, relics | Implemented, partially RuntimeVerified by mechanism | Local player only. Unsupported mechanics return Unknown or direct-only result when explicitly supported. |
| `Combat/IncomingDamageRead` | Typed reader result | Forecast layer | Implemented | No game access. |
| `Combat/IncomingDamageDisplayRead` / `IncomingDamageDisplayOptions` | Typed incoming damage `N` result and user-selected calculation switches | HUD forecast snapshot | Implemented, Built | Hidden when selected categories cannot be applied from trusted evidence. |
| `Combat/HookDamageCompat` | Stable/beta compatibility wrapper for read-only native damage preview | `Hook.ModifyDamage`, `ModifyDamageHookType`, `CardPreviewMode` | Implemented, BuiltAgainstStable | Supports stable 10-argument and beta 11-argument signatures; does not execute real damage. |
| `Forecast/LocalDamageForecast` | Combines blockable raw, effective Block, and direct HP loss | `IncomingDamageRead` | Implemented, RuntimeVerified through HUD matrices | Does not read game state. |
| `Forecast/ForecastResult` | Immutable display contract | UI layer | Implemented | States are Hidden, KnownDamage, Unknown. |
| `Forecast/ForecastHudSnapshot` | Display snapshot carrying both existing expected HP loss and optional incoming damage `N` | `ForecastResult`, `IncomingDamageDisplayRead` | Implemented, Built | Display-only; does not alter mechanics. |
| `Patches/ForecastRefreshPatch` | Health-bar HUD label lifecycle, refreshes, final snapshot commit | `NHealthBar`, `Hook`, `CardPile`, `Player`, reader, forecast, UI helpers | Implemented, RuntimeVerified for main HUD paths | Coordinates display only. |
| `Patches/NativeCoveringScreenLifecyclePatch` | Notifies HUD when native covering screens enter/exit | Native screen node types, visibility tracker | Implemented | Only tracks known covering screens. |
| `Patches/PartyWatchBaseLibTitlePatch` | Keeps the BaseLib list identity stable and localizes only this Mod's page title after page load / language changes | BaseLib `NModConfigSubmenu`, Harmony, `PartyWatchConfigText` | Implemented, Built, Installed, RuntimeVerified | Targets only `sts2-party-watch-v2`; relies on the verified BaseLib title resolver, page-load method, and title field. |
| `Settings/PartyWatchBaseLibConfig` | BaseLib-facing persisted config and generated settings UI | BaseLib `SimpleModConfig`, Godot controls, `PartyWatchConfigText` | Implemented, Built, Installed, RuntimeVerified through Phase 12C-B G4 | Uses BaseLib standard rows; language selector is a standard row, not custom header layout. |
| `Settings/PartyWatchSettingsAdapter` | Syncs BaseLib config values into the business-facing settings layer | `PartyWatchBaseLibConfig`, `PartyWatchUiSettings` | Implemented, Built, Installed | Keeps combat/HUD code free of BaseLib references. |
| `Settings/PartyWatchConfigText` | English / Simplified Chinese text table for the Damage Forecast config page | `PartyWatchBaseLibConfig`, `PartyWatchHudAnchor` | Implemented, Built, Installed, RuntimeVerified | Code-only fallback; hover descriptions via `settings_ui.json` are not implemented in Phase 12B. |
| `UI/PartyWatchUiSettings` | Business-facing HUD settings read by HUD/combat display code | Godot `Color` | Implemented | Values are now fed by BaseLib config instead of the removed custom settings panel. |
| `UI/PartyWatchHudDisplay` | HUD text segments, style, color, and positioning | `ForecastHudSnapshot`, `PartyWatchUiSettings` | Implemented, Built | Keeps existing `-N` anchor stable when expected loss is visible. |
| `UI/PartyWatchHudVisibilityPolicy` | Conservative render/hide decision | local combat state, health-bar state, covering-screen tracker | Implemented | Hides instead of guessing through overlays. |
| `UI/PartyWatchHudSnapshotStore` | Display-only turn snapshot and freeze lifecycle | `ForecastResult`, local player/creature identity | Implemented | Does not change forecast mechanics. |
| `UI/PartyWatchNativeCoveringScreenTracker` | Tracks native screens that cover combat | native Godot node type names | Implemented | Explicit known-screen list. |

## Mechanics Readers

| Reader / helper | Reads | Current status | Runtime status | Unsupported boundary |
| --- | --- | --- | --- | --- |
| `AttackIntent.GetTotalDamage(...)` via `LocalIncomingDamageReader` | Native enemy attack preview | Implemented | RuntimeVerified for ordinary attack and regression scenes | Missing/unreadable intents hide or return Unknown. |
| `CardTurnEndDamageInspector` | Whether turn-end hand cards call damage | Implemented | RuntimeVerified through Burn and follow-up hand damage work | Does not call real card effects. |
| `VerifiedPreAttackBlockReader` | Frost, PlatingPower, Orichalcum, FakeOrichalcum, RippleBasin, CloakClasp | Implemented | RuntimeVerified | No generic Block scanner. |
| `VerifiedFixedTurnEndHpLossReader` | Beckon, Bad Luck, Regret | Implemented | RuntimeVerified | No generic `HpLossVar` scanner. |
| `VerifiedTurnEndPowerDamageReader` | ConstrictPower, DisintegrationPower | Implemented | RuntimeVerified | Poison, Doom, Demise, and generic Power damage are not included. |
| `VerifiedHpLossResultModifier` | local IntangiblePower, TungstenRod, BeatingRemnant | Implemented, Conditional | Intangible runtime verified; Tungsten/Beating boundaries documented | Requires known event order and granularity. |
| `VerifiedEnemyDamageModifier` + `LegacyDiamondDiademDamageForecast` | Capability-routed legacy and v0.109 Diamond Diadem | Implemented, Conditional | Legacy path historically RuntimeVerified; v0.109 runtime pending | Legacy per-hit forecast is isolated; current/unknown mechanisms keep native damage and never hide the whole HUD. |
| `EnemyPreActionSurvivalPreview` | enemy identity, HP, Poison, current intent contribution | Implemented, Conditional | Code/build evidence;专项 Poison matrix not fully backfilled | Unreadable specialized paths conservatively keep the current intent. |
| `PoisonTickPreview` | Poison amount, opponent Accelerant, verified special powers, and native preview when available | Implemented, Conditional | Code/build evidence | Uses native `PoisonPower.CalculateTotalDamageNextTurn()` when present and safe fallbacks otherwise. |
| `ObservedHpLossBudgetTracker` | observed local HP loss within the current budget window | Implemented | Used by BeatingRemnant forecast path | Healing does not reduce observed spent budget. |

## Important Settings

| Setting | Default | Effect | Boundary |
| --- | ---: | --- | --- |
| `HudEnabled` | true | Enables or hides the Damage Forecast HUD | Display-only. |
| `ShowLocalHudInMultiplayer` | true | Allows local-player HUD in multiplayer combat | Does not enable teammate or shared party HUD. |
| `ShowBreakdownDetails` | false | Shows optional `🛡` / `♥` detail label | Does not change prediction. |
| `FreezeHudWithinPlayerTurn` | true | Uses display snapshots within the turn and after ending turn | Does not change prediction. |
| `DamageDisplayMode` | `ExpectedHpLossOnly` | Selects expected loss only, incoming damage only, or both | Does not change existing `-N` prediction. |
| `IncomingDamagePlacement` | `RightOfExpectedHpLoss` | Places `N` left or right of `-N` in Both mode | Ignored when `-N` is not visible. |
| Incoming damage calculation switches | false | Optionally apply current Block, Power Block, relic Block, Power HP-loss modifiers, and relic HP-loss modifiers to `N` | Unknown selected paths hide `N`. |
| `HudAnchor` | `HealthBarRight` | Single-player anchor preset | Multiplayer positioning can force below local health bar. |
| `OffsetX` / `OffsetY` | 0 / 0 | Manual position offset | Persisted by BaseLib config. |
| `TotalLossColor` | white | Main `-N` color; shown as `HUD Color` in Phase 12B | Persisted by BaseLib config. |
| `BlockableDetailColor` | light blue | `🛡` detail color | Persisted by BaseLib config. |
| `DirectHpLossDetailColor` | pink/red | `♥` detail color | Persisted by BaseLib config. |

## Native Hooks

| Hook | Purpose | Notes |
| --- | --- | --- |
| `NHealthBar.SetCreature` | Bind and refresh local health-bar labels | Uses reflection only to read `_creature`. |
| `NHealthBar.RefreshValues` | Refresh HUD after health/block changes | Display lifecycle only. |
| `NHealthBar.SetHpBarContainerSizeWithOffsets` | Reposition labels after native size changes | No mechanics. |
| `CardPile.InvokeContentsChanged` | Refresh when hand contents change | Only hand pile triggers refresh. |
| `Player.AddRelicInternal` / `RemoveRelicInternal` / `MeltRelicInternal` | Refresh after relic state changes | No relic mutation by Damage Forecast. |
| `BeatingRemnant.BeforeSideTurnStart` | Reset observed budget window for the owner | Reads owner and turn participants. |
| `Hook.BeforeSideTurnStart` | Clear/start player-turn display snapshot | Local participant only. |
| `Hook.BeforeTurnEnd` / `Hook.BeforeSideTurnEnd` | Commit final forecast snapshot | Optional compatibility patches: stable v0.107.1 uses `BeforeTurnEnd`, beta v0.108.0 uses `BeforeSideTurnEnd`. Local player side only. |
| `Hook.AfterCombatEnd` | Clear snapshots and refresh/hide HUD | Display cleanup. |
| BaseLib `ModConfigRegistry.Register(...)` | Registers the Damage Forecast config page | Main-menu Mod Configuration route is the supported route. |

## Removed / Not Present

- `ForecastHudController`, `ForecastHudView`, and `HealthBarLocator` are no longer production HUD classes.
- `ShowHudInMultiplayer` is not a current setting name. Current field is `ShowLocalHudInMultiplayer`.
- There is no formal `FormalMultiplayerHud` implementation.
- The old `Patches/PartyWatchSettingsPatch.cs` custom settings route has been removed.
- Damage Forecast does not write its own settings JSON; persistence is handled by BaseLib config.
- The in-combat built-in BaseLib config route is visible but currently unusable and remains a known limitation.
