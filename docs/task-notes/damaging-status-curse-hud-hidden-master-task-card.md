# Damage Forecast — 伤害类状态/诅咒牌导致 HUD 隐藏修复主任务卡

日期：2026-07-24

任务类型：玩家可见严重行为缺陷、手牌事件读取、回归测试与匹配产物运行验证

## Current Control

State: `Closed`
Last completed: `HC5 — full runtime attestation, authority reconciliation and Git checkpoint`
Next: `None — future Mod-card compatibility remains a separate proposed task`
Approved: `Yes — HC5 Git checkpoint; no push/tag/Workshop`
Evidence: `§28`
Repository: `Checkpoint — the commit containing this closure record`

---

## 0. 下一 Session 执行指令

先完整阅读：

1. 本任务卡；
2. `docs/task-notes/task-closure-standard.md`；
3. `docs/project-state.md` 中 `Current HUD Behavior`、`Supported Mechanics` 与 `Post-G6 Product State`；
4. `src/DamageForecast/Combat/LocalIncomingDamageReader.cs`；
5. `src/DamageForecast/Combat/CardTurnEndDamageInspector.cs`；
6. `src/DamageForecast/Combat/VerifiedFixedTurnEndHpLossReader.cs`；
7. `tests/DamageForecast.ContractTests/Program.cs` 及现有 Burn/Block/modifier/projection contracts。

首次进入只执行 HC0 只读诊断。不要因为本任务卡存在就自动修改代码、运行构建、安装 Mod、启动游戏、stage、commit、push、tag 或更新 Workshop。

HC0 完成后必须报告：

- 实际复现范围；
- 运行中 artifact 与当前 HEAD artifact 的绑定关系；
- 根因是否确认，或仍有哪些竞争假设；
- 最小回归测试矩阵；
- HC1/HC2 的精确候选文件；
- 是否需要用户提供额外卡牌场景。

只有用户明确批准后才能进入下一 Gate。

---

## 1. 一句话目标

确认并修复“手牌中出现会对本机角色造成伤害或生命损失的状态牌/诅咒牌时，完整 Damage Forecast HUD 隐藏”的问题，同时保持保守 Unknown 边界、事件顺序、`N`/`-N` 一致性、双目标兼容和 Post-G6 技术身份不变。

---

## 2. 当前已知事实与证据边界

### 2.1 用户运行时观察

当前用户观察：

- 手中存在会造成角色伤害或生命损失的 Status / Curse card 时，完整 HUD 会隐藏；
- 没有这类牌时，HUD 表现正常；
- 该范围取代此前仅记录 Bad Luck / Regret 的窄描述。

该观察是有效的 L3 用户运行证据，但当前尚未完成：

- 逐卡牌精确矩阵；
- exact artifact 绑定；
- 每张牌的 `N`、`-N` 与 advanced detail 分层结果；
- 根因确认。

不得把“全面命名迁移成功”写成该缺陷的原因，也不得仅凭时间先后认定因果。

### 2.2 当前仓库与运行产物不是同一证据层

- 当前仓库 closure HEAD：`32df9ae`。
- C4 stable/beta contracts：各 `226/226`。
- C4 matching publish DLL SHA256：
  `81207FBEDC390E4CB33F706152C8B98AB9FE02097FC0A61F23BC68EB278B31D0`。
- 当前 `docs/project-state.md` 记录的稳定本地安装 DLL SHA256：
  `EC22C91A20BE88E2B39D5285769771B5999556534FCEDE5EF000E1A82FB0EE43`。
- C4 artifact 尚未安装或启动。

因此，现有用户复现首先绑定 C3 已安装 artifact，不自动证明当前 HEAD/C4 artifact 具有完全相同的运行表现。HC0 必须维持这一区分。

---

## 3. 当前静态根因候选

### 3.1 高优先级候选：手牌事件分类顺序提前失败

当前静态路径：

```text
LocalIncomingDamageReader
  -> TryReadHandTurnEndDamage / TryReadOrderedHandTurnEndEvents
  -> CardTurnEndDamageInspector.TryGetVerifiedSingleBlockableDamageVar
  -> 非精确 Burn 的 turn-end damage call 返回 false
  -> 整条 hand read 返回 false
  -> expected/incoming read 变成 Unknown
  -> HUD 无可显示结果
```

在 ordered hand event 路径中，
`VerifiedFixedTurnEndHpLossReader.TryReadEvent(...)` 位于 generic damage
inspector 之后。若前者已经返回失败，Beckon、Bad Luck、Regret 等已验证
fixed direct HP-loss reader 可能没有机会参与分类。

该路径与用户现象高度吻合，但目前仍只是根因候选，原因包括：

- 尚未对每张实际复现牌确认 IL / DynamicVar / concrete type；
- 历史 Burn 场景曾通过，而用户当前描述可能包含 Burn；
- 当前运行 artifact 不是 C4/HEAD artifact；
- 仍需排除 reader 已返回 Known、但 HUD lifecycle/display 隐藏的其他路径。

### 3.2 当前测试缺口

现有 contracts 已覆盖：

- exact Burn shape 的接受；
- derived/unverified/multi-var/unblockable shape 的拒绝；
- Block、HP-loss modifier、Poison、projection 与 lifecycle 的纯策略。

当前没有直接覆盖：

- Burn、Beckon、Bad Luck、Regret 在同一个 hand event routing 中的优先级；
- verified direct HP-loss card 是否能绕过 generic unsupported rejection；
- 多张 supported damage card 的 native order 与不重复计数；
- hand routing 失败时 `N` 与 `-N` 是否一起变成 Unknown；
- non-damaging Status/Curse 与 truly unsupported damaging card 的区分。

---

## 4. 任务范围

### 4.1 包含

- 精确复现矩阵与 artifact 绑定；
- 手牌 turn-end damage / HP-loss 分类顺序诊断；
- game-neutral 或最小 game-coupled 回归测试；
- 最小生产修复；
- stable v0.107.1 / beta v0.109.0 contracts 与 Release build；
- 用户另行批准后的 matching-artifact runtime smoke；
- current product facts 与本任务卡收口；
- 用户另行批准后的 Git checkpoint。

### 4.2 不包含

- Forecast Engine AR1–AR8 架构重构；
- 把两条 `N`/`-N` 管线整体合并；
- 新增未验证卡牌机制支持；
- 将所有未来/modded damaging card 自动视为 Known；
- HUD 视觉重设计；
- 设置默认值、配置 schema 或 migration 变更；
- Mod ID、assembly、namespace、Harmony owner 或安装 identity 变更；
- Workshop 上传、visibility 或 item 修改；
- repository-root rename；
- 其他未登记 bug。

---

## 5. 必须保持的行为不变量

1. 不执行真实伤害、卡牌动作、命令队列、RNG、存档或网络操作。
2. Burn 只在现有 verified exact-type / single blockable DamageVar 边界内保持支持。
3. Beckon、Bad Luck、Regret 只按已验证 fixed direct HP-loss 语义处理。
4. 不得把 Direct HP Loss 错算为 Blockable damage。
5. 不得双计同一张牌或同时走 generic 与 fixed reader。
6. 手牌 native execution order 必须稳定。
7. truly unsupported / ambiguous damaging card 仍可保守返回 Unknown。
8. non-damaging Status / Curse 不得因为类型分类本身导致 HUD 隐藏。
9. `N`、`-N` 与 advanced details 必须使用一致的 hand-source 支持判断。
10. Intangible、Tungsten Rod、Beating Remnant 与 Block 顺序不得发生未批准变化。
11. stable/beta 兼容路径、配置 18 项、Post-G6 identity 与 Workshop 状态保持不变。

---

## 6. 根因判定树

按下列顺序检查，不跳步：

1. **Artifact 层**
   绑定用户复现 DLL、manifest、game branch 与日志；区分 C3 installed artifact 和 C4/HEAD artifact。

2. **Card classification 层**
   对每张复现牌记录 concrete type、turn-end effect、DamageVar/HpLossVar、generic inspector 与 fixed reader 结果。

3. **Hand aggregation 层**
   记录该牌是否使 `TryReadHandTurnEndDamage` 或
   `TryReadOrderedHandTurnEndEvents` 提前失败、重复或漏读。

4. **Evaluation 层**
   确认 expected read、incoming read、modifier 与 Block 阶段的最终 state。

5. **HUD/lifecycle 层**
   只有 reader/evaluation 已是 Known 但 HUD 仍隐藏时，才扩大到
   `ForecastRefreshPatch`、snapshot、visibility 或 node ownership。

不要在未证明 reader 为 Known 前先修改 HUD visibility。

---

## 7. Gate 计划

### HC0 — Baseline and Root-Cause Revalidation

类型：只读

动作：

- 核对 branch、HEAD、remote、status、tags 与用户未提交改动；
- 核对当前游戏分支、游戏进程、active manifest/DLL hash 与日志 baseline；
- 区分 installed C3 artifact、C4 publish artifact 与当前 HEAD；
- 建立逐卡牌静态矩阵；
- 追踪 expected/incoming hand routing 的精确返回路径；
- 判断 §3.1 是否足以升级为 Confirmed root cause；
- 输出 HC1 测试方案和 HC2 最小 diff。

完成门槛：

- 没有文件修改；
- 没有 build/install/game/Git/Workshop 动作；
- 根因结论明确标记为 Confirmed 或 Candidate；
- 下一 Gate 需要重新获得用户批准。

### HC1 — Contract-First Reproduction

类型：测试边界

动作：

- 为 hand source classification/routing 建立最小可测试 seam；
- 增加 supported blockable、supported direct、non-damaging 与 unsupported damaging 四类测试；
- 覆盖单卡、多卡、顺序、不重复计数和 `N`/`-N` 同源支持判定；
- 在修复前证明新 regression case 能捕获当前错误；
- 不安装、不启动游戏、不改 HUD 视觉。

候选测试 ID 前缀：`HC-`

完成门槛：

- 测试能区分预期支持与真正 unsupported；
- 不把当前错误行为当作永久正确合同；
- HC2 修复范围已由失败测试锁定。

### HC2 — Narrow Production Fix

类型：最小行为修复

候选方向（以 HC0/HC1 证据为准，不预先强制实现）：

- 先识别 verified fixed direct HP-loss event，再应用 generic unsupported rejection；或
- 建立一次性 hand-card classification result，明确
  `VerifiedBlockable`、`VerifiedDirect`、`NoDamage`、`UnsupportedDamage`；
- expected/incoming 两条现有路径暂时复用同一个 classification seam；
- 不在本 Gate 展开完整 Forecast Engine 重构。

完成门槛：

- 新回归测试通过；
- 现有全部 contracts 不回归；
- truly unsupported card 仍保守；
- 没有身份、配置、Workshop 或 HUD 视觉变化。

### HC3 — Dual-Target Automated Verification

类型：L0/L1/L2

动作：

- `Test-ForecastGuardrails.ps1 -Target all`；
- stable/beta Release build；
- 如生成 publish，验证两边 exact two files 与 identity；
- 记录 artifact hashes；
- `git diff --check` 与 forbidden artifact review；
- 精确审查 diff，仅包含获批文件。

完成门槛：

- stable/beta contracts 全通过；
- 两边 0 warnings / 0 errors；
- publish whitelist 通过；
- 没有 DLL/PDB/PCK/log/bin/obj/publish/work/game files 被跟踪或暂存。

HC3 不构成运行时验证。

### HC4 — Matching-Artifact Runtime Verification

类型：L3，必须单独批准

前置条件：

- 游戏进程为 0；
- HC3 artifact 已记录 hash；
- 先执行 install Plan；
- 用户明确批准真实 Install；
- active artifact 与 HC3 artifact hash 完全匹配。

用户运行矩阵至少覆盖：

- 无伤害类 Status/Curse；
- Burn；
- Beckon；
- Bad Luck；
- Regret；
- 两张以上 supported damaging cards；
- non-damaging Status/Curse；
- 一张确认 unsupported/ambiguous damaging card（若安全且可获得）；
- expected-only、incoming-only 或 both 中至少覆盖当前用户实际使用模式；
- advanced details 当前设置保持不变；
- 覆盖界面与回合冻结基本 smoke。

Codex 不代替用户启动游戏。退出后只读检查 fresh log、active hash、cfg 与单实例状态。

完成门槛：

- supported cards 不再导致完整 HUD 隐藏；
- 数值、lane 与事件顺序符合预期；
- unsupported 场景按记录的保守策略处理；
- 无重复 Mod、配置页、HUD nodes 或 attributable error；
- 当前 18 项配置保持。

### HC5 — Authority and Repository Closure

类型：文档/Git，必须单独批准

动作：

- 本任务卡顶部 `Current Control` 更新为真实状态；
- 仅在产品事实变化时更新 `docs/project-state.md`；
- 需要时更新 `docs/mechanics-evidence.md`；
- `docs/task-notes/README.md` 只调整链接/分类，不复制 Gate 细节；
- staged diff 精确审查；
- commit 是默认 checkpoint；
- push/tag 只有用户明确批准时执行。

没有 Git checkpoint 前，状态只能是
`Work Complete / Checkpoint Pending`，不得写 `Closed`。

---

## 8. 最低测试矩阵

| 场景 | 预期分类 | 关键断言 |
|---|---|---|
| 普通非伤害牌 | NoDamage | 不影响 HUD |
| 非伤害 Status/Curse | NoDamage | 不因为 card type 隐藏 |
| exact Burn | VerifiedBlockable | 单事件、Blockable、不重复 |
| Beckon | VerifiedDirect | fixed direct amount、绕过 Block |
| Bad Luck | VerifiedDirect | fixed direct amount、绕过 Block |
| Regret | VerifiedDirect | hand-count amount、绕过 Block |
| Burn + fixed direct | 两个 supported events | native order、lane 分离、总量一致 |
| 两张 fixed direct | 两个 direct events | 不丢失、不重复 |
| unverified damaging card | UnsupportedDamage | 保守 Unknown，稳定 reason |
| supported card + unsupported card | Unsupported aggregate | 不显示误导性部分总量 |
| selected Block on/off | 相同 source events | 只改变 projection 阶段 |
| modifiers on/off | 相同 source events | 只改变 modifier 阶段 |

如果 HC0 证明某张用户复现牌不属于上述已知类型，应新增精确行，不用“所有状态牌/诅咒牌”概括代替证据。

---

## 9. 候选文件范围

HC0 只读重点：

- `src/DamageForecast/Combat/LocalIncomingDamageReader.cs`
- `src/DamageForecast/Combat/CardTurnEndDamageInspector.cs`
- `src/DamageForecast/Combat/VerifiedFixedTurnEndHpLossReader.cs`
- `src/DamageForecast/Combat/UpcomingHpLossEvent.cs`
- `src/DamageForecast/Combat/VerifiedTurnEndDamagePolicy.cs`
- `tests/DamageForecast.ContractTests/Program.cs`

HC1/HC2 候选修改：

- 上述 hand classification/reader 文件中的最小集合；
- 新建一个窄的 pure classification policy（仅在证据支持时）；
- 新建 `tests/DamageForecast.ContractTests/HandCardDamageContractCases.cs`；
- `tests/DamageForecast.ContractTests/Program.cs` 注册新 cases；
- 本任务卡增量证据。

默认不得修改：

- `ForecastRefreshPatch.cs`；
- HUD node/display/layout；
- BaseLib settings/config migration；
- manifest/csproj identity；
- Forecast Engine AR task card。

只有 HC0 证明问题位于这些默认排除层时，才能停止并请求扩大范围。

---

## 10. Stop / Escalation 条件

遇到以下任一情况立即停止并报告：

- 当前运行 artifact 无法绑定；
- 实际复现范围与用户描述显著不同；
- Burn 也失败但当前分类路径静态应接受 Burn；
- 修复需要把所有未知/modded cards 视为 Known；
- 修复需要改变 HP-loss lane、Block 或 modifier 语义；
- stable/beta 行为或 API 不一致；
- 需要修改 HUD lifecycle/visibility 才能继续；
- 需要安装、启动游戏、Git 或 Workshop，但尚未单独批准；
- 发现用户未提交改动与候选文件冲突；
- diff 扩大为 Forecast Engine 架构重构。

报告格式：

```text
Blocking condition:
Evidence:
Affected invariant:
Safe alternatives attempted:
Recommended options:
Required user decision:
```

---

## 11. 证据分层

- **L0**：identity、manifest、artifact whitelist、hash。
- **L1**：纯 classification/event/projection contracts。
- **L2**：stable/beta target-coupled contracts 与 Release build。
- **L3**：matching installed artifact + 用户真实游戏场景。

不得用 L1/L2 代替 L3，也不得用旧 installed artifact 的 L3 证明新 artifact。

---

## 12. 最终收口模板

最终只保留四项：

```text
Result: <修复了哪些精确 card/source 场景>
Current state: <当前运行 artifact、仍 unsupported 的边界>
Authority: <本任务卡及已同步 current product facts>
Repository: <commit/checkpoint，或明确 Checkpoint Pending>
```

---

## 13. 可复制启动提示

```text
请完整阅读：
C:\Users\ROG\Documents\Codex\STS2-Party-Watch-v2\docs\task-notes\damaging-status-curse-hud-hidden-master-task-card.md

从 HC0 开始，只做只读基线、artifact 绑定、逐卡牌分类和根因复核。
不要修改代码、运行构建、安装、启动游戏、stage、commit、push、tag 或更新 Workshop。

重点确认：
1. 用户复现绑定的是哪个 DLL / manifest / game branch；
2. Burn、Beckon、Bad Luck、Regret 与其他实际复现牌分别如何被分类；
3. generic damage inspector 是否在 verified fixed direct reader 之前提前失败；
4. expected/incoming read 最终分别返回 Known、Hidden 还是 Unknown；
5. HC1 最小回归测试和 HC2 最小修复文件。

HC0 完成后报告结论并请求 HC1 批准，不要自动修复。
```

---

## 14. Follow-up：可持续的官方牌结构识别扩展

### 14.1 规划决定

HC2 的 narrow routing fix 只解决了 verified direct HP-loss 的分类顺序。
HC4 用户运行验证中 Regret / Bad Luck 通过而 Toxic 失败，因此原 HC 目标
尚未完成，HC4 只能标记为 partial，不能进入 HC5 收口。

结构识别工作并回当前 HC4，作为重新审批的 recovery sub-gates：

- 当前扩展只面向 STS2 官方程序集中的牌；
- 从“精确 Burn 类型白名单”升级为“已证明安全的行为结构识别”；
- 目标是让采用同一种简单伤害结构的未来官方 Status / Curse
  自动进入 `VerifiedBlockable`，不必逐卡增加名称白名单；
- Mod 卡牌兼容只记录未来设计入口，本轮不实现、不默认信任；
- 直接生命损失仍由独立 reader 管理，本轮不泛化
  Beckon、Bad Luck、Regret；
- 无法完整证明目标、数值来源和伤害性质时继续返回
  `UnsupportedDamage`，不得输出部分总量或猜测值。

这不是独立的“扩展已开始”，也不表示原 HC 已完成。既有
HC1/HC2/HC3/HC4 批准不自动授权任何 recovery sub-gate。

### 14.2 当前证据基础

现有 inspector 已经能够：

- 找到 concrete card 的 `OnTurnEndInHand`；
- 跟随 async state machine 到 `MoveNext`；
- 扫描是否调用 `CreatureCmd.Damage`；
- 读取实例上的 `DamageVar`；
- 拒绝多 `DamageVar` 和 `Unblockable`。

当前限制是：

- 只要发现 damage call，只有 exact `Burn` 会被信任；
- 现有布尔缓存只记录“是否出现 damage call”，不能表达调用次数、目标、
  数值来源或稳定拒绝原因；
- 仅删除 exact-Burn 判断会把“发现 damage call”错误提升成
  “已经证明可安全预测”，不可接受。

已知 stable/beta 静态形状显示 Burn、Decay、Infection、Toxic、Wither
都属于单 `DamageVar` 的回合结束伤害候选；其中用户当前 L3 已确认
Toxic 仍因 Burn-only 边界而隐藏。其余卡牌必须在新的 recovery Gate 中分别保留
L2 与 L3 证据标签，不得用结构相似替代逐场景运行证明。

---

## 15. 目标设计与信任边界

### 15.1 不再按卡名信任

候选 recognizer 不使用 Burn、Toxic、Decay 等卡名作为接受条件。
它为每个 concrete card type 生成一个静态结构描述，再结合当前 card
实例的 DynamicVars 产生分类结果。

建议的静态描述至少包含：

- concrete type 与 declaring type；
- 来源 assembly 是否为当前官方游戏 assembly；
- 是否声明有效的 `OnTurnEndInHand`；
- sync method 或 async state-machine `MoveNext` 是否可解析；
- `CreatureCmd.Damage` call-site 数量；
- call-site 使用的 exact overload 与可选参数来源；
- 每个 call-site 的目标值来源；
- 每个 call-site 的伤害值来源；
- source-card 参数是否来自同一个 concrete card 实例；
- 是否存在无法解析的 token、opcode、控制流或参数来源；
- 一个稳定、可测试的接受或拒绝 reason。

实例级信息至少包含：

- `DamageVar` 数量；
- 唯一 `DamageVar` 的当前值；
- `ValueProp.Unblockable`；
- recognizer 真正需要的其他可预测性 flags。

静态方法形状可以按 `Type` 缓存；DynamicVar 的当前值和属性不得缓存，
必须按当前 card 实例读取。

### 15.2 第一版唯一自动接受路径

只有同时证明以下条件，才分类为 `VerifiedBlockable`：

1. `HasTurnEndInHandEffect` 为真；
2. concrete card type 来自当前加载的官方 STS2 card assembly；
3. concrete type 自己提供可解析的 `OnTurnEndInHand` 实现；
4. sync body 或其 async state-machine 中恰好存在一个
   `CreatureCmd.Damage` call-site；
5. call target 可证明是该牌持有者的 player creature，而不是敌人、
   队友、任意参数或无法确认的局部变量；
6. amount 可证明来自当前 card 实例中唯一的 `DamageVar`；
7. 该 `DamageVar` 的 props 精确为当前已验证的
   `Unpowered | Move`；任何 `Unblockable`、未知或额外行为 flag
   在第一版都拒绝；
8. `DamageVar.BaseValue` 是非负、可转换为预测管线整数范围的值；
9. stable 的四参数 DamageVar overload，或 beta 的五参数 overload
   可以被精确识别；beta 额外的 `CardPlay` 参数必须可证明为 `null`；
10. 不存在第二个 damage call、第二个 DamageVar、无法归属的 amount、
   无法归属的 target，或会改变本次伤害次数/目标/数值的未识别路径；
11. 解析过程没有异常、未知 metadata token、截断 IL 或不支持的 opcode。

任何条件不满足都 fail closed。分类规则保持：

```text
verified direct reader 命中
  -> VerifiedDirect
否则没有 damage call
  -> NoDamage
否则完整通过官方结构证明
  -> VerifiedBlockable
否则
  -> UnsupportedDamage
```

### 15.3 明确不由第一版自动接受

- 任意非官方 assembly 中的 card type；
- 继承官方牌但来自 Mod assembly 的派生类型；
- 多段、多目标、随机、条件性或循环伤害；
- 间接 helper、delegate、virtual dispatch 或无法追踪的数据流；
- 数值来自手牌数、能量、Buff、随机数、其他 DynamicVar 或运行命令结果；
- `Unblockable` 或无法确认是否经过 Block 的伤害；
- 对敌人、队友或动态选择目标的伤害；
- 直接 HP loss；
- 需要执行 card action、command queue、RNG 或游戏逻辑才能知道结果的牌。

拒绝不是错误：它表示预测证据不足，HUD 继续使用现有保守 Unknown 策略。

### 15.4 持久维护要求

- 接受规则必须由语义字段驱动，不得改成不断增长的卡名 allowlist；
- reflection/IL 提取与 pure acceptance policy 分离；
- 每个拒绝必须有稳定 reason，便于游戏更新后定位是 assembly、call count、
  target、amount、props 还是 parser 发生变化；
- stable 与 beta 对同一规则分别验证，不能假设 IL 完全相同；
- parser 遇到未来不认识的结构只降级为 Unsupported，不得抛出到 HUD 主路径；
- 不执行任何被检查的游戏方法；
- 不改变 direct HP-loss、Block、modifier、事件顺序、`N`/`-N`
  或 advanced details 的既有管线。

---

## 16. HC4 Recovery Gate 计划

### HC4-R0 — Official Structural Recognizer Baseline

类型：只读、设计冻结

动作：

- 重新绑定 stable/beta 游戏版本、目标 assembly 和当前仓库状态；
- 对 Burn、Decay、Infection、Toxic、Wither 记录：
  concrete/declaring type、sync/async body、damage call-site 数、
  target provenance、amount provenance、DamageVar 数量与 props；
- 选取 non-damaging、direct HP-loss 和可获得的复杂/歧义牌作为负样本；
- 确认 stable/beta 是否能使用同一个语义 descriptor；
- 冻结 descriptor 字段、接受条件、拒绝 reason 和候选修改文件；
- 明确哪些要求能够由 IL 静态证明，哪些必须继续拒绝。

完成门槛：

- 不修改生产代码或测试；
- 不 build、install、launch、Git 或 Workshop；
- 每个正样本和负样本都有预期分类与证据层；
- 若无法证明 target 或 amount provenance，停止并缩小方案，不得退化为
  “官方 assembly + 单 DamageVar 即接受”；
- HC4-R1 需要用户重新批准。

### HC4-R1 — Contract-first Descriptor and Policy

类型：测试/seam

动作：

- 建立纯 `TurnEndDamageShapeDescriptor` 与纯 acceptance policy；
- 先增加预期失败的正向 contracts：
  Burn、Decay、Infection、Toxic、Wither；
- 增加对抗性 contracts：
  非官方 assembly、官方类型的 Mod 派生类、零/多 damage call、
  零/多 DamageVar、Unblockable、错误目标、间接/未知 amount、
  parser failure；
- 保留 direct-reader 优先级、NoDamage、mixed supported/unsupported、
  native order 与 `N`/`-N` 同源合同；
- 失败测试必须证明当前 exact-Burn 策略无法满足新合同。

完成门槛：

- red evidence 只来自预期的新正向合同；
- 旧的 conservative rejection 合同没有被改写成宽松预期；
- 测试 seam 不需要执行真实 card method、damage command 或 RNG；
- HC4-R2 需要用户重新批准。

### HC4-R2 — Structural Recognizer Production Change

类型：窄生产实现

候选动作：

- 将 `CardTurnEndDamageInspector` 的布尔扫描升级为描述符提取；
- 将 `VerifiedTurnEndDamagePolicy` 改为基于来源、call-site、target、
  amount 与实例 DynamicVar 形状作纯判断；
- 缓存仅保存 type-level 静态 descriptor；
- 保持现有
  `TryGetVerifiedSingleBlockableDamageVar(...)` 调用合同，除非 HC4-R0
  证明窄签名调整能明显降低双路径漂移；
- expected/incoming 两条 hand reader 继续共享同一个 classification
  结果和 fail-closed 语义；
- 不改 `VerifiedFixedTurnEndHpLossReader` 的卡牌范围。

默认生产候选文件：

- `src/DamageForecast/Combat/CardTurnEndDamageInspector.cs`
- `src/DamageForecast/Combat/VerifiedTurnEndDamagePolicy.cs`
- 可新增一个窄的 descriptor/reason 文件

只有确有编译或共享调用需要时才修改：

- `src/DamageForecast/Combat/LocalIncomingDamageReader.cs`
- `src/DamageForecast/Combat/HandCardDamageClassificationPolicy.cs`

完成门槛：

- HC4-R1 新合同由 red 变 green；
- Burn、direct HP-loss 和 non-damaging 行为不回归；
- Mod 类型与歧义结构仍为 Unsupported；
- parser 异常不能逃逸到 HUD；
- HC4-R3 需要用户重新批准。

### HC4-R3 — Dual-target Automated Verification

类型：L0/L1/L2

动作：

- stable contracts；
- beta contracts；
- `Test-ForecastGuardrails.ps1 -Target all`；
- stable/beta Release build；
- publish exact-two-files、identity、hash 与 forbidden-artifact 检查；
- `git diff --check`；
- 审核精确 diff 与获批文件范围。

完成门槛：

- stable/beta 全部 contracts 通过；
- 两边 0 warnings / 0 errors；
- publish whitelist 与 identity 不变；
- 无游戏文件、build output、probe、log 或 work artifact 被跟踪；
- HC4-R3 只证明自动化兼容，不标记为 L3；
- 安装与运行需要新的 HC4-R4 批准。

### HC4-R4 — Matching-artifact User Runtime Matrix

类型：L3，分 Plan / Install 两次批准

最低矩阵：

- Burn；
- Toxic；
- Decay；
- Infection；
- Wither；
- Regret 或 Bad Luck，证明 direct lane 未回归；
- non-damaging Status / Curse；
- 两张 supported blockable cards；
- supported blockable + verified direct；
- 有 Block / 无 Block；
- 当前用户启用的 `N`、`-N` 与 advanced details 组合。

动作边界：

- Codex 先生成 install Plan 和 rollback evidence；
- 用户另行批准后才安装；
- Codex 不代替用户启动游戏；
- 用户完成场景后，Codex 只读核对 fresh log、active hash、config 与单实例；
- 未覆盖的卡牌只能标记 L2，不得写 RuntimeVerified。

### Return to HC5 — Authority and Git Closure

类型：文档/Git，单独批准

动作：

- 用实际证据更新本任务卡 `Current Control`；
- 只有产品事实真实改变时才更新 `docs/project-state.md` 和 mechanics evidence；
- 记录已支持的结构，不宣称“支持所有未来官方牌”；
- staged diff 精确审查；
- 用户批准后建立 Git checkpoint；
- push/tag/Workshop 仍需独立批准。

---

## 17. HC4 Recovery 最低合同矩阵

| ID family | 输入形状 | 预期 |
|---|---|---|
| HC4R-OFFICIAL | 官方、单 self-target Damage call、唯一 DamageVar、可格挡 | VerifiedBlockable |
| HC4R-NONE | 无 Damage call 的普通/Status/Curse | NoDamage |
| HC4R-DIRECT | Beckon / Bad Luck / Regret 已验证路径 | VerifiedDirect，优先于 generic |
| HC4R-ORIGIN | 非官方 assembly 或 Mod 派生类型 | UnsupportedDamage |
| HC4R-CALLS | 零以外但不是恰好一次、或间接未知调用 | UnsupportedDamage |
| HC4R-TARGET | enemy / teammate / parameter / unknown target | UnsupportedDamage |
| HC4R-AMOUNT | 非唯一 DamageVar、其他来源或 unknown provenance | UnsupportedDamage |
| HC4R-PROPS | Unblockable 或无法确认 Block 语义 | UnsupportedDamage |
| HC4R-PARSER | malformed/unsupported IL、token resolution failure | UnsupportedDamage，不抛出 |
| HC4R-ORDER | 多张 supported、supported + direct | native order、lane 分离、不重复 |
| HC4R-AGG | supported + unsupported | aggregate Unknown，不显示部分值 |
| HC4R-PARITY | stable/beta 与 expected/incoming | 同一分类规则，差异显式记录 |

HC4-R1 必须同时包含 pure policy cases 和 stable/beta target-coupled
real-card cases。只用手工构造 descriptor 不能证明真实游戏类型仍符合结构。

---

## 18. 不变量与 Stop 条件增补

除 §5 和 §10 外，HC4 recovery 遇到以下情况也必须停止：

- target provenance 只能通过执行游戏代码确认；
- amount 与唯一 DamageVar 的关联无法静态证明；
- async state-machine 控制流无法可靠定位唯一 damage call；
- stable/beta 需要两套含义不同的宽松规则；
- 接受当前官方卡必须同时接受 Mod 派生类型；
- 需要执行 card callback、command、RNG 或构造战斗状态；
- 需要修改 direct HP-loss 或 projection/modifier 顺序；
- 生产 diff 超出 §16 的候选文件且没有新的批准。

安全降级顺序：

1. 缩小到能够完整证明的官方结构；
2. 保留单张 exact adapter 作为例外，但必须显式记录，不能冒充通用支持；
3. 无法可靠证明时保持 Unsupported。

---

## 19. Future：Mod Card Compatibility（本轮只记录）

Mod 卡牌适配不能通过移除 assembly 限制直接获得。未来应另建审批范围，
优先考虑“显式、数据化、可版本化的兼容注册”，而不是默认执行或信任
任意 Mod card code。

未来候选层级：

```text
OfficialStructural
  当前官方 assembly 的严格结构证明

RegisteredModDescriptor
  Mod 作者或 compatibility adapter 显式注册：
  concrete type、事件时机、target、lane、DynamicVar key/shape、版本范围

UserAllowlistedStructural
  可选高级模式；用户显式允许某个 assembly 后仍执行同样的严格结构证明
```

未来 Mod 方案必须满足：

- 默认关闭对任意 Mod assembly 的自动信任；
- 注册数据有 provider、assembly/version、card type 与 schema version；
- 冲突注册、版本不匹配、缺少变量或结构漂移时返回 Unsupported；
- 预测时不调用 Mod card 的实际效果方法；
- 第一阶段只允许数据化的单次 self-target blockable DamageVar；
- 动态公式、直接 HP loss、多段/条件/RNG 需要独立 adapter 能力和新 Gate；
- Mod 支持状态在 advanced diagnostics 中可区分
  official / registered / user-allowlisted；
- 任何 Mod compatibility API 都不得成为当前 HC4 recovery 的前置条件。

该章节只是未来架构记录，不是实现授权，也不承诺具体 API。

---

## 20. HC4-R0 批准边界（已完成）

本节保留 HC4-R0 启动时的批准范围。用户随后以“继续做下去吧”批准
HC4-R0；实际执行与证据见 §21。该批准没有自动延伸到 HC4-R1。

可复制批准语：

```text
批准 HC4-R0 Official Structural Recognizer Baseline：只读核对 stable/beta
官方牌结构、target/amount provenance、正负样本和候选文件；不修改代码或
测试，不构建、不安装、不启动游戏、不执行 Git/Workshop。
```

---

## 21. HC4-R0 增量证据

### 21.1 Gate 结果

- Result: `Complete`
- Changed: 只更新本任务卡的 HC4-R0 证据与后续审批状态；
  未修改生产代码或测试。
- Verified: stable/beta snapshot 身份、官方 turn-end card 全矩阵、
  Damage call 数量/overload、五张正样本的 target/amount/source-card
  provenance 与 DynamicVar shape。
- Preserved: 未 build、install、launch、stage、commit、push、tag
  或修改 Workshop；既有 active Mod 未改变。
- Next: `HC4-R1 — Contract-first Descriptor and Policy`，未批准。

### 21.2 目标程序集绑定

| Target | Version / commit | sts2.dll SHA256 | Module MVID |
|---|---|---|---|
| stable | v0.107.1 / 59260271 | `A1F9E653F1E28E4076558FEE1E60D218619CB7E057B887C6417F62C62C6D7A52` | `97f10687-c306-4798-ab75-8b9f23f34dfb` |
| beta | v0.109.0 / c12f634d | `EE45848FF6319DFC7AF2538D3A52D05D82BEF35EE4C5FD0400DC9EFE8F9054AA` | `a49d3537-5a42-4dcd-9877-663e394f2b44` |

两边 assembly identity 都是
`sts2, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null`。
因此实现不能只用 assembly display version 判断目标版本；运行时的
`card.GetType().Assembly == typeof(CardModel).Assembly` 只用于确认
当前类型属于当前加载的官方游戏程序集，不用来跨版本认同 binary。

### 21.3 官方 turn-end card 完整矩阵

stable 与 beta 的官方 `Cards` namespace 都得到相同的语义集合：

| Card | CreatureCmd.Damage calls | Amount overload/source | 当前分类预期 |
|---|---:|---|---|
| Burn | 1 | `DamageVar` / `DynamicVars.Damage` | VerifiedBlockable |
| Decay | 1 | `DamageVar` / `DynamicVars.Damage` | VerifiedBlockable |
| Infection | 1 | `DamageVar` / `DynamicVars.Damage` | VerifiedBlockable |
| Toxic | 1 | `DamageVar` / `DynamicVars.Damage` | VerifiedBlockable |
| Wither | 1 | `DamageVar` / `DynamicVars.Damage` | VerifiedBlockable |
| BadLuck | 1 | decimal / `HpLoss.BaseValue` | VerifiedDirect only |
| Beckon | 1 | decimal / `HpLoss.BaseValue` | VerifiedDirect only |
| Regret | 1 | decimal / `CardsInHand` | VerifiedDirect only |
| Debt | 0 | none | NoDamage |
| Doubt | 0 | none | NoDamage |
| Shame | 0 | none | NoDamage |

五张 blockable 正样本在 stable/beta 都满足：

```text
async <>4__this card instance
  -> Owner
  -> Player.Creature                 // target

same card instance
  -> DynamicVars
  -> Damage                          // amount

same card instance                   // source card
  -> CreatureCmd.Damage exactly once
```

每张牌都只有一个 `DamageVar`，当前 BaseValue 分别是
Burn 2、Decay 2、Infection 3、Toxic 5、Wither 3，props 都精确为
`Unpowered | Move`。

### 21.4 stable/beta 差异

- stable 使用
  `Damage(context, creature, DamageVar, card)`；
- beta 使用
  `Damage(context, creature, DamageVar, card, CardPlay)`，最后一个参数
  在五张正样本中都明确为 `null`；
- Wither 的 compiler-generated state-machine 名称从 stable
  `d__17` 变为 beta `d__26`；
- IL offset 也有变化。

结论：recognizer 必须按 resolved method signature、参数来源和同一实例
关系判断；不得依赖 IL offset、state-machine 名称或 exact binary hash。

### 21.5 冻结的 descriptor

HC4-R1 的 pure descriptor 至少包含：

```text
Origin:
  OfficialCurrentGameAssembly | ExternalAssembly

BodyResolution:
  NoTurnEndEffect | ParsedSync | ParsedAsync | Unreadable

DamageCall:
  CallCount
  OverloadKind = DamageVarStable | DamageVarBeta | Decimal | Other
  TargetProvenance = SameCardOwnerCreature | Other | Unknown
  AmountProvenance = SameCardDynamicVarsDamage | Other | Unknown
  SourceCardProvenance = SameCard | Other | Unknown
  BetaCardPlayProvenance = NotApplicable | Null | NonNull | Unknown

InstanceShape:
  DamageVarCount
  BaseValueIsSupported
  PropsAreExactUnpoweredMove
```

type-level IL 结果可以缓存；`HasTurnEndInHandEffect`、DynamicVar 数量、
BaseValue 与 props 必须按 card 实例读取。

### 21.6 冻结的 fail-closed reasons

第一版至少区分：

- `NoTurnEndEffect`
- `NoDamageCall`
- `ExternalDamagingType`
- `InheritedOrForeignTurnEndMethod`
- `UnreadableMethodBody`
- `DamageCallCountMismatch`
- `UnsupportedDamageOverload`
- `UnknownTargetProvenance`
- `UnknownAmountProvenance`
- `UnknownSourceCardProvenance`
- `UnexpectedBetaCardPlay`
- `DamageVarCountMismatch`
- `UnsupportedDamageVarValue`
- `UnsupportedDamageVarProps`
- `VerifiedSingleOfficialBlockableDamage`

`NoDamageCall` 只有在 method body 成功解析且确实为零 Damage call 时成立；
解析失败不得伪装成 NoDamage。外部/Mod 卡如果成功证明没有 Damage call，
可以保持 NoDamage；只要包含 damage call，本轮没有官方信任或未来注册
证据，就保持 Unsupported。

### 21.7 候选文件冻结

HC4-R1 测试候选：

- `tests/DamageForecast.ContractTests/HandCardDamageContractCases.cs`
- `tests/DamageForecast.ContractTests/Program.cs`
- 可新增一个仅承载 pure descriptor/policy cases 的窄测试文件

HC4-R2 生产候选：

- `src/DamageForecast/Combat/CardTurnEndDamageInspector.cs`
- `src/DamageForecast/Combat/VerifiedTurnEndDamagePolicy.cs`
- 可新增
  `src/DamageForecast/Combat/TurnEndDamageShapeDescriptor.cs`

默认不需要修改：

- `LocalIncomingDamageReader.cs`
- `HandCardDamageClassificationPolicy.cs`
- `VerifiedFixedTurnEndHpLossReader.cs`
- HUD、projection、modifier、settings、identity 或安装脚本

若 HC4-R1/HC4-R2 证明必须越过该默认边界，应停止并重新请求批准。

### 21.8 HC4-R1 精确批准边界（已完成）

HC4-R1 只允许建立测试 seam 与 red contracts；不修改生产实现、不追求
green、不运行完整 guardrail/Release、不安装、不启动游戏、不执行 Git。
用户随后以“继续”批准该范围；实际执行与证据见 §22，该批准未延伸到
HC4-R2。

可复制批准语：

```text
批准 HC4-R1 Contract-first Descriptor and Policy：只新增/调整测试 seam、
pure descriptor policy contracts 与 stable/beta real-card contracts，
先取得精确 red evidence；不修改生产实现，不运行完整 guardrail/Release，
不安装、不启动游戏、不执行 Git/Workshop。
```

---

## 22. HC4-R1 增量证据

### 22.1 Gate 结果

- Result: `Complete — expected red`
- Changed:
  `tests/DamageForecast.ContractTests/HandCardDamageContractCases.cs`
  增加 `HC4R-001`–`HC4R-027`；未修改生产代码。
- Verified: stable/beta target-coupled contracts 都成功编译并运行到相同的
  精确失败集合。
- Preserved: 旧 239 项合同全部继续通过；未运行完整 guardrail/Release，
  未 install、launch、Git 或 Workshop。
- Next: `HC4-R2 — Structural Recognizer Production Change`，未批准。

### 22.2 精确 red evidence

| Target | Discovered | Passed | Failed | Skipped |
|---|---:|---:|---:|---:|
| stable v0.107.1 / 59260271 | 266 | 262 | 4 | 0 |
| beta v0.109.0 / c12f634d | 266 | 262 | 4 | 0 |

两边唯一失败：

- `HC4R-002` Decay：expected accepted，actual false；
- `HC4R-003` Infection：expected accepted，actual false；
- `HC4R-004` Toxic：expected accepted，actual false；
- `HC4R-005` Wither：expected accepted，actual false。

`HC4R-001` Burn 继续通过，证明 red 没有破坏当前已验证基线。

### 22.3 green controls

以下新增合同在 stable/beta 都通过：

- Debt、Doubt、Shame 成功解析为 NoDamage；
- stable DamageVar overload、beta null-CardPlay overload 与等价 sync
  descriptor 的 pure spec；
- external damaging type、foreign/inherited method、unreadable body、
  multiple calls、decimal overload、未知 target/amount/source、
  beta non-null CardPlay、未知控制流、多 DamageVar、不支持的 BaseValue
  与 props 全部 fail closed；
- external parsed zero-damage 保持 NoDamage；
- NoTurnEndEffect 保持 NoDamage；
- verified direct 继续优先于 generic rejection。

这些 pure descriptor cases 是测试侧 specification seam，尚未接入生产
inspector；HC4-R2 必须让生产 descriptor/policy 与同一矩阵一致。

### 22.4 非证据运行

第一次 stable/beta invocation 使用了相对 BaseLib 路径，MSBuild 从项目
目录解析后找不到引用，在测试执行前退出。该结果不计为 red evidence。
随后仅改用仓库现有 BaseLib 绝对路径重跑；没有恢复、下载或修改依赖。

BaseLib 3.3.4 SHA256：
`C593F14EAAB504FC1D31C89DA7C029116D269F65706D9612D6F71A048E504235`。

### 22.5 HC4-R2 精确候选 diff

允许的生产候选：

- `src/DamageForecast/Combat/CardTurnEndDamageInspector.cs`
- `src/DamageForecast/Combat/VerifiedTurnEndDamagePolicy.cs`
- 可新增
  `src/DamageForecast/Combat/TurnEndDamageShapeDescriptor.cs`

允许的测试调整：

- 将测试侧 pure descriptor specification seam 绑定到实际生产 policy；
- 不得删除或放宽 `HC4R-012`–`HC4R-024` 的 fail-closed 预期；
- 仅在生产 reason/type 命名需要时作机械调整。

默认不修改：

- `LocalIncomingDamageReader.cs`
- `HandCardDamageClassificationPolicy.cs`
- `VerifiedFixedTurnEndHpLossReader.cs`
- HUD、projection、modifier、settings、identity 与安装脚本

### 22.6 HC4-R2 批准边界（已完成）

HC4-R2 只实现并接入严格的官方结构 recognizer，使四个精确 red 变 green，
并重跑 stable/beta contracts。不得运行完整 guardrail/Release，不得安装、
启动游戏或执行 Git/Workshop；若必须修改 §22.5 默认排除文件，应先停止。
用户随后以“继续”批准该范围；实际执行与证据见 §23，该批准未延伸到
HC4-R3。

可复制批准语：

```text
批准 HC4-R2 Structural Recognizer Production Change：仅修改
CardTurnEndDamageInspector、VerifiedTurnEndDamagePolicy，可新增窄
TurnEndDamageShapeDescriptor，并将 HC4R pure contracts 绑定生产 policy；
使 stable/beta 的四个精确 red 变 green，保留全部 fail-closed 合同。
不修改 hand reader/direct reader/HUD，不运行完整 guardrail/Release，
不安装、不启动游戏、不执行 Git/Workshop。
```

---

## 23. HC4-R2 增量证据

### 23.1 Gate 结果

- Result: `Complete — dual-target contract green`
- Changed:
  - `src/DamageForecast/Combat/CardTurnEndDamageInspector.cs`
  - `src/DamageForecast/Combat/VerifiedTurnEndDamagePolicy.cs`
  - 新增
    `src/DamageForecast/Combat/TurnEndDamageShapeDescriptor.cs`
  - `tests/DamageForecast.ContractTests/HandCardDamageContractCases.cs`
    将 pure specification seam 绑定生产 policy，并新增 reason 稳定性合同。
- Preserved: 未修改 hand reader、direct reader、HUD、projection、modifier、
  settings、identity 或安装脚本。
- Next: `HC4-R3 — Dual-target Automated Verification`，未批准。

### 23.2 生产行为

recognizer 不含 Burn、Decay、Infection、Toxic、Wither 等卡名：

- type-level reflection/IL descriptor 使用 `ConcurrentDictionary<Type, ...>`
  缓存；
- card 实例的 `HasTurnEndInHandEffect`、DamageVar count、BaseValue 与 props
  每次重新读取，不缓存动态值；
- sync method 与 async state-machine 都有独立 body resolution；
- stable 四参数 DamageVar overload 与 beta 五参数/null-CardPlay overload
  分开识别；
- target 必须来自同一 card 的 `Owner.Creature`；
- amount 必须来自同一 card 的 `DynamicVars.Damage`；
- source-card 参数必须是同一 card；
- async concrete-card local 必须能追溯到 `<>4__this`；
- local call slice 不允许 branch，跨过 damage call 的 conditional branch
  只允许 compiler-generated `<>1__state` resume dispatch；
- 只有一个 Damage call、一个 DamageVar、非负整数范围 BaseValue 且 props
  精确为 `Unpowered | Move` 才进入 VerifiedBlockable；
- parser/token/opcode/provenance/overload/control-flow 任一不确定都返回
  Unsupported，不向 HUD 主路径抛异常。

外部/Mod damaging type 默认 Unsupported；成功解析且零 Damage call 的
外部牌仍保持 NoDamage。未来 Mod registration 没有在本 Gate 实现。

### 23.3 fail-closed reason

生产 policy 现在返回 decision + stable reason，合同覆盖：

- NoTurnEndEffect / NoDamageCall；
- ExternalDamagingType / InheritedOrForeignTurnEndMethod；
- UnreadableMethodBody / DamageCallCountMismatch；
- UnsupportedDamageOverload；
- UnknownTarget / UnknownAmount / UnknownSourceCard provenance；
- UnexpectedBetaCardPlay / UnsupportedControlFlow；
- DamageVar count/value/props rejection；
- VerifiedSingleOfficialBlockableDamage。

未知 body-resolution enum 也保守归入 UnreadableMethodBody。

### 23.4 stable/beta green evidence

| Target | Discovered | Passed | Failed | Skipped | Build diagnostics |
|---|---:|---:|---:|---:|---|
| stable v0.107.1 / 59260271 | 267 | 267 | 0 | 0 | 0 warnings / 0 errors |
| beta v0.109.0 / c12f634d | 267 | 267 | 0 | 0 | 0 warnings / 0 errors |

HC4-R1 的四个精确 red：

- HC4R-002 Decay；
- HC4R-003 Infection；
- HC4R-004 Toxic；
- HC4R-005 Wither；

已全部由真实生产 inspector 变 green。Burn、三个 NoDamage 样本、direct
优先级、全部旧合同和全部 fail-closed cases 同时通过。

该 evidence 是 target-coupled contract/build evidence，不是完整
HC4-R3 guardrail/Release，也不是 L3 runtime。

### 23.5 diff / artifact 边界

- 获批的三个生产候选和一个测试文件没有 trailing whitespace；
- `git diff --check` exit 0；输出只有既有工作树的 LF/CRLF 提示；
- 三个 recognizer 生产文件中不存在五张当前卡牌的名称 allowlist；
- contract artifacts 只生成在 ignored `work/`；
- repository 仍为 Checkpoint Pending；
- 未 install、launch、stage、commit、push、tag 或修改 Workshop。

### 23.6 HC4-R3 批准边界（已完成）

HC4-R3 才允许运行完整：

- `Test-ForecastGuardrails.ps1 -Target all`；
- stable/beta Release build；
- publish exact-two-files、identity、hash；
- forbidden-artifact review；
- `git diff --check` 与精确获批文件审查。

HC4-R3 不允许安装或启动游戏，也不允许 stage、commit、push、tag 或
Workshop 修改。

可复制批准语：

```text
批准 HC4-R3 Dual-target Automated Verification：运行完整 stable/beta
guardrail、contracts、Release build、publish exact-two-files、
identity/hash、git diff 与 forbidden-artifact 检查；不安装、不启动游戏，
不执行 stage/commit/push/tag，不修改 Workshop。
```

用户在 Codex 明确复述以上执行范围后以“继续”批准 HC4-R3。该批准没有
延伸到 HC4-R4 Plan、安装或运行游戏。

---

## 24. HC4-R3 Dual-target Automated Verification 证据

### 24.1 Gate 结果

- Result: `Complete — stable/beta guardrail and publish verification passed`
- Approval: 用户在明确的“不安装、不启动游戏、不做 Git checkpoint”
  边界下批准继续。
- First invocation: 直接执行 `.ps1` 被本机 PowerShell execution policy
  在脚本载入前拦截；随后使用仅限该 PowerShell 进程的
  `-ExecutionPolicy Bypass` 重跑，未修改机器策略。
- Command:
  `powershell.exe -NoProfile -ExecutionPolicy Bypass -File
  .\scripts\Test-ForecastGuardrails.ps1 -Target all`
- Result: `QUALITY_GATE targets=2 status=PASS exit_code=0`
- Next: `HC4-R4 — Matching-artifact Runtime Verification Plan`，未批准。

### 24.2 stable/beta guardrail 与 Release

| Target | Snapshot | Contracts | Release build |
|---|---|---|---|
| stable | v0.107.1 / 59260271 | 267 discovered / 267 passed / 0 failed / 0 skipped | 0 warnings / 0 errors |
| beta | v0.109.0 / c12f634d | 267 discovered / 267 passed / 0 failed / 0 skipped | 0 warnings / 0 errors |

HC4R-001 至 HC4R-028 在两边全部通过；其中 Burn、Decay、Infection、
Toxic、Wither、三个 NoDamage 样本、direct priority 与 fail-closed
矩阵均保持 green。

### 24.3 matching publish evidence

执行 `Build-DualTargets.ps1` 后，独立重跑
`Test-IdentityPublishTrees.ps1`。两次校验均返回：

- `contractStatus=active-migrated`；
- `artifactsIdentical=true`；
- `differentFiles=[]`；
- stable/beta 各自恰好 2 个文件；
- assembly、manifest id 均为 `damage-forecast`；
- manifest version 为 `v0.3.0`。

| File | Stable SHA256 | Beta SHA256 |
|---|---|---|
| `damage-forecast.dll` | `9600B23C85DB1AF7CFEDD75536CCA1FC2ECCC6455AD6C18C1AD6FF54AB25E44B` | `9600B23C85DB1AF7CFEDD75536CCA1FC2ECCC6455AD6C18C1AD6FF54AB25E44B` |
| `damage-forecast.json` | `FF8D4E07E574F9FC89EDEDF0D569EE8A7CADFE2A6A2907CAA9E3097F476C32DB` | `FF8D4E07E574F9FC89EDEDF0D569EE8A7CADFE2A6A2907CAA9E3097F476C32DB` |

发布树的精确白名单为：

- `damage-forecast.dll`；
- `damage-forecast.json`。

四个 publish 文件均命中 `.gitignore` 的 `work/` 规则。

### 24.4 diff / forbidden-artifact / authority 边界

- `git diff --check` exit 0；仅输出既有工作树的 LF/CRLF 提示；
- guardrail：
  `tracked_forbidden=0 working_forbidden=0 exit_code=0`；
- Release/publish 没有向可见 worktree 增加源码、游戏文件、probe、log 或
  build output；
- HC4 获批的 production/test diff 范围不变；两个已知无关 task card
  仍保持原有 dirty/untracked 状态，未被本 Gate 修改；
- repository 仍为 `Checkpoint Pending`；
- 未 install、launch、stage、commit、push、tag 或修改 Workshop。

### 24.5 Evidence 等级与 HC4-R4 边界

本节只构成 L0/L1/L2 自动化、双目标编译与 matching-artifact 证据，
不构成 L3 游戏运行证明。当前游戏安装目录中仍不是本节新生成的
`9600B23...E44B` DLL。

HC4-R4 必须保持两次批准：

1. 先批准只读 Plan：核对 active/publish hash、单实例、游戏进程状态与
   rollback evidence，不安装；
2. 用户审阅 Plan 后，另行批准 Install，才允许替换 Mod；游戏启动与
   用户 runtime matrix 仍按明确边界执行。

可复制的下一 Gate 批准语：

```text
批准 HC4-R4 Matching-artifact Runtime Verification Plan：只生成并核对
安装 Plan、active/publish hash、单实例、游戏进程状态与 rollback
evidence；不执行安装、不启动游戏、不执行 Git/Workshop 操作。
```

---

## 25. HC4-R4 Matching-artifact Runtime Verification Plan 证据

### 25.1 Gate 结果

- Result: `Complete — read-only install and rollback preflight passed`
- Approval: 用户在 HC4-R3 完成并收到只读 Plan 批准语后以“继续”批准。
- Mode: `Plan`；`executeRequested=false`。
- Action: `target-upgrade`。
- Game process: `gameRunning=false`。
- 本 Gate 未安装、未恢复备份、未启动游戏、未修改 Workshop，未执行
  Git 操作。
- Next: `HC4-R4 — Matching-artifact Install`，未批准。

### 25.2 reviewed active / staging binding

stable 与 beta 发布树在 HC4-R3 已证明字节一致，因此 Install Plan 选用
stable 路径：

`work/publish/stable/damage-forecast`

| Artifact | Manifest SHA256 | DLL SHA256 |
|---|---|---|
| active `damage-forecast v0.3.0` | `FF8D4E07E574F9FC89EDEDF0D569EE8A7CADFE2A6A2907CAA9E3097F476C32DB` | `F8B7778A2897437C4437BA9DE549FAAB06A816DF4877DCAE14EC9D4B865B71A6` |
| reviewed HC4 staging `damage-forecast v0.3.0` | `FF8D4E07E574F9FC89EDEDF0D569EE8A7CADFE2A6A2907CAA9E3097F476C32DB` | `9600B23C85DB1AF7CFEDD75536CCA1FC2ECCC6455AD6C18C1AD6FF54AB25E44B` |

版本号和 manifest 不变，但 DLL hash 不同，因此不是 no-op；安装目标是
把 active DLL 从 `F8B777...71A6` 精确替换为 `9600B2...E44B`。

### 25.3 single-instance / Workshop evidence

- local `legacyActiveCount=0`；
- local `targetActiveCount=1`；
- `orphanArtifactCount=0`；
- 从本机 `appmanifest_2868840.acf` 确认 STS2 appid 为 `2868840`；
- 显式扫描
  `C:\Program Files (x86)\Steam\steamapps\workshop\content\2868840`；
- `workshopScanned=true`；
- `workshopIdentityCount=0`。

因此当前识别到的是 loader 本地一个 Damage Forecast、Workshop 零个，
安装完成后目标仍是一个 Mod identity，而不是两个 Damage Forecast。

### 25.4 backup / rollback evidence

reviewed transaction id：

`20260724T100440712Z`

安装前必须把当前 active 目录整体移动到 loader 扫描根之外：

`C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\.damage-forecast-backups\20260724T100440712Z-damage-forecast-v0.3.0`

该 planned backup 将保存当前 `F8B777...71A6` DLL，供 HC4 新安装出现
问题时回到本次安装前版本。安装脚本还会：

1. 先在 mods root 下创建 transaction staging 并复核 staging hash；
2. 把旧 active 移到上面的 recoverable backup；
3. 以 rename 激活新目录；
4. 复核 legacy=0、target=1 和精确 installed hash；
5. 写 install ledger；
6. 任一步失败时把失败目标移出 loader root，并自动移回旧 active。

本机已有上一条 transaction 的独立备份：

`C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\.damage-forecast-backups\20260724T080318454Z-damage-forecast-v0.3.0`

其只读 Rollback Plan 已通过：

- action=`rollback-target`；
- backup manifest SHA256=
  `FF8D4E07E574F9FC89EDEDF0D569EE8A7CADFE2A6A2907CAA9E3097F476C32DB`；
- backup DLL SHA256=
  `EC22C91A20BE88E2B39D5285769771B5999556534FCEDE5EF000E1A82FB0EE43`；
- current config SHA256=
  `B2DB575CF81C8562B65FE85F387E226D33844C97855F51698CFAB12F37AE04F8`；
- config rollback action=`target-direct`。

这条旧备份证明当前回滚机制可形成合法 Plan；它不是即将生成的
`F8B777...71A6` 备份。安装后必须针对新备份重新生成并保存 Rollback
Plan，才把“回到安装前版本”的 evidence 闭合。

### 25.5 reviewed Install 命令与停止条件

只有用户另行批准后，才允许执行以下 hash-locked 命令：

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass `
  -File .\scripts\Install-LocalMod.ps1 `
  -Mode Install `
  -StagingDir work\publish\stable\damage-forecast `
  -TransactionId 20260724T100440712Z `
  -ExpectedManifestSha256 FF8D4E07E574F9FC89EDEDF0D569EE8A7CADFE2A6A2907CAA9E3097F476C32DB `
  -ExpectedDllSha256 9600B23C85DB1AF7CFEDD75536CCA1FC2ECCC6455AD6C18C1AD6FF54AB25E44B `
  -ExpectedActiveManifestSha256 FF8D4E07E574F9FC89EDEDF0D569EE8A7CADFE2A6A2907CAA9E3097F476C32DB `
  -ExpectedActiveDllSha256 F8B7778A2897437C4437BA9DE549FAAB06A816DF4877DCAE14EC9D4B865B71A6 `
  -IncludeWorkshop `
  -WorkshopRoot "C:\Program Files (x86)\Steam\steamapps\workshop\content\2868840" `
  -Execute
```

任何 reviewed staging/active hash 变化、游戏正在运行、Workshop 出现
identity、孤儿 identity、重复 identity 或路径越界都会 fail closed。

安装执行后仅允许立即进行：

- installed manifest/DLL 精确 hash；
- local legacy=0、target=1、orphan=0；
- Workshop identity=0；
- 新 backup 精确两文件与 `F8B777...71A6` hash；
- install ledger；
- 针对新 backup 的只读 Rollback Plan。

以上任一失败就停止，不启动游戏。即使全部通过，也不由 Codex 启动
游戏；L3 runtime matrix 仍交由用户手动执行。

可复制的 Install 批准语：

```text
批准 HC4-R4 Install：按 §25 reviewed hash-locked Plan 安装 matching
artifact；安装后只读核对 active hash、单实例、新 backup/ledger 与
Rollback Plan。不要启动游戏，不执行 Git/Workshop 操作；任一核对失败
立即停止。
```

---

## 26. HC4-R4 Install 与 rollback evidence

### 26.1 Gate 结果

- Result: `Complete — reviewed matching artifact installed`
- Approval: 用户在收到 §25 精确 Install 批准语后以“继续”批准。
- Transaction: `20260724T100440712Z`
- Installer action: `target-upgrade`
- Installed identity/version: `damage-forecast v0.3.0`
- 本 Gate 未由 Codex 启动游戏，未修改 Workshop，未执行 Git 操作。
- Evidence level: 安装与静态/只读 preflight 已验证；尚不是 L3 combat
  runtime evidence。

### 26.2 installed artifact

active tree：

`C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\damage-forecast`

精确包含：

| File | Size | SHA256 |
|---|---:|---|
| `damage-forecast.dll` | 224768 | `9600B23C85DB1AF7CFEDD75536CCA1FC2ECCC6455AD6C18C1AD6FF54AB25E44B` |
| `damage-forecast.json` | 371 | `FF8D4E07E574F9FC89EDEDF0D569EE8A7CADFE2A6A2907CAA9E3097F476C32DB` |

安装后重跑 Install Plan 得到：

- action=`target-already-current`；
- active 与 staging manifest/DLL hash 完全一致；
- `legacyActiveCount=0`；
- `targetActiveCount=1`；
- `orphanArtifactCount=0`；
- `workshopScanned=true`；
- `workshopIdentityCount=0`；
- `gameRunning=false`。

因此当前 loader 只会看到一个 Damage Forecast，而不是两个。

### 26.3 recoverable previous version

安装前 active 已完整移到：

`C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\.damage-forecast-backups\20260724T100440712Z-damage-forecast-v0.3.0`

该 backup 精确包含：

| File | Size | SHA256 |
|---|---:|---|
| `damage-forecast.dll` | 204288 | `F8B7778A2897437C4437BA9DE549FAAB06A816DF4877DCAE14EC9D4B865B71A6` |
| `damage-forecast.json` | 371 | `FF8D4E07E574F9FC89EDEDF0D569EE8A7CADFE2A6A2907CAA9E3097F476C32DB` |

install ledger：

`C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\.damage-forecast-backups\20260724T100440712Z-install-ledger.json`

ledger 记录 `action=target-upgrade`、新 staging 精确 hash 与上述
`previousActiveBackupPath`。

### 26.4 post-install Rollback Plan

针对新 backup 的只读 Plan 已通过：

- mode=`Plan`；
- operation=`Rollback`；
- action=`rollback-target`；
- `executeRequested=false`；
- active DLL=`9600B23...E44B`；
- rollback backup DLL=`F8B777...71A6`；
- active/backup manifest 均为 `FF8D4E...32DB`；
- current config SHA256=
  `B2DB575CF81C8562B65FE85F387E226D33844C97855F51698CFAB12F37AE04F8`；
- config rollback action=`target-direct`；
- local legacy=0、target=1、orphan=0；
- gameRunning=false。

所以如果新版本有问题，可以通过另行批准的 hash-locked Rollback
把 Mod 恢复为本次安装前的 `F8B777...71A6` 版本；本 Gate 没有实际
执行回滚。

### 26.5 下一步：用户运行矩阵

Codex 停在游戏启动前。用户手动启动并测试：

1. Burn；
2. Toxic；
3. Decay；
4. Infection；
5. Wither；
6. Regret 或 Bad Luck；
7. 一张 non-damaging Status / Curse；
8. 两张 supported blockable cards；
9. supported blockable + verified direct；
10. 有 Block / 无 Block；
11. 当前启用的 `N`、`-N` 与 advanced details 组合。

用户报告场景结果后，Codex 才进行只读 fresh log、active hash、config
与单实例核对。未实际覆盖的卡牌仍只保留 L2，不标记 RuntimeVerified。

---

## 27. HC4-R4 用户 runtime smoke 与只读证据

### 27.1 用户结果边界

用户在安装 matching artifact 并收到优先测试清单后报告：

`成功了`

在收到精确的剩余矩阵清单和诚实证据边界后，用户进一步确认：

`完全通过 收口`

因此可记录：

- matching artifact 的本次用户 runtime matrix：`PASS`；
- §26.5 列出的卡牌、组合与有/无 Block 场景均由用户确认完全通过；
- HUD 不再因这些当前官方 damage-dealing Status / Curse 手牌隐藏；
- 结合 §27.2～§27.4 的 fresh log、active hash、config 与单实例证据，
  本矩阵可标记为 L3 RuntimeVerified。

### 27.2 fresh log

游戏已正常退出，`SlayTheSpire2` 进程数为 0。

fresh log：

`C:\Users\ROG\AppData\Roaming\SlayTheSpire2\logs\godot.log`

- size=`901624`；
- last write UTC=`2026-07-24T15:55:17.5037843Z`；
- SHA256=
  `26F4D79B2A177F89F57272DC4EEB95CFCC829DA44EC97AB9EDD223211A301097`；
- 唯一发现 local
  `mods\damage-forecast\damage-forecast.json`；
- 唯一加载 `mods\damage-forecast\damage-forecast.dll`；
- `DamageForecast.MainFile` initializer count=1；
- BaseLib `damage-forecast` registration count=1；
- `[Damage Forecast] Loaded` count=1；
- `Finished mod initialization for 'Damage Forecast'` count=1；
- Mod list 为 `Damage Forecast [damage-forecast] (v0.3.0)`；
- Damage Forecast attributable error/exception/failed/blocked/disabled
  count=0。

日志不记录逐张手牌 HUD 数值，因此不从日志反推 Toxic 等具体卡牌是否
显示正确；该部分只使用用户观察。

### 27.3 post-runtime active / single-instance

运行后重跑只读 Install Plan：

- action=`target-already-current`；
- active manifest SHA256=
  `FF8D4E07E574F9FC89EDEDF0D569EE8A7CADFE2A6A2907CAA9E3097F476C32DB`；
- active DLL SHA256=
  `9600B23C85DB1AF7CFEDD75536CCA1FC2ECCC6455AD6C18C1AD6FF54AB25E44B`；
- active 与 reviewed staging 完全一致；
- local legacy=0、target=1、orphan=0；
- Workshop 已扫描，identity=0；
- gameRunning=false。

运行没有替换或修改 matching artifact，且仍只有一个 Damage Forecast。

### 27.4 config evidence

current config：

`C:\Users\ROG\AppData\Roaming\SlayTheSpire2\mod_configs\DamageForecast.cfg`

- exact key count=18；
- size=`802`；
- last write UTC=`2026-07-24T15:55:16.5934381Z`；
- SHA256=
  `B2DB575CF81C8562B65FE85F387E226D33844C97855F51698CFAB12F37AE04F8`；
- 与安装前、Rollback Plan 时的 config hash 相同；
- `EnableDamageForecastHud=True`；
- `DamageDisplayMode=Both`；
- `IncomingDamagePlacement=RightOfExpectedHpLoss`；
- `ShowAdvancedShieldHeartDetails=False`；
- Current/Power/Relic Block incoming options 均为 `False`。

本次可绑定为 `Both` 模式、advanced details 关闭的 runtime smoke；
不能声明 advanced details 开启组合已经覆盖。

### 27.5 HC4-R4 精确矩阵（已完成）

用户确认以下本次矩阵完全通过：

- Burn；
- Toxic；
- Decay；
- Infection；
- Wither；
- Regret 或 Bad Luck；
- non-damaging Status / Curse；
- 两张 supported blockable cards；
- supported blockable + verified direct；
- 有 Block / 无 Block。

运行时配置证据绑定 `DamageDisplayMode=Both`、
`ShowAdvancedShieldHeartDetails=False`。因此本次 RuntimeVerified
范围包含当前 `Both` 模式，不额外声称 advanced details 开启布局已被
本次日志/config 绑定。该 UI 组合不影响本缺陷的卡牌分类修复收口。

---

## 28. HC5 Authority and Task Closure

Result: 当前官方 Burn、Toxic、Decay、Infection、Wither 严格结构识别、
既有 verified direct 路径、non-damaging Status / Curse、多卡组合及
有/无 Block 矩阵已通过 stable/beta 267/267 contracts、双目标
0-warning/0-error Release、matching-artifact 安装与用户 L3 runtime。
原 HUD 整体隐藏缺陷在已测试范围内修复。

Current state: 本机 active 为唯一 `damage-forecast v0.3.0`，DLL SHA256
`9600B23C85DB1AF7CFEDD75536CCA1FC2ECCC6455AD6C18C1AD6FF54AB25E44B`；
Workshop identity=0；fresh log 仅初始化一次且可归因错误为 0；安装前
`F8B777...71A6` artifact 有可执行的 hash-locked rollback Plan。
识别器只接受已验证的当前官方单 self-target blockable DamageVar 结构；
外部/Mod damage card 默认 Unsupported，也不宣称所有未来官方牌会自动
兼容。

Authority: 已同步本任务卡、`docs/project-state.md`、
`docs/mechanics-evidence.md` 与 `docs/task-notes/README.md`。历史 task
notes 保持原文。

Repository: 用户已批准 HC5 Git checkpoint；包含本 closure record 的
commit 即为本任务 checkpoint，因此最终状态为 `Closed`。该 checkpoint
必须精确排除既有无关修改
`forecast-engine-architecture-stabilization-master-task-card.md` 与
`sts2sim-damage-forecast-evaluation-master-task-card.md`，只 stage 本任务
的 production、contracts、installer fix 与四份已同步 authority 文件。
未授权 push、tag 或 Workshop 修改。
