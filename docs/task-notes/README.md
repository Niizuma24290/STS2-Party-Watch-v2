# v2 任务登记

`docs/task-notes/` 是 v2 的唯一任务登记文件夹。不要创建第二套平行进度目录。

## 当前状态

Phase 1A 至 Phase 9A 已完成并有对应历史记录。Phase 9B 普通 Poison、设置入口修复、本机多人 HUD 和部分 Workshop 订阅版验证实际上属于 Workshop 上传完成后的后续补充，但历史文件名已沿用 `phase-9*`；这些文件名保留为证据，不再继续扩展新的 Phase 9 子阶段。Phase 10 视为 Workshop 上传 / 订阅测试里程碑。之后新增补充工作统一从 Phase 11 登记。最后补记中的原生毒血条、`AccelerantPower`、Exoskeleton / HardToKill、Slippery、敌方 Intangible、TestSubject、SewerClam 等 Poison 边界已汇总到本总记录和 `docs/project-state.md`。正式 `FormalMultiplayerHud`、队友顶栏预测和共享队伍 HUD 继续冻结。

## 当前唯一任务

从 Phase 11 开始登记后续补充；当前下一步是用临时本机血条中线辅助线和 HUD 文本中心诊断线检查默认 HUD 位置对齐效果，完成后删除这些辅助线，同时继续保持正式多人 HUD 冻结。

## 禁止事项

- 不提交 DLL、PDB、PCK、logs、publish 输出、NuGet 缓存、Workshop uploader 文件、封面、`mod_id.txt`、`work/` 或游戏目录文件。
- 不修改游戏文件。
- 不把代码确认写成运行时验证事实。
- 不创建第二套平行进度目录。
- 不把本机多人 HUD 写成正式多人/队友/共享 HUD。

## 登记规则

1. 更新对应 Phase 或补记文件。
2. 更新本 README 的当前状态和总计划表。
3. 更新 `docs/project-state.md` 的当前快照。
4. 明确写出下一步唯一任务。
5. 写入实际提交 hash。
6. 区分 Implemented、Built、Installed、RuntimeVerified、DocumentedOnly。

## 笔记规划

| 层级 | 文件 | 用途 | 删除规则 |
| --- | --- | --- | --- |
| 总索引 | `README.md` | task notes 入口、当前状态、计划表、近期补记入口 | 不删 |
| 总笔记 | `project-total-note.md` | 汇总最近所有散信息、历史遗漏、当前支持范围与限制 | 不删 |
| 项目总状态 | `../project-state.md` | 面向整个项目的当前事实总览，不只服务 task notes | 不删 |
| Phase 历史证据 | `phase-0-*` 到 `phase-9*` | 记录每阶段实际改动、验证、限制、commit hash；其中部分 `phase-9*` 是历史误编号的后续补充 | 不因总笔记已摘要而删除 |
| Workshop 里程碑 | `workshop-private-rc-*` | Phase 10：Workshop 上传 / 订阅测试里程碑和发布工作区证据 | 不删 |
| Phase 11 补充 | `phase-11-supplements.md` / `phase-11-<topic>.md` | Workshop 上传完成后的新增补充、补验证、Bug fix 和文档同步 | 后续新增工作优先写这里 |
| 专题补记 | `modding-settings-entry-*`、`workshop-private-rc-*` | 记录跨 Phase 的具体修复 / 发布准备证据 | 不删，除非未来有同主题正式替代笔记并在此索引标明 |

已清理的重复 / 冲突文件：旧的 `phase-9-documentation-reconciliation.md` 已重命名为 `project-total-note.md`；旧的 `phase-10-multiplayer-research-frozen.md` 只有冻结政策且与 Phase 10 Workshop 里程碑冲突，已删除，冻结规则改由本索引、路线图和 Phase 11 说明承载。

## 总计划表

| 阶段 | 状态 | 任务 | 完成标准 | 下一步依赖 |
| --- | --- | --- | --- | --- |
| Phase 0 | 已完成 | 仓库初始化 | v2 新仓库、文档、远程 main 已建立 | 无 |
| Phase 1A | 已完成 | Mod 发现与加载验证 | Steam 启动、Mod 列表可见、Loaded 日志确认 | Phase 1B |
| Phase 1B | 已完成 | 单人攻击 HUD 运行时验证 | `🛡 -N` 在单人攻击 Intent 场景正确显示 | Phase 5 |
| Phase 5 | 已完成 | Blockable Incoming Damage 汇总 | 怪物攻击、手牌回合末 blockable DamageVar、第一批 EffectiveBlock 候选纳入 HUD 并回归验证 | Phase 6 |
| Phase 6A | 已完成 | Direct HP Loss 固定值 | Beckon、Bad Luck 显示 `♥ -N` 并完成 Steam 矩阵 | Phase 6B |
| Phase 6B | 已完成 | Regret Direct HP Loss | Regret 按当前手牌总数进入 `♥ -N` | Phase 6C |
| Phase 6C | 已完成 | Direct HP Loss 联合验证 | Beckon、Bad Luck、Regret 的 `♥ -N` 完成 Steam 联合验证 | Phase 7 |
| Phase 7 | 已完成，条件支持 | HP Loss 结果修正机制 | TungstenRod、BeatingRemnant 按正常游戏范围收口，复数棍子控制台无效状态排除 | Phase 8 |
| Phase 8 | 已完成，条件支持 | 非 Block 承伤修正机制 | DiamondDiademPower 最小代码接入并完成用户 Steam 验证 | Phase 9 |
| Phase 9 | 已完成 | 单人正式版收口 | 默认总 `-N`、高级 `🛡/♥`、文档与回归收口 | Phase 9A |
| Phase 9A | 已完成 | HUD lifecycle、隐藏策略、设置与显示生命周期 | HUD 可见性、冻结快照、设置面板、位置与颜色项完成 | 设置入口修复 / Workshop prep |
| 设置入口修复 | 已完成 | 不占用 native Settings / Modding 主入口 | 设置入口只在 Party Watch 自身 Mod info panel 中显示 | Steam 设置入口验证 |
| Phase 9B | 已实现，历史误编号；后续归 Phase 11 | Poison 行动前存活预览 | 普通 Poison 确定击杀的 enemy instance 不再贡献当前 Attack Intent | Phase 11 专项 Steam 验证 |
| 本机多人 HUD | 已完成，条件支持 | 多人战斗中显示本机 local HUD | Workshop 订阅版已看到本机 `-6` 在本机角色血条下方 | 正式多人 HUD 仍冻结 |
| Phase 10 | 已完成 / 私密订阅里程碑 | Steam Workshop upload / subscription test | item `3755598583`、private visibility、订阅版本机 HUD 验证、封面和 preview 上传记录 | 不得误写为 public release |
| Phase 11 | 当前后续阶段 | 补充与维护 | 新增补丁、补验证和文档同步从 Phase 11 登记；本机血条中线辅助线和 HUD 文本中心诊断线已实现，当前版本共享同一目标中心，但尚未运行时验证 | `phase-11-supplements.md` |

## Phase 9B Poison 总记录

已实现 / 有代码证据：

- `EnemyPreActionSurvivalPreview` / `PoisonTickPreview` 已接入 `LocalIncomingDamageReader`。
- 普通敌人会按 enemy instance 独立预览，确定会在行动前死于 Poison 时，该实例当前 `AttackIntent` 不进入最终 `-N`。
- 敌人身份优先使用 `Creature.CombatId`，否则使用 native object reference + 当前快照 index；不使用显示名称做 key。
- 读取当前 `PoisonPower.Amount`、enemy HP、当前 Intent contribution、opponent `AccelerantPower.Amount`。
- `AccelerantPower` 只读已生效层数；不重放 `Accelerant` 卡牌，不修改命令队列。

原生毒血条补记：

- `PoisonPower.CalculateTotalDamageNextTurn()` 是 native poison foreground 的模型层总量入口。
- 触发次数为 `min(PoisonPower.Amount, 1 + alive opponents AccelerantPower amount sum)`。
- 每段 poison preview 会调用 `Hook.ModifyDamage(...)`，因此可作为后续 HardToKill / damage cap 小步接入的证据候选。
- `NHealthBar.IsPoisonLethal(...)` 和 `_poisonForeground` 只是 UI 预览，不证明 enemy Intent 是否取消。

当前未验证 / 不支持：

- 普通 Poison kill / no-kill、多敌人、同名敌人的 Steam 专项矩阵尚未完整补齐。
- `HardToKillPower` / Exoskeleton：已记录 native poison-bar 证据，尚未接入 Party Watch。
- `SlipperyPower`：仍有层数时不得用 Poison survival preview 移除该敌人 Intent；后续应仅禁用该 enemy instance 的 Poison 修正。
- 敌方 `IntangiblePower`：每段 Poison cap 和 Intent 取消需要分开验证。
- `TestSubject` / `AdaptablePower`：复活 / 阶段后的当前 Intent 取消点未确认。
- `ToughEgg` / `HatchPower`、`SewerClam` / `HardenedShellPower`：保持 unsupported / research-only。

## 近期补记索引

- `project-total-note.md`: 项目总笔记；本次全量文档同步与历史补记。
- `modding-settings-entry-2026-07-02.md`: Party Watch 设置入口从 native Settings/Modding 主入口移到自身 Mod info panel 的修复记录。
- `phase-9b-poison-pre-action-survival.md`: Poison 行动前存活预览与原生毒血条补充证据。
- `phase-11-supplements.md`: Workshop 上传完成后的补充与维护阶段入口。
- `workshop-private-rc-2026-07-01.md`: Workshop workspace、private/subscription 测试和多人本机 HUD 记录。
## Current Diagnostic Update

Phase 11 local HUD alignment now includes a temporary `[STS2 Party Watch][HUD Align]` runtime coordinate log in commit `0bf9a7e9ee597a1eb756b3432dadea4c7ee7734e`.

Status boundary: Implemented, Built, and Installed only. RuntimeVerified remains pending until Steam runtime coordinates are captured from the log.

Next task: use the log line, plus the cyan health-bar center guide and magenta HUD text center guide, to compare runtime centers without screenshot pixel measuring; then remove the temporary guides and log.
