# Damage Forecast — Forecast Engine 测试护栏建设主任务卡

日期：2026-07-21

状态：Closed / TG0-TG7 completed / 2026-07-21

仓库：`https://github.com/Niizuma24290/Damage-Forecast.git`

当前分支：`main`

任务类型：行为特征化、确定性测试、生命周期护栏、技术身份一致性检查、双目标验证治理

上位约束：

- [`forecast-engine-architecture-stabilization-master-task-card.md`](forecast-engine-architecture-stabilization-master-task-card.md)
- [`../architecture.md`](../architecture.md)
- [`../project-state.md`](../project-state.md)

与后续工作的关系：

- 本任务是 G6 Full Technical Identity Migration 和 Forecast Engine 架构实施前的测试准备，不是 G6，也不是架构 cutover。
- 本任务完成后，架构卡 AR1 的“Characterization Test Foundation”可以按实际交付标记为已满足或部分满足；不得重复建设第二套测试体系。
- 本任务不得决定新的 Mod ID、程序集、namespace、持久化 key、安装路径或 Workshop 身份。
- 本任务不得借测试 seam 提前实现统一 Forecast Engine、删除旧 Reader、切换 HUD 或改变玩家可见计算。

---

## 0. 给下一 Session 的执行指令

下一 Session 必须先完整阅读本任务卡，然后只读以下文件：

1. `docs/task-notes/README.md`
2. `docs/project-state.md`
3. `docs/architecture.md`
4. `docs/build-environment.md`
5. `docs/task-notes/forecast-engine-architecture-stabilization-master-task-card.md`
6. `docs/task-notes/phase-12c-audit/code-audit-findings.md` 中 AUD-0010
7. `docs/task-notes/phase-12c-audit/name-migration-inventory.md`
8. `tests/STS2PartyWatchCode.ContractTests/STS2PartyWatchCode.ContractTests.csproj`
9. `tests/STS2PartyWatchCode.ContractTests/Program.cs`
10. 本卡第 6 节列出的生产 seam

首次进入只允许执行 TG0。任务卡存在不等于批准修改测试、源码、脚本、项目文件或文档。

TG0 完成后必须向用户报告：

- 当前 HEAD、branch、remote、status 与本卡基线是否一致；
- 用户是否已有未提交改动；
- 当前 contract harness 的实际测试数量、结构和双目标结果；
- 第一批最小实现建议及精确文件范围；
- 是否发现本卡与当前代码不一致；
- 请求批准的 Gate，只能是 TG1，不能自动进入 TG2。

未经对应批准，不得：

- 修改源码、测试、脚本、项目或权威文档；
- restore/build/publish 产生新输出；
- 安装 Mod、启动游戏、操作 Workshop；
- stage、commit、push 或 tag；
- 执行 G6 或 Forecast Engine 架构重构。

---

## 1. 一句话任务目标

在任何全面技术改名或 Forecast Engine 重构之前，把当前可信行为冻结成分层、确定性、可定位、双目标可重复的测试契约，使“有意迁移”能够显式更新身份契约，而“无意回归”能在进入游戏人工验证前被快速阻止。

---

## 2. 为什么需要独立测试护栏任务

当前已有 executable contract harness，但它仍是初始形态：

- 只有一个 `Program.cs`；
- 当前基线为 28 个顺序 `Check(...)` 断言；
- 成功输出中的 `28` 是手工维护文本；
- 覆盖重点是 Burn 形状、结果钳制、配置文案、BaseLib 标题兼容和 AUD-0007 profiler；
- 复杂事件顺序、Block 分类/消耗、Poison、HP-loss modifier、快照冻结、覆盖界面恢复、连续战斗清理和 `N`/`-N` 同源性缺少确定性护栏；
- 多数复杂逻辑直接接收 STS2 `Player`、`Creature`、Relic 或 Godot 对象，测试难以安全构造；
- stable/beta 都能运行 harness，但当前测试仍依赖对应游戏参考程序集和 BaseLib compile dependency；
- 现有 runtime 证据很重要，但人工证据无法替代每次小改动后的快速回归信号。

这张任务卡解决的是“在改名和重构前建立安全网”，不是追求一个好看的测试数量，也不是把真实游戏集成伪装成单元测试。

---

## 3. 当前权威基线

下一 Session 必须在 TG0 重新验证，不得盲信：

```text
Known repository: Niizuma24290/Damage-Forecast
Known branch: main
Known documentation closure HEAD: 308ffd5
Known product/source release tip: 0de7d74
Known release tag: v0.1.0
Known documentation tag: documentation-authority-consolidated
Known player-facing name: Damage Forecast / 伤害预测
Known technical Mod ID: sts2-party-watch-v2
Known root namespace: STS2PartyWatch
Known project path: src/STS2PartyWatchCode/STS2PartyWatchCode.csproj
Known test project: tests/STS2PartyWatchCode.ContractTests/STS2PartyWatchCode.ContractTests.csproj
Known stable target: v0.107.1 / 59260271
Known beta target: v0.109.0 / c12f634d
Known contract baseline: 28/28 per target
Known BaseLib compile baseline: 3.3.4, hash pinned
Known final installed/runtime artifact evidence: project-state authority
Known Workshop state: private/subscription milestone; no update authorized
```

如果当前仓库已变化：

- 不 reset、checkout、restore、clean 或覆盖；
- 区分用户改动、其他 Session 改动和本任务历史；
- 以当前代码为准重新给出 diff 计划；
- 不为了匹配旧数字回退新工作；
- 更新本卡 checkpoint 时记录基线差异，但不得改写历史证据。

---

## 4. 成功定义

本任务完成时必须同时满足：

1. 测试拥有稳定名称、类别、场景 ID、自动计数和可定位失败输出。
2. 当前玩家行为由行为契约保护，而不是由大段文本快照或旧类名保护。
3. `Hidden / Known / Unknown`、direct-only fallback、零值隐藏和负值钳制都有确定性测试。
4. `N`、`-N`、blockable/direct breakdown 的当前投影语义有场景表。
5. Intangible、TungstenRod、BeatingRemnant 的顺序、粒度和保守降级有纯策略测试。
6. ordinary Poison 及已支持的窄特殊边界具有不依赖真实战斗的核心策略测试；真实 native hook 行为仍留给 runtime。
7. 冻结、覆盖界面临时隐藏、战斗结束清理和连续战斗状态隔离具有纯状态机护栏。
8. stable v0.107.1 与 beta v0.109.0 使用同一测试源并全部通过。
9. 技术身份一致性检查能发现“只改了一半”的 G6；同时不会永久禁止用户批准的有意迁移。
10. 新测试不执行真实伤害、命令、卡牌、RNG、游戏写入、网络或 Workshop 行为。
11. 不以测试名义改变当前生产结果、默认设置、UI、技术身份或发布状态。
12. 任务形成一个可被 G6 和架构卡共同复用的测试入口，不创建第二套平行 harness。

维护性验收：

> 增加一个 synthetic forecast scenario 时，应主要新增一条具名场景数据和预期结果；不应复制一份 runner、启动游戏、创建 Godot UI 或修改无关玩家文案。

### 4.1 弹性与不可变边界

本卡规定需要证明的能力，不强制下一 Session 机械照搬候选名称或文件拆分。

允许根据当前代码调整：

- test case、fixture、runner 和 pure policy 的具体类型名；
- 同一测试项目内的文件数量和目录布局；
- 场景使用 record、builder、factory 或显式参数表；
- 一个大 Gate 拆成多个更小审批批次；
- TG3、TG4、TG5 的内部先后顺序，但不得越过 TG1/TG2 基线；
- 是否为某个 native adapter 保留少量 target-coupled case；
- 失败输出的具体格式，只要稳定且可定位；
- 第 6.3 节约 150 行审查触发器，可依据真实耦合说明调整。

不得调整掉：

- L0/L1/L2/L3 证据等级区分；
- 当前保守 Hidden/Known/Unknown 与 direct-only 行为；
- event order、granularity、lane 和 modifier category；
- lifecycle 创建、提交、临时隐藏、永久清理和跨战斗隔离；
- stable/beta 同源测试；
- G6 identity migration contract；
- 无真实游戏副作用；
- 每个实施 Gate 单独批准；
- runtime、commit、push、tag、Workshop 的独立授权。

偏离候选方案时必须记录：

```text
Decision ID
Original proposal
Observed constraint
Chosen alternative
Why required invariants remain protected
Files and test IDs affected
Verification and rollback impact
User approval if required
```

---

## 5. 护栏分层

测试层级必须明确。某一层通过不得被描述成更高层已验证。

| 层级 | 名称 | 负责证明 | 不负责证明 |
|---|---|---|---|
| L0 | Repository / Identity Contracts | manifest、项目、产物、关键 ID、持久化/兼容映射的内部一致性 | 游戏能加载、旧安装真实升级成功 |
| L1 | Pure Behavior Contracts | 纯结果、事件、modifier、projection、状态机规则 | Harmony target、Godot 节点、native API 真实语义 |
| L2 | Target-Coupled Contract Harness | 同一生产程序集在 stable/beta 参考 DLL 上可编译和执行、兼容入口存在 | Steam 运行时、BaseLib Workshop 版本集成 |
| L3 | Runtime Matrix | ModLoader、Harmony、Godot、BaseLib、真实战斗生命周期和玩家体验 | 替代 L0-L2 的每次提交快速反馈 |

规则：

- L0-L2 是本任务的自动化范围。
- L3 只设计矩阵并复用已有证据；只有用户明确批准 install/game Gate 后才执行。
- “BuildVerified”“ContractVerified”“RuntimeVerified”必须分开记录。
- 不允许用 mock 的 native 类型证明真实 Harmony/Godot 集成。

---

## 6. 本任务允许触及的 seam

本节是候选范围，不是一次性全部修改授权。每个 Gate 仍需精确文件批准。

### 6.1 现有测试入口

```text
tests/STS2PartyWatchCode.ContractTests/STS2PartyWatchCode.ContractTests.csproj
tests/STS2PartyWatchCode.ContractTests/Program.cs
```

允许在同一测试项目下新增分类文件，例如：

```text
TestCase.cs
TestRunner.cs
ForecastResultContractTests.cs
ProjectionContractTests.cs
HpLossModifierContractTests.cs
PoisonPolicyContractTests.cs
LifecycleContractTests.cs
IdentityContractTests.cs
Fixtures/*.cs
```

名称可调整。不得为了整洁新建第二套功能重叠的测试项目。

### 6.2 当前纯或接近纯的生产 seam

```text
Forecast/ForecastResult.cs
Forecast/LocalDamageForecast.cs
Combat/IncomingDamageRead.cs
Combat/IncomingDamageDisplayRead.cs
Combat/IncomingDamageDisplayOptions.cs
Combat/UpcomingHpLossEvent.cs
Combat/VerifiedHpLossResultModifier.cs
Combat/EnemyPreActionSurvivalPreview.cs
UI/PartyWatchHudSnapshotStore.cs
Combat/ObservedHpLossBudgetTracker.cs
Settings/PartyWatchConfigText.cs
Patches/PartyWatchBaseLibTitlePatch.cs
```

### 6.3 最小 seam 提取规则

允许的 seam：

- 把“从 native 对象读取输入”与“对值执行纯策略”分开；
- 让生产 adapter 继续读取 `Player`、`Creature`、Power、Relic，再把不可变输入交给纯策略；
- 把状态转移从静态存储包装器中提取为内部纯状态机；
- 将 private 纯函数提升为 `internal`，由现有 `InternalsVisibleTo` 测试；
- 引入小型 immutable input/result record；
- 保持原有 public/internal 调用入口和返回语义不变。

默认禁止：

- `#if TEST` 分叉生产业务行为；
- 在测试中执行真实 `CreatureCmd`、Hook damage 命令或卡牌流程；
- 为测试伪造复杂 native 对象图并把它当成 runtime 证据；
- 反射 private 生产字段来绕过缺失的设计 seam；
- 引入通用 DI 容器、mocking framework 或 snapshot framework；
- 新增运行时 DLL；
- 把统一 Forecast Engine 模型提前塞进本任务；
- 为了可测而公开本应 internal/private 的 API。

如果一个 seam 提取会改变生产分支或超过约 150 行非测试 diff，停止并单独报告。这个数字是审查触发器，不是可以通过机械拆行规避的硬配额。

---

## 7. 测试 runner 规范

默认继续使用零新增第三方测试包的 executable harness。若下一 Session 推荐 xUnit/NUnit/MSTest，必须先说明干净构建、依赖恢复、双目标和维护收益，并取得单独批准；不得为框架迁移阻塞行为护栏。

runner 最低能力：

- 每个 case 有稳定 `ID`、category、name；
- 表格场景可以自动展开为独立 case；
- 自动统计 discovered/passed/failed/skipped；
- 成功数量不再硬编码在字符串中；
- 失败输出包含 case ID、期望、实际和最小上下文；
- 一个失败不阻止后续独立 case 执行；
- 退出码 `0` 表示全部通过，非 `0` 表示至少一个失败或 runner 错误；
- 默认不重试失败，不掩盖 flaky；
- 场景顺序固定或由稳定 ID 排序，不依赖字典/反射发现顺序；
- 不读取系统时间、随机数、区域格式或用户语言作为断言输入；
- 输出不包含无界 native object dump；
- stable/beta 输出的 case 集合必须可比较。

推荐命名：

```text
FR-001  Forecast.Unknown.WithTrustedDirect_ReturnsDirectOnly
PR-001  Projection.Default_ExpectedOnly
HM-001  HpLoss.IntangibleThenTungstenThenBudget
PO-001  Poison.Ordinary.ExactLethal_RemovesIntent
LC-001  Lifecycle.CoveringScreen_DoesNotDiscardCommittedSnapshot
ID-001  Identity.ManifestStemMatchesModIdAndAssembly
CP-001  Compat.ModifyDamageSignature_IsSupported
```

测试数量不是 DoD。只有覆盖矩阵和失败可诊断性满足时才算完成。

---

## 8. 分阶段实施计划与 Gate

后续 Gate 不自动继承授权。每个 Gate 使用小 diff、单一目的和可回滚边界。

### TG0 — Baseline And Coverage Revalidation

类型：只读

动作：

- 检查 branch、remote、HEAD、tags、status 和用户改动；
- 核对 test project、`InternalsVisibleTo`、BaseLib 和 stable/beta reference roots；
- 运行只读文件/代码清单；
- 若用户批准执行命令，再分别运行当前 stable/beta 28-case baseline；
- 建立“当前已覆盖 / 可纯测 / 必须 runtime / 暂不支持”矩阵；
- 给出 TG1 精确 diff 计划。

完成门槛：

- 未修改文件；
- 未安装或启动游戏；
- 未 stage/commit/push/tag；
- 用户收到基线和 TG1 审批请求。

### TG1 — Named Runner Foundation

目标：重组 harness，不增加生产语义。

建议范围：

- 引入具名 case/runner；
- 将现有 28 个断言无语义变化迁移；
- 自动计算总数；
- 保留 stable/beta 命令；
- 输出精确失败信息。

完成门槛：

- 迁移前后 28 个行为断言一一可追踪；
- stable/beta case 集合相同且全部通过；
- 生产源码无修改，或只修改测试可见性元数据且有解释；
- 无新增第三方包，除非单独批准；
- 一个独立 commit 候选，但 commit 仍需批准。

回滚点：TG1 单独 diff/commit。

### TG2 — Core Result And Projection Characterization

目标：冻结当前基础语义，不触碰 native 读取。

最低范围：

- `IncomingDamageRead -> LocalDamageForecast -> ForecastResult`；
- Hidden、Unknown、Known、零输出、负值、direct-only fallback；
- `IncomingDamageDisplayRead` 非正值隐藏；
- expected、incoming 和 breakdown 的显示资格/投影纯策略；
- display mode 和 placement 不改变数值；
- 非法 enum/输入安全降级。

必要时允许提取小型纯 projection helper，但旧 HUD 输出必须仍走等价路径。

完成门槛：

- 所有当前状态转换都有表驱动 case；
- 测试断言行为，不断言私有方法实现或文件名；
- stable/beta 全通过；
- 玩家显示、默认设置、UI 节点和 native Reader 无变化。

回滚点：TG2 单独 diff/commit。

### TG3 — Event, Block And HP-Loss Modifier Guardrails

目标：把最危险的顺序/粒度行为放进纯策略测试。

建议 seam：

```text
native adapter reads current powers/relics/budget
    -> immutable modifier inputs
    -> pure ordered modifier policy
    -> existing HpLossResultModificationResult
```

最低场景：

- blockable/direct lane 汇总；
- 事件按显式 native order；
- 负 amount 钳制；
- Intangible 单 direct event；
- aggregate direct + Intangible -> Unsupported；
- TungstenRod 逐事件减免；
- aggregate event + TungstenRod -> Unsupported；
- BeatingRemnant 已花费/剩余/零/非法预算；
- `Intangible -> TungstenRod -> BeatingRemnant`；
- power/relic modifier 分别关闭；
- unsupported 返回已完成 lanes 的当前保守语义；
- current/Power/Relic Block 选择组合，且未选择类别不进入 `N`。

完成门槛：

- 游戏对象只在 adapter 层读取；
- 纯策略测试不需要构造真实 Relic/Power/Creature；
- 原生产入口签名可保留为 wrapper；
- 不借机改变真实原生顺序；
- stable/beta 全通过。

回滚点：modifier seam 和测试作为单一目的 commit 候选。

### TG4 — Poison Policy Characterization

目标：冻结当前已经声明支持的 Poison 核心算法与保守边界，不模拟完整敌人回合。

最低场景：

- 无 Poison；
- ordinary no-kill / kill / exact lethal；
- Accelerant trigger count 上限；
- Poison 每 tick 递减求和；
- Intangible exact lethal 窄规则；
- Slippery 逐层/逐 tick 消耗；
- HardenedShell budget 小于 HP、等于 HP、足以致死；
- HardToKill/native preview 可用与不可用的保守路径；
- unsupported 特殊组合保留原 Intent；
- 多敌人只移除对应 stable identity 的 intent contribution。

允许把当前 private 的纯 Poison 数学提升到小型 internal policy。native `Creature`/Power 发现、反射和 `CalculateTotalDamageNextTurn()` 调用仍留在 adapter/target-coupled 层。

完成门槛：

- 不执行 real Poison、damage、death、remove 或 enemy turn；
- 不新增 Nemesis/ToughEgg 支持；
- 不把尚未 runtime 验证的组合升级为 RuntimeVerified；
- 测试预期与 `mechanics-evidence.md` 当前边界一致；
- stable/beta 全通过。

回滚点：TG4 单独 diff/commit。

### TG5 — Lifecycle And Session-State Guardrails

目标：把最容易跨帧/跨战斗残留的状态转移提取为可测试规则。

建议 seam：

- `PartyWatchHudSnapshotStore` 保留 native wrapper；
- 纯状态机只接收稳定 player/creature identity、事件和 `ForecastHudSnapshot`；
- `ObservedHpLossBudgetTracker` 的预算计算与 native identity 存储分开；
- covering-screen temporary hide 与 permanent invalidation 使用不同事件。

最低场景：

- player turn start 清空旧 committed/live 状态；
- live result 更新；
- turn ending 提交最终 snapshot；
- freeze off 直接显示 latest；
- covering screen hide 不丢 committed snapshot；
- covering screen close 恢复同一 committed snapshot；
- combat end 清空 snapshot 与 HP-loss budget；
- 连续两场战斗不复用旧状态；
- player/creature identity 切换；
- invalid/hidden result 不成为可显示 committed snapshot；
- mid-combat debug relic injection 仍作为非生产边界，不为它改变生产语义。

完成门槛：

- 所有静态状态有明确 owner、创建、重置和 clear 测试；
- 不创建 Godot 节点；
- 不移动 Harmony patch 或重写 ForecastRefreshPatch；
- AUD-0005 和 AUD-0013 的已关闭语义被测试固定；
- stable/beta 全通过。

回滚点：TG5 单独 diff/commit。

### TG6 — Identity, Persistence And Packaging Guardrails

目标：为 G6 防止“改了一半”，同时允许经批准的原子迁移。

本 Gate 分成两类断言：

#### A. 永久行为不变量

- 玩家名仍为 `Damage Forecast` / `伤害预测`，除非单独产品决策；
- single-player/local-player multiplayer 边界不变；
- manifest、assembly、DLL、BaseLib registration、Harmony owner 和安装目录必须形成一个明确兼容方案；
- settings 升级不得静默重置；
- 只加载一个 active Mod identity；
- 历史 evidence 不因技术改名被批量重写；
- Workshop ID/visibility 不得由 G6 测试任务改变。

#### B. 可迁移技术期望

当前基线先断言：

```text
Mod ID / manifest stem / assembly = sts2-party-watch-v2
root namespace = STS2PartyWatch
project path = src/STS2PartyWatchCode
BaseLib key / Harmony owner = sts2-party-watch-v2
diagnostic prefix = [STS2 Party Watch]
```

G6 获批后，不允许零散改写这些断言。G6 必须先提供一个显式 migration contract，至少记录：

```text
Identity field
Legacy value
Target value
Must change / must remain / compatibility alias
Persistence impact
Installed-artifact impact
Rollback behavior
Static verification
Runtime verification
```

测试应验证 contract 内部一致性，而不是永远硬编码旧名字。测试更新必须与对应兼容实现位于同一可审查 Gate，不能先把测试改绿再留下一半迁移。

最低 L0 检查：

- manifest JSON 合法；
- manifest filename stem、manifest `id`、预期 DLL/assembly 的关系一致；
- `has_dll=true`、`has_pck=false`、依赖和 `min_game_version` 保持预期；
- project/output/publish/install scripts 指向同一 active identity；
- BaseLib key、Harmony owner 和 persistence disposition 可追踪；
- publish whitelist 仅 DLL + manifest；
- 仓库不跟踪 DLL/PDB/PCK/log/bin/obj/publish/work/uploader/game files；
- 历史文档中的旧标识不会被 L0 当成失败，扫描只针对 current authority、源码、脚本和 active paths。

完成门槛：

- 当前技术身份一致性测试通过；
- G6 可直接复用 migration-contract 结构；
- 测试不会误扫全部历史笔记；
- 没有执行实际 G6；
- 没有修改 Mod ID、文件名、namespace、持久化 key、安装路径或 Workshop。

回滚点：TG6 单独 diff/commit。

### TG7 — Dual-Target Entry, Evidence And Closure

目标：形成下个 Session 可重复运行的一条权威测试入口并正式交接给 G6/AR1。

建议能力：

- 一个仓库脚本或清楚文档化的命令按顺序运行 stable/beta contract harness；
- 复用现有 frozen reference roots 和 BaseLib bootstrap；
- 输出每目标 case 数、失败列表、耗时和最终退出码；
- 可选择只跑一个目标用于开发，但 closure 必须双目标；
- 不自动安装、不启动游戏、不上传 Workshop；
- 输出写入 ignored `work/` 或仅终端，不提交二进制结果。

完成门槛：

- stable contract：全部通过；
- beta contract：全部通过；
- stable/beta Release build：0 warnings / 0 errors；
- 如本 Gate 获批 publish：两个 publish tree 均通过 whitelist；
- `git diff --check` 通过；
- staged/working tree 审查无禁入产物；
- 更新本卡 closure、`build-environment.md` 的唯一测试命令和任务索引；
- commit/push/tag 仍需用户单独批准。

推荐 closure tag，仅在用户明确批准后：

```text
forecast-test-guardrails-baseline
```

---

## 9. 最低自动化矩阵

本矩阵是语义要求。具体 case 数、类名和文件布局可调整，但删减必须记录理由并获批。

### 9.1 Forecast state

- Hidden input -> Hidden；
- Known raw > block -> Known out damage；
- Known raw <= block 且无 direct -> Hidden；
- Unknown blockable + trusted direct -> Known direct-only；
- Unknown 且无 trusted direct -> Unknown；
- negative raw/block/direct 的现有钳制或保守行为；
- 非法 state 不产生猜测值；
- lane 总量不溢出或按明确策略安全失败。

### 9.2 Projection

- 默认 expected-only；
- incoming-only；
- both；
- placement left/right 数值相同；
- current/Power/Relic Block 的全部关键组合；
- Power/Relic modifier 的独立开关；
- breakdown 与 expected 总量一致；
- Unknown 不显示误导性 partial incoming；
- direct-only expected 不被 incoming projection 错误继承。

### 9.3 Event/modifier

- stable order；
- equal order 的明确策略或禁止输入；
- blockable/direct lane 保留；
- single vs aggregate granularity；
- Intangible；
- TungstenRod；
- BeatingRemnant；
- 三者组合顺序；
- invalid budget；
- modifier category switches。

### 9.4 Poison

- ordinary/no Poison；
- kill/no-kill/exact lethal；
- Accelerant；
- Intangible 窄规则；
- Slippery；
- HardToKill/native preview fallback；
- HardenedShell budget；
- unsupported combination keeps intent；
- per-enemy identity isolation。

### 9.5 Lifecycle

- live/frozen/committed；
- turn start/end；
- covering hide/restore；
- permanent invalidation；
- combat end；
- continuous combat；
- identity switch；
- settings refresh 不改变已计算语义；
- relic/hand refresh event 只触发刷新，不重复改变 state。

### 9.6 Identity/compatibility

- current active identity coherence；
- migration contract schema；
- persistence disposition completeness；
- manifest/build/install path consistency；
- stable/beta hook capability；
- BaseLib title/current product copy；
- diagnostic once-only；
- artifact whitelist；
- forbidden tracked files。

---

## 10. Runtime 验证边界

测试护栏完成不等于所有 runtime 行为重新验证。下列项目继续属于 L3，并只在用户明确批准后运行：

1. Steam/ModLoader 实际只加载一个 Mod。
2. 默认 `-N`、incoming-only、both、左右位置。
3. current/Power/Relic Block 各一例。
4. Intangible、TungstenRod、BeatingRemnant 代表场景。
5. ordinary Poison 与已支持特殊边界代表例。
6. turn-end freeze。
7. covering screen hide/restore committed snapshot。
8. relic add/remove/melt refresh。
9. 连续两场战斗 snapshot/budget 清理。
10. BaseLib 英文/简中切换和完整重启持久化。
11. 本机多人 HUD；不声明 teammate/shared HUD。
12. G6 时的旧安装升级、配置连续性、重复 artifact 和 rollback smoke。

记录格式：

```text
Runtime ID
Artifact hash
Game version/branch
Installed paths
Settings
Scene setup
Expected
Observed
Pass/Fail/Blocked
Evidence/user confirmation
```

---

## 11. G6 Readiness Gate

只有以下条件全部满足，才建议用户批准 G6 实施：

- TG1-TG7 已关闭；
- stable/beta 自动护栏全绿；
- current identity coherence baseline 已记录；
- persistence keys 和 active paths 已清单化；
- G6 migration contract 模板可直接填写；
- 玩家行为测试不依赖 `PartyWatch*` 类名或旧 namespace；
- 历史文档扫描已排除，避免 G6 批量改写 evidence；
- runtime upgrade/rollback 矩阵已写好但未擅自执行；
- 工作区不存在未归属改动。

如果用户选择先做 G6 的纯 source-symbol rename，也仍必须先通过相应 TG baseline。不得把“只是改名”当作无需行为护栏的理由。

---

## 12. Forecast Architecture Readiness Gate

只有以下条件全部满足，才建议架构卡进入 AR2 及以后：

- 本任务形成的 runner 是唯一 contract 入口；
- core result/projection、modifier、Poison 和 lifecycle 场景都可重复运行；
- pure seam 与 native adapter 的边界清楚；
- 架构重构不会要求测试直接构造 Godot/native 大对象图；
- legacy/candidate shadow comparison 可以复用现有 scenario fixtures；
- 现有行为预期具有稳定 case ID；
- 未解决测试缺口已列为显式风险，而不是被“全部通过”掩盖。

本任务不得提前满足架构卡的下列内容：统一 event model、candidate engine、shadow production path、rule catalog、single-evaluation cutover、HUD coordinator split 或 settings schema consolidation。它只提供测试基础。

---

## 13. 风险登记与控制

| Risk | 影响 | 控制 |
|---|---|---|
| 测试锁死旧技术名字 | G6 无法有意迁移 | 行为契约与 identity contract 分层；G6 显式更新 migration contract |
| 为可测性提前重构 | 测试任务变成架构大改 | seam 限额、逐 Gate 批准、禁止 unified engine |
| 只增加断言数量 | 虚假安全感 | 以矩阵、失败定位和层级证据验收，不以数量验收 |
| mock native 对象过多 | 测试与真实游戏偏离 | 纯值策略 + native adapter；runtime 仍单列 |
| runner 框架依赖膨胀 | clean build/双目标变脆 | 默认零新增包；框架迁移单独批准 |
| stable/beta case 漂移 | 只保护一个版本 | 同一测试源、case-set 对比、双目标 closure |
| brittle 文本快照 | 无关文案变化导致大量失败 | 断言结构化结果；仅精确产品文案使用字符串契约 |
| static state 泄漏 | 测试相互污染、flaky | 每 case 独立 state；显式 reset/clear；禁止重试掩盖 |
| Poison 过度声明 | 把纯算法当 runtime 证明 | L1/L2/L3 分级；保留 RuntimeVerified 边界 |
| Identity 扫描历史文档 | 迫使改写历史证据 | 只扫 current authority、active source/scripts/paths |
| 测试更新先于实现 | 半迁移被伪装成绿色 | 迁移 contract、兼容实现和期望更新同 Gate 审查 |
| 产物/游戏文件误入 Git | 仓库污染 | whitelist + forbidden tracked-file guard + staged review |

---

## 14. Stop / Escalation Conditions

遇到以下任一情况立即停止当前 Gate 并报告：

- 测试预期与两个权威文件冲突且无法确定哪个是当前行为；
- 需要改变玩家可见语义才能让测试通过；
- 需要决定新的 G6 identity；
- 需要修改持久化 key、Mod ID、assembly、namespace 或安装路径；
- pure seam 无法在小 diff 内提取；
- stable/beta 对同一行为产生无法解释的不同结果；
- 需要真实安装/启动游戏但尚未批准；
- 需要新运行时 DLL、重量级依赖或新增网络访问；
- 发现用户未提交改动与当前 Gate 重叠；
- 自动化无法证明的场景被要求写成 RuntimeVerified；
- 必须更改 Workshop 或游戏目录；
- 测试出现 flaky、依赖执行顺序或重试后才通过。

报告格式：

```text
Blocking condition
Evidence
Affected test IDs / invariant
Files involved
Safe alternatives attempted
Recommended options
Required user decision
```

---

## 15. Definition Of Done

### Harness

- 一个权威 contract test project/入口。
- case 自动发现或显式登记，自动计数。
- 稳定 case ID、category 和精确失败输出。
- 无硬编码成功总数。
- 无默认 retry 或顺序依赖。

### Coverage

- 第 9 节矩阵全部有自动 case，或有获批的明确 L3/deferred disposition。
- AUD-0002、0004、0005、0011、0013 的已关闭行为有对应护栏。
- `N`/`-N` 当前差异和共同输入边界被固定。
- identity coherence 与 future migration contract 可执行。

### Safety

- 无真实 damage/card/command/RNG/save/network/Workshop 行为。
- 无玩家可见行为、默认值、技术身份或 Workshop 变化。
- test seam 不扩大 production public API。
- 没有第二个 runtime DLL。

### Build

- stable v0.107.1：tests pass，Release build 0 warnings / 0 errors。
- beta v0.109.0：tests pass，Release build 0 warnings / 0 errors。
- publish 如获批则仅包含 manifest + DLL。
- `git diff --check` 通过。
- 无 forbidden artifacts 被跟踪或暂存。

### Documentation and handoff

- 本卡 closure 填写完成。
- `docs/build-environment.md` 记录唯一权威测试命令。
- `docs/task-notes/README.md` 路由更新。
- 架构卡 AR1 状态说明已同步，但不提前批准 AR2。
- G6 readiness 和 architecture readiness 分开报告。
- commit/push/tag 仅在用户明确批准后完成并记录。

---

## 16. 建议提交边界

以下仅是建议，任何 commit/push/tag 都需用户单独批准：

```text
test: add named contract test runner
test: characterize forecast result and projection semantics
test: characterize HP-loss event modifier order
test: characterize conservative poison policies
test: characterize forecast lifecycle state
test: guard technical identity and packaging coherence
test: add dual-target guardrail entry
docs: close forecast test guardrails
```

规则：

- 不把全部 Gate squash 成一个不可审查提交；
- 不 force-push；
- 不在红灯状态打 tag；
- 生产 seam 与对应测试必须在同一个小批次；
- runner 重组不得与机制 seam 混在一个提交；
- docs closure 单独提交；
- annotated tag 只在最终双目标 closure 后创建。

---

## 17. 每个 Session 的 checkpoint 模板

```text
Date/session
Current TG stage
Approved scope
HEAD / branch / remote
Dirty files before work
Files changed by this session
Production seam changed: None / list
Player behavior changed: None / list
Test cases before / after
New case IDs
Stable test/build exact result
Beta test/build exact result
Runtime performed: Yes/No
Install performed: Yes/No
Commit/push/tag performed: Yes/No + hashes
Workshop changed: No unless explicitly approved
Deferred L3 items
Decisions/deviations
Rollback point
Exact next action
Approval required next
```

关键进度必须写入文件，不得只留在聊天里。

---

## 18. 下一 Session 的第一批建议动作

下一 Session 只做 TG0：

1. 读取本卡及指定 authority。
2. 检查工作区与基线。
3. 逐项清点现有 28 个断言并给出稳定 ID 草案。
4. 判断 `Program.cs` 可如何仅在测试项目内拆分。
5. 给出 TG1 精确文件列表和预计 diff。
6. 报告，不修改文件。
7. 请求用户批准 TG1。

第一批实现即使获批，也只能做 runner foundation。不得同时提取 modifier、Poison、lifecycle seam。

---

## 19. 可直接复制给下一 Session 的启动提示

```text
请完整阅读：
C:\Users\ROG\Documents\Codex\STS2-Party-Watch-v2\docs\task-notes\forecast-engine-test-guardrails-master-task-card.md

本次只执行 TG0：只读基线、现有 28 项测试清点、覆盖缺口分类和 TG1 最小 diff 设计。

不要修改文件，不要 restore/build/publish，除非我明确批准对应命令；不要安装、启动游戏、stage、commit、push、tag、更新 Workshop、执行 G6 或开始 Forecast Engine 重构。

完成后报告：
1. HEAD/branch/remote/status；
2. stable/beta 当前 harness 基线；
3. 28 个断言的分类和稳定 ID 草案；
4. TG1 预计修改文件与回滚点；
5. 本任务卡与当前代码的任何偏差；
6. 请求 TG1 批准。
```

---

## 20. Closure 预留

Final status: Closed; TG0-TG7 completed on 2026-07-21.

Final commit: None; commit/push remains separately gated.

Final tag: None; `forecast-test-guardrails-baseline` was not created.

Runner entry: `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Test-ForecastGuardrails.ps1`

Final case count and categories: 152 unique named cases — BurnShape 5, ForecastResult 14, IncomingDisplay 2, Diagnostics 2, ConfigText 4, BaseLibTitle 7, VisibilityProfiler 7, Projection 16, BlockPolicy 14, HpLossModifier 18, PoisonPolicy 24, Lifecycle 19, IdentityPackaging 16, QualityGate 4.

Stable tests/build: `v0.107.1 / 59260271`; 152/152 contracts pass; Release build 0 warnings / 0 errors.

Beta tests/build: `v0.109.0 / c12f634d`; 152/152 contracts pass; Release build 0 warnings / 0 errors.

Production seams added: pure HUD projection, Block/event, HP-loss modifier, Poison, HUD snapshot lifecycle, and HP-loss budget policies; native Godot/game objects remain in adapters/wrappers.

Behavior changes: conservative invalid projection normalization and explicit stable-order/lifecycle handling only; supported player-visible forecast formulas and product boundaries remain unchanged.

Identity guard status: current identity is internally consistent; 18 persisted settings are inventoried; one active manifest and DLL+manifest packaging whitelist are enforced; future migration targets remain undecided in the reusable migration contract.

G6 readiness: test foundation is ready for a separately approved formal G6 master task card. No technical identity migration was executed.

Architecture AR1 readiness: Characterization Test Foundation is satisfied for the covered L0/L1/L2 seams; AR1 and later architecture implementation remain separately gated.

Deferred L3/runtime matrix: incoming `N` modes/placement/category switches, advanced details, ordinary Poison full matrix, combined Tungsten Rod/Beating Remnant order, consecutive-combat runtime isolation, NCardPileScreen, exact relic-add refresh, dropdown popup item text, and matching-artifact branch smoke remain at their recorded Pending/Deferred levels.

Final artifacts/hashes if produced: none; TG7 did not publish.

Forbidden-artifact review: `git diff --check` passed; tracked and working-tree review found no DLL/PDB/PCK/log/EXE or bin/obj/publish/work/uploader artifact.

Workshop state: untouched; no upload, visibility, ID, install, or launch operation was performed.

Post-closure audit: on 2026-07-21, review found two corrections. First, the TG4 Slippery adapter read every modified Poison tick before the pure policy could stop on an already lethal prefix; the adapter now preserves the legacy lethal early-stop behavior, and `PO-024` guards a lethal partial sequence so later Hook reads are not required. Second, the TG7 contract command still used project-local ignored `bin/obj`; it now uses .NET `--artifacts-path` so new quality-gate contract output is isolated below ignored `work/forecast-guardrails/`, with `QG-002` enforcing the route. Existing ignored caches were not deleted. The fresh authoritative stable+beta run passed 152/152 contracts and both Release builds with 0 warnings / 0 errors.

Exact next optional task: the formal G6 Full Technical Identity Migration task card now exists as `full-technical-identity-migration-master-task-card.md`, status Proposed / Not Started. The next optional Gate is separately approved read-only G6-0; do not execute migration automatically.
