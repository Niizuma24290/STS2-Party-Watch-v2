# Phase 1–4｜单人攻击 HUD 基线

## 状态

已完成

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
- Steam 启动后的单人战斗中，敌人 Intent = 9 时 HUD 位于本机玩家血条右侧。
- Intent = 9 / Block = 0：运行时确认显示 `🛡 -9`。
- Intent = 9 / Block = 5：运行时确认显示 `🛡 -4`。
- Intent = 9 / Block = 10：运行时确认 HUD 隐藏。
- 多次 Block 变化后 HUD 会刷新。
- 本次未保留诊断日志。

## 仅代码确认、尚未运行时验证

- 多段攻击与多个敌人攻击累计仍未在本轮截图中独立验证。
- 非攻击 Intent 不显示仍未在本轮截图中独立验证。
- 读取失败场景不崩溃、不显示猜测值仍未在本轮截图中独立验证。

## 未解决问题

- Frost / 覆甲等攻击前确定性 Block 尚未纳入 `🛡 -N`。
- `♥ -N` 尚未实现。
- 多人 HUD 仍冻结。

## 实际改动文件

- `src/STS2PartyWatchCode/Combat/LocalIncomingDamageReader.cs`
- `src/STS2PartyWatchCode/Patches/ForecastRefreshPatch.cs`
- `docs/project-state.md`
- `docs/interface-map.md`
- `docs/mechanics-evidence.md`
- `docs/task-notes/README.md`
- `docs/task-notes/phase-1-4-singleplayer-baseline.md`

## 下一步唯一任务

- Phase 5：攻击前确定性 Block 修正

## 预期提交文件

- `src/STS2PartyWatchCode/Combat/LocalIncomingDamageReader.cs`
- `src/STS2PartyWatchCode/Patches/ForecastRefreshPatch.cs`
- `docs/project-state.md`
- `docs/interface-map.md`
- `docs/mechanics-evidence.md`
- `docs/task-notes/README.md`
- `docs/task-notes/phase-1-4-singleplayer-baseline.md`

## 提交记录

- `afbccfbf15cde6089565f8c28c83901eec4652ec`：`fix: render singleplayer attack forecast beside health bar`
