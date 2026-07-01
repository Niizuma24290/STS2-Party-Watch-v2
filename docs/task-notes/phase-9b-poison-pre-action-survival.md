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

未提交。

## 下一步唯一任务

关闭正在占用 mod DLL 的游戏进程后重跑安装脚本，然后从 Steam 启动游戏执行普通 Poison / 多敌人 / 同名敌人验证矩阵。
