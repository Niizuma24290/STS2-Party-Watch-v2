# Phase 1–4｜单人攻击 HUD 基线

## 状态

进行中

## 目标

空 Mod 可加载 → 只读读取单人战斗数据 → HUD 可显示 → 使用原生 AttackIntent 预览显示 `🛡 -N`。

## 本任务允许做的事

- 建立符合当前 STS2 ModLoader 规则的可部署 Mod 工程前置。
- 只读读取单人战斗数据。
- 建立 HUD 显示路径。
- 使用原生 AttackIntent 预览计算并显示 `🛡 -N`。

## 本任务禁止做的事

- Frost / 覆甲 Block 修正。
- `♥ -N`。
- 多人 HUD。
- 完整回合模拟。
- 通用伤害引擎。
- 提交 DLL、PDB、PCK、logs、publish 输出、NuGet 缓存或游戏目录文件。

## 完成标准

- Mod 可加载。
- 可只读读取单人战斗数据。
- HUD 可显示 `🛡 -N`。
- 使用原生 AttackIntent 预览作为攻击伤害来源。
- 已记录运行时验证结果和未验证内容。

## 已验证事实

- 已通过本机 BaseLib 示例确认 Mod 文件为 manifest JSON、DLL、可选 PCK 同级放置。
- 已通过 ModTemplate-StS2 wiki 确认 Windows local mods 位于游戏安装目录的 `mods` 下。
- 已通过工程配置确认 v2 使用 `net9.0`、程序集名 `sts2-party-watch-v2`、metadata `sts2-party-watch-v2.json`。
- 已通过工程配置确认 `GenerateDependencyFile=false`，publish 输出不应包含 `.deps.json`。
- 已通过工程配置确认入口类 `STS2PartyWatch.MainFile` 使用 `[ModInitializer(nameof(Initialize))]` 并输出 `[STS2 Party Watch] Loaded`。

## 仅代码确认、尚未运行时验证

- `NCombatUi.Activate` patch 创建/刷新 HUD controller。
- `NCombatUi.Deactivate` patch 清理 HUD。
- 单人路径通过 `LocalContext.GetMe(ICombatState)` 读取本机玩家。
- 当前本机 Block 来自 `Creature.Block`。
- 敌人攻击 RAW 来自 `AttackIntent.GetTotalDamage(new[] { localCreature }, enemy)`。
- 多个敌人的可读攻击 Intent 会先累计 RAW，再统一减当前 Block。
- HUD 文本严格为 `🛡 -N`。
- HUD 尝试定位在本机玩家血条右侧，并预留水平空间；定位失败时回退到玩家节点右侧偏移。
- 非单人、无战斗、无本机玩家、无本机 Creature、无攻击 Intent、RAW 不可读、OUT <= 0 时隐藏。

## 未解决问题

- 尚未在本任务启动游戏；Steam 启动验证留到 Phase 1A。
- 尚未验证游戏内 Mod 列表显示 `STS2 Party Watch v2`。
- 尚未验证日志出现 `[STS2 Party Watch] Loaded`。
- 尚未验证 Mod 不导致启动崩溃。
- 尚未验证非战斗 HUD 隐藏。
- 尚未验证进入单人战斗后 HUD 正常工作。
- 尚未验证单段攻击、多段攻击、多个敌人累计、Block 变化刷新、非攻击 Intent 隐藏、OUT = 0 隐藏。

## 实际改动文件

- `src/STS2PartyWatchCode/STS2PartyWatchCode.csproj`
- `src/STS2PartyWatchCode/sts2-party-watch-v2.json`
- `src/STS2PartyWatchCode/MainFile.cs`
- `scripts/Install-LocalMod.ps1`
- `README.md`
- `docs/build-environment.md`
- `docs/project-state.md`
- `docs/task-notes/README.md`
- `docs/task-notes/phase-1-4-singleplayer-baseline.md`

## 下一步唯一任务

- Phase 1A：Steam 启动后的 Mod 发现与加载验证

## 预期提交文件

- `src/STS2PartyWatchCode/STS2PartyWatchCode.csproj`
- `src/STS2PartyWatchCode/sts2-party-watch-v2.json`
- `src/STS2PartyWatchCode/MainFile.cs`
- `scripts/Install-LocalMod.ps1`
- `README.md`
- `docs/build-environment.md`
- `docs/project-state.md`
- `docs/task-notes/README.md`
- `docs/task-notes/phase-1-4-singleplayer-baseline.md`

## 提交记录

- 本轮提交：`build: complete v2 mod packaging prerequisites`。最终 hash 以结束汇报为准。
