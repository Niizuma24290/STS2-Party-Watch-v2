# Workshop upload preparation - 2026-07-01

## Scope

- 本轮只做 Steam Workshop 私密上传测试准备与 Git hygiene 记录。
- 不修改预测机制、HUD UI 代码、README、发布内容或 Git 跟踪规则。
- 不执行 Workshop 上传器。
- 不执行 public 发布。

## Current Workshop State

- Public release status: not public.
- This Git sync does not run `ModUploader.exe`.
- Workshop title, tags, cover, and private upload test still need final preparation.
- Any uploader state files under `work/` stay outside Git and are not release artifacts.

## Uploader

- Source: Mega Crit official `sts2-mod-uploader` release.
- Release: `v0.2.0`
- `ModUploader.exe`: `C:\Users\ROG\Documents\Codex\STS2-Party-Watch-v2\work\tools\sts2-mod-uploader-v0.2.0\extracted\ModUploader.exe`
- Uploader log: `C:\Users\ROG\Documents\Codex\STS2-Party-Watch-v2\work\tools\sts2-mod-uploader-v0.2.0\extracted\mod-uploader.log`

## Workshop Workspace

- Workspace: `C:\Users\ROG\Documents\Codex\STS2-Party-Watch-v2\work\workshop-upload-rc-20260701`
- Content files:
  - `content\sts2-party-watch-v2.dll`
  - `content\sts2-party-watch-v2.json`
  - `image.png`
  - `workshop.json`
  - `mod_id.txt`
- These files are local upload workspace files only.
- `image.png`, previews, uploader binaries, uploader logs, DLLs, and `mod_id.txt` must stay ignored.

## Future Private Upload Test Command

```powershell
cd "C:\Users\ROG\Documents\Codex\STS2-Party-Watch-v2\work\tools\sts2-mod-uploader-v0.2.0\extracted"
.\ModUploader.exe upload -w "C:\Users\ROG\Documents\Codex\STS2-Party-Watch-v2\work\workshop-upload-rc-20260701"
```

Do not run this command during Git synchronization.

## Clean Environment Preparation

- Before private upload testing, ensure manual local installs do not mask the Workshop subscription.
- If a manual install backup is needed, keep it under ignored `work/`.

## Git And Artifact Hygiene

- `work/` remains ignored.
- DLL, manifest copies, cover image, previews, `mod_id.txt`, uploader zip/exe, and uploader log remain outside Git tracking.
- Do not stage `work/`, `publish/`, `bin/`, `obj/`, logs, DLLs, PDBs, PCKs, uploader files, or game install directory files.

## Pending Private Upload Test

1. Prepare final cover image.
2. Prepare Workshop tags.
3. Run a private upload test only when explicitly requested.
4. Subscribe to the private Workshop item and wait for Steam to download it.
5. Launch Slay the Spire 2 from Steam.
6. Confirm the HUD is loaded from the Workshop subscription, not from a manual `mods` directory.
7. Verify:
   - Default total incoming HP loss `-N`.
   - Advanced `🛡 / ♥` details.
   - Settings page, position, and colors.
   - HUD hides under covering screens.
   - In-turn display behavior.
   - HUD cleanup after combat ends.
   - Tungsten Rod, Beating Remnant, and Diamond Diadem cases.

## Next Step

准备 Workshop 封面、tags 与私密上传测试。

## Actual Workshop Update Notes

- Current Workshop item: `3755598583`
- Current title: `Damage Forecast / 伤害预测`
- Current visibility remains `private`.
- Description now includes first-person future multiplayer plans, including top-left party HP bar forecasts, teammate warnings before turn resolution, SL context, and the Bad Luck / 13 HP joke.
- Description ends with one `GitHub` link:
  - `[Niizuma24290/STS2-Party-Watch-v2](https://github.com/Niizuma24290/STS2-Party-Watch-v2)`
- Description was restructured to lead with the player-facing hook and split `Current features` / `Known gaps` / `Future plans`.
- Removed the limiting words `several` and `部分` from the relic / power modifier feature line.
- Uploaded commit `ed50d7b` (`fix: reuse local HUD forecast in multiplayer`) to the same Workshop item `3755598583`.
- The uploaded DLL hash matched the freshly published DLL hash: `FD89D7F7EE28DD9F7A0C116E6CD33780BD08DE01AC1EA4460B80CAF227D1C5E4`.
- `workshop.json` change note for this update: `Reuse local HUD forecast in multiplayer.`
- The Workshop description now says the current release supports the local player's HUD in multiplayer battles, while teammate top-left party HP forecasts remain a future plan.
- Multiplayer runtime screenshot confirmed the local player's `-6` Party Watch HUD appears under the local character health bar. The teammate row's blue shield value in the top-left party UI is treated as native multiplayer Block/status display, not Party Watch teammate prediction.
- Teammate top-left forecasts, teammate HUD, and shared party HUD remain frozen future `FormalMultiplayerHud` work.
- Replaced the main Workshop cover image with a new user-provided damage forecast diagram, resized to `900x900` PNG (`957,915` bytes), then uploaded to the same item.
- Two user-provided gameplay screenshots were compressed and uploaded as Workshop preview images:
  - `previews\forecast-total-example.jpg`
  - `previews\forecast-details-example.jpg`
