# v2 任务登记

`docs/task-notes/` 是 v2 的唯一任务登记文件夹。不要创建第二套平行的进度目录。

## 当前状态

Phase 1A 已完成。Phase 1B 已完成。Phase 5 已完成：攻击 Intent、Burn/DamageVar、第一批 EffectiveBlock 候选均已进入 `🛡 -N` 并完成 Steam 运行时回归验证。Phase 6 已完成：Beckon / Bad Luck / Regret 已进入 `♥ -N` 并完成 Steam 运行时联合验证。Phase 7 已按正常游戏范围收口：TungstenRod 与 BeatingRemnant 已接入，复数棍子控制台无效状态不作为正式行为依据。Phase 8 已完成 DiamondDiademPower 最小代码接入并通过用户 Steam 运行时验证。Phase 9 已完成代码、文档与用户 Steam 运行时收口；本轮追加水盆 + 惊涛、Constrict / Disintegration Power 自伤、IntangiblePower / 无实体 HP loss 结果修补均已验证通过。Phase 9A 已完成前端 UI 产品化代码接入：HUD 可见性策略、回合内显示快照、原生 Settings 屏设置面板、位置与颜色设置均已构建/发布/安装通过。Phase 9B 已完成普通 Poison 行动前存活预览代码接入、构建与 publish；安装因游戏目录 DLL 被占用失败，等待安装与 Steam 运行时验证。Workshop 工作区已准备在 ignored `work/` 下；本轮 Git 同步不执行上传，不公开发布。

## 当前唯一任务

关闭占用 mod DLL 的游戏进程后，完成 Phase 9B 安装与 Steam 运行时验证。

## 禁止事项

- 不提交 DLL、PDB、PCK、logs、publish 输出、NuGet 缓存或游戏目录文件。
- 不修改游戏文件。
- 不把代码确认写成运行时验证事实。
- 不创建第二套平行的进度目录。
- Phase 10 仍冻结，不开发正式多人伤害 HUD。

## 每次任务结束的登记规则

1. 更新对应 Phase 文件。
2. 更新 `docs/task-notes/README.md` 的状态表。
3. 更新 `docs/project-state.md` 的当前快照。
4. 明确写出下一步唯一任务。
5. 写入实际提交 hash。
6. 不把“代码推测”写成“运行时验证事实”。

## 总计划表

| 阶段 | 状态 | 任务 | 完成标准 | 下一步依赖 |
| --- | --- | --- | --- | --- |
| Phase 0 | 已完成 | 仓库初始化 | v2 新仓库、文档、远程 main 已建立 | 无 |
| Phase 1A | 已完成 | Mod 发现与加载验证 | Steam 启动、Mod 列表可见、Loaded 日志确认 | Phase 1B |
| Phase 1B | 已完成 | 单人攻击 HUD 运行时验证 | `🛡 -N` 在单人攻击 Intent 场景正确显示 | Phase 5 |
| Phase 5 | 已完成 | Blockable Incoming Damage 汇总 | 怪物攻击、手牌回合末 blockable DamageVar、第一批 EffectiveBlock 候选已纳入 `🛡 -N` 并完成回归验证 | Phase 6 |
| Phase 6A | 已完成 | Direct HP Loss 固定值 | Beckon、Bad Luck 显示 `♥ -N`，并完成 Steam 运行时矩阵 | Phase 6B |
| Phase 6B | 已完成 | Regret Direct HP Loss | Regret 按当前手牌总数进入 `♥ -N` | Phase 6C |
| Phase 6C | 已完成 | Direct HP Loss 联合运行时验证 | Beckon、Bad Luck、Regret 的 `♥ -N` 完成 Steam 联合验证 | Phase 7 |
| Phase 7 | 已完成 | HP Loss 结果修正机制 | TungstenRod、BeatingRemnant 已按正常游戏范围收口；复数棍子控制台无效状态排除 | Phase 8 |
| Phase 8 | 已完成 | 非 Block 承伤修正机制 | DiamondDiademPower 最小代码接入已构建/发布通过，并完成用户 Steam 运行时验证 | Phase 9 |
| Phase 9 | 已完成 | 单人正式版收口 | 代码、文档与用户 Steam 运行时回归均已收口 | Phase 9A |
| Phase 9A | 已完成，待 Workshop 私密测试 | 前端 UI、设置与显示生命周期 | HUD 可见性、冻结快照、设置面板、位置与颜色项已构建/发布/安装通过 | Workshop 私密上传测试 |
| Phase 9B | 代码已接入，待安装与运行时验证 | Poison 行动前存活预览 | 普通 Poison 确定击杀的 enemy instance 不再贡献当前 Attack Intent；build / publish 通过 | 关闭游戏后安装 / Steam 验证 |
| Workshop prep | 工作区已准备，未公开发布 | Steam Workshop private 上传准备 | ignored `work/` 中保留上传工作区；DLL、PDB、PCK、logs、封面、uploader、`mod_id.txt` 均不进入 Git | 准备封面、tags 与私密上传测试 |
| Phase 10 | 冻结 | 多人研究 | 仅研究多人真实目标与原生 target-aware 伤害预览，证据不足前不做正式多人 HUD | 证据充分后再开启 |
追加修补：`ConstrictPower` 与 `DisintegrationPower` 已按原生 blockable Power 自伤路径接入 `🛡` 预测，并已通过用户 Steam 运行时验证。

追加修补：`IntangiblePower` 已按每个已验证 direct HP loss 事件先变为 1、再进入 `TungstenRod` / `BeatingRemnant` 的顺序接入；blockable 伤害继续信任原生预览；用户 Steam 运行时验证已通过。

追加修补：`TungstenRod` / `BeatingRemnant` 共存时，Party Watch 预测固定先应用 `TungstenRod`，再应用 `BeatingRemnant`；等待私密上传测试后的订阅版运行时验证。

Workshop 准备收口：工作区位于 ignored `work/` 下；`workshop-private-rc-2026-07-01.md` 记录待确认的 uploader、工作区、封面、日志、Git hygiene 与后续私密上传测试步骤。
