# Damage Forecast — Forecast Engine 架构稳定化主任务卡

日期：2026-07-21

状态：Proposed / Not Started / 本任务卡本身不授权代码修改

仓库：`https://github.com/Niizuma24290/Damage-Forecast.git`

当前分支：`main`

任务类型：内部架构重构、测试能力建设、变更扩散治理；默认不改变玩家可见行为

与 G6 的关系：本任务不是 Full Technical Identity Migration。不得借此修改 Mod ID、程序集、命名空间、持久化 key、Workshop ID 或本地安装路径。

---

## 0. 给下一 Session 的执行指令

下一 Session 必须先完整阅读本任务卡，再读取：

1. `docs/architecture.md`
2. `docs/project-state.md`
3. `docs/task-notes/phase-12c-audit/code-audit-findings.md`
4. `docs/task-notes/phase-13a-damage-display.md`
5. 本任务卡列出的核心源码与测试文件

首次进入时只允许执行 AR0 基线复核和只读分析。不要因为本任务卡存在就默认获得代码修改、安装、启动游戏、commit、push、tag 或 Workshop 权限。

AR0 完成后，向用户报告：

- 当前基线是否仍与本卡一致；
- 是否存在用户未提交改动；
- 推荐的第一批最小 diff；
- 第一批预计触及的文件；
- 是否需要调整本卡中的候选设计。

只有用户明确批准后，才能进入 AR1 及后续实现阶段。

---

## 1. 一句话任务目标

把当前“每加一个功能就修改多个中央文件”的增量堆叠结构，渐进改造成“统一事件评估 + 分阶段规则注册 + 单一结果投影 + 明确生命周期协调”的架构，使新增普通机制主要落在独立规则和测试中，同时保持当前保守预测语义、双版本兼容、HUD 行为和玩家设置不变。

---

## 2. 背景与问题定义

当前 Mod 已经实现并验证大量独立机制，但其扩展方式主要是持续向中央 Reader、HUD refresh、设置和特殊机制文件中增加条件分支。

已确认的结构热点：

| 文件 | 当前规模/职责 | 主要维护风险 |
|---|---|---|
| `Combat/LocalIncomingDamageReader.cs` | 733 行；读取敌人、手牌、Power、Block、遗物、事件顺序，并分别构建 `-N` 与 `N` | 两条平行流程会重复遍历和重复适配同一机制 |
| `Patches/ForecastRefreshPatch.cs` | 651 行；Harmony 入口、计算触发、快照、节点、刷新、回合生命周期 | UI、生命周期和计算触发相互牵连 |
| `Settings/PartyWatchBaseLibConfig.cs` | 524 行；持久化属性、顺序、UI 创建、文本重写、反射兼容 | 新增设置需要在多处重复登记 |
| `Combat/EnemyPreActionSurvivalPreview.cs` | 366 行；Poison 及多个特殊 Power/敌人边界 | 特殊情况继续增长时会形成条件分支树 |

近期提交热度也表明 UI/协调层已经成为变更中心：在 G5 前的 36 个累计提交中，`PartyWatchHudDisplay.cs` 被修改 15 次，`ForecastRefreshPatch.cs` 被修改 10 次。

当前最关键的结构性重复：

- `LocalIncomingDamageReader.ReadKnown(...)` 构建默认预计 HP Loss `-N`；
- `LocalIncomingDamageReader.ReadIncomingDamageKnown(...)` 为可选来袭总伤害 `N` 重新遍历同一批敌人、手牌、Power 和 modifier；
- 两条流程对 unsupported、Block、事件粒度和 modifier 的处理并非由同一个中间结果投影，因此未来容易发生语义漂移。

当前测试现状：

- 现有 executable contract harness 有 28 项断言；
- 已覆盖保守卡牌形状策略、基础结果钳制、配置文案、BaseLib 标题兼容和 AUD-0007 profiler；
- 复杂事件顺序、Block 消耗、Poison 特殊边界、Intangible/TungstenRod/BeatingRemnant 组合、快照状态机和 `N`/`-N` 一致性仍主要依赖人工/运行时验证。

这不是当前功能错误结论，而是未来变更成本和回归风险结论。

---

## 3. 当前权威基线

下一 Session 必须重新验证以下数据，不得盲信：

```text
Baseline repository: Niizuma24290/Damage-Forecast
Baseline branch: main
Known repository closure commit: e0f770c
Known architecture/source baseline commit: 0de7d74
Known release marker: v0.1.0
Known closure marker: g5-repository-closure
Known manifest version: v0.1.0
Known Mod ID: sts2-party-watch-v2
Known stable target: v0.107.1
Known beta target: v0.109.0
Known contract count: 28/28 per target
Known final DLL size: 112,128 bytes
Known final DLL SHA256: 1BAC85248FD83B62BE9867FD3A2F8F3B54D676D96D3F6B25F71B720A62D544F0
Known final manifest SHA256: A0CCDE08C2C7C0DB1C5D3BFC374C7523DFF0ECF492D00E6F154BF1AE1DAC5E11
```

如果基线已变化：

- 不自动 reset、checkout、restore 或覆盖；
- 先识别哪些变化属于用户；
- 报告与本卡假设的差异；
- 根据当前代码调整实施计划；
- 未经批准不得为了匹配本卡而回退较新的工作。

---

## 4. 成功定义

本项目完成时必须同时满足：

1. `N`、`-N` 和明细来自一次统一评估，而不是两套独立游戏状态遍历。
2. 新增一个普通伤害来源、Block 来源、行动前存活规则或 HP-loss modifier 时，不需要修改中央算法分支树。
3. 每个机制具有独立、可定位、可单测的规则边界。
4. 游戏版本差异与反射集中在兼容层，不散落到业务规则。
5. HUD patch 只转发游戏事件；计算、生命周期、视图更新由独立组件协调。
6. 配置持久化值可一次性映射为不可变运行时设置快照。
7. 复杂预测规则拥有确定性测试；Harmony/Godot/真实游戏集成继续由双目标构建和运行时矩阵覆盖。
8. 默认玩家可见行为、默认配置、技术身份、Workshop 状态和保守 Unknown 策略保持不变，除非用户另行批准明确的行为修正。
9. 最终生产产物仍符合仓库既有发布白名单和双目标一致性要求。

维护性验收示例：

> 在测试中增加一个 synthetic damage contributor，除新增规则/fixture、规则目录登记和对应测试外，不应修改 HUD、快照、BaseLib、Harmony patch 或另一条平行计算流程。

该示例描述目标结果，不强制使用特定接口名或文件名。

---

## 5. 硬性不变量

下列要求是规范性约束，设计调整不能绕过：

### 5.1 只读安全

- 不执行真实伤害。
- 不执行卡牌、命令队列、Power/Relic 命令或 RNG。
- 不修改战斗状态、存档、网络状态、玩家/敌人状态或房间状态。
- Native preview 调用必须维持现有只读边界。

### 5.2 保守正确性

- 不可信或不完整的总量不得显示猜测值。
- 当前支持的 direct-only fallback 不得因重构被意外删除。
- 事件顺序或粒度不可信时，必须保留 Unknown/保守降级能力。
- 不得把聚合伤害伪装成逐 hit 事件。
- 不得把本机多人 HUD 扩写成队友或共享队伍预测。

### 5.3 玩家行为兼容

- 默认仍只显示预计 HP Loss `-N`。
- `N`、左右位置、Block/modifier 开关、详情、颜色、冻结和多人本机开关保持当前持久化语义。
- BaseLib 当前英文/简中标题、标签和完整重启持久化不得回归。
- 覆盖界面隐藏、回合冻结、战斗结束清理和本机玩家判断不得回归。

### 5.4 技术身份保持

除非用户单独批准 G6：

- Mod ID 保持 `sts2-party-watch-v2`；
- manifest 文件名、程序集/DLL、项目、namespace、Harmony owner、BaseLib key、路径和诊断前缀保持不变；
- Workshop item `3755598583` 不更新；
- 不修改 Workshop visibility；
- 不把本架构项目与品牌/技术身份迁移混做。

### 5.5 版本兼容

- stable v0.107.1 和 beta v0.109.0 均必须构建和运行 contract harness；
- 不得只为了一个版本简化掉另一个版本的兼容路径；
- capability detection 失败时必须有明确、保守、可诊断的 fallback。

---

## 6. 明确非目标

本项目默认不包含：

- Phase 13B 或其他新玩家功能；
- 新敌人、新遗物、新卡牌或新 Power 支持；
- HUD 视觉重设计；
- 全面多人/队友 HUD；
- Workshop 发布或营销资料更新；
- G6 全技术身份迁移；
- 以性能为理由重写 AUD-0007 已证明成本非实质性的 visibility patch；
- 将代码拆成多个运行时 DLL；
- 引入重量级 DI 容器、通用规则脚本语言或运行时插件发现；
- 为了减少行数而牺牲明确的保守边界。

如果执行过程中发现真实功能缺陷：先登记、隔离和报告。除非修复是继续重构的必要前置条件，否则不要把它混入当前架构 diff。

---

## 7. 弹性设计原则

本卡规定能力边界，不强制具体命名。

以下名称都是候选，可由下一 Session 根据实际代码调整：

```text
CombatSnapshot
ForecastContext
ForecastEvent
ForecastEvaluation
ForecastRuleCatalog
IForecastEventContributor
IPreActionSurvivalRule
IBlockContributor
IHpLossModifier
GameCapabilities
IGameApiAdapter
ForecastCoordinator
HudPresenter
DamageForecastSettings
```

允许调整的内容：

- record/class/interface 的具体名称；
- 文件夹划分；
- 一个接口拆成多个窄接口；
- 显式 catalog 使用数组、构造函数或简单组合根；
- shadow compare 的具体记录格式；
- 先迁移哪一类低风险规则；
- 在保持单 DLL 产物的前提下，纯逻辑代码的逻辑模块边界。

不允许调整掉的能力：

- 单一事件/评估源；
- 明确执行顺序；
- 事件粒度；
- Known/Hidden/Unknown 与原因；
- 规则隔离；
- 版本兼容隔离；
- 生命周期显式清理；
- 新旧结果可比较；
- 双目标验证；
- 无行为变化的阶段性边界。

如果下一 Session 认为候选架构有更安全或更简单的实现，可以修改方案，但必须在任务 note 中记录：

```text
Decision ID
Original proposal
Observed constraint
Chosen alternative
Why invariants remain satisfied
Files/verification affected
```

---

## 8. 建议目标架构

```text
Game/Harmony adapters
        |
        v
Immutable combat snapshot / compatibility façade
        |
        v
Ordered rule pipeline
  - event contributors
  - pre-action survival rules
  - block contributors
  - HP-loss modifiers
        |
        v
Single ForecastEvaluation
  - events
  - derived totals
  - confidence/status
  - reason codes
        |
        +--> Expected HP Loss projection (-N)
        +--> Incoming Damage projection (N + selected options)
        +--> Breakdown projection
        |
        v
Snapshot/session state
        |
        v
HUD presenter
```

### 8.1 统一事件模型最低能力

统一事件至少能够表达：

- stable source identity；
- source category；
- actor/target identity（如计算所需）；
- native execution phase/order；
- Blockable 或 Direct HP Loss lane；
- 原始可信 amount；
- 单事件/逐 hit/聚合粒度；
- 已应用与未应用的转换阶段；
- Known/Unsupported/Unavailable 证据状态；
- 稳定 reason code；
- 可供诊断的机制名称，但不依赖显示名称作为身份 key。

现有 `UpcomingHpLossEvent` 是候选演进起点，但不得仅为复用名称而强行保留不够表达需求的形状。

### 8.2 规则阶段建议

推荐将规则按语义阶段隔离，而不是全部放进一个万能接口：

1. **Source collection**：敌人 Intent、手牌 turn-end、Power turn-end、direct HP loss。
2. **Pre-action survival**：Poison 是否让特定敌人在当前 Intent 前死亡。
3. **Source-specific damage adaptation**：例如 legacy Diamond Diadem。
4. **Block contribution/consumption**：当前 Block、Power Block、Relic Block。
5. **HP-loss result modification**：Intangible、TungstenRod、BeatingRemnant。
6. **Projection**：根据用户设置生成 `N`、`-N` 和 breakdown。

规则顺序必须显式、可读、可测试。不要依赖反射发现顺序、字典枚举顺序或文件加载顺序。

### 8.3 统一评估与投影

统一评估不是只返回一个最终整数。它应保留足够中间信息，让不同显示模式从同一证据生成：

- `-N`：完整支持的预计最终 HP Loss；
- `N`：按用户勾选的 Block/modifier 阶段投影；
- breakdown：Blockable 与 Direct HP Loss；
- Unknown：明确说明是哪一个来源、阶段、顺序或粒度不可用。

严禁通过从 `-N` 反推 `N`，也严禁为 `N` 再遍历一次原生游戏状态。

### 8.4 设置快照

建议把当前长参数映射改成不可变运行时设置快照：

```text
BaseLib persisted properties
    -> one validated DamageForecastSettings value
    -> SettingsChanged(snapshot)
```

设置描述表可以统一维护：

- key/property；
- section；
- order；
- default；
- English/Simplified Chinese label key；
- enum item labels；
- control metadata。

如果 BaseLib API 约束导致静态持久化属性必须保留，可以保留属性作为 persistence façade，但运行时代码只读取一个原子快照。

### 8.5 生命周期协调

Harmony patch 应仅把 native 事件转换为内部信号：

```text
HandChanged
RelicChanged
PlayerTurnStarted
PlayerTurnEnding
CombatEnded
CoveringScreenChanged
SettingsChanged
HealthBarAttached/Resized
```

`ForecastCoordinator` 候选职责：

- 判断是否需要重新读取；
- 合并同一帧重复刷新请求；
- 管理当前 combat/player session；
- 请求 evaluator 计算；
- 管理 live/frozen/committed snapshot；
- 把最终 view model 交给 presenter；
- 在战斗结束、玩家切换或无效节点时清理状态。

HUD presenter 不得读取游戏机制；规则不得创建 Godot 节点。

### 8.6 版本兼容层

启动时一次性构建 capability map，候选内容包括：

- `Hook.ModifyDamage` 10/11 参数签名；
- stable/beta turn-end hook；
- Diamond Diadem legacy/current 机制；
- native Poison preview 方法；
- 必要 private member 是否可读。

业务规则依赖兼容 façade，不自行重复反射。能力不可用时返回结构化 unavailable/unsupported 结果并保留稳定诊断码。

---

## 9. 分阶段实施计划与 Gate

各阶段必须保持小 diff、单一目的和可回滚性。后续阶段不得自动获得授权。

### AR0 — Baseline And Design Revalidation

类型：只读

目标：确认任务卡仍适用于当前仓库。

动作：

- 检查分支、remote、HEAD、tags、status、tracked/untracked/ignored 边界；
- 重新统计核心文件职责和测试覆盖；
- 读取现有架构/审计/Phase 13A 记录；
- 确认 stable/beta reference snapshot 路径和脚本仍可用；
- 列出用户当前未提交改动，不覆盖；
- 提交“建议第一批 diff”给用户批准。

输出：

- 当前基线摘要；
- 变更冲突风险；
- 任务卡偏差记录；
- AR1 审批请求。

完成门槛：没有写源码、没有 stage/commit/push/install/game/Workshop。

### AR1 — Characterization Test Foundation

目标：在改结构前冻结当前行为。

建议范围：

- 建立 game-neutral 或最少 game-coupled 的预测 fixture；
- 为现有事件顺序、Block 消耗和 modifier 规则增加确定性测试；
- 为 `N`/`-N`/breakdown 投影建立场景表；
- 为生命周期状态提取可测试的纯状态机 seam；
- 不改变生产结果。

最低测试矩阵见第 10 节。

完成门槛：

- stable/beta contract harness 均通过；
- 新测试能够在不启动游戏的情况下重复运行；
- 生产代码行为无变化，或仅增加不改变行为的 test seam；
- diff 中没有 HUD 视觉、设置默认值或 Workshop 变化。

回滚点：单独 commit；如后续设计失败，测试仍可保留。

### AR2 — Unified Domain Model Scaffold

目标：引入统一事件和评估类型，但暂不替换生产显示。

建议范围：

- 定义 event、granularity、lane、phase/order、status/reason；
- 定义单一 evaluation 和 projection 输入；
- 允许旧 Reader 输出/适配到新模型；
- 不删除旧 `ReadKnown` 或 `ReadIncomingDamageKnown`。

完成门槛：

- 新类型为纯数据/纯计算边界；
- 全部新增状态有测试；
- 没有玩家可见变化；
- 不引入第二个运行时 DLL。

回滚点：独立 commit。

### AR3 — Candidate Engine And Shadow Comparison

目标：建立候选统一引擎，同时保持旧引擎为显示权威。

Shadow 模式规则：

- 旧结果继续驱动 HUD；
- 候选结果只用于比较；
- 默认生产可关闭或完全剔除 shadow 诊断；
- 不一致记录必须限频并包含稳定 reason code；
- 不记录敏感或无界对象 dump；
- 不因候选引擎异常影响旧显示。

比较至少包括：

- expected state/amount；
- incoming state/amount；
- blockable/direct lanes；
- unsupported reason；
- frozen/live snapshot 边界（如阶段已覆盖）。

完成门槛：

- 纯场景测试中 legacy/candidate 一致；
- stable/beta 构建通过；
- shadow 关闭时不改变生产 artifact 行为；
- 未获得运行时批准时不得安装或启动游戏。

回滚点：独立 commit，可完全关闭候选路径。

### AR4 — Incremental Rule Migration

目标：逐类把机制迁入显式规则目录。

推荐顺序，从低耦合到高风险：

1. fixed direct HP-loss cards；
2. turn-end Power damage；
3. current/Power/Relic Block contributors；
4. enemy AttackIntent event construction；
5. Diamond Diadem compatibility strategy；
6. Poison pre-action survival；
7. Intangible/TungstenRod/BeatingRemnant event modifiers。

每迁移一类必须：

- 保留原有保守边界；
- 增加或迁移对应测试；
- shadow 比较一致；
- 使用单独 commit 或清晰可审查的小批次；
- 不同时修改 HUD 和 BaseLib。

如果某机制无法安全适配统一模型，允许暂停该机制迁移并记录设计缺口；不要用特殊字段污染所有事件只为赶进度。

完成门槛：所有当前支持机制由候选引擎表达，且 legacy/candidate 在测试矩阵中一致。

### AR5 — Single Evaluation Cutover

目标：让 `N`、`-N` 和 breakdown 正式来自同一个 evaluation。

动作：

- 切换生产 projection；
- 保留短期可诊断 fallback；
- 删除重复游戏状态遍历；
- 不立即重构 HUD 节点和 BaseLib。

完成门槛：

- `N`/`-N` 不再调用两条原生读取管线；
- stable/beta 构建与 contract harness 全通过；
- artifact 白名单和哈希已记录；
- 用户批准后完成核心 runtime matrix；
- 没有未经批准的行为修正。

回滚点：cutover 单独 commit；在 runtime 通过前保留可恢复的 legacy reference，不使用 destructive history rewrite。

### AR6 — Lifecycle Coordinator And HUD Presenter Split

目标：把 patch、状态和视图职责分开。

建议范围：

- Harmony patch 只转发内部信号；
- coordinator 管理刷新和 combat session；
- snapshot store/HP-loss budget 纳入显式 session 生命周期或通过单一 owner 清理；
- presenter 管理 labels、style、position、show/hide；
- 合并同一帧重复 refresh request，但不得基于 AUD-0007 虚构性能收益。

完成门槛：

- turn start/end、combat end、覆盖界面、relic/hand/settings 触发测试清晰；
- 所有静态状态都有 owner、创建点和清理点；
- HUD 不计算机制；
- patch 不生成预测公式；
- runtime 验证覆盖冻结/恢复和连续战斗清理。

### AR7 — Settings Snapshot And Schema Consolidation

目标：减少新增设置的多处重复登记。

建议范围：

- 建立不可变 settings snapshot；
- 将 BaseLib persisted properties 映射为一个 snapshot；
- 合并 order/section/label/enum metadata 的重复来源；
- 保持旧 key、默认值和升级行为。

完成门槛：

- 当前所有设置 round-trip 测试通过；
- 英文/简中、切换和完整重启持久化 runtime 验证通过；
- 新增 synthetic setting 的维护性测试或审查证明不再需要修改多层手写映射；
- 不创建自定义 settings screen。

AR7 可在 AR6 前后调整，但不得与 AR5 cutover 放进同一大 diff。

### AR8 — Cleanup And Closure

目标：删除已被替代的重复路径，更新权威文档并正式收口。

动作：

- 删除确认无调用的 legacy pipeline、dead adapters 和临时 shadow 诊断；
- 保留必要 compatibility fallback 和历史 note；
- 更新 architecture/interface-map/project-state/task index；
- 记录最终测试、runtime、artifact 和变更扩散验收；
- 精确审查 staged files；
- commit/push/tag 仍需用户明确批准。

完成门槛：见第 13 节最终 DoD。

---

## 10. 最低自动化测试矩阵

具体测试框架可调整，但覆盖语义不可随意缩减。

### 10.1 Evaluation 基础状态

- 无战斗/无本机玩家/死亡角色 -> Hidden；
- 已知零伤害 -> Hidden；
- 已知 blockable/direct 组合 -> 正确 lanes 和总量；
- blockable unknown + trusted direct -> 保留当前 direct-only 语义；
- total unknown -> `N`/`-N` 按当前保守策略处理；
- 负值钳制；
- overflow/非法顺序输入的安全降级。

### 10.2 事件顺序与 Block

- 当前 Block 按 native order 消耗；
- Power Block 与 Relic Block 分类选择；
- 手牌、Power、敌人事件顺序；
- repeated attack 的逐 hit 事件；
- aggregate attack 不得伪造逐 hit；
- 无伤害事件不改变剩余 Block；
- 多敌人 stable identity 与 snapshot fallback。

### 10.3 HP-loss modifiers

- Intangible 对已验证单个 direct event 的限制；
- aggregate direct + Intangible -> Unsupported；
- TungstenRod 逐事件减伤；
- aggregate enemy HP loss + TungstenRod -> Unsupported；
- BeatingRemnant 已花费预算、剩余预算和零预算；
- TungstenRod -> BeatingRemnant 的当前顺序；
- invalid remaining budget -> 结构化 Unknown/Unsupported；
- power/relic modifier 选项分别开关；
- modifier 组合不改变事件 lane 身份。

### 10.4 Poison pre-action survival

- 无 Poison；
- ordinary kill / no-kill / exact lethal；
- Accelerant 多触发；
- Intangible exact lethal；
- Slippery 层数消费；
- HardToKill 保守保留 Intent；
- HardenedShell budget 小于 HP、等于 HP、足以致死；
- native poison preview 可用/不可用；
- unsupported 组合保守保留 Intent；
- 多敌人中只移除对应 enemy instance 的 Intent contribution。

### 10.5 Projection

- 默认只显示 `-N`；
- incoming-only；
- both；
- `N` 的 current/Power/Relic Block 组合；
- `N` 的 power/relic HP-loss modifier 组合；
- `N` 与 `-N` 从同一 evaluation 生成；
- breakdown 与总量一致；
- Unknown 不显示误导性部分总量；
- incoming placement 不改变数值。

### 10.6 Session/lifecycle

- combat start；
- player turn start；
- final snapshot commit；
- frozen snapshot；
- covering screen hide/restore；
- settings change refresh；
- relic add/remove/melt；
- hand change；
- combat end clear；
- 连续战斗不复用旧 snapshot/budget；
- local player identity changes/invalid native node；
- mid-combat debug injection 维持既有非生产边界。

### 10.7 Compatibility

- stable 10-argument ModifyDamage；
- beta 11-argument ModifyDamage；
- stable/beta turn-end hook detection；
- legacy/current/unknown Diamond Diadem capability；
- reflection member missing/throwing；
- 每个 compatibility failure 具有稳定、限频 reason/diagnostic code。

---

## 11. 运行时验证矩阵

只有用户明确批准 install/game gate 后执行。

最低 smoke：

1. 游戏启动仅加载一个 `Damage Forecast [sts2-party-watch-v2]`。
2. 默认单人战斗只显示 `-N`。
3. incoming-only 与 both 模式。
4. `N` 左右位置。
5. Block 开关至少各覆盖 current、Power、Relic 一例。
6. modifier 至少覆盖 Intangible、TungstenRod、BeatingRemnant。
7. Poison ordinary kill/no-kill 与已验证特殊边界代表例。
8. 回合结束冻结。
9. 覆盖界面隐藏和恢复。
10. relic add/remove 后刷新。
11. 连续两场战斗 snapshot 和 HP-loss budget 清理。
12. BaseLib 英文/简中切换和完整重启持久化。
13. 多人只验证本机 HUD，不声明 teammate/shared HUD。

对每项记录：

```text
Build/artifact hash
Game version/branch
Settings
Scene setup
Expected result
Observed result
Pass/Fail/Blocked
Evidence path or user confirmation
```

---

## 12. 风险登记与控制

| Risk | 影响 | 控制措施 |
|---|---|---|
| Big-bang 重写改变语义 | 已验证机制回归 | 分阶段迁移、legacy authority、shadow compare |
| 统一模型过度抽象 | 规则难读、调试困难 | 使用窄接口和明确阶段，不建立通用脚本引擎 |
| 两套引擎长期并存 | 维护成本翻倍 | shadow 有明确删除 Gate，只在迁移期存在 |
| 事件顺序抽象错误 | Block/modifier 计算错误 | 显式 phase/order、characterization tests、aggregate granularity |
| Game object 泄漏进纯规则 | 无法单测、版本耦合 | adapter/snapshot 边界；必要 native handle 仅留在 integration context |
| 设置 key 变化 | 用户配置重置 | 保留 persisted property/key，先改 runtime snapshot |
| 生命周期清理遗漏 | 连续战斗残留 | 单一 session owner + 状态机测试 + runtime 连战矩阵 |
| 版本反射失效 | HUD Unknown 或 load 失败 | capability map、稳定 reason code、双目标构建 |
| 用户未提交改动冲突 | 数据丢失 | AR0 scope freeze、精确 diff、禁止 destructive restore |
| 重构与新功能混合 | diff 不可审计 | 非目标冻结；缺陷/功能另立任务卡 |
| 产物结构变化 | Mod 无法加载 | 初期不拆 runtime assembly；artifact whitelist 验证 |
| 文档与代码再次漂移 | 后续 Session 误判 | 每 Gate 更新 checkpoint，不改写历史事实 |

---

## 13. 最终 Definition Of Done

### 架构

- 单一 evaluation 驱动 `N`、`-N` 和 breakdown。
- 当前支持机制位于明确规则阶段。
- patch、coordinator、session state、presenter 职责分离。
- 版本差异集中在 compatibility 层。
- settings runtime 使用不可变快照或等价的单一来源。
- 所有全局/静态状态都有明确 owner 和清理路径。

### 正确性

- 现有默认显示和设置默认值不变。
- 现有保守 Unknown/direct-only/native fallback 语义经过测试固定。
- 当前支持的 Poison、Block、HP-loss modifier、Diamond Diadem 路径无未批准变化。
- 单人、本机多人、冻结、覆盖界面和连续战斗边界通过验证。

### 测试与构建

- 新核心场景测试全部通过。
- stable v0.107.1 contract/build/publish：0 warnings / 0 errors。
- beta v0.109.0 contract/build/publish：0 warnings / 0 errors。
- 两目标产物符合当前白名单；如应相同则 hash 一致，否则差异有解释和批准。
- `git diff --check` 通过。
- 无 DLL/PDB/PCK/log/bin/obj/publish/work/uploader/game files 被暂存。

### 维护性

- 新增 synthetic contributor 的维护性验收通过。
- 新机制不需要复制一条新的游戏状态遍历管线。
- reason code 能定位 unsupported/compatibility 边界。
- 旧重复管线和临时 shadow 代码已按 Gate 清理。

### 交付

- architecture、interface map、project state、task index 和本任务卡 closure section 已更新。
- 每个 implementation Gate 使用单一目的、简洁内容标志的 commit。
- push/tag 只在用户明确批准后执行。
- Workshop 未经单独批准保持不变。

---

## 14. Commit、Tag 与审查策略

推荐提交边界；名称可调整，但每个 commit 只承担一个目的：

```text
test: characterize forecast event semantics
refactor: add unified forecast event model
refactor: add shadow forecast evaluator
refactor: migrate <rule-group> forecast rules
refactor: cut over to unified forecast evaluation
refactor: separate forecast lifecycle coordinator
refactor: consolidate runtime settings snapshot
docs: close forecast architecture stabilization
```

推荐里程碑 tag，仅在对应阶段验证并获得 push/tag 批准后创建：

```text
forecast-refactor-test-baseline
forecast-refactor-shadow-engine
forecast-refactor-engine-cutover
forecast-refactor-architecture-closure
```

规则：

- 不 squash 已用于阶段验收的开发历史；
- 不 force-push；
- 不在测试失败状态打完成 tag；
- 不把多个规则迁移塞进无法审查的大提交；
- docs-only 收口使用独立 commit；
- tag 使用 annotated tag 和一行简短说明。

---

## 15. Stop / Escalation Conditions

遇到以下任一情况立即停止当前实现批次并报告：

- 需要改变玩家可见计算语义才能继续；
- 需要修改技术身份或持久化 key；
- 需要更新 Workshop；
- 需要覆盖、删除或重写用户已有改动；
- stable/beta 不能同时保留；
- 统一事件模型无法表达一个已验证机制而不引入全局特殊字段；
- legacy/candidate 出现无法解释的不一致；
- 需要真实安装/启动游戏但用户尚未批准；
- 需要额外 runtime DLL 或改变 artifact 白名单；
- 当前批次显著超出已批准文件/目标范围。

报告必须包含：

```text
Blocking condition
Evidence
Affected invariant
Attempted safe alternatives
Recommended options
Required user decision
```

---

## 16. 每个 Session 的 checkpoint 模板

每个执行 Session 结束前更新本卡或独立 handoff note：

```text
Date/session
Current AR stage
Approved scope
HEAD / branch / remote
Dirty files before work
Files changed by this session
Behavior changes: None / list
Tests run and exact results
Stable build result
Beta build result
Runtime performed: Yes/No
Install performed: Yes/No
Commit/push/tag performed: Yes/No + hashes
Workshop changed: No unless explicitly approved
Open mismatches / reason codes
Decisions and deviations
Rollback point
Exact next action
Approval required next
```

不得只在聊天中记录关键进度。

---

## 17. 下一 Session 的第一批建议动作

下一 Session 不应直接重构 733 行 Reader。建议严格从以下最小批次开始：

1. AR0 只读复核。
2. 设计 game-neutral `ForecastEvent`/scenario fixture 的最小形状。
3. 先为当前 `ApplySelectedBlock` 和 `VerifiedHpLossResultModifier` 行为建立纯场景测试。
4. 为 `N`/`-N` 同源投影建立最小 characterization table。
5. 提交第一批预计 diff、文件列表和测试清单，请求 AR1 批准。

第一批实现不得：

- 删除旧 Reader 路径；
- 切换 HUD；
- 移动 Harmony patch；
- 改设置结构；
- 改 Poison 算法；
- 安装或启动游戏；
- commit/push/tag。

这样即使后续目标架构调整，第一批测试资产仍然可复用。

---

## 18. 可直接复制给下一 Session 的启动提示

```text
请完整阅读：
C:\Users\ROG\Documents\Codex\STS2-Party-Watch-v2\docs\task-notes\forecast-engine-architecture-stabilization-master-task-card.md

从 AR0 开始，只做只读基线复核和设计校准。不要修改代码、stage、commit、push、tag、安装、启动游戏或更新 Workshop。

重点确认：
1. 当前 main/remote/HEAD/status 与任务卡基线是否一致；
2. LocalIncomingDamageReader 中 N/-N 两条管线的实际重复和差异；
3. 可以先提取并测试的最小纯计算 seam；
4. 第一批 AR1 建议修改文件、测试矩阵、风险和回滚点；
5. 任务卡候选架构中需要调整但不破坏硬性不变量的部分。

完成 AR0 后先向我报告并请求 AR1 批准，不要自动开始实现。
```

---

## 19. Closure 预留

此节只在 AR8 完成后填写：

```text
Final status:
Final commit:
Final tags:
Unified engine cutover commit:
Legacy pipeline removed:
Stable tests/build:
Beta tests/build:
Runtime matrix:
Final artifacts and hashes:
Architecture documents updated:
Known deferred items:
Workshop state:
Next optional direction:
```
