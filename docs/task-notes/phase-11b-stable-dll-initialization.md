# Phase 11B - Stable DLL Initialization Investigation

Date: 2026-07-04

Scope: focused stable-branch initialization repair. This is not full stable certification, not BaseLib UI work, not Poison expansion, and not a Workshop release task.

## Target

Installed stable game target after the user switched Steam back to the public branch:

- Game root: `C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2`
- `release_info.json`: version `v0.107.1`, branch `v0.107.1`, commit `59260271`, date `2026-06-18T15:43:56-07:00`
- Project reference path: `$(Sts2GameRoot)\data_sts2_windows_x86_64\sts2.dll`
- Resolved reference root: `C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64`
- Local BaseLib dependency present from Workshop subscription: `BaseLib` version `v3.3.4`

## Initial Evidence

The local mods directory contained only Party Watch under:

```text
C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\sts2-party-watch-v2
```

The pre-repair installed Party Watch artifact was:

- DLL timestamp: `2026-07-04 01:58:14`
- DLL size: `87552`
- DLL SHA256: `D3BE541A1C8C0B7F57E7AA2D4D5F48413DEA521BA2149BA7660A5E34AC8F615D`

The first proven failure layer was build-time stable API incompatibility, before requiring an in-game repro.

Build command:

```powershell
C:\sts2\dotnet\dotnet.exe build src\STS2PartyWatchCode\STS2PartyWatchCode.csproj --no-restore -c Release
```

First error:

```text
ForecastRefreshPatch.cs(426,31): error CS0117: "Hook" does not contain a definition for "BeforeSideTurnEnd"
```

After repairing that hook target, the next stable API errors were:

```text
VerifiedTurnEndPowerDamageReader.cs(54,29): error CS1501: "ModifyDamage" has no overload that takes 11 arguments
VerifiedEnemyDamageModifier.cs(124,30): error CS1501: "ModifyDamage" has no overload that takes 11 arguments
VerifiedEnemyDamageModifier.cs(138,29): error CS1501: "ModifyDamage" has no overload that takes 11 arguments
LocalIncomingDamageReader.cs(503,29): error CS1501: "ModifyDamage" has no overload that takes 11 arguments
```

Classification: stable API incompatibility caused by Phase 11A v0.108.0 beta API retarget changes being compiled against the v0.107.1 stable DLL.

## Repair

Minimal repair:

- Kept project references pointed at the current local game install.
- Did not copy game DLL/XML files into the repository.
- Did not alter Workshop files or upload anything.
- Preserved the beta-side targets by making compatibility dynamic instead of reverting to stable-only code.

Code changes:

- `ForecastRefreshPatch.cs`
  - Shared turn-end snapshot logic moved to `CommitTurnEndSnapshot(...)`.
  - Added stable optional Harmony patch for `Hook.BeforeTurnEnd`.
  - Added beta optional Harmony patch for `Hook.BeforeSideTurnEnd`.
  - Each optional patch uses `HarmonyPrepare` plus `AccessTools.Method(...)` so only the hook method present in the running game is patched.
- `HookDamageCompat.cs`
  - Added a small compatibility wrapper around `Hook.ModifyDamage`.
  - Detects the current native signature by reflection.
  - Calls the stable 10-argument form when compiled/running against v0.107.1.
  - Calls the beta 11-argument form with a null `CardPlay` slot when compiled/running against v0.108.0.
- `LocalIncomingDamageReader.cs`
- `VerifiedEnemyDamageModifier.cs`
- `VerifiedTurnEndPowerDamageReader.cs`
  - Replaced direct read-only `Hook.ModifyDamage(...)` preview calls with `HookDamageCompat.ModifyDamage(...)`.

## Build And Install

Restore:

```powershell
C:\sts2\dotnet\dotnet.exe restore src\STS2PartyWatchCode\STS2PartyWatchCode.csproj --configfile NuGet.Config
```

Result: success.

Stable build:

```powershell
C:\sts2\dotnet\dotnet.exe build src\STS2PartyWatchCode\STS2PartyWatchCode.csproj --no-restore -c Release
```

Result: success, 0 warnings, 0 errors.

Installed local development artifact:

- Install directory: `C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\sts2-party-watch-v2`
- Installed DLL timestamp: `2026-07-04 02:08:28`
- Installed DLL size: `89600`
- Installed DLL SHA256: `4B324F5B76A96322B3AD660F21EE04DA241D4DA83B26CC64F25235606FBA899A`

## Runtime Smoke

User launched the stable game after the repair and reported success.

Status labels:

- `BuiltAgainstStable`: verified.
- `StableLoadVerified`: verified by user report after installing the fixed local development artifact.
- `StableSettingsSmokeVerified`: not reverified in this task.
- `StableHudSmokeVerified`: not reverified in this task.
- `StablePartiallyVerified`: stable load smoke passed; broader HUD/settings mechanics remain outside this quick repair.

## Key Files And Successful Pattern

Key files:

- `src/STS2PartyWatchCode/Patches/ForecastRefreshPatch.cs`: stable/beta optional turn-end hook patches and shared final snapshot commit.
- `src/STS2PartyWatchCode/Combat/HookDamageCompat.cs`: compatibility wrapper for stable 10-argument and beta 11-argument `Hook.ModifyDamage`.
- `src/STS2PartyWatchCode/Combat/LocalIncomingDamageReader.cs`: hand turn-end damage preview now uses the compatibility wrapper.
- `src/STS2PartyWatchCode/Combat/VerifiedEnemyDamageModifier.cs`: Diamond Diadem enemy damage preview now uses the compatibility wrapper.
- `src/STS2PartyWatchCode/Combat/VerifiedTurnEndPowerDamageReader.cs`: Constrict / Disintegration turn-end power preview now uses the compatibility wrapper.
- `src/STS2PartyWatchCode/STS2PartyWatchCode.csproj`: still points at the local game install and keeps game assemblies `Private=false`.
- `src/STS2PartyWatchCode/sts2-party-watch-v2.json`: still requires BaseLib `3.3.4`; the subscribed Workshop BaseLib artifact was present during this smoke.

Successful pattern / lessons:

- First prove the failure layer. Switching Steam to stable made the project compile against stable `sts2.dll`, which exposed the API break before needing another in-game crash.
- Treat beta and stable as two valid local API surfaces, not as a choice where one must be broken. Optional Harmony patches let `BeforeTurnEnd` and `BeforeSideTurnEnd` coexist safely.
- Hide native signature drift behind one small wrapper. `HookDamageCompat` keeps the read-only forecast callers clean and prevents future 10/11-argument `ModifyDamage` churn from spreading across combat code.
- Confirm the loaded artifact after every local install. The final stable DLL hash is recorded so later smoke tests can prove they loaded the repaired build.
- Keep the repair outside Phase 12 UI scope. BaseLib configuration work was not expanded or refactored during this stable initialization fix.

## Outcome

Phase 11B is closed as a stable load repair:

- Root cause: stable API incompatibility introduced by v0.108.0 beta retarget work.
- Built against stable v0.107.1.
- Installed local development artifact.
- Stable load smoke verified by user report.
- Settings and HUD behavior were not reverified in this task.
- Beta baseline is intended to be preserved through optional hook detection and `ModifyDamage` signature detection, but beta was not rerun after switching the local install back to stable.
