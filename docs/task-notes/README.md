# Documentation Start Here

`docs/task-notes/` 是 Damage Forecast 的唯一任务登记和历史证据目录。
不要创建第二套平行进度目录。

当前玩家可见名称为 `Damage Forecast` / `伤害预测`。`Party Watch` 是旧玩家可见名称；历史记录、技术标识、路径和 `[STS2 Party Watch]` 诊断前缀均保留。

## 当前状态和唯一任务

- Phase 12C G0-G5 已收口；仓库、运行时验证和 tags 以
  [`docs/project-state.md`](../project-state.md) 及对应 Phase 12C closure note 为准。
- Documentation Authority Consolidation 已完成 DC0-DC6 并正式关闭；DC3 采用最小范围，
  DC4 经评估后跳过。关闭记录见
  [`documentation-authority-consolidation-master-task-card.md`](documentation-authority-consolidation-master-task-card.md)。
- 当前没有已批准的 active task。
- [`forecast-engine-architecture-stabilization-master-task-card.md`](forecast-engine-architecture-stabilization-master-task-card.md) 仍是
  `Proposed / Queued`，没有获得 AR1 或代码实施授权。
- G6 Full Technical Identity Migration、Workshop 和正式多人 HUD 均未获本任务授权。

## 默认阅读顺序

普通新 Session 默认只读：

1. 本文件；
2. [`docs/project-state.md`](../project-state.md)。

仅当未来存在已批准任务时，再读取该任务卡。

然后按任务主题读取对应 Tier 1 或精确历史证据。不要默认加载全部 Phase、audit 或 handoff note。

## Authority map

| 主题 | 当前唯一 authority |
| --- | --- |
| 玩家产品、安装、功能边界 | [`README.md`](../../README.md) |
| 当前实现、验证等级、限制、release/repository 状态 | [`docs/project-state.md`](../project-state.md) |
| 当前生产架构 | [`docs/architecture.md`](../architecture.md) |
| 当前机制支持、条件支持、未知与证据矩阵 | [`docs/mechanics-evidence.md`](../mechanics-evidence.md) |
| 当前构建、依赖、stable/beta 和验证命令 | [`docs/build-environment.md`](../build-environment.md) |
| 已完成里程碑短摘要与未来方向 | [`docs/v2-roadmap.md`](../v2-roadmap.md) |
| 当前任务、状态词和历史证据路由 | 本文件 |

其他文档可以提供历史证据，但不得竞争这些 current-state 职责。

## 状态词

- `Proposed / Not Started`：尚未批准实施。
- `Approved / In Progress`：仅批准记录的当前 Gate 和文件范围。
- `Blocked`：达到任务卡规定的阻塞条件。
- `Closed`：已完成并通过对应验证。
- `Historical / Superseded`：保留当时事实，不是当前 next-step authority。

## 按主题读取

| 任务 | 追加读取 |
| --- | --- |
| 预测机制 | [`docs/mechanics-evidence.md`](../mechanics-evidence.md) + 精确 Phase note |
| 架构 | [`docs/architecture.md`](../architecture.md) + [Forecast Engine task card](forecast-engine-architecture-stabilization-master-task-card.md)（仅在批准后） |
| 构建/兼容 | [`docs/build-environment.md`](../build-environment.md) + 对应 stable/beta note |
| 产品/玩家文案 | [`README.md`](../../README.md) + current rename record |
| 路线和排序 | [`docs/v2-roadmap.md`](../v2-roadmap.md) |
| 审计复核 | [`phase-12c-audit/`](phase-12c-audit/) 下精确 ledger |
| Workshop | [`workshop-private-rc-2026-07-01.md`](workshop-private-rc-2026-07-01.md) + [`docs/project-state.md`](../project-state.md) |
| G6 | [`name-migration-inventory.md`](phase-12c-audit/name-migration-inventory.md) + 未来获批的 G6 task card |

## Task-note classification

- `CURRENT_AUTHORITY`：本文件。
- `ACTIVE_TASK`：无。
- `PROPOSED_TASK`：[`forecast-engine-architecture-stabilization-master-task-card.md`](forecast-engine-architecture-stabilization-master-task-card.md)，保持 queued，尚未获得 AR1。
- `SUPERSEDED_HANDOFF`：所有 `*handoff*` 文件；保留 hash、工作区和当时 next-step。
- `RAW_EVIDENCE`：`phase-12c-aud-0007-*` 和 `phase-12c-audit/*`。
- `HISTORICAL_RECORD`：已关闭的 Documentation Authority Consolidation task card、`project-total-note.md` 及其余 completed Phase、runtime、Workshop 和 compatibility note。

完整 41 文件逐项 disposition 见[已关闭的 consolidation task card](documentation-authority-consolidation-master-task-card.md)。
所有历史 note 默认不读、不移动、不删除，正文中的旧 next-step 只代表其原日期。

## 禁止事项

- 不提交 DLL、PDB、PCK、logs、publish 输出、NuGet 缓存、Workshop uploader 文件、封面、`mod_id.txt`、`work/` 或游戏目录文件。
- 不修改游戏文件。
- 不把代码确认写成运行时验证事实。
- 不创建第二套平行进度目录。
- 不把本机多人 HUD 写成正式多人/队友/共享 HUD。
