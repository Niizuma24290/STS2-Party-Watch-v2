# Build Environment

## Toolchain

- Use `C:\sts2\dotnet\dotnet.exe`.
- Do not use the system `dotnet` for this repository.
- Target framework: `net9.0`.
- By default, the project references STS2 runtime assemblies from:

```text
C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64
```

- The default can be overridden with `-p:Sts2ReferenceRoot=<reference-root>`.
- `Sts2ReferenceRoot` must point directly at the directory containing `sts2.dll`, `GodotSharp.dll`, and `0Harmony.dll`.
- Reference snapshots are kept outside the repository. The first frozen beta snapshot is:

```text
C:\Users\ROG\Documents\Codex\STS2-reference-snapshots\v0.109.0-beta-c12f634d\data_sts2_windows_x86_64
```

- The first frozen stable snapshot is:

```text
C:\Users\ROG\Documents\Codex\STS2-reference-snapshots\v0.107.1-stable-59260271\data_sts2_windows_x86_64
```

## BaseLib compile dependency

- The compile-only BaseLib dependency is pinned to `3.3.4` and downloaded from the official BaseLib GitHub release.
- `scripts/Restore-BaseLibDependency.ps1` verifies SHA256 `C593F14EAAB504FC1D31C89DA7C029116D269F65706D9612D6F71A048E504235` before accepting the DLL.
- The verified DLL is stored under ignored `work/dependencies/BaseLib/3.3.4/`; it is never committed or copied into the published mod.
- `STS2PartyWatchCode.csproj` uses a direct compile reference. Override it with `-p:BaseLibReferencePath=<verified-dll>` when necessary.
- `scripts/Build-DualTargets.ps1` bootstraps the dependency automatically when the default path is missing.

## ModLoader packaging rules confirmed for this project

Confirmed from the current StS2 mod template wiki and local BaseLib workshop install:

- Local mods are loaded from the game install `mods` directory on Windows.
- A mod is packaged as a small folder containing its manifest JSON, DLL, and optional PCK together.
- The manifest file should use the mod ID as the file stem.
- The manifest `id` determines the DLL/PCK names the game attempts to load.
- The current manifest field names use snake_case, including `has_pck`, `has_dll`, and `affects_gameplay`.
- Code-only mods set `has_dll` to `true` and `has_pck` to `false`.
- v2 has no extra dependency DLLs to ship; STS2, GodotSharp, and Harmony are loaded from the game runtime and are referenced with `Private=false`.

Reference points:

- Alchyr ModTemplate-StS2 wiki: setup, local mods, manifest, and publish rules.
- Local BaseLib install:

```text
C:\Program Files (x86)\Steam\steamapps\workshop\content\2868840\3737335127\BaseLib
├─ BaseLib.json
├─ BaseLib.dll
└─ BaseLib.pck
```

## v2 project rules

- Project: `src/STS2PartyWatchCode/STS2PartyWatchCode.csproj`
- Assembly name: `sts2-party-watch-v2`
- Manifest: `src/STS2PartyWatchCode/sts2-party-watch-v2.json`
- Mod ID: `sts2-party-watch-v2`
- Entry class: `STS2PartyWatch.MainFile`
- Entry marker: `[ModInitializer(nameof(Initialize))]`
- Startup log marker: `[STS2 Party Watch] Loaded`

The publish output must not contain `.deps.json`. The ModLoader scans JSON files under mod folders as manifests, so dependency JSON files can be misread as invalid mod metadata. The project therefore sets `GenerateDependencyFile=false`.

## Commands

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Restore-BaseLibDependency.ps1
C:\sts2\dotnet\dotnet.exe restore .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj
C:\sts2\dotnet\dotnet.exe build .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore
C:\sts2\dotnet\dotnet.exe publish .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Save-Sts2ReferenceSnapshot.ps1 -SnapshotName v0.109.0-beta-c12f634d
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Save-Sts2ReferenceSnapshot.ps1 -ReferenceRoot C:\Users\ROG\Documents\Codex\STS2-Party-Watch-v2\work\reflect\bin\Debug\net9.0 -SnapshotName v0.107.1-stable-59260271 -Version v0.107.1 -Branch v0.107.1 -Commit 59260271 -ReleaseDate 2026-06-18T15:43:56-07:00
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Build-DualTargets.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Build-DualTargets.ps1 -BetaReferenceRoot C:\Users\ROG\Documents\Codex\STS2-reference-snapshots\v0.109.0-beta-c12f634d\data_sts2_windows_x86_64
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Build-DualTargets.ps1 -StableReferenceRoot C:\Users\ROG\Documents\Codex\STS2-reference-snapshots\v0.107.1-stable-59260271\data_sts2_windows_x86_64 -BetaReferenceRoot C:\Users\ROG\Documents\Codex\STS2-reference-snapshots\v0.109.0-beta-c12f634d\data_sts2_windows_x86_64
git diff --check
git status
```

Expected publish output:

```text
src/STS2PartyWatchCode/bin/Release/net9.0/publish/
├─ sts2-party-watch-v2.json
└─ sts2-party-watch-v2.dll

work/publish/beta/sts2-party-watch-v2/
├─ sts2-party-watch-v2.json
└─ sts2-party-watch-v2.dll

work/publish/stable/sts2-party-watch-v2/
├─ sts2-party-watch-v2.json
└─ sts2-party-watch-v2.dll
```

Forbidden publish/commit artifacts:

```text
*.deps.json
*.dll in git
*.pdb
*.pck
*.log
bin/
obj/
work/
NuGet cache
```

## Local install rule

Expected local install directory:

```text
C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\sts2-party-watch-v2
```

Expected install tree:

```text
mods/
└─ sts2-party-watch-v2/
   ├─ sts2-party-watch-v2.json
   └─ sts2-party-watch-v2.dll
```

There must not be an extra nested `publish/` directory under the mod folder.

Phase 9 task cards use only the explicit commands above. Do not use `STS2PartyWatch.sln`, `NuGet.Config`, or `tools/check-forbidden-files.ps1` for Phase 9.

## Dual target build baseline

- `scripts/Save-Sts2ReferenceSnapshot.ps1` copies the current installed game's or an explicit `ReferenceRoot`'s `release_info.json`, `sts2.dll`, `sts2.xml`, `GodotSharp.dll`, and `0Harmony.dll` to a repository-external snapshot and writes `manifest.json` plus `SHA256SUMS.txt`.
- With no reference-root arguments, `scripts/Build-DualTargets.ps1` reads each snapshot's `release_info.json`, selects stable `v0.107.1` and beta `v0.109.0`, and builds both targets.
- Explicit `StableReferenceRoot` and `BetaReferenceRoot` arguments remain available for one-target builds or manual overrides.
- Stable and beta outputs are independent: `work/bin/<target>/`, `work/obj/<target>/`, and `work/publish/<target>/sts2-party-watch-v2/`.
- The script redirects NuGet and MSBuild scratch paths into ignored `work/` directories to avoid machine temp lock issues.
- The script fails immediately when `dotnet restore`, `build`, or `publish` returns a non-zero exit code.
- Current beta baseline: v0.109.0 / branch `v0.109.0` / commit `c12f634d`, build passed with 0 warnings and 0 errors.
- Current stable baseline: v0.107.1 / branch `v0.107.1` / commit `59260271`, captured from the old `work/reflect/bin/Debug/net9.0` reference candidate; `sts2.xml` shows the stable hook surface with `Hook.BeforeTurnEnd` and 10-argument `Hook.ModifyDamage`. Build passed with 0 warnings and 0 errors.
- The shared mod manifest sets `min_game_version` to `0.107.1`, so it applies to both baselines; API selection comes from the reference DLLs, not from separate manifest JSON files.

## Contract tests

The dependency-light executable harness currently covers conservative verified
turn-end card-shape rules, forecast-result clamping, configuration text,
BaseLib title compatibility, and the approved audit/Phase 13A seams. Run it once
against each supported reference root:

```powershell
C:\sts2\dotnet\dotnet.exe run --project .\tests\STS2PartyWatchCode.ContractTests\STS2PartyWatchCode.ContractTests.csproj -c Release -p:Sts2ReferenceRoot=C:\Users\ROG\Documents\Codex\STS2-reference-snapshots\v0.107.1-stable-59260271\data_sts2_windows_x86_64
C:\sts2\dotnet\dotnet.exe run --project .\tests\STS2PartyWatchCode.ContractTests\STS2PartyWatchCode.ContractTests.csproj -c Release -p:Sts2ReferenceRoot=C:\Users\ROG\Documents\Codex\STS2-reference-snapshots\v0.109.0-beta-c12f634d\data_sts2_windows_x86_64
```

Each run must report `28 contract tests passed.` The 28/28 stable and beta
closure evidence is recorded in
[`phase-12c-g4-runtime-verification.md`](task-notes/phase-12c-g4-runtime-verification.md)
and
[`phase-12c-g5-repository-closure.md`](task-notes/phase-12c-g5-repository-closure.md).
Historical audit notes
retain earlier 7/10/17/21-test checkpoints as evidence of their original dates.
This harness does not replace the game runtime matrix for Harmony, Godot UI,
combat lifecycle, or BaseLib integration.

## Not verified in this task

- Steam launch.
- Game Mod list visibility.
- Runtime Loaded log.
- HUD creation, hiding, placement, or attack forecast behavior.
