# Mechanics Evidence

## Carried observations from the v1 research prototype - must be revalidated in v2

- AttackIntent.GetTotalDamage(IEnumerable<Creature> targets, Creature attacker) was observed in v1.
- Single attack example: 8 => total 8.
- Multi-hit example: 4x3 => total 12.
- Single-player examples observed:
- RAW 13, Block 0 => OUT 13.
- RAW 13, Block 5 => OUT 8.
- RAW 13, Block 10 => OUT 3.
- A tested Intangible scene produced native intent values 1x2 plus 1, and RAW 3.

## Phase 1–4 code-confirmed mechanics

- The v2 code calls native `AttackIntent.GetTotalDamage(new[] { localCreature }, enemy)` for every readable enemy `AttackIntent`.
- The v2 code sums readable enemy attack RAW values before subtracting Block.
- The v2 code reads current local `Creature.Block` exactly once per refresh.
- The v2 code calculates `OUT = max(0, RAW - Block)`.
- The v2 HUD only displays `🛡 -N` when OUT is greater than 0.
- Non-attack intents, missing combat state, missing local player, missing local creature, dead local creature, no attack intent, and OUT <= 0 hide the HUD.

## Phase 1–4 runtime validation

- Phase 1A load validation completed through Steam launch.
- Mod list shows `STS2 Party Watch v2`.
- Startup log showed `[STS2 Party Watch] Loaded` and `Loaded 2 mods (2 total)`.
- Noncombat HUD hidden was verified.
- Steam-launched singleplayer combat verified enemy Intent = 9 with current Block = 5 displays `🛡 -4` beside the local player health bar.
- User confirmed the same Intent = 9 test path is correct for Block = 0 displaying `🛡 -9` and Block = 10 hiding the HUD.
- Multiple Block changes refreshed the HUD in the same combat.
- Direct `SlayTheSpire2.exe` launch reached Steam initialization error: `No appID found`; direct exe launch is not a valid runtime validation path.

## v2 validation requirement

Carried observations remain background only. Only the Steam-launched v2 scenes above are considered Phase 1–4 runtime proof.

## Phase 5 blockable incoming damage contract

```text
🛡 -N = max(0, MonsterIntentRaw + HandTurnEndDamageRaw - EffectiveBlock)
EffectiveBlock = CurrentBlock + VerifiedPreAttackBlock
```

| 来源 | 是否进 🛡 | 原因 | 读取方式 | 运行时状态 |
| --- | ---: | --- | --- | --- |
| 敌人 Attack / DeathBlow Intent | 是 | Blockable damage | `AttackIntent.GetTotalDamage(...)` | Phase 1B 已验证普通 Attack Intent |
| Burn 等回合末 `DamageVar` | 是 | `DamageVar` 且无 `ValueProp.Unblockable`，运行时已确认 Burn 走 `🛡` | `CardPile.Get(PileType.Hand, player)` + `DamageVar` + `Hook.ModifyDamage(...)` | Phase 5A+5B 已验证 Burn |
| Beckon | 否 | `HpLossVar` / `ValueProp.Unblockable` | 留给 Phase 6 | 留给 Phase 6 |
| Bad Luck | 否 | `HpLossVar` / `ValueProp.Unblockable` | 留给 Phase 6 | 留给 Phase 6 |
| Regret | 否 | `ValueProp.Unblockable`，数值依赖手牌数 | 留给 Phase 6 | 留给 Phase 6 |
| Frost / 覆甲 / 本轮遗物 Block | 不属于 raw | 属于 `EffectiveBlock` 修正 | 窄范围只读入口 | Phase 5C 已运行时验证 |

## Phase 5A+5B code-confirmed mechanics

- 手牌读取使用 `CardPile.Get(PileType.Hand, player)`。
- 回合末伤害候选通过 `CardTurnEndDamageInspector.DoesTurnEndInHandCallDamage(card)` 确认卡牌 `OnTurnEndInHand` 状态机调用 `CreatureCmd.Damage`。
- 只读取 `card.DynamicVars.Values.OfType<DamageVar>()`，不读取 `HpLossVar`。
- `DamageVar` 若带有 `ValueProp.Unblockable`，不进入 `🛡 -N`。
- `DamageVar` 预览值通过 `Hook.ModifyDamage(...)` 读取，不调用真实结算。
- `CardPile.InvokeContentsChanged` 后会刷新已登记的玩家血条 HUD。

## Phase 5A+5B runtime validation

- Steam 启动后验证 Burn 在手牌中会进入 `🛡 -N` 汇总。
- Intent 9 + Burn 2 的 Blockable raw = 11。
- Intent 9 + Burn 2 + Block 0 显示 `🛡 -11`。
- Intent 9 + Burn 2 + Block 5 显示 `🛡 -6`。
- Intent 9 + Burn 2 + Block 10 显示 `🛡 -1`。
- Burn 手牌变化后 HUD 会刷新。
- Beckon、Bad Luck、Regret 仍保留给 Phase 6 的 `♥ -N`，本轮未实现。

## Phase 5C EffectiveBlock source ledger

| 来源 | 是否本轮接入候选 | 原因 | 读取方式 / 入口 | 运行时状态 |
| --- | ---: | --- | --- | --- |
| Frost | 是 | 回合结束 orb passive trigger 会在手牌回合末伤害前给 Block；`PassiveVal` 包含 Focus 修正 | `player.PlayerCombatState.OrbQueue.Orbs.OfType<FrostOrb>().Sum(o => o.PassiveVal)` | Phase 5C 已验证 |
| 覆甲 | 是 | `PlatingPower.BeforeSideTurnEndEarly` 给 Block，注释说明早于回合末伤害 | `localCreature.GetPower<PlatingPower>()?.Amount` 或等价只读 power 入口 | Phase 5C 已验证 |
| 奥利哈刚 | 是 | 当前 Block 为 0 时在 very early 标记，随后给 Block；检查早于覆甲 | `Orichalcum` 持有状态 + `DynamicVars.Block` + 当前 Block 条件 | Phase 5C 已验证 |
| 假奥利哈刚 | 是 | 逻辑同奥利哈刚，数值为 3 | `FakeOrichalcum` 持有状态 + `DynamicVars.Block` + 当前 Block 条件 | Phase 5C 已验证 |
| 波纹水盆 | 是 | 本回合未打出 Attack 时，`BeforeSideTurnEnd` 给 Block | `RippleBasin` 持有状态 + 当前回合 `CardPlaysFinished` 中是否有本机 Attack | Phase 5C 已验证 |
| 斗篷扣 | 是 | `BeforeSideTurnEnd` 按手牌数给 Block | `CloakClasp` 持有状态 + `PileType.Hand.GetPile(player).Cards.Count` | Phase 5C 已验证 |
| 钨钢棍 | 否，本轮只记录 | `ModifyHpLostAfterOsty` 减 HP loss，不是 Block | `TungstenRod.ModifyHpLostAfterOsty` | 后续补足 |
| 律动残余 | 否，本轮只记录 | 限制本回合 HP loss 上限，不是 Block | `BeatingRemnant.ModifyHpLostAfterOsty` + `DamageReceivedThisTurn` | 后续补足 |
| 钻石头冠 | 否，本轮只记录 | 回合末施加 `DiamondDiademPower`，后续将 powered attack 伤害乘 0.5，不是 Block | `DiamondDiadem.BeforeSideTurnEnd` / `DiamondDiademPower.ModifyDamageMultiplicative` | 后续补足 |

## Phase 5C code-confirmed mechanics

- `IncomingDamageRead` 现在传递 `EffectiveBlock`，预测层仍只做 `max(0, RawDamage - EffectiveBlock)`。
- `VerifiedPreAttackBlockReader` 只读取六个固定候选：Frost、覆甲、奥利哈刚、假奥利哈刚、波纹水盆、斗篷扣。
- Frost 使用 `player.PlayerCombatState.OrbQueue.Orbs.OfType<FrostOrb>()`，读取每个 `FrostOrb.PassiveVal`。
- 覆甲使用 `localCreature.GetPower<PlatingPower>()?.Amount`。
- 奥利哈刚和假奥利哈刚仅在 `Creature.Block == 0` 时计入各自 `BlockVar`。
- 波纹水盆使用 `CombatManager.Instance.History.CardPlaysFinished` 判断本回合本机玩家是否已打出 Attack。
- 斗篷扣使用 `CardPile.Get(PileType.Hand, player).Cards.Count * BlockVar`。
- 任一读取失败会返回 Unknown 并隐藏 HUD，不显示猜测值。
- 未加入通用 Power / Relic / Orb 扫描器；未修改 raw 伤害、Burn 分类或 HUD 文本。

## Phase 5C runtime validation

- Steam 运行时已验证 Frost 可正确进入 `EffectiveBlock`。
- Steam 运行时已验证覆甲可正确进入 `EffectiveBlock`。
- Steam 运行时已验证奥利哈刚可正确进入 `EffectiveBlock`。
- Steam 运行时已验证假奥利哈刚可正确进入 `EffectiveBlock`。
- Steam 运行时已验证波纹水盆可正确进入 `EffectiveBlock`。
- Steam 运行时已验证斗篷扣可正确进入 `EffectiveBlock`。
- Steam 运行时已验证 Phase 5C 后 `🛡 -N` 仍只显示盾牌栏结果，不显示 raw、Block 或来源明细。

## Phase 5D runtime regression

- Steam 运行时已验证基础攻击回归：Intent 9 / Block 0 显示 `🛡 -9`，Block 5 显示 `🛡 -4`，Block 10 隐藏。
- Steam 运行时已验证无攻击 / 无 DamageVar 隐藏，非战斗隐藏。
- Steam 运行时已验证 Burn / DamageVar 回归：Burn 2 / 无攻击 / Block 0 显示 `🛡 -2`，Burn 2 / 无攻击 / Block 2 隐藏。
- Steam 运行时已验证 Intent 9 + Burn 2 + Block 5 显示 `🛡 -6`。
- Steam 运行时已验证 Burn 进入手牌和离开手牌时 HUD 刷新。
- Steam 运行时已验证 Frost、覆甲、奥利哈刚、假奥利哈刚、波纹水盆、斗篷扣各自真实战斗案例通过。
- Steam 运行时已验证多个 `VerifiedPreAttackBlock` 来源共存时不重复计数。
- Steam 运行时已验证当前 Block 已包含的值不会重复加入。
- Steam 运行时已验证 `VerifiedPreAttackBlock` 与 Burn 共存时仍正确。
- Steam 运行时已验证 Block 足以覆盖总伤害时 HUD 隐藏。

## Phase 5 closure classification

| 分类 | 来源 |
| --- | --- |
| 已纳入 `🛡` | AttackIntent / DeathBlow 原生攻击预览、手牌 blockable `DamageVar`、Frost、PlatingPower、Orichalcum、FakeOrichalcum、RippleBasin、CloakClasp |
| 已排除出 `🛡` | `HpLossVar`、`ValueProp.Unblockable`、Beckon、Bad Luck、Regret |
| Phase 6A 已代码接入 `♥` | Beckon / Bad Luck |
| 留给 Phase 6B / 后续 | Regret、TungstenRod、BeatingRemnant |
| 后续伤害修正机制 | DiamondDiadem / DiamondDiademPower |

## Phase 6A direct HP loss contract

```text
♥ -N = DirectHpLoss
DirectHpLoss = BeckonLoss + BadLuckLoss
```

`♥ -N` 不与 `🛡 -N` 合并，不受当前 Block、Frost、覆甲或遗物 Block 影响。

| 来源 | 是否进入 `♥` | 原因 | 读取方式 | 运行时状态 |
| --- | ---: | --- | --- | --- |
| Beckon | 是 | shipped `OnTurnEndInHand` 从 `HpLossVar(6m)` 读取并以 `ValueProp.Unblockable` 调 `CreatureCmd.Damage` | 当前手牌中精确类型 `Beckon` + `HpLossVar.BaseValue == 6` | 已验证 |
| Bad Luck | 是 | shipped `OnTurnEndInHand` 从 `HpLossVar(13m)` 读取并以 `ValueProp.Unblockable` 调 `CreatureCmd.Damage` | 当前手牌中精确类型 `BadLuck` + `HpLossVar.BaseValue == 13` | 已验证 |
| Regret | 否 | 数值依赖手牌数读取时点，本轮未验证 | 留给 Phase 6B | 尚未验证 |
| TungstenRod | 否 | 修改实际 HP loss 结果，不是本轮固定来源 | 留给后续 | 尚未验证 |
| BeatingRemnant | 否 | 限制每回合 HP loss 上限，不是本轮固定来源 | 留给后续 | 尚未验证 |

## Phase 6A code-confirmed mechanics

- Beckon 的 shipped 类型是 `MegaCrit.Sts2.Core.Models.Cards.Beckon`。
- Beckon `HasTurnEndInHandEffect == true`，`CanonicalVars` 为 `HpLossVar(6m)`。
- Beckon `OnTurnEndInHand` 调用 `CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this)`。
- Bad Luck 的 shipped 类型是 `MegaCrit.Sts2.Core.Models.Cards.BadLuck`。
- Bad Luck `CanonicalKeywords` 为 `Eternal` 和 `Unplayable`，`HasTurnEndInHandEffect == true`，`CanonicalVars` 为 `HpLossVar(13m)`。
- Bad Luck `OnTurnEndInHand` 先等待 `0.25f`，再以同样的 Unblockable props 调用 `CreatureCmd.Damage(...)`。
- `CombatManager.DoTurnEnd` 从当前 `PileType.Hand` 收集 `HasTurnEndInHandEffect` 卡牌；这些卡不会进入同一段提前 Ethereal exhaust 分支。
- `CardModel.OnTurnEndInHandWrapper` 先将卡加入 Play pile，执行 `OnTurnEndInHand`，之后若有 Ethereal 才 exhaust，否则加入 discard pile。
- 因此 Beckon / Bad Luck 必须在回合结束读取时仍在手牌；若已离开手牌，则不会被本轮 `♥` 预测计入。
- 本实现只读取匹配类型自己的 `HpLossVar`，不扫描所有 `HpLossVar`，不调用真实伤害结算。

## Phase 6A runtime validation

- Steam 运行时已验证 Beckon 在手牌中显示 `♥ -6`，且与真实回合结束结算一致。
- Steam 运行时已验证 Bad Luck 在手牌中显示 `♥ -13`，且与真实回合结束结算一致。
- Steam 运行时已验证 Beckon + Bad Luck 同时在手牌时显示 `♥ -19`。
- Steam 运行时已验证当前 Block 改变不影响 `♥` 数值。
- Steam 运行时已验证 Beckon / Bad Luck 离开手牌后，`♥` 同步减少或隐藏。
- Steam 运行时已验证无 Beckon / Bad Luck 时不显示 `♥`，非战斗两行隐藏。
- Steam 运行时截图案例：手牌含 Bad Luck（`霉运`）与 Burn（`灼伤`），敌人 Intent 16，当前 Block 0，HUD 显示 `🛡 -18` 与 `♥ -13` 分两行；`🛡 -18` = 敌人攻击 16 + Burn 2，`♥ -13` = Bad Luck direct HP loss。
- 截图证据位置：`C:\Users\ROG\AppData\Local\Temp\codex-clipboard-2405da0f-af6a-41d9-aea8-addc99785f37.png`（用户提供，未提交）。
- 本轮未观察到崩溃、错位、漏刷新或重复计算。
