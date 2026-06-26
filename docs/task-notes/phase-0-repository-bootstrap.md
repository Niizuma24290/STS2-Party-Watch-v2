# Phase 0 Repository Bootstrap

## Goal

Create a clean, independent v2 repository with frozen scope, architecture rules, and an implementation roadmap.

## Files created

- README.md.
- .gitignore.
- docs/project-state.md.
- docs/architecture.md.
- docs/decisions.md.
- docs/mechanics-evidence.md.
- docs/interface-map.md.
- docs/compatibility.md.
- docs/build-environment.md.
- docs/v2-roadmap.md.
- docs/task-notes/phase-0-repository-bootstrap.md.
- src/STS2PartyWatchCode/.gitkeep.
- tools/.gitkeep.

## What was intentionally not created

- Gameplay code.
- HUD code.
- C# project files.
- Solution files.
- Godot project files.
- Build artifacts.
- NuGet.Config.
- .local-nuget.
- local.props.
- local package folders.
- work/.gitkeep.

## Architecture rules frozen in this phase

- Combat reader => forecast calculator => result => HUD view.
- No HUD code may call GetTotalDamage.
- No combat reader may access Godot UI nodes.
- No patch may calculate RAW or OUT.
- No module may depend on Minty.
- No v2.0 module may read remote player state.
- No temporary diagnostics code enters production modules without a separate scoped task.

## Exact next task

Phase 1 - Empty Mod load proof.
