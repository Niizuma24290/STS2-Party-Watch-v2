# STS2 Party Watch v2

STS2 Party Watch v2 is a read-only single-player combat forecast HUD for Slay the Spire 2.

It answers one narrow question:

```text
If I end the turn from the current single-player combat state, how much verified damage or HP loss should I expect?
```

## HUD

The default HUD shows only total expected HP loss:

```text
-18
```

- `-N`: the sum of trusted final blockable prediction and trusted direct HP loss.
- Advanced details are optional and disabled by default.
- When enabled, advanced details can show `🛡 N` and `♥ N` as source breakdowns.
- `🛡 N`: verified blockable incoming damage after verified block and HP-loss result modifiers.
- `♥ N`: verified direct HP loss that does not go through Block.

When a value cannot be predicted precisely, Party Watch hides that value instead of showing a guess.

## Supported Single-Player Scope

- Enemy AttackIntent / DeathBlow intent damage.
- Burn-style hand turn-end blockable `DamageVar`.
- Verified end-turn block sources: Frost, PlatingPower, Orichalcum, FakeOrichalcum, RippleBasin, CloakClasp.
- Direct HP loss from Beckon, Bad Luck, and Regret.
- HP loss result modifiers from Tungsten Rod and Beating Remnant within the verified normal-game scope.
- Diamond Diadem enemy attack reduction within the verified single-hit / multi-hit forecast path.
- Session-only HUD settings for enablement, advanced details, turn display freezing, position, and colors.

Read-only safety guarantees:

- v2 reads local combat state and native attack intent damage.
- v2 does not modify combat state, player state, enemy state, cards, powers, relics, or room state.
- v2 does not call real damage, command, RNG, save, or network entry points.
- Unknown is safer than an incorrect prediction.

Explicitly out of scope:

- Multiplayer.
- Remote player state.
- Formal multiplayer damage HUD.
- Generic turn simulation, generic damage engines, or generic HP loss engines.
- Unsupported or unverified relic, power, card, or enemy mechanisms.
- Guessing partial blockable damage when required event order or hit granularity is unknown.
- Persistent game-save or guessed settings-file writes.
- Generic diagnostics systems.

## Installation

Build and publish with the repository toolchain:

```powershell
C:\sts2\dotnet\dotnet.exe build .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore
C:\sts2\dotnet\dotnet.exe publish .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore
```

The publish output is:

```text
src/STS2PartyWatchCode/bin/Release/net9.0/publish/
├─ sts2-party-watch-v2.json
└─ sts2-party-watch-v2.dll
```

For local testing, place those two files in:

```text
C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\sts2-party-watch-v2
```

Do not commit build outputs, DLLs, PDBs, PCKs, logs, `bin/`, `obj/`, `publish/`, or `work/`.

## Current Status

Phase 7 and Phase 8 backend prediction work is complete, including Tungsten Rod, Beating Remnant, and Diamond Diadem support within the verified single-player scope. Phase 9A HUD lifecycle, hiding policy, turn display behavior, settings panel, position, and color controls are complete.

The Workshop workspace is prepared under ignored `work/` files, but this repository state is not a public Workshop release. The next task is to prepare the Workshop cover, tags, and private upload test.
