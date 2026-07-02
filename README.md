# STS2 Party Watch v2

STS2 Party Watch v2 is a read-only combat forecast HUD for Slay the Spire 2.

It answers one practical question:

```text
If I end the turn now, how much trusted HP loss should I expect?
```

## HUD

The default HUD shows one total expected HP-loss number:

```text
-18
```

Advanced details are available but disabled by default:

- `🛡 N`: trusted blockable incoming HP loss after verified Block and supported HP-loss result modifiers.
- `♥ N`: trusted direct HP loss that does not go through Block.

When Party Watch cannot predict a value reliably, it hides that value instead of guessing.

## Current Scope

Supported in the current codebase:

- local-player HUD in single-player combat;
- local-player HUD in multiplayer combat when `Show Local HUD in Multiplayer` is enabled;
- enemy AttackIntent / DeathBlow intent damage;
- hand turn-end blockable `DamageVar`, including Burn-style cards;
- Frost, PlatingPower, Orichalcum, FakeOrichalcum, RippleBasin, and CloakClasp;
- Beckon, Bad Luck, and Regret direct HP loss;
- ConstrictPower and DisintegrationPower self-damage;
- IntangiblePower, Tungsten Rod, and Beating Remnant within documented event-granularity limits;
- Diamond Diadem / DiamondDiademPower within documented single-player limits;
- ordinary Poison pre-action enemy survival preview within documented ordinary-enemy limits.

Explicitly not claimed:

- public Workshop release status;
- teammate HUD, shared party HUD, or network-aware multiplayer forecasts;
- full turn simulation;
- generic damage / HP-loss engines;
- unsupported special enemy Poison lifecycles or HP-loss budgets;
- persistent settings writes.

## Settings

Open the native Modding screen, select `Party Watch HUD`, then use the Party Watch settings button in that mod's info panel.

Settings are session-only:

- enable Party Watch HUD;
- show local HUD in multiplayer;
- show advanced `🛡 / ♥` details;
- freeze HUD numbers after turn end;
- position preset and X/Y offset;
- total, shield-detail, and heart-detail colors;
- restore defaults.

Party Watch does not replace the native Settings or Modding menu entry.

## Installation

Build and publish with the repository toolchain:

```powershell
C:\sts2\dotnet\dotnet.exe build .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore
C:\sts2\dotnet\dotnet.exe publish .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore
```

The publish output is:

```text
src/STS2PartyWatchCode/bin/Release/net9.0/publish/
|- sts2-party-watch-v2.json
`- sts2-party-watch-v2.dll
```

For local testing, place those two files in:

```text
C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\sts2-party-watch-v2
```

Do not commit build outputs, DLLs, PDBs, PCKs, logs, `bin/`, `obj/`, `publish/`, `work/`, uploader files, cover assets, `mod_id.txt`, or game directory files.

## Workshop Status

The repository has Workshop preparation and private/subscription test notes under `docs/task-notes/`, and the upload workspace is kept under ignored `work/`.

This README does not describe the mod as publicly published. Public Workshop release status should only be added after an explicit public publish record exists.

## Bugs

I hope it covers 99.99% of cases. Bug reports are welcome. I'll keep improving it.
