# Phase 11C | Poison runtime verification and safe special-power expansion

## Scope

User explicitly reopened Poison pre-action survival work as a Phase 11 follow-up. This note records the first narrow closure:

- Implement `HardToKillPower` / Exoskeleton support for enemy Poison pre-action survival.
- Keep the existing per-enemy instance identity behavior.
- Do not simulate turns, execute Poison, replay cards, or read UI poison-bar controls as data.
- Do not change formal multiplayer HUD, network behavior, saves, combat state, BaseLib settings, or game files.

User also changed the special-enemy product rule: unsupported Poison-sensitive enemy mechanics should not make the whole `-N` forecast Unknown. Instead, Party Watch should disable only that enemy instance's Poison survival override and keep its already-read base Attack Intent.

## Code Map Confirmed

Actual current path:

```text
LocalIncomingDamageReader.ReadKnown(...)
-> per enemy AttackIntent.GetTotalDamage(...)
-> EnemyPreActionSurvivalPreview.Preview(...)
-> PoisonTickPreview.Preview(...)
-> include or skip that enemy instance's current AttackIntent
-> ForecastResult via LocalDamageForecast
-> PartyWatchHudSnapshotStore / HUD rendering
```

Key files:

- `src/STS2PartyWatchCode/Combat/EnemyPreActionSurvivalPreview.cs`
- `src/STS2PartyWatchCode/Combat/LocalIncomingDamageReader.cs`
- `src/STS2PartyWatchCode/Combat/HookDamageCompat.cs`
- `src/STS2PartyWatchCode/Patches/ForecastRefreshPatch.cs`

## Implemented

- `PoisonTickPreview` now uses native `PoisonPower.CalculateTotalDamageNextTurn()` when available.
- `HardToKillPower` no longer blocks Poison pre-action survival preview.
- If native poison preview says total Poison damage is at least current HP, that enemy instance's current Attack Intent is excluded from `-N`.
- If native poison preview is unavailable on a `HardToKillPower` enemy, Party Watch conservatively keeps that enemy's Intent rather than using ordinary hand-rolled Poison damage.
- `SlipperyPower` now has a narrow read-only preview: each active Slippery layer caps one incoming Poison tick to 1 HP loss and is consumed for the next tick.
- `HardenedShellPower` / SewerClam now has a narrow read-only budget preview: Party Watch reads `DisplayAmount` as remaining HP-loss budget, caps Poison HP loss to that remaining budget, and only removes the current Intent when the remaining budget is at least current HP and Poison can spend enough of that budget to kill before action.
- enemy `IntangiblePower` narrow Poison preview was restored on 2026-07-18 after the v0.109 Diamond Diadem compatibility baseline proved the earlier damage-display failure was unrelated. Without HardToKill, Slippery, or HardenedShell combinations, Party Watch treats each Intangible Poison tick as at most 1 HP loss and removes the current Intent only when the readable trigger count reaches current HP.

## Build / Install

- Build: passed with `C:\sts2\dotnet\dotnet.exe build .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore`.
- Publish: passed with `C:\sts2\dotnet\dotnet.exe publish .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore`.
- Local install: completed through `scripts\Install-LocalMod.ps1`.
- Installed artifact: `C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\sts2-party-watch-v2\sts2-party-watch-v2.dll`.
- Installed DLL SHA256 after HardToKill install: `B6B44584738075689C93AAFD9BFFFC727B4709B82C9723D86B36AFC15BF3A8A9`.
- Installed DLL SHA256 after Slippery implementation install: `0D12DD91EFCEE9787B14A65D5ECF50A65616F2EF9025EBC9883F60EA8FB6DE3B`.
- Installed DLL SHA256 after revive/phase lifecycle gate update: `F22395FE2B3DB496EF1E9D06B70406DB4EC61C5A1FB94D2F4FAB40EDAB150319`.
- Installed DLL SHA256 after HardenedShell / SewerClam budget support install: `959D6ADD30B753C8518BD138C5E4CB66EA79E998DF39107CCB1D2B4423519A11`.
- Installed DLL SHA256 after narrow enemy Intangible support install, before rollback: `815A1A4418B38EF8954C4C6C89EEF7F875ECB98B6039636847F02E7EB4AD26CC`.
- Installed DLL SHA256 after enemy Intangible rollback and v0.109 Diamond Diadem skip: `5403E89242FBBC5D0D3565D88FE9FA53C727FB435CF491D719B672D986B24B41`.
- Installed DLL SHA256 after the 2026-07-18 enemy Intangible restoration on the v0.109 compatibility baseline: `B8E5ACB84580BCE901CA51623522B86560CFF178A07CD0F33619E1E49FAB7E8A`.
- Source commit: not committed yet; base before this working-tree change was `4febf61 docs: record HUD cleanup smoke success`.

## Runtime Results

### H-01 / H-02 / H-03: Exoskeleton / HardToKillPower

User supplied Steam runtime observations on 2026-07-04:

- Before this implementation, a scene with Exoskeleton, Poison, and Attack Intent could hide `-N` entirely because HardToKill was treated as unsupported.
- Native poison bar evidence:
  - 54 Poison with no extra Poison trigger previewed only 9 damage against 26 HP, matching `HardToKillPower(9)`.
  - After the user applied an extra trigger source, the enemy died because three capped Poison ticks produced 27 total damage against 26 HP.
- After installing this build, user confirmed Exoskeleton works, including in a scene with relic and curse interactions.
- Screenshot reference provided by user: `C:\Users\ROG\AppData\Local\Temp\codex-clipboard-a1d752d4-e997-4de4-9e06-d618aecac970.png`.
- Observed HUD: `-11` remained visible in the Exoskeleton / Poison scene with other forecast modifiers active.

RuntimeVerified:

- `HardToKillPower` / Exoskeleton no longer hides `-N`.
- Party Watch respects native capped Poison preview for Exoskeleton.
- Existing relic / curse forecast composition was not broken by the HardToKill Poison path in the user-provided scene.

## Slippery / 墨宝

User clarified the runtime rule from a 墨宝 scene:

- Slippery behaves like a count-based Intangible for incoming damage.
- Each damage event is capped to 1 while Slippery layers remain.
- Receiving a damage event consumes one Slippery layer.
- Later Poison ticks in the same enemy turn can deal normal damage after Slippery is exhausted.

Implemented rule:

```text
for each Poison tick:
  preview damage through read-only Hook.ModifyDamage
  if Slippery layers remain and damage > 0:
    HP loss = 1
    consume one local preview Slippery layer
  else:
    HP loss = preview damage
```

The motivating scene from user screenshot:

```text
middle 墨宝: 16 HP, 30 Poison, SlipperyPower(1), current Intent 3x3
player: one active extra Poison trigger
expected Poison HP loss before enemy action: 1 + 29 = 30
expected Party Watch result after fix: remove the middle enemy's 9 Intent; keep only the two 4-damage enemies, total -8
```

Runtime status: RuntimeVerified by user screenshot on 2026-07-05.

User supplied Steam runtime observation:

- Middle 墨宝 had active Slippery and Poison and a `3x3` current Attack Intent.
- Before the Slippery fix, HUD showed `-17`, keeping all three enemies' attacks.
- After the Slippery tick/layer preview install, HUD showed `-8`, keeping only the left and right 4-damage enemies.
- Screenshot reference provided by user: `C:\Users\ROG\AppData\Local\Temp\codex-clipboard-52abe9dc-75a7-4a2d-a634-989bf4e6c911.png`.

RuntimeVerified:

- `SlipperyPower` no longer blindly keeps the poisoned enemy's current Intent.
- Per-tick Slippery layer consumption can remove the current Attack Intent when Poison will kill before action.
- The HUD remains visible and does not fall back to Unknown/hidden for this supported Slippery scene.

Revive / phase / hatch lifecycle update:

- User confirmed that revive or phase-change enemies do not immediately attack after death, revive, or phase transition.
- Based on that product/runtime rule, `TestSubject` / `AdaptablePower`, `NemesisPower`, and `HatchPower` / ToughEgg no longer block Poison survival override.
- These enemies may now remove their current Intent when supported Poison preview reaches current HP.
- TestSubject phase 3 is RuntimeVerified for the representative exact-lethal Intangible + Poison boundary recorded below. Nemesis and ToughEgg remain without separate family-specific runtime captures.

## HardenedShell / SewerClam / 鬼祟珊瑚群

User clarified the desired runtime rule:

- SewerClam / 鬼祟珊瑚群 has a finite HP-loss budget through `HardenedShellPower`.
- Poison should participate in current Intent removal only when the remaining HP-loss budget can be read and is large enough to allow the enemy's current HP to be lost before action.
- If the remaining budget cannot be read, or if the remaining budget is smaller than current HP, Party Watch keeps base Intent instead of guessing a kill.

Implemented rule:

```text
remainingBudget = HardenedShellPower.DisplayAmount
if remainingBudget < current HP:
  keep current Intent
else:
  poisonHpLoss = min(previewed Poison damage, remainingBudget)
  remove current Intent only when poisonHpLoss >= current HP
```

Implementation details:

- Party Watch reads `DisplayAmount` only; it does not fall back to `Amount`, because `Amount` can represent the total budget rather than the remaining budget.
- If native `PoisonPower.CalculateTotalDamageNextTurn()` is available, the HardenedShell path uses that native total and then caps it by the remaining budget.
- If native Poison preview is unavailable on a `HardToKillPower` combination, Party Watch keeps base Intent.
- Ordinary fallback Poison preview is also capped by the remaining HardenedShell budget.

Runtime status: RuntimeVerified by user report on 2026-07-17.

## Enemy Intangible / TestSubject phase 3

User clarified the desired narrow rule:

- Enemy Intangible makes each Poison tick deal at most 1 HP loss.
- This support is low value and should stay narrow.
- Only remove current Intent when the readable Poison trigger count can cover current HP.

Active rule restored on 2026-07-18:

```text
triggerCount = min(current Poison, 1 + alive opponents' AccelerantPower amount sum)
if current HP <= triggerCount:
  remove current Intent
else:
  keep current Intent
```

Active code shape:

```csharp
if (state.NativeEnemy.GetPower<IntangiblePower>()?.Amount > 0
    && TryPreviewIntangiblePoisonDamage(state, out var intangiblePreviewedDamage))
{
    return PoisonTickPreviewResult.CreateSupported(
        intangiblePreviewedDamage,
        willRemainAliveBeforeAction: intangiblePreviewedDamage < state.CurrentHp);
}

private static bool TryPreviewIntangiblePoisonDamage(
    EnemyPreActionState state,
    out int previewedHpLoss)
{
    previewedHpLoss = 0;
    if (state.NativeEnemy.GetPower<HardToKillPower>()?.Amount > 0
        || state.NativeEnemy.GetPower<SlipperyPower>()?.Amount > 0
        || HasHardenedShellBudget(state.NativeEnemy))
    {
        return true;
    }

    var triggers = PreviewPoisonTriggerCount(state.CurrentPoison, state.OpponentAccelerant);
    previewedHpLoss = Math.Min(Math.Max(0, state.CurrentHp), triggers);
    return true;
}
```

Restoration and runtime result:

- The no-Poison v0.109 baseline was confirmed by the user before restoration: TestSubject phase 3 retained and displayed its native attack correctly, so the earlier hidden-damage failure was not reproduced on the capability-routed Diamond Diadem build.
- Release build and publish passed with 0 warnings and 0 errors; `git diff --check` passed apart from existing line-ending notices.
- Published and installed DLL SHA256 both equal `B8E5ACB84580BCE901CA51623522B86560CFF178A07CD0F33619E1E49FAB7E8A`.
- User Steam verification on 2026-07-18 captured TestSubject phase 3 at 9 HP with 20 Poison, Intangible(1), opponent Accelerant(8), and native 45 Attack Intent. The readable trigger count was exactly `min(20, 1 + 8) = 9`, so Party Watch removed that 45 contribution at the equality boundary.
- The user reported the runtime result successful. This combines with the already-confirmed TestSubject revive / phase behavior that the old attack does not execute immediately after Poison death and transition.
- Preserved screenshot: `docs/task-notes/assets/phase-11c-testsubject-intangible-tsi-04-2026-07-18.png`; SHA256 `900852D903DE607AE69ECEAF039C48C8CE89DCEB2A4417358BE79B7EE22586F4`.
- No fresh `godot.log` was found under the previously known app-userdata paths, matching the prior v0.108 smoke limitation. Runtime evidence for this closure is therefore the user visual confirmation plus the preserved screenshot, not a fresh log claim.
- The exact-lethal TSI-04 boundary and TestSubject phase-3 lifecycle outcome are RuntimeVerified. The full TSI-02/03/05/07/08 matrix was not separately captured and must not be described as individually verified.

Runtime status: `Implemented / Built / Installed / RuntimeVerified (representative exact-lethal TestSubject phase-3 boundary)`.

## Success Pattern

1. Read base Attack Intent per native enemy instance first.
2. Apply Poison only as an adjustment to that specific enemy instance's current Intent.
3. Use native `PoisonPower.CalculateTotalDamageNextTurn()` as the total Poison damage source for supported damage-cap cases.
4. For supported special HP-loss mechanics such as Slippery, HardenedShell, and enemy Intangible, add only narrow read-only previews with clear fallback-to-keep behavior.
5. For unsupported HP-loss / lifecycle mechanics, disable the Poison override and keep base Intent rather than hiding `-N` or guessing removal.
6. Treat UI poison bar screenshots as corroborating runtime observation, not as Party Watch's data source.

## Next Single Task

Phase 11C enemy Intangible / TestSubject phase-3 restoration is closed. Do not reopen it without a new contradictory runtime case; any future work should be a separately scoped matrix expansion or a different project task.
