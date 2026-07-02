# Phase 11｜补充与维护

## 定位

Phase 11 是 Workshop 上传完成后的后续补充 / 维护阶段。

历史上部分后续修补和文档补记仍沿用了 `phase-9*` 文件名，例如 Phase 9B Poison、设置入口修复、本机多人 HUD 和 Workshop 订阅版验证记录。那些文件名保留为历史证据，不再继续扩展新的 Phase 9 子阶段。

## 适用范围

后续新增工作若不属于旧 Phase 的原始收口任务，应登记到 Phase 11，或创建 `phase-11-<topic>.md` 专题补记。

适合放入 Phase 11 的工作：

- 已发布 / 已上传后的 Bug fix。
- 订阅版或安装版运行时补验证。
- Poison、HUD、设置入口、Workshop 描述等既有功能的补充修正。
- 文档同步、证据补记、状态澄清。
- 小范围兼容性补丁。

不适合直接放入 Phase 11 的工作：

- 正式多人 HUD、队友 HUD、共享队伍 HUD。
- 网络状态修改或远程玩家目标猜测。
- 大规模预测引擎重写。

这些需要单独开新阶段或新设计文档，且必须先有可复现证据。

## 当前 Phase 11 入口

- Phase 9B 普通 Poison 专项 Steam 验证矩阵。
- 已上传 / 订阅版状态下的 Tungsten Rod + Beating Remnant 顺序验证。
- 设置入口完整 Steam 导航矩阵。
- 本机血条中线临时开发辅助线的 Steam 运行时确认与后续删除。
- 后续任何新补丁的 task note 命名应优先使用 `phase-11-<topic>.md`。

## 编号规则

- Phase 9：保留为单人正式版收口和历史误编号文件的证据范围。
- Phase 10：Workshop 上传完成后的发布 / 订阅测试里程碑。
- Phase 11：Phase 10 之后的补充与维护。

## 2026-07-02 本机血条中线临时辅助线

Implementation commit: `68d94c0d54672757d17a4799b54f08e14ec91a4e`

No-switch testing commit: `cd8c07e5c9afc35e23c5cf41a7c542d880b5b44d`

Default HUD alignment commit: `8d6467204d25e90bf23141c2b42743ee25e3ed5d`

Text visual center nudge commit: `af52c338fe21ea79411adc2074ea2a7591d5dddd`

Measured text bounds alignment commit: `8b2881d2a3f343a64ae3a44181eaea57aad29649`

HUD text center guide commit: `9d198445d1c3f14e193b73ffafab3542fe23d036`

Shared center target commit: `ef91fcb1a8a1616b4376d7741b2ab2343ac19980`

Alignment runtime coordinate log commit: `0bf9a7e9ee597a1eb756b3432dadea4c7ee7734e`

Vertical local-origin clamp fix commit: `e398089b92f51f41bb5277263f4b0c0399dc7822`

Detail HUD forecast-line alignment commit: `370d631b7076b174b1a9d42b353aadcdb97ff202`

Debug guide removal commit: `070774b70ef07a5ead50f3e82ad60f1a6a3c6c0f`

Implemented:

- Removed the temporary guide switch for the current test build. The guide now draws directly from the existing local HUD path whenever the local HUD number is eligible to show.
- The first local install did not show the guide because the guide was still gated behind an off switch; the current test build has no guide toggle and is intended only for alignment observation.
- Added a single horizontal `ColorRect` guide under the existing local HUD parent when the switch is enabled.
- The guide uses the existing `NHealthBar.HpBarContainer` position and the existing local-player `ForecastRefreshPatch.TryGetLocalCreature` path; it does not add a second local-player or health-bar lookup system.
- The guide is only shown after the normal local HUD number is eligible to show. Non-local bars, hidden HUD state, non-combat state, invalid health bars, unknown / zero forecast output, and disabled local multiplayer HUD all hide the guide.
- Adjusted the default `HealthBarRight` HUD vertical placement so the main HUD label center uses the same local `NHealthBar.HpBarContainer` center line as the guide.
- Added a `-10f` visual-center nudge for the main HUD text after screenshot pixel checks showed the visible label glyphs sat about 10 px below the guide even when the label control rect was centered.
- Replaced the temporary `-10f` nudge with measured HUD text bounds: the main label text is assigned before positioning, `Font.GetStringSize(...)` measures the current HUD string height, and the default right-anchor position centers that measured text box on the health-bar center line.
- Added a second temporary diagnostic guide line over the HUD text: magenta marks the current main HUD label measured-text center, while cyan remains the local health-bar center line.
- Consolidated the default right-anchor HUD target and cyan guide target through the same health-bar center helper. The cyan guide now uses the same `OffsetY` as the HUD target, while its X start remains at the raw health-bar center.
- Moved temporary guide drawing and alignment logging into `PartyWatchHudDebugGuide` so the formal HUD display file keeps only the shared center/text-size helpers and layout calculation.
- Added a throttled `[STS2 Party Watch][HUD Align]` runtime log line after HUD and guide layout. It reports anchor, session offset, raw health-bar center, offset target center, main HUD control center, measured text size, guide centers, local/global rects, and the actual Y deltas.
- Removed the `MathF.Max(0f, position.Y)` clamp from the main HUD label position. The runtime log showed the desired local Y was `-9` (`targetCenter.Y=8`, `mainLabel.Size.Y=34`), but the clamp forced it to `0`, creating `main.deltaY=9` and `guide.deltaY=9`.
- Moved the advanced detail HUD to the right of the main `-N` control by positioning it at `mainLabel.Position.X + mainLabel.Size.X + 12f`.
- The advanced detail HUD now keeps its control center on the same forecast line as the main `-N` label.
- The `[HUD Align]` debug log now includes `detail.visible`, `detail.pos`, `detail.size`, `detail.center`, `detail.deltaY`, and `detail.global` so the advanced detail line can be checked from runtime coordinates.
- Removed the temporary cyan health-bar guide, magenta HUD text guide, `[HUD Align]` runtime log, and `PartyWatchHudDebugGuide` helper after the alignment observation task.
- No settings panel, forecast, poison, power, intent, damage, or multiplayer strategy was changed.

Built:

- `C:\sts2\dotnet\dotnet.exe build .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore`
- Result: success, 0 warnings, 0 errors.
- `C:\sts2\dotnet\dotnet.exe publish .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore`
- Result: success; publish output produced under `src/STS2PartyWatchCode/bin/Release/net9.0/publish/`.
- `C:\sts2\dotnet\dotnet.exe publish .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore`
- Result on 2026-07-03: success; produced a local test build with the temporary guide drawn without a switch.
- `C:\sts2\dotnet\dotnet.exe build .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore`
- Result after default HUD alignment: success, 0 warnings, 0 errors.
- `C:\sts2\dotnet\dotnet.exe build .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore`
- Result after visual-center nudge: success, 0 warnings, 0 errors.
- `C:\sts2\dotnet\dotnet.exe publish .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore`
- Result after visual-center nudge: success.
- `C:\sts2\dotnet\dotnet.exe build .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore`
- Result after measured text bounds alignment: success, 0 warnings, 0 errors.
- `C:\sts2\dotnet\dotnet.exe publish .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore`
- Result after measured text bounds alignment: success.
- `C:\sts2\dotnet\dotnet.exe build .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore`
- Result after HUD text center guide: success, 0 warnings, 0 errors.
- `C:\sts2\dotnet\dotnet.exe publish .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore`
- Result after HUD text center guide: success.
- `C:\sts2\dotnet\dotnet.exe build .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore`
- Result after shared center target: success, 0 warnings, 0 errors.
- `C:\sts2\dotnet\dotnet.exe publish .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore`
- Result after shared center target: success.
- `C:\sts2\dotnet\dotnet.exe build .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore`
- Result after alignment runtime coordinate log: success, 0 warnings, 0 errors.
- `C:\sts2\dotnet\dotnet.exe publish .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore`
- Result after alignment runtime coordinate log: success.
- `C:\sts2\dotnet\dotnet.exe build .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore`
- Result after vertical local-origin clamp fix: success, 0 warnings, 0 errors.
- `C:\sts2\dotnet\dotnet.exe publish .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore`
- Result after vertical local-origin clamp fix: success.
- `C:\sts2\dotnet\dotnet.exe build .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore`
- Result after detail HUD forecast-line alignment: success, 0 warnings, 0 errors.
- `C:\sts2\dotnet\dotnet.exe publish .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore`
- Result after detail HUD forecast-line alignment: success.
- `C:\sts2\dotnet\dotnet.exe build .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore`
- Result after debug guide removal: success, 0 warnings, 0 errors.
- `C:\sts2\dotnet\dotnet.exe publish .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore`
- Result after debug guide removal: success.

Installed:

- Copied `sts2-party-watch-v2.dll` and `sts2-party-watch-v2.json` into `C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\sts2-party-watch-v2` on 2026-07-02.
- Re-copied the no-switch guide test build into the same local game mod directory on 2026-07-03.
- Re-copied the default HUD alignment test build into the same local game mod directory on 2026-07-03.
- Re-copied the visual-center nudge test build into the same local game mod directory on 2026-07-03.
- Re-copied the measured text bounds alignment test build into the same local game mod directory on 2026-07-03.
- Re-copied the HUD text center guide diagnostic build into the same local game mod directory on 2026-07-03.
- Re-copied the shared center target diagnostic build into the same local game mod directory on 2026-07-03.
- Re-copied the alignment runtime coordinate log diagnostic build into the same local game mod directory on 2026-07-03. Installed DLL: `sts2-party-watch-v2.dll`, length `86528`, timestamp `2026-07-03 01:30:39`.
- Re-copied the vertical local-origin clamp fix build into the same local game mod directory on 2026-07-03. Installed DLL: `sts2-party-watch-v2.dll`, length `86528`, timestamp `2026-07-03 01:45:09`.
- Re-copied the detail HUD forecast-line alignment build into the same local game mod directory on 2026-07-03. Installed DLL: `sts2-party-watch-v2.dll`, length `87040`, timestamp `2026-07-03 01:52:15`.
- Re-copied the debug-guide-removal build into the same local game mod directory on 2026-07-03. Installed DLL: `sts2-party-watch-v2.dll`, length `81408`, timestamp `2026-07-03 01:58:19`.
- This was a local game-directory install only, not a Workshop upload.

RuntimeVerified:

- Runtime log captured before the clamp fix showed the cause of the vertical mismatch: `anchor=HealthBarRight`, `offset=(0,0)`, `targetCenter=(-1,8)`, `main.center=(162,17)`, `main.deltaY=9`, `cyanGuide.centerY=8`, `magentaGuide.centerY=17`, and `guide.deltaY=9`.
- Runtime log captured after the clamp fix showed `main.deltaY=0`, `cyanGuide.centerY=8`, `magentaGuide.centerY=8`, and `guide.deltaY=0`, confirming the main `-N` control and guides are on the same runtime line.
- Not run after the detail HUD forecast-line alignment change. No post-detail-change Steam log with advanced details enabled has been recorded yet.
- RuntimeVerified after the debug-guide-removal build: user confirmed the HUD view is OK, and latest `godot.log` inspection found no `HUD Align`, `STS2PartyWatchHealthBarCenterGuide`, `STS2PartyWatchHudTextCenterGuide`, or `PartyWatchHudDebugGuide` hits.

DocumentedOnly:

- This note records the implementation, build, publish, local install boundary, pre-removal runtime coordinate evidence, and final post-removal smoke result.

## 下一步唯一任务

Final closure status: complete for the local debug-guide-removal build. Formal multiplayer HUD remains frozen.
## Current Final Closure Update

Debug guide removal commit `070774b70ef07a5ead50f3e82ad60f1a6a3c6c0f` removed the temporary cyan health-bar guide, magenta HUD text guide, `[HUD Align]` runtime log, and `PartyWatchHudDebugGuide` helper.

The locally installed debug-guide-removal build is RuntimeVerified for local smoke: DLL `sts2-party-watch-v2.dll`, length `81408`, timestamp `2026-07-03 01:58:19`; latest `godot.log` timestamp `2026-07-03 02:12:44` contained no temporary guide or `[HUD Align]` noise. DLL SHA256: `A7E87950FDF19C8BE3985894F988B009CDA188BB341733EC27C415F5E5B4A02D`. JSON SHA256: `F93CED0594EC8EE7D49A4DF616BB4373251F08B12FFAC730B68262A2D567ED4C`. Formal multiplayer HUD remains frozen.
