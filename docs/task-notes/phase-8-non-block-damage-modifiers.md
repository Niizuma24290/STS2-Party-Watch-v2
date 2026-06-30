# Phase 8｜非 Block 承伤修正机制

## 状态

Phase 8B 已完成；用户 Steam 运行时验证通过。

## 目标

补 DiamondDiademPower 等“改变实际承伤、但不属于 Block”的伤害修正机制。

## 当前范围

- DiamondDiadem
- DiamondDiademPower

不扩展其他遗物、Power、多人 HUD、额外伤害修正或新的预测机制。

## 本轮实际改动

- 只读检查当前游戏安装目录中的 `sts2.dll` / `sts2.xml`。
- 记录 `DiamondDiadem` / `DiamondDiademPower` 的原生证据。
- 新增窄入口 `VerifiedEnemyDamageModifier`，只处理 Diamond Diadem 对已验证敌方 attack damage 事件的修正。
- 在 `LocalIncomingDamageReader` 的敌方 AttackIntent 事件读取后、Block / Tungsten Rod / Beating Remnant 计算前应用 Diamond Diadem 修正。
- 当前 `DiamondDiademPower` 已存在时，继续信任原生 `AttackIntent.GetSingleDamage(...)` / `GetTotalDamage(...)`，避免双重减半。
- 当前 Power 尚未存在但玩家持有 Diamond Diadem，且 `CardsPlayedThisTurn` 在考虑 `StampedePower` 后仍不超过 `CardThreshold` 时，预估回合结束会获得 Power，并按原生证据对敌方 single hit damage 乘以 `0.5` 后取整。
- `StampedePower` 只作为 Diamond Diadem 触发判断里的计数修正：若当前已有 2 张、玩家有 `StampedePower`、手牌中有 Attack，则预测惊涛先自动打一张导致王冠不触发。
- 若敌方攻击只能读到聚合值而不能证明 single hit 粒度，则隐藏受 Diamond Diadem 影响的 `🛡`，不使用聚合值减半。
- 用户验证前发现实际游戏 mods 目录仍加载旧 DLL；已按用户要求将本轮 publish 输出更新到游戏 mods 目录，并用 SHA256 确认两边 DLL 一致。
- 用户随后完成 Steam 运行时验证，确认 Diamond Diadem HUD 预测行为正确。

## 原生机制证据

游戏版本证据：

- 程序集：`C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\sts2.dll`
- XML 文档：`C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\sts2.xml`
- 文件时间：`2026-06-19 18:57:30`
- 程序集版本：`sts2, Version=0.1.0.0`
- CodeGraph：工具可用，但本仓库未初始化 `.codegraph`，本轮未生成或提交 CodeGraph 产物。

证据表：

| 字段 | 证据 |
| --- | --- |
| 机制 | Diamond Diadem 在玩家回合结束时，若本回合打出牌数不超过阈值，则给玩家添加 `DiamondDiademPower`；该 Power 让符合条件的 incoming powered attack damage 乘以 `0.5`。 |
| 游戏版本 | 本地 `sts2.dll` / `sts2.xml`，文件时间 `2026-06-19 18:57:30`，程序集版本 `0.1.0.0`。 |
| 类 / 方法 / 字段 | `MegaCrit.Sts2.Core.Models.Relics.DiamondDiadem`；字段 `_cardsPlayedThisTurn`；属性 `CardsPlayedThisTurn`；方法 `AfterCardPlayed`、`BeforeSideTurnEnd`、`AfterCombatEnd`、`RefreshCounter`。`MegaCrit.Sts2.Core.Models.Powers.DiamondDiademPower`；方法 `ModifyDamageMultiplicative`、`AfterSideTurnEnd`。 |
| 原生 hook 阶段 | `DiamondDiadem.AfterCardPlayed` 计数；`DiamondDiadem.BeforeSideTurnEnd` 添加 Power；`DiamondDiademPower.ModifyDamageMultiplicative` 参与 `Hook.ModifyDamage` 的 multiplicative 阶段；`DiamondDiademPower.AfterSideTurnEnd` 移除 Power。 |
| 作用对象 | `DiamondDiademPower.ModifyDamageMultiplicative` 先检查 `target == Owner`，不满足则返回 `1`；因此只影响 Power 持有者收到的伤害。 |
| 输入伤害值是什么 | `ModifyDamageMultiplicative(target, amount, props, dealer, cardSource)` 的 `amount` 是 `Hook.ModifyDamage` 在 additive 阶段之后传入 multiplicative 阶段的 decimal damage。 |
| 输出伤害值是什么 | 条件满足时返回乘数 `0.5`；不满足时返回 `1`。最终 damage 由 `Hook.ModifyDamage` 继续进入 cap 阶段并 `Math.Max(0, decimal)`。 |
| 取整规则 | `Hook.ModifyDamage` 内未截断；`AttackIntent.GetSingleDamage(...)` 对 `Hook.ModifyDamage` 的 decimal 结果执行显式 `int` 转换，再 `Math.Max(0, int)`。这等价于对单 hit 结果向零截断。 |
| 是否按每一 hit | `SingleAttackIntent.GetTotalDamage(...)` 返回 `GetSingleDamage(...)`；`MultiAttackIntent.GetTotalDamage(...)` 返回 `GetSingleDamage(...) * Repeats`。因此攻击 Intent 预览侧是每 hit 先修正并取整，再乘 hit 数。 |
| 与 Block 的相对顺序 | `CreatureCmd.Damage` 的 async state machine 中先调用 `Hook.ModifyDamage`，之后才计算 Block / unblocked damage；因此 Diamond Diadem 属于 Block 前 damage 修正。 |
| 与 Weak / Vulnerable 的相对顺序 | `Hook.ModifyDamageInternal` 顺序为 additive hooks -> multiplicative hooks -> cap hooks。`DiamondDiademPower` 位于 multiplicative hook；与 Weak / Vulnerable 等其他 multiplicative hook 的同阶段相对枚举顺序本轮未进一步证明。 |
| 与 Tungsten Rod / Beating Remnant 的相对顺序 | `CreatureCmd.Damage` 中 `Hook.ModifyDamage` 先于 Block 与 HP loss；`Hook.ModifyHpLost` 在 Block 后执行。Tungsten Rod / Beating Remnant 属于 HP loss 修改，因此 Diamond Diadem 早于它们。 |
| Power 获得时点 | `DiamondDiadem.BeforeSideTurnEnd(choiceContext, side, participants)`：若 `participants` 包含遗物 owner 的 creature，且 `CardsPlayedThisTurn <= DynamicVars["CardThreshold"].BaseValue`，调用 `PowerCmd.Apply<DiamondDiademPower>(..., amount: 1, target: owner.Creature, applier: owner.Creature, cardSource: null, silent: false)`。 |
| Power 移除时点 | `DiamondDiademPower.AfterSideTurnEnd(choiceContext, side, participants)`：当 `side == CombatSide.Enemy` 时调用 `PowerCmd.Remove(this)`。 |
| 手牌 / 自动打牌计数来源 | `DiamondDiadem.AfterCardPlayed(choiceContext, cardPlay)`：当 `cardPlay.Card.Owner == Owner` 且 `CombatManager.Instance.IsInProgress` 时，将 `CardsPlayedThisTurn` 加 1 并刷新 counter。本轮未证明 `BeforeCardAutoPlayed` 是否也会产生 `AfterCardPlayed` 事件。 |
| Party Watch 可读取的状态 | 可读 `player.Relics.OfType<DiamondDiadem>()`、`DiamondDiadem.CardsPlayedThisTurn`、`DynamicVars["CardThreshold"].BaseValue`、`localCreature.GetPower<DiamondDiademPower>()`、`AttackIntent.GetSingleDamage(...)` / `GetTotalDamage(...)` / `Repeats`。 |
| 是否可精确预测 | 证据支持在可证明单 hit 粒度时预测；若只能拿到聚合 enemy damage，且 Diamond Diadem 需要 per-hit 取整，则不得用聚合值减半代替。 |

补充证据：

- `PlayerTurnPhase.AutoPostPlay` 的 XML 文档列出 `StampedePower` 作为 AutoPostPlay 示例。
- `MegaCrit.Sts2.Core.Models.Powers.StampedePower+<AfterAutoPostPlayPhaseEntered>d__4.MoveNext` 调用 `CardCmd.AutoPlay`；本轮将其作为用户所称“惊涛”的候选原生类型处理，待用户运行时验证中文名映射。

## Phase 8B 决策门状态

| 条件 | 状态 |
| --- | --- |
| 能确认 `DiamondDiademPower` 是否当前生效 | 可通过 `localCreature.GetPower<DiamondDiademPower>()` 读取当前 Power；未来回合末是否会获得由 `DiamondDiadem.CardsPlayedThisTurn <= CardThreshold` 并结合 `StampedePower` 窄规则判断。 |
| 能确认它修正的是敌方 damage，而不是直接 HP loss | 已确认：Power 实现 `ModifyDamageMultiplicative`，不是 `ModifyHpLost*`。 |
| 能确认取整规则 | 已确认攻击 Intent 预览侧在 `GetSingleDamage(...)` 对 decimal 结果转 `int`。 |
| 能确认现有攻击读取是否能够保留原生所需的单 hit 粒度 | 现有 Phase 7 代码已有 `GetSingleDamage(...) * Repeats == GetTotalDamage(...)` 的单 hit 验证门；但 Phase 8B 尚未接入。 |
| 能确认它发生在 Block 前还是 Block 后 | 已确认发生在 Block 前。 |

## 明确不支持情形

- 不对聚合 enemy damage 输出猜测后的 Diamond Diadem 修正 HUD 数字。
- 遇到无法拆分为 verified single hit 的敌方聚合攻击时，必须隐藏受 Diamond Diadem 影响的 `🛡`，记录 `UnsupportedBecauseAggregateEnemyDamageWithPerHitRounding`。
- 不对 Burn、Beckon、Bad Luck、Regret、其他 direct HP loss、玩家自伤或通用 damage 系统应用 Diamond Diadem。
- 不把公开描述或社区经验当作实现事实。

## 运行时验证

本轮 Codex 未从 Steam 启动游戏；Steam 运行时验证由用户完成。

用户提供的运行时观察：

- 拥有 Diamond Diadem 时，敌人头上的伤害显示会在玩家点击结束回合、`DiamondDiademPower` 生效后才变化。
- 本回合打出 0 / 1 / 2 张牌时，敌人攻击伤害在玩家回合结束时减半；打出 3 张或更多牌时不触发。
- Diamond Diadem 不影响状态牌和诅咒造成的伤害，例如 Burn / Regret 等不受影响。
- 单 hit 奇数伤害按每 hit 向下取整：例如 9 -> 4，11 -> 5，13 -> 6。
- 多 hit 攻击按每 hit 分别减半并取整后相加，不在聚合总值层减半。
- 有 Block 时，先应用 Diamond Diadem 减半，再扣 Block。
- 与 Tungsten Rod / Beating Remnant 同时存在时，观察顺序为 Diamond Diadem 先修正 enemy damage，然后 Tungsten Rod 修正 HP loss，最后 Beating Remnant 限制本回合 HP loss。
- 自动打牌会增加 Diamond Diadem 的本回合打牌计数；未来接入必须读取原生 `CardsPlayedThisTurn`，不能自建只统计手动打牌的计数器。
- 用户观察确认：“惊涛”类回合结束自动打攻击牌效果先于 Diamond Diadem 阈值检查执行。因此未来 Phase 8B 若玩家有 Diamond Diadem，还需要在王冠分支内考虑：当前是否同时有惊涛、手牌是否有攻击牌、以及当前 `CardsPlayedThisTurn` 是否已经为 2；若是，则惊涛会先自动打出攻击牌把计数推到 3，王冠不应预测触发。
- 用户运行时验证确认：安装本轮新 DLL 后，Diamond Diadem 可用时 HUD 会提前显示“现在点击结束回合后的实际承伤”，例如敌人头顶仍显示原生 `8x2 (16)` 时，HUD 正确显示王冠作用后的 `🛡 -8`。

## 未验证项

- `BeforeCardAutoPlayed` / 自动打牌是否最终触发 `AfterCardPlayed` 并进入 `CardsPlayedThisTurn`。
- “惊涛”中文名与 `StampedePower` 的运行时映射仍需用户验证。
- `StampedePower` 多层、多张可自动打 Attack、无合法目标等边界情况未扩展处理；当前只按“有该 Power + 手牌有 Attack + 当前计数为阈值”将王冠触发预测改为不触发。
- `DiamondDiademPower` 与其他 multiplicative hook（例如 Weak / Vulnerable）在同阶段内部的具体枚举先后；当前只确认同属 multiplicative 阶段。
- Diamond Diadem 与 Tungsten Rod / Beating Remnant 组合的完整 HUD 显示矩阵仍未由 Codex 复验。
- Steam 运行时下 0 / 1 / 2 / 3 张牌、奇数伤害、偶数伤害、多 hit、有 Block / 无 Block 的实际结果已有用户观察；Codex 未亲自启动 Steam 复验。

## 构建与发布

- `C:\sts2\dotnet\dotnet.exe build .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore` 通过，0 warning / 0 error。
- `C:\sts2\dotnet\dotnet.exe publish .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore` 通过。
- 已按用户要求更新实际游戏 mods 目录 DLL：publish DLL 与 `C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\sts2-party-watch-v2\sts2-party-watch-v2.dll` 的 SHA256 均为 `32B5C963E50FAE6BC04293E30C0E95D634877895F242C1C280FE44C64032B516`。

## 本任务允许做的事

- 只读确认 shipped 机制与实际时序。
- 在证据充分时，把非 Block 承伤修正纳入预测。
- 保持已验证的 `🛡 -N` 与 `♥ -N` 来源规则清晰分离。

## 本任务禁止做的事

- 接入 TungstenRod / BeatingRemnant。
- 开发多人 HUD。
- 创建完整回合模拟器。
- 调用真实结算、命令、RNG、存档或网络入口。

## 下一步唯一任务

- 进入 Phase 9：单人正式版收口。

## 提交 hash

未提交。
