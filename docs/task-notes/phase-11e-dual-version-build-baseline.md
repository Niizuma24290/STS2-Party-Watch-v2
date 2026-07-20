# Phase 11E - Dual Version Build Baseline

Date: 2026-07-18

Status: Closed as `BuildVerified / RuntimeUnverified`.

This phase proves repeatable compilation and packaging against the frozen stable and beta reference assemblies. It does not prove that either output loads or behaves correctly in its matching game branch.

## Scope

Establish the build baseline for future stable / beta compatibility verification without changing forecast mechanics.

This task does not claim full stable / beta runtime compatibility. It freezes repository-external reference surfaces for the current v0.109 beta and a recovered v0.107.1 stable candidate, then proves that the project can build from explicit reference roots into independent stable and beta output directories.

## Implemented

- `src/STS2PartyWatchCode/STS2PartyWatchCode.csproj`
  - Added `Sts2ReferenceRoot`.
  - Default remains `$(Sts2GameRoot)\data_sts2_windows_x86_64`.
  - `sts2.dll`, `GodotSharp.dll`, and `0Harmony.dll` now resolve from `Sts2ReferenceRoot`.
  - Explicitly excludes stale generated `.cs` files under the project `bin/` and `obj/` directories when custom target obj directories are used.
- `scripts/Save-Sts2ReferenceSnapshot.ps1`
  - Copies `release_info.json`, `sts2.dll`, `sts2.xml`, `GodotSharp.dll`, and `0Harmony.dll` to a repository-external snapshot.
  - Can capture from the installed game root or from an explicit existing `ReferenceRoot`.
  - Can write provided release metadata when the source reference root has no original `release_info.json`.
  - Writes `manifest.json` and `SHA256SUMS.txt`.
- `scripts/Build-DualTargets.ps1`
  - With no reference-root arguments, discovers snapshots by reading `release_info.json` and maps stable `v0.107.1` and beta `v0.109.0` to their reference DLL directories.
  - Still accepts `StableReferenceRoot` and `BetaReferenceRoot` for one-target builds or manual overrides.
  - Uses independent `work/bin/<target>/`, `work/obj/<target>/`, and `work/publish/<target>/sts2-party-watch-v2/` paths.
  - Redirects `NUGET_SCRATCH`, `TEMP`, and `TMP` into ignored `work/` directories.
  - Fails immediately on non-zero `dotnet` exit codes.
- `docs/build-environment.md`
  - Documents explicit reference-root builds, snapshot location, and dual target script usage.

## Captured Beta Snapshot

Snapshot root:

```text
C:\Users\ROG\Documents\Codex\STS2-reference-snapshots\v0.109.0-beta-c12f634d
```

Reference root:

```text
C:\Users\ROG\Documents\Codex\STS2-reference-snapshots\v0.109.0-beta-c12f634d\data_sts2_windows_x86_64
```

Native release info:

```text
version: v0.109.0
branch: v0.109.0
commit: c12f634d
date: 2026-07-17T02:31:41+00:00
```

Key snapshot hashes:

```text
release_info.json 47687FF3B852DE923902C1082C37573CDCFF4670C43374B46D1474C665D9C576
sts2.dll          EE45848FF6319DFC7AF2538D3A52D05D82BEF35EE4C5FD0400DC9EFE8F9054AA
sts2.xml          5B2FFB64D65061621A10A437FE57F1BC2DB9B33E79DA5B8E2EFD7EF0EA672E89
GodotSharp.dll    0E4897ECDFB31456A97C7D8028DFB8D7DBDC632E2F73FC9B438D7B266A139289
0Harmony.dll      EF1898322C9F5C86DC1B0758B272A9C440823B4A41CA9A0B82A3AA6B3D206387
```

## Captured Stable Snapshot

Snapshot root:

```text
C:\Users\ROG\Documents\Codex\STS2-reference-snapshots\v0.107.1-stable-59260271
```

Reference root:

```text
C:\Users\ROG\Documents\Codex\STS2-reference-snapshots\v0.107.1-stable-59260271\data_sts2_windows_x86_64
```

Source reference root:

```text
C:\Users\ROG\Documents\Codex\STS2-Party-Watch-v2\work\reflect\bin\Debug\net9.0
```

Stable identity is based on Phase 11B's recorded v0.107.1 stable metadata plus local file evidence. The source `sts2.xml` confirms the stable API surface with `Hook.BeforeTurnEnd` and the 10-argument `Hook.ModifyDamage` signature.

Native release info recorded in the snapshot:

```text
version: v0.107.1
branch: v0.107.1
commit: 59260271
date: 2026-06-18T15:43:56-07:00
```

Key snapshot hashes:

```text
release_info.json 9F81CB62B1D0A2EE21243942E3BCC25451C2A860D1F4D92438FEE7AFB949F3A5
sts2.dll          A1F9E653F1E28E4076558FEE1E60D218619CB7E057B887C6417F62C62C6D7A52
sts2.xml          940CCC0CD6C2BE3D75AE831A1B91A3375DE571D94FDF896F45B26761148ECCCE
GodotSharp.dll    0E4897ECDFB31456A97C7D8028DFB8D7DBDC632E2F73FC9B438D7B266A139289
0Harmony.dll      EF1898322C9F5C86DC1B0758B272A9C440823B4A41CA9A0B82A3AA6B3D206387
```

## Verification

Beta snapshot command:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Save-Sts2ReferenceSnapshot.ps1 -SnapshotName v0.109.0-beta-c12f634d
```

Stable snapshot command:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Save-Sts2ReferenceSnapshot.ps1 -ReferenceRoot C:\Users\ROG\Documents\Codex\STS2-Party-Watch-v2\work\reflect\bin\Debug\net9.0 -SnapshotName v0.107.1-stable-59260271 -Version v0.107.1 -Branch v0.107.1 -Commit 59260271 -ReleaseDate 2026-06-18T15:43:56-07:00
```

Initial beta-only build command:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Build-DualTargets.ps1 -BetaReferenceRoot C:\Users\ROG\Documents\Codex\STS2-reference-snapshots\v0.109.0-beta-c12f634d\data_sts2_windows_x86_64
```

Dual target build command:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Build-DualTargets.ps1
```

The shared mod manifest remains unchanged. Its `min_game_version` is `0.107.1`, which permits both baselines; the selected reference DLLs determine compile-time API compatibility.

Result:

```text
Stable build: passed, 0 warnings, 0 errors.
Stable publish output: work/publish/stable/sts2-party-watch-v2
Stable output DLL SHA256: A9D4BFC675F48DDC87774EBCC52008027185D70C06ACF43ECCE5B30E7621A296
Stable output JSON SHA256: A12AE8A5FD44292DFF347350CEB64B005C54DE82D5155F5CE1B50A2F6C6C96BA

Beta build: passed, 0 warnings, 0 errors.
Beta publish output: work/publish/beta/sts2-party-watch-v2
Beta output DLL SHA256: A9D4BFC675F48DDC87774EBCC52008027185D70C06ACF43ECCE5B30E7621A296
Beta output JSON SHA256: A12AE8A5FD44292DFF347350CEB64B005C54DE82D5155F5CE1B50A2F6C6C96BA
```

The stable and beta output DLL hashes are identical because the source uses runtime capability detection and optional Harmony patches instead of target-specific compile branches. This is a build result only, not a runtime compatibility claim.

## Important Boundaries

- No forecast, HUD, Poison, Diamond Diadem, BaseLib, or Harmony startup behavior was intentionally changed.
- No game runtime smoke was run in this task.
- No local install was performed in this task.
- Repository-external reference snapshots are not commit artifacts.
- `work/` outputs remain ignored build artifacts.

## Successful Pattern

- Freeze reference assemblies outside the repository before claiming a repeatable branch build.
- Build from an explicit `Sts2ReferenceRoot` instead of whatever Steam currently has installed.
- Keep target-specific `bin/obj/publish` directories; stable and beta builds must not share intermediate outputs.
- Treat native command exit codes explicitly in PowerShell scripts.
- Redirect NuGet/MSBuild scratch paths into ignored workspace directories to avoid machine temp lock failures.

## Next Boundary

Next single task: perform explicit runtime smoke planning for both branches, starting with artifact installation/load verification rules. Do not claim runtime compatibility until each branch has been installed into the matching game branch and smoke verified.

Required future evidence, tracked outside this closed build-baseline phase:

- Stable: matching branch installation, loaded DLL path/hash, startup/load smoke, and minimal HUD smoke.
- Beta: matching branch installation, loaded DLL path/hash, startup/load smoke, and minimal HUD smoke.
