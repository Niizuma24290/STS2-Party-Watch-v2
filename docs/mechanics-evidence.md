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
| Frost / 覆甲 / 本轮遗物 Block | 不属于 raw | 属于 `EffectiveBlock` 修正 | 窄范围只读入口 | 待 Phase 5C 验证 |

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
| Frost | 是 | 回合结束 orb passive trigger 会在手牌回合末伤害前给 Block；`PassiveVal` 包含 Focus 修正 | `player.PlayerCombatState.OrbQueue.Orbs.OfType<FrostOrb>().Sum(o => o.PassiveVal)` | 待验证 |
| 覆甲 | 是 | `PlatingPower.BeforeSideTurnEndEarly` 给 Block，注释说明早于回合末伤害 | `localCreature.GetPower<PlatingPower>()?.Amount` 或等价只读 power 入口 | 待验证 |
| 奥利哈刚 | 是 | 当前 Block 为 0 时在 very early 标记，随后给 Block；检查早于覆甲 | `Orichalcum` 持有状态 + `DynamicVars.Block` + 当前 Block 条件 | 待验证 |
| 假奥利哈刚 | 是 | 逻辑同奥利哈刚，数值为 3；实际可持有性待验证 | `FakeOrichalcum` 持有状态 + `DynamicVars.Block` + 当前 Block 条件 | 待验证 |
| 波纹水盆 | 是 | 本回合未打出 Attack 时，`BeforeSideTurnEnd` 给 Block | `RippleBasin` 持有状态 + 当前回合 `CardPlaysFinished` 中是否有本机 Attack | 待验证 |
| 斗篷扣 | 是 | `BeforeSideTurnEnd` 按手牌数给 Block | `CloakClasp` 持有状态 + `PileType.Hand.GetPile(player).Cards.Count` | 待验证 |
| 钨钢棍 | 否，本轮只记录 | `ModifyHpLostAfterOsty` 减 HP loss，不是 Block | `TungstenRod.ModifyHpLostAfterOsty` | 后续补足 |
| 律动残余 | 否，本轮只记录 | 限制本回合 HP loss 上限，不是 Block | `BeatingRemnant.ModifyHpLostAfterOsty` + `DamageReceivedThisTurn` | 后续补足 |
| 钻石头冠 | 否，本轮只记录 | 回合末施加 `DiamondDiademPower`，后续将 powered attack 伤害乘 0.5，不是 Block | `DiamondDiadem.BeforeSideTurnEnd` / `DiamondDiademPower.ModifyDamageMultiplicative` | 后续补足 |
