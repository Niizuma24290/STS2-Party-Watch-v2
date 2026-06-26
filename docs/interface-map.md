# Interface Map

| 状态 | 用途 | 类型 / 成员 | 读取方式 | 运行时是否验证 | 已知限制 |
| --- | --- | --- | --- | --- | --- |
| 已运行时验证 | 判断当前是否处于战斗 | `ICombatState.IsLiveCombat()` | 公开只读方法 | 是 | 非战斗 HUD 隐藏已验证；直接 exe 启动仍不作为验证方式。 |
| 已运行时验证 | 获取本机玩家 | `LocalContext.GetMe(ICombatState)` | 公开只读静态方法 | 是 | 当前实现要求 `combatState.Players.Count == 1`，多人返回 Unknown。 |
| 已运行时验证 | 获取本机玩家 Creature | `Player.Creature` 与玩家 `NHealthBar._creature` | 公开属性读取；UI 绑定处使用窄范围 reflection 读取血条绑定 Creature | 是 | 仅单人本机玩家路径；reflection 只用于确认 UI 血条绑定对象。 |
| 已运行时验证 | 读取本机玩家当前 Block | `Creature.Block` | 公开属性读取 | 是 | v2.0 只读取当前 Block 一次，不额外计算 Frost、覆甲、遗物 Block。 |
| 仅代码确认，尚未运行时验证 | 读取本机玩家当前 HP | `Creature.CurrentHp` | 公开属性读取 | 否 | Phase 1–4 不显示 HP，仅作为接口候选保留。 |
| 仅代码确认，尚未运行时验证 | 读取本机玩家最大 HP | `Creature.MaxHp` | 公开属性读取 | 否 | Phase 1–4 不显示 HP，仅作为接口候选保留。 |
| 已运行时验证 | 判断实体是否存活 | `Creature.IsAlive` | 公开只读属性 | 是 | 死亡或不可读实体隐藏 HUD。 |
| 已运行时验证 | 读取敌人列表 | `ICombatState.Enemies` | 公开只读集合 | 是 | 只读取敌方 Creature，不读取队友或远程玩家。 |
| 已运行时验证 | 读取敌人当前 Intent 列表 | `enemy.Monster.NextMove.Intents` | 公开属性链 | 是 | 只处理可读 `NextMove.Intents`；不可读时跳过或 Unknown。 |
| 已运行时验证 | 判断攻击 Intent | `intent is AttackIntent` | 公开类型判断 | 是 | 非攻击 Intent 不显示。 |
| 已运行时验证 | 原生攻击伤害预览 | `AttackIntent.GetTotalDamage(IEnumerable<Creature> targets, Creature owner)` | 公开方法调用 | 是 | 运行时确认 Intent 9 / Block 0、5、10 场景；不手写 Strength、Weak、Vulnerable、多段等公式。 |
| 已运行时验证 | 玩家血条绑定生命周期 | `NHealthBar.SetCreature` | Harmony postfix | 是 | 仅创建/刷新本机玩家血条旁标签，不修改 combat state。 |
| 已运行时验证 | 玩家血条数值刷新 | `NHealthBar.RefreshValues` | Harmony postfix | 是 | Block 变化后 HUD 刷新已验证。 |
| 已运行时验证 | 定位血条右侧 | `NHealthBar.HpBarContainer` 与私有 `SetHpBarContainerSizeWithOffsets(Vector2)` | 公开属性读取；Harmony postfix 监听尺寸更新 | 是 | 标签挂在 `HpBarContainer` 的同级父节点或等价父节点；仅用于 UI 定位。 |
| 仅代码确认，尚未运行时验证 | 读取本机玩家手牌 | `CardPile.Get(PileType.Hand, player)` | 公开静态方法 | 否 | Phase 5A+5B 只读取本机玩家手牌，不读取队友。 |
| 仅代码确认，尚未运行时验证 | 判断卡牌是否有回合末手牌伤害 | `CardModel.HasTurnEndInHandEffect` + `OnTurnEndInHand` 状态机 IL 中的 `CreatureCmd.Damage` 调用 | 只读 reflection / IL inspection，按卡牌类型缓存 | 否 | 仅用于筛选候选卡；无法确认则排除。 |
| 仅代码确认，尚未运行时验证 | 读取回合末 blockable 卡牌伤害变量 | `card.DynamicVars.Values.OfType<DamageVar>()` | 公开集合读取 | 否 | `HpLossVar` 不读取；`ValueProp.Unblockable` 不进入 `🛡 -N`。 |
| 仅代码确认，尚未运行时验证 | 原生修正回合末卡牌 DamageVar | `Hook.ModifyDamage(...)` | 公开静态方法，只读预览调用 | 否 | 不调用 `CreatureCmd.Damage(...)`、`BeforeDamageReceived`、`AfterDamageReceived` 或真实结算。 |
| 仅代码确认，尚未运行时验证 | 手牌变化刷新 HUD | `CardPile.InvokeContentsChanged` | Harmony postfix | 否 | 只刷新已登记玩家血条 HUD，不每帧扫描。 |
| 已运行时验证 | 读取 Frost 回合末预期 Block | `player.PlayerCombatState.OrbQueue.Orbs.OfType<FrostOrb>()` 与 `FrostOrb.PassiveVal` | 公开只读属性 / 集合读取 | 是 | 仅计入 Frost passive；不模拟 evoke；Focus 通过 `PassiveVal` 走原生修正。 |
| 已运行时验证 | 读取覆甲回合末预期 Block | `localCreature.GetPower<PlatingPower>()?.Amount` | 公开只读 power 入口 | 是 | 只读取本机玩家当前覆甲层数。 |
| 已运行时验证 | 读取奥利哈刚预期 Block | `player.Relics.OfType<Orichalcum>()` + relic `BlockVar` + `Creature.Block == 0` | 公开只读 relic 集合 / DynamicVars 读取 | 是 | 仅在当前 Block 为 0 时计入。 |
| 已运行时验证 | 读取假奥利哈刚预期 Block | `player.Relics.OfType<FakeOrichalcum>()` + relic `BlockVar` + `Creature.Block == 0` | 公开只读 relic 集合 / DynamicVars 读取 | 是 | 仅在当前 Block 为 0 时计入。 |
| 已运行时验证 | 读取波纹水盆预期 Block | `player.Relics.OfType<RippleBasin>()` + `CombatManager.Instance.History.CardPlaysFinished` | 公开只读 relic 集合 / combat history 读取 | 是 | 仅判断本回合本机玩家是否已打出 Attack；历史入口异常时隐藏 HUD。 |
| 已运行时验证 | 读取斗篷扣预期 Block | `player.Relics.OfType<CloakClasp>()` + `CardPile.Get(PileType.Hand, player).Cards.Count` + relic `BlockVar` | 公开只读 relic 集合 / 手牌集合读取 | 是 | 只按当前手牌数计算。 |
