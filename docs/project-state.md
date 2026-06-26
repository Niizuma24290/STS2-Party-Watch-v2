# Project State

## 当前快照

- 任务登记文件夹：`docs/task-notes/`
- 当前唯一任务：Phase 1A：Steam 启动后的 Mod 发现与加载验证
- 当前分支：`main`
- 当前状态：Phase 1 的工程前置已补齐；不在本任务启动游戏；Mod 发现、Loaded 日志和 HUD 运行时验证留到 Phase 1A。
- 约束：不提交 DLL、PDB、PCK、logs、publish 输出、NuGet 缓存或游戏目录文件。

## 已通过工程配置确认

- 目标框架为 `net9.0`。
- 程序集名为 `sts2-party-watch-v2`。
- metadata 文件为 `src/STS2PartyWatchCode/sts2-party-watch-v2.json`。
- metadata `id`、DLL 文件名、安装目录名统一为 `sts2-party-watch-v2`。
- metadata 使用当前模板和 BaseLib 示例一致的 snake_case 字段：`has_pck`、`has_dll`、`affects_gameplay`。
- `has_dll=true`，`has_pck=false`，当前不发布 PCK。
- `GenerateDependencyFile=false`，publish 输出不应包含 `.deps.json`。
- STS2、GodotSharp、Harmony 依赖从游戏运行目录引用，项目不复制这些 DLL。
- 入口类为 `STS2PartyWatch.MainFile`，入口声明为 `[ModInitializer(nameof(Initialize))]`。
- 初始化时输出 `[STS2 Party Watch] Loaded`。
- `scripts/Install-LocalMod.ps1` 固化了 publish 输出检查和 local mods 安装层级。

## 仅依据 ModLoader 规则确认

- Windows local mods 目录应位于游戏安装目录下的 `mods`。
- v2 预期安装目录：

```text
C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\sts2-party-watch-v2
```

- v2 预期安装树：

```text
mods/
└─ sts2-party-watch-v2/
   ├─ sts2-party-watch-v2.json
   └─ sts2-party-watch-v2.dll
```

- 不应存在 `mods/sts2-party-watch-v2/publish/...` 这类多嵌套一层的结构。

## 尚未经过游戏运行时验证

- 从 Steam 启动游戏。
- 游戏内 Mod 列表显示 `STS2 Party Watch v2`。
- 日志出现 `[STS2 Party Watch] Loaded`。
- Mod 未导致启动崩溃。
- 非战斗 HUD 隐藏。
- 单人战斗 HUD 显示、刷新、定位与 `🛡 -N` 预测行为。

## 阶段状态

| 阶段 | 状态 | 任务 | 完成标准 | 下一步依赖 |
| --- | --- | --- | --- | --- |
| Phase 0 | 已完成 | 仓库初始化 | v2 新仓库、文档、远程 main 已建立 | 无 |
| Phase 1–4 | 进行中 | 单人攻击 HUD 基线 | Mod 可加载、可只读读取战斗、HUD 显示 `🛡 -N` | Phase 1A 运行时加载验证 |
| Phase 5 | 未开始 | 攻击前确定性 Block | Frost、覆甲等已验证 Block 纳入 `🛡 -N` | Phase 6 |
| Phase 6 | 未开始 | Direct HP Loss | Beckon、Bad Luck、Regret 显示 `♥ -N` | Phase 7 |
| Phase 7 | 未开始 | 单人验证与收口 | 单人 HUD 规则、运行时验证、文档收口 | 后续机制补充 |
| Phase 8 | 冻结 | 多人研究 | 仅研究真实目标与原生预览，不做正式多人 HUD | 证据充分后再开启 |

## 任务登记规则

- 每次任务结束必须更新对应 Phase 文件。
- 每次任务结束必须更新 `docs/task-notes/README.md` 的状态表。
- 每次任务结束必须更新本文档的当前快照。
- 每次任务结束必须明确下一步唯一任务。
- 每次任务结束必须写入实际提交 hash。
- 不把代码推测写成运行时验证事实。
