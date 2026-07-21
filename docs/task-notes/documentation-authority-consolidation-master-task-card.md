# Damage Forecast — 文档权威与笔记精简主任务卡

日期：2026-07-21

状态：Closed — DC0-DC6 complete；DC4 skipped

仓库：`https://github.com/Niizuma24290/Damage-Forecast.git`

任务类型：Documentation-only information architecture / authority reconciliation / historical preservation

优先级：应在 G6 Full Technical Identity Migration 和 Forecast Engine 架构稳定化的实质实现之前完成

---

## 0. 给下一 Session 的强制启动规则

下一 Session 必须先完整阅读本任务卡。

首次执行只允许进入 DC0：只读基线、资料盘点和方案校准。不得因为本任务卡存在就自动获得以下权限：

- 修改或移动现有笔记；
- 删除任何文件或历史证据；
- 修改源码、测试、脚本、manifest 或构建配置；
- stage、commit、push 或 tag；
- 安装、启动游戏或更新 Workshop；
- 开始 G6 全面技术改名；
- 开始 Forecast Engine 架构重构。

DC0 完成后，必须先向用户报告：

1. 当前文档数量、总行数和默认阅读成本；
2. 当前 authority 冲突、重复内容和过期 next-step 指针；
3. 建议保留的默认阅读集；
4. 建议合并、标记、归档但不删除的文档；
5. 是否建议物理移动文件，及其链接影响；
6. 第一批文档 diff 的精确范围；
7. 请求 DC1/DC2 实施批准。

未经批准，不得自动开始整理。

---

## 1. 一句话目标

在不丢失审计、运行时、hash、截图、历史决策和兼容证据的前提下，把当前大量任务笔记整理成“少量当前权威入口 + 可路由的历史证据库”，让新 Session 能快速确认当前状态、唯一下一步和需要按需读取的历史资料，同时消除重复维护和过期指针造成的上下文噪音。

---

## 2. 当前已知基线

下一 Session 必须重新统计，不得直接沿用：

```text
Known repository: Niizuma24290/Damage-Forecast
Known branch: main
Known HEAD before proposed task cards: e0f770c
Known tracked worktree state before proposed task cards: clean
Known Markdown files under docs/task-notes: 40
Known total task-note lines: 8,356
Known total task-note bytes: 465,230 (~454 KiB)
Known proposed untracked task cards:
  - forecast-engine-architecture-stabilization-master-task-card.md
  - documentation-authority-consolidation-master-task-card.md
```

计数说明：第二份任务卡创建后数量自然会增加。DC0 必须区分：

- tracked historical/current documents；
- untracked user-requested proposed task cards；
- ignored build/work artifacts；
- assets；
- repository-external master plans。

不得把数量变化本身当作问题；核心指标是默认阅读成本、权威冲突和重复维护成本。

---

## 3. 问题定义

当前资料体系具有丰富证据，但存在以下信息架构问题：

### 3.1 当前状态与历史记录混杂

- completed phase note 保留了当时真实的“下一步”；
- session handoff 保留了当时真实的 dirty worktree、remote、hash 和未完成项；
- 后续项目已经完成，但旧 note 中的 next-step 仍然会被全文搜索命中；
- 新 Session 容易把历史时点陈述误判为当前指令。

### 3.2 当前权威文件职责重叠

代表性重叠来源：

- `docs/project-state.md`
- `docs/v2-roadmap.md`
- `docs/task-notes/README.md`
- `docs/task-notes/project-total-note.md`
- 各 phase closure/handoff

它们可能重复维护当前功能、验证状态、已知限制和下一任务。

### 3.3 证据与摘要粒度不分层

- AUD 原始测量、审计台账、运行时记录非常详细；
- 日常新 Session 通常只需要当前状态和指向性链接；
- 现在缺少明确的“默认不要读全部证据，遇到哪个问题再读哪份”的路由策略。

### 3.4 笔记数量不等于可维护性

直接删除或压缩所有历史 note 会损失：

- commit/hash/artifact 证据；
- user runtime confirmation；
- 失败尝试和回滚原因；
- 旧版本 API 差异；
- Workshop/private subscription 历史；
- 审计可追溯性。

因此本任务的目标不是“把 40 份文件强行变成 5 份”，而是把默认权威和历史证据明确分层。

---

## 4. 成功定义

本任务完成后必须达到：

1. 新 Session 有一个明确的 START HERE 入口。
2. 当前产品状态、架构、机制证据、构建基线和未来任务各有唯一权威文件。
3. 默认阅读集不要求加载全部 completed phase/audit/handoff note。
4. 全部 task-note Markdown 都被分类并能从中央索引找到。
5. 历史 note 的真实时间点、hash、截图、测试和失败事实不被改写。
6. 旧的“下一步”能被识别为 Historical/Superseded，而不会与当前任务竞争权威。
7. `project-total-note.md` 与其他当前权威文件不再重复承担 current-state 职责。
8. 内部 Markdown 链接、图片引用和关键路径在整理后仍有效。
9. 当前两个 Proposed task card 有明确状态和阅读路由，但不会被误写为已批准/已开始。
10. 整理过程不修改代码、功能、technical identity、build artifact 或 Workshop。

软目标而非绝对硬限制：

- 默认入口/当前任务摘要应足够短，通常可在一次普通 Session 中完整读取；
- 当前权威文件只保留“现在仍然成立”的信息；
- 深度机制、审计和运行时证据通过链接按需读取；
- 不以追求任意行数目标为理由删除必要信息。

---

## 5. 文档权威层级

以下层级是建议模型。下一 Session 可以调整名称，但不得取消“当前权威”和“历史证据”的分离。

### Tier 0 — Start Here / 路由入口

候选：`docs/task-notes/README.md`，不建议无必要再增加新的永久入口文件。

职责：

- 当前项目一句话状态；
- 当前已批准任务 / Proposed tasks；
- 默认阅读顺序；
- authority map；
- 按主题进入历史证据的链接；
- 状态词定义；
- 明确哪些文档默认不读。

禁止：

- 复制完整机制说明；
- 复制长运行时记录；
- 维护第二份完整 roadmap；
- 复述所有 phase 细节。

### Tier 1 — Current Authority / 当前权威

建议职责分配：

| 文件 | 唯一职责 |
|---|---|
| `README.md` | 玩家/使用者产品说明、安装/功能边界 |
| `docs/project-state.md` | 当前实现、当前验证等级、当前限制、当前 release/repository 状态 |
| `docs/architecture.md` | 当前生产架构和硬边界；不写尚未实施的候选架构为事实 |
| `docs/mechanics-evidence.md` | 当前机制支持/条件支持/未知与证据矩阵 |
| `docs/build-environment.md` | 当前可复现构建、依赖、stable/beta reference 与验证命令 |
| `docs/v2-roadmap.md` | 已完成里程碑的短摘要、当前/未来方向和冻结项 |
| `docs/task-notes/README.md` | 文档路由、当前任务入口和历史索引 |

一个当前事实只能有一个主要 authority。其他文件可以引用，但不复制整段。

### Tier 2 — Active / Proposed Task Cards

包括：

- 本文档；
- `forecast-engine-architecture-stabilization-master-task-card.md`；
- 未来若用户批准创建的 G6 technical migration task card；
- 当前实际执行阶段的 checkpoint/handoff。

必须显式标记：

```text
Proposed / Not Started
Approved / In Progress
Blocked
Closed
Superseded
```

Proposed 不等于 current approved task。

### Tier 3 — Historical Phase Records

包括：

- completed phase task notes；
- completed runtime verification；
- historical Workshop/private subscription note；
- completed compatibility investigations；
- historical decisions and handoffs。

规则：

- 保留原始事实；
- 不再作为当前 next-step authority；
- 从中央索引按主题路由；
- 可以加非侵入式 status banner，但不得重写正文使其看起来像当时就知道未来结果。

### Tier 4 — Raw Audit / Measurement Evidence

包括：

- `phase-12c-audit/*`；
- AUD-0007 性能测量；
- 原始运行时 hash/日志摘要；
- screenshot assets；
- document disposition 和 name migration inventory。

规则：

- 默认不要求新 Session 全量读取；
- 仅在相关风险、mechanic、版本或历史争议出现时按需读取；
- 不为了变短而改写或删除原始审计证据；
- 当前结论应由 Tier 1 摘要并链接到证据。

---

## 6. 文档分类标签

DC0 必须为每一份 task note 分配一个主分类：

```text
CURRENT_AUTHORITY
PROPOSED_TASK
ACTIVE_TASK
HISTORICAL_RECORD
SUPERSEDED_HANDOFF
RAW_EVIDENCE
ASSET_INDEX
DUPLICATE_CANDIDATE
```

每份文档还应记录：

```text
Path
Title
Date/phase
Primary role
Current authority: Yes/No
Default read: Yes/No
Preserve verbatim: Yes/No
Superseded by
Inbound links
Outbound links/assets
Recommended action
Risk if moved/deleted
```

分类表可以暂存在本任务卡的 progress/attachment 中；除非确有必要，不要为了分类再永久创建多份登记文件。

---

## 7. 硬性保留边界

### 7.1 不得丢失的证据

- commit hash、tag、branch、remote 和时间点；
- DLL/manifest/asset hash、大小和 artifact whitelist；
- build、contract、runtime 结果；
- user confirmation 和截图路径；
- stable/beta API 差异；
- failure、rollback、diagnostic 和根因；
- Workshop item/visibility/subscription 历史；
- 已批准/未批准 Gate 的历史事实；
- migration inventory、document disposition 和审计 finding ID；
- 旧玩家名称与旧技术身份的历史事实。

### 7.2 不得误写

- 不把 BuildVerified 写成 RuntimeVerified；
- 不把 private/subscription Workshop 写成 public release；
- 不把本机多人 HUD 写成 teammate/shared HUD；
- 不把 Proposed task 写成 Approved/In Progress；
- 不把历史旧 URL 当成文档错误而全局替换；
- 不把历史旧名称全部替换为 Damage Forecast；
- 不把当时未批准的 G5/G6 描述改写成“当时已批准”。

### 7.3 不得顺带修改

- 源码、测试、脚本、manifest；
- Mod ID、DLL、namespace、settings key；
- Workshop；
- 游戏文件；
- Forecast Engine task scope；
- G6 technical migration implementation。

---

## 8. 精简策略

### 8.1 推荐第一阶段：不移动文件

优先采用低风险 authority consolidation：

- 精简 Tier 0/Tier 1 当前权威内容；
- 在中央索引分类所有历史 note；
- 标记 superseded handoff；
- 把重复 current-state 内容改成链接；
- 保持历史文件路径稳定；
- 修复明显失效的 current links；
- 不物理移动或删除历史文件。

优点：

- 最小链接 churn；
- Git 历史和外部引用稳定；
- 容易审查和回滚；
- 立即降低默认阅读成本。

### 8.2 可选第二阶段：物理 archive

只有在 DC0/DC1 证明有实际收益并获得单独批准后，才考虑：

```text
docs/task-notes/archive/phases/
docs/task-notes/archive/handoffs/
docs/task-notes/archive/audit/
```

物理移动前必须：

- 构建完整 inbound/outbound link map；
- 检查 README、docs、task cards 和外部 master plan 的引用；
- 记录旧路径到新路径 mapping；
- 使用 Git move/可追踪 rename；
- 修复全部内部链接；
- 验证图片资源；
- 不移动可能被外部任务直接引用的文件，除非有稳定 redirect/index strategy。

物理 archive 不是本任务完成的必要条件。默认建议先不移动。

### 8.3 删除策略

默认不删除任何历史 note。

只有满足全部条件的文件才可成为删除候选：

- 内容是完全重复或生成性临时文件；
- 没有唯一 hash/runtime/failure/decision 证据；
- 没有重要 inbound link；
- 已在保留文件中完整且准确地承接；
- 用户明确批准删除精确路径；
- 删除后 link check 通过。

无法证明则保留并分类。

---

## 9. 分阶段实施与 Gate

### DC0 — Read-Only Inventory And Authority Audit

类型：只读

动作：

- 检查 branch/remote/HEAD/status/tags；
- 识别 tracked、untracked、ignored、assets；
- 统计所有 Markdown 的路径、行数、大小和修改日期；
- 提取标题、状态、日期、next-step、URL 和主要链接；
- 建立 authority/role 分类草案；
- 查找 current-state 冲突和过期指针；
- 标出不能自动改写的历史 literal；
- 提出第一批最小文档 diff。

输出：

- inventory summary；
- authority conflict list；
- proposed default-read set；
- proposed disposition table；
- broken/suspicious link list；
- DC1/DC2 审批请求。

完成门槛：没有写文件、没有 stage/commit/push/tag。

### DC1 — Authority Model Approval And Scope Freeze

目标：在改文档前让用户批准哪些文件承担当前权威。

必须确认：

- Tier 0 入口；
- Tier 1 文件职责；
- `project-total-note.md` 的未来角色；
- Proposed task cards 的入口和状态；
- 是否允许给历史 note 加 banner；
- 是否允许物理移动；
- 是否存在允许删除的精确候选；
- commit/push/tag 边界。

输出：scope freeze，不做大范围内容修改。

### DC2 — Current Authority Consolidation

目标：让当前事实只有一套权威表达。

推荐顺序：

1. 精简 `docs/task-notes/README.md` 为 START HERE/router；
2. 把 `docs/project-state.md` 限定为当前状态；
3. 把 `docs/v2-roadmap.md` 限定为阶段摘要与未来方向；
4. 确认 `architecture.md` 只描述生产现状；
5. 确认 `mechanics-evidence.md` 承担机制矩阵；
6. 确认 `build-environment.md` 承担可复现构建；
7. 合并 `project-total-note.md` 中仍唯一有效的当前内容，然后标记其历史/被替代角色。

规则：

- 使用链接代替大段复制；
- 不修改历史 note 正文；
- 不改变技术结论；
- 每次只处理一个 authority 主题；
- 保留 current hash/version/status 的可追溯来源。

完成门槛：

- current authority 无明显矛盾；
- 当前唯一任务/Proposed tasks 状态一致；
- 新 Session 能从 Tier 0 路由到所有 Tier 1。

回滚点：docs-only 独立 commit，是否 commit 仍需批准。

### DC3 — Historical Routing And Supersession

目标：让历史资料容易查，但默认不干扰当前任务。

动作候选：

- 在中央索引按主题/phase 分类；
- 标记 session handoff 为 `SUPERSEDED_HANDOFF`；
- 标记 audit/measurement 为 `RAW_EVIDENCE`；
- 标记 completed phase 为 `HISTORICAL_RECORD`；
- 为旧 next-step 提供中央 superseded-by 路由；
- 如获批准，可在历史 note 顶部增加最小非侵入式 banner。

Banner 候选格式：

```text
> Historical record. Preserved as evidence for its original date.
> Current authority: <path>. Do not use this file's historical next-step as the current task.
```

不得批量重写历史正文。

### DC4 — Optional Physical Archive

默认：Skip。

只有用户批准精确目录/文件 mapping 后执行。

完成门槛：

- link map 完整；
- move 与 link repair 在同一审查批次；
- 0 broken internal links；
- assets 可解析；
- Git 能识别合理 rename；
- 外部直接路径风险已记录。

### DC5 — Link, Terminology And Context Validation

必须检查：

- Markdown relative links；
- 本地图片/asset references；
- task-note filename references；
- old/new GitHub URL 的 current vs historical 边界；
- `Damage Forecast` / `Party Watch` 当前与历史语义；
- current next-step/approved task 唯一性；
- Workshop public/private wording；
- BuildVerified/RuntimeVerified 分类；
- G5 closed、G6 optional、Forecast Engine Proposed 状态；
- `git diff --check`；
- staged path boundary；
- 无源码或 artifact 进入 diff。

建议生成临时 link checker 输出，但不要无必要把 generated report 永久提交。

### DC6 — Documentation Closure

目标：完成文档精简项目收口。

输出：

- before/after 统计；
- 默认阅读集；
- authority map；
- historical routing summary；
- moved/deleted files（通常 none，除非批准）；
- preserved evidence statement；
- broken link result；
- exact diff scope；
- 下一优先顺序：AR1 test baseline -> optional G6 -> Forecast Engine refactor。

commit/push/tag 只有用户明确批准后执行。

---

## 10. 默认阅读集建议

完成后，普通新 Session 默认只应读取：

```text
1. docs/task-notes/README.md
2. docs/project-state.md
3. 当前已批准任务卡
```

然后按任务类型追加：

| 任务类型 | 按需读取 |
|---|---|
| 预测机制 | `docs/mechanics-evidence.md` + 对应 historical note |
| 架构重构 | `docs/architecture.md` + Forecast Engine task card |
| 构建/兼容 | `docs/build-environment.md` + 对应 stable/beta note |
| 产品/玩家文案 | `README.md` + current rename note |
| 未来路线 | `docs/v2-roadmap.md` |
| 审计复核 | `phase-12c-audit/*` 中精确 ledger |
| Workshop | `workshop-private-rc-2026-07-01.md` + current project state |
| G6 全面改名 | name migration inventory + 未来 G6 task card |

不要让普通任务默认读取全部 40+ task notes。

---

## 11. `project-total-note.md` 处理建议

该文件当前与 project-state、task README、roadmap 存在职责重叠。

推荐处置：

1. 逐段判断是否仍含唯一事实；
2. 唯一且当前的事实合并到对应 Tier 1 authority；
3. 历史事实保留在原始 phase note 或该文件中；
4. 将该文件标记为 Historical Reconciliation Record / Superseded as current authority；
5. 从默认阅读集移除；
6. 不直接删除。

如果它仍承担不可替代的跨 phase 总表，可以保留为 Tier 3 total historical index，但不得再维护 current next-step。

---

## 12. Proposed Task Cards 的处理

当前两个 Proposed task card 应明确区分：

1. `documentation-authority-consolidation-master-task-card.md`
   - 当前用户选择的下一项工作；
   - 仍需下一 Session 从 DC0 开始并请求实现批准。

2. `forecast-engine-architecture-stabilization-master-task-card.md`
   - Proposed / queued；
   - 文档精简完成后重新校准；
   - 建议顺序为 AR1 测试基线，然后根据用户决定进入 G6 或继续架构阶段；
   - 不得被文档精简 Session 自动实现。

未来 G6 task card 尚未创建，不得在索引中写成 Approved。

---

## 13. 验证清单

### 内容一致性

- [ ] 当前产品名一致。
- [ ] 技术身份保留状态一致。
- [ ] G5 closure 状态一致。
- [ ] Workshop 未公开/未在 G5 更新的边界一致。
- [ ] Proposed 与 Approved 状态一致。
- [ ] 当前唯一下一方向一致。
- [ ] multiplayer 本机边界一致。
- [ ] stable/beta baseline 一致。
- [ ] contract count 和 artifact hash 的 current authority 一致。

### 历史完整性

- [ ] 历史 commit/hash 未丢失。
- [ ] 历史 user runtime confirmation 未改写。
- [ ] audit finding/ID 可追溯。
- [ ] screenshots/assets 可到达。
- [ ] old URL/name 在历史上下文中保留。
- [ ] old next-step 被路由为 historical/superseded。

### 链接与仓库边界

- [ ] 0 broken internal Markdown links。
- [ ] 0 broken local asset references。
- [ ] 无意外 source/test/script/manifest diff。
- [ ] 无 DLL/PDB/PCK/log/bin/obj/publish/work/uploader/game files staged。
- [ ] `git diff --check` 通过。
- [ ] staged files 与批准 scope 完全一致。

### 可用性

- [ ] 新 Session 只读 Tier 0 + project-state 就能确认当前状态。
- [ ] 每类任务都有清晰按需阅读路径。
- [ ] 不需要全文搜索才能找到当前任务。
- [ ] 历史证据仍能通过主题/phase 找到。

---

## 14. 风险与控制

| Risk | 影响 | 控制 |
|---|---|---|
| 为精简而删除唯一证据 | 审计和回归链断裂 | 默认不删除；逐文件 disposition；用户精确批准 |
| 全局替换旧名称/URL | 历史失真 | current/historical 分类；禁止粗暴替换 |
| 物理移动造成断链 | 下一 Session 找不到资料 | 第一阶段不移动；link map + 单独 Gate |
| 新增更多索引反而更复杂 | 文档数量继续膨胀 | 优先复用 task README；临时 register 不永久化 |
| 摘要错误覆盖原始事实 | 权威漂移 | 摘要链接原始证据；保留 verification class |
| Proposed task 被误认为批准 | 未授权实现 | 状态标签 + Tier 2 路由 |
| 用户未提交 task card 被覆盖 | 用户工作丢失 | DC0 inventory；不 reset/restore/checkout |
| docs-only 任务混入代码 | 审查困难 | exact path scope；staged boundary check |
| 任意行数目标导致信息丢失 | 过度压缩 | 行数仅软目标，以可路由和唯一权威为验收 |

---

## 15. Stop / Escalation Conditions

遇到以下情况必须停止并报告：

- 某文件是否包含唯一证据无法确定；
- 需要删除或移动文件但用户未批准精确路径；
- current authority 之间存在实质技术结论冲突；
- 发现用户新建/修改但未登记的文档；
- 修复链接需要修改仓库外文件；
- 整理需要改变代码、设置、technical identity 或 Workshop；
- 需要把历史事实改写成当前品牌语言才能继续；
- staged diff 超出批准的文档清单；
- 当前任务排序发生变化。

报告格式：

```text
Issue
Affected documents
Conflicting claims/evidence
Historical/current classification
Safe options
Recommended option
Required user decision
```

---

## 16. Commit、Tag 与发布纪律

如果用户批准 commit，推荐使用小而清晰的 docs-only 边界：

```text
docs: establish documentation authority map
docs: consolidate current project state
docs: route historical task records
docs: close documentation consolidation
```

不应把物理 archive move 与 current-state 文案重写放在同一个不可审查提交中。

如果用户批准 tag，推荐 annotated milestone：

```text
documentation-authority-consolidated
```

Tag 说明候选：

```text
Current documentation authority consolidated with historical evidence preserved
```

规则：

- 不 force-push；
- 不 squash 已单独验收的整理阶段；
- 未通过 link/static 验证不打 closure tag；
- push/tag 必须再次确认 remote/branch；
- Workshop 不随 docs push 更新。

---

## 17. Session Checkpoint 模板

每个执行 Session 结束前记录：

```text
Date/session
Current DC stage
Approved scope
Branch / remote / HEAD
Tracked/untracked state before work
Document count / lines / bytes
Files classified this session
Current authority changes
Historical files modified/moved/deleted
Links checked and failures
Files changed
Non-doc changes: must be None
Commit/push/tag: Yes/No + hashes
Workshop changed: No
Open conflicts
Decisions/deviations
Rollback point
Exact next action
Next approval required
```

关键状态不得只留在聊天上下文。

---

## 18. 下一 Session 的第一步

严格按以下顺序：

1. 读取本任务卡。
2. 运行只读 Git baseline/status 检查。
3. 重新统计全部 docs/task-notes Markdown 和 assets。
4. 解析每份文件的标题、状态、日期、next-step 和 links。
5. 构建分类/disposition 草案。
6. 对 Tier 0/Tier 1 做冲突扫描。
7. 提出不移动文件的第一批精简方案。
8. 向用户报告并请求 DC1/DC2 批准。

第一批不得：

- 改任何文档；
- 删除/move；
- 更新 task README；
- 改 `project-total-note.md`；
- commit/push/tag；
- 开始 Forecast Engine 或 G6。

---

## 19. 可直接复制给下一 Session 的启动提示

```text
请完整阅读：
C:\Users\ROG\Documents\Codex\STS2-Party-Watch-v2\docs\task-notes\documentation-authority-consolidation-master-task-card.md

从 DC0 开始，只做只读文档基线、authority 冲突和 disposition 盘点。不要修改或移动任何文件，不要 stage、commit、push、tag，不要改代码，不要安装/启动游戏，不要更新 Workshop，也不要开始 Forecast Engine 或 G6。

重点交付：
1. 当前文档数量、行数、默认阅读成本；
2. Tier 0/Tier 1 当前权威建议；
3. 每份 task note 的分类和处置草案；
4. 过期 next-step、重复 current-state 和可疑链接；
5. 优先采用“不移动、不删除”的第一批最小精简 diff；
6. 需要用户批准的 DC1/DC2 精确范围。

完成 DC0 后先向我报告，不要自动实施。
```

---

## 20. DC0 / DC1 Checkpoint — 2026-07-21

```text
Date/session: 2026-07-21
Current DC stage: DC2 first batch approved / in progress
Approved scope: README.md; docs/project-state.md; docs/v2-roadmap.md;
  docs/build-environment.md; docs/task-notes/README.md;
  docs/task-notes/project-total-note.md; this master task card
Branch / remote / HEAD: main / origin / e0f770ca1fb7e01a4d811c1317cffcc2c05a9054
Tracked/untracked state before work: tracked clean; two untracked user task cards
Document count / lines / bytes: task-notes Markdown 41 / 9,159 / 490,023
DC0 classification: CURRENT_AUTHORITY 1; PROPOSED_TASK 2;
  DUPLICATE_CANDIDATE 1; RAW_EVIDENCE 6; SUPERSEDED_HANDOFF 6;
  HISTORICAL_RECORD 25
Authority decision: docs/task-notes/README.md is Tier 0; Tier 1 roles follow
  section 5; project-total-note.md becomes historical reconciliation
Physical move/delete: not approved; none planned
Historical banners: not approved as a bulk DC3 operation
Commit/push/tag: not approved
Workshop changed: No
Non-doc changes: must be None
Next approval required: review DC2 first-batch diff before any further batch
```

DC1 approval received from the user:

- approve the Tier 0/Tier 1 authority model;
- approve `project-total-note.md` as historical reconciliation rather than
  current authority;
- keep the Forecast Engine card Proposed/Queued;
- approve only the seven-file DC2 first batch recorded above;
- do not move/delete files, rewrite historical bodies, or commit/push/tag.

DC2 first-batch execution result:

```text
Files changed: exactly the seven approved Markdown files
Current authority changes: Tier 0 router established; project-state and roadmap
  current pointers reconciled; build contract baseline updated to 28/28;
  player README documents Phase 13A N; project-total-note marked historical
Historical phase/audit/handoff files modified: None
Files moved/deleted: None
Local Markdown references checked: 30
Broken local Markdown references: 0
git diff --check: passed; informational LF-to-CRLF checkout warnings only
Non-doc changes: None
Staged files: None
Commit/push/tag: No
Workshop changed: No
Exact next action: user review of the DC2 first-batch diff
Next approval required: any further DC2 refinement, DC3 banners, DC4 archive,
  commit, push, or tag
```

---

## 21. DC2 Completion Checkpoint — 2026-07-21

The user approved the second DC2 batch for:

```text
docs/architecture.md
docs/mechanics-evidence.md
```

Second-batch result:

```text
Architecture authority: reconciled with Phase 13A incoming-N pipeline,
  ForecastHudSnapshot, three measured HUD segments, and frozen beta v0.109.0
Mechanics authority: current ledger now includes Phase 13A incoming-N forward
  projection, inclusion switches, conservative Unknown boundary, and current
  verification class
Additional current-pointer refinement: docs/project-state.md beta hook wording
  reconciled from historical v0.108.0 to frozen beta v0.109.0
Historical phase/audit/handoff bodies modified: None
Source/test/script/manifest changes: None
Files moved/deleted: None
Commit/push/tag: No
Workshop changed: No
```

DC2 completion gates:

- current authority has no known material contradiction: Passed;
- current task and Proposed task states are consistent: Passed;
- Tier 0 routes a new Session to all Tier 1 authorities: Passed.

Current stage: DC2 complete. DC3 historical routing/banner work requires a
separate user approval. DC4 physical archive remains skipped by default.

---

## 22. DC2 Repair Checkpoint — 2026-07-21

### 22.1 Corrected classification

DC0 的原始分类是盘点时快照；下表是 DC6 closure 后的最终分类：

| Classification | Count | Meaning |
| --- | ---: | --- |
| `CURRENT_AUTHORITY` | 1 | Tier 0 task router |
| `ACTIVE_TASK` | 0 | 当前没有已批准任务 |
| `PROPOSED_TASK` | 1 | Forecast Engine task card；仍未获得 AR1 |
| `HISTORICAL_RECORD` | 27 | completed task/Phase/runtime/Workshop/compatibility 记录，含本卡与 `project-total-note.md` |
| `SUPERSEDED_HANDOFF` | 6 | 保留当时 hash、工作区和 next-step 的交接记录 |
| `RAW_EVIDENCE` | 6 | audit ledger、inventory 与 measurement evidence |
| **Total** | **41** | `DUPLICATE_CANDIDATE` 已归零；没有文件获准删除或移动 |

### 22.2 Complete 41-file disposition register

缩写：`A/D/P` = current authority / default read / preserve historical body。
`Y/Y/N` 只适用于可维护的当前 authority；`N/N/Y` 表示正文保留但默认不读。
Inbound 是本次基线中其他 Markdown 文件对该文件的引用数，不含 self-link；它用于评估移动风险，
不是 authority 等级。所有 `keep` 均表示留在原路径，本任务不授权 move/delete。

| Path | Title / date-phase | Disposition | A/D/P | Current route / superseded by | Inbound | Outbound / assets | Recommended action | Move/delete risk |
| --- | --- | --- | --- | --- | ---: | --- | --- | --- |
| `documentation-authority-consolidation-master-task-card.md` | Documentation Authority Consolidation / 2026-07-21 | `HISTORICAL_RECORD` | N/N/Y | Tier0 router + DC6 closure | 0 | Tier0/Tier1 + audit links | keep closed task evidence | High：完整 authority/disposition/closure evidence |
| `forecast-engine-architecture-stabilization-master-task-card.md` | Forecast Engine Architecture Stabilization / proposed | `PROPOSED_TASK` | N/N/Y | `architecture.md`；获批 AR1 后才激活 | 1 | architecture/source routes | keep queued；不得实施 | High：router 已引用且为 untracked user file |
| `modding-settings-entry-2026-07-02.md` | Modding settings entry / 2026-07-02 | `HISTORICAL_RECORD` | N/N/Y | `project-state.md` + `architecture.md` | 3 | note links | keep historical | Medium |
| `phase-0-repository-bootstrap.md` | Repository bootstrap / Phase 0 | `HISTORICAL_RECORD` | N/N/Y | `v2-roadmap.md` + `project-state.md` | 2 | note links | keep historical | Medium |
| `phase-11a-v108-quick-compatibility.md` | v1.08 quick compatibility / Phase 11A | `HISTORICAL_RECORD` | N/N/Y | `build-environment.md` + `project-state.md` | 6 | build/source links | keep historical | High：compatibility evidence |
| `phase-11b-stable-dll-initialization.md` | Stable DLL initialization / Phase 11B | `HISTORICAL_RECORD` | N/N/Y | `build-environment.md` + `project-state.md` | 4 | build/source links | keep historical | High：stable evidence |
| `phase-11c-poison-runtime-verification-safe-expansion.md` | Poison runtime verification / Phase 11C | `HISTORICAL_RECORD` | N/N/Y | `mechanics-evidence.md` + `project-state.md` | 4 | `assets/phase-11c-testsubject-intangible-tsi-04-2026-07-18.png` | keep historical | High：runtime evidence + asset |
| `phase-11d-diamond-diadem-mechanism-compatibility.md` | Diamond Diadem compatibility / Phase 11D | `HISTORICAL_RECORD` | N/N/Y | `mechanics-evidence.md` | 5 | source/note links | keep historical | High：mechanics evidence |
| `phase-11e-dual-version-build-baseline.md` | Dual-version build baseline / Phase 11E | `HISTORICAL_RECORD` | N/N/Y | `build-environment.md` + `project-state.md` | 5 | build/handoff links | keep historical | High：dual-target baseline |
| `phase-11-supplements.md` | Supplements index / Phase 11 | `HISTORICAL_RECORD` | N/N/Y | Tier0 router + topical Tier1 | 6 | Phase 11 note routes | keep historical index | High：many inbound routes |
| `phase-12a-baselib-auto-config-evaluation.md` | BaseLib auto-config evaluation / Phase 12A | `HISTORICAL_RECORD` | N/N/Y | `architecture.md` + `project-state.md` | 4 | BaseLib/source links | keep historical | Medium |
| `phase-12a-completion-handoff-2026-07-03.md` | Phase 12A completion handoff / 2026-07-03 | `SUPERSEDED_HANDOFF` | N/N/Y | `project-state.md` + Phase 12B record | 3 | `assets/phase-12a-runtime/*` | keep handoff | High：handoff + assets |
| `phase-12a-runtime-verification-2026-07-03.md` | Phase 12A runtime verification / 2026-07-03 | `HISTORICAL_RECORD` | N/N/Y | `project-state.md` + Phase 12B record | 5 | `assets/phase-12a-runtime/*` | keep historical | High：runtime evidence + assets |
| `phase-12a-session-handoff-2026-07-03.md` | Phase 12A session handoff / 2026-07-03 | `SUPERSEDED_HANDOFF` | N/N/Y | `project-state.md` + Phase 12B record | 2 | note links | keep handoff | High：session/hash state |
| `phase-12b-config-localization-language-color.md` | Config localization/language/color / Phase 12B | `HISTORICAL_RECORD` | N/N/Y | `architecture.md` + `project-state.md` | 6 | source/note links | keep historical | High：many inbound routes |
| `phase-12c-aud-0007-visibility-performance-measurement.md` | AUD-0007 visibility/performance / Phase 12C | `RAW_EVIDENCE` | N/N/Y | `project-state.md` current verification summary | 1 | audit/source links | keep raw evidence | High：measurement ledger |
| `phase-12c-audit/audit-final-report.md` | Audit final report / Phase 12C | `RAW_EVIDENCE` | N/N/Y | `project-state.md` + G5 closure | 3 | audit ledger links | keep raw evidence | High：audit conclusion basis |
| `phase-12c-audit/audit-progress.md` | Audit progress / Phase 12C | `RAW_EVIDENCE` | N/N/Y | final report + `project-state.md` | 2 | audit links | keep raw evidence | High：audit chronology |
| `phase-12c-audit/code-audit-findings.md` | Code audit findings / Phase 12C | `RAW_EVIDENCE` | N/N/Y | final report + current Tier1 | 3 | source/audit links | keep raw evidence | High：finding ledger |
| `phase-12c-audit/document-disposition-register.md` | Document disposition register / Phase 12C | `RAW_EVIDENCE` | N/N/Y | 本卡的当前 41-file register | 3 | documentation links | keep prior raw register | High：prior disposition evidence |
| `phase-12c-audit/name-migration-inventory.md` | Name migration inventory / Phase 12C | `RAW_EVIDENCE` | N/N/Y | `project-state.md`; future G6 task | 2 | source/path inventory | keep raw evidence | High：G6 input inventory |
| `phase-12c-g4-runtime-verification.md` | Surface rename runtime verification / G4 | `HISTORICAL_RECORD` | N/N/Y | `project-state.md` | 3 | source/runtime links | keep historical | High：runtime evidence |
| `phase-12c-g5-repository-closure.md` | Repository closure / G5 | `HISTORICAL_RECORD` | N/N/Y | `project-state.md` | 4 | commit/tag/remote evidence | keep historical | High：repository evidence |
| `phase-12c-surface-rename.md` | Surface rename / Phase 12C | `HISTORICAL_RECORD` | N/N/Y | `README.md` + `project-state.md` | 2 | source/audit links | keep historical | High：identity evidence |
| `phase-13a-damage-display.md` | Damage display / Phase 13A | `HISTORICAL_RECORD` | N/N/Y | `architecture.md` + `mechanics-evidence.md` + `project-state.md` | 5 | source/test links | keep historical | High：current feature provenance |
| `phase-1-4-singleplayer-baseline.md` | Single-player baseline / Phases 1-4 | `HISTORICAL_RECORD` | N/N/Y | `v2-roadmap.md` + `project-state.md` | 2 | source/note links | keep historical | Medium |
| `phase-5-blockable-incoming-damage.md` | Blockable incoming damage / Phase 5 | `HISTORICAL_RECORD` | N/N/Y | `mechanics-evidence.md` | 2 | source links | keep historical | Medium |
| `phase-6-direct-hp-loss.md` | Direct HP loss / Phase 6 | `HISTORICAL_RECORD` | N/N/Y | `mechanics-evidence.md` | 2 | source links | keep historical | Medium |
| `phase-7-hp-loss-result-modifiers.md` | HP-loss result modifiers / Phase 7 | `HISTORICAL_RECORD` | N/N/Y | `mechanics-evidence.md` | 1 | source links | keep historical | Medium |
| `phase-8-non-block-damage-modifiers.md` | Non-block damage modifiers / Phase 8 | `HISTORICAL_RECORD` | N/N/Y | `mechanics-evidence.md` | 2 | source links | keep historical | Medium |
| `phase-9a-followup-2026-07-01.md` | UI follow-up / 2026-07-01 | `HISTORICAL_RECORD` | N/N/Y | `architecture.md` + `project-state.md` | 2 | source/note links | keep historical | Medium |
| `phase-9a-ui-settings-lifecycle.md` | UI/settings lifecycle / Phase 9A | `HISTORICAL_RECORD` | N/N/Y | `architecture.md` + `project-state.md` | 2 | source links | keep historical | Medium |
| `phase-9b-poison-pre-action-survival.md` | Poison pre-action survival / Phase 9B | `HISTORICAL_RECORD` | N/N/Y | `mechanics-evidence.md` | 4 | source/note links | keep historical | High：Poison provenance |
| `phase-9-singleplayer-validation.md` | Single-player validation / Phase 9 | `HISTORICAL_RECORD` | N/N/Y | `project-state.md` + `v2-roadmap.md` | 2 | runtime/source links | keep historical | High：runtime milestone |
| `project-total-note.md` | Project Total Note / 2026-07-02 | `HISTORICAL_RECORD` | N/N/Y | all Tier1 authorities + Tier0 router | 5 | many historical links | keep in place; authority extracted | High：broad reconciliation evidence |
| `README.md` | Documentation Start Here / current | `CURRENT_AUTHORITY` | Y/Y/N | 本文件为 Tier0 | 13 | all Tier1/task routes | keep concise router | High：default entry point |
| `session-handoff-dual-target-build-baseline-2026-07-18.md` | Dual-target baseline handoff / 2026-07-18 | `SUPERSEDED_HANDOFF` | N/N/Y | `build-environment.md` + Phase 11E | 3 | build/note links | keep handoff | High：snapshot/hash state |
| `session-handoff-dual-target-build-complete-2026-07-18.md` | Dual-target complete handoff / 2026-07-18 | `SUPERSEDED_HANDOFF` | N/N/Y | `build-environment.md` + Phase 11E | 3 | build/note links | keep handoff | High：completion evidence |
| `session-handoff-dual-version-compatibility-2026-07-17.md` | Dual-version compatibility handoff / 2026-07-17 | `SUPERSEDED_HANDOFF` | N/N/Y | `build-environment.md` + Phase 11E | 3 | source/build links | keep handoff | High：compatibility state |
| `session-handoff-poison-testsubject-intangible-2026-07-18.md` | Poison/TestSubject/Intangible handoff / 2026-07-18 | `SUPERSEDED_HANDOFF` | N/N/Y | `mechanics-evidence.md` + Phase 11C | 3 | Phase 11C runtime asset | keep handoff | High：runtime/hash state |
| `workshop-private-rc-2026-07-01.md` | Workshop private RC / 2026-07-01 | `HISTORICAL_RECORD` | N/N/Y | `project-state.md` + Tier0 Workshop route | 8 | Workshop/runtime links | keep historical | High：external release evidence |

### 22.3 Repair scope and final-validation record

This repair corrected the DC2 record without widening authorization:

- Tier0 now distinguishes the active documentation card from the queued Forecast Engine card.
- The former 139-line historical snapshot was removed from the Tier0 physical default-read file;
  its evidence remains in the individual Phase/audit/handoff records and `project-total-note.md`.
- `project-total-note.md` now records paragraph/topic-level authority extraction and has no current-authority role.
- The register above persists a disposition for every one of the 41 task-note Markdown files.
- No historical Phase/audit/handoff body was rewritten; no file was moved or deleted.
- The two task cards remain untracked user files. Because the tracked Tier0 router links them,
  any future documentation commit must either include both cards or deliberately revise those links;
  this is a commit/closure risk, not authorization to stage them now.

Section 22 supersedes the earlier DC2 completion-gate summary wherever the repair found a
classification, disposition-persistence, physical-default-read, or touched-file discrepancy.
Final statistics and validation results after the repair pass:

```text
Documentation files actually touched across DC2 (9 unique):
  README.md
  docs/architecture.md
  docs/build-environment.md
  docs/mechanics-evidence.md
  docs/project-state.md
  docs/task-notes/README.md
  docs/task-notes/project-total-note.md
  docs/task-notes/documentation-authority-consolidation-master-task-card.md
  docs/v2-roadmap.md
Second-batch actual touches: docs/architecture.md; docs/mechanics-evidence.md;
  docs/project-state.md (current beta-hook pointer refinement)
Repair-pass touches: docs/task-notes/README.md; docs/task-notes/project-total-note.md;
  docs/task-notes/documentation-authority-consolidation-master-task-card.md
Task-note Markdown count / lines / bytes after DC2 repair: 41 / 9,323 / 496,729
Physical default-read set: 3 files / 1,424 lines / 78,790 bytes
Tier0 router alone: 83 lines / 4,872 bytes (historical tail removed)
Disposition coverage: 41 rows; missing 0; extra 0; class counts match section 22.1
Local Markdown links / broken: 45 / 0
git diff --check: passed; informational LF-to-CRLF checkout warnings only
Tracked changes: 8 Markdown files; untracked user files: 2 task cards
Non-doc changes / moves / deletes: None / None / None
Staged files / commit / push / tag / Workshop: None / No / No / No / unchanged
```

This checkpoint closed at DC2. Later execution state is recorded in section 23.

---

## 23. DC3 / DC4 / DC5 Checkpoint — 2026-07-21

- DC3 was explicitly approved as a minimal exception-only pass.
- Exactly six `SUPERSEDED_HANDOFF` files received a three-line historical/current-authority banner.
- DC3 commit `22a6e1a` was pushed to `origin/main`; no tag was moved.
- DC4 physical archive was evaluated and skipped: no file was moved or deleted.
- DC5 started with a clean, synchronized repository and found stale DC3 status wording in four
  current-authority files; those current pointers were corrected before validation was rerun.
- DC5 validation passed: 57 local Markdown links and 2 local asset links resolve with 0 broken.
- Two unresolved backticked `.md` names are intentional non-link historical filename records:
  `phase-10-multiplayer-research-frozen.md` and `phase-12a1-config-localization-language-color.md`.
- Product/technical identity, Workshop private/public boundary, verification classes, G5/G6/
  Forecast Engine states, default routing, and repository boundaries are consistent.
- Remote `main` resolves to `22a6e1a`; the published annotated consolidation tag still resolves
  to the DC0-DC2 authority commit `2eb6d57` and was not moved by minimal DC3.
- DC5 diff contains only the four current-authority Markdown status corrections; no source,
  test, script, manifest, artifact, move, delete, or staged change is present.

This checkpoint ended with DC5 complete. DC6 closure is recorded in section 24.

---

## 24. DC6 Documentation Closure — 2026-07-21

```text
Final status: Closed — DC0-DC6 complete; DC4 skipped
Before task-note Markdown count / lines / bytes: 41 / 9,159 / 490,023
After task-note Markdown count / lines / bytes: 41 / 9,379 / 500,389
Default-read set: docs/task-notes/README.md + docs/project-state.md;
  2 files / 418 lines / 37,327 bytes
Current authority map: Tier0 task router plus README.md, project-state.md,
  architecture.md, mechanics-evidence.md, build-environment.md, and v2-roadmap.md
Final task-note classification: CURRENT_AUTHORITY 1; ACTIVE_TASK 0;
  PROPOSED_TASK 1; HISTORICAL_RECORD 27; SUPERSEDED_HANDOFF 6; RAW_EVIDENCE 6
Historical evidence preserved: 39 records; all remain at original paths
Files moved: None
Files deleted: None
Broken links/assets: 0 / 0
Final committed delivery: 2eb6d57 (DC0-DC2 authority consolidation);
  22a6e1a (minimal DC3 handoff banners)
DC5/DC6 closure delivery: approved as a docs-only commit; the exact hash is the
  Git commit containing this closure record
Final tag: documentation-authority-consolidated -> 2eb6d57; not moved by DC3
Remote baseline before closure delivery: origin/main -> 22a6e1a;
  post-push origin/main must resolve to the closure commit
Workshop state: unchanged; private/subscription evidence only, not public release
Next approved task: None
Next candidate: Forecast Engine read-only AR0 revalidation; Proposed/Queued
```

Closure boundary:

- no source, test, script, manifest, artifact, install, game, or Workshop change;
- no physical archive, move, or delete;
- no active task was inferred from a Proposed task card;
- future commit, push, tag, Forecast Engine AR1, G6, Workshop, or multiplayer work
  requires separate user approval.
