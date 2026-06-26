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
| Burn 等回合末 `DamageVar` | 候选 / 已代码接入 | `DamageVar` 且无 `ValueProp.Unblockable`，理论上走 Block | `CardPile.Get(PileType.Hand, player)` + `DamageVar` + `Hook.ModifyDamage(...)` | 待 Phase 5A+5B 运行时验证 |
| Beckon | 否 | `HpLossVar` / `ValueProp.Unblockable` | 留给 Phase 6 | 留给 Phase 6 |
| Bad Luck | 否 | `HpLossVar` / `ValueProp.Unblockable` | 留给 Phase 6 | 留给 Phase 6 |
| Regret | 否 | `ValueProp.Unblockable`，数值依赖手牌数 | 留给 Phase 6 | 留给 Phase 6 |
| Frost / 覆甲 | 不属于 raw | 属于 `EffectiveBlock` 修正 | 窄范围只读入口 | 待后续 5C/5D 验证 |

## Phase 5A+5B code-confirmed mechanics

- 手牌读取使用 `CardPile.Get(PileType.Hand, player)`。
- 回合末伤害候选通过 `CardTurnEndDamageInspector.DoesTurnEndInHandCallDamage(card)` 确认卡牌 `OnTurnEndInHand` 状态机调用 `CreatureCmd.Damage`。
- 只读取 `card.DynamicVars.Values.OfType<DamageVar>()`，不读取 `HpLossVar`。
- `DamageVar` 若带有 `ValueProp.Unblockable`，不进入 `🛡 -N`。
- `DamageVar` 预览值通过 `Hook.ModifyDamage(...)` 读取，不调用真实结算。
- `CardPile.InvokeContentsChanged` 后会刷新已登记的玩家血条 HUD。
