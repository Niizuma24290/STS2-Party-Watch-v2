# 任务收口标准

状态：Active

适用范围：本仓库后续任务与 Gate

接入记录：Post-G6 C3D/C4 执行期间不要求中途改写；C4 完成后已按本标准接入任务索引并执行最终收口。

## 1. 目的

用最少的维护成本回答四个问题：

1. 做成了什么；
2. 现在处于什么状态；
3. 哪份文件是当前权威；
4. 仓库是否已有可追溯 checkpoint。

本标准不建立第二套进度系统，不要求重复整理历史，不要求每个小任务创建任务卡。

## 2. 核心规则

1. 一个任务只保留一个进度权威；多 Gate 任务通常是其 master task card。
2. 当前状态只写一次，放在权威任务卡顶部的 `Current Control`。
3. 每个 Gate 只追加本 Gate 的增量证据，不复制前序测试、hash 或结论。
4. `docs/task-notes/README.md` 只负责链接和路由，不复制任务状态细节。
5. `docs/project-state.md` 只记录当前仍成立的产品事实，不写 Gate 过程、待批准步骤或会过期的任务描述。
6. 历史记录保留原文；已关闭任务卡原则上不重写，只能追加明确标注的勘误。
7. 未形成 Git checkpoint 时，不得把仓库状态写成 `Closed`。
8. 不为了形式收口而补做任务范围外的安装、运行、发布、Workshop 或配置修改。

## 3. 任务规模

### 3.1 普通单 Session 任务

不单独创建 master task card。完成实现与验证后：

- 报告结果、测试、风险和未决项；
- 仅在产品事实发生变化时更新 `docs/project-state.md`；
- 在获批后形成 Git checkpoint。

### 3.2 多 Gate、高风险或外部状态任务

只使用一张 master task card。它必须包含：

- 顶部唯一的 `Current Control`；
- 各 Gate 的增量证据；
- 最终四项收口记录。

## 4. Current Control 模板

```text
State: C3D Complete / C4 Approval Pending
Last completed: C3D
Next: C4
Approved: No
Evidence: §25
Repository: Checkpoint Pending
```

更新 Gate 时直接替换这一个区块，不在其他文件重复维护同一状态。

## 5. Gate 证据模板

每个 Gate 通常控制在 5～10 行，只写增量：

```text
### C3D
- Result: Complete
- Changed: <本 Gate 的实际变更>
- Verified: <本次实际运行的检查>
- Preserved: <关键不变量或明确未触碰项>
- Risks / Pending: <剩余风险或下一批准边界>
- Evidence: <必要的路径、hash、日志或前序章节引用>
```

同一测试或证据已在前序 Gate 记录时，引用章节即可，不重复粘贴。

## 6. 最终收口模板

最终收口只保留四项：

```text
Result: <最终交付结果>
Current state: <当前可观察事实与剩余限制>
Authority: <已同步的权威文件>
Repository: <commit/checkpoint，或明确的 pending 原因>
```

外部动作若明确不在任务范围，可写为 `Out of Scope / Unchanged`；这不会阻止本地任务收口。

## 7. 状态词

- `In Progress`：仍在实施或验证。
- `Verification Complete / Closure Pending`：实现和验证完成，权威或仓库尚未收口。
- `Work Complete / Checkpoint Pending`：工作完成，但尚无获批的 Git checkpoint。
- `Closed`：结果已验证、当前事实正确、权威已同步、仓库已有可追溯 checkpoint。
- `Blocked`：存在明确外部阻塞，当前无法继续。

`commit` 是默认 checkpoint。`push`、`tag`、发布和 Workshop 更新只有在任务明确要求且单独获批时才属于 `Closed` 条件。

## 8. 文件职责

| 文件 | 只负责 | 不负责 |
|---|---|---|
| master task card | Gate、审批边界、增量证据、任务收口 | 全项目状态汇总 |
| `docs/task-notes/README.md` | 当前/排队/历史任务的链接路由 | 复制 Gate、测试、hash |
| `docs/project-state.md` | 当前仍成立的产品事实 | 下一任务、审批队列、过程日志 |
| historical docs / ledger | 不可变历史证据 | 当前状态权威 |

## 9. 并发 Session 规则

1. 不修改另一活跃 Session 正在维护的任务卡或权威文件。
2. 必须新增全局规则时，先独立落盘，再由最终 authority/closure Gate 接入索引。
3. 候选审计和并行报告只能作为输入；主线负责去重并写入唯一权威。
4. Post-G6 C3D/C4 执行期间不要求其改用本模板，也不并发改动其任务卡、`README.md` 或 `project-state.md`；该过渡期已随 C4 完成而结束。

## 10. 快速收口检查

- [ ] 结果和验证已记录
- [ ] `Current Control` 只存在于一个权威位置
- [ ] 产品事实仅在确有变化时更新
- [ ] 任务索引只增加或调整链接
- [ ] 未复制旧 Gate 的长证据
- [ ] 未改写历史原文
- [ ] Git checkpoint 状态表述真实
- [ ] 下一步需要新授权时已停止并请求批准

本文件已在 Post-G6 C4 完成后接入 `docs/task-notes/README.md`，未在 C3D/C4 执行期间制造并发修改冲突。
