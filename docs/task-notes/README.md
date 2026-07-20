# v2 任务登记

`docs/task-notes/` 是 v2 的唯一任务登记文件夹。不要创建第二套平行进度目录。

当前玩家可见名称为 `Damage Forecast` / `伤害预测`。`Party Watch` 是旧玩家可见名称；历史记录、技术标识、路径和 `[STS2 Party Watch]` 诊断前缀均保留。

## 当前状态

Phase 1A 至 Phase 9A 已完成并有对应历史记录。Phase 9B 普通 Poison、设置入口修复、本机多人 HUD 和部分 Workshop 订阅版验证实际上属于 Workshop 上传完成后的后续补充，但历史文件名已沿用 `phase-9*`；这些文件名保留为证据，不再继续扩展新的 Phase 9 子阶段。Phase 10 视为 Workshop 上传 / 订阅测试里程碑。之后新增补充工作统一从 Phase 11 登记。Phase 11A v1.08 beta 快速兼容 smoke 和 Phase 11B stable DLL 初始化修复均已补记。最后补记中的原生毒血条、`AccelerantPower`、Exoskeleton / HardToKill、Slippery、敌方 Intangible、TestSubject、SewerClam 等 Poison 边界已汇总到本总记录和 `docs/project-state.md`。正式 `FormalMultiplayerHud`、队友顶栏预测和共享队伍 HUD 继续冻结。

## 当前唯一任务

Phase 12C-A G2 审计、AUD-0007 性能测量、Phase 12C-B G3 Surface Rename 和独立 G4 运行验证均已收口。Mod 列表、BaseLib 中英文标题与标签、设置重启持久化、单一加载身份和核心单人 HUD 均已 RuntimeVerified。当前下一道门是 G5；未经单独批准，不 commit、不 push、不更新 Workshop 或远程资料。正式多人 HUD 继续冻结。

## 禁止事项

- 不提交 DLL、PDB、PCK、logs、publish 输出、NuGet 缓存、Workshop uploader 文件、封面、`mod_id.txt`、`work/` 或游戏目录文件。
- 不修改游戏文件。
- 不把代码确认写成运行时验证事实。
- 不创建第二套平行进度目录。
- 不把本机多人 HUD 写成正式多人/队友/共享 HUD。

## 登记规则

1. 更新对应 Phase 或补记文件。
2. 更新本 README 的当前状态和总计划表。
3. 更新 `docs/project-state.md` 的当前快照。
4. 明确写出下一步唯一任务。
5. 写入实际提交 hash。
6. 区分 Implemented、Built、Installed、RuntimeVerified、DocumentedOnly。
7. 每次收口都必须补上“关键文件”和“解决问题的诀窍 / 成功模式”，方便后续同类问题复用。

## 笔记规划

| 层级 | 文件 | 用途 | 删除规则 |
| --- | --- | --- | --- |
| 总索引 | `README.md` | task notes 入口、当前状态、计划表、近期补记入口 | 不删 |
| 总笔记 | `project-total-note.md` | 汇总最近所有散信息、历史遗漏、当前支持范围与限制 | 不删 |
| 项目总状态 | `../project-state.md` | 面向整个项目的当前事实总览，不只服务 task notes | 不删 |
| Phase 历史证据 | `phase-0-*` 到 `phase-9*` | 记录每阶段实际改动、验证、限制、commit hash；其中部分 `phase-9*` 是历史误编号的后续补充 | 不因总笔记已摘要而删除 |
| Workshop 里程碑 | `workshop-private-rc-*` | Phase 10：Workshop 上传 / 订阅测试里程碑和发布工作区证据 | 不删 |
| Phase 11 补充 | `phase-11-supplements.md` / `phase-11-<topic>.md` | Workshop 上传完成后的新增补充、补验证、Bug fix 和文档同步 | 后续新增工作优先写这里 |
| 专题补记 | `modding-settings-entry-*`、`workshop-private-rc-*` | 记录跨 Phase 的具体修复 / 发布准备证据 | 不删，除非未来有同主题正式替代笔记并在此索引标明 |

已清理的重复 / 冲突文件：旧的 `phase-9-documentation-reconciliation.md` 已重命名为 `project-total-note.md`；旧的 `phase-10-multiplayer-research-frozen.md` 只有冻结政策且与 Phase 10 Workshop 里程碑冲突，已删除，冻结规则改由本索引、路线图和 Phase 11 说明承载。

## 总计划表

| 阶段 | 状态 | 任务 | 完成标准 | 下一步依赖 |
| --- | --- | --- | --- | --- |
| Phase 0 | 已完成 | 仓库初始化 | v2 新仓库、文档、远程 main 已建立 | 无 |
| Phase 1A | 已完成 | Mod 发现与加载验证 | Steam 启动、Mod 列表可见、Loaded 日志确认 | Phase 1B |
| Phase 1B | 已完成 | 单人攻击 HUD 运行时验证 | `🛡 -N` 在单人攻击 Intent 场景正确显示 | Phase 5 |
| Phase 5 | 已完成 | Blockable Incoming Damage 汇总 | 怪物攻击、手牌回合末 blockable DamageVar、第一批 EffectiveBlock 候选纳入 HUD 并回归验证 | Phase 6 |
| Phase 6A | 已完成 | Direct HP Loss 固定值 | Beckon、Bad Luck 显示 `♥ -N` 并完成 Steam 矩阵 | Phase 6B |
| Phase 6B | 已完成 | Regret Direct HP Loss | Regret 按当前手牌总数进入 `♥ -N` | Phase 6C |
| Phase 6C | 已完成 | Direct HP Loss 联合验证 | Beckon、Bad Luck、Regret 的 `♥ -N` 完成 Steam 联合验证 | Phase 7 |
| Phase 7 | 已完成，条件支持 | HP Loss 结果修正机制 | TungstenRod、BeatingRemnant 按正常游戏范围收口，复数棍子控制台无效状态排除 | Phase 8 |
| Phase 8 | 已完成，条件支持 | 非 Block 承伤修正机制 | DiamondDiademPower 最小代码接入并完成用户 Steam 验证 | Phase 9 |
| Phase 9 | 已完成 | 单人正式版收口 | 默认总 `-N`、高级 `🛡/♥`、文档与回归收口 | Phase 9A |
| Phase 9A | 已完成 | HUD lifecycle、隐藏策略、设置与显示生命周期 | HUD 可见性、冻结快照、设置面板、位置与颜色项完成 | 设置入口修复 / Workshop prep |
| 设置入口修复 | 已完成 | 不占用 native Settings / Modding 主入口 | 设置入口只在 Party Watch 自身 Mod info panel 中显示 | Steam 设置入口验证 |
| Phase 9B | 已实现，历史误编号；后续归 Phase 11 | Poison 行动前存活预览 | 普通 Poison 确定击杀的 enemy instance 不再贡献当前 Attack Intent | Phase 11 专项 Steam 验证 |
| 本机多人 HUD | 已完成，条件支持 | 多人战斗中显示本机 local HUD | Workshop 订阅版已看到本机 `-6` 在本机角色血条下方 | 正式多人 HUD 仍冻结 |
| Phase 10 | 已完成 / 私密订阅里程碑 | Steam Workshop upload / subscription test | item `3755598583`、private visibility、订阅版本机 HUD 验证、封面和 preview 上传记录 | 不得误写为 public release |
| Phase 11 | 当前后续阶段 | 补充与维护 | 新增补丁、补验证和文档同步从 Phase 11 登记；本机血条中线辅助线和 HUD 文本中心诊断线已实现，当前版本共享同一目标中心，但尚未运行时验证 | `phase-11-supplements.md` |
| Phase 11A | 已完成 / 部分验证 | STS2 v1.08 beta quick compatibility smoke | Built against v1.08、load smoke、core HUD、damage update、HUD lifecycle、covering-screen hide 已验证；不声明完整 v1.08 兼容 | Phase 11B / Phase 12 |
| Phase 11B | 已完成 / stable load 修复 | Stable branch DLL initialization investigation | stable v0.107.1 build 通过，local artifact 安装，用户确认 stable 启动成功；settings/HUD 未在本阶段重验 | Phase 12B runtime verification |
| Phase 11E | 已收口 / BuildVerified / RuntimeUnverified | stable + beta 双目标构建基线 | v0.107.1 stable 与 v0.109.0 beta 快照可由无参数脚本重复构建；两分支匹配安装、加载和 HUD smoke 均未验证 | 后续独立 runtime smoke |
| Phase 11C | 已收口 / 条件 RuntimeVerified | Poison 行动前存活预测专项 | Exoskeleton / `HardToKillPower`、墨宝 / `SlipperyPower`、`HardenedShellPower` / SewerClam 已验证；敌方 `IntangiblePower` 窄规则已恢复，TestSubject phase 3 exact-lethal 边界已由用户截图和运行时确认 | 完整矩阵扩展须另立任务；当前不重开 |
| Phase 12C-B G3 | 已收口 / StaticVerified / BuildVerified / ContractVerified / RuntimeUnverified | 玩家可见品牌改为 `Damage Forecast` / `伤害预测` | 技术身份与历史证据保留；21 项契约测试和 stable/beta 双目标构建通过 | G4 安装与运行时验证 |
| Phase 12C-B G4 | 已收口 / RuntimeVerified | 安装并验证 Surface Rename 产物 | `Damage Forecast` 单一列表项、BaseLib 中英文首开/切换、完整重启持久化、加载身份与核心单人 HUD 均通过；最终 28 项契约和双目标构建通过 | G5 需单独批准 |

## Phase 9B Poison 总记录

已实现 / 有代码证据：

- `EnemyPreActionSurvivalPreview` / `PoisonTickPreview` 已接入 `LocalIncomingDamageReader`。
- 普通敌人会按 enemy instance 独立预览，确定会在行动前死于 Poison 时，该实例当前 `AttackIntent` 不进入最终 `-N`。
- 敌人身份优先使用 `Creature.CombatId`，否则使用 native object reference + 当前快照 index；不使用显示名称做 key。
- 读取当前 `PoisonPower.Amount`、enemy HP、当前 Intent contribution、opponent `AccelerantPower.Amount`。
- `AccelerantPower` 只读已生效层数；不重放 `Accelerant` 卡牌，不修改命令队列。

原生毒血条补记：

- `PoisonPower.CalculateTotalDamageNextTurn()` 是 native poison foreground 的模型层总量入口。
- 触发次数为 `min(PoisonPower.Amount, 1 + alive opponents AccelerantPower amount sum)`。
- 每段 poison preview 会调用 `Hook.ModifyDamage(...)`，因此可作为后续 HardToKill / damage cap 小步接入的证据候选。
- `NHealthBar.IsPoisonLethal(...)` 和 `_poisonForeground` 只是 UI 预览，不证明 enemy Intent 是否取消。

当前未验证 / 不支持：

- 普通 Poison kill / no-kill、多敌人、同名敌人的 Steam 专项矩阵尚未完整补齐。
- `HardToKillPower` / Exoskeleton：Phase 11C 已接入 native `PoisonPower.CalculateTotalDamageNextTurn()`，并由用户 Steam 验证在遗物和诅咒影响下不再隐藏 `-N`。
- `SlipperyPower`：Phase 11C 已实现逐 tick / 逐层滑溜预览；每层滑溜将一个 Poison tick 的 HP loss 压到 1 并在预览中消耗。墨宝场景已由用户 Steam 验证。
- `SewerClam` / `HardenedShellPower`：Phase 11C 已条件接入；只读 `DisplayAmount` 作为剩余 HP-loss 预算，读不到或剩余预算小于当前 HP 时保留基础 Intent，否则将 Poison HP loss 按剩余预算封顶后判断是否移除当前 Intent。用户 Steam 验证已成功。
- 敌方 `IntangiblePower`：Phase 11C 窄规则已于 2026-07-18 恢复；没有 HardToKill / Slippery / HardenedShell 组合时，每段 Poison 按最多 1 HP loss 处理，只有触发段数覆盖当前 HP 才移除当前 Intent。TestSubject phase 3 exact-lethal 等号边界已 RuntimeVerified。
- `TestSubject` / `AdaptablePower`：第三阶段 Intangible exact-lethal 场景已验证；`NemesisPower`、`ToughEgg` / `HatchPower` 虽已放开 supported Poison 预览，但仍没有各自独立的运行时截图。

## 近期补记索引

- `project-total-note.md`: 项目总笔记；本次全量文档同步与历史补记。
- `modding-settings-entry-2026-07-02.md`: Party Watch 设置入口从 native Settings/Modding 主入口移到自身 Mod info panel 的修复记录。
- `phase-9b-poison-pre-action-survival.md`: Poison 行动前存活预览与原生毒血条补充证据。
- `phase-11-supplements.md`: Workshop 上传完成后的补充与维护阶段入口。
- `phase-11a-v108-quick-compatibility.md`: v0.108.0 beta 快速兼容 smoke，记录 beta API retarget、HUD 刷新和覆盖界面隐藏修复。
- `phase-11b-stable-dll-initialization.md`: v0.107.1 stable DLL 初始化修复，记录 stable/beta hook 与 `ModifyDamage` 兼容诀窍和关键文件。
- `phase-11c-poison-runtime-verification-safe-expansion.md`: Poison 专项收口；记录 Exoskeleton / `HardToKillPower`、墨宝 / `SlipperyPower`、HardenedShell / SewerClam、敌方 Intangible 条件接入、安装 hash 和 TestSubject phase-3 exact-lethal 运行时验证。
- `phase-11d-diamond-diadem-mechanism-compatibility.md`: Diamond Diadem 机制能力路由；保留旧出牌计数减伤与旧惊涛修正，接入 v0.109 首回合 Block + Blur 原生路径，并保证未知机制不隐藏整个 HUD。
- `phase-11e-dual-version-build-baseline.md`: 双版本构建基线；新增仓库外参考程序集快照脚本、显式 `Sts2ReferenceRoot`、stable/beta 独立输出构建脚本，并用 v0.107.1 stable 与 v0.109 beta 快照跑通双目标构建。
- `session-handoff-dual-version-compatibility-2026-07-17.md`: 王冠兼容完成后的 Session 交接；记录当前脏工作区、安装哈希、双版本适配边界和下一步唯一构建基础设施任务。
- `session-handoff-poison-testsubject-intangible-2026-07-18.md`: Poison × 实验体第三阶段 Intangible 专项交接与收口；记录上次误判、回滚历史、窄算法恢复、安装 hash 和代表性 exact-lethal 运行时证据。
- `session-handoff-dual-target-build-baseline-2026-07-18.md`: 双版本构建基线后的 Session 交接；记录 beta 快照、构建脚本、验证结果和下一步 stable 快照任务。
- `session-handoff-dual-target-build-complete-2026-07-18.md`: stable 快照捕获和 stable+beta 双目标构建通过后的 Session 交接。
- `workshop-private-rc-2026-07-01.md`: Workshop workspace、private/subscription 测试和多人本机 HUD 记录。
## Current Diagnostic Update

Phase 11 local HUD alignment used a temporary `[STS2 Party Watch][HUD Align]` runtime coordinate log in commit `0bf9a7e9ee597a1eb756b3432dadea4c7ee7734e`.

Status boundary: the main `-N` alignment was RuntimeVerified by the temporary runtime coordinate log before the debug instrumentation was removed. The debug-guide-removal build is now RuntimeVerified for local smoke: user confirmed the HUD view is OK, and latest log inspection found no temporary guide lines or `[HUD Align]` log noise.

Captured runtime coordinates showed the mismatch source: `MathF.Max(0f, position.Y)` clamped the main HUD label from desired local Y `-9` to `0`, producing `main.deltaY=9` and `guide.deltaY=9`. Commit `e398089b92f51f41bb5277263f4b0c0399dc7822` removes that local Y clamp; post-fix logs later confirmed `main.deltaY=0` and `guide.deltaY=0`.

Post-fix runtime logs showed `main.deltaY=0` and `guide.deltaY=0`. Commit `370d631b7076b174b1a9d42b353aadcdb97ff202` moves the advanced detail HUD to the right of `-N` and keeps it on the same forecast line.

Commit `070774b70ef07a5ead50f3e82ad60f1a6a3c6c0f` removes the temporary guide lines, `[HUD Align]` log, and `PartyWatchHudDebugGuide` helper.

Historical next-task note: this Phase 11 supplement pointer is superseded by the current Phase 12C-B G3 task at the top of this file.

## Phase 12A / 12B Update - 2026-07-03

Phase 12A is complete at the BaseLib automatic config baseline level:

- `docs/task-notes/phase-12a-completion-handoff-2026-07-03.md`
- `docs/task-notes/phase-12a-runtime-verification-2026-07-03.md`

Phase 12B was the next config polish task at the time of this 2026-07-03 update:

- `docs/task-notes/phase-12b-config-localization-language-color.md`

Phase 12B keeps BaseLib's automatic layout. Do not implement the hand-drawn text/table mockup as a manual layout, because it can wrap and misalign. If older notes describe Phase 12B as a fully custom right-side UI, that custom-UI idea is postponed behind this BaseLib-native localization/language/color task.

## Phase 13A Update - 2026-07-16

Phase 13A is implemented, built, installed, and settings-page smoke verified. The full combat HUD matrix is not fully runtime verified.

- Task note: `phase-13a-damage-display.md`
- Default behavior remains `ExpectedHpLossOnly`, showing only existing `-N`.
- New optional incoming damage `N` can be shown alone or together with `-N`.
- Both mode supports left/right placement for `N` without moving the existing `-N` anchor when `-N` is visible.
- BaseLib config uses short labels for the incoming calculation switches: current Block, Power Block, relic Block, Power damage reduction, and relic damage reduction.
- Build passed with `C:\sts2\dotnet\dotnet.exe build .\src\STS2PartyWatchCode\STS2PartyWatchCode.csproj -c Release --no-restore`, 0 warnings, 0 errors.
- User confirmed the installed settings page / settings controls are OK.
- Next task: Phase 13B health-bar difference display remains separate; optional deeper Phase 13A combat HUD matrix can be run if needed.
