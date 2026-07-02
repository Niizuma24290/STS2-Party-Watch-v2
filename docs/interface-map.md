# Interface Map

Last reconciled: 2026-07-02

## Production Modules

| File / type | Responsibility | Key dependencies | Verification status | Limits |
| --- | --- | --- | --- | --- |
| `MainFile` | Mod bootstrap and Harmony patch setup | Mod loader / Harmony | Implemented | No mechanics or HUD rendering logic. |
| `Combat/LocalIncomingDamageReader` | Reads local combat state and returns `IncomingDamageRead` | `LocalContext`, `ICombatState`, `AttackIntent`, hand pile, powers, relics | Implemented, partially RuntimeVerified by mechanism | Local player only. Unsupported mechanics return Unknown or direct-only result when explicitly supported. |
| `Combat/IncomingDamageRead` | Typed reader result | Forecast layer | Implemented | No game access. |
| `Forecast/LocalDamageForecast` | Combines blockable raw, effective Block, and direct HP loss | `IncomingDamageRead` | Implemented, RuntimeVerified through HUD matrices | Does not read game state. |
| `Forecast/ForecastResult` | Immutable display contract | UI layer | Implemented | States are Hidden, KnownDamage, Unknown. |
| `Patches/ForecastRefreshPatch` | Health-bar HUD label lifecycle, refreshes, final snapshot commit | `NHealthBar`, `Hook`, `CardPile`, `Player`, reader, forecast, UI helpers | Implemented, RuntimeVerified for main HUD paths | Coordinates display only. |
| `Patches/NativeCoveringScreenLifecyclePatch` | Notifies HUD when native covering screens enter/exit | Native screen node types, visibility tracker | Implemented | Only tracks known covering screens. |
| `Patches/PartyWatchSettingsPatch` / `PartyWatchModdingSettingsPatch` | Adds Party Watch settings button and panel inside Party Watch's native Modding info panel | `NModInfoContainer.Fill(Mod)`, `PartyWatchUiSettings` | Implemented, build/publish evidence | Does not occupy native Settings or Modding main entry. |
| `UI/PartyWatchUiSettings` | Session-only HUD settings | Godot `Color` | Implemented | No persistent settings API used. |
| `UI/PartyWatchHudDisplay` | HUD text, style, color, and positioning | `ForecastResult`, `PartyWatchUiSettings` | Implemented | Display-only. |
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
| `VerifiedEnemyDamageModifier` | Diamond Diadem and `DiamondDiademPower` | Implemented, Conditional | RuntimeVerified for supported single-player path | Aggregate enemy damage with per-hit rounding remains Unknown. |
| `EnemyPreActionSurvivalPreview` | enemy identity, HP, Poison, current intent contribution | Implemented, Conditional | Code/build evidence;专项 Poison matrix not fully backfilled | Special enemy lifecycle, caps, and HP-loss budgets return unsupported. |
| `PoisonTickPreview` | ordinary Poison amount and opponent Accelerant | Implemented, Conditional | Code/build evidence | Does not call native `PoisonPower.CalculateTotalDamageNextTurn()` yet. |
| `ObservedHpLossBudgetTracker` | observed local HP loss within the current budget window | Implemented | Used by BeatingRemnant forecast path | Healing does not reduce observed spent budget. |

## Important Settings

| Setting | Default | Effect | Boundary |
| --- | ---: | --- | --- |
| `HudEnabled` | true | Enables or hides Party Watch HUD | Display-only. |
| `ShowLocalHudInMultiplayer` | true | Allows local-player HUD in multiplayer combat | Does not enable teammate or shared party HUD. |
| `ShowBreakdownDetails` | false | Shows optional `🛡` / `♥` detail label | Does not change prediction. |
| `FreezeHudWithinPlayerTurn` | true | Uses display snapshots within the turn and after ending turn | Does not change prediction. |
| `HudAnchor` | `HealthBarRight` | Single-player anchor preset | Multiplayer positioning can force below local health bar. |
| `OffsetX` / `OffsetY` | 0 / 0 | Manual position offset | Session-only. |
| `TotalLossColor` | white | Main `-N` color | Session-only. |
| `BlockableDetailColor` | light blue | `🛡` detail color | Session-only. |
| `DirectHpLossDetailColor` | pink/red | `♥` detail color | Session-only. |

## Native Hooks

| Hook | Purpose | Notes |
| --- | --- | --- |
| `NHealthBar.SetCreature` | Bind and refresh local health-bar labels | Uses reflection only to read `_creature`. |
| `NHealthBar.RefreshValues` | Refresh HUD after health/block changes | Display lifecycle only. |
| `NHealthBar.SetHpBarContainerSizeWithOffsets` | Reposition labels after native size changes | No mechanics. |
| `CardPile.InvokeContentsChanged` | Refresh when hand contents change | Only hand pile triggers refresh. |
| `Player.AddRelicInternal` / `RemoveRelicInternal` / `MeltRelicInternal` | Refresh after relic state changes | No relic mutation by Party Watch. |
| `BeatingRemnant.BeforeSideTurnStart` | Reset observed budget window for the owner | Reads owner and turn participants. |
| `Hook.BeforeSideTurnStart` | Clear/start player-turn display snapshot | Local participant only. |
| `Hook.BeforeTurnEnd` | Commit final forecast snapshot | Local player side only. |
| `Hook.AfterCombatEnd` | Clear snapshots and refresh/hide HUD | Display cleanup. |
| `NModInfoContainer.Fill(Mod)` | Add settings entry for Party Watch's own mod info panel | Does not modify native Modding entry flow. |

## Removed / Not Present

- `ForecastHudController`, `ForecastHudView`, and `HealthBarLocator` are no longer production HUD classes.
- `ShowHudInMultiplayer` is not a current setting name. Current field is `ShowLocalHudInMultiplayer`.
- There is no formal `FormalMultiplayerHud` implementation.
- There is no persistent settings file writer.
