# Phase 13A - Damage Display Mode and Incoming Damage N

Status: Implemented, Built, Installed, SettingsRuntimeVerified. Combat HUD matrix not fully runtime verified.

Commit hash: No commit yet.

Date: 2026-07-16

## Product Semantics

Default HUD behavior remains unchanged:

```text
-N = ForecastResult.OutDamage + ForecastResult.DirectHpLoss
```

Phase 13A adds a separate positive incoming-damage display:

```text
N = known incoming damage after the user-selected local defense / reduction categories
```

`N` is not turn-end actual damage and is not derived by reversing `-N` or `OutDamage`. It is hidden when the selected calculation cannot be built from trusted forward evidence.

## Settings Added

All settings are in the BaseLib generated config page. No custom settings screen was added.

Display controls:

- `DamageDisplayMode`
  - `ExpectedHpLossOnly` - expected HP loss only, default.
  - `IncomingDamageOnly` - incoming damage `N` only.
  - `Both` - show both values.
- `IncomingDamagePlacement`
  - `LeftOfExpectedHpLoss`
  - `RightOfExpectedHpLoss`, default.

Incoming damage calculation controls:

- `IncludeCurrentBlockInIncomingDamage` - UI label: Apply Current Block / 计入当前护盾.
- `IncludePowerBlockInIncomingDamage` - UI label: Apply Power Block / 计入能力护盾.
- `IncludeRelicBlockInIncomingDamage` - UI label: Apply Relic Block / 计入遗物护盾.
- `IncludePowerHpLossModifiersInIncomingDamage` - UI label: Apply Power Damage Reduction / 计入能力减伤.
- `IncludeRelicHpLossModifiersInIncomingDamage` - UI label: Apply Relic Damage Reduction / 计入遗物减伤.

Default values keep upgrade behavior stable: `DamageDisplayMode = ExpectedHpLossOnly`, incoming placement right, and every incoming-defense switch off.

The config group title is:

```text
Incoming Damage N Calculation / 来袭总伤害 N 的计算方式
```

## Key Files

- `src/STS2PartyWatchCode/Combat/IncomingDamageDisplayOptions.cs`
- `src/STS2PartyWatchCode/Combat/IncomingDamageDisplayRead.cs`
- `src/STS2PartyWatchCode/Forecast/ForecastHudSnapshot.cs`
- `src/STS2PartyWatchCode/Combat/LocalIncomingDamageReader.cs`
- `src/STS2PartyWatchCode/Combat/VerifiedPreAttackBlockReader.cs`
- `src/STS2PartyWatchCode/Combat/VerifiedHpLossResultModifier.cs`
- `src/STS2PartyWatchCode/Patches/ForecastRefreshPatch.cs`
- `src/STS2PartyWatchCode/UI/PartyWatchHudDisplay.cs`
- `src/STS2PartyWatchCode/UI/PartyWatchHudSnapshotStore.cs`
- `src/STS2PartyWatchCode/UI/PartyWatchUiSettings.cs`
- `src/STS2PartyWatchCode/Settings/PartyWatchBaseLibConfig.cs`
- `src/STS2PartyWatchCode/Settings/PartyWatchSettingsAdapter.cs`
- `src/STS2PartyWatchCode/Settings/PartyWatchConfigText.cs`

## Design Notes

- `ForecastResult` remains the existing `-N` display contract.
- `ForecastHudSnapshot` carries both the existing expected HP loss and the new incoming damage display value.
- `LocalIncomingDamageReader.ReadIncomingDamageForLocalCreature(...)` builds `N` from the same trusted source readers, but is more conservative than the existing direct-only fallback: if a total incoming source is unsupported, `N` becomes Unknown.
- `VerifiedPreAttackBlockReader` now exposes split `PowerBlock` and `RelicBlock` while preserving the old aggregate `Block` property for existing `-N` code.
- `VerifiedHpLossResultModifier.Apply(...)` now accepts optional power/relic modifier flags. Defaults preserve existing `-N` behavior.
- HUD layout now uses three measured text segments: expected loss, incoming damage, and advanced details.
- The existing `-N` label remains the anchor when it is visible. Incoming-only mode uses the incoming label at the same anchor point.
- Advanced details remain to the right of the visible number group.

## Supported Boundaries

Supported incoming sources match current trusted readers:

- Enemy `AttackIntent` values that are supported by existing survival and damage-modifier readers.
- Hand turn-end blockable `DamageVar`.
- `ConstrictPower` and `DisintegrationPower` turn-end blockable damage.
- Beckon, Bad Luck, and Regret direct HP-loss events.
- Current Block.
- Power-style pre-attack Block: Frost and PlatingPower.
- Relic pre-attack Block: Orichalcum, FakeOrichalcum, RippleBasin, CloakClasp.
- Power HP-loss result modifier: local IntangiblePower in the currently verified event-granularity scope.
- Relic HP-loss result modifiers: TungstenRod and BeatingRemnant in the currently verified event-granularity scope.

Unsupported or incomplete selected paths hide `N` rather than showing a partial total.

## Verification

Build:

```text
C:\sts2\dotnet\dotnet.exe build .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore
```

Result:

```text
Build succeeded.
0 warnings.
0 errors.
```

Static check:

```text
git diff --check
```

Result: no whitespace errors. Git printed existing line-ending warnings for dirty files.

Install:

```text
powershell -ExecutionPolicy Bypass -File .\scripts\Install-LocalMod.ps1
```

Result:

```text
Installed sts2-party-watch-v2 to C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\sts2-party-watch-v2
```

Installed files:

```text
sts2-party-watch-v2.dll   105984 bytes   2026-07-16 18:27:39
sts2-party-watch-v2.json     376 bytes   2026-07-03 22:05:11
```

Runtime verification recorded:

- User confirmed on 2026-07-16 that the settings page / settings controls are OK after local install.
- This verifies the BaseLib config surface for Phase 13A at smoke level.
- This does not claim the full combat HUD matrix is complete.

Not performed:

- Full Steam combat HUD matrix.

## Closure

Phase 13A is closed for implementation, build, install, and settings-page smoke verification.

Remaining optional runtime matrix if deeper combat validation is needed:

- default upgrade behavior still shows only `-N`;
- incoming-only mode;
- both mode with left and right placement;
- current Block, Power Block, Relic Block toggles;
- Intangible, TungstenRod, and BeatingRemnant boundaries;
- covering-screen and frozen-snapshot behavior.

Phase 13B remains separate: health-bar difference display.
