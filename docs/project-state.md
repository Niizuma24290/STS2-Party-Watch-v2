# Project State

Last reconciled: 2026-07-03

## Positioning

STS2 Party Watch v2 is a read-only Slay the Spire 2 combat HUD mod. Its primary supported surface is the local player's forecast HUD. The default display is a single total expected HP loss value:

```text
-N
```

The total is display-only composition of trusted forecast fields:

```text
N = ForecastResult.OutDamage + ForecastResult.DirectHpLoss
```

Party Watch does not simulate a full turn, does not call real damage or command execution paths, and does not modify combat state, saves, network state, cards, powers, relics, or room state.

## Current HUD Behavior

- Default HUD line: total expected HP loss `-N`.
- Advanced details: optional `🛡 N` and `♥ N`, disabled by default.
- `🛡 N`: trusted final blockable HP loss after verified block and supported HP-loss result modifiers.
- `♥ N`: trusted direct HP loss that does not go through Block.
- Unknown, unsupported, non-combat, zero-output, hidden UI, or untrusted partial values are hidden rather than guessed.
- HUD refreshes from health-bar lifecycle hooks, hand pile changes, relic add/remove/melt, selected turn lifecycle hooks, and settings changes.
- `FreezeHudWithinPlayerTurn` is enabled by default. It freezes a display snapshot within the player turn and commits a final snapshot at `Hook.BeforeTurnEnd`; it does not alter forecast mechanics.
- Covering native pages, modal/popup/overlay screens, combat end, invalid player state, or disabled HUD settings hide and clear relevant display state.
- The temporary local health-bar center guide, HUD text center guide, and alignment runtime log have been removed after the alignment observation task.
- Default `HealthBarRight` HUD placement now centers the main HUD label on the same local health-bar center line used by the temporary guide. User X/Y offsets still apply after the default position is calculated.
- The default right-anchor HUD alignment now assigns the current HUD string before positioning, measures that string with `Font.GetStringSize(...)`, and centers the measured text bounds on the health-bar center line instead of centering a fixed empty label rect.
- The default right-anchor HUD target was verified against the temporary cyan guide using the same health-bar center helper while the diagnostic build was active.
- The temporary `[STS2 Party Watch][HUD Align]` runtime log was used for alignment diagnosis and has been removed.
- Runtime coordinate logs showed the vertical mismatch was caused by clamping the main HUD label's local Y position to zero. The local health-bar target center was `Y=8`, the measured main label height was `34`, and the desired label top was `-9`; clamping it to `0` produced `main.deltaY=9`. The local Y clamp has been removed for the main label.
- Post-clamp runtime logs showed `main.deltaY=0` and `guide.deltaY=0`, confirming the main `-N` control and temporary guides are on the same runtime line.
- The advanced detail HUD is now placed to the right of the main `-N` control and centered on the same forecast line. The temporary diagnostic log included detail position, center, and delta fields before the debug instrumentation was removed.

## Supported Mechanics

Status tags:

- `Implemented`: current code exists.
- `RuntimeVerified`: Steam, Workshop subscription, or task note runtime evidence exists.
- `Conditional`: implemented only under documented constraints.
- `Unsupported`: deliberately hidden or not implemented.

| Mechanism | Current status | Evidence boundary |
| --- | --- | --- |
| Enemy `AttackIntent` / DeathBlow intent damage | Implemented, RuntimeVerified | Uses native `AttackIntent.GetTotalDamage(...)`; ordinary attack scenes verified. |
| Hand turn-end blockable `DamageVar`, including Burn-style cards | Implemented, RuntimeVerified | Reads current local hand and previews `DamageVar` through native `Hook.ModifyDamage(...)`; Burn matrix verified. |
| Frost | Implemented, RuntimeVerified | Reads `FrostOrb.PassiveVal` as verified pre-attack Block. |
| PlatingPower | Implemented, RuntimeVerified | Reads current local `PlatingPower.Amount`. |
| Orichalcum / FakeOrichalcum | Implemented, RuntimeVerified, Conditional | Counted only when current Block is 0. |
| RippleBasin | Implemented, RuntimeVerified, Conditional | Handles already-played Attack and pending `StampedePower` attack exclusion. |
| CloakClasp | Implemented, RuntimeVerified | Reads current hand count and relic Block var. |
| Beckon / Bad Luck / Regret | Implemented, RuntimeVerified | Verified direct HP loss sources for `♥`. |
| ConstrictPower / DisintegrationPower self-damage | Implemented, RuntimeVerified | Treated as blockable turn-end Power damage based on code evidence and user Steam verification. |
| IntangiblePower on local player direct HP loss | Implemented, RuntimeVerified, Conditional | Applies to verified single HP-loss events before Tungsten Rod and Beating Remnant. Aggregate direct HP loss remains unsupported. |
| TungstenRod | Implemented, Conditional | Applies only to verified single HP-loss events. Aggregate enemy HP loss with Tungsten Rod returns Unknown. |
| BeatingRemnant | Implemented, Conditional | Uses observed current-window HP loss budget and verified event order. Tungsten Rod is applied before Beating Remnant in Party Watch forecast. |
| DiamondDiademPower / Diamond Diadem | Implemented, RuntimeVerified, Conditional | Existing Power trusts native intent preview. Future Power is forecast only when card-count and per-hit conditions are readable. |
| Ordinary Poison pre-action enemy survival preview | Implemented, Conditional | Code integrated for ordinary enemies with readable `PoisonPower` and `AccelerantPower`; build/publish evidence exists, but the专项 Steam matrix is not fully backfilled. |
| Local HUD in multiplayer battles | Implemented, RuntimeVerified, Conditional | Workshop subscription runtime screenshot confirmed local player's `-6` HUD under the local character health bar. This is still local-player HUD, not teammate HUD. |
| Formal multiplayer HUD, teammate HUD, shared party HUD | Unsupported | Explicitly frozen. No target-aware multiplayer forecast is claimed. |

## Phase 9B Poison Total Record

Current implementation:

- `EnemyPreActionSurvivalPreview` / `PoisonTickPreview` run inside `LocalIncomingDamageReader` before an enemy instance's current `AttackIntent` is added to the forecast.
- Enemy identity is per native instance: prefer `Creature.CombatId`, otherwise native object reference plus same-snapshot enemy index. Enemy display name is not used as a key.
- Supported ordinary path reads current `PoisonPower.Amount`, current enemy HP, current intent contribution, and opponent `AccelerantPower.Amount`.
- `AccelerantPower` is read only if it is already active at the forecast moment. Party Watch does not replay `Accelerant` cards or command queues.
- If ordinary Poison is predicted to kill that enemy before its action, that enemy instance's current Attack Intent is excluded from final `-N`.
- The preview does not execute real Poison, damage, death, removal, enemy action, RNG, save, or network behavior.

Native poison-bar evidence recorded from the final task notes:

- Native `PoisonPower.CalculateTotalDamageNextTurn()` is the game's model-side total poison preview used by the native poison health-bar foreground.
- Native trigger count is `min(PoisonPower.Amount, 1 + alive opponents' AccelerantPower amount sum)`.
- Native poison preview calls `Hook.ModifyDamage(...)` per tick, so it can reflect damage-cap style powers such as `HardToKillPower` or damage-cap side of `IntangiblePower`.
- Native `NHealthBar.IsPoisonLethal(...)` and `_poisonForeground` are UI preview signals only. They do not prove enemy Intent cancellation, death lifecycle, revive, phase behavior, or HP-loss budget behavior.

Current unsupported / research-only Poison boundaries:

- `HardToKillPower` / Exoskeleton: native poison bar evidence is documented, but Party Watch does not yet use `CalculateTotalDamageNextTurn()` for this case.
- `SlipperyPower`: when active, Poison survival preview must not remove that enemy's Intent; the enemy should keep contributing until a supported rule is implemented.
- Enemy `IntangiblePower`: poison damage cap / HP-loss cap interaction is documented but not yet verified for enemy pre-action survival.
- `TestSubject` / `AdaptablePower`: phase, revive, and current Intent cancellation timing are not yet implemented.
- `ToughEgg` / `HatchPower`: hatch lifecycle remains unsupported.
- `SewerClam` / `HardenedShellPower`: per-turn HP-loss budget and reset timing remain unsupported.

Verification state:

- Implemented: ordinary Poison pre-action survival code path exists.
- Built / publish evidenced: recorded in Phase 9B task notes.
- RuntimeVerified: not yet fully backfilled for the ordinary Poison matrix.
- DocumentedOnly: native poison bar details and special enemy strategy notes.

## Front-End Capabilities

- HUD label is anchored to the local player's `NHealthBar.HpBarContainer` parent or equivalent health-bar node parent.
- Main label and detail label are separate nodes.
- The temporary health-bar center guide and HUD text guide were removed after the alignment observation task. The remaining HUD labels continue to use the local health-bar lifecycle and parent node.
- Native covering screen lifecycle is tracked by `PartyWatchNativeCoveringScreenTracker` and `NativeCoveringScreenLifecyclePatch`.
- Settings live in `PartyWatchUiSettings` for the current game session.
- Settings include HUD enabled, local HUD in multiplayer, advanced details, freeze behavior, anchor preset, X/Y offset, total color, shield detail color, heart detail color, and restore defaults.
- Current settings are session-only. No verified official per-mod persistence API is used, and Party Watch does not write guessed settings files or game saves.
- The settings entry no longer occupies the native Settings or Modding entry. It appears only inside the native Modding screen's Party Watch mod-info panel by patching `NModInfoContainer.Fill(Mod)` for `sts2-party-watch-v2`.

## Multiplayer Definition

`LocalSinglePlayerHud` is the current HUD product surface. It can be shown in single-player combat and, when `ShowLocalHudInMultiplayer` is true, in multiplayer combat for the local player only.

Current multiplayer behavior:

- Shows one local HUD anchored to the local player's health bar when the local forecast is trusted.
- Does not display remote teammate forecasts.
- Does not create a party/shared HUD.
- Does not send network messages.
- Does not modify multiplayer state.
- Hides numeric output when the local target or damage cannot be trusted.

`FormalMultiplayerHud` remains frozen future work.

## Known Limitations

- Ordinary Poison pre-action survival preview still needs a dedicated Steam runtime matrix for ordinary Poison, multiple enemies, same-name enemies, and unsupported special enemies.
- `HardToKillPower` / Exoskeleton Poison interaction is documented from native poison-bar evidence but not implemented in Party Watch.
- `SlipperyPower`, enemy `IntangiblePower`, `TestSubject` / `AdaptablePower`, `ToughEgg` / `HatchPower`, and `SewerClam` / `HardenedShellPower` Poison survival cases are unsupported or research-only as listed in the Phase 9B Poison total record.
- Tungsten Rod with aggregate enemy HP loss remains unsupported because per-hit or per-event granularity is required.
- Diamond Diadem aggregate enemy damage with per-hit rounding unknown remains unsupported.
- Settings persistence is session-only.
- The health-bar center guide, HUD text center guide, `[HUD Align]` runtime log, and `PartyWatchHudDebugGuide` helper were removed in commit `070774b70ef07a5ead50f3e82ad60f1a6a3c6c0f`, Built and locally installed. The main `-N` alignment is RuntimeVerified by post-clamp logs. Post-removal smoke verification passed: user confirmed the HUD view is OK, and latest log inspection found no temporary guide or `[HUD Align]` debug noise. Installed DLL SHA256: `A7E87950FDF19C8BE3985894F988B009CDA188BB341733EC27C415F5E5B4A02D`.
- Workshop state must not be described as a public release unless a public publish is explicitly recorded.

## Release State

- Branch: `main`.
- Remote: `https://github.com/Niizuma24290/STS2-Party-Watch-v2.git`.
- GitHub: local `main` currently contains commits ahead of `origin/main` before this reconciliation is pushed.
- Workshop workspace: prepared under ignored `work/`.
- Workshop item status from task notes: private / subscription test work exists, including item `3755598583`, uploaded DLL hash, cover/preview upload records, and local multiplayer HUD subscription validation. The repository state is not documented as a public Workshop release.
- Not committed by policy: DLL, PDB, PCK, logs, `bin/`, `obj/`, `publish/`, `work/`, uploader files, cover assets, `mod_id.txt`, and game directory files.

## Phase Numbering

- Phase 9: historical single-player release cleanup and legacy `phase-9*` notes.
- Phase 10: Workshop upload / private subscription-test milestone.
- Phase 11: all new supplements, maintenance fixes, runtime verification backfills, and documentation updates after Workshop upload.
- Do not create new Phase 9 subphases. Use `docs/task-notes/phase-11-supplements.md` or `phase-11-<topic>.md`.

## Next Single Task

Next Phase 11 supplement can proceed. Keep formal multiplayer HUD work frozen.
