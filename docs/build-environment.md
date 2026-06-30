# Build Environment

## Toolchain

- Use `C:\sts2\dotnet\dotnet.exe`.
- Do not use the system `dotnet` for this repository.
- Target framework: `net9.0`.
- The project references STS2 runtime assemblies from:

```text
C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64
```

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
C:\sts2\dotnet\dotnet.exe build .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore
C:\sts2\dotnet\dotnet.exe publish .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore
git diff --check
git status
```

Expected publish output:

```text
src/STS2PartyWatchCode/bin/Release/net9.0/publish/
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

## Not verified in this task

- Steam launch.
- Game Mod list visibility.
- Runtime Loaded log.
- HUD creation, hiding, placement, or attack forecast behavior.
