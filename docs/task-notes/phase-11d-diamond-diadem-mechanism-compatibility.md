# Phase 11D Diamond Diadem Mechanism Compatibility

Date: 2026-07-17

## Scope

Preserve the previously verified card-count Diamond Diadem forecast as recoverable source code while supporting the rewritten v0.109 relic without allowing either mechanism to hide the whole Party Watch HUD.

This is the first mechanism-level compatibility pattern. It does not claim that the complete mod supports both the stable and beta game branches yet.

## Native v0.109 Evidence

Local read-only inspection of the v0.109 `sts2.dll` found:

- `DiamondDiadem` no longer exposes `CardsPlayedThisTurn`.
- `DiamondDiademPower` no longer exists.
- `DiamondDiadem.CanonicalVars` contains a 20-point `BlockVar`.
- `DiamondDiadem.AfterSideTurnStart(...)` checks that the owner is a participant and that `PlayerCombatState.TurnNumber <= 1`.
- The hook then gains the relic's Block and applies one `BlurPower` to the owner.
- The current mechanism does not modify enemy attack damage and has no card-count interaction with `StampedePower`.

Therefore Party Watch must trust the current native attack and Block state for v0.109. It must not replay the legacy 0.5 damage multiplier.

## Source Ownership

- `VerifiedEnemyDamageModifier.cs` detects the available Diamond Diadem mechanism and routes the forecast.
- `LegacyDiamondDiademDamageForecast.cs` retains the old card threshold, future 0.5 per-hit damage, cap ordering, and legacy-only pending `StampedePower` correction.
- `VerifiedPreAttackBlockReader.cs` keeps its independent Ripple Basin / Stampede correction unchanged.

The legacy implementation is formal source code, not a documentation-only snippet.

## Capability Routing

```text
CardsPlayedThisTurn member + DiamondDiademPower type
  -> LegacyCardCountDamageReduction

AfterSideTurnStart member without the legacy surface
  -> FirstTurnBlockAndBlur

Anything else or any reflection failure
  -> NativeOnlyFallback
```

Legacy-only APIs are read by member/type name. The shared DLL has no direct reference to the removed `DiamondDiademPower` type or `CardsPlayedThisTurn` property.

Only the legacy strategy reads the pending Stampede condition. The v0.109 and native-only paths always keep native enemy attack damage.

## Failure Policy

Diamond Diadem compatibility failure is local to the relic forecast:

- unreadable mechanism: keep native attack damage;
- unreadable legacy threshold or card count: keep native attack damage;
- aggregate legacy attack without verified per-hit rounding: keep native attack damage;
- unexpected exception: keep native attack damage.

A Diamond Diadem API change must never turn the complete incoming-damage read into `Unknown` or hide all HUD numbers again.

## Verification

- v0.109 Release build: passed, 0 warnings and 0 errors.
- Local publish/install: completed through `scripts/Install-LocalMod.ps1`.
- Installed DLL SHA256: `A9D4BFC675F48DDC87774EBCC52008027185D70C06ACF43ECCE5B30E7621A296`.
- v0.109 runtime verification: pending.
- Legacy stable-branch runtime re-verification: pending until a fixed old reference/runtime target is available.

## Runtime Matrix

Current mechanism:

- first player turn receives native 20 Block and one Blur;
- Party Watch uses native enemy attack damage;
- Stampede present or absent does not change crown handling;
- later turns do not replay the first-turn grant.

Legacy mechanism:

- current `DiamondDiademPower` trusts native intent and avoids double reduction;
- card count at or below threshold forecasts the 0.5 per-hit reduction;
- card count above threshold keeps native damage;
- pending Stampede at the threshold disables the legacy crown trigger;
- unreadable per-hit granularity keeps native damage instead of hiding the HUD.

## Next Boundary

After both current-mechanism smoke verification and a later legacy-branch verification, use this capability-routing pattern as input to a separate whole-mod stable/beta compatibility task.
