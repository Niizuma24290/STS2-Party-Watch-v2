# Phase 9A｜前端 UI、设置与显示生命周期

## 状态

代码接入、构建、发布、本地安装已完成；Steam 运行时验证等待用户执行。

## 实际改动

- `ForecastRefreshPatch` 改为只协调 HUD 生命周期：读取已验证预测结果、交给显示快照、可见性策略、布局样式模块处理。
- 新增 `PartyWatchHudVisibilityPolicy`：集中判断 HUD 是否应该显示。
- 新增 `PartyWatchHudSnapshotStore`：实现默认开启的 `FreezeHudWithinPlayerTurn` 显示层快照。
- 新增 `PartyWatchUiSettings`：集中管理 HUD 启用、高级明细、冻结、位置、颜色等设置。
- 新增 `PartyWatchHudDisplay`：集中生成 HUD 文本、应用样式和位置。
- 新增 `PartyWatchSettingsPatch`：在原生 `NSettingsScreen` 注入 Party Watch HUD 设置面板。

## HUD 层级与隐藏规则

- HUD 仍挂在本机玩家 `NHealthBar.HpBarContainer` 的父节点下，而不是全局最顶层。
- `ShouldRenderHud()` 至少要求：HUD 启用、血条节点有效、`ICombatState.IsLiveCombat()`、单人战斗、本机玩家、玩家存活、血条容器可见。
- UI 遮挡侧使用 Godot 节点树的可见节点判断：若可见节点属于 `MegaCrit.Sts2.Core.Nodes.Screens.*` 的非战斗屏，或属于 `CommonUi` 的 modal / popup / overlay，则保守隐藏。
- 额外按节点名 token 保守隐藏 Settings、Pause、Map、Reward、Shop、Merchant、Event、CardSelect、CardReward、CardGrid、Deck、Library 等常见全屏/模态界面。
- 隐藏时清空 `RichTextLabel.Text` 并 `Hide()`；HUD 节点 `MouseFilter=Ignore`，不应留下可点击遮挡。

## HUD 刷新与冻结生命周期

- 默认开启 `FreezeHudWithinPlayerTurn`。
- 玩家 side turn 开始时：清除上一轮快照并允许提交当前回合第一份有效结果。
- 当前玩家回合首次得到可信 `KnownDamage` 且总值大于 0 时：提交为本回合 HUD 快照。
- 快照存在后：后续手牌、Block、临时状态变化不再改动显示数字；高级明细和颜色变化只重新排版同一份快照。
- 玩家点击结束回合进入 `Hook.BeforeTurnEnd` 时：停止提交新快照并继续显示已提交快照。
- 下一次玩家 side turn 开始时：清除快照并允许提交新结果。
- HUD 禁用、非战斗页面遮挡、战斗结束、玩家失效等情况会清除快照并隐藏 HUD。
- 关闭冻结设置后，HUD 回到实时刷新模式。

## 设置项与默认值

- `Enable Party Watch HUD`：默认开。
- `Show advanced shield / heart details`：默认关。
- `Freeze HUD numbers after turn end`：默认开。
- `Anchor preset`：默认 `Health bar right`；可选 right / left / above / below。
- `X offset`：默认 `24`，范围 `-320..320`。
- `Y offset`：默认 `0`，范围 `-240..240`。
- `Total expected loss` 颜色：默认白色。
- `Shield detail` 颜色：默认浅蓝。
- `Heart detail` 颜色：默认浅红。
- `Restore default settings`：恢复以上默认值并刷新面板。

## 配置持久化方式或限制

- 本轮未找到可证实的官方“单个 Mod 自定义配置持久化 API”。
- 本轮不写游戏存档、CombatState、游戏资源文件，也不猜测官方 settings 文件路径。
- 设置当前为本次运行会话内生效；重新启动游戏后预计回到默认值。
- 后续若拿到官方 Mod 配置 API 或 BaseLib 可验证设置 API，再单独接入持久化。

## 运行时验证

Codex 已完成：

- `C:\sts2\dotnet\dotnet.exe build .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore` 通过，0 warning / 0 error。
- `C:\sts2\dotnet\dotnet.exe publish .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore` 通过。
- `git diff --check` 通过，仅有既有 LF/CRLF 提示。
- `git status` 已检查，构建产物仍被忽略。
- `powershell -ExecutionPolicy Bypass -File .\scripts\Install-LocalMod.ps1` 成功安装到游戏 mods 目录。

## 未验证项

- Steam 启动后的实际设置页布局、控件可点性、颜色选择器表现。
- Settings、暂停、卡牌查看、奖励、地图、商店、事件等页面的 HUD 隐藏矩阵。
- 冻结显示在真实结束回合动画期间是否符合预期。
- 位置预设在不同分辨率和 UI 缩放下是否需要微调。
- 设置不持久化是已知限制，不是运行时 bug。

## 后续可调整项

- 基于截图微调 HUD 位置、字号、行距、颜色、阴影和设置面板尺寸。
- 若官方配置 API 可证实，接入持久化。
- 若某个遮挡页面没有被当前节点树策略捕获，记录其真实节点类型后补充到集中可见性策略。

## 提交 hash

- 代码提交：`0a1ee83`
- 文档提交：见本任务最终汇报。

## 下一步唯一任务

基于实际游戏截图进行 HUD 位置、字号、间距、颜色与设置页可读性的视觉微调。
