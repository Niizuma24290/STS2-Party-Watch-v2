# Project State

## 当前快照

- 任务登记文件夹：`docs/task-notes/`
- 当前唯一任务：Phase 6：Beckon / Bad Luck / Regret 的 ♥ -N 时序验证与最小接入
- 当前分支：`main`
- 当前状态：Phase 1A 已完成；Phase 1B 已完成；Phase 5 已完成并完成全量回归验证。
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
- Frost / 覆甲 / 奥利哈刚 / 假奥利哈刚 / 波纹水盆 / 斗篷扣仍未纳入 `EffectiveBlock`，等待 Phase 5C 单独验证。
- Steam 运行时已验证：Intent 9 + Burn 2 进入 `🛡` 汇总，Blockable raw = 11。
- Steam 运行时已验证：Block 0 显示 `🛡 -11`，Block 5 显示 `🛡 -6`，Block 10 显示 `🛡 -1`。
- Steam 运行时已验证：Burn 在手牌中参与盾牌栏计算，手牌变化后 HUD 刷新。
- Phase 5A+5B 代码提交：`b170994a58fa21b18c7a37de01db147a1df15746`。

## Phase 5C 已完成

- 本轮仅接入第一批 `VerifiedPreAttackBlock` 候选：Frost、覆甲、奥利哈刚、假奥利哈刚、波纹水盆、斗篷扣。
- 代码确认：`EffectiveBlock = CurrentBlock + VerifiedPreAttackBlock`，再计算 `🛡 -N = max(0, BlockableRaw - EffectiveBlock)`。
- 代码确认：Frost 读取 `FrostOrb.PassiveVal`，该值包含 Focus 的 `Hook.ModifyOrbValue` 修正。
- 代码确认：覆甲读取 `PlatingPower.Amount`。
- 代码确认：奥利哈刚 / 假奥利哈刚仅在当前 Block 为 0 时读取其 `BlockVar`。
- 代码确认：波纹水盆读取本回合是否已打出本机 Attack。
- 代码确认：斗篷扣读取当前手牌数并乘以其 `BlockVar`。
- 构建确认：`C:\sts2\dotnet\dotnet.exe build src/STS2PartyWatchCode/STS2PartyWatchCode.csproj -c Release --no-restore` 通过。
- Steam 运行时已验证：上述六个来源均可正确作为 `VerifiedPreAttackBlock` 影响 `🛡 -N`。
- 本轮未实现：钨钢棍、律动残余、钻石头冠、`♥ -N`、多人 HUD、完整回合模拟器。
- Phase 5C 功能提交：`a08cf59eeae9ce0d2d80b204339b80bfa9188815`。

## Phase 5D 已完成

- Steam 运行时回归已验证：Intent 9 / Block 0 显示 `🛡 -9`。
- Steam 运行时回归已验证：Intent 9 / Block 5 显示 `🛡 -4`。
- Steam 运行时回归已验证：Intent 9 / Block 10 隐藏。
- Steam 运行时回归已验证：无攻击 / 无 DamageVar 隐藏，非战斗隐藏。
- Steam 运行时回归已验证：Burn 2 / 无攻击 / Block 0 显示 `🛡 -2`。
- Steam 运行时回归已验证：Burn 2 / 无攻击 / Block 2 隐藏。
- Steam 运行时回归已验证：Intent 9 + Burn 2 + Block 5 显示 `🛡 -6`。
- Steam 运行时回归已验证：Burn 进入或离开手牌后 HUD 刷新。
- Steam 运行时回归已验证：Frost、覆甲、奥利哈刚、假奥利哈刚、波纹水盆、斗篷扣各自真实战斗案例通过。
- Steam 运行时回归已验证：多个 `VerifiedPreAttackBlock` 来源共存时不重复计数；Block 足以覆盖总伤害时 HUD 隐藏；与 Burn 共存时仍正确。
- 机制缺口收口：Beckon、Bad Luck、Regret、钨钢棍、律动残余留给 Phase 6；钻石头冠 / `DiamondDiademPower` 归为后续伤害修正机制。
- Phase 5D 收口提交：`5d9d4cdece14a9244efbc26fe4dd0e5173554a38`。

## 阶段状态

| 阶段 | 状态 | 任务 | 完成标准 | 下一步依赖 |
| --- | --- | --- | --- | --- |
| Phase 0 | 已完成 | 仓库初始化 | v2 新仓库、文档、远程 main 已建立 | 无 |
| Phase 1A | 已完成 | Mod 发现与加载验证 | Steam 启动、Mod 列表可见、Loaded 日志确认 | Phase 1B |
| Phase 1B | 已完成 | 单人攻击 HUD 运行时验证 | `🛡 -N` 在单人攻击 Intent 场景正确显示 | Phase 5 |
| Phase 5 | 已完成 | Blockable Incoming Damage 汇总 | 怪物攻击、手牌回合末 blockable DamageVar、第一批 EffectiveBlock 候选已纳入 `🛡 -N` 并完成回归验证 | Phase 6 |
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
