# Phase 9B｜Poison 结算与敌人行动前存活预览

## 状态

代码接入已完成第一阶段普通 Poison 致死预览；构建通过。Steam 运行时验证尚未执行。

## 实际改动

- 新增 `EnemyPreActionSurvivalPreview` / `PoisonTickPreview`，只处理敌人行动前 Poison tick 是否会让单个敌人实例无法执行当前 Intent。
- `EnemyPreActionState` 绑定当前战斗中的单个原生 `Creature` 敌人对象，并记录：
  - stable identity：优先 `Creature.CombatId`，否则保留原生 object reference，并使用同一快照内 enemy list index。
  - display name：`Creature.Name`。
  - native class/type：`enemy.Monster.GetType().FullName`。
  - current HP、current Poison、current Intent contribution。
  - relevant Powers：`PoisonPower`、`AccelerantPower`、`HardToKillPower`、`SlipperyPower`、`IntangiblePower`、`AdaptablePower`、`HatchPower`、`NemesisPower`。
  - lifecycle state：当前 alive/dead，并用 unsupported 门排除已知特殊生命周期。
- `LocalIncomingDamageReader` 的敌人攻击读取改为逐 enemy instance：
  - 先读取该 enemy instance 的当前 AttackIntent contribution。
  - 再执行 Poison pre-action preview。
  - 确定会在行动前死亡的 enemy instance 不贡献 Intent。
  - Unsupported 时返回 Unknown/隐藏，不猜继续攻击，也不部分扣除 Intent。
- 敌方攻击事件 source 现在包含 enemy instance identity 与 snapshot index，便于后续调试记录区分同名敌人。
- `Hook.BeforeTurnEnd` 时改为重新读取当前战斗状态并提交最终 HUD 快照；敌方行动期间继续显示该快照。
- 没有调用 `CreatureCmd.Damage(...)`、`AttackCommand.Execute(...)` 或任何真实结算入口。

## Poison 原生顺序证据

- `PoisonPower.AfterSideTurnStart` 在 owner 所在 side turn start 时触发。
- `CombatManager.StartTurn` 在 enemy side 的顺序为：

```text
Hook.BeforeSideTurnStart
-> creature.AfterTurnStart / ClearBlock
-> Hook.AfterSideTurnStart
-> CombatManager.ExecuteEnemyTurn
```

- `Hook.AfterSideTurnStart` 调用 combat hook listeners 的 `model.AfterSideTurnStart(...)`，因此敌人的 `PoisonPower.AfterSideTurnStart` 早于 `ExecuteEnemyTurn`。
- `PoisonPower` 每次 tick 调用 `CreatureCmd.Damage(..., owner, Amount, ValueProp.Unblockable | ValueProp.Unpowered, null, null)`。
- 每次 tick 后如果 owner 仍 alive，才 `PowerCmd.Decrement(this)`；如果已死，则等待后退出。
- `ExecuteEnemyTurn` 遍历 `_state.Enemies.ToList()`，但只对 `_state.ContainsCreature(enemy)` 的敌人执行 `PerformIntent()` / `TakeTurn()`。普通敌人若在 Poison tick 中死亡并被移除，不会执行当前 Intent。

## 已支持敌方机制

- 普通敌人。
- 当前有 `PoisonPower`，无已知特殊 damage cap、HP loss cap、复活、阶段转换、hatch lifecycle。
- `AccelerantPower` 已作为只读 Poison 相关 Power 接入：按 `PoisonPower.TriggerCount` 规则增加 tick 次数，最大不超过当前 Poison 层数。

## 明确不支持敌方机制

- `HardToKillPower` / `Exoskeleton`：damage cap 已确认会影响 Poison，但未做运行时矩阵。
- `SlipperyPower`：`ModifyHpLostAfterOsty` 会把 HP loss 压到 1，并在 `AfterDamageReceived` 消耗层数；逐 tick 矩阵未验证。
- 敌方 `IntangiblePower`：damage cap 与 HP loss cap 都会影响 Poison；本轮不接入敌方 Poison 预览。
- `TestSubject` / `AdaptablePower` / `NemesisPower`：存在复活、阶段与 Intangible 切换机制。
- `ToughEgg` / `HatchPower`：存在 hatch / 换形 / SetMaxAndCurrentHp 生命周期。
- 任何未证实 `ShouldDie`、`ShouldCreatureBeRemovedFromCombatAfterDeath`、`ShouldStopCombatFromEnding` 相关机制。

## HUD 快照提交时机

- 玩家 side turn start：清空旧快照。
- 玩家回合内：冻结开关开启时仍保持当前回合已提交显示稳定。
- `Hook.BeforeTurnEnd`：重新读取当前 Poison / 敌人状态 / 手牌回合末事件 / Power / 遗物修正，提交本轮最终快照。
- 敌方行动期间：保持 `BeforeTurnEnd` 提交的快照，不用任意秒数延迟。

## 运行时验证

Codex 尚未从 Steam 启动游戏；本轮仅完成代码接入和构建验证。

待 Steam 验证矩阵：

| 场景 | 预期 |
| --- | --- |
| 普通敌人，无 Poison | Intent 正常进入最终 `-N` |
| 普通敌人，Poison 不足以击杀 | Intent 保留 |
| 普通敌人，Poison 足以击杀 | 该 enemy instance 的 Intent 不进入最终 `-N` |
| 多敌人，一个被 Poison 杀死，一个存活攻击 | 只移除死亡 enemy instance 的 Intent |
| 两只同名敌人，一只 Poison 足以击杀，一只不足 | 最终 `-N` 只移除会死亡的那一只 |
| AccelerantPower 已生效 | 只读取当前 Power/Poison 状态，不重放卡牌 |
| Burn / Beckon / Bad Luck / Regret + Poison | `BeforeTurnEnd` 最终快照与真实行动前顺序一致 |
| `HardToKillPower` / `SlipperyPower` / 敌方 `IntangiblePower` / `TestSubject` / `ToughEgg` | 未接入前保持 Unknown/隐藏，不伪造死亡 |

每次验证记录：

```text
enemy instance identity
enemy name / native type
HP
Poison
previewed Poison damage
willExecuteCurrentIntent
included Intent amount
hand turn-end events
HUD -N
actual enemy acted?
actual player HP delta
match?
```

## 构建与仓库检查

- `C:\sts2\dotnet\dotnet.exe build .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore` 通过，0 warning / 0 error。
- `C:\sts2\dotnet\dotnet.exe publish .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore` 通过。
- `git diff --check` 通过，仅有既有 LF/CRLF 提示。
- `powershell -ExecutionPolicy Bypass -File .\scripts\Install-LocalMod.ps1` 内部 build / publish 通过，但复制到游戏 mods 目录失败：`sts2-party-watch-v2.dll` 正被另一个进程占用。
- 安装与 Steam 运行时验证待关闭游戏后重跑。

## 改动文件

- `.gitignore`
- `src/STS2PartyWatchCode/Combat/EnemyPreActionSurvivalPreview.cs`
- `src/STS2PartyWatchCode/Combat/LocalIncomingDamageReader.cs`
- `src/STS2PartyWatchCode/Patches/ForecastRefreshPatch.cs`
- `src/STS2PartyWatchCode/UI/PartyWatchHudSnapshotStore.cs`
- `docs/mechanics-evidence.md`
- `docs/task-notes/phase-9b-poison-pre-action-survival.md`

## 提交 hash

- `81e06a3`：`docs: document poison pre-action survival evidence`
- `0130eff`：`feat: preview poison-based enemy action cancellation`

## 下一步唯一任务

关闭正在占用 mod DLL 的游戏进程后重跑安装脚本，然后从 Steam 启动游戏执行普通 Poison / 多敌人 / 同名敌人验证矩阵。

## 追加记录：原生毒血条与特殊敌人策略

本节记录 2026-07-02 用户截图与本地反编译确认，仅作为后续小步接入依据；当前提交代码尚未改动。

### 原生毒血条相关入口

- `PoisonPower.CalculateTotalDamageNextTurn()`：
  - 原生血条毒预览的模型层总量入口。
  - 读取 `TriggerCount`，触发次数为 `min(PoisonPower.Amount, 1 + alive opponents AccelerantPower 总层数)`。
  - 逐段计算 `PoisonPower.Amount - i`，并对每一段调用 `Hook.ModifyDamage(..., ValueProp.Unblockable | ValueProp.Unpowered, ModifyDamageHookType.All, CardPreviewMode.None, ...)`。
  - 因为它走 `ModifyDamage`，可自然纳入 `HardToKillPower.ModifyDamageCap`、`IntangiblePower.ModifyDamageCap` 这类 damage cap。
  - 它不执行真实 `CreatureCmd.Damage`，也不走完整 HP loss / death / phase 生命周期；`SlipperyPower.ModifyHpLostAfterOsty`、`HardenedShellPower.ModifyHpLostBeforeOstyLate` 这类 HP loss 修正不能只靠它证明。
- `NHealthBar.IsPoisonLethal(int poisonDamage)`：
  - UI 层 lethal 判断，只检查 `poisonDamage > 0`、目标有 `PoisonPower`、且 `poisonDamage >= Creature.CurrentHp`。
  - 它不判断当前 Intent 是否取消，不判断复活 / 阶段转换，也不读取 enemy attack。
- `NHealthBar._poisonForeground`：
  - 纯 UI Control，用于显示毒预览绿色血条区间。
  - `RefreshForeground()` 先调用 `PoisonPower.CalculateTotalDamageNextTurn()` 得到总毒伤；若 `IsPoisonLethal` 为 true，隐藏红色 HP 前景并用 poison foreground 覆盖当前 HP；否则把红色 HP 前景缩到 `CurrentHp - poisonDamage`，毒前景显示两者之间的宽度。
  - 可作为运行时对照，不应作为 Party Watch 核心数据源；反推宽度会受 UI 尺寸、动画、Doom 和显示状态影响。

### 触媒 / Accelerant 记录

- 本地 `sts2.dll` 类型列表未发现 `Catalyst`。
- 当前版本可确认的原生卡牌为 `MegaCrit.Sts2.Core.Models.Cards.Accelerant`：
  - `CardType.Power`、`CardRarity.Rare`、`TargetType.Self`。
  - `OnPlay` 对玩家 Creature 施加 `AccelerantPower`。
  - 基础 `DynamicVar("Accelerant", 1m)`；升级后 `UpgradeValueBy(1m)`，即多 1 层。
- `PoisonPower.TriggerCount` 读取 Poison owner 的 alive opponents 上 `AccelerantPower` 层数；对敌人身上的 Poison 来说，就是读取玩家侧已生效的 `AccelerantPower`。
- Party Watch 仍不重放 `Accelerant` 卡牌，只读取结束回合时已经存在的 `AccelerantPower.Amount`。

### 用户截图敌人策略记录

| 截图 / 敌人 | 原生类 / Power | 用户观察 | 后续 Party Watch 策略 |
| --- | --- | --- | --- |
| 墨灵 boss / 带滑溜 | `SlipperyPower` | 当前仍有滑溜层数时，即使识别到 Poison，也不要用 Poison 行动前致死预览去移除 Intent；滑溜消失后再恢复普通 Poison 计算。 | 后续改为：若 enemy 有 `SlipperyPower.Amount > 0`，Poison survival preview 对该 enemy instance 禁用；该敌人的当前 Intent 按原有读取保留，不因 Poison 被移除。 |
| 实验体 boss | `TestSubject`、`AdaptablePower`、可能有 `IntangiblePower` | 敌方 `IntangiblePower` 会让每段 Poison 至多 1；`AccelerantPower` 可让回合结束 Poison 触发 2-3 段。复活 / 阶段敌人当前回合不会继续攻击。 | 先记录，不马上接入。后续需要确认 death / revive / phase 后当前 Intent 是否稳定取消；若确认，则可在 Poison 足以触发该生命周期时排除 Intent。 |
| 鬼祟珊瑚群 | `SewerClam`、`HardenedShellPower` | 每回合最多失去 20 HP；需要同时考虑本回合剩余限伤预算与剩余 HP，复杂度类似玩家 `BeatingRemnant` / 水盆类预算。 | 暂不接入 Poison survival preview。`HardenedShellPower` 走 `ModifyHpLostBeforeOstyLate`，不是单纯 damage cap，不能只用 `CalculateTotalDamageNextTurn()`。 |
| 外骨骼虫 | `Exoskeleton`、`HardToKillPower(9)` | 单次伤害最高 9；原生毒血条可正确显示带 `AccelerantPower` 的多段 Poison，例如 12 毒且触发 3 段时为 `min(12,9) + min(11,9) + min(10,9) = 27`。 | 下一优先级候选。因为 `PoisonPower.CalculateTotalDamageNextTurn()` 每段会走 `Hook.ModifyDamage`，可优先用该原生总量支持 `HardToKillPower` 场景，再做 Steam 验证。 |

### 接入顺序更新建议

1. 先把普通 Poison 预览的伤害来源改为优先调用 `PoisonPower.CalculateTotalDamageNextTurn()`，并保持逐 enemy instance 身份绑定。
2. 单独接入 `HardToKillPower` / `Exoskeleton`，用原生 `CalculateTotalDamageNextTurn()` 验证 12 毒 + `AccelerantPower` 三段 = 27 的场景。
3. `SlipperyPower.Amount > 0` 时不隐藏整个预测，也不移除该敌人的 Intent；仅禁用该 enemy instance 的 Poison survival 修正。滑溜消失后恢复普通 / 已支持规则。
4. 敌方 `IntangiblePower` 与 `TestSubject` 分开处理：先验证每段 Poison 为 1，再验证阶段 / 复活后当前 Intent 是否取消。
5. `HardenedShellPower` / `SewerClam` 最后处理；未确认剩余预算读取与重置时机前保持不支持。
