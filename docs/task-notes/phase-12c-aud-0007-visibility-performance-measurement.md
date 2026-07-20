# AUD-0007 — Global CanvasItem Visibility Patch Performance Measurement

Date: 2026-07-20
Status: Complete — measured non-material; Production restored
Task type: diagnostic measurement only
Parent finding: `AUD-0007` in `docs/task-notes/phase-12c-audit/code-audit-findings.md`

## Copy This Into the New Session

```text
Read and execute:
C:\Users\ROG\Documents\Codex\STS2-Party-Watch-v2\docs\task-notes\phase-12c-aud-0007-visibility-performance-measurement.md

This is a measurement-only task for AUD-0007. Implement low-overhead diagnostic instrumentation, build the requested A/B artifacts, and guide me through the user runtime matrix. Do not optimize or narrow the production patches unless a later, separately approved gate authorizes a fix. Preserve the dirty worktree. Do not commit, push, launch the game, or install a diagnostic build without asking me first.
```

## 1. Objective

Measure whether the three global Harmony postfixes on:

```text
Godot.CanvasItem.Show()
Godot.CanvasItem.Hide()
Godot.CanvasItem.set_Visible(bool)
```

create material overhead or redundant Party Watch HUD refreshes.

The task must answer:

1. How often is each global postfix called in representative UI/combat scenes?
2. How many calls are rejected by the covering-screen type check?
3. How many calls match a known native combat covering screen?
4. Does one logical visibility transition arrive through more than one patched entry point?
5. How many `ForecastRefreshPatch.RefreshRegisteredBars()` calls result?
6. What are the aggregate and worst measured costs of type checking, the complete callback, and triggered HUD refresh?
7. Does a patch-disabled A/B control show a repeatable performance difference?

This task does not assume that the current implementation is slow.

## 2. Repository and Baseline

```text
Repository: C:\Users\ROG\Documents\Codex\STS2-Party-Watch-v2
Branch: main
Baseline HEAD: 4febf61d68dd5bb0000955d75aa9fe6b99c3d692
Finding status at task creation: Needs Runtime Verification / Low / E1
Stable reference: v0.107.1 stable snapshot
Beta reference: v0.109.0 beta snapshot
Current production DLL SHA256 after G2: 3D558648BF9F359172773ECF44381528F34B75368B99F6B9C9FD9A7F4DAA5639
```

The worktree already contains user-owned tracked and untracked changes. Do not reset, checkout, clean, stash, or delete unknown files.

## 3. Current Code Path

Primary files:

```text
src/STS2PartyWatchCode/Patches/NativeCoveringScreenLifecyclePatch.cs
src/STS2PartyWatchCode/UI/PartyWatchNativeCoveringScreenTracker.cs
src/STS2PartyWatchCode/Patches/ForecastRefreshPatch.cs
```

Current broad path:

```text
CanvasItem.Show/Hide/set_Visible postfix
  -> NativeCoveringScreenVisibilityPatch.UpdateCoveringScreenVisibility(...)
  -> PartyWatchNativeCoveringScreenTracker.IsNativeCombatCoveringScreen(...)
  -> walk runtime type hierarchy and compare against known type names
  -> if matched: MarkOpened/MarkClosed
  -> ForecastRefreshPatch.RefreshRegisteredBars()
```

The registered concern is breadth and possible duplication, not an established player-visible slowdown.

## 4. Authorization Boundary

### Allowed

- Read the current source, build scripts, reference snapshots, and existing runtime notes.
- Add diagnostic-only source guarded by an explicit build property/compile symbol.
- Add deterministic tests for counter aggregation and summary formatting.
- Produce instrumented A and patch-disabled B artifacts under ignored `work/` paths.
- Run restore, contract tests, stable/beta build, publish, hashes, whitelist checks, and `git diff --check`.
- After explicit user approval, install one named diagnostic artifact at a time.
- Read the resulting local game log after the user finishes a run.
- Update this task card and Phase 12C audit ledgers with measured evidence.

### Not allowed without a later approval

- Narrow, replace, debounce, cache, or remove the production visibility patches as a fix.
- Change covering-screen behavior, HUD semantics, settings, forecast mechanics, or Phase 13 behavior.
- Work on AUD-0013, AUD-0008, AUD-0009, G3 Surface Rename, or technical identity migration.
- Add per-callback log lines.
- Commit DLL/PDB/log/publish/work outputs.
- Commit, push, publish Workshop content, or alter remote state.
- Launch the game on behalf of the user.

If measurement identifies material cost, stop with a separate minimal-fix proposal and request approval.

## 5. Required Instrumentation Design

The next session may adjust exact type/member names, but must preserve these properties.

### 5.1 Production-default isolation

Add an MSBuild property whose default is disabled, for example:

```xml
<PartyWatchVisibilityProfiling>false</PartyWatchVisibilityProfiling>
```

Only define the diagnostic compile symbol when explicitly enabled. A normal build must contain no active probe, no diagnostic summaries, and no profiling allocations.

Add a separate diagnostic property for the B control that compiles out or skips only the three global `CanvasItem` visibility postfixes. The default production build must retain the current patches.

Suggested conceptual matrix:

| Artifact | Profiling probe | Global visibility postfixes | Purpose |
| --- | --- | --- | --- |
| Production | Off | On | Normal product; must remain behaviorally unchanged |
| A | On | On | Measure calls, matching, duplication, and callback/refresh cost |
| B | On where useful | Off | Performance control only; covering-screen behavior may intentionally differ |

Do not use the B artifact for correctness acceptance.

### 5.2 Hot-path rules

- Use `Stopwatch.GetTimestamp()` and aggregate ticks.
- Use fixed counters and lock-free/low-lock updates such as `Interlocked`.
- Do not allocate a record, list, formatted string, stack trace, or log entry per callback.
- Do not call reflection beyond what the production type check already does.
- Track duplicate state only after a node has matched a known covering-screen type; a small weak-key diagnostic map is acceptable there.
- Do not sample every frame or add a global `_Process`/frame callback merely for this task.

### 5.3 Required counters

At minimum collect:

```text
measurement window duration
Show postfix calls
Hide postfix calls
set_Visible postfix calls
all visibility postfix calls
non-covering-screen fast returns
covering-screen matches
real open/close state transitions
duplicate/no-state-change notifications
RefreshRegisteredBars calls caused by this visibility patch
registered health bars visited by those refreshes, if available without broad refactoring
type-check aggregate ticks and maximum ticks
full callback aggregate ticks and maximum ticks
triggered HUD-refresh aggregate ticks and maximum ticks
```

Derived values:

```text
callbacks per second
match percentage
duplicate percentage among matched notifications
refreshes per real transition
average and maximum type-check microseconds
average and maximum complete-callback microseconds
callback milliseconds per second of runtime
average and maximum triggered-refresh microseconds
```

### 5.4 Summary output

Use one stable prefix:

```text
[STS2 Party Watch][AUD-0007]
```

Emit summaries only at low-frequency boundaries, preferably:

- recognized covering-screen close: snapshot only, without resetting the full combat counters;
- `Hook.AfterCombatEnd`: final combat summary and reset;
- one explicit diagnostic dump point if a safe existing debug entry is available.

Example schema:

```text
[STS2 Party Watch][AUD-0007] reason=combat-end window_s=123.4 show=... hide=... set_visible=... total=... unmatched=... matched=... transitions=... duplicates=... refreshes=... bars_visited=... type_total_ms=... type_avg_us=... type_max_us=... callback_total_ms=... callback_ms_per_s=... callback_max_us=... refresh_total_ms=... refresh_avg_us=... refresh_max_us=...
```

Formatting occurs only when dumping a summary, never inside every postfix.

## 6. Build and Static Verification for the Next Session

Before code changes:

```powershell
git status --short --branch
git rev-parse HEAD
git diff --cached --name-only
```

After instrumentation:

1. Run the existing contract harness against stable and beta.
2. Run the existing dual-target production build and confirm 0 warnings / 0 errors.
3. Build A and B into separate ignored directories, for example:

```text
work/perf/a/sts2-party-watch-v2/
work/perf/b/sts2-party-watch-v2/
```

4. Record each DLL SHA256 and exact compile properties.
5. Confirm each publish directory contains only:

```text
sts2-party-watch-v2.dll
sts2-party-watch-v2.json
```

6. Confirm the normal production build still contains the global visibility patch and no active profiling output.
7. Run:

```powershell
git diff --check
git status --short
git diff --cached --name-only
```

Do not install either diagnostic build until the user explicitly approves the named artifact/hash.

## 7. Installation Gate

The game must be closed before replacing the DLL.

The next session should:

1. Present A/B hashes and identify which one will be installed.
2. Ask the user for explicit installation approval.
3. Install only the approved artifact.
4. Verify installed DLL hash equals the selected artifact hash.
5. Never run A and B together or leave duplicate mod directories.

`scripts/Install-LocalMod.ps1` normally republishes the default project. For diagnostic artifacts, use its `-SkipPublish -PublishDir <exact-path>` path only after verifying the resolved directory and receiving approval, or extend the script narrowly so the selected diagnostic output is unambiguous.

## 8. What the User Must Do

The user only performs game runtime actions. Codex handles instrumentation, builds, hashes, installation after approval, and log analysis.

### Before each run

- Close the game before Codex installs the selected A or B artifact.
- Keep the same Steam branch, resolution, graphics settings, VSync/frame cap, mods, character, and test route for A and B.
- Do not change Party Watch settings between A and B.
- After Codex confirms the installed hash, launch the game manually through Steam.

### Run A — instrumented current behavior

Perform one warm-up, then three measured passes when practical.

#### M1: Combat idle

1. Enter an ordinary combat.
2. Do not open overlays or play cards for 60 seconds.
3. Confirm there is no visible HUD malfunction.

#### M2: Normal combat activity

1. Play at least five cards with ordinary animations.
2. End two player turns.
3. Let two enemy turns complete.
4. Note any visible hitch or HUD flicker.

#### M3: Covering-screen transitions

During combat, open and close each available screen ten times:

```text
pause/settings
deck/card pile
map
any other native covering screen already listed by the mod
```

Include at least five rapid open/close cycles and at least one open/close after committing the frozen end-turn snapshot.

#### M4: Combat boundary

1. Finish the combat.
2. Enter a second combat.
3. Repeat one ordinary screen open/close.
4. Finish or safely exit so the combat-end summary is written.

After the run, exit the game and tell the session:

```text
A 测试完成，可以读取日志。
```

The session should locate and read the newest relevant log; the user should not need to manually extract counters unless log discovery fails.

### Run B — global-patch-disabled control

After Codex presents and installs B with approval:

1. Repeat M1, M2, and M3 with the same settings and approximate timing.
2. Ignore expected differences in covering-screen hiding; B is a performance control, not a functional build.
3. Record only repeatable stutter/FPS differences, not a single isolated frame hitch.
4. Exit the game and report:

```text
B 测试完成，可以读取日志。
```

If the user has a trusted existing performance overlay, record average FPS and 1% low for each measured pass. Do not require installing a new third-party profiler merely for this Low-severity finding.

## 9. Evidence Table the Next Session Must Fill

| Metric | A idle | A normal combat | A overlay loop | B comparable run |
| --- | ---: | ---: | ---: | ---: |
| Window seconds | TBD | TBD | TBD | TBD |
| Calls/second | TBD | TBD | TBD | N/A or TBD |
| Unmatched percentage | TBD | TBD | TBD | N/A |
| Matched notifications | TBD | TBD | TBD | N/A |
| Real transitions | TBD | TBD | TBD | N/A |
| Duplicate percentage | TBD | TBD | TBD | N/A |
| Refreshes/transition | TBD | TBD | TBD | N/A |
| Callback ms/second | TBD | TBD | TBD | N/A |
| Maximum callback µs | TBD | TBD | TBD | N/A |
| Refresh total/maximum | TBD | TBD | TBD | N/A |
| Average FPS, if available | TBD | TBD | TBD | TBD |
| 1% low, if available | TBD | TBD | TBD | TBD |
| Visible hitch/flicker | TBD | TBD | TBD | TBD |

Also record:

```text
Steam branch/game version
A DLL hash
B DLL hash
installed hash for each run
other enabled mods
resolution/VSync/frame cap
exact screens exercised
number of repetitions
log path and relevant summary lines
```

## 10. Operational Decision Rules

These are task-specific triage thresholds, not universal engine guarantees.

### Close AUD-0007 as measured/non-material

Use this disposition when all representative runs show:

- callback aggregate cost at or below approximately `1 ms per second` of runtime;
- no repeatable callback spike above `0.25 ms`, after excluding known full HUD refresh cost and measurement noise;
- no more than one redundant refresh beyond the real open/close transition expectation;
- no repeatable A/B average-FPS or 1%-low degradation greater than roughly 3%;
- no visible transition hitch attributable to Party Watch.

### Produce a separate optimization proposal

Stop and propose, but do not implement, a fix when any repeated scenario shows:

- callback aggregate cost above approximately `5 ms per second`;
- repeated callback/refresh spikes above `1 ms`;
- more than three HUD refreshes for one logical screen transition;
- stable A/B performance degradation above roughly 5%;
- reproducible HUD flicker or transition hitch correlated with duplicate callbacks.

### Gray zone

For results between those bands, repeat the same scenario and report uncertainty. Keep AUD-0007 as `Needs Runtime Verification` rather than forcing a conclusion.

## 11. If Optimization Is Later Authorized

Measurement may support one of these future directions, but this task must not choose or implement one prematurely:

1. Deduplicate `Show/Hide/set_Visible` notifications before refreshing.
2. Refresh only when tracker state actually changes.
3. Cache matched runtime types after first classification.
4. Patch narrower known covering-screen lifecycle methods instead of global `CanvasItem` methods.
5. Preserve the current broad fallback only for game-version compatibility where narrower hooks are absent.

Any future fix requires stable/beta build and the covering-screen freeze/restore runtime regression matrix.

## 12. Required Cleanup

After A/B evidence is captured:

1. Ask the user to close the game.
2. Reinstall the normal production build, not A or B.
3. Verify the installed DLL hash matches the normal production artifact.
4. Confirm no diagnostic DLL, log, PDB, or publish output is Git-visible.
5. Keep raw logs and A/B outputs only under ignored paths.
6. Production source must either compile the probe out by default or remove the temporary probe after evidence is preserved. Do not leave runtime profiling enabled.

## 13. Closure Deliverables

The next session must provide:

- exact instrumentation/source changes;
- A/B build commands, properties, output paths, and hashes;
- stable/beta build and contract-test results;
- completed measurement table;
- representative summary log lines;
- a clear finding disposition with evidence level;
- whether an optimization task is justified;
- confirmation that the production build was restored;
- `git diff --check`, HEAD, staged-state, and artifact-boundary results;
- updates to:

```text
docs/task-notes/phase-12c-aud-0007-visibility-performance-measurement.md
docs/task-notes/phase-12c-audit/code-audit-findings.md
docs/task-notes/phase-12c-audit/audit-progress.md
docs/task-notes/phase-12c-audit/audit-final-report.md
```

Do not mark AUD-0007 Resolved based only on a successful build, raw callback counts without timing, or user-reported lack of obvious lag.

## 14. Stop Conditions

Stop and ask the user before:

- installing A, B, or the restored production build;
- launching or controlling the game;
- making any performance optimization;
- changing production behavior or compatibility coverage;
- committing or pushing;
- expanding into AUD-0013 or G3.

Final task state before runtime evidence should be:

```text
Instrumented / Built / Awaiting User Runtime Measurement
```

## 15. 2026-07-20 Instrumentation and Build Checkpoint

Current state:

```text
Instrumented / Built / Awaiting User Runtime Measurement
```

Instrumentation implemented:

- `PartyWatchVisibilityProfiling=false` is the default. Setting it to `true` defines `PARTY_WATCH_VISIBILITY_PROFILING` and compiles in the AUD-0007 probe.
- `PartyWatchDisableGlobalVisibilityPatches=false` is the default. Setting it to `true` defines `PARTY_WATCH_DISABLE_GLOBAL_VISIBILITY_PATCHES` and compiles out only the three global `CanvasItem` visibility postfixes.
- The probe uses fixed `Interlocked` counters and `Stopwatch.GetTimestamp()` timing. It performs no per-callback logging or record/list allocation.
- The measured callback records entry-point counts, matched/unmatched type checks, real tracker transitions, duplicate notifications, triggered refreshes, registered bars visited, and aggregate/maximum type-check, callback, and refresh durations.
- Cumulative summaries are emitted after a real covering-screen close. `Hook.AfterCombatEnd` emits the final summary and resets the combat window.
- The stable prefix is `[STS2 Party Watch][AUD-0007]`.

Deterministic contract coverage:

```text
stable v0.107.1: 17 contract tests passed
beta v0.109.0:   17 contract tests passed
```

Production dual-target verification:

```text
stable v0.107.1: 0 warnings / 0 errors
beta v0.109.0:   0 warnings / 0 errors
Production DLL SHA256 (both targets): 3D558648BF9F359172773ECF44381528F34B75368B99F6B9C9FD9A7F4DAA5639
```

The Production hash is unchanged from the pre-task G2 artifact. Static inspection confirms that Production contains `NativeCoveringScreenVisibilityPatch` and contains neither `Aud0007VisibilityProfiler` nor the AUD-0007 summary prefix.

Runtime artifacts for the installed beta `v0.109.0` branch:

| Artifact | Compile property | Output | DLL SHA256 |
| --- | --- | --- | --- |
| A | `PartyWatchVisibilityProfiling=true` | `work/perf/a/sts2-party-watch-v2/` | `E39767623FA54FFBCEBE4C4896DD58CDA8D718F1B6F30677A59953EF071D7BE2` |
| B | `PartyWatchDisableGlobalVisibilityPatches=true` | `work/perf/b/sts2-party-watch-v2/` | `73AC1502E601C3E4F0A7F6B4C4B07DDDCE2A6C47ADC7307A422F661A8BDCA9DF` |

Artifact inspection confirms:

- every publish directory contains only `sts2-party-watch-v2.dll` and `sts2-party-watch-v2.json`;
- A contains both the global visibility patch and the AUD-0007 profiler;
- B contains neither the global visibility patch nor the profiler;
- all publish and performance outputs are ignored by `.gitignore` through `work/`;
- `git diff --check` passes;
- HEAD remains `4febf61d68dd5bb0000955d75aa9fe6b99c3d692` and the staged set remains empty.

The user explicitly approved installation of A. With the game confirmed closed, `scripts/Install-LocalMod.ps1 -SkipPublish -PublishDir work/perf/a/sts2-party-watch-v2` installed the named artifact. The installed DLL SHA256 was verified as `E39767623FA54FFBCEBE4C4896DD58CDA8D718F1B6F30677A59953EF071D7BE2`; exactly one Party Watch manifest was present afterward. Codex did not launch the game. Current gate: awaiting user runtime measurement of A.

### First A runtime attempt

The user completed the requested run on beta `v0.109.0` and exited the game. The installed DLL still matched A SHA256 `E39767623FA54FFBCEBE4C4896DD58CDA8D718F1B6F30677A59953EF071D7BE2` afterward. The newest log was:

```text
C:\Users\ROG\AppData\Roaming\SlayTheSpire2\logs\godot.log
Last write: 2026-07-20 22:45:00
```

The log confirmed that the local Party Watch DLL loaded and the mod initialized. It contained multiple combat-room creation entries but no `[STS2 Party Watch][AUD-0007]` summary lines, so this attempt supplies no usable performance counters and cannot close AUD-0007. No Party Watch exception was found. The A probe therefore needs a more reliable diagnostic-only low-frequency dump boundary before repeating the run.

The user also reported a separate functional observation: when opening the in-combat draw-pile or discard-pile viewer, the Party Watch HUD does not hide and remains visible behind/under the covering card screen. Screenshot evidence:

```text
C:\Users\ROG\AppData\Local\Temp\codex-clipboard-a64f9e35-f4b9-4a72-b1d3-b53fa6b75ddb.png
```

The screenshot shows the Party Watch numeric HUD still rendered near the center-bottom health-bar area while the pile viewer covers the combat scene. Static beta reference evidence identifies this viewer as `MegaCrit.Sts2.Core.Nodes.Screens.NCardPileScreen`; that type exists in `sts2.dll`/`sts2.xml` but is absent from `PartyWatchNativeCoveringScreenTracker.CoveringScreenTypeNames`. This is recorded as a covering-screen compatibility/coverage gap, not as an AUD-0007 performance result. No production behavior change is authorized by this observation.

To make diagnostic output independent of covering-screen recognition and the observed non-firing combat-end dump, a revised A2 probe adds one cumulative `reason=periodic` summary every 30 seconds after the first visibility callback. The timer and summary code remain compiled only under `PartyWatchVisibilityProfiling=true`; Production remains byte-identical to the pre-task artifact.

```text
A2 output: work/perf/a2/sts2-party-watch-v2/
A2 compile property: PartyWatchVisibilityProfiling=true
A2 DLL SHA256: C4FE8A64FE308314FBE3052B5803B9D476BCA07687B0D13AC6EAB301718DF9AC
A2 build: 0 warnings / 0 errors
Stable contract tests: 17 passed
Beta contract tests: 17 passed
Production SHA256 after A2: 3D558648BF9F359172773ECF44381528F34B75368B99F6B9C9FD9A7F4DAA5639
```

The user explicitly approved A2 installation. With the game confirmed closed, A2 was installed from the fixed publish directory with `-SkipPublish`. The installed DLL SHA256 matched `C4FE8A64FE308314FBE3052B5803B9D476BCA07687B0D13AC6EAB301718DF9AC`, and exactly one Party Watch manifest remained present. Codex did not launch the game. Current gate: awaiting the shortened A2 runtime measurement.

### A2 runtime result

The user completed the shortened A2 run and exited the game. The installed A2 hash remained correct and the newest `godot.log` confirmed normal Party Watch loading with no Party Watch exception, but again contained no AUD-0007 line. This establishes that the periodic timer was never initialized: A2 still initialized the profiler lazily from the first visibility postfix, so it could not distinguish a genuine zero-call result from a postfix registration/execution problem.

The next diagnostic revision must initialize the profiler explicitly after `Harmony.PatchAll(...)` and write a one-time patch-registration status line for all three targets. This remains diagnostic-only and does not change Production behavior.

A3 implements that diagnostic correction. It starts the 30-second timer from `MainFile.Initialize()`, emits a one-time `probe=started` line reporting whether each target owns the expected postfix, and emits periodic summaries even when all counters are zero. The first compile attempt exposed diagnostic-only syntax/namespace errors; those were corrected before any artifact was published or installed. Final verification:

```text
A3 output: work/perf/a3/sts2-party-watch-v2/
A3 compile property: PartyWatchVisibilityProfiling=true
A3 DLL SHA256: 3F2CC86A0BB12068D968D93589A8B6C0EBDB7778892822148C18EBD890E74CF5
A3 build: 0 warnings / 0 errors
Stable contract tests: 17 passed
Beta contract tests: 17 passed
Production SHA256 after A3: 3D558648BF9F359172773ECF44381528F34B75368B99F6B9C9FD9A7F4DAA5639
```

The user explicitly approved A3 installation. With the game confirmed closed, A3 was installed from the fixed publish directory with `-SkipPublish`. The installed DLL SHA256 matched `3F2CC86A0BB12068D968D93589A8B6C0EBDB7778892822148C18EBD890E74CF5`, and exactly one Party Watch manifest remained present. Codex did not launch the game. Current gate: awaiting the short A3 confirmation run.

### A3 runtime result

The user completed the A3 run and exited the game. The installed hash remained correct and the normal game log again confirmed Party Watch loading without a Party Watch exception, but contained neither `probe=started` nor a periodic summary. The normal load message calls both `GD.Print` and `Console.WriteLine` but appears only once in `godot.log`; combined with the missing A3 lines, this shows that `Console.WriteLine` is not a reliable post-initialization output channel for this runtime.

The next diagnostic artifact must write to a dedicated AUD-0007 file under the game's AppData log directory. This avoids both console-capture dependence and background-thread calls into Godot. Production remains unaffected.

A4 implements the dedicated log channel. At mod initialization it creates `party-watch-aud-0007-<UTC timestamp>.log` under `%APPDATA%\SlayTheSpire2\logs`, writes the patch-registration status immediately, and appends the 30-second summaries under a low-frequency file lock. The first compile attempt found a diagnostic-only `System.Environment`/`Godot.Environment` name ambiguity; it was corrected before publishing or installation. Final verification:

```text
A4 output: work/perf/a4/sts2-party-watch-v2/
A4 compile property: PartyWatchVisibilityProfiling=true
A4 DLL SHA256: 99C24CA8025DD0F409FF427C6D22E3CAE03A3D96773D8FF4733DA26EEE917146
A4 build: 0 warnings / 0 errors
Stable contract tests: 17 passed
Beta contract tests: 17 passed
Production SHA256 after A4: 3D558648BF9F359172773ECF44381528F34B75368B99F6B9C9FD9A7F4DAA5639
```

The user explicitly approved A4 installation. With the game confirmed closed, A4 was installed from the fixed publish directory with `-SkipPublish`. The installed DLL SHA256 matched `99C24CA8025DD0F409FF427C6D22E3CAE03A3D96773D8FF4733DA26EEE917146`, and exactly one Party Watch manifest remained present. Codex did not launch the game. Current gate: awaiting the short A4 confirmation run and dedicated-log inspection.

### A4 measured result

The user completed the A4 run and exited the game. The installed hash remained correct. Dedicated log:

```text
C:\Users\ROG\AppData\Roaming\SlayTheSpire2\logs\party-watch-aud-0007-20260720T153515Z.log
Last write: 2026-07-20 23:43:45 local time
```

The startup line confirmed `show_patched=true`, `hide_patched=true`, and `set_visible_patched=true`. No Party Watch error was found in the normal game log.

Representative interval deltas derived from the cumulative 30-second snapshots:

| Metric | A idle/startup 0-30s | A steady/normal 30-330s | A overlay activity 330-360s | A full 0-510s |
| --- | ---: | ---: | ---: | ---: |
| Window seconds | 30.002 | 300.001 | 29.999 | 510.002 |
| Calls | 1,111 | 18,300 | 2,451 | 30,993 |
| Calls/second | 37.031 | 61.000 | 81.703 | 60.770 |
| Unmatched | 1,111 | 18,300 | 2,447 | 30,989 |
| Matched notifications | 0 | 0 | 4 | 4 |
| Real transitions | 0 | 0 | 2 | 2 |
| Duplicate notifications | 0 | 0 | 2 | 2 |
| Duplicate percentage among matched | N/A | N/A | 50.000% | 50.000% |
| Visibility-triggered refreshes | 0 | 0 | 4 | 4 |
| Bars visited | 0 | 0 | 4 | 4 |
| Refreshes/transition | N/A | N/A | 2.000 | 2.000 |
| Type-check total | 5.886 ms | 110.084 ms | 13.191 ms | 183.915 ms |
| Type-check average | 5.298 us | 6.016 us | 5.382 us | 5.934 us |
| Type-check maximum observed | 193.500 us | 193.500 us cumulative | <=193.500 us | 193.500 us |
| Callback total | 5.936 ms | 110.896 ms | 15.604 ms | 187.599 ms |
| Callback ms/second | 0.198 | 0.370 | 0.520 | 0.368 |
| Callback maximum observed | 193.600 us | 193.600 us cumulative | 807.300 us | 807.300 us |
| Refresh total | 0 | 0 | 2.104 ms | 2.104 ms |
| Refresh average/maximum | N/A | N/A | 525.925 / 611.300 us | 525.925 / 611.300 us |
| FPS / 1% low | N/A | N/A | N/A | N/A |
| Visible hitch/flicker | not reported | not reported | not reported | not reported |

The full run was dominated by `set_Visible`: 30,879 of 30,993 callbacks. Only 4 callbacks (0.013%) matched a known covering screen. Aggregate callback cost stayed below the task's approximately 1 ms/second non-material threshold in every derived interval, including the overlay interval at approximately 0.520 ms/second. The 807.300 us raw callback maximum occurred only after matched overlay activity and includes the triggered HUD refresh path; the refresh maximum was 611.300 us. No repeated callback or refresh maximum exceeded 1 ms.

Two real tracker transitions produced four matched notifications and four refreshes, demonstrating duplicate delivery and `2.0` refreshes per measured transition. This does not meet the optimization trigger of more than three refreshes for one logical transition, but it is above the ideal one-refresh-per-transition result and should remain visible in the final uncertainty/disposition discussion.

Final B control artifact rebuilt from the post-A4 source:

```text
B output: work/perf/b-final/sts2-party-watch-v2/
B compile property: PartyWatchDisableGlobalVisibilityPatches=true
B DLL SHA256: 73AC1502E601C3E4F0A7F6B4C4B07DDDCE2A6C47ADC7307A422F661A8BDCA9DF
B build: 0 warnings / 0 errors
```

Static inspection confirms B contains neither `NativeCoveringScreenVisibilityPatch` nor the AUD-0007 profiler/prefix. B has not been installed pending explicit approval.

### B installation and A-artifact cleanup

The user approved proceeding with B. With the game confirmed closed, B was installed from `work/perf/b-final/sts2-party-watch-v2/` using `-SkipPublish`. The installed DLL SHA256 matched `73AC1502E601C3E4F0A7F6B4C4B07DDDCE2A6C47ADC7307A422F661A8BDCA9DF`, and exactly one Party Watch manifest remained present. Codex did not launch the game.

At the user's explicit request, the ignored A/A2/A3/A4 publish, bin, obj, NuGet-scratch, and MSBuild-temp directories were removed after every absolute target was verified to remain under the repository `work/` root. Twenty directories were removed. The final B artifact and normal Production publish artifacts remain available. The A4 measurement results, instrumentation method, task documentation, gated source, screenshot reference, and raw dedicated AppData log were retained as evidence.

Current gate: awaiting the user B runtime control, followed by explicit approval to restore Production.

### B runtime result and provisional disposition

The user completed B with the same beta branch and reported no perceptible performance difference from A4. B behavior hid the HUD when the map was open but did not hide it for the other exercised covering screens; this is expected control behavior because the three global visibility postfixes are absent and is not used for correctness acceptance.

Post-run verification:

```text
Installed B SHA256: 73AC1502E601C3E4F0A7F6B4C4B07DDDCE2A6C47ADC7307A422F661A8BDCA9DF
Game log: C:\Users\ROG\AppData\Roaming\SlayTheSpire2\logs\godot.log
Log last write: 2026-07-20 23:59:15
Party Watch load: confirmed
Party Watch errors/exceptions: none found
Average FPS / 1% low: N/A; no in-game measurement facility was available
User A/B performance observation: no perceptible difference
```

Provisional AUD-0007 disposition: **Measured / Non-material; optimization not justified**.

Evidence basis:

- A4 aggregate callback cost was approximately `0.368 ms/second` over 510 seconds and remained below approximately `0.520 ms/second` in the derived overlay interval.
- Type checking averaged `5.934 us` with a `193.500 us` maximum.
- The complete callback maximum was `807.300 us`; it occurred only after matched overlay activity and includes the triggered HUD refresh path, whose maximum was `611.300 us`.
- No measured callback or refresh maximum exceeded `1 ms`, no Party Watch exception occurred, and the user observed no repeatable A/B performance difference or hitch.
- Duplicate delivery is real: two measured tracker transitions produced four matched notifications and four refreshes (`2.0` refreshes/transition). This is below the task's optimization trigger of more than three refreshes for one logical transition and produced no observed performance impact.
- The separate `NCardPileScreen` covering-screen gap remains recorded and must not be conflated with the performance disposition.

### Production restoration and final cleanup

The user explicitly approved restoring Production. With the game confirmed closed, the normal beta Production artifact was reinstalled with `-SkipPublish`. The installed DLL SHA256 was verified as:

```text
3D558648BF9F359172773ECF44381528F34B75368B99F6B9C9FD9A7F4DAA5639
```

Exactly one Party Watch manifest remained installed and Codex did not launch the game. This hash matches the verified Production build and differs from every diagnostic/control artifact.

At the user's explicit request, all remaining B and B-final publish, bin, obj, NuGet-scratch, and MSBuild-temp directories were deleted after each resolved path was verified to remain under the repository `work/` root. Ten B-series top-level test directories were removed. A/A2/A3/A4 had already been removed; a recursive name-boundary check now reports zero remaining A, A2, A3, A4, B, or B-final test directories. These deleted ignored artifacts are not recoverable except by rebuilding.

Final AUD-0007 disposition: **Resolved — measured non-material; no optimization justified**.

- Production behavior is restored and unchanged by default builds.
- The optional instrumentation remains compile-gated and disabled by default so the measurement method can be reused for later features.
- The measured duplicate notifications remain documented as technical redundancy, but not as a material performance issue.
- The separate `NCardPileScreen` draw/discard-pile HUD coverage gap remains documented; it is not part of the performance disposition and no fix was authorized in this task.
- FPS / 1% low remains `N/A`; no in-game measurement facility was available.

The user accepted formal G2 closure on 2026-07-21. This measurement task is closed and requires no further action unless a separate compatibility or optimization gate is opened.
