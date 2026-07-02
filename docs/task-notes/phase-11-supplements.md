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

Implemented:

- Removed the temporary guide switch for the current test build. The guide now draws directly from the existing local HUD path whenever the local HUD number is eligible to show.
- The first local install did not show the guide because the guide was still gated behind an off switch; the current test build has no guide toggle and is intended only for alignment observation.
- Added a single horizontal `ColorRect` guide under the existing local HUD parent when the switch is enabled.
- The guide uses the existing `NHealthBar.HpBarContainer` position and the existing local-player `ForecastRefreshPatch.TryGetLocalCreature` path; it does not add a second local-player or health-bar lookup system.
- The guide is only shown after the normal local HUD number is eligible to show. Non-local bars, hidden HUD state, non-combat state, invalid health bars, unknown / zero forecast output, and disabled local multiplayer HUD all hide the guide.
- Adjusted the default `HealthBarRight` HUD vertical placement so the main HUD label center uses the same local `NHealthBar.HpBarContainer` center line as the guide.
- Added a `-10f` visual-center nudge for the main HUD text after screenshot pixel checks showed the visible label glyphs sat about 10 px below the guide even when the label control rect was centered.
- Replaced the temporary `-10f` nudge with measured HUD text bounds: the main label text is assigned before positioning, `Font.GetStringSize(...)` measures the current HUD string height, and the default right-anchor position centers that measured text box on the health-bar center line.
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

Installed:

- Copied `sts2-party-watch-v2.dll` and `sts2-party-watch-v2.json` into `C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\sts2-party-watch-v2` on 2026-07-02.
- Re-copied the no-switch guide test build into the same local game mod directory on 2026-07-03.
- Re-copied the default HUD alignment test build into the same local game mod directory on 2026-07-03.
- Re-copied the visual-center nudge test build into the same local game mod directory on 2026-07-03.
- Re-copied the measured text bounds alignment test build into the same local game mod directory on 2026-07-03.
- This was a local game-directory install only, not a Workshop upload.

RuntimeVerified:

- Not run after the measured text bounds alignment. The provided screenshot confirmed the guide was visible and showed the text glyphs about 10 px below the guide before this measured-bounds change, but no post-change Steam screenshot has been recorded yet.

DocumentedOnly:

- This note records the implementation, build, publish, and local install boundary only. It must not be treated as runtime alignment verification.

## 下一步唯一任务

进入 Steam 运行时临时打开本机血条中线辅助线，确认单人和本机多人 HUD 对齐观察结果；确认完成后删除该临时辅助线。正式多人 HUD 仍冻结。
