# Interface Map

| 状态 | 用途 | 类型 / 成员 | 读取方式 | 运行时是否验证 | 已知限制 |
| --- | --- | --- | --- | --- | --- |
| 仅代码确认，尚未运行时验证 | 判断当前是否处于战斗 | `ICombatState.IsLiveCombat()` | 公开只读方法 | 否 | 直接 exe 启动被 Steam 初始化阻止，尚未进入战斗验证。 |
| 仅代码确认，尚未运行时验证 | 获取本机玩家 | `LocalContext.GetMe(ICombatState)` | 公开只读静态方法 | 否 | 当前实现要求 `combatState.Players.Count == 1`，多人返回 Unknown。 |
| 仅代码确认，尚未运行时验证 | 获取本机玩家 Creature | `Player.Creature` | 公开只读属性 | 否 | 仅单人本机玩家路径。 |
| 仅代码确认，尚未运行时验证 | 读取本机玩家当前 Block | `Creature.Block` | 公开属性读取 | 否 | v2.0 只读取当前 Block 一次，不额外计算 Frost、覆甲、遗物 Block。 |
| 仅代码确认，尚未运行时验证 | 读取本机玩家当前 HP | `Creature.CurrentHp` | 公开属性读取 | 否 | Phase 1–4 不显示 HP，仅作为接口候选保留。 |
| 仅代码确认，尚未运行时验证 | 读取本机玩家最大 HP | `Creature.MaxHp` | 公开属性读取 | 否 | Phase 1–4 不显示 HP，仅作为接口候选保留。 |
| 仅代码确认，尚未运行时验证 | 判断实体是否存活 | `Creature.IsAlive` | 公开只读属性 | 否 | 死亡或不可读实体隐藏 HUD。 |
| 仅代码确认，尚未运行时验证 | 读取敌人列表 | `ICombatState.Enemies` | 公开只读集合 | 否 | 只读取敌方 Creature，不读取队友或远程玩家。 |
| 仅代码确认，尚未运行时验证 | 读取敌人当前 Intent 列表 | `enemy.Monster.NextMove.Intents` | 公开属性链 | 否 | 只处理可读 `NextMove.Intents`；不可读时跳过或 Unknown。 |
| 仅代码确认，尚未运行时验证 | 判断攻击 Intent | `intent is AttackIntent` | 公开类型判断 | 否 | 非攻击 Intent 不显示。 |
| 仅代码确认，尚未运行时验证 | 原生攻击伤害预览 | `AttackIntent.GetTotalDamage(IEnumerable<Creature> targets, Creature owner)` | 公开方法调用 | 否 | RAW 必须来自该入口；不手写 Strength、Weak、Vulnerable、多段等公式。 |
| 仅代码确认，尚未运行时验证 | 获取当前战斗房间节点 | `NCombatRoom.Instance` | 公开静态属性 | 否 | 仅用于 HUD 锚定，不用于 combat 计算。 |
| 仅代码确认，尚未运行时验证 | 获取本机玩家节点 | `NCombatRoom.GetCreatureNode(Creature)` | 公开方法调用 | 否 | 用于把 HUD 放在玩家血条右侧。 |
| 仅代码确认，尚未运行时验证 | 定位血条右侧 | `NCreature._stateDisplay._healthBar` | 窄范围 reflection，仅 UI 定位 | 否 | 若字段不可读，回退到玩家节点右侧固定偏移；不读取或修改 combat state。 |
