# Phase 1–4｜单人攻击 HUD 基线

## 状态

进行中 / Phase 1B 阻塞

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

- Phase 1A 已完成。
- Steam 启动成功。
- Mod 列表可见 `STS2 Party Watch v2`。
- 日志出现 `[STS2 Party Watch] Loaded`。
- 日志出现 `Loaded 2 mods (2 total)`。
- 非战斗 HUD 隐藏已验证。

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

- Phase 1B 进行中 / 阻塞。
- 单人战斗中敌人 Intent = 9 时 HUD 未显示。
- Block 0 → 5 → 10 后仍未显示。
- Block = 5 时理论预期应为 `🛡 -4`。
- 当前尚未确定问题位于：战斗读取、Intent 识别、`GetTotalDamage`、OUT 计算或 HUD 渲染。
- 未做代码修复。

## 实际改动文件

- `docs/project-state.md`
- `docs/task-notes/README.md`
- `docs/task-notes/phase-1-4-singleplayer-baseline.md`

## 下一步唯一任务

- Phase 1B：定位并最小修复单人攻击 HUD 不显示问题

## 预期提交文件

- `docs/project-state.md`
- `docs/task-notes/README.md`
- `docs/task-notes/phase-1-4-singleplayer-baseline.md`

## 提交记录

- 待提交：`docs: record load validation and HUD display blocker`
