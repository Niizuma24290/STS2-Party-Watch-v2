# Project State

## 当前快照

- 任务登记文件夹：`docs/task-notes/`
- 当前唯一任务：Phase 5C：Frost 是否应计入 EffectiveBlock 的运行时验证与最小接入
- 当前分支：`main`
- 当前状态：Phase 1A 已完成；Phase 1B 已完成；Phase 5A+5B 已完成。
- 约束：不提交 DLL、PDB、PCK、logs、publish 输出、NuGet 缓存或游戏目录文件。

## Phase 1A 已完成

- Steam 启动成功。
- Mod 列表可见 `STS2 Party Watch v2`。
- 日志出现 `[STS2 Party Watch] Loaded`。
- 日志出现 `Loaded 2 mods (2 total)`。

## Phase 1B 已完成

- 非战斗 HUD 隐藏已验证。
- Steam 启动后的单人战斗中，敌人 Intent = 9 时 HUD 会在本机玩家血条右侧显示。
- Intent = 9 / Block = 0：运行时确认显示 `🛡 -9`。
- Intent = 9 / Block = 5：运行时确认显示 `🛡 -4`。
- Intent = 9 / Block = 10：运行时确认 HUD 隐藏。
- 多次 Block 变化后 HUD 会刷新。
- 本次未保留诊断日志。

## Phase 1B 根因与修复

- 根因：原实现将 HUD 作为 `NCombatUi` 全局子节点创建和刷新，未可靠绑定到玩家血条生命周期，导致战斗和 Intent 可见但 HUD 标签未显示。
- 修复：参考 Minty 的 `NHealthBar` 生命周期策略，改为在本机玩家 `NHealthBar.SetCreature`、`NHealthBar.RefreshValues`、血条容器尺寸更新后创建、定位和刷新标签。
- 最终策略：标签挂在 `HpBarContainer` 的同级父节点或等价父节点，默认隐藏；仅在单人本机玩家、可读攻击 Intent、`OUT > 0` 时显示 `🛡 -N`。
- 修复提交：`afbccfbf15cde6089565f8c28c83901eec4652ec`。

## Phase 5A+5B 已完成

- 目标：`🛡 -N = max(0, MonsterIntentRaw + HandTurnEndDamageRaw - EffectiveBlock)`。
- 本次代码已接入手牌回合末 blockable `DamageVar`，并在手牌内容变化后刷新 HUD。
- 明确排除：`HpLossVar`、`ValueProp.Unblockable`、Beckon、Bad Luck、Regret。
- Frost / 覆甲仍未纳入 `EffectiveBlock`，等待后续单独验证。
- Steam 运行时已验证：Intent 9 + Burn 2 进入 `🛡` 汇总，Blockable raw = 11。
- Steam 运行时已验证：Block 0 显示 `🛡 -11`，Block 5 显示 `🛡 -6`，Block 10 显示 `🛡 -1`。
- Steam 运行时已验证：Burn 在手牌中参与盾牌栏计算，手牌变化后 HUD 刷新。
- Phase 5A+5B 代码提交：`b170994a58fa21b18c7a37de01db147a1df15746`。

## 阶段状态

| 阶段 | 状态 | 任务 | 完成标准 | 下一步依赖 |
| --- | --- | --- | --- | --- |
| Phase 0 | 已完成 | 仓库初始化 | v2 新仓库、文档、远程 main 已建立 | 无 |
| Phase 1A | 已完成 | Mod 发现与加载验证 | Steam 启动、Mod 列表可见、Loaded 日志确认 | Phase 1B |
| Phase 1B | 已完成 | 单人攻击 HUD 运行时验证 | `🛡 -N` 在单人攻击 Intent 场景正确显示 | Phase 5 |
| Phase 5 | 进行中 | Blockable Incoming Damage 汇总 | 怪物攻击与手牌回合末 blockable DamageVar 纳入 `🛡 -N`，已验证 Block 纳入 EffectiveBlock | Phase 5C |
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
