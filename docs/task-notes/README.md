# v2 任务登记

`docs/task-notes/` 是 v2 的唯一任务登记文件夹。不要创建第二套平行的进度目录。

## 当前状态

Phase 1A 已完成。Phase 1B 已完成。Phase 5 已完成：攻击 Intent、Burn/DamageVar、第一批 EffectiveBlock 候选均已进入 `🛡 -N` 并完成 Steam 运行时回归验证。Phase 6 已完成：Beckon / Bad Luck / Regret 已进入 `♥ -N` 并完成 Steam 运行时联合验证。

## 当前唯一任务

Phase 7：接入 TungstenRod、BeatingRemnant 等会修正 HP Loss 结果的机制

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
| Phase 7 | 待做 | HP Loss 结果修正机制 | 接入 TungstenRod、BeatingRemnant 等会修正 HP Loss 结果的机制 | Phase 8 |
| Phase 8 | 待做 | 非 Block 承伤修正机制 | 补 DiamondDiademPower 等改变实际承伤、但不属于 Block 的伤害修正机制 | Phase 9 |
| Phase 9 | 待做 | 单人正式版收口 | 异常场景、回归测试、文档整理、清理临时诊断与 UI 打磨 | Phase 10 |
| Phase 10 | 冻结 | 多人研究 | 仅研究多人真实目标与原生 target-aware 伤害预览，证据不足前不做正式多人 HUD | 证据充分后再开启 |
