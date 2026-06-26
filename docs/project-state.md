# Project State

## 当前快照

- 任务登记文件夹：`docs/task-notes/`
- 当前唯一任务：Phase 1B：定位并最小修复单人攻击 HUD 不显示问题
- 当前分支：`main`
- 当前状态：Phase 1A 已完成；Phase 1B 进行中 / 阻塞。
- 约束：不提交 DLL、PDB、PCK、logs、publish 输出、NuGet 缓存或游戏目录文件。

## Phase 1A 已完成

- Steam 启动成功。
- Mod 列表可见 `STS2 Party Watch v2`。
- 日志出现 `[STS2 Party Watch] Loaded`。
- 日志出现 `Loaded 2 mods (2 total)`。

## Phase 1B 进行中 / 阻塞

- 非战斗 HUD 隐藏已验证。
- 单人战斗中敌人 Intent = 9 时 HUD 未显示。
- Block 0 → 5 → 10 后仍未显示。
- Block = 5 时理论预期应为 `🛡 -4`。
- 当前尚未确定问题位于：战斗读取、Intent 识别、`GetTotalDamage`、OUT 计算或 HUD 渲染。
- 未做代码修复。

## 阶段状态

| 阶段 | 状态 | 任务 | 完成标准 | 下一步依赖 |
| --- | --- | --- | --- | --- |
| Phase 0 | 已完成 | 仓库初始化 | v2 新仓库、文档、远程 main 已建立 | 无 |
| Phase 1A | 已完成 | Mod 发现与加载验证 | Steam 启动、Mod 列表可见、Loaded 日志确认 | Phase 1B |
| Phase 1B | 进行中 / 阻塞 | 单人攻击 HUD 运行时验证 | `🛡 -N` 在单人攻击 Intent 场景正确显示 | 定位并最小修复 HUD 不显示问题 |
| Phase 5 | 未开始 | 攻击前确定性 Block | Frost、覆甲等已验证 Block 纳入 `🛡 -N` | Phase 6 |
| Phase 6 | 未开始 | Direct HP Loss | Beckon、Bad Luck、Regret 显示 `♥ -N` | Phase 7 |
| Phase 7 | 未开始 | 单人验证与收口 | 单人 HUD 规则、运行时验证、文档收口 | 后续机制补充 |
| Phase 8 | 冻结 | 多人研究 | 仅研究真实目标与原生预览，不做正式多人 HUD | 证据充分后再开启 |

## 任务登记规则

- 每次任务结束必须更新对应 Phase 文件。
- 每次任务结束必须更新 `docs/task-notes/README.md` 的状态表。
- 每次任务结束必须更新本文档的当前快照。
- 每次任务结束必须明确下一步唯一任务。
- 每次任务结束必须写入实际提交 hash。
- 不把代码推测写成运行时验证事实。
