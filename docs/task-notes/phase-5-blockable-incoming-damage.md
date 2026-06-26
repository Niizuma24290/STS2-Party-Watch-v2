# Phase 5｜Blockable Incoming Damage 汇总

## 状态

进行中 / Phase 5A+5B 已代码完成，等待 Steam 运行时验证

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

Frost / 覆甲等 `VerifiedPreAttackBlock` 仍等待后续 5C/5D 单独验证。

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
- 实现 Frost / 覆甲 / 遗物 Block。
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
- 本次未做 Steam 运行时验证。

## 仅代码确认、尚未运行时验证

- 手牌读取使用 `CardPile.Get(PileType.Hand, player)`。
- 回合末伤害候选通过 `CardTurnEndDamageInspector.DoesTurnEndInHandCallDamage(card)` 确认卡牌 `OnTurnEndInHand` 状态机调用 `CreatureCmd.Damage`。
- 仅统计 `card.DynamicVars.Values.OfType<DamageVar>()`。
- `ValueProp.Unblockable` 的 `DamageVar` 会被排除在 `🛡 -N` 外。
- `DamageVar` 预览值通过 `Hook.ModifyDamage(player.RunState, combatState, playerCreature, playerCreature, damageVar.BaseValue, damageVar.Props, card, ModifyDamageHookType.All, CardPreviewMode.None, out _)` 读取。
- `CardPile.InvokeContentsChanged` 会触发已登记玩家血条 HUD 刷新。

## 未解决问题

- Burn / DamageVar 场景尚未 Steam 运行时验证。
- Frost 尚未纳入 `EffectiveBlock`。
- 覆甲尚未纳入 `EffectiveBlock`。
- Beckon / Bad Luck / Regret 的 `♥ -N` 尚未实现。

## 实际改动文件

- `src/STS2PartyWatchCode/Combat/CardTurnEndDamageInspector.cs`
- `src/STS2PartyWatchCode/Combat/LocalIncomingDamageReader.cs`
- `src/STS2PartyWatchCode/Patches/ForecastRefreshPatch.cs`
- `docs/project-state.md`
- `docs/interface-map.md`
- `docs/mechanics-evidence.md`
- `docs/task-notes/README.md`
- `docs/task-notes/phase-5-blockable-incoming-damage.md`

## 下一步唯一任务

- Phase 5A+5B：Steam 启动后验证 Burn/DamageVar 进入 `🛡 -N` 与手牌变化刷新

## 预期提交文件

- `src/STS2PartyWatchCode/Combat/CardTurnEndDamageInspector.cs`
- `src/STS2PartyWatchCode/Combat/LocalIncomingDamageReader.cs`
- `src/STS2PartyWatchCode/Patches/ForecastRefreshPatch.cs`
- `docs/project-state.md`
- `docs/interface-map.md`
- `docs/mechanics-evidence.md`
- `docs/task-notes/README.md`
- `docs/task-notes/phase-5-blockable-incoming-damage.md`

## 提交记录

- 待提交：`feat: include hand turn-end damage in shield forecast`
