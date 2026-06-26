# v2 任务登记

`docs/task-notes/` 是 v2 的唯一任务登记文件夹。不要创建第二套平行的进度目录。

## 当前状态

Phase 0 已完成。Phase 1–4 已完成代码实现、restore/build/publish，运行时验证未完成。

## 当前唯一任务

Phase 1–4：单人攻击 HUD 基线运行时验证

## 禁止事项

- 不写任何 Mod 功能代码。
- 不创建 DLL、PDB、PCK、logs 或其他构建产物。
- 不修改游戏文件。
- 不把代码推测写成运行时验证事实。
- 不创建第二套平行的进度目录。
- 不开发正式多人伤害 HUD。

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
| Phase 1–4 | 进行中 | 单人攻击 HUD 基线 | Mod 可加载、可只读读取战斗、HUD 显示 `🛡 -N` | 运行时验证完成后进入 Phase 5 |
| Phase 5 | 未开始 | 攻击前确定性 Block | Frost、覆甲等已验证 Block 纳入 `🛡 -N` | Phase 6 |
| Phase 6 | 未开始 | Direct HP Loss | Beckon、Bad Luck、Regret 显示 `♥ -N` | Phase 7 |
| Phase 7 | 未开始 | 单人验证与收口 | 单人 HUD 规则、运行时验证、文档收口 | 后续机制补充 |
| Phase 8 | 冻结 | 多人研究 | 仅研究真实目标与原生预览，不做正式多人 HUD | 证据充分后再开启 |
