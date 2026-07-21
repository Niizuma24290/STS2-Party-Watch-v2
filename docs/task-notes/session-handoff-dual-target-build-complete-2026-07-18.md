# Session Handoff - Dual Target Build Complete, Runtime Unverified

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
Repository-external stable and beta reference snapshots now exist, and one no-argument command discovers their versions and builds both into independent output directories. Runtime compatibility is still not claimed.
```

Closure status: `BuildVerified / RuntimeUnverified`. Phase 11E is closed at the build-baseline boundary; runtime smoke remains separate future work.

## Completed This Continuation

- Searched for a v0.107.1 stable reference root.
- Found an old stable-shaped reference candidate at:

```text
C:\Users\ROG\Documents\Codex\STS2-Party-Watch-v2\work\reflect\bin\Debug\net9.0
```

- Confirmed the candidate has `sts2.dll`, `sts2.xml`, `GodotSharp.dll`, and `0Harmony.dll`.
- Confirmed its `sts2.xml` exposes the stable hook surface:

```text
Hook.BeforeTurnEnd
Hook.ModifyDamage(... CardModel, ModifyDamageHookType, CardPreviewMode, out IEnumerable<AbstractModel>)
```

- Extended `scripts/Save-Sts2ReferenceSnapshot.ps1` so it can capture from an explicit `ReferenceRoot`.
- Captured stable snapshot outside the repository.
- Ran `scripts/Build-DualTargets.ps1` with both stable and beta reference roots.
- Simplified `scripts/Build-DualTargets.ps1` so its no-argument mode reads snapshot `release_info.json` files and selects stable v0.107.1 plus beta v0.109.0 automatically.
- Updated `docs/build-environment.md`.
- Updated `docs/task-notes/phase-11e-dual-version-build-baseline.md`.
- Updated `docs/task-notes/README.md`.

## Reference Snapshots

Stable:

```text
C:\Users\ROG\Documents\Codex\STS2-reference-snapshots\v0.107.1-stable-59260271\data_sts2_windows_x86_64
version: v0.107.1
branch: v0.107.1
commit: 59260271
sts2.dll SHA256: A1F9E653F1E28E4076558FEE1E60D218619CB7E057B887C6417F62C62C6D7A52
```

Beta:

```text
C:\Users\ROG\Documents\Codex\STS2-reference-snapshots\v0.109.0-beta-c12f634d\data_sts2_windows_x86_64
version: v0.109.0
branch: v0.109.0
commit: c12f634d
sts2.dll SHA256: EE45848FF6319DFC7AF2538D3A52D05D82BEF35EE4C5FD0400DC9EFE8F9054AA
```

## Verification

Dual target command:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Build-DualTargets.ps1
```

Result:

```text
Stable build: passed, 0 warnings, 0 errors.
Stable publish: passed.
Stable output: work/publish/stable/sts2-party-watch-v2

Beta build: passed, 0 warnings, 0 errors.
Beta publish: passed.
Beta output: work/publish/beta/sts2-party-watch-v2
```

Output hashes:

```text
Stable DLL SHA256: A9D4BFC675F48DDC87774EBCC52008027185D70C06ACF43ECCE5B30E7621A296
Stable JSON SHA256: A12AE8A5FD44292DFF347350CEB64B005C54DE82D5155F5CE1B50A2F6C6C96BA
Beta DLL SHA256:   A9D4BFC675F48DDC87774EBCC52008027185D70C06ACF43ECCE5B30E7621A296
```

The identical stable/beta DLL hash is expected for this code shape because the compatibility layer uses runtime reflection and optional Harmony patch discovery.

## Not Done

```text
- No local install was performed.
- No Steam branch switch was performed.
- No game runtime smoke was performed.
- No runtime loaded-module verification was performed.
- No forecast, HUD, Poison, Diamond Diadem, BaseLib, or Harmony startup logic was intentionally changed.
- No commit was created.
```

## Git State Reminder

The worktree remains dirty from earlier tasks. Do not reset, checkout, or clean unknown changes.

New/modified files from this continuation:

```text
scripts/Save-Sts2ReferenceSnapshot.ps1
docs/build-environment.md
docs/task-notes/README.md
docs/task-notes/phase-11e-dual-version-build-baseline.md
docs/task-notes/session-handoff-dual-target-build-complete-2026-07-18.md
```

Artifacts, not commit targets:

```text
work/bin/
work/obj/
work/publish/
C:\Users\ROG\Documents\Codex\STS2-reference-snapshots\
```

## Next Single Task

```text
Plan and execute runtime smoke for one branch at a time: install the matching build into the matching Steam branch, prove the loaded DLL path/hash, then run minimal HUD/load checks. Do not claim stable/beta runtime compatibility until both branches have explicit runtime evidence.
```
