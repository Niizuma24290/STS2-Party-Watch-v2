# Session Handoff - Dual Target Build Baseline

> Historical / Superseded handoff. Preserve as dated evidence.
> Current task and authority: [`README.md`](README.md) + [`docs/project-state.md`](../project-state.md).

Date: 2026-07-18

## Project Position

```text
Project: STS2 Party Watch v2
Repository: C:\Users\ROG\Documents\Codex\STS2-Party-Watch-v2
Branch: main
Latest commit at session start: 4febf61d68dd5bb0000955d75aa9fe6b99c3d692
Current phase: Phase 11E dual version build baseline
```

Current one-line state:

```text
The v0.109 beta reference assemblies are frozen outside the repo, and the project can build from an explicit beta reference root into an independent beta output directory. Stable reference capture and full dual-target verification are still pending.
```

## Original Task

Continue from the 2026-07-17 handoff, but first state the plan before doing the work.

The prior handoff's next single task was:

```text
Establish the dual-version build baseline: capture the current v0.109 beta reference assemblies and hash manifest outside the repo, then make STS2PartyWatchCode.csproj plus a new build script support explicit Sts2ReferenceRoot and independent stable / beta output directories. First prove the current beta snapshot. Do not change forecast mechanics.
```

## Completed

- Read the prior handoff and required compatibility notes.
- Confirmed current game install is v0.109.0 / branch `v0.109.0` / commit `c12f634d`.
- Added `Sts2ReferenceRoot` to `STS2PartyWatchCode.csproj`.
- Added `scripts/Save-Sts2ReferenceSnapshot.ps1`.
- Added `scripts/Build-DualTargets.ps1`.
- Captured the current v0.109 beta reference snapshot outside the repository.
- Ran beta target build/publish from the frozen beta snapshot.
- Documented the build baseline in `docs/build-environment.md`.
- Added `docs/task-notes/phase-11e-dual-version-build-baseline.md`.

## Files Changed This Session

New:

```text
scripts/Build-DualTargets.ps1
scripts/Save-Sts2ReferenceSnapshot.ps1
docs/task-notes/phase-11e-dual-version-build-baseline.md
docs/task-notes/session-handoff-dual-target-build-baseline-2026-07-18.md
```

Modified:

```text
src/STS2PartyWatchCode/STS2PartyWatchCode.csproj
docs/build-environment.md
docs/task-notes/README.md
```

Repository-external snapshot:

```text
C:\Users\ROG\Documents\Codex\STS2-reference-snapshots\v0.109.0-beta-c12f634d
```

## Verification

Snapshot command:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Save-Sts2ReferenceSnapshot.ps1 -SnapshotName v0.109.0-beta-c12f634d
```

Build command:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Build-DualTargets.ps1 -BetaReferenceRoot C:\Users\ROG\Documents\Codex\STS2-reference-snapshots\v0.109.0-beta-c12f634d\data_sts2_windows_x86_64
```

Result:

```text
Beta build: passed, 0 warnings, 0 errors.
Beta publish: passed.
Publish directory: work/publish/beta/sts2-party-watch-v2
DLL SHA256: A9D4BFC675F48DDC87774EBCC52008027185D70C06ACF43ECCE5B30E7621A296
JSON SHA256: A12AE8A5FD44292DFF347350CEB64B005C54DE82D5155F5CE1B50A2F6C6C96BA
```

Known script issues fixed during the session:

```text
- Direct .ps1 execution was blocked by local PowerShell execution policy, so verification uses powershell -NoProfile -ExecutionPolicy Bypass -File.
- Native dotnet failures initially did not stop the script; Build-DualTargets now checks $LASTEXITCODE.
- NuGet/MSBuild temp access failed under machine temp paths; Build-DualTargets redirects scratch/temp to ignored work/ directories.
- Target-specific obj output initially missed project.assets.json; restore now runs per target with the same BaseIntermediateOutputPath.
- Old project obj generated .cs files caused duplicate assembly attributes; csproj now excludes bin/ and obj/ generated .cs files from default compile globs.
```

## Not Done

```text
- No v0.107.1 stable reference root was captured.
- No stable target build was run.
- No simultaneous stable + beta build was run.
- No local install was performed.
- No runtime smoke was performed.
- No forecast, HUD, Poison, Diamond Diadem, BaseLib, or Harmony startup logic was intentionally changed.
- No commit was created.
```

## Current Git State Reminder

The worktree was already dirty before this session, with many source and documentation changes from earlier tasks. Do not reset, checkout, or clean those changes.

Expected new files from this session are listed above. `work/` build output and `C:\Users\ROG\Documents\Codex\STS2-reference-snapshots\...` are artifacts, not commit targets.

## Next Single Task

```text
Capture or otherwise provide a v0.107.1 stable reference root outside the repository, then run scripts/Build-DualTargets.ps1 with both StableReferenceRoot and BetaReferenceRoot. Keep forecast mechanics unchanged.
```

Do not claim full stable / beta compatibility until both reference roots build and both runtime branches are explicitly smoke verified.
