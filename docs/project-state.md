# Project State

## 当前快照

- 任务登记文件夹：`docs/task-notes/`
- 当前唯一任务：Phase 1–4：单人攻击 HUD 基线运行时验证
- 当前分支：`main`
- 当前状态：Phase 0 已完成；Phase 1–4 已完成代码实现、restore/build/publish，运行时验证未完成；Phase 8 多人研究冻结。
- 约束：本仓库已有 Phase 1–4 Mod 源码；无已跟踪 DLL、PDB、PCK、logs、NuGet 缓存或构建产物。

## 阶段状态

| 阶段 | 状态 | 任务 | 完成标准 | 下一步依赖 |
| --- | --- | --- | --- | --- |
| Phase 0 | 已完成 | 仓库初始化 | v2 新仓库、文档、远程 main 已建立 | 无 |
| Phase 1–4 | 进行中 | 单人攻击 HUD 基线 | Mod 可加载、可只读读取战斗、HUD 显示 `🛡 -N` | 运行时验证完成后进入 Phase 5 |
| Phase 5 | 未开始 | 攻击前确定性 Block | Frost、覆甲等已验证 Block 纳入 `🛡 -N` | Phase 6 |
| Phase 6 | 未开始 | Direct HP Loss | Beckon、Bad Luck、Regret 显示 `♥ -N` | Phase 7 |
| Phase 7 | 未开始 | 单人验证与收口 | 单人 HUD 规则、运行时验证、文档收口 | 后续机制补充 |
| Phase 8 | 冻结 | 多人研究 | 仅研究真实目标与原生预览，不做正式多人 HUD | 证据充分后再开启 |

## 已运行的本地验证

- `C:\sts2\dotnet\dotnet.exe restore src/STS2PartyWatchCode/STS2PartyWatchCode.csproj` 成功。
- `C:\sts2\dotnet\dotnet.exe build src/STS2PartyWatchCode/STS2PartyWatchCode.csproj -c Release --no-restore` 成功，0 warning，0 error。
- `C:\sts2\dotnet\dotnet.exe publish src/STS2PartyWatchCode/STS2PartyWatchCode.csproj -c Release --no-build -o work/publish/STS2PartyWatchCode` 成功。
- 发布包曾被复制到本机 STS2 `mods/STS2PartyWatchCode` 目录用于验证；验证受阻后该临时安装目录已清理。
- 直接启动 `SlayTheSpire2.exe` 被 Steam 初始化错误阻止，未到达 Mod 加载或战斗阶段。

## 未完成运行时验证

- 游戏识别并加载 Mod。
- 日志中出现 `[STS2 Party Watch] Loaded`。
- 非战斗 HUD 隐藏。
- 单人战斗 HUD 显示、隐藏和刷新。
- 单段攻击、多段攻击、多个敌人累计、Block 变化、非攻击 Intent、OUT=0。

## 任务登记规则

- 每次任务结束必须更新对应 Phase 文件。
- 每次任务结束必须更新 `docs/task-notes/README.md` 的状态表。
- 每次任务结束必须更新本文件的当前快照。
- 每次任务结束必须明确下一步唯一任务。
- 每次任务结束必须写入实际提交 hash。
- 不把代码推测写成运行时验证事实。
