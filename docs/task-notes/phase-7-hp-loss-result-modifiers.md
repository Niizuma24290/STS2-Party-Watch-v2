# Phase 7: HP Loss Result Modifiers

## Status

Closed for normal-game reachable scope.

## Scope

This phase handles only the verified HP-loss result modifiers:

- `TungstenRod`
- `BeatingRemnant`

It does not add a generic damage engine, a full turn simulator, multiplayer HUD support, Diamond Diadem handling, or any new unverified relic/power behavior.

## Actual Changes

- Added `VerifiedHpLossRelicModifier`, a narrow modifier for already verified upcoming HP loss events.
- Added `UpcomingHpLossEvent` with lane, native order, verified HP loss, and single-event granularity metadata.
- Kept the existing Phase 6 formula path unchanged when the player has neither `TungstenRod` nor `BeatingRemnant`.
- When either relic is present, `LocalIncomingDamageReader` now builds an ordered future HP loss event stream:
  - current hand order turn-end events first;
  - enemy attack HP loss last, split by `AttackIntent.GetSingleDamage(...)` and `Repeats` when the split matches `GetTotalDamage(...)`.
- `Beckon`, `BadLuck`, and `Regret` now expose per-card direct HP loss events while preserving the old total reader.
- Verified blockable turn-end hand `DamageVar` cards are treated as blockable hand events after the existing verified damage and block logic determines their actual HP loss.
- `BeatingRemnant` now uses a HUD-side observed HP loss budget instead of directly trusting native private `_damageReceivedThisTurn`.
- Added `ObservedHpLossBudgetTracker`: player side-turn start resets the budget, HP drops increase spent budget, and healing does not reduce spent budget.
- `TungstenRod` is applied only to single verified events. Unsplittable aggregate enemy attack with positive HP loss remains unsupported when Tungsten Rod is present.
- HUD refresh now also runs after native relic add/remove/melt methods so console-added relics can update the forecast.

## Resolved Budget Source Issue

Steam screenshots showed cases where `BeatingRemnant` forecasts were capped at 19 or 18 even when the player had not lost HP in the current visible test window. The earlier implementation directly trusted native private `_damageReceivedThisTurn`; this field can retain state that is not reliable enough for HUD forecasting after console setup, prior-turn blood/heal testing, or similar validation flows.

The implementation now uses a narrow HUD-side observed HP loss budget tracker:

- Reset on the player's `BeforeSideTurnStart` budget boundary.
- Track actual player HP drops during the current budget window.
- Accumulate drops only; healing must not reduce the accumulated spent budget.
- Use this observed spent amount for `BeatingRemnant` HUD forecasting.
- Keep `_damageReceivedThisTurn` only as optional diagnostic evidence, not as the forecast budget source.

A later UI-only enhancement may display the observed spent budget on or near the Beating Remnant relic icon, for example `spent/20`.

## Mechanism Evidence

- `TungstenRod.ModifyHpLostAfterOsty` applies `max(0, amount - HpLossReduction)` to the owner creature.
- `BeatingRemnant.ModifyHpLostAfterOsty` applies `min(amount, MaxHpLoss - DamageReceivedThisTurn)` to the owner creature.
- `BeatingRemnant.AfterDamageReceived` adds `DamageResult.UnblockedDamage` to `DamageReceivedThisTurn`.
- `BeatingRemnant.BeforeSideTurnStart` resets the budget only when the owner creature is in that side-turn's participants.
- `CombatManager.DoTurnEnd` scans `PileType.Hand.GetPile(player).Cards` in current hand order, snapshots `HasTurnEndInHandEffect` cards, then invokes `OnTurnEndInHandWrapper` in that snapshot order.
- `Burn` and other cards proven by `CardTurnEndDamageInspector` to call `CreatureCmd.Damage` with a blockable `DamageVar` participate in the same hand-order turn-end chain before enemy attacks.
- `AttackIntent` exposes `GetSingleDamage(...)`, `GetTotalDamage(...)`, and `Repeats`; enemy events are treated as single verified events only when single damage multiplied by repeats equals the total.

## Runtime Validation

Closed by scoped Steam testing and user review for normal gameplay assumptions:

- `TungstenRod` is treated as a unique relic in normal gameplay.
- `BeatingRemnant` uses HUD-observed actual HP loss spent in the current budget window.
- Console-created multiple `TungstenRod` states are excluded from production validation and must not drive behavior changes.

## Validation Notes

- Normal single `TungstenRod` behavior is the production target.
- `BeatingRemnant` budget is intentionally based on observed HP drops rather than native private `_damageReceivedThisTurn`, because console setup can leave native private budget state unsuitable for HUD prediction.
- Multiple `TungstenRod` relics can still produce confusing control-console test cases, but this state is impossible in normal gameplay and is explicitly out of production scope.

## Explicit Unsupported Cases

- Unsplittable aggregate enemy attack HP loss with `TungstenRod` when the final post-block HP loss is positive.
- Multiple `TungstenRod` relics are a console-only invalid state and must not drive production behavior changes.
- Unknown hand-event granularity or order.
- Missing Beating Remnant observed-budget initialization before first HUD observation falls back to 0 and initializes from current HP.
- Any future blockable turn-end hand damage whose `CreatureCmd.Damage` call or `DamageVar` cannot be verified.
- Any future damage modifier outside Tungsten Rod / Beating Remnant.

## Only Next Step

Phase 8: implement the non-Block damage-taking modifier path for the Act 3 Queen crown / `DiamondDiademPower`.

## Commit Hash

Not committed in this task turn.
