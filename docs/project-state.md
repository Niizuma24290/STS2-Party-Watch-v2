## 追加快照：Phase 9A 前端 UI、设置与显示生命周期

- 本轮只做前端产品化，不修改 `LocalIncomingDamageReader`、`LocalDamageForecast`、遗物/Power 修正或任何预测公式。
- HUD 可见性集中到 `PartyWatchHudVisibilityPolicy.ShouldRenderHud()`：要求有效单人本机战斗、玩家存活、血条可见，并在 Settings、暂停、卡牌查看、奖励、地图、商店、事件、modal / popup / overlay 等可见时保守隐藏。
- HUD 显示层新增 `FreezeHudWithinPlayerTurn` 快照：玩家 side turn 开始清空并等待第一份可信结果；提交后本回合保持稳定；点击结束回合后继续显示快照；下一玩家回合重新提交。
- 新增原生 `NSettingsScreen` 注入面板，支持启用 HUD、高级 `🛡 / ♥` 明细、冻结开关、位置预设、X/Y 偏移、三组颜色和恢复默认。
- 未找到可证实的官方单 Mod 配置持久化 API；本轮设置仅本次运行会话生效，不写游戏存档、CombatState、游戏资源文件或猜测 settings 路径。
- 代码已构建、发布并安装；等待用户 Steam 运行时验证页面隐藏、冻结显示和设置面板可用性。
- 下一步唯一任务：基于实际游戏截图进行 HUD 位置、字号、间距、颜色与设置页可读性的视觉微调。

## 追加快照：TungstenRod / BeatingRemnant 预测顺序

- 本轮只调整 Party Watch 的 HP loss 结果修正预测顺序：`TungstenRod` 固定早于 `BeatingRemnant`。
- 目的：当心脏与棍子共存且未来总 HP loss 大于心脏预算时，HUD 预测先应用棍子减免，再由心脏按剩余预算封顶。
- 本轮不修改真实游戏结算、不扩展新机制、不改变 HUD 显示格式。
- Steam 运行时尚未验证；需要用户确认单棍子 + 心脏 + 当前回合未受伤 + 单段大于 20 伤害时，HUD 与实际 HP 下降是否一致。

## 追加快照：IntangiblePower / 无实体

- 本轮只接入 `IntangiblePower` 对已验证 direct HP loss 与 HP loss 结果链的影响。
- 用户实测规则确认：无实体按每个独立事件先变为 1；Block 可抵消该 1；多段攻击按每 hit 1；Beckon / Bad Luck / Regret 各自变为 1；无实体先于 `TungstenRod` 与 `BeatingRemnant`。
- blockable damage 仍信任原生 `AttackIntent` / `Hook.ModifyDamage` 预览，避免二次 cap；direct HP loss 保留单事件粒度后再按无实体修正。
- 本轮不接入 BufferPower、PoisonPower、DoomPower、DemisePower 或通用 HP loss 引擎。
- 代码已构建、发布并安装；用户 Steam 运行时验证已通过。

## 追加快照：Constrict / Disintegration Power 自伤

- 本轮只接入 `ConstrictPower` 与 `DisintegrationPower` 两个玩家回合末 Power 自伤来源。
- 两者按本地原生 IL 证据走 `DamageProps.nonCardUnpowered`，因此进入可信 blockable `🛡`，不进入 direct HP loss `♥`。
- 有 `TungstenRod` / `BeatingRemnant` 时，事件顺序接入为：手牌回合末事件 -> `ConstrictPower` -> `DisintegrationPower` -> 敌方 AttackIntent。
- `PoisonPower`、`DoomPower`、`DemisePower` 与通用 Power damage 引擎仍未接入。
- 代码已构建、发布并安装；用户 Steam 运行时验证已通过。

# Project State

## 当前快照

- 任务登记文件夹：`docs/task-notes/`
- 当前唯一任务：基于实际游戏截图进行 HUD 位置、字号、间距、颜色与设置页可读性的视觉微调
- 当前分支：`main`
- 当前状态：Phase 1A 已完成；Phase 1B 已完成；Phase 5 已完成并完成全量回归验证；Phase 6A 已完成代码接入与 Steam 运行时验证；Phase 6B 已按 shipped 代码机制接入 Regret；Phase 6C 已完成 Beckon / Bad Luck / Regret 的 Steam 运行时联合验证；Phase 7 已按正常游戏范围收口：TungstenRod 与 BeatingRemnant 已接入，复数棍子控制台无效状态不作为正式行为依据；Phase 8 已完成 DiamondDiademPower 最小接入并通过用户 Steam 运行时验证；Phase 9 合并 HUD 显示改造、水盆 + 惊涛修补、Constrict / Disintegration Power 自伤修补、IntangiblePower / 无实体 HP loss 结果修补均已完成代码、文档与用户 Steam 运行时收口；追加 TungstenRod 早于 BeatingRemnant 的预测顺序调整等待用户 Steam 运行时验证；Phase 9A 前端 UI、设置与显示生命周期已完成代码接入、构建、发布与安装，等待用户 Steam 运行时验证。
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

## Phase 6A 已完成

- 本轮只接入 Beckon / Bad Luck 的固定 direct HP loss，不接入 Regret。
- 代码确认：Beckon 的 shipped 类型为 `MegaCrit.Sts2.Core.Models.Cards.Beckon`，`HasTurnEndInHandEffect == true`，回合末从 `HpLossVar(6m)` 读取数值并以 `ValueProp.Unblockable` 调用 `CreatureCmd.Damage`。
- 代码确认：Bad Luck 的 shipped 类型为 `MegaCrit.Sts2.Core.Models.Cards.BadLuck`，`HasTurnEndInHandEffect == true`，回合末从 `HpLossVar(13m)` 读取数值并以 `ValueProp.Unblockable` 调用 `CreatureCmd.Damage`。
- 代码确认：`CombatManager.DoTurnEnd` 只从当前手牌收集回合末手牌效果；`CardModel.OnTurnEndInHandWrapper` 先执行 `OnTurnEndInHand`，再按 Ethereal / Discard 规则收尾。
- 已接入并验证：`DirectHpLoss = BeckonLoss + BadLuckLoss`，并以独立 `♥ -N` 行显示；`🛡 -N` 仍按既有 blockable 公式计算。
- 构建确认：`C:\sts2\dotnet\dotnet.exe build src\STS2PartyWatchCode\STS2PartyWatchCode.csproj --no-restore` 通过。
- 发布确认：`C:\sts2\dotnet\dotnet.exe publish src\STS2PartyWatchCode\STS2PartyWatchCode.csproj --no-restore` 通过。
- 原任务卡中的 `STS2PartyWatch.sln`、`NuGet.Config`、`tools/check-forbidden-files.ps1` 在当前仓库不存在；已用实际项目路径完成 restore/build/publish，并用 `git status --short --ignored` 确认 publish 输出仍为 ignored。
- Steam 运行时已验证：无 Beckon / Bad Luck 时不显示 `♥`；仅 Beckon 显示 `♥ -6`；仅 Bad Luck 显示 `♥ -13`；Beckon + Bad Luck 显示 `♥ -19`；Block 改变不影响 `♥`；卡离开手牌后 `♥` 同步减少或隐藏；非战斗两行隐藏。
- Steam 运行时已验证：Bad Luck 与 Burn、敌人攻击共存时，`🛡 -18` 与 `♥ -13` 分两行显示，`♥` 不受 Block 影响。
- 证据案例：用户提供截图 `C:\Users\ROG\AppData\Local\Temp\codex-clipboard-2405da0f-af6a-41d9-aea8-addc99785f37.png`，手牌含 `霉运` 与 `灼伤`，敌人 Intent 16，Block 0，HUD 显示 `🛡 -18` / `♥ -13`。
- 本轮未完成：Regret、TungstenRod、BeatingRemnant、DiamondDiadem / DiamondDiademPower。

## Phase 6B 已完成

- 本轮只接入 Regret 的 direct HP loss，不接入 TungstenRod、BeatingRemnant、DiamondDiadem / DiamondDiademPower。
- shipped 代码确认：Regret 的类型为 `MegaCrit.Sts2.Core.Models.Cards.Regret`，`HasTurnEndInHandEffect == true`。
- shipped 代码确认：Regret 在 `BeforeSideTurnEnd` 中若自身仍在 `PileType.Hand`，记录 `base.Pile.Cards.Count`。
- shipped 代码确认：Regret 在 `OnTurnEndInHand` 中以记录的 `CardsInHand` 调用 `CreatureCmd.Damage(..., ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this)`。
- 已接入：`RegretLoss = 当前手牌总数 × 当前手牌中的 Regret 数量`，当前手牌总数包含 Regret 自己。
- 已接入：`DirectHpLoss = BeckonLoss + BadLuckLoss + RegretLoss`；`♥ -N` 不受 Block 影响，不进入 `🛡 -N`。
- 本轮未新增刷新 patch，继续复用 `CardPile.InvokeContentsChanged` 与玩家血条刷新链。
- Steam 运行时已验证 Regret 可按当前手牌总数进入 `♥ -N`。

## Phase 6C 已完成

- Steam 运行时联合验证已完成：Beckon、Bad Luck、Regret 可共同进入 `DirectHpLoss` 与 `♥ -N`。
- Steam 运行时已验证：`♥ -N` 与 `🛡 -N` 分行显示，不合并。
- Steam 运行时已验证：Regret 的贡献不受当前 Block 影响，不进入 `🛡 -N`。

## Phase 7 已完成

- 已接入窄范围 `VerifiedHpLossRelicModifier`，只处理已验证 HP Loss 事件上的 `TungstenRod` 与 `BeatingRemnant`。
- 已接入按当前手牌顺序排列的 Burn / Beckon / Bad Luck / Regret 未来事件流；Burn 先经过既有 Block 逻辑得到实际 HP loss，再进入遗物修正。
- 已接入 `BeatingRemnant` 共享预算：手牌回合末事件先消耗预算，敌人攻击事件在最后消耗剩余预算。
- 已接入敌方 `AttackIntent.GetSingleDamage(...) * Repeats == GetTotalDamage(...)` 时的逐 hit 事件拆分，棍子可逐 hit 生效。
- 已限制 `TungstenRod` 只修正可证明为单次 HP loss 的事件；存在棍子且敌方攻击无法拆分、又会造成正数 HP loss 时，盾牌行保持不支持。
- 已确认多根 `TungstenRod` 属于控制台制造的无效状态，不作为正式玩法行为修改依据。
- 已在原生遗物 add/remove/melt 后刷新 HUD，覆盖控制台中途添加遗物后的显示更新。
- 已新增 HUD 侧实际 HP 下降累计预算：玩家 `BeforeSideTurnStart` 清零，当前窗口内只累计 HP 下降，回血不抵消；`BeatingRemnant` 预测改用该观测预算，不再直接信任 `_damageReceivedThisTurn`。
- Phase 7 按正常游戏范围收口；复数棍子等控制台无效状态不作为 Phase 7 阻塞项。
- 下一步进入 Phase 8 运行时验证：王冠 / `DiamondDiademPower` 最小接入。

## Phase 8 已完成

- 已新增窄范围 `VerifiedEnemyDamageModifier`，只处理 Diamond Diadem 对已验证敌方 attack damage 事件的修正。
- 当前 `DiamondDiademPower` 已存在时继续信任原生 `AttackIntent.GetSingleDamage(...)` / `GetTotalDamage(...)`，避免双重减半。
- 当前 Power 尚未存在但玩家持有 Diamond Diadem，且 `CardsPlayedThisTurn` 在考虑 `StampedePower` 后仍不超过 `CardThreshold` 时，预估回合结束会获得 Power，并按每 hit 乘 `0.5` 后取整。
- `StampedePower` 已按窄范围用于王冠计数修正；本轮追加用于波纹水盆判断：若结束回合会先自动打出非 `Unplayable` Attack，则水盆不应预测获得 Block。
- 无法证明 single hit 粒度的敌方聚合攻击不会使用总值减半；受 Diamond Diadem 影响的 `🛡` 保持 Unknown。
- 不处理 Burn、Beckon、Bad Luck、Regret、其他 direct HP loss、玩家自伤、多人 HUD 或通用 damage 系统。
- 构建确认：`C:\sts2\dotnet\dotnet.exe build .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore` 通过。
- 发布确认：`C:\sts2\dotnet\dotnet.exe publish .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore` 通过。
- 初次用户验证时 HUD 未变化，根因确认为实际游戏 mods 目录仍加载旧 DLL。
- 已按用户要求更新实际游戏 mods 目录 DLL，并确认 publish DLL 与游戏目录 DLL 的 SHA256 均为 `32B5C963E50FAE6BC04293E30C0E95D634877895F242C1C280FE44C64032B516`。
- 用户 Steam 运行时验证通过：敌人头顶可继续显示原生伤害，HUD 会提前显示 Diamond Diadem 作用后的“现在点击结束回合后的实际承伤”。

## Phase 9 已完成代码与文档收口，等待最终运行时回归

- 已删除未引用的旧 UI 路径：`ForecastHudController`、`ForecastHudView`、`HealthBarLocator`。
- 当前生产 HUD 路径只保留 `ForecastRefreshPatch` 在本机 `NHealthBar` 旁创建和刷新一个 label。
- 合并 HUD 显示改造已接入：默认只显示 `-(OutDamage + DirectHpLoss)`。
- 可信来源明细 `🛡 OutDamage` / `♥ DirectHpLoss` 已收为 UI 层高级开关，默认关闭；尚未接入正式设置页或外部配置文件。
- 合并总值只由已有可信 `ForecastResult.OutDamage` 与 `ForecastResult.DirectHpLoss` 相加，不重新读取游戏状态，不改机制计算。
- 已修复 HP loss relic 路径的 blockable hand damage 限制：有 Tungsten Rod / Beating Remnant 时，不再只允许 Burn；已通过 `CardTurnEndDamageInspector` 证明会造成 blockable `DamageVar` 的手牌回合末伤害也能进入逐事件修正。
- 已追加修复波纹水盆 + 惊涛：`VerifiedPreAttackBlockReader` 在水盆分支中考虑 pending `StampedePower` 自动打出的非 `Unplayable` Attack，避免错误预加水盆 Block；用户 Steam 运行时验证已通过。
- Unknown 或 0 输出隐藏，不显示猜测值。
- 已更新 `docs/architecture.md`，记录当前生产职责边界：combat reader 读机制，forecast 组合数值，patch 只负责 UI label。
- 已更新 `docs/task-notes/phase-9-singleplayer-validation.md`，整理单人回归矩阵、已知限制和最终验证入口。
- 已更新 `README.md`、`docs/decisions.md`、`docs/build-environment.md`、`docs/mechanics-evidence.md` 和 `docs/v2-roadmap.md`，同步最终展示说明、决策、构建命令、HUD 合同和路线图措辞。
- 构建确认：`C:\sts2\dotnet\dotnet.exe build .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore` 通过，0 warning / 0 error。
- 发布确认：`C:\sts2\dotnet\dotnet.exe publish .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore` 通过。
- 仓库检查：`git diff --check` 通过，仅有 LF/CRLF 提示；`git status --short` 未显示构建产物进入 git。
- Phase 10 继续冻结：只研究多人真实目标与 target-aware 原生预览，不开发正式多人伤害 HUD。

## 阶段状态

| 阶段 | 状态 | 任务 | 完成标准 | 下一步依赖 |
| --- | --- | --- | --- | --- |
| Phase 0 | 已完成 | 仓库初始化 | v2 新仓库、文档、远程 main 已建立 | 无 |
| Phase 1A | 已完成 | Mod 发现与加载验证 | Steam 启动、Mod 列表可见、Loaded 日志确认 | Phase 1B |
| Phase 1B | 已完成 | 单人攻击 HUD 运行时验证 | `🛡 -N` 在单人攻击 Intent 场景正确显示 | Phase 5 |
| Phase 5 | 已完成 | Blockable Incoming Damage 汇总 | 怪物攻击、手牌回合末 blockable DamageVar、第一批 EffectiveBlock 候选已纳入 `🛡 -N` 并完成回归验证 | Phase 6 |
| Phase 6A | 已完成 | Direct HP Loss 固定值 | Beckon、Bad Luck 显示 `♥ -N`，并完成 Steam 运行时矩阵 | Phase 6B |
| Phase 6B | 已完成 | Regret Direct HP Loss | Regret 按当前手牌总数进入 `♥ -N` | Phase 6C |
| Phase 6C | 已完成 | Direct HP Loss 联合运行时验证 | Beckon、Bad Luck、Regret 的 `♥ -N` 完成 Steam 联合验证 | Phase 7 |
| Phase 7 | 已完成 | HP Loss 结果修正机制 | TungstenRod、BeatingRemnant 已按正常游戏范围收口；复数棍子控制台无效状态排除 | Phase 8 |
| Phase 8 | 已完成 | 非 Block 承伤修正机制 | DiamondDiademPower 最小代码接入已构建/发布通过，并完成用户 Steam 运行时验证 | Phase 9 |
| Phase 9 | 已完成 | 单人正式版收口 | 合并 HUD 显示改造及本轮追加小修均已完成代码、文档与用户 Steam 运行时收口 | Phase 10 |
| Phase 10 | 冻结 | 多人研究 | 仅研究多人真实目标与原生 target-aware 伤害预览，证据不足前不做正式多人 HUD | 证据充分后再开启 |

## 任务登记规则

- 每次任务结束必须更新对应 Phase 文件。
- 每次任务结束必须更新 `docs/task-notes/README.md` 的状态表。
- 每次任务结束必须更新本文档的当前快照。
- 每次任务结束必须明确下一步唯一任务。
- 每次任务结束必须写入实际提交 hash。
- 不把代码推测写成运行时验证事实。
