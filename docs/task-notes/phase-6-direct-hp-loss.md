# Phase 6A｜Beckon / Bad Luck Direct HP Loss

## 状态

已完成代码接入与 Steam 运行时验证。

本轮只处理：

- Beckon → `♥ -6`
- Bad Luck → `♥ -13`

本轮不处理 Regret、TungstenRod、BeatingRemnant、DiamondDiadem / DiamondDiademPower、任何新的 `🛡` 来源、多人 HUD、完整回合模拟器或通用 DirectHpLossForecastService。

## shipped 代码确认

| 卡牌 | shipped 类型 | 必须仍在手牌 | 回合结束触发 | HP loss / Unblockable 入口 | 固定数值入口 | 是否被 Block 抵挡 | 运行时状态 |
| --- | --- | ---: | ---: | --- | --- | ---: | --- |
| Beckon | `MegaCrit.Sts2.Core.Models.Cards.Beckon` | 是 | 是 | `OnTurnEndInHand` 调 `CreatureCmd.Damage(..., ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this)` | `HpLossVar(6m)` / `DynamicVars.HpLoss.BaseValue` | 否 | 已验证 |
| Bad Luck | `MegaCrit.Sts2.Core.Models.Cards.BadLuck` | 是 | 是 | `OnTurnEndInHand` 先 `Cmd.Wait(0.25f)`，再调同样的 Unblockable `CreatureCmd.Damage` | `HpLossVar(13m)` / `DynamicVars.HpLoss.BaseValue` | 否 | 已验证 |

## 时序确认

- `CombatManager.DoTurnEnd` 从 `PileType.Hand.GetPile(player).Cards` 收集 `HasTurnEndInHandEffect` 卡牌。
- 对于有 `HasTurnEndInHandEffect` 的卡牌，`DoTurnEnd` 不走同一轮的提前 Ethereal exhaust 分支，而是加入 `turnEndCards`。
- `CardModel.OnTurnEndInHandWrapper` 先把该卡加入 Play pile，再执行 `OnTurnEndInHand`。
- `OnTurnEndInHand` 执行后，如果卡牌带 Ethereal，才 exhaust；否则加入 discard pile。
- 因此：若 Beckon / Bad Luck 在回合结束时仍在手牌，它们会先结算 HP loss，再离开手牌。若它们在回合结束前已因其他流程离开手牌，本 HUD 读取当前手牌，不显示对应 `♥`。
- Bad Luck 的 shipped keywords 是 `Eternal` 和 `Unplayable`；Beckon 本体未声明 Ethereal / Exhaust / Discard 特殊关键字。

## 接入规则

新增 `VerifiedFixedTurnEndHpLossReader`：

- 只读取本机单人玩家当前手牌。
- 只识别 `Beckon` 和 `BadLuck` 两个 shipped 类型。
- 只在卡牌仍在 `PileType.Hand` 且 `HasTurnEndInHandEffect == true` 时计入。
- 只读取匹配卡牌自己的 `HpLossVar`，并要求 Beckon 精确等于 6、Bad Luck 精确等于 13。
- 不扫描所有 `HpLossVar`。
- 不调用 `CreatureCmd.Damage(...)`、命令、RNG、存档或网络入口。

内部目标：

```text
DirectHpLoss = BeckonLoss + BadLuckLoss
```

UI 规则：

```text
DirectHpLoss > 0 → 显示 ♥ -N
DirectHpLoss = 0 → 隐藏 ♥ 行
```

`♥ -N` 与 `🛡 -N` 分行显示，不合并，也不受 Block 影响。

## 刷新链

本轮复用既有刷新链：

- `NHealthBar.SetCreature`
- `NHealthBar.RefreshValues`
- `NHealthBar.SetHpBarContainerSizeWithOffsets`
- `CardPile.InvokeContentsChanged`

因此以下变化应触发 HUD 重算：

- Beckon 进入手牌
- Beckon 离开手牌
- Bad Luck 进入手牌
- Bad Luck 离开手牌
- 两张同时存在

未新增每帧扫描，未新增默认开启的诊断日志。

## Steam 运行时验证

Steam 启动游戏后，用户完成 Phase 6A-RV 验证并确认 Beckon / Bad Luck 的 `♥ -N` 与真实回合结束结算一致。

| 场景 | 预期 | 状态 |
| --- | --- | --- |
| 无 Beckon / Bad Luck | 不显示 `♥` | 已验证 |
| 仅 Beckon 在手牌 | `♥ -6` | 已验证 |
| 仅 Bad Luck 在手牌 | `♥ -13` | 已验证 |
| Beckon + Bad Luck 都在手牌 | `♥ -19` | 已验证 |
| 当前 Block 改变 | `♥` 数值不变 | 已验证 |
| Beckon / Bad Luck 离开手牌 | `♥` 同步减少或隐藏 | 已验证 |
| 与 Burn 共存 | `🛡` 与 `♥` 分开显示 | 已验证 |
| 与敌人攻击共存 | `🛡` 与 `♥` 分开显示 | 已验证 |
| 非战斗 | 两行都隐藏 | 已验证 |

### 具体证据案例

- Steam 运行时截图：`C:\Users\ROG\AppData\Local\Temp\codex-clipboard-2405da0f-af6a-41d9-aea8-addc99785f37.png`（用户提供，未提交）。
- 手牌内容：`霉运`（Bad Luck）与 `灼伤`（Burn）同时在手牌。
- 敌人 Intent：16 攻击。
- 当前 Block：0。
- HUD 显示：`🛡 -18` 与 `♥ -13` 分两行显示。
- 解释：`🛡 -18` = 敌人攻击 16 + Burn 2；`♥ -13` = Bad Luck 固定 Unblockable HP loss。
- 结果：`♥` 未与 `🛡` 合并，未被 Block 影响，未观察到崩溃、错位、漏刷新或重复计算。
- 用户同时确认 Beckon（呼唤）场景成功，`♥ -6` 与真实回合结束结算一致。

## Regret 未接入原因

Regret 的数值依赖手牌数读取时点，且本轮任务明确只验证 Beckon / Bad Luck 的固定值来源。Regret 不在本轮读取，不猜 0，也不显示猜测数值。

## 实际改动文件

- `src/STS2PartyWatchCode/Combat/IncomingDamageRead.cs`
- `src/STS2PartyWatchCode/Combat/LocalIncomingDamageReader.cs`
- `src/STS2PartyWatchCode/Combat/VerifiedFixedTurnEndHpLossReader.cs`
- `src/STS2PartyWatchCode/Forecast/ForecastResult.cs`
- `src/STS2PartyWatchCode/Forecast/LocalDamageForecast.cs`
- `src/STS2PartyWatchCode/Patches/ForecastRefreshPatch.cs`
- `docs/project-state.md`
- `docs/interface-map.md`
- `docs/mechanics-evidence.md`
- `docs/task-notes/README.md`
- `docs/task-notes/phase-6-direct-hp-loss.md`

## 提交记录

- `48fd471e68405573b4d2918752d0dc6aeb1e406f`：`feat: add fixed direct hp loss forecast`
- `6ed186506a52285e692e8507ad69ef84a284d049`：`docs: record beckon and bad luck runtime validation`

## 下一步唯一任务

- Phase 6B：Regret 的手牌数读取时点验证与最小接入
