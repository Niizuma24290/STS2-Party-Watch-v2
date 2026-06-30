# Phase 9｜单人正式版收口

## 状态

合并 HUD 显示改造已完成代码接入；本轮水盆 + 惊涛、Constrict / Disintegration Power 自伤修补均已通过用户 Steam 运行时验证。

## 追加修补：水盆 + 惊涛

### 实际改动

- 修复 `VerifiedPreAttackBlockReader` 的 `RippleBasin` 预测：除本回合已打出本机 Attack 外，若玩家当前有 `StampedePower`，且手牌中存在本玩家可自动打出的非 `Unplayable` Attack，则预测惊涛会在结束回合 `AutoPostPlay` 阶段先自动打出 Attack，水盆不再计入预期 Block。
- 没有修改 HUD 汇总、显示格式、高级明细开关、伤害结算、命令队列、RNG、存档、网络或游戏文件。

### 原生证据

- 本地反编译 `MegaCrit.Sts2.Core.Models.Relics.RippleBasin`：`BeforeSideTurnEnd` 在本玩家参与且本回合 `CardPlaysFinished` 中没有本玩家 Attack 时调用 `CreatureCmd.GainBlock(..., DynamicVars.Block, ...)`。
- 本地反编译 `MegaCrit.Sts2.Core.Models.Powers.StampedePower`：`AfterAutoPostPlayPhaseEntered` 会从手牌中选择 `CardType.Attack` 且不含 `CardKeyword.Unplayable` 的卡，并调用 `CardCmd.AutoPlay(...)`。
- 本地反编译 `CombatManager.EndPlayerTurnPhaseOneInternal` 与 `sts2.xml`：`PlayerTurnPhase.AutoPostPlay` / `Hook.AfterAutoPostPlayPhaseEntered` 发生在后续 `BeforeTurnEnd` 与 `DoTurnEnd` 前；因此惊涛自动打出的 Attack 会早于水盆的 `BeforeSideTurnEnd` 判断。

### 运行时验证

- 已由用户完成 Steam 运行时验证：水盆 + 惊涛 + 手牌 Attack 场景下，HUD 不再把水盆 4 Block 加入预期，结果符合原生结算顺序。

### 未验证项

- “惊涛”中文名与 `StampedePower` 的运行时映射已在本轮用户 Steam 验证语境中确认。
- 惊涛多层、多张 Attack、无合法目标或其他自动打牌失败边界未扩展模拟；本轮只按原生可读的窄条件判断“会有至少一张非 `Unplayable` Attack 被自动打出”。

### 明确风险或不支持情形

- 若手牌 Attack 因目标选择或其他原生限制未能实际完成自动打出，本预测可能会少算水盆 4 Block；当前不引入通用自动打牌模拟器。
- 基础水盆、Frost、PlatingPower、Orichalcum、FakeOrichalcum、CloakClasp 等既有 EffectiveBlock 来源不改变。

### 下一步唯一任务

- 水盆 + 惊涛修补已收口；下一步仍只处理当前排队中的 Constrict / Disintegration Power 自伤运行时验证。

### 提交 hash

- `8958029`

## 目标

完成单人正式版收口：异常场景、回归测试、文档整理、清理临时诊断与 UI 打磨。

本轮追加 UI 层改造：把原本平级显示的 `🛡` 与 `♥` 改为默认只显示“总预计失血”；可信来源明细保留为高级开关。

默认 HUD 显示格式：

```text
-{shield + heart}
```

高级明细开关打开时，可追加：

```text
🛡 {shield}   ♥ {heart}
```

## 本任务允许做的事

- 整理单人 HUD 运行时验证矩阵。
- 收口异常处理和 Unknown 规则。
- 删除临时诊断。
- 更新文档和已知限制。

## 本任务禁止做的事

- 加入第三种正式 HUD 输出。
- 开发正式多人伤害 HUD。
- 引入未验证机制。
- 保留临时诊断作为生产逻辑。

## 完成标准

- 单人 HUD 规则完成运行时验证。
- 文档完成收口。
- 已知限制清楚记录。
- 生产模块中不保留临时诊断。

## 已验证事实

- 最终 HUD 默认只显示一个总预计失血主值；`🛡` / `♥` 两类可信来源明细默认关闭，只作为高级显示选项。
- Phase 1B 已运行时验证：普通敌方 Attack Intent 可正确进入 `🛡`，Block 0 / 部分 Block / 全覆盖 Block 均符合预期。
- Phase 5 已运行时验证：Burn 等 blockable 回合末 `DamageVar` 可进入 `🛡`，并与敌方攻击和 Block 正确合并。
- Phase 5C 已运行时验证：Frost、覆甲、奥利哈刚、假奥利哈刚、波纹水盆、斗篷扣可作为已验证 `EffectiveBlock` 影响 `🛡`。
- Phase 6 已运行时验证：Beckon、Bad Luck、Regret 进入 `♥`，不进入 `🛡`，且不受 Block 影响。
- Phase 7 已按正常游戏范围收口：TungstenRod 与 BeatingRemnant 可修正已验证 HP loss 事件；复数 TungstenRod 属于控制台无效状态，不作为正式行为依据。
- Phase 8 已由用户运行时验证：Diamond Diadem 可用时，HUD 提前显示“现在点击结束回合后的实际承伤”，敌人头顶仍可保留原生当前 intent 数字。

## 仅代码确认、尚未运行时验证

- Phase 9 删除未引用的早期 HUD 控制器/视图/定位器文件；当前生产 HUD 路径只剩 `ForecastRefreshPatch` 在本机 `NHealthBar` 旁创建和刷新 label。
- `ForecastRefreshPatch.BuildForecastText(...)` 只用当前已有 `ForecastResult.OutDamage` 与 `ForecastResult.DirectHpLoss` 生成展示文本。
- 合并总值 `displayTotalPredictedHpLoss` 仅为 `OutDamage + DirectHpLoss`，不重新读取游戏状态，不复制预测逻辑。
- `ShowHudBreakdownDetails` 是 UI 层高级明细开关，默认 `false`；打开后才显示 `🛡` / `♥` 构成说明。
- `IncomingDamageRead.UnknownDirect(...)` 仍允许在 blockable 伤害不可证明时只显示已验证的 direct HP loss；不会猜测未知 `🛡` 数字。
- 生产源码中未发现 `TODO` / `FIXME` / 临时诊断开关；保留的 `[STS2 Party Watch] Loaded` 属于启动加载证明日志。

## 单人回归矩阵

| 场景 | 预期 HUD | 状态 |
| --- | --- | --- |
| 非战斗 | 隐藏 | 已验证 |
| 无攻击、无回合末伤害 | 隐藏 | 已验证 |
| 普通敌方攻击，Block 0 | `-N` | 待验证合并 HUD |
| 普通敌方攻击，部分 Block | `-(N-Block)` | 待验证合并 HUD |
| 普通敌方攻击，Block 足够覆盖 | 隐藏 | 已验证 |
| 敌方攻击 + Burn | `-N` | 待验证合并 HUD |
| Burn 单独存在且未被 Block 覆盖 | `-N` | 待验证合并 HUD |
| Beckon / Bad Luck / Regret | `-N` | 待验证合并 HUD |
| `🛡` 与 `♥` 同时存在 | `-(shield + heart)` | 待验证合并 HUD |
| TungstenRod / BeatingRemnant | 修正实际 HP loss 预测 | 已验证正常游戏范围 |
| Diamond Diadem 可触发 | `🛡` 显示王冠作用后的实际承伤 | 已验证 |
| 多人战斗 | 不做正式多人 HUD | 冻结到 Phase 10 |

## 合并 HUD 显示规则

| 当前可信最终值 | 显示 |
| --- | --- |
| `OutDamage > 0` 且 `DirectHpLoss > 0` | `-(OutDamage + DirectHpLoss)` |
| 仅 `OutDamage > 0` | `-OutDamage` |
| 仅 `DirectHpLoss > 0` | `-DirectHpLoss` |
| 两者都为 0、Hidden、Unknown 且无可信 direct HP loss | 清空并隐藏 HUD |

高级明细开关打开时，第二行显示可信 `🛡 OutDamage` / `♥ DirectHpLoss` 构成。本轮不新增 `?`、至少值、来源展开、第三种图标或不支持提示。复杂不确定性展示留给后续 UI 打磨任务。

## 最终支持边界

### VerifiedSupported

| 机制 | HUD | 支持说明 |
| --- | --- | --- |
| 怪物 AttackIntent / DeathBlow | `🛡` | 使用原生预览 API 读取敌方攻击。 |
| Burn 等手牌回合末 blockable `DamageVar` | `🛡` | 仅接入可证明调用 `CreatureCmd.Damage` 且不带 `ValueProp.Unblockable` 的手牌回合末伤害。 |
| Frost | `🛡` | 作为已验证 end-turn `EffectiveBlock` 来源。 |
| PlatingPower | `🛡` | 作为已验证 end-turn `EffectiveBlock` 来源。 |
| Orichalcum | `🛡` | 当前 Block 为 0 时作为已验证 `EffectiveBlock` 来源。 |
| FakeOrichalcum | `🛡` | 当前 Block 为 0 时作为已验证 `EffectiveBlock` 来源。 |
| RippleBasin | `🛡` | 根据本回合是否已打出本机 Attack 判断。 |
| CloakClasp | `🛡` | 按当前手牌数和遗物 BlockVar 预测。 |
| Beckon | `♥` | 当前手牌中固定 direct HP loss。 |
| Bad Luck | `♥` | 当前手牌中固定 direct HP loss。 |
| Regret | `♥` | 当前手牌总数乘以 Regret 数量。 |
| Diamond Diadem | `🛡` | 在已验证 single-hit / multi-hit 粒度下，提前预测王冠作用后的敌方攻击承伤。 |

### ConditionallySupported

| 机制 | HUD | 条件 |
| --- | --- | --- |
| Tungsten Rod | `🛡` / `♥` | 仅修正已验证单事件 HP loss；敌方攻击需要可拆为 verified single-hit 事件。 |
| Beating Remnant | `🛡` / `♥` | 仅在事件顺序与本回合剩余预算可证明时应用；预算由 HUD 观测到的实际 HP 下降窗口维护。 |
| Diamond Diadem + StampedePower | `🛡` | 只处理窄规则：有 StampedePower、手牌有 Attack、当前计数等于阈值时，预测自动打牌先发生并阻止王冠触发。 |

### UnsupportedOrHidden

| 情形 | 处理 |
| --- | --- |
| 多人正式 HUD | Phase 10 前冻结，不显示正式多人伤害 HUD。 |
| blockable 伤害缺少必要 hit 粒度或事件顺序 | 隐藏 `🛡`，不显示部分合计或估算值。 |
| direct HP loss 不可证明 | 隐藏 `♥`。 |
| 非战斗、无本机玩家、无有效血条、玩家死亡、CombatState 不可读 | 清空并隐藏 HUD。 |
| 未验证卡牌、Power、遗物、敌人特殊机制 | 不接入，不显示猜测数字。 |
| 通用回合模拟器、通用 damage / HP loss 引擎 | 不实现。 |

## 未解决问题

- Phase 9 最终手动回归尚未由用户完成。
- 默认总值 HUD 的游戏内布局、字体和血条旁间距尚未由截图确认。
- 高级明细目前是 UI 层开关，默认关闭；尚未接入正式设置页或外部配置文件。
- 多人真实目标与 target-aware 原生预览仍冻结到 Phase 10 研究。
- Diamond Diadem 与其他 multiplicative hook 的同阶段内部相对顺序仍只保留为已知限制；当前实现依赖原生 Hook 阶段证据和用户运行时验证。

## 构建与仓库检查

- `C:\sts2\dotnet\dotnet.exe build .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore` 通过，0 warning / 0 error。
- `C:\sts2\dotnet\dotnet.exe publish .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore` 通过。
- `git diff --check` 通过，仅输出既有 LF/CRLF 提示。
- `git status --short` 已检查；未看到 DLL / PDB / PCK / logs / bin / obj / publish / 游戏目录文件进入 git。

## 实际改动文件

- `src/STS2PartyWatchCode/UI/ForecastHudController.cs`：删除未引用的旧 HUD 控制器。
- `src/STS2PartyWatchCode/UI/ForecastHudView.cs`：删除未引用的旧 HUD view。
- `src/STS2PartyWatchCode/UI/HealthBarLocator.cs`：删除未引用的旧 UI 定位 helper。
- `src/STS2PartyWatchCode/Patches/ForecastRefreshPatch.cs`：将 HUD 文本改为默认只显示总预计失血；保留默认关闭的高级明细开关，可显示可信 `🛡` / `♥` 构成；默认 label 高度回到单行。
- `src/STS2PartyWatchCode/Combat/LocalIncomingDamageReader.cs`：修复 HP loss relic 路径下的 blockable hand damage 事件收集；有棍子时不再只允许 Burn，腐朽这类已验证 blockable `DamageVar` 也能进入逐事件修正。
- `README.md`：更新展示向说明，记录 `🛡` / `♥` 语义、支持范围与保守隐藏策略。
- `docs/architecture.md`：更新为当前生产 HUD 路径与职责边界。
- `docs/build-environment.md`：同步 Phase 9 允许的 build / publish / diff / status 命令。
- `docs/decisions.md`：更新单人正式版 HUD 决策。
- `docs/mechanics-evidence.md`：补充 Phase 9 单人正式版 HUD 合同。
- `docs/project-state.md`：同步 Phase 9 当前状态。
- `docs/task-notes/README.md`：同步 Phase 9 当前状态。
- `docs/v2-roadmap.md`：移除对旧 HUD 类名的描述。
- `docs/task-notes/phase-9-singleplayer-validation.md`：记录 Phase 9 收口状态、验证矩阵与已知限制。

## 下一步唯一任务

- 用户执行合并 HUD 的 Steam 运行时验证；截图确认后再做布局/视觉微调。

## 预期提交文件

- `docs/architecture.md`
- `docs/build-environment.md`
- `docs/decisions.md`
- `docs/mechanics-evidence.md`
- `docs/project-state.md`
- `docs/task-notes/README.md`
- `docs/task-notes/phase-9-singleplayer-validation.md`
- `README.md`
- `src/STS2PartyWatchCode/UI/ForecastHudController.cs`
- `src/STS2PartyWatchCode/UI/ForecastHudView.cs`
- `src/STS2PartyWatchCode/UI/HealthBarLocator.cs`

## 提交记录

- `8958029`：修复水盆 + 惊涛预测，并收口当前单人基线。
- `5d08a90`：记录水盆 + 惊涛 Steam 运行时验证通过。
## 追加修补：Constrict / Disintegration 回合末 Power 自伤

### 实际改动

- 新增 `VerifiedTurnEndPowerDamageReader`，只读取本机玩家身上的 `ConstrictPower` 与 `DisintegrationPower`。
- 两者均按原生证据作为 blockable `DamageProps.nonCardUnpowered` 伤害进入 `🛡` lane。
- `LocalIncomingDamageReader` 普通路径将这两个 Power 的预测伤害加入 `RawDamage`，再由既有 `EffectiveBlock` 逻辑抵扣。
- 有 `TungstenRod` / `BeatingRemnant` 时，这两个 Power 被插入既有逐事件 HP loss 预算流，顺序为：手牌回合末事件 -> `ConstrictPower` -> `DisintegrationPower` -> 敌方 AttackIntent。
- 未接入 `PoisonPower`、`DoomPower`、`DemisePower` 或通用 Power 扫描器。

### 原生证据

- `SlitheringStrangler.ConstrictMove` 调用 `PowerCmd.Apply<ConstrictPower>(...)`。
- `ConstrictPower.AfterSideTurnEnd` 在持有者参与回合结束时调用 `CreatureCmd.Damage(owner, Amount, DamageProps.nonCardUnpowered, owner, null)`。
- `KnowledgeDemon` 的诅咒选择可生成 `Disintegration`；`Disintegration.OnChosen` 调用 `PowerCmd.Apply<DisintegrationPower>(...)`。
- `DisintegrationPower.AfterSideTurnEndLate` 在持有者参与回合结束时调用同样的 `CreatureCmd.Damage(..., DamageProps.nonCardUnpowered, owner, null)`。
- `CombatManager.EndPlayerTurnPhaseOneInternal` 顺序确认：`DoTurnEnd` 的手牌回合末事件早于后续 `AfterTurnEnd`；因此 Power 自伤应排在手牌回合末伤害之后、敌方行动之前。

### 运行时验证

- 已由用户完成 Steam 运行时验证：`ConstrictPower` / `DisintegrationPower` 在玩家结束回合前进入 HUD 总预计失血，结果符合原生 blockable Power 自伤路径。

### 未验证项

- `ConstrictPower` 与 `DisintegrationPower` 的基础 HUD 预测已完成用户 Steam 运行时验证。
- 两者与 `TungstenRod` / `BeatingRemnant` 同场景下的实际逐事件顺序未单独运行时回归。

### 明确风险或不支持情形

- `PoisonPower` 是 `DamageProps.nonCardHpLoss`，本轮不接入；如果密林花蛇实际给的是 Poison 而不是 Constrict，需要另开一个 direct HP loss 小修。
- `DoomPower` 是致死判定，不是固定伤害数字，本轮不显示。
- 未实现通用 Power damage 引擎。

### 下一步唯一任务

- 本轮 Constrict / Disintegration Power 自伤修补已收口；下一步仍保持 Phase 10 冻结，只在后续有新明确问题时单独开小任务。

### 提交 hash

- `988ecb4`
- 验证收口提交：`3680f58`

## 追加修补：IntangiblePower / 无实体

### 实际改动

- 将原 `VerifiedHpLossRelicModifier` 收敛为 `VerifiedHpLossResultModifier`，使同一逐事件 HP loss 结果链可同时处理 `IntangiblePower`、`TungstenRod` 与 `BeatingRemnant`。
- `LocalIncomingDamageReader` 在本机玩家拥有 `IntangiblePower` 时进入逐事件 HP loss 预测路径。
- `VerifiedFixedTurnEndHpLossReader` 新增事件列表读取入口，保留 Beckon / Bad Luck / Regret 的单事件粒度。
- 无实体只在 `DirectHpLoss` lane 中对已验证单事件应用 `>0 -> 1`；blockable lane 继续信任原生 `AttackIntent` / `Hook.ModifyDamage` 预览，避免多段或聚合 blockable 伤害被二次压成 1。
- 无实体修正顺序排在 `TungstenRod` 与 `BeatingRemnant` 前。
- 没有修改 HUD 显示格式、高级明细开关、CombatState、命令队列、RNG、存档、网络或游戏文件。

### 原生证据

- 本地 `sts2.xml`：`ModifyDamageHookType.Cap` 是 damage-capping hook，示例为 `IntangiblePower`。
- 本地 `sts2.xml`：`IntangiblePower.ModifyDamageCap(...)` 将收到的 damage cap 到 1；同一说明写明 HP loss 逻辑由 `IntangiblePower.ModifyHpLostAfterOsty(...)` 处理。
- 本地 `sts2.xml`：`Hook.ModifyHpLost(...)` 在 damage 扣除 Block 后运行 HP-loss-modification hooks。
- 用户实测规则：无实体按每个独立事件先变 1；Block 可以抵消该 1；多段攻击按每 hit 1；direct HP loss 每张 Beckon / Bad Luck / Regret 各自变 1；无实体先于 `TungstenRod` 与 `BeatingRemnant`。

### 运行时验证

- 本轮 Codex 未启动 Steam 游戏；当前为代码接入、构建、发布与安装验证。
- 用户已提供实际机制规则，用于确定本轮实现顺序。

### 未验证项

- Steam 运行时尚未验证本轮新 DLL。
- 无实体 + 多段攻击 + Block 的 HUD 结果尚未由本轮 Steam 复验。
- 无实体 + Beckon / Bad Luck / Regret 的 HUD 结果尚未由本轮 Steam 复验。
- 无实体 + `TungstenRod` / `BeatingRemnant` 组合尚未由本轮 Steam 复验。

### 明确风险或不支持情形

- 若某个 direct HP loss 来源不能证明单事件粒度，不把聚合值猜测压成 1。
- 本轮不接入 BufferPower、PoisonPower、DoomPower、DemisePower 或通用 HP loss 引擎。
- blockable damage 仍依赖原生 `AttackIntent` / `Hook.ModifyDamage` 预览已处理无实体；本轮不新增第二套 blockable cap 公式。

### 下一步唯一任务

- 用户从 Steam 运行时验证无实体与 Block、多段攻击、direct HP loss、Tungsten Rod / Beating Remnant 的组合。

### 构建与安装

- `C:\sts2\dotnet\dotnet.exe build .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore` 通过，0 warning / 0 error。
- `C:\sts2\dotnet\dotnet.exe publish .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore` 通过。
- `git diff --check` 通过，仅输出既有 LF/CRLF 提示。
- `powershell -ExecutionPolicy Bypass -File .\scripts\Install-LocalMod.ps1` 首次因 Program Files 写入权限失败；提升权限重跑后安装成功到 `C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\sts2-party-watch-v2`。

### 提交 hash

- 未提交。
