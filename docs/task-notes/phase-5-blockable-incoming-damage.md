# Phase 5｜Blockable Incoming Damage 汇总

## 状态

已完成

## 目标

把 `🛡 -N` 从“怪物攻击穿透当前 Block”升级为“所有会被 Block 抵挡的本回合预计伤害穿透有效 Block”。

```text
🛡 -N = max(0, MonsterIntentRaw + HandTurnEndDamageRaw - EffectiveBlock)
EffectiveBlock = CurrentBlock + VerifiedPreAttackBlock
```

本次 Phase 5A+5B 只实现：

- `MonsterIntentRaw`
- 手牌回合末 `DamageVar` 的 `HandTurnEndDamageRaw`
- 手牌变化后刷新 HUD

Frost / 覆甲 / 奥利哈刚 / 假奥利哈刚 / 波纹水盆 / 斗篷扣等 `VerifiedPreAttackBlock` 已完成最小代码接入与 Steam 运行时验证。

## 本任务允许做的事

- 保持已验证的怪物攻击 Intent 读取路径。
- 读取本机玩家手牌。
- 只统计在手牌中、回合末会调用 `CreatureCmd.Damage`、且持有 blockable `DamageVar` 的卡。
- 对 `DamageVar` 调用只读 `Hook.ModifyDamage(...)` 取得原生修正后的预览值。
- 在 `CardPile.InvokeContentsChanged` 后刷新已登记的 HUD。
- 更新 Phase 5 规则台账和接口文档。

## 本任务禁止做的事

- 调用 `CreatureCmd.Damage(...)`、`Creature.DamageBlockInternal(...)` 或任何真实结算入口。
- 把 `HpLossVar` 纳入 `🛡 -N`。
- 把 `ValueProp.Unblockable` 来源纳入 `🛡 -N`。
- 实现 `♥ -N`。
- 实现 Frost / 覆甲 / 遗物 Block 的正式接入，除非对应来源已完成本 Phase 的代码确认与运行时验证。
- 开发多人 HUD。
- 提交 DLL、PDB、PCK、logs、publish 输出、NuGet 缓存或游戏目录文件。

## 完成标准

- `Intent 9 + Burn 2 + Block 5` 可显示 `🛡 -6`。
- `Burn 2 + Block 0` 可显示 `🛡 -2`。
- `Burn 2 + Block 2` 隐藏。
- Beckon / Bad Luck / Regret 不进入 `🛡 -N`。
- 手牌 Burn 增减后 HUD 会刷新。
- 未验证来源保持排除或记录为未验证。

## 已验证事实

- Phase 1B 已验证：Intent 9 / Block 0、5、10 的怪物攻击基线可显示并刷新。
- Steam 运行时已验证：Intent 9 + Burn 2 进入 `🛡` 汇总，Blockable raw = 11。
- Steam 运行时已验证：Intent 9 + Burn 2 + Block 0 显示 `🛡 -11`。
- Steam 运行时已验证：Intent 9 + Burn 2 + Block 5 显示 `🛡 -6`。
- Steam 运行时已验证：Intent 9 + Burn 2 + Block 10 显示 `🛡 -1`。
- Steam 运行时已验证：Burn 在手牌中参与盾牌栏计算，手牌变化后 HUD 刷新。
- Steam 运行时已验证：Frost、覆甲、奥利哈刚、假奥利哈刚、波纹水盆、斗篷扣均可进入 `EffectiveBlock` 并正确影响 `🛡 -N`。
- Phase 5D Steam 回归已验证：Intent 9 / Block 0 显示 `🛡 -9`，Block 5 显示 `🛡 -4`，Block 10 隐藏。
- Phase 5D Steam 回归已验证：无攻击 / 无 DamageVar 隐藏，非战斗隐藏。
- Phase 5D Steam 回归已验证：Burn 2 / 无攻击 / Block 0 显示 `🛡 -2`，Burn 2 / 无攻击 / Block 2 隐藏。
- Phase 5D Steam 回归已验证：Intent 9 + Burn 2 + Block 5 显示 `🛡 -6`。
- Phase 5D Steam 回归已验证：Burn 进入或离开手牌后 HUD 刷新。
- Phase 5D Steam 回归已验证：多个 `VerifiedPreAttackBlock` 来源共存时不重复计数，当前 Block 已包含值不重复加入，与 Burn 共存时仍正确，Block 足以覆盖总伤害时 HUD 隐藏。

## 仅代码确认、尚未运行时验证

- Burn 以外的其他回合末 blockable `DamageVar` 尚未逐个运行时验证。

## 未解决问题

- Beckon / Bad Luck / Regret 的 `♥ -N` 尚未实现。
- 钨钢棍（`TungstenRod`）、律动残余（`BeatingRemnant`）、钻石头冠（`DiamondDiadem` / `DiamondDiademPower`）会影响伤害结果，但不在 Phase 5C 第一批接入范围；后续单独补足。

## Phase 5D 机制缺口收口

| 分类 | 来源 |
| --- | --- |
| 已纳入 `🛡` | AttackIntent / DeathBlow、手牌 blockable `DamageVar`、Frost、PlatingPower、Orichalcum、FakeOrichalcum、RippleBasin、CloakClasp |
| 已排除 | `HpLossVar`、`ValueProp.Unblockable`、Beckon、Bad Luck、Regret |
| 留给 Phase 6 | Beckon / Bad Luck / Regret、TungstenRod、BeatingRemnant |
| 后续伤害修正机制 | DiamondDiadem / DiamondDiademPower |

## Phase 5C 第一批 EffectiveBlock 候选

| 来源 | 类型 / 入口 | 是否本轮做 | 代码确认的时点 / 规则 | 运行时状态 |
| --- | --- | ---: | --- | --- |
| Frost | `player.PlayerCombatState.OrbQueue.Orbs` 中的 `FrostOrb.PassiveVal` | 是 | `FrostOrb.BeforeTurnEndOrbTrigger` 调 `CreatureCmd.GainBlock`；`PassiveVal` 已走 `Hook.ModifyOrbValue`，包含 Focus 影响 | 已验证 |
| 覆甲 | `PlatingPower.Amount` | 是 | `BeforeSideTurnEndEarly` 给 `Amount` Block，注释说明早于回合末伤害 | 已验证 |
| 奥利哈刚 | `Orichalcum` + `DynamicVars.Block` | 是 | `BeforeSideTurnEndVeryEarly` 在当前 Block 为 0 时标记，`BeforeSideTurnEnd` 给 6 Block；检查早于覆甲 | 已验证 |
| 假奥利哈刚 | `FakeOrichalcum` + `DynamicVars.Block` | 是 | 同奥利哈刚，给 3 Block | 已验证 |
| 波纹水盆 | `RippleBasin` + `DynamicVars.Block` + 本回合 Attack 历史 | 是 | `BeforeSideTurnEnd` 若本回合未打出 Attack，则给 4 Block | 已验证 |
| 斗篷扣 | `CloakClasp` + `DynamicVars.Block` + 手牌数 | 是 | `BeforeSideTurnEnd` 按手牌数量给 Block | 已验证 |

## Phase 5 后续遗物补足候选

| 来源 | 类型 / 入口 | 为什么不进本轮 | 后续方向 |
| --- | --- | --- | --- |
| 钨钢棍 | `TungstenRod.ModifyHpLostAfterOsty` | 修改 HP loss，不是获得 Block | Phase 6 / HP loss 结果修正 |
| 律动残余 | `BeatingRemnant.ModifyHpLostAfterOsty` | 限制每回合 HP loss 上限，不是获得 Block | Phase 6 / HP loss cap |
| 钻石头冠 | `DiamondDiadem` 施加 `DiamondDiademPower` | 回合末施加伤害减半 Power，不是直接获得 Block | 后续伤害修正机制补足 |

## 实际改动文件

- `src/STS2PartyWatchCode/Combat/IncomingDamageRead.cs`
- `src/STS2PartyWatchCode/Combat/LocalIncomingDamageReader.cs`
- `src/STS2PartyWatchCode/Combat/VerifiedPreAttackBlockReader.cs`
- `src/STS2PartyWatchCode/Forecast/LocalDamageForecast.cs`
- `docs/project-state.md`
- `docs/interface-map.md`
- `docs/mechanics-evidence.md`
- `docs/task-notes/README.md`
- `docs/task-notes/phase-5-blockable-incoming-damage.md`

## 下一步唯一任务

- Phase 6：Beckon / Bad Luck / Regret 的 ♥ -N 时序验证与最小接入

## 预期提交文件

- `src/STS2PartyWatchCode/Combat/IncomingDamageRead.cs`
- `src/STS2PartyWatchCode/Combat/LocalIncomingDamageReader.cs`
- `src/STS2PartyWatchCode/Combat/VerifiedPreAttackBlockReader.cs`
- `src/STS2PartyWatchCode/Forecast/LocalDamageForecast.cs`
- `docs/project-state.md`
- `docs/interface-map.md`
- `docs/mechanics-evidence.md`
- `docs/task-notes/README.md`
- `docs/task-notes/phase-5-blockable-incoming-damage.md`

## 提交记录

- `b170994a58fa21b18c7a37de01db147a1df15746`：`feat: include hand turn-end damage in shield forecast`
- `a08cf59eeae9ce0d2d80b204339b80bfa9188815`：`feat: account for verified pre-attack block`
- `5d9d4cdece14a9244efbc26fe4dd0e5173554a38`：`docs: close phase 5 blockable damage validation`
