# Damage Forecast

Damage Forecast (formerly Party Watch) is a read-only local-player combat forecast HUD for Slay the Spire 2.

It answers one practical question:

```text
If I end the turn now, how much trusted HP loss should I expect?
```

## HUD

The default HUD shows one total expected HP-loss number:

```text
-18
```

Phase 13A also provides an optional incoming-damage value `N`, disabled by
default. It is separate from the existing expected HP-loss value `-N` and does
not change that calculation. Available display modes are:

- `ExpectedHpLossOnly` (default);
- `IncomingDamageOnly`;
- `Both`, with `N` placed to the left or right of `-N`.

Advanced details are available but disabled by default:

- `🛡 N`: trusted blockable incoming HP loss after verified Block and supported HP-loss result modifiers.
- `♥ N`: trusted direct HP loss that does not go through Block.

When Damage Forecast cannot predict a value reliably, it hides that value instead of guessing.

## Current Scope

Supported in the current codebase:

- local-player HUD in single-player combat;
- local-player HUD in multiplayer combat when `Show local-player Damage Forecast HUD in Multiplayer` is enabled;
- enemy AttackIntent / DeathBlow intent damage;
- hand turn-end blockable `DamageVar`, including Burn-style cards, subject to the current damaging Status/Curse HUD defect below;
- Frost, PlatingPower, Orichalcum, FakeOrichalcum, RippleBasin, and CloakClasp;
- exact fixed turn-end HP-loss readers for Beckon, Bad Luck, and Regret, subject to the known hand-classification defect below;
- ConstrictPower and DisintegrationPower self-damage;
- IntangiblePower, Tungsten Rod, and Beating Remnant within documented event-granularity limits;
- capability-routed Diamond Diadem support: preserved legacy card-count reduction plus v0.109 native first-turn Block/Blur handling;
- ordinary Poison pre-action enemy survival preview within documented ordinary-enemy limits.

Explicitly not claimed:

- public Workshop release status;
- teammate HUD, shared party HUD, or network-aware multiplayer forecasts;
- full turn simulation;
- generic damage / HP-loss engines;
- unsupported special enemy Poison lifecycles or HP-loss budgets;
- unsupported persistence paths outside BaseLib's settings storage.

Known behavior defect: in the current v0.3.0 runtime, the user reports that any
damage-dealing Status or Curse card in hand makes the complete HUD disappear;
scenes without such a card behave normally. This supersedes the earlier narrow
Regret/Bad Luck report and includes, rather than isolates, that pair. Historical
Burn and direct-HP-loss runtime evidence remains historical evidence; the exact
current card matrix and root cause are still pending diagnosis. This is recorded
separately from the successful technical-identity/config migration.

## Settings

Open `Main Menu -> Mod Configuration`, then select `Damage Forecast`.

Settings are persisted by BaseLib:

- enable Damage Forecast HUD;
- show the local-player Damage Forecast HUD in multiplayer;
- select the expected-HP-loss/incoming-damage display mode;
- place incoming damage to the left or right when both values are shown;
- choose whether incoming damage includes current Block, supported Power/Orb
  Block, relic Block, Power modifiers, and relic modifiers;
- show advanced `🛡 / ♥` details;
- freeze HUD numbers after turn end;
- position preset and X/Y offset;
- total, shield-detail, and heart-detail colors;
- restore defaults.

Damage Forecast does not replace any existing settings screen. Changes apply immediately.

The active technical identity is `damage-forecast`. Current settings use
`DamageForecast.Settings.DamageForecastBaseLibConfig` and `DamageForecast.cfg`.
The bounded `Compatibility/**` subsystem recognizes the former config only as a
legacy migration source for direct upgrade and rollback support.

## Installation

Build and publish with the repository toolchain:

```powershell
C:\sts2\dotnet\dotnet.exe build .\src\DamageForecast\DamageForecast.csproj -c Release --no-restore
C:\sts2\dotnet\dotnet.exe publish .\src\DamageForecast\DamageForecast.csproj -c Release --no-restore
```

The publish output is:

```text
src/DamageForecast/bin/Release/net9.0/publish/
|- damage-forecast.json
`- damage-forecast.dll
```

Before local testing, generate a read-only install/upgrade plan from the verified two-file staging tree:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Install-LocalMod.ps1 -Mode Plan -StagingDir .\work\publish\stable\damage-forecast
```

The expected target directory is:

```text
C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\damage-forecast
```

Real `Install` or `Rollback` execution requires separate approval and explicit `-Execute`; the default plan does not move files or access Workshop.

Do not commit build outputs, DLLs, PDBs, PCKs, logs, `bin/`, `obj/`, `publish/`, `work/`, uploader files, cover assets, `mod_id.txt`, or game directory files.

## Workshop Status

The repository has Workshop preparation and private/subscription test notes under `docs/task-notes/`, and the upload workspace is kept under ignored `work/`.

This README does not describe the mod as publicly published. Public Workshop release status should only be added after an explicit public publish record exists.

## Bugs

I hope it covers 99.99% of cases. Bug reports are welcome. I'll keep improving it.
