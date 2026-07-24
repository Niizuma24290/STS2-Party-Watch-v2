# Damage Forecast — G6 Full Technical Identity Migration 主任务卡

状态：Closed / G6-0 through G6-7 Complete

创建日期：2026-07-21

任务性质：一次受 Gate 约束、可验证、可回滚的完整技术身份迁移；不是全局字符串替换。

玩家产品身份：`Damage Forecast` / `伤害预测`，保持不变。

当前技术身份：`damage-forecast` / `DamageForecast`；仅配置兼容岛保留 `STS2PartyWatch.Settings` / `STS2PartyWatch.cfg`。

候选新技术身份：`damage-forecast` / `DamageForecast`；仅为预审建议，尚未批准。

与其他任务的关系：Forecast Engine 测试护栏 TG0-TG7 已关闭并提供复用入口；Forecast Engine 架构卡继续保持 Proposed / Queued。本卡不授权架构重构、机制修正、Workshop 更新或正式多人 HUD。

---

## 0. 强制启动协议

首次进入本卡只允许执行 G6-0。创建或阅读本卡不等于批准 G6-1 或任何实现。

G6-0 必须只读完成：

1. 读取本卡、任务索引、`name-migration-inventory.md`、测试护栏卡和 `identity-contract.json`；
2. 重新记录 HEAD、branch、remote 和进入前工作区改动归属；
3. 重新扫描仓库 current authority、active source、tests 和 scripts 中的身份命中；
4. 重新检查本机 local mods、Workshop content、最近日志和 BaseLib 配置位置；
5. 不把预审中的本机路径、hash、Workshop 状态或配置文件观察直接当成当前事实；
6. 输出完整决策表、第一批预计 diff、风险、回滚点和验证矩阵；
7. 停止并请求用户批准 G6-1。

G6-0 未经明确批准，不得自行开始。后续每个 Gate 也必须分别批准，不继承前一 Gate 的授权。

任何 Gate 默认禁止：

- 安装到游戏目录；
- 启动游戏；
- 修改真实 BaseLib 配置；
- 删除旧安装或备份；
- 修改 Workshop item、visibility 或 content；
- stage、commit、push、tag；
- 改名本地仓库根目录；
- 修改 Forecast 玩家可见公式、默认设置或功能边界。

---

## 1. 任务目标

把当前分散在 manifest、assembly、DLL、namespace、项目路径、BaseLib、Harmony、诊断、安装脚本和持久化中的旧技术身份迁移为一套经用户批准的新身份，同时满足：

- Loader 最终只看到一个 active Mod；
- 新旧 manifest/DLL 不会共存并双初始化；
- 18 个设置逐项连续，不静默恢复默认值；
- clean install、旧安装升级和 rollback 都有确定流程；
- stable/beta 使用同一 contract harness 和本机双目标入口验证；
- current authority 更新，历史证据原文保留；
- 任何失败都能回到旧身份、旧设置和单一安装状态。

成功不是“仓库里搜不到 `PartyWatch`”。成功是迁移契约、产物、安装、持久化、运行时和回滚相互一致。

---

## 2. Authority 与复用边界

### 2.1 必读 authority

- 本卡：G6 范围、Gate、决策和 closure authority；
- `docs/task-notes/phase-12c-audit/name-migration-inventory.md`：旧名称分类与历史保留边界；
- `tests/STS2PartyWatchCode.ContractTests/identity-contract.json`：当前技术身份和 migration field 基线；
- `docs/task-notes/forecast-engine-test-guardrails-master-task-card.md`：已关闭的行为/身份护栏；
- `docs/build-environment.md`：stable/beta 本机质量门；
- `docs/task-notes/README.md`：任务状态与阅读路由。

### 2.2 只作候选证据

2026-07-21 G6 预审报告中的推荐名称、本机安装、配置文件、日志、hash 和 Workshop 观察只用于设计 G6-0 检查，不是持续 authority。

### 2.3 不创建第二套状态

- 不复制完整 Forecast runtime 矩阵；本卡只增加迁移专属验证；
- 不创建第二个 test harness；复用现有 contract project；
- 不创建第二条 stable/beta 质量命令；扩展并复用 `scripts/Test-ForecastGuardrails.ps1`；
- 不把 G6 进度写入 `project-state.md` 充当平行任务日志；仅在 closure 更新当前状态；
- ignored `work/` 可保存恢复 checkpoint 和运行证据，但不是 authority。

---

## 3. 已确认基线与未决候选

下表记录建卡时的仓库基线。G6-0 必须重新验证。

| 身份面 | 当前基线 | 候选目标 | 当前 disposition |
| --- | --- | --- | --- |
| 玩家英文名 | `Damage Forecast` | 保持 | must remain |
| 玩家简中名 | `伤害预测` | 保持 | must remain |
| Mod ID | `sts2-party-watch-v2` | `damage-forecast` | undecided |
| manifest stem | `sts2-party-watch-v2` | `damage-forecast` | undecided |
| assembly / DLL | `sts2-party-watch-v2` / `.dll` | `damage-forecast` / `.dll` | undecided |
| install directory | `sts2-party-watch-v2` | `damage-forecast` | undecided |
| root namespace | `STS2PartyWatch` | `DamageForecast` | undecided |
| project path | `src/STS2PartyWatchCode/STS2PartyWatchCode.csproj` | `src/DamageForecast/DamageForecast.csproj` | undecided |
| test project/path | `STS2PartyWatchCode.ContractTests` | `DamageForecast.ContractTests` | undecided |
| BaseLib registration key | `sts2-party-watch-v2` | `damage-forecast` | undecided |
| Harmony owner | `sts2-party-watch-v2` | `damage-forecast` | undecided |
| diagnostic prefix | `[STS2 Party Watch]` | `[Damage Forecast]` | undecided |
| persistence owner/file | 必须在 G6-0 运行时重查 | 保留、迁移或兼容岛 | undecided |
| Workshop item | 外部状态，G6-0 重查 | 复用、替换或不触及 | undecided |
| repository root directory | 当前目录名 `STS2-Party-Watch-v2` | 可选 `Damage-Forecast` | undecided / closure 后手工项 |
| manifest version | `v0.1.0` | `v0.2.0`、`v1.0.0` 或其他 | undecided |

候选值不得先写入 active contract、测试期望或生产代码来制造“已决定”的假象。

---

## 4. 永久不变量

除非用户另开行为修正任务，G6 必须保持：

- 玩家名称 `Damage Forecast` / `伤害预测`；
- read-only local-player HUD 功能边界；
- 本机多人 HUD 不扩张为队友/共享 HUD；
- manifest `has_dll=true`、`has_pck=false`；
- BaseLib 依赖和最低游戏版本仅在另行批准时变化；
- Forecast、Block、modifier、Poison、lifecycle 的现有 contract 语义；
- 18 个设置的含义、默认值、顺序、属性值和 enum 序列化值；
- publish tree 只包含一个 manifest 和一个 DLL；
- stable/beta frozen reference roots 仍由本机权威入口选择；
- Workshop 默认完全不变；
- 历史命令、旧路径、hash、日志、截图和 dated evidence 原文不被批量改写。

G6 不得借“改名”顺带调整类型设计、文件布局美学、Forecast 架构或玩家文案。

---

## 5. G6-0 必须提交给用户的决策表

以下每项必须明确选择并记录理由；不能用候选建议代替批准：

| Decision ID | 用户决定 | 至少提供的选项 |
| --- | --- | --- |
| G6-D01 | 新 slug / Mod ID | `damage-forecast` 或用户指定值 |
| G6-D02 | namespace / project / test project | 全部 `DamageForecast`；仅普通源码改名并保留兼容岛；其他 |
| G6-D03 | BaseLib 持久化策略 | 永久保留旧 owner/file；一次迁移后保留 fallback；显式复制到新 owner |
| G6-D04 | 旧 namespace 兼容岛 | 仅 config class；扩大到明确外部引用；完全不保留并接受风险 |
| G6-D05 | 18 个属性名和 enum 值 | 必须保持；任何例外逐项批准 |
| G6-D06 | diagnostic prefix | 新旧双标记一个版本；多个版本；仅新标记；永久 legacy alias |
| G6-D07 | release / manifest version | 明确版本值与升级意义 |
| G6-D08 | Workshop disposition | 不触及；复用已核实 item；新 item；其他 |
| G6-D09 | installer 对旧目录的权限 | 只报告；经批准移到备份；经批准完成后清理 |
| G6-D10 | stable/beta runtime 深度 | 两目标全矩阵；stable 全矩阵 + beta build/contracts；其他明确组合 |
| G6-D11 | 第三方二进制兼容 | 无已知引用；提供 alias；阻塞等待清单 |
| G6-D12 | repository root rename | 不改；closure 后用户手工改；另开受控步骤 |
| G6-D13 | 旧诊断/安装兼容期限 | 明确版本或永久期限 |
| G6-D14 | commit / push / tag | 每项继续单独批准；不得在 G6-0 预授权 |

若用户尚未决定某项，相关实施 Gate 必须停在 Proposed，不能自行选择推荐值。

### 5.1 2026-07-21 已批准裁决

用户已批准主线 G6-0 的 G6-D01～D14 推荐方案；机器可验证的完整值与批准证据以 `identity-contract.json` schema 2 为准。

| Decision | Approved disposition |
| --- | --- |
| G6-D01 | 目标 slug / Mod ID 为 `damage-forecast`。 |
| G6-D02 | 普通 namespace、source project、test project 迁移到 `DamageForecast`；config 兼容岛除外。 |
| G6-D03 | 永久继续读写 `STS2PartyWatch.cfg`，不复制到新 owner/file。 |
| G6-D04 | 仅保留 `STS2PartyWatch.Settings.PartyWatchBaseLibConfig` 兼容岛。 |
| G6-D05 | 18 个属性名、类型、默认值、顺序、enum 名称/数值和语义全部保持。 |
| G6-D06 | 新主前缀 `[Damage Forecast]`；`v0.2.x` 保留一次 legacy startup marker。 |
| G6-D07 | migration release / manifest version 为 `v0.2.0`。 |
| G6-D08 | G6 不触及 Workshop；本机验收后再批准是否复用 item `3755598583`。 |
| G6-D09 | 获批升级时旧目录只移动到可恢复备份，不删除；清理另批。 |
| G6-D10 | stable 完整 migration runtime；beta contracts/build/publish/matching-artifact/config smoke。 |
| G6-D11 | 当前无已知第三方二进制引用；不建立广泛 alias。 |
| G6-D12 | G6 不改 repository root；closure 后再决定。 |
| G6-D13 | config、旧安装检测、Godot node lookup 永久兼容；legacy log marker 仅 `v0.2.x`。 |
| G6-D14 | commit、push、tag、Workshop 均继续分别批准。 |

---

## 6. Migration contract 生命周期

G6-0 时的 `identity-contract.json` 是 `current-baseline`，不是目标身份开关；G6-1 已将其推进为 `approved-migration`。后续仍必须按以下可审查状态推进：

1. `current-baseline`：全部 legacy 值与 active 仓库一致，target/disposition 未决；
2. `approved-migration`：每个 field 有用户批准的 target 和 disposition，但生产仍不得半迁移；
3. `migration-in-progress`：只在一个获批实现 Gate 的 working diff 中短暂存在；
4. `active-migrated`：active identity、源码、脚本、产物和兼容实现全部一致；
5. rollback：恢复上一份完整 contract，不能只恢复测试期望。

每个 migration field 必须包含并验证：

```text
Identity field
Legacy value
Target value
Disposition: must-change / must-remain / compatibility-alias
Persistence impact
Installed-artifact impact
Compatibility owner and lifetime
Rollback behavior
Static verification
Build verification
Runtime verification
Decision ID / approval evidence
```

G6-1 必须增强 validator，至少拒绝：

- 未知 contract status；
- `must-change` 但 target 为空或等于 legacy；
- `must-remain` 但 target 不等于 legacy；
- `compatibility-alias` 没有 active target、alias owner 或期限；
- active identity、manifest、project、scripts 和 whitelist 指向不同 contract 状态；
- 只更新测试期望、未同时提供兼容实现的半迁移；
- 重复或遗漏 migration field；
- 未引用对应用户 decision 的目标值。

---

## 7. 原子迁移定义

迁移完成时必须同时成立：

- active source 只有一个 Mod ID；
- source tree 顶层只有一个 active manifest；
- project assembly、DLL、manifest stem/id、publish whitelist 和 install directory 一致；
- BaseLib registration 与获批 persistence 策略一致；
- Harmony owner 唯一，旧 owner 只可用于检测；
- Loader 可见目录中旧 active manifest/DLL 数为 0，新 identity 恰好 1 份；
- 新旧 assembly 不会同时初始化；
- 18 个设置可读取且值不变；
- rollback 可恢复旧 manifest、DLL、目录、配置读取和单一加载。

进程内 `_initialized` 只能防止同一 assembly 重入，不能防止旧、新 assembly 各初始化一次。重复安装检测必须发生在安装/启动前，不能依靠 `MainFile` guard。

---

## 8. 分阶段 Gate

### G6-0 — Revalidation And Decision Proposal

性质：只读。

输出：

- 当前 Git/工作区和改动归属；
- active identity 全清单与命中分类；
- local mods / Workshop / logs / BaseLib config 的新证据；
- 第 5 节全部 decision 的推荐选项与风险；
- G6-1 精确文件清单、预计行数和回滚点；
- 未决项和 required approval。

完成后停止，请求 G6-1 批准。

### G6-1 — Approved Contract And L0 Guard Strengthening

性质：contract/test 为主；不改 active 技术身份。

允许目标：

- 把用户批准的决策写入 migration contract；
- 增强 schema/validator；
- 增加旧/新身份、兼容期限、重复安装和半迁移静态断言；
- 建立 18 设置 continuity fixture；
- 证明 current-baseline 仍全绿。

完成门槛：

- contract 内部一致；
- 现有 152+ 行为/质量用例不因改名计划而失效；
- stable/beta contracts 和 Release build 全绿；
- 未修改 manifest、assembly、namespace、Mod ID、BaseLib key、Harmony owner、安装目录或 Workshop。

回滚点：仅 contract/fixture/test diff。

### G6-2 — Compiler-Aware Source And Project Preparation

性质：只迁移普通源码符号和经批准的项目/path 身份；不得形成外部半迁移。

规则：

- 使用编译器可验证的 namespace/type/file/path 改名，不做全局字符串替换；
- config class、18 属性名、Godot node lookup、诊断 parser、compatibility alias 逐项分类；
- persistence 或外部 load identity 未准备好时，保留明确兼容岛；
- test project、`InternalsVisibleTo`、scripts 和 current links 同批保持可解析；
- historical evidence 不改写。

完成门槛：

- active project 能被唯一脚本解析；
- stable/beta contracts 和 Release build 全绿；
- 玩家可见 contract 无变化；
- 尚未批准的 Mod ID、BaseLib、安装或 Workshop 身份保持旧值。

回滚点：单一 compiler-aware rename 批次；不得依赖 destructive history rewrite。

### G6-3 — Atomic Load Identity And Persistence Compatibility

性质：真正的技术身份 cutover，必须把实现、contract 和兼容边界放在同一可审查 Gate。

必须同批处理：

- manifest stem/id；
- assembly/DLL；
- Mod ID；
- BaseLib registration/persistence strategy；
- Harmony owner；
- diagnostic strategy；
- publish whitelist；
- install identity constants；
- 旧 identity conflict detection；
- 18 项设置读取/迁移/回退实现。

禁止留下“新 manifest + 旧 DLL”“新 BaseLib key + 无旧配置读取”“测试已改绿但生产未迁移”等中间结果作为完成状态。

完成门槛：

- active-migrated contract 全绿；
- active source 只存在一个目标 identity；
- legacy literals 仅存在于兼容、升级、rollback、测试 fixture 或历史 evidence 的明确 allowlist；
- 每个 allowlist 命中有 owner、用途和移除期限；
- 18 项 continuity fixture 全通过。

回滚点：恢复完整旧 contract 和旧 active identity；不可只回退 manifest。

### G6-4 — Recoverable Install, Upgrade And Rollback Tooling

性质：先实现和静态测试工具；默认不执行真实安装。

安装器必须支持：

1. 验证游戏未运行，或拒绝继续；
2. 扫描 local mods 和获准范围内的 Workshop content；
3. 解析 manifest，不只按目录名猜测；
4. 识别旧/new Mod ID、manifest、DLL 和重复副本；
5. 验证 staging tree 恰好为目标 DLL + manifest；
6. 在 mods root 内验证所有 resolved paths；
7. 先把旧 active 目录移动到带时间/identity 的可恢复备份；
8. 通过同卷 rename 或等价受控步骤切换 staging；
9. 切换后验证 legacy active=0、target active=1；
10. 失败时恢复旧目录并复核单一加载状态；
11. 在 runtime 验收前不删除备份；
12. 默认不触及 Workshop，不自动删除未知第三方目录。

脚本必须提供 dry-run / plan 输出。真实 move/install/rollback 仍需用户在执行时单独批准。

回滚点：脚本 diff；真实执行时另有本机备份路径和 hash ledger。

### G6-5 — Dual-Target Build, Publish And Static Closure Candidate

默认复用：

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Test-ForecastGuardrails.ps1
```

若需要 publish，必须在本 Gate 单独批准，并验证：

- stable/beta contracts 全绿且 case set 相同；
- stable/beta Release build 0 warnings / 0 errors；
- 每个 publish tree 恰好包含目标 manifest + DLL；
- manifest JSON、id/stem、assembly/DLL、version 和 dependency 一致；
- 两目标 hash 相同则记录；不同则必须解释并获批；
- legacy active artifact 不出现在 publish tree；
- `git diff --check` 通过；
- Git 不跟踪 DLL/PDB/PCK/log/EXE、bin/obj/publish/work/uploader/game files。

本 Gate 不自动安装或启动游戏。

### G6-6 — Migration-Specific Local Runtime

只有用户明确批准 install/game 后执行。本卡不重复一般 Forecast runtime 矩阵，只验证迁移专属风险。

最低场景：

| Runtime ID | 场景 | 必须观察 |
| --- | --- | --- |
| G6-R01 | clean install | Loader 恰好一个目标 Mod；一次 initializer；一次 BaseLib registration；一个配置页 |
| G6-R02 | old local install upgrade | 旧 active=0，新 active=1；无双 HUD/双 patch/双配置页 |
| G6-R03 | 18-setting continuity | 升级前后逐属性值、enum、颜色、offset 和语言完全一致 |
| G6-R04 | full restart persistence | 退出并完整重启后仍读取同一 18 值 |
| G6-R05 | deliberate old+new conflict | installer/start preflight 拒绝或安全报告；不得进入双加载 |
| G6-R06 | rollback | 恢复旧单一安装；旧配置可读；无残留新 active artifact |
| G6-R07 | diagnostic attribution | 日志符合批准的新/旧 prefix 兼容期限且无重复初始化 |
| G6-R08 | matching artifact smoke | 安装 hash 与获批 publish artifact 一致 |

每行记录：artifact hash、game version/branch、安装路径、旧/new manifest 计数、配置备份 hash、expected、observed、Pass/Fail/Blocked 和证据。

stable/beta 的执行深度由 G6-D10 决定，不得把未执行目标写成 RuntimeVerified。

### G6-7 — Authority Reconciliation And Closure

只更新 current authority 和索引：

- README；
- `docs/project-state.md`；
- `docs/architecture.md`；
- `docs/interface-map.md`；
- `docs/build-environment.md`；
- `docs/mechanics-evidence.md` 中仍属 current 的精确引用；
- `docs/v2-roadmap.md`；
- `docs/task-notes/README.md`；
- 本卡 closure；
- `name-migration-inventory.md` 的 future disposition/closure 注记。

历史 Phase note、handoff、旧 release record、hash、截图、日志和 dated command 保持原文；如需说明，只追加迁移映射或 supersession 注记。

commit、push、tag、Workshop 和 repository root rename 仍分别请求批准。本 Gate 通过不自动授权它们。

---

## 9. BaseLib 18 项连续性矩阵

所有行默认 `must remain`。G6-0 如提出例外，必须逐行批准。

| # | Property | 值类型/连续性要求 |
| --- | --- | --- |
| 1 | `ConfigLanguage` | enum 名称和值保持 |
| 2 | `EnablePartyWatchHud` | bool 保持；属性名默认保留 |
| 3 | `ShowAdvancedShieldHeartDetails` | bool 保持 |
| 4 | `FreezeHudNumbersAfterTurnEnd` | bool 保持 |
| 5 | `DamageDisplayMode` | enum 名称、值和语义保持 |
| 6 | `IncomingDamagePlacement` | enum 名称、值和语义保持 |
| 7 | `IncludeCurrentBlockInIncomingDamage` | bool 保持 |
| 8 | `IncludePowerBlockInIncomingDamage` | bool 保持 |
| 9 | `IncludeRelicBlockInIncomingDamage` | bool 保持 |
| 10 | `IncludePowerHpLossModifiersInIncomingDamage` | bool 保持 |
| 11 | `IncludeRelicHpLossModifiersInIncomingDamage` | bool 保持 |
| 12 | `ShowLocalPlayerHudInMultiplayer` | bool 与 local-player-only 语义保持 |
| 13 | `HudAnchorPreset` | enum 名称、值和语义保持 |
| 14 | `HorizontalOffset` | float 精确读取保持 |
| 15 | `VerticalOffset` | float 精确读取保持 |
| 16 | `TotalExpectedLossColor` | Color 序列化值保持 |
| 17 | `ShieldDetailColor` | Color 序列化值保持 |
| 18 | `HeartDetailColor` | Color 序列化值保持 |

测试必须至少包含：全非默认 fixture、全默认 fixture、三种 enum 的非默认值、非整数 offset、三种非默认颜色、完整重启和 rollback 后读取。

不能仅断言“配置文件存在”或“18 个 key 名存在”；必须比较属性值。

---

## 10. Legacy 命中 disposition

迁移后的旧字符串只能属于：

- `compatibility-active`：运行时仍需，必须有 owner 和期限；
- `upgrade-detection`：只用于发现旧安装/config；
- `rollback`：只用于恢复旧 identity；
- `test-fixture`：旧版本输入；
- `historical-evidence`：排除在 L0 扫描之外；
- `external-reference`：经核实的第三方/Workshop/repository 引用。

所有 active source/scripts/current authority 命中必须进入机器可检查 allowlist。未知命中是 Gate failure，不得靠忽略大小写全局替换消除。

Godot node names、BaseLib property names、serialized enum names、reflection string、diagnostic code 和 support parser 都必须按责任分类，不能仅因含 `PartyWatch` 就改名。

---

## 11. 安装与回滚安全规则

- 真实配置修改前先复制到用户可定位的备份并记录 hash；
- 旧安装在验收前只移动，不删除；
- 不对计算但未验证的路径执行递归删除或移动；
- resolved install、staging、backup 必须位于明确的 mods/获批备份根；
- 不跟随指向根目录、用户目录、游戏根或仓库根的异常链接；
- 不覆盖未知目录；冲突时停止并报告；
- rollback 后重新扫描 old/new manifest/DLL 数量；
- Workshop content 默认只读扫描；任何修改需独立授权；
- 配置迁移应可重复运行，不得重复转换或丢失值；
- runtime 通过前不得清理 backup。

---

## 12. Stop / Escalation Conditions

出现任一情况立即停止当前 Gate：

- 新 identity、persistence、compatibility 期限、version 或 Workshop disposition 未获决定；
- 发现未归属 working-tree 改动与当前文件重叠；
- G6 预审中的本机事实与 G6-0 重查不一致；
- BaseLib 实际配置 owner/file 机制无法从代码或可逆实验确认；
- 第三方 Mod/工具引用旧 assembly/type 且没有批准的兼容方案；
- 旧/new manifest 或 DLL 已在 local/Workshop 中共存；
- 无法证明目标路径位于批准的 mods/backup 根；
- 需要删除旧目录、配置或 Workshop content；
- stable/beta 对相同 contract 产生无法解释的差异；
- 任何原有 Forecast 行为 contract 失败；
- 18 项中任一值恢复默认、改变或无法比较；
- installer 不能在失败后恢复单一旧安装；
- publish tree 不再是恰好两个文件；
- 需要改变玩家可见行为、默认值、产品名或多人边界；
- 需要启动游戏、安装、网络写入、commit、push 或 tag 但尚未批准；
- diff 显著超出当前 Gate 的文件/行数预算。

报告格式：

```text
Blocking condition
Evidence
Affected identity fields / decision IDs
Files or external paths involved
Safe alternatives attempted
Rollback state
Recommended options
Required user decision
```

---

## 13. Definition Of Done

### Contract

- 全部 migration field 有批准值、disposition、兼容 owner/期限和验证；
- active-migrated contract 与生产、脚本、产物一致；
- 半迁移、重复 identity 和未知 legacy 命中能使测试失败；
- 现有行为 contract 不依赖旧普通源码符号。

### Source and build

- compiler-aware rename 完成，无未解释 legacy active symbol；
- stable/beta contracts 全绿；
- stable/beta Release build 0 warnings / 0 errors；
- publish tree 各自恰好 manifest + DLL；
- `git diff --check` 通过，无 forbidden tracked/working artifact。

### Persistence

- 18 项逐值通过旧安装升级和完整重启；
- compatibility 策略与用户批准一致；
- rollback 后旧设置仍可读；
- 没有静默默认值回退。

### Install/runtime

- clean install、upgrade、duplicate preflight 和 rollback 的批准矩阵通过；
- Loader 只看到一个 active Mod；
- initializer、Harmony、HUD 和 BaseLib config page 各一份；
- 安装 artifact hash 与获批 publish artifact 一致；
- 未执行目标保持 Pending/Deferred，不伪报 RuntimeVerified。

### Documentation/safety

- current authority 指向新 active identity；
- 历史证据原文保留；
- Workshop 只按明确批准处理；
- backup 的保留/清理状态有记录；
- commit/push/tag 和 repository root rename 各自有明确记录或保持未执行；
- 本卡 closure 和任务索引更新完成。

---

## 14. 每 Gate checkpoint

```text
Date/session
Current G6 Gate
Approved scope and decision IDs
HEAD / branch / remote
Dirty files before work and ownership
Files changed by this Gate
Active identity before / after
Migration contract status before / after
Legacy allowlist additions/removals
18-setting continuity result
Stable contract/build result
Beta contract/build result
Publish performed: Yes/No + hashes
Install/runtime performed: Yes/No + exact paths
Backup/config hashes
Old/new active manifest and DLL counts
Rollback tested/performed: Yes/No
Commit/push/tag performed: Yes/No + hashes
Workshop changed: No unless separately approved
Player-visible behavior changed: None / approved list
Decisions/deviations
Rollback point
Exact next action
Approval required next
```

关键状态必须写入本卡 closure 或 ignored recovery checkpoint，不能只留在聊天上下文。

---

## 15. 推荐提交边界

以下仅是建议；所有 commit/push/tag 仍需单独批准：

```text
test: approve technical identity migration contract
refactor: prepare compiler-aware Damage Forecast symbols
feat: migrate Damage Forecast technical identity with config compatibility
build: add recoverable identity upgrade and rollback tooling
test: verify dual-target migrated artifacts
docs: close full technical identity migration
```

不得把全部 G6 squash 成一个无法审查的巨型提交，也不得在 runtime 未通过时打完成 tag。

---

## 16. 下一 Session 的精确第一步

下一 Session 在用户批准后只执行 G6-0：

```text
完整阅读：
docs/task-notes/full-technical-identity-migration-master-task-card.md

本次只执行 G6-0，只读重查 Git/工作区、active identity、name inventory、
本机 local/Workshop 重复项、最近日志和 BaseLib 配置身份。
不要修改文件，不要更新 identity-contract，不要 restore/build/publish/install，
不要启动游戏，不要修改配置、Workshop、仓库目录，不要 stage/commit/push/tag。

完成后报告：
1. 当前证据与旧预审的差异；
2. G6-D01..G6-D14 推荐选择、风险和未决项；
3. G6-1 精确文件/测试/diff 计划；
4. 回滚点和 stop conditions；
5. 请求 G6-1 批准。
```

---

## 17. Closure

```text
Final status: Closed / G6-0 through G6-7 Complete
Approved target identity: Damage Forecast / damage-forecast / DamageForecast
Permanent compatibility islands and expiry: STS2PartyWatch.Settings.PartyWatchBaseLibConfig and STS2PartyWatch.cfg permanent; legacy install/node detection allowlisted; legacy diagnostic support marker through v0.2.x
Final manifest/version/assembly/DLL/install identity: damage-forecast v0.2.0 / damage-forecast / damage-forecast.dll / mods/damage-forecast
Final BaseLib registration and persistence strategy: register damage-forecast; continue reading/writing STS2PartyWatch.cfg without copy or transform
18-setting continuity result: PASS across upgrade, rollback, clean install, and restart; 797 bytes; SHA256 783C0B880EB1851D813801A6FD35F4D266AE4A2FE69BE50C4812FC7BEA714553
Stable contract/build/publish/runtime result: 181/181; Release 0 warnings/0 errors; exact two-file publish; full migration runtime matrix PASS on v0.107.1 commit 59260271
Beta contract/build/publish/runtime result: 181/181; Release 0 warnings/0 errors; exact two-file publish; matching-artifact/config smoke PASS on v0.109.0 commit c12f634d
Clean install / upgrade / duplicate / rollback result: PASS / PASS / fail-closed PASS / PASS
Final artifact hashes: DLL FB863FF75EA33D6B3F7F9B1471E12E33D5FEBD75595EB680A2D0A49369FDE880; manifest 09D4DC4EFA54BAA11AB3B3B1E2660FF8DB15D5EA153103AB837AAD22DBEB68C5
Local install and backup disposition: stable target active as the only identity; reviewed 11-file recovery set safely moved to Windows Recycle Bin under explicit G6-7 cleanup approval; original game-root backup path absent; recoverable until Recycle Bin is emptied
Workshop disposition and evidence: not scanned or modified by G6
Historical evidence preservation review: PASS; dated commands, paths, hashes, runtime statements, and original inventory rows preserved; closure mapping appended
Authority files updated: README, project-state, architecture, interface-map, build-environment, mechanics-evidence, v2-roadmap, task index, name-migration inventory, and this closure
Commit / push / tag hashes or not performed: not performed
Repository root rename disposition: not performed
Known limitations / deferred work: Regret and Bad Luck direct-HP-loss hand-classification defect confirmed; Beckon suspected/pending runtime; unrelated beta-to-stable run-save downgrade incompatibility documented
Exact next optional task: create a separate behavior-fix task for verified fixed turn-end HP-loss classification before the strict Burn classifier, with full integration coverage and unknown/modded damage cards remaining fail-closed
```

当前 closure：G6-0 through G6-7 Closed。G6-6C 首次 runtime 的 mods 内备份 duplicate-load 已由 G6-6D 修正；beta matching-artifact/config smoke 与 stable rollback/upgrade/clean-install/conflict-preflight 完整矩阵均 PASS。最终本机状态为 stable、legacy=0、target=1、Loader-visible manifest=1，配置 18 项及 hash 未变。经 G6-7 明确批准，已核验的 11-file recovery set 整目录安全送入 Windows 回收站，游戏根原备份路径已不存在且仍可从回收站恢复。Workshop 未扫描或修改，commit/push/tag 与 repository root rename 未执行。Regret/BadLuck 既有行为缺陷转入独立后续候选，Beckon 保持 Suspected/Pending Runtime。

## 18. G6-1 checkpoint — 2026-07-21

```text
Date/session: 2026-07-21 / G6-1
Approved scope and decisions: contract/test only; G6-D01..G6-D14 approved
HEAD / branch / remote: 308ffd5ff9b3e82b9726d4f1cc93a05c73eaaefb / main / origin
Dirty files before work: existing TG1-TG7 and audit corrections; retained without rollback or overwrite
Files changed by this Gate: identity contract/validator/packaging cases/new migration cases/two fixtures/Program.cs/this card
Active identity before / after: sts2-party-watch-v2 + STS2PartyWatch / unchanged
Migration contract status before / after: current-baseline schema 1 / approved-migration schema 2
Persistence contract: BaseLib registration key is registry-only; STS2PartyWatch.cfg remains permanent storage
Legacy compatibility: STS2PartyWatch.Settings.PartyWatchBaseLibConfig / permanent
18-setting continuity: PASS, ordered default + all-nondefault + enum values + restart + rollback
Stable result: 164/164 contracts; Release build 0 warnings / 0 errors
Beta result: 164/164 contracts; Release build 0 warnings / 0 errors
Publish/install/runtime performed: No
Local config/Workshop changed: No
Commit/push/tag performed: No
Player-visible behavior changed: None
Rollback point: only this Gate's eight contract/fixture/test/task-card files
Exact next action: G6-2 compiler-aware ordinary symbol/project/test-project preparation
Approval required next: explicit G6-2 approval
```

## 19. G6-2 checkpoint — 2026-07-22

```text
Date/session: 2026-07-22 / G6-2
Approved scope and decisions: compiler-aware source/project/test-project preparation; G6-D02, G6-D04, G6-D05, G6-D13
HEAD / branch / remote: 308ffd5ff9b3e82b9726d4f1cc93a05c73eaaefb / main / origin
Dirty files before work: existing TG1-TG7, audit corrections, and G6-1 contract work; retained without rollback or overwrite
Files changed by this Gate: source/test project directories and csproj names; ordinary source namespaces/test imports; ProjectReference/InternalsVisibleTo; three scripts; current README/build-environment links; identity contract/tests; this checkpoint
Active identity before / after: legacy load identity + STS2PartyWatch compiler identity / legacy load identity + DamageForecast compiler identity
Migration contract status before / after: approved-migration / migration-in-progress
Legacy allowlist: STS2PartyWatch.Settings.PartyWatchBaseLibConfig retained permanently; legacy project/test paths retained only as contract legacyValue evidence
18-setting continuity: PASS, ordered default + all-nondefault + enum values + restart + rollback
Stable result: 164/164 contracts; Release build 0 warnings / 0 errors
Beta result: 164/164 contracts; Release build 0 warnings / 0 errors
Publish/install/runtime performed: No
Local config/Workshop changed: No
Old/new active manifest and DLL counts: source manifest 1 legacy / 0 target; build emits legacy DLL only
Rollback tested/performed: No; rollback point is the single project/path/namespace batch
Commit/push/tag performed: No
Player-visible behavior changed: None
Decisions/deviations: project/type navigation is compiler-resolved; 18 properties, enum values, Godot lookups, diagnostic prefix, manifest, assembly, BaseLib key, Harmony owner and install identity remain unchanged
Exact next action: G6-3 atomic load-identity cutover plus permanent STS2PartyWatch.cfg compatibility and explicit legacy-conflict allowlist
Approval required next: explicit G6-3 approval
```

## 20. G6-3 checkpoint — 2026-07-22

```text
Date/session: 2026-07-22 / G6-3
Approved scope and decisions: atomic load identity and persistence compatibility; G6-D01, D03..D07, D09..D11, D13
HEAD / branch / remote: 308ffd5ff9b3e82b9726d4f1cc93a05c73eaaefb / main / origin
Dirty files before work: existing TG1-TG7, audit corrections, G6-1 contract work, and G6-2 compiler rename; retained without rollback or overwrite
Files changed by this Gate: target manifest name/content; project assembly/manifest include; MainFile and diagnostics; new loaded-assembly conflict detector; build/install identity constants and preflight; identity contract/validator/tests; current README/build/authority docs; this checkpoint
Active identity before / after: legacy load identity + DamageForecast compiler identity / damage-forecast load identity + DamageForecast compiler identity
Migration contract status before / after: migration-in-progress / active-migrated
Persistence contract: BaseLib registration key is damage-forecast; config type/file remain STS2PartyWatch.Settings.PartyWatchBaseLibConfig / STS2PartyWatch.cfg with no copy or transform
Legacy allowlist: six owned entries covering config namespace, old slug conflict detection, one v0.2.x log marker, and three permanent Godot node names
18-setting continuity: PASS, ordered default + all-nondefault + enum values + restart + rollback
Stable result: 168/168 contracts; Release build 0 warnings / 0 errors
Beta result: 168/168 contracts; Release build 0 warnings / 0 errors
Publish/install/runtime performed: No
Local config/Workshop changed: No
Old/new active manifest and DLL counts: source manifest 0 legacy / 1 target; guarded builds emit damage-forecast.dll only
Rollback tested/performed: No; rollback point is the complete G6-3 identity batch, never manifest alone
Commit/push/tag performed: No
Player-visible behavior changed: None; approved technical metadata now reports damage-forecast v0.2.0
Decisions/deviations: installer only refuses legacy conflicts; it does not move/delete anything until separately approved G6-4 tooling
Exact next action: G6-4 dry-run-first recoverable install/upgrade/rollback tooling with path, manifest, process, staging, backup and restoration guards
Approval required next: explicit G6-4 approval
```

## 21. G6-4 checkpoint — 2026-07-22

```text
Date/session: 2026-07-22 / G6-4
Approved scope and decisions: dry-run-first recoverable install/upgrade/rollback tooling; no real machine mutation
HEAD / branch / remote: 308ffd5ff9b3e82b9726d4f1cc93a05c73eaaefb / main / origin
Dirty files before work: existing TG1-TG7 and G6-1..G6-3 migration work; retained without rollback or overwrite
Files changed by this Gate: Install-LocalMod.ps1; install-tool contract/validator; new IdentityUpgradeTool contract cases and Program registration; README/build-environment; this checkpoint
Active identity before / after: damage-forecast + DamageForecast / unchanged
Migration contract status before / after: active-migrated / active-migrated
Installer default and execution gate: Mode=Plan; Install requires reviewed TransactionId + staging manifest/DLL SHA256 plus explicit -Execute; Rollback requires explicit -Execute
Safety guards: game process, manifest parsing, exact two-file staging, duplicate/orphan identity detection, resolved mods-root paths, Workshop default exclusion
Recovery guards: timestamp/transaction backup, target staging under mods root, rename activation, post-state verification, failure restore, retained target recovery and hash ledger; no Remove-Item
Temporary simulation: clean plan, legacy upgrade plan, duplicate rejection, staging-extra rejection, missing -Execute rejection, upgrade activation and rollback restoration PASS under work/identity-upgrade-contracts
18-setting continuity: PASS; no config file read or write by installer tests
Stable result: 175/175 contracts; Release build 0 warnings / 0 errors
Beta result: 175/175 contracts; Release build 0 warnings / 0 errors
Publish/install/runtime performed: No real publish/install/runtime; isolated temporary-directory move/rollback simulation only
Local config/Workshop changed: No
Backup/config hashes: no real backup/config; temporary hash ledgers destroyed with owned test fixtures
Rollback tested/performed: temporary-directory rollback PASS; real rollback not performed
Commit/push/tag performed: No
Player-visible behavior changed: None
Decisions/deviations: Workshop can be read-only scanned only with explicit root; any detected Workshop identity blocks local execution, and the script never mutates Workshop
Rollback point: G6-4 script/contract/docs diff only
Exact next action: G6-5 dual-target build, separately approved publish, exact two-file tree and hash/static closure candidate
Approval required next: explicit G6-5 approval, including separate publish authorization if publish is desired
```

## 22. G6-5 pre-publish checkpoint — 2026-07-22

```text
Date/session: 2026-07-22 / G6-5 pre-publish
Approved scope and decisions: G6-5 started; implementation, contract/static validation, and non-publish stable/beta guards only; actual publish not yet separately approved
HEAD / branch / remote: 308ffd5ff9b3e82b9726d4f1cc93a05c73eaaefb / main / origin
Dirty files before work: existing TG1-TG7 and G6-1..G6-4 migration work; retained without rollback or overwrite
Files changed by this checkpoint: new read-only Test-IdentityPublishTrees.ps1; dual-target exact-tree/validator integration; publish validation contract/model/cases/Program registration; build environment/task index; this checkpoint
Active identity before / after: damage-forecast + DamageForecast / unchanged
Migration contract status before / after: active-migrated / active-migrated
Publish validator contract: stable+beta; exactly two top-level files; manifest/id/stem/version/dependency/capability alignment; managed assembly identity; SHA256 comparison; hash differences rejected unless separately approved
Negative fixture result: arbitrary extra file, legacy manifest ID, and unapproved stable/beta hash difference rejected; fixtures use ignored temporary trees only
18-setting continuity: PASS through the shared contract set; no config read/write
Stable pre-publish result: 180/180 contracts; Release build 0 warnings / 0 errors
Beta pre-publish result: 180/180 contracts; Release build 0 warnings / 0 errors
Publish/install/runtime performed: No; no actual publish tree generated by this Gate yet
Local config/Workshop changed: No
Commit/push/tag performed: No
Player-visible behavior changed: None
Rollback point: G6-5 validator/contract/test/docs diff only
Exact next action: after final non-publish dual-target guard, request separate stable+beta publish approval; then run Build-DualTargets, record exact tree/hash evidence, and stop before install/runtime
Approval required next: explicit G6-5 stable+beta publish approval
```

## 23. G6-5 completion checkpoint — 2026-07-22

```text
Date/session: 2026-07-22 / G6-5 completion
Approved scope and decisions: explicit stable/beta publish approval; repository-ignored publish outputs and static artifact validation only
HEAD / branch / remote: 308ffd5ff9b3e82b9726d4f1cc93a05c73eaaefb / main / origin
Dirty files before work: existing TG1-TG7 and G6-1..G6-5 pre-publish work; retained without rollback or overwrite
Files changed by completion pass: Build-DualTargets named-parameter validator forwarding; static contract assertion; task index and this checkpoint
Active identity before / after: damage-forecast + DamageForecast / unchanged
Migration contract status before / after: active-migrated / active-migrated
Stable build/publish: PASS; Release 0 warnings / 0 errors; work/publish/stable/damage-forecast contains exactly damage-forecast.dll + damage-forecast.json
Beta build/publish: PASS; Release 0 warnings / 0 errors; work/publish/beta/damage-forecast contains exactly damage-forecast.dll + damage-forecast.json
Manifest validation: id=damage-forecast; stem=damage-forecast; version=v0.2.0; has_dll=true; has_pck=false; min_game_version=0.107.1; BaseLib min_version=3.3.4
Assembly validation: stable/beta managed assembly name=damage-forecast; no legacy active artifact
Stable DLL SHA256: FB863FF75EA33D6B3F7F9B1471E12E33D5FEBD75595EB680A2D0A49369FDE880
Beta DLL SHA256: FB863FF75EA33D6B3F7F9B1471E12E33D5FEBD75595EB680A2D0A49369FDE880
Stable manifest SHA256: 09D4DC4EFA54BAA11AB3B3B1E2660FF8DB15D5EA153103AB837AAD22DBEB68C5
Beta manifest SHA256: 09D4DC4EFA54BAA11AB3B3B1E2660FF8DB15D5EA153103AB837AAD22DBEB68C5
Hash comparison: artifactsIdentical=true; no difference approval used
Deviation/fix: first automatic validator call used array splatting and mis-bound named parameters after both publishes succeeded; changed to hashtable named-parameter splatting, added a static assertion, reran the complete integration successfully
Install/runtime performed: No
Local config/Workshop changed: No
Rollback tested/performed: No real install state changed; publish outputs are ignored build artifacts
Commit/push/tag performed: No
Player-visible behavior changed: None
Rollback point: G6-5 validator/build-script/contract/test/docs diff; ignored publish outputs can be regenerated
Exact next action: G6-6 migration-specific local runtime only after separate install and game-launch approval
Approval required next: explicit G6-6 install/game approval with target scope
```

## 24. G6-6A pre-migration runtime baseline — 2026-07-22

```text
Date/session: 2026-07-22 / G6-6A
Approved scope: read-only inspection plus user-launched current installed Mod baseline; no install and no intentional config/Workshop mutation
Game target: beta v0.109.0 / commit c12f634d
Installed identity: sts2-party-watch-v2 / manifest v0.1.0 / assembly sts2-party-watch-v2, Version=1.0.0.0
Installed tree: exactly sts2-party-watch-v2.json + sts2-party-watch-v2.dll; no G6-5 damage-forecast publish artifact installed
Installed DLL SHA256: 1BAC85248FD83B62BE9867FD3A2F8F3B54D676D96D3F6B25F71B720A62D544F0
Installed manifest SHA256: A0CCDE08C2C7C0DB1C5D3BFC374C7523DFF0ECF492D00E6F154BF1AE1DAC5E11
Fresh log SHA256: 4820E8D38380300940A4D0A9F1E21823D609289866EE75D1366EC35C7F072836
Load identity result: one manifest discovery, one assembly load, one STS2PartyWatch.MainFile initializer, one BaseLib registration, one legacy Loaded marker, and one completed Damage Forecast initialization
Ordinary runtime result: old Mod loads, creates the HUD, and displays it in ordinary hand states
Confirmed pre-existing defect: with Regret or BadLuck in hand, the complete HUD hides although an enemy retains a normal attack intent; expected and incoming values both disappear; removing the direct HP-loss card immediately restores the HUD
Pending runtime: Beckon was not reproduced and remains Suspected / Pending Runtime; it is not recorded as confirmed
Code evidence: CardTurnEndDamageInspector detects CreatureCmd.Damage and only trusts exact Burn; Regret/BadLuck therefore return false before VerifiedFixedTurnEndHpLossReader.TryReadEvent; both hand-reader paths become Unknown and projection has no displayable value
Regression provenance: commit 0de7d743581e49d7c961b1267c1a3dc1fb14243e introduced the strict card-shape ordering after earlier direct HP-loss runtime evidence
Coverage gap: no Regret/BadLuck/Beckon full hand-reader -> snapshot -> projection integration cases
Disposition: Pre-existing Behavior Defect, not a G6 technical identity migration regression; G6-5 publish uses the same source and is Code-Inferred to inherit it, not RuntimeVerified
Deferred repair direction only: recognize verified fixed direct HP-loss before strict Burn classification; keep unknown/modded CreatureCmd.Damage fail-closed; add full mixed-event integration coverage after G6 closure
Config before/after: exact ordered 18 keys and values byte-identical; SHA256 remained 783C0B880EB1851D813801A6FD35F4D266AE4A2FE69BE50C4812FC7BEA714553; BaseLib normal-exit save refreshed LastWriteTime only
Workshop changed: No; not scanned during G6-6A
Production source changed: No
Exact next action: separately approve migrated G6-6 install/runtime, while evaluating identity/load/config continuity independently from the known direct HP-loss HUD defect
Approval required next: explicit G6-6 install and game-launch scope
```

## 25. G6-6B real-mods read-only upgrade Plan — 2026-07-22

```text
Date/session: 2026-07-22 / G6-6B
Approved scope: real mods read-only upgrade Plan only; no install, move, delete, config mutation, or Workshop operation
Game process: not running before and after Plan
Game target: beta v0.109.0 / commit c12f634d
Staging target: work/publish/beta/damage-forecast; byte-identical to approved stable publish tree
Plan action: upgrade
Plan transactionId: 20260722T064407984Z
Staging manifest SHA256: 09D4DC4EFA54BAA11AB3B3B1E2660FF8DB15D5EA153103AB837AAD22DBEB68C5
Staging DLL SHA256: FB863FF75EA33D6B3F7F9B1471E12E33D5FEBD75595EB680A2D0A49369FDE880
Detected state: legacyActiveCount=1; targetActiveCount=0; orphanArtifactCount=0
Workshop: workshopScanned=false; workshopIdentityCount=0; no Workshop path was read or changed
Target install path: C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\damage-forecast
Planned recoverable backup: C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\.damage-forecast-backups\20260722T064407984Z-sts2-party-watch-v2
Planned operations only: game/process/staging/path verification; same-root staging; move legacy to recoverable backup; rename activation; verify legacy=0/target=1; restore legacy on failure
Execution requested/performed: No / No; -Execute was not passed
Post-Plan filesystem verification: legacy directory remains present; target directory absent; backup root absent; mods root still contains only sts2-party-watch-v2
Legacy manifest SHA256 after Plan: A0CCDE08C2C7C0DB1C5D3BFC374C7523DFF0ECF492D00E6F154BF1AE1DAC5E11 (unchanged)
Legacy DLL SHA256 after Plan: 1BAC85248FD83B62BE9867FD3A2F8F3B54D676D96D3F6B25F71B720A62D544F0 (unchanged)
STS2PartyWatch.cfg after Plan: 797 bytes; SHA256 783C0B880EB1851D813801A6FD35F4D266AE4A2FE69BE50C4812FC7BEA714553 (unchanged)
Production source/player behavior changed: No
Commit/push/tag performed: No
Exact next action: separately approve the reviewed transaction/hash-bound Install -Execute command; retain the backup through migrated runtime acceptance
Approval required next: explicit G6-6 install execution approval; game launch remains a separate user action
```

## 26. G6-6C transaction-bound real upgrade — 2026-07-22

```text
Date/session: 2026-07-22 / G6-6C
Approved scope: continue with the reviewed transaction/hash-bound Install -Execute; no game launch, backup deletion, config mutation, or Workshop operation
Execution command binding: transactionId=20260722T064407984Z; beta staging manifest/DLL SHA256 exactly as approved in G6-6B
Game process: not running before, during, or after install verification
Install result: PASS; active legacy identity moved to recoverable backup and damage-forecast activated
Filesystem state after install, before Loader runtime: legacyActiveCount=0; targetActiveCount=1; orphanArtifactCount=0; post-install Plan action=target-already-active
Active install path: C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\damage-forecast
Active manifest: damage-forecast v0.2.0; exactly damage-forecast.json + damage-forecast.dll
Active manifest SHA256: 09D4DC4EFA54BAA11AB3B3B1E2660FF8DB15D5EA153103AB837AAD22DBEB68C5
Active DLL SHA256: FB863FF75EA33D6B3F7F9B1471E12E33D5FEBD75595EB680A2D0A49369FDE880
Active assembly identity: damage-forecast
Recoverable legacy backup: C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\.damage-forecast-backups\20260722T064407984Z-sts2-party-watch-v2
Backup manifest SHA256: A0CCDE08C2C7C0DB1C5D3BFC374C7523DFF0ECF492D00E6F154BF1AE1DAC5E11
Backup DLL SHA256: 1BAC85248FD83B62BE9867FD3A2F8F3B54D676D96D3F6B25F71B720A62D544F0
Backup assembly identity: sts2-party-watch-v2; backup retained and not loader-active
Install ledger: C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\.damage-forecast-backups\20260722T064407984Z-install-ledger.json
Install ledger SHA256: BA45C6F2CAFD3E2E75682C3F9B895C34020A125F22E74F06B476012A53B3CF9E
STS2PartyWatch.cfg after install: 797 bytes; SHA256 783C0B880EB1851D813801A6FD35F4D266AE4A2FE69BE50C4812FC7BEA714553 (unchanged)
Workshop: excluded; not scanned or changed
Rollback performed: No; recoverable inputs and ledger retained pending runtime acceptance
Game/runtime performed after install: Yes; runtime acceptance failed because Loader recursively treated the backup as active content
Production repository source changed by this step: No
Commit/push/tag performed: No
Exact next action: user launches beta v0.109.0 and verifies single target identity, one initializer/config page/HUD, 18-setting continuity, full restart persistence, diagnostic markers, and installed hash attribution; known direct HP-loss defect remains separate
Stop condition: do not delete backup or begin rollback until migrated runtime evidence is recorded and separately reviewed
```

## 27. G6-6 runtime duplicate-load stop — 2026-07-22

```text
Date/session: 2026-07-22 / post-G6-6C beta runtime
User observation: Mod list showed two Damage Forecast entries while config and HUD appeared normal
Fresh log SHA256: 25F7DAD7E22EF958015AAA88A7712CC5733FA06E7BD5456106372235FE3B2BC6
Loader discovery: mods/.damage-forecast-backups/20260722T064407984Z-install-ledger.json, the backed-up sts2-party-watch-v2.json, and active damage-forecast.json were all recursively discovered
Loaded order: backed-up sts2-party-watch-v2.dll first; active damage-forecast.dll second
Legacy runtime: STS2PartyWatch.MainFile initialized, BaseLib registered sts2-party-watch-v2, and [STS2 Party Watch] Loaded was emitted
Target runtime: DamageForecast.MainFile was called but emitted "Disabled because legacy identity sts2-party-watch-v2 is also loaded"
Mod list result: Damage Forecast [sts2-party-watch-v2] v0.1.0 plus Damage Forecast [damage-forecast] v0.2.0
Actual functional owner: legacy v0.1.0; apparently normal config/HUD behavior cannot be attributed to v0.2.0
Workshop involvement: none for Damage Forecast; both identities came from the local game mods tree
Root cause: the approved G6-4 backupRootPolicy=inside-mods-root and installer self-scan exclusion did not model the game's recursive manifest discovery; a backup under mods remains loader-active
Disposition: migration runtime FAIL/STOP; do not mark load/config/HUD continuity RuntimeVerified for the new identity
Preservation: target install, legacy backup, ledger, and STS2PartyWatch.cfg remain in place; no deletion or corrective move performed in this diagnostic step
Post-runtime hash check: target manifest/DLL, backup manifest/DLL, and 797-byte STS2PartyWatch.cfg all retain their pre-launch SHA256 values
Required corrective design: move backups outside every Loader-scanned mods root while retaining a game-root/path boundary, update installer/contract/tests, preserve hashes and rollback metadata, then relocate the existing backup without deletion
Required negative coverage: an upgrade backup must not be discoverable by a recursive Loader-style *.json scan; ledger JSON must also remain outside the scan root
Exact next action: obtain approval for G6-6D corrective tooling plus a recoverable same-volume relocation of the existing backup root, followed by single-identity static verification and user-run beta restart
Approval required next: explicit G6-6D tooling correction and backup relocation approval; no game launch until relocation is verified
```

## 28. G6-6D backup boundary correction — 2026-07-22

```text
Date/session: 2026-07-22 / G6-6D
Approved scope: correct the backup boundary and move the existing backup intact out of mods; no backup deletion, config mutation, or Workshop mutation
Installer policy before / after: inside-mods-root / inside-game-root-outside-loader-mods-root
Loader scan model: recursive-json-under-mods-root
Unsafe custom backup disposition: reject before mutation
Tooling changes: default backup root is gameRoot/.damage-forecast-backups; path guard requires it inside game root and outside mods; local identity and post-action checks use recursive Loader-style manifest discovery
Contract coverage added: IU-007 requires exactly one Loader-visible manifest after upgrade and rollback; IU-008 rejects a backup root inside mods without mutation
Stable result: 181/181 contracts; Release build 0 warnings / 0 errors
Beta result: 181/181 contracts; Release build 0 warnings / 0 errors
Git diff/artifact guard: PASS; tracked forbidden=0; working forbidden=0
Game process before and after relocation: 0
Relocation source: C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\.damage-forecast-backups
Relocation destination: C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\.damage-forecast-backups
Relocation result: source absent; destination present; same-volume whole-directory move; no file deleted or rewritten
Preserved ledger: 20260722T064407984Z-install-ledger.json; 588 bytes; SHA256 BA45C6F2CAFD3E2E75682C3F9B895C34020A125F22E74F06B476012A53B3CF9E
Preserved legacy DLL: sts2-party-watch-v2.dll; 112128 bytes; SHA256 1BAC85248FD83B62BE9867FD3A2F8F3B54D676D96D3F6B25F71B720A62D544F0
Preserved legacy manifest: sts2-party-watch-v2.json; 375 bytes; SHA256 A0CCDE08C2C7C0DB1C5D3BFC374C7523DFF0ECF492D00E6F154BF1AE1DAC5E11
Ledger interpretation: its embedded legacyBackupPath is immutable historical G6-6C evidence and still names the original location; the verified current backup path is the relocation destination above, and any future rollback must supply that current absolute path explicitly
Active target DLL: damage-forecast.dll; 135168 bytes; SHA256 FB863FF75EA33D6B3F7F9B1471E12E33D5FEBD75595EB680A2D0A49369FDE880
Active target manifest: damage-forecast.json; 371 bytes; SHA256 09D4DC4EFA54BAA11AB3B3B1E2660FF8DB15D5EA153103AB837AAD22DBEB68C5
Loader-visible recursive JSON after relocation: exactly mods/damage-forecast/damage-forecast.json
Real corrected Plan: action=target-already-active; legacyActiveCount=0; targetActiveCount=1; orphanArtifactCount=0; backupOutsideLoaderRoot=true; Workshop not scanned
Persistence verification: STS2PartyWatch.cfg remains 797 bytes with exactly the approved ordered 18 keys; SHA256 783C0B880EB1851D813801A6FD35F4D266AE4A2FE69BE50C4812FC7BEA714553
Workshop disposition: not scanned, installed, moved, deleted, or modified
Runtime status: pending; static single-identity state is verified, but v0.2.0 load/config/HUD continuity is not yet RuntimeVerified
Exact next action: user launches beta v0.109.0 once, confirms one Damage Forecast [damage-forecast] v0.2.0 entry and ordinary-hand HUD/config continuity, exits normally, then the next read-only step inspects the fresh log and rechecks the 18-setting config/hash
Stop condition: do not delete the relocated backup, run rollback, modify config/Workshop, or claim migrated RuntimeVerified before the fresh runtime evidence is reviewed
```

## 29. Post-G6-6D beta migration smoke — 2026-07-22

```text
Date/session: 2026-07-22 / corrected beta runtime
Environment: beta v0.109.0; game commit c12f634d
User observation: the requested single-identity/config/ordinary-HUD smoke looked normal; game exited normally before log inspection
Fresh log path: C:\Users\ROG\AppData\Roaming\SlayTheSpire2\logs\godot.log
Fresh log timestamp: 2026-07-22T15:11:47.9813699+08:00
Fresh log length / SHA256: 44664 bytes / 008436D50462869EF189E038089BBF4850791733543F3BD9C7808403A7FB8987
Loader discovery: exactly mods/damage-forecast/damage-forecast.json for this identity
Mod list: exactly Damage Forecast [damage-forecast] v0.2.0 for this identity
Initializer: DamageForecast.MainFile called once; finished once
BaseLib registration: damage-forecast registered once
Diagnostic attribution: [Damage Forecast] Loaded once
Legacy absence: no sts2-party-watch-v2 manifest, no STS2PartyWatch.MainFile call, no legacy-conflict disable message, and no identity-related error hit
Loaded artifact: damage-forecast.dll; 135168 bytes; SHA256 FB863FF75EA33D6B3F7F9B1471E12E33D5FEBD75595EB680A2D0A49369FDE880
Loaded manifest: damage-forecast.json; 371 bytes; SHA256 09D4DC4EFA54BAA11AB3B3B1E2660FF8DB15D5EA153103AB837AAD22DBEB68C5
Persistence after normal exit: STS2PartyWatch.cfg remains 797 bytes; exactly 18 approved ordered keys; SHA256 783C0B880EB1851D813801A6FD35F4D266AE4A2FE69BE50C4812FC7BEA714553
Backup after runtime: exactly three preserved files; ledger, legacy DLL, and legacy manifest hashes remain BA45C6F2CAFD3E2E75682C3F9B895C34020A125F22E74F06B476012A53B3CF9E / 1BAC85248FD83B62BE9867FD3A2F8F3B54D676D96D3F6B25F71B720A62D544F0 / A0CCDE08C2C7C0DB1C5D3BFC374C7523DFF0ECF492D00E6F154BF1AE1DAC5E11
Beta G6-D10 disposition: matching-artifact and config smoke PASS; G6-R02 upgrade outcome, G6-R03 continuity, G6-R04 restart persistence, G6-R07 diagnostic attribution, and G6-R08 artifact match are verified at beta-smoke depth
Not claimed: stable runtime, stable clean install, deliberate conflict runtime/preflight on the real installation, or real rollback
Known separate defect: Regret/BadLuck direct HP-loss HUD hiding remains the pre-existing behavior defect; Beckon remains Suspected/Pending Runtime
Workshop disposition: not scanned or modified
Commit/push/tag: not performed
Exact next action: request G6-6E approval for the stable full migration runtime matrix, with every real install/rollback mutation bound to reviewed paths and hashes and every game launch performed by the user
Stop condition: do not switch the real installation, rollback, delete any backup, modify config/Workshop, or declare G6 closed without separate G6-6E approval
```

## 30. G6-6E stable full migration runtime matrix — 2026-07-22

```text
Date/session: 2026-07-22 / G6-6E
Approved scope: stable full migration runtime matrix; every real mutation recoverable and transaction-bound; no backup deletion, config mutation, or Workshop mutation; user performs every game launch
Environment: stable v0.107.1; branch v0.107.1; game commit 59260271
Initial state: damage-forecast v0.2.0 active; legacy=0; target=1; Loader-visible identity manifest=1

Rollback transaction: 20260722T072318301Z
Rollback source: active mods/damage-forecast
Rollback legacy source: gameRoot/.damage-forecast-backups/20260722T064407984Z-sts2-party-watch-v2
Rollback target recovery: gameRoot/.damage-forecast-backups/20260722T072318301Z-target-before-rollback
Rollback ledger: 20260722T072318301Z-rollback-ledger.json; 556 bytes; SHA256 CEC5544DC6A8069A6184A4D79EAF0D4CE8FE365D5E6A6C6E4D0C10735D268DCE
Rollback static result: legacy=1; target=0; Loader-visible manifest=1; config hash unchanged
Rollback runtime result: user confirmed old identity config/ordinary-HUD normal; log shows one sts2-party-watch-v2 manifest, one STS2PartyWatch.MainFile initializer, one legacy BaseLib registration, and no identity-related error
Rollback runtime log: 933032 bytes; SHA256 599F4A900780083F2A294E1795018B41ED154293037C8700D4AA900D8AC096C2

Stable upgrade Plan/transaction: 20260722T073334528Z; action=upgrade; legacy=1; target=0; orphan=0; backupOutsideLoaderRoot=true
Stable upgrade publish hashes: manifest 09D4DC4EFA54BAA11AB3B3B1E2660FF8DB15D5EA153103AB837AAD22DBEB68C5; DLL FB863FF75EA33D6B3F7F9B1471E12E33D5FEBD75595EB680A2D0A49369FDE880
Stable upgrade legacy backup: gameRoot/.damage-forecast-backups/20260722T073334528Z-sts2-party-watch-v2
Stable upgrade ledger: 20260722T073334528Z-install-ledger.json; 582 bytes; SHA256 5B5BE8047972FD16F8F7241582B06843C02C892597DE46EFAEAE3B492B77CAC4
Stable upgrade static result: legacy=0; target=1; Loader-visible manifest=1; config hash unchanged
Stable upgrade runtime result: user confirmed one target config/ordinary-HUD normal; log shows one damage-forecast manifest, one DamageForecast.MainFile initializer, one target BaseLib registration, one v0.2.0 mod-list entry, and no identity-related error
Stable upgrade runtime log: 45054 bytes; SHA256 326D70A297B7315A71DAC5755B2F28F5D204C957BD2BDBFB91F28BC23BDE39B7

Conflict preflight case: work/g6-6e-conflict-20260722T094644200Z/game using exact real legacy and target artifact hashes
Conflict preflight result: Plan exit code 1 with Duplicate identity state: legacy=1; target=1; four files before/after unchanged; no backup root created; real mods never contained both identities

Clean-install preparation transaction: 20260722T094734097Z
Clean-install preparation recovery: gameRoot/.damage-forecast-backups/20260722T094734097Z-target-before-clean-install
Clean-install preparation ledger: 20260722T094734097Z-clean-install-prep-ledger.json; 1064 bytes; SHA256 9212BCF4B5AA91D5104F51B1904C8AC1379BD0E784AF82B2F57D48E413277A93
Clean-install preparation result: legacy=0; target=0; no identity manifest Loader-visible; config hash unchanged
Clean-install Plan/transaction: 20260722T094810282Z; action=clean-install; legacy=0; target=0; orphan=0; backupOutsideLoaderRoot=true
Clean-install ledger: 20260722T094810282Z-install-ledger.json; 454 bytes; SHA256 B62610844001A05BD4783851422842F31A1916CF761BF6C74B058D6CDAA30B82
Clean-install static result: legacy=0; target=1; Loader-visible manifest=1; exact approved stable publish hashes; preparation recovery preserved
Clean-install runtime result: user confirmed one target config/ordinary-HUD normal; log shows one damage-forecast manifest, one DamageForecast.MainFile initializer, one target BaseLib registration, one v0.2.0 mod-list entry, and no identity-related error
Clean-install runtime log: 1224259 bytes; SHA256 4C8D0EB8FE437A47D0038CAB74B8190728DE20BF1A3DF243DD007E37B91574EE

G6 runtime matrix disposition: G6-R01 clean install PASS; G6-R02 old-install upgrade PASS; G6-R03 18-setting continuity PASS; G6-R04 full-restart persistence PASS; G6-R05 conflict preflight PASS; G6-R06 rollback PASS; G6-R07 diagnostic attribution PASS; G6-R08 matching artifact PASS
Final active DLL: damage-forecast.dll; 135168 bytes; SHA256 FB863FF75EA33D6B3F7F9B1471E12E33D5FEBD75595EB680A2D0A49369FDE880
Final active manifest: damage-forecast.json; 371 bytes; SHA256 09D4DC4EFA54BAA11AB3B3B1E2660FF8DB15D5EA153103AB837AAD22DBEB68C5
Final persistence: STS2PartyWatch.cfg remains 797 bytes with exactly the approved ordered 18 keys; SHA256 783C0B880EB1851D813801A6FD35F4D266AE4A2FE69BE50C4812FC7BEA714553
Final backup disposition: 11 files across preserved rollback/upgrade/clean-install recovery sets and five ledgers; no backup deleted or rewritten
Final guard: stable 181/181 and beta 181/181 contracts; both Release builds 0 warnings / 0 errors; diff/artifact guard PASS
Version-switch incident: beta SerializableRun v19 was newer than stable v16; stable preserved it as a FutureVersion file and required abandoning/starting a stable-compatible run. Log evidence attributes this to game-version downgrade, not Damage Forecast; assistant did not modify save files
Known separate defect: Regret/BadLuck direct HP-loss HUD hiding remains the pre-existing behavior defect; Beckon remains Suspected/Pending Runtime
Workshop disposition: not scanned or modified
Commit/push/tag: not performed
Final local state: stable v0.107.1; legacy=0; target=1; Loader-visible identity manifest=1; game exited
Exact next action: request separate G6-7 approval to reconcile current authority, close the G6 task card/index, preserve historical evidence, and report repository/backup/Workshop/commit dispositions without deleting backups or modifying Workshop
Stop condition: do not perform G6-7 authority reconciliation, delete/clean backups, modify Workshop, stage/commit/push/tag, or claim full G6 closure without separate approval
```

## 31. G6-7 authority reconciliation, cleanup, and closure — 2026-07-22

```text
Date/session: 2026-07-22 / G6-7
Approved scope: reconcile current authority and close G6; additionally clean the completed migration backups safely; no install/config/Workshop mutation and no stage/commit/push/tag
Authority files reconciled: README.md; docs/project-state.md; docs/architecture.md; docs/interface-map.md; docs/build-environment.md; docs/mechanics-evidence.md; docs/v2-roadmap.md; docs/task-notes/README.md; name-migration-inventory.md; this task card
Current-state corrections: damage-forecast registration/load identity; permanent STS2PartyWatch.cfg persistence island; G6 stable/beta runtime depth; completed guardrail status; exact publish/install hashes; confirmed Regret/Bad Luck defect and Beckon pending boundary
Historical preservation: original Phase 12C and inventory rows retained; G6 mapping appended; dated commands, old paths, hashes, runtime statements, and evidence not bulk rewritten

Cleanup target: C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\.damage-forecast-backups
Cleanup boundary: exact game-root child, strictly outside mods, attributes=Directory, reparsePoint=false
Cleanup preflight: game process=0; directory count=3; file count=11; total bytes=386825; legacy=0; target=1; Loader-visible JSON=1; config SHA256 unchanged
Reviewed cleanup inventory:
  20260722T064407984Z-install-ledger.json | 588 | BA45C6F2CAFD3E2E75682C3F9B895C34020A125F22E74F06B476012A53B3CF9E
  20260722T072318301Z-rollback-ledger.json | 556 | CEC5544DC6A8069A6184A4D79EAF0D4CE8FE365D5E6A6C6E4D0C10735D268DCE
  20260722T072318301Z-target-before-rollback/damage-forecast.dll | 135168 | FB863FF75EA33D6B3F7F9B1471E12E33D5FEBD75595EB680A2D0A49369FDE880
  20260722T072318301Z-target-before-rollback/damage-forecast.json | 371 | 09D4DC4EFA54BAA11AB3B3B1E2660FF8DB15D5EA153103AB837AAD22DBEB68C5
  20260722T073334528Z-install-ledger.json | 582 | 5B5BE8047972FD16F8F7241582B06843C02C892597DE46EFAEAE3B492B77CAC4
  20260722T073334528Z-sts2-party-watch-v2/sts2-party-watch-v2.dll | 112128 | 1BAC85248FD83B62BE9867FD3A2F8F3B54D676D96D3F6B25F71B720A62D544F0
  20260722T073334528Z-sts2-party-watch-v2/sts2-party-watch-v2.json | 375 | A0CCDE08C2C7C0DB1C5D3BFC374C7523DFF0ECF492D00E6F154BF1AE1DAC5E11
  20260722T094734097Z-clean-install-prep-ledger.json | 1064 | 9212BCF4B5AA91D5104F51B1904C8AC1379BD0E784AF82B2F57D48E413277A93
  20260722T094734097Z-target-before-clean-install/damage-forecast.dll | 135168 | FB863FF75EA33D6B3F7F9B1471E12E33D5FEBD75595EB680A2D0A49369FDE880
  20260722T094734097Z-target-before-clean-install/damage-forecast.json | 371 | 09D4DC4EFA54BAA11AB3B3B1E2660FF8DB15D5EA153103AB837AAD22DBEB68C5
  20260722T094810282Z-install-ledger.json | 454 | B62610844001A05BD4783851422842F31A1916CF761BF6C74B058D6CDAA30B82
Cleanup method/result: whole exact directory sent to Windows Recycle Bin; recoverable=true; original target path absent; no permanent Remove-Item deletion used
Post-cleanup state: stable v0.107.1; legacy=0; target=1; Loader-visible identity manifest=1; STS2PartyWatch.cfg 797 bytes / 18 ordered keys / SHA256 783C0B880EB1851D813801A6FD35F4D266AE4A2FE69BE50C4812FC7BEA714553
Workshop: not accessed or modified
Commit/push/tag: not performed
Repository root rename: not performed
Final guards before and after cleanup: stable 181/181 and beta 181/181 on both runs; both Release builds 0 warnings / 0 errors; diff/artifact guard PASS
Final status: G6 Closed
Exact next optional task: separately approve creation of the direct HP-loss hand-classification behavior-fix task; no behavior fix is included in G6
```
