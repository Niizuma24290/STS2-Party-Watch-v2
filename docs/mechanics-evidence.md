# Mechanics Evidence

## 2026-07-02 Current mechanism ledger

This section is the reconciliation index for the currently implemented mod. Older phase notes below remain historical evidence.

| Mechanism | Current support | Affects | Core native evidence or hook | Current implementation entry | Runtime verification status | Known unsupported boundary |
| --- | --- | --- | --- | --- | --- | --- |
| AttackIntent / DeathBlow | Implemented | `🛡`, total `-N` | `AttackIntent.GetTotalDamage(...)`; `GetSingleDamage(...)` when per-hit granularity is needed | `LocalIncomingDamageReader.ReadKnown(...)` | RuntimeVerified for ordinary attack and regressions | Unreadable intent returns Hidden/Unknown. |
| Burn-style hand turn-end `DamageVar` | Implemented | `🛡`, total `-N` | `HasTurnEndInHandEffect`, `CreatureCmd.Damage`, `DamageVar`, `Hook.ModifyDamage(...)` | `CardTurnEndDamageInspector`, `TryReadHandTurnEndDamage`, `TryReadOrderedHandTurnEndEvents` | RuntimeVerified for Burn and later hand-damage paths | `HpLossVar` and unblockable props do not enter `🛡`. |
| Frost | Implemented | EffectiveBlock, `🛡`, total `-N` | `FrostOrb.PassiveVal` | `VerifiedPreAttackBlockReader.ReadFrostBlock(...)` | RuntimeVerified | Does not simulate evoke. |
| PlatingPower | Implemented | EffectiveBlock, `🛡`, total `-N` | `PlatingPower.Amount` | `VerifiedPreAttackBlockReader.ReadPlatingBlock(...)` | RuntimeVerified | Reads current local player Power only. |
| Orichalcum / FakeOrichalcum | Implemented, Conditional | EffectiveBlock, `🛡`, total `-N` | relic `BlockVar`, current Block condition | `ReadOrichalcumBlock(...)`, `ReadFakeOrichalcumBlock(...)` | RuntimeVerified | Counted only when current Block is 0. |
| RippleBasin | Implemented, Conditional | EffectiveBlock, `🛡`, total `-N` | `CardPlaysFinished`, pending `StampedePower` attack timing | `ReadRippleBasinBlock(...)` | RuntimeVerified, including Stampede correction | Does not simulate arbitrary auto-play target failures. |
| CloakClasp | Implemented | EffectiveBlock, `🛡`, total `-N` | current hand count and relic `BlockVar` | `ReadCloakClaspBlock(...)` | RuntimeVerified | Current hand only. |
| Beckon | Implemented | `♥`, total `-N` | `HpLossVar(6)`, unblockable turn-end hand effect | `VerifiedFixedTurnEndHpLossReader` | RuntimeVerified | Only exact supported card path. |
| Bad Luck | Implemented | `♥`, total `-N` | `HpLossVar(13)`, unblockable turn-end hand effect | `VerifiedFixedTurnEndHpLossReader` | RuntimeVerified | Only exact supported card path. |
| Regret | Implemented | `♥`, total `-N` | hand count captured before turn-end effect | `VerifiedFixedTurnEndHpLossReader` | RuntimeVerified | Uses current public hand count, not private fields. |
| ConstrictPower / DisintegrationPower | Implemented | `🛡`, total `-N` | turn-end Power self-damage through blockable non-card damage props | `VerifiedTurnEndPowerDamageReader` | RuntimeVerified | No generic Power damage scanner. |
| IntangiblePower on local player | Implemented, Conditional | verified HP-loss result chain | `ModifyDamageCap`, `ModifyHpLostAfterOsty` | `VerifiedHpLossResultModifier.Apply(...)` | RuntimeVerified for supported direct HP-loss events | Aggregate direct HP loss with Intangible is unsupported. |
| TungstenRod | Implemented, Conditional | verified HP-loss result chain | `HpLossReduction` dynamic var | `VerifiedHpLossResultModifier.Apply(...)` | Code evidence and historical notes; current order still needs subscription/runtime closure where noted | Requires single-event granularity. Aggregate enemy HP loss returns Unknown. |
| BeatingRemnant | Implemented, Conditional | verified HP-loss result chain | `MaxHpLoss` dynamic var and turn budget | `ObservedHpLossBudgetTracker`, `VerifiedHpLossResultModifier.Apply(...)` | Code evidence and historical notes; current order still needs subscription/runtime closure where noted | Requires ordered verified events and non-negative remaining budget. |
| Diamond Diadem / DiamondDiademPower | Implemented, Conditional | enemy attack damage, `🛡`, total `-N` | `DiamondDiademPower.ModifyDamageMultiplicative`, card threshold | `VerifiedEnemyDamageModifier.ApplyDiamondDiadem(...)` | RuntimeVerified for supported path | Aggregate enemy damage with per-hit rounding unknown returns Unknown. |
| Ordinary Poison pre-action survival | Implemented, Conditional | enemy AttackIntent inclusion, total `-N` | `PoisonPower.AfterSideTurnStart`, `TriggerCount`, enemy action after side-turn start | `EnemyPreActionSurvivalPreview`, `PoisonTickPreview` | Code/build/publish evidence;专项 Steam matrix not fully backfilled | HardToKill, Slippery, enemy Intangible, TestSubject/Adaptable, Hatch/ToughEgg, HardenedShell/SewerClam unsupported. |
| Local HUD in multiplayer | Implemented, Conditional | display only | local health bar / local player identity | `PartyWatchHudVisibilityPolicy`, `ForecastRefreshPatch`, `PartyWatchUiSettings.ShowLocalHudInMultiplayer` | Workshop subscription runtime evidence for local `-6` HUD | Not teammate HUD, not shared HUD, no network behavior. |

## Status vocabulary

- `Implemented`: current repository code exists.
- `RuntimeVerified`: task notes or commits record Steam / Workshop runtime verification.
- `Conditional`: supported only inside the named evidence boundary.
- `Unsupported`: hidden or returned Unknown; not claimed as working.

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
| 波纹水盆 | 是 | 本回合未打出 Attack 时，`BeforeSideTurnEnd` 给 Block；若 `StampedePower` 将在 `AutoPostPlay` 阶段先自动打出 Attack，则水盆不会给 Block | `RippleBasin` 持有状态 + 当前回合 `CardPlaysFinished` 中是否有本机 Attack + 窄范围 pending `StampedePower` Attack 判断 | Phase 5C 已验证基础水盆；水盆 + 惊涛组合已通过用户 Steam 验证 |
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
- 波纹水盆使用 `CombatManager.Instance.History.CardPlaysFinished` 判断本回合本机玩家是否已打出 Attack；并额外按原生时序处理 pending `StampedePower`：`AutoPostPlay` 阶段会先于 `RippleBasin.BeforeSideTurnEnd`，若惊涛持有者手牌中存在非 `Unplayable` Attack，则预测其会先自动打出 Attack，水盆不再计入预期 Block。
- 用户 Steam 运行时已验证水盆 + 惊涛 + 手牌 Attack 场景：HUD 不再把水盆 4 Block 加入预期，符合原生结算顺序。
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
| Phase 6A 已代码接入并运行时验证 `♥` | Beckon / Bad Luck |
| Phase 6B 已代码接入并运行时验证 `♥` | Regret |
| 留给后续 | TungstenRod、BeatingRemnant |
| 后续伤害修正机制 | DiamondDiadem / DiamondDiademPower |

## Phase 6 direct HP loss contract

```text
♥ -N = DirectHpLoss
DirectHpLoss = BeckonLoss + BadLuckLoss + RegretLoss
RegretLoss = 当前手牌总数 × 当前手牌中的 Regret 数量
```

`♥ -N` 不与 `🛡 -N` 合并，不受当前 Block、Frost、覆甲或遗物 Block 影响。

| 来源 | 是否进入 `♥` | 原因 | 读取方式 | 运行时状态 |
| --- | ---: | --- | --- | --- |
| Beckon | 是 | shipped `OnTurnEndInHand` 从 `HpLossVar(6m)` 读取并以 `ValueProp.Unblockable` 调 `CreatureCmd.Damage` | 当前手牌中精确类型 `Beckon` + `HpLossVar.BaseValue == 6` | 已验证 |
| Bad Luck | 是 | shipped `OnTurnEndInHand` 从 `HpLossVar(13m)` 读取并以 `ValueProp.Unblockable` 调 `CreatureCmd.Damage` | 当前手牌中精确类型 `BadLuck` + `HpLossVar.BaseValue == 13` | 已验证 |
| Regret | 是 | shipped `BeforeSideTurnEnd` 记录当前手牌数，`OnTurnEndInHand` 以该值造成 Unblockable HP loss | 当前手牌总数 × 当前手牌中的 Regret 数量 | 已验证 |
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

## Phase 6B code-confirmed mechanics

- Regret 的 shipped 类型是 `MegaCrit.Sts2.Core.Models.Cards.Regret`。
- Regret `CanonicalKeywords` 为 `Unplayable`，`HasTurnEndInHandEffect == true`。
- Regret 在 `BeforeSideTurnEnd` 中，若自身仍在 `PileType.Hand`，记录 `base.Pile.Cards.Count`。
- Regret 在 `OnTurnEndInHand` 中调用 `CreatureCmd.Damage(choiceContext, Owner.Creature, CardsInHand, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this)`，随后清零内部 `CardsInHand`。
- 本实现不读取 Regret 私有字段 `_cardsInHand`，而是使用同一公开来源 `CardPile.Get(PileType.Hand, player).Cards.Count` 预测将被写入的手牌数。
- 多张 Regret 同时在手牌时，每张 Regret 各贡献一次当前手牌总数。
- Regret 进入 `DirectHpLoss` 与 `♥ -N`，不进入 `🛡 -N`，不受 Block 影响。

## Phase 6B runtime validation

- Steam 运行时已验证 Regret 可按当前手牌总数进入 `♥ -N`。
- Steam 运行时已验证 Regret 的贡献不受当前 Block 影响，不进入 `🛡 -N`。

## Phase 6C runtime validation

- Steam 运行时联合验证已完成：Beckon、Bad Luck、Regret 可共同进入 `DirectHpLoss` 与 `♥ -N`。
- Steam 运行时已验证 `♥ -N` 与 `🛡 -N` 分行显示，不合并。

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

## Phase 9 single-player release contract

最终单人 HUD 默认只显示总预计失血：

```text
-{OutDamage + DirectHpLoss}
```

- `-N`：只由当前已有可信 `ForecastResult.OutDamage` 与 `ForecastResult.DirectHpLoss` 相加得到，不重新读取游戏状态，不新增预测算法。
- `🛡 N` / `♥ N`：仅作为高级明细显示选项；默认关闭。其数值仍分别来自已验证的 blockable 未来承伤与 direct HP loss。

隐藏 / Unknown 规则：

- 非战斗、非本机玩家、多人、无可证明伤害、最终输出为 0 时隐藏。
- blockable 伤害不可证明但 direct HP loss 可证明时，总预计失血等于该可信 direct HP loss；高级明细打开时只显示 `♥ N`。
- 需要 single-hit 粒度但只能读到聚合值的场景，不把未知 blockable 值计入总值；高级明细打开时也不显示猜测后的 `🛡`。
- 不显示 raw damage、Block、来源明细、诊断状态或第三种正式 HUD 行。
## Constrict / Disintegration Power damage code evidence

- `SlitheringStrangler.ConstrictMove` applies `ConstrictPower`.
- `ConstrictPower.AfterSideTurnEnd` calls `CreatureCmd.Damage(owner, Amount, DamageProps.nonCardUnpowered, owner, null)`.
- `KnowledgeDemon` can create the `Disintegration` choice; `Disintegration.OnChosen` applies `DisintegrationPower`.
- `DisintegrationPower.AfterSideTurnEndLate` calls `CreatureCmd.Damage(owner, Amount, DamageProps.nonCardUnpowered, owner, null)`.
- Party Watch treats both as verified blockable `🛡` sources. User Steam runtime validation has confirmed the basic HUD forecast path.

## IntangiblePower HP-loss result evidence

- 本地 `sts2.xml` 记录 `ModifyDamageHookType.Cap` 会调用 `AbstractModel.ModifyDamageCap(...)`，并点名 `IntangiblePower` 作为 damage-capping 示例。
- 本地 `sts2.xml` 记录 `IntangiblePower.ModifyDamageCap(...)` caps damage received at 1；同一说明写明 HP loss 逻辑由 `IntangiblePower.ModifyHpLostAfterOsty(...)` 处理，damage cap 侧的重复逻辑用于 block loss 与 targeted attack preview。
- 本地 `sts2.xml` 记录 `Hook.ModifyHpLost(...)` 在 damage 扣除 Block 后运行 HP-loss-modification hooks；无 redirection 的 preview 调用可使用 `HpLossHookPhase.All`。
- 用户实测规则确认：无实体会让每个独立伤害 / HP loss 事件先变为 1；Block 可抵消该 1；多段攻击按每 hit 1 计算；多个 Burn / 腐朽 / Constrict 等独立事件各自为 1。
- 用户实测规则确认：Beckon、Bad Luck、Regret 这类 direct HP loss 也分别变为每事件 1。
- 用户实测规则确认：与 `TungstenRod` / `BeatingRemnant` 共存时，无实体先把事件压到 1，再进入棍子减免或残余预算。
- Party Watch 当前实现只在已有逐事件 HP loss 结果修正链中接入 `IntangiblePower`：blockable damage 仍信任原生 `AttackIntent` / `Hook.ModifyDamage` 预览；direct HP loss 使用已验证单事件粒度先按无实体压到 1，再应用 `TungstenRod` / `BeatingRemnant`。
- 若 direct HP loss 失去单事件粒度，Party Watch 不把聚合值猜测压成 1，改为 Unknown。

## TungstenRod / BeatingRemnant forecast order

- Party Watch 的 HP loss 结果修正预测顺序固定为：`IntangiblePower` -> `TungstenRod` -> `BeatingRemnant`。
- 本轮仅调整 Party Watch 预测顺序，让 `TungstenRod` 早于 `BeatingRemnant`；真实游戏原生结算顺序仍需 Steam 运行时验证。
- 若未来总 HP loss 大于心脏剩余预算，HUD 将先应用棍子减免，再用心脏剩余预算封顶。

## Phase 9B Poison pre-action survival evidence

游戏版本证据：

- 程序集：`C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\sts2.dll`
- XML 文档：`C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\sts2.xml`
- 文件时间：`2026-06-19 18:57`
- 程序集版本：`sts2, Version=0.1.0.0`

| 机制 | 类 / 方法 / 字段 | 原生结算阶段 | 作用范围 | 是否影响 Poison | 与 Poison 的顺序 | 是否可被当前只读数据准确预览 | Party Watch 接入结论 |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Poison 基础 tick | `MegaCrit.Sts2.Core.Models.Powers.PoisonPower.AfterSideTurnStart`；`PoisonPower.TriggerCount`；`PoisonPower.CalculateTotalDamageNextTurn` | Enemy side `Hook.AfterSideTurnStart`，早于 `CombatManager.ExecuteEnemyTurn` | Power owner 自身 | 是 | 敌方行动前；每 tick 用当前 `PoisonPower.Amount`，若 owner 仍活着才 `PowerCmd.Decrement(this)` | 普通敌人、无特殊 damage/HP-loss/lifecycle power 时可预览 | 已接入：按单个 enemy instance 预览 Poison 致死，确定死于行动前时只排除该实例当前 Attack Intent |
| 敌方行动取消 | `CombatManager.StartTurn`；`CombatManager.ExecuteEnemyTurn`；`CreatureCmd.Kill`；`Creature.IsAlive` / `CombatState.ContainsCreature(enemy)` | Enemy side start 后才执行敌人行动 | 每个原生 `Creature` 敌人对象 | 间接影响 | Poison death 发生在 `ExecuteEnemyTurn` 前；已移除敌人不会执行 `PerformIntent` / `TakeTurn` | 普通死亡移除可由 `CurrentHp`、`IsAlive`、`CurrentPoison` 和 unsupported 门判断 | 已接入普通场景；特殊死亡阻止/复活/阶段转换保持 Unknown |
| Accelerant | `MegaCrit.Sts2.Core.Models.Powers.AccelerantPower`；`PoisonPower.TriggerCount` | PoisonPower 自己读取 opponent 的 Accelerant 层数 | Poison owner 的活着 opponents | 是 | 同一 Poison tick 阶段内增加触发次数，最大不超过当前 Poison 层数 | 单人本机战斗可读 opponents 的 `AccelerantPower.Amount` | 已接入为只读状态；不重放 `Accelerant` 卡或任何卡牌流程 |
| 触媒 / 卡牌重放 | 当前类型列表未出现 `Catalyst`；当前 Poison 相关卡牌包括 `Accelerant`、`DeadlyPoison`、`PoisonedStab` | 卡牌打出时，不属于本预览 | 卡牌目标 / 命令队列 | 可能提前改变 Poison | 在预览读取前已经生效或未生效 | 不能安全重放，且任务禁止重放 | 不接入：只读取结束回合时已经存在的 `PoisonPower.Amount` / `AccelerantPower.Amount` |
| 单次伤害上限 9 | `MegaCrit.Sts2.Core.Models.Monsters.Exoskeleton.AfterAddedToRoom` 施加 `HardToKillPower(9)`；`HardToKillPower.ModifyDamageCap` | `Hook.ModifyDamage` cap 阶段 | Power owner 收到的 damage | 是，Poison 走 `CreatureCmd.Damage` / `Hook.ModifyDamage` | Poison HP loss 前的 damage cap | 类名和 hook 已确认，但本轮未验证完整 Poison 致死/行动取消矩阵 | 未接入；敌人有 `HardToKillPower` 且有 Poison 时返回 Unknown |
| Slippery | `MegaCrit.Sts2.Core.Models.Powers.SlipperyPower.ModifyHpLostAfterOsty`；`AfterDamageReceived` | `Hook.ModifyHpLost` AfterOsty；damage received 后消耗层数 | Power owner 的 HP loss | 是 | Poison damage 扣 Block 后进入 HP loss 修改；每次实际 damage received 后 decrement | 规则已定位，但逐 tick 消耗和多触发矩阵未运行时验证 | 未接入；敌人有 `SlipperyPower` 且有 Poison 时返回 Unknown |
| Intangible | `MegaCrit.Sts2.Core.Models.Powers.IntangiblePower.ModifyDamageCap`；`ModifyHpLostAfterOsty` | damage cap 与 HP loss AfterOsty | Power owner | 是 | Poison 先经 damage cap，再经 HP loss cap | 已知会影响 Poison，但敌方 Poison 致死矩阵未验证 | 未接入敌人 Poison 预览；敌人有 `IntangiblePower` 且有 Poison 时返回 Unknown |
| TestSubject 阶段 / 复活 | `MegaCrit.Sts2.Core.Models.Monsters.TestSubject`；`AdaptablePower.AfterDeath`；`AdaptablePower.ShouldCreatureBeRemovedFromCombatAfterDeath`; `RESPAWN_MOVE` | death hook 后进入 respawn move / revive | TestSubject enemy | 是，影响“是否真的不行动”结论 | Poison 可触发 death，但 `AdaptablePower` 阻止移除并安排复活阶段 | 当前无法只靠普通 HP<=0 判断其本次行动结论 | 未接入；`TestSubject` / `AdaptablePower` 返回 Unknown |
| ToughEgg hatch lifecycle | `MegaCrit.Sts2.Core.Models.Monsters.ToughEgg`；`HatchPower.AfterSideTurnEnd`；`HATCH_MOVE` / `SetMaxAndCurrentHp` | enemy move / side turn end | ToughEgg enemy | 可能影响生命周期 | 不属于普通死亡模型 | 当前未验证 Poison 致死与 hatch 状态组合 | 未接入；`ToughEgg` / `HatchPower` 返回 Unknown |

普通 Poison 接入所需字段：

- stable enemy instance identity：优先 `Creature.CombatId`；没有时保留原生 `Creature` object reference，并只在同一快照内使用列表 index。
- display name：`Creature.Name`。
- native class / type：`enemy.Monster.GetType().FullName`。
- current HP：`Creature.CurrentHp`。
- current Poison：`enemy.GetPower<PoisonPower>()?.Amount`。
- current Intent contribution：该 enemy instance 当前 `AttackIntent.GetTotalDamage(...)` 合计。
- relevant Powers / DynamicVars：当前记录 `PoisonPower`、`AccelerantPower`、`HardToKillPower`、`SlipperyPower`、`IntangiblePower`、`AdaptablePower`、`HatchPower`、`NemesisPower` 的类型与 amount。
- phase / death lifecycle state：当前 `Creature.IsAlive` / `Creature.IsDead`，并通过 unsupported 门排除已知阶段/复活类。

### 2026-07-02 native poison bar follow-up

用户提供截图并要求记录原生毒血条相关机制；以下为本地 `sts2.dll` 反编译摘要，不提交反编译源码。

- `PoisonPower.CalculateTotalDamageNextTurn()` 是原生毒血条使用的模型层总毒伤入口：
  - `TriggerCount = min(PoisonPower.Amount, 1 + alive opponents AccelerantPower amount sum)`。
  - 每段用 `PoisonPower.Amount - i`。
  - 每段调用 `Hook.ModifyDamage(..., ValueProp.Unblockable | ValueProp.Unpowered, ModifyDamageHookType.All, CardPreviewMode.None, ...)`。
  - 返回所有段 damage 的整数总和。
- `NHealthBar.RefreshForeground()` 读取 `creature.GetPower<PoisonPower>()?.CalculateTotalDamageNextTurn() ?? 0`，再设置 `_poisonForeground` 显示毒血条。
- `NHealthBar.IsPoisonLethal(int poisonDamage)` 只是 UI 层判断：`poisonDamage > 0 && creature.HasPower<PoisonPower>() && poisonDamage >= creature.CurrentHp`。
- `_poisonForeground` 是 UI Control；它只表达毒预览宽度，不是结算事件，也不提供 enemy Intent 取消结论。

更新后的特殊敌人证据与接入结论：

| 机制 | 类 / 方法 / 字段 | 原生结算 / 预览证据 | 是否影响 Poison | Party Watch 接入结论 |
| --- | --- | --- | --- | --- |
| Accelerant / 触发次数 | `MegaCrit.Sts2.Core.Models.Cards.Accelerant`；`AccelerantPower`；`PoisonPower.TriggerCount` | `Accelerant` 是 Rare Power，基础施加 1 层 `AccelerantPower`，升级多 1 层；Poison owner 从 alive opponents 读取 `AccelerantPower` 总层数 | 是，增加 Poison tick 段数，最大不超过当前 Poison 层数 | 只读取已生效 `AccelerantPower.Amount`；不重放卡牌。当前类型列表未发现 `Catalyst` |
| 外骨骼虫单次 9 | `MegaCrit.Sts2.Core.Models.Monsters.Exoskeleton`；`HardToKillPower(9)`；`HardToKillPower.ModifyDamageCap` | `HardToKillPower` 对 owner 返回 damage cap = `Amount`；`CalculateTotalDamageNextTurn()` 每段走 `Hook.ModifyDamage` | 是；例如 12 毒 + 3 段可按 `min(12,9)+min(11,9)+min(10,9)=27` | 下一优先候选；可用原生总毒伤接入后单独 Steam 验证 |
| Slippery | `SlipperyPower.ModifyHpLostAfterOsty`；`SlipperyPower.AfterDamageReceived` | HP loss >= 1 时压到 1；收到未格挡伤害后 decrement | 是，但属于 HP loss 结果修正，不由 `CalculateTotalDamageNextTurn()` 完整证明 | 用户要求：当前仍有 `SlipperyPower.Amount > 0` 时禁用该敌人的 Poison survival 修正，保留其 Intent；层数消失后再恢复已支持 Poison 计算 |
| 敌方无实体 | `IntangiblePower.ModifyDamageCap`；`IntangiblePower.ModifyHpLostAfterOsty` | damage cap 与 HP loss cap 均压到 1；`CalculateTotalDamageNextTurn()` 可反映 damage cap 侧 | 是，每个 Poison tick 通常至多 1 | 与 `TestSubject` 生命周期分开验证；先证实每段 Poison 结果，再证实当前 Intent 是否取消 |
| 鬼祟珊瑚群限伤 | `MegaCrit.Sts2.Core.Models.Monsters.SewerClam`；`HardenedShellPower` | `HardenedShellPower.DisplayAmount = Amount - damageReceivedThisTurn`；`ModifyHpLostBeforeOstyLate` 将 HP loss 限到剩余预算；`AfterDamageReceived` 累加 `result.UnblockedDamage`；`BeforeSideTurnStart` 重置预算 | 是，但属于 HP loss budget，不是单纯 damage cap | 暂不接入。需要先验证预算重置时机和 enemy side Poison tick 前的剩余预算 |
| 实验体生命周期 | `MegaCrit.Sts2.Core.Models.Monsters.TestSubject`；`AdaptablePower`；`IntangiblePower` | 用户观察复活 / 阶段敌人当前回合不会继续攻击；原生仍需确认 death / respawn 后当前 Intent 取消点 | 是，影响“是否执行当前 Intent” | 暂不接入；后续单独验证生命周期后可从 Unknown 改为确定排除 Intent |
