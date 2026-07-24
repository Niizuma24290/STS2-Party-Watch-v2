# Post-G6 全面命名统一主任务卡

## Current Control

State: `Work Complete / Checkpoint Pending`
Last completed: `C4`
Next: `Git checkpoint`
Approved: `Yes — commit / push / checkpoint tag`
Evidence: `§26`
Repository: `Checkpoint Pending`

任务性质：独立于 G6 的 Post-G6 命名统一工程。

---

## 0. Session handoff 摘要

### 0.1 当前结论

本任务可行，但不能作为一次普通的全文字符串替换执行。必须把普通 C# 标识、BaseLib persistence identity、Godot runtime node、安装升级与历史证据分层处理。

已批准的总体路线（2026-07-22 用户确认）：

> 直接发布一个主体全面改名的 Damage Forecast 版本，同时在最新版中保留一个边界清晰、容量充足、幂等且可退休的 compatibility migration subsystem。Workshop 用户无需先运行中间版本；无论从哪个受支持的旧版本直接跳到最新版，新版都负责迁移旧配置。主体代码、玩家可见内容、新配置和 Godot nodes 不再使用 PartyWatch。

本路线不追求活动仓库内所有旧字符串的绝对物理 0。允许旧名称存在于：

- `src/DamageForecast/Compatibility/**` 下受 contract 管理的 compatibility migration subsystem；
- installer/rollback 的精确 legacy identity 数据；
- migration contract/negative fixtures；
- historical evidence。

除上述边界外，普通生产代码和玩家可见面必须为 0。

推荐执行顺序：

1. C1：普通类名、文件名、内部符号、MSBuild 开关和 Godot node 名的编译感知改名；
2. C2：18 项配置迁移、新 BaseLib config identity、validator、marker 和 rollback 支持；
3. C3：真实安装、完整重启、设置写入、rollback、重新升级及获批清理；
4. C4：active-scope 零旧名护栏、current authority 更新与最终 closure。

Workshop upload 仍需单独批准；本路线只确定未来上传内容必须具备直接旧版→最新版迁移能力，不构成当前 Workshop 修改授权。

### 0.2 累计 Gate 结果

- C0 已完成只读 inventory 和方案设计；
- C1 已于 2026-07-22 获用户明确批准并完成；
- stable v0.107.1 与 beta v0.109.0 在 C3B-Prep 后各 216/216 contracts 通过，双目标 Release build、publish exact-two-files 和 matching-artifact guard 通过；
- C2 已于 2026-07-22 获用户明确批准并完成；当前版本/manifest 为 `v0.3.0`；
- C3A 只读 Plan 与 C3B-Prep 已于 2026-07-22 完成；同 identity upgrade/rollback/re-upgrade tooling 已在临时 fixture 验证；
- C3B 真实安装与首次 runtime/migration 已于 2026-07-22 完成并核验；active 为 v0.3.0，v0.2.0 恢复备份、install ledger、新配置、migration backup/marker 和 fresh log 均有证据；当前扩大后的 HUD 行为缺陷另见第 22.6 节；
- C3C stable runtime 已于 2026-07-22 完成：设置写入/完整重启、v0.3→v0.2 reverse-sync rollback、旧版读取、v0.2→v0.3 re-upgrade 和最终完整重启均通过；当前 active 为唯一 v0.3.0；
- C3BR 已于 2026-07-24 完成：beta 216/216、双目标 matching publish、v0.109.0 one-load/one-page/one-HUD/config continuity smoke 均通过，随后已切回 stable v0.107.1 且未启动；
- C3D 已于 2026-07-24 完成：7 个精确目标已送入 Windows Recycle Bin，active v0.3/new cfg/completed marker/Workshop staging/C4 artifacts 的删除后复核均通过；
- C4 已于 2026-07-24 完成：stable/beta 各 226/226 contracts、双目标 Release build、matching exact-two publish、active-scope guard、compatibility inventory、current authority 和外部状态复核均通过；
- 本任务卡是给下一 Session 的完整工作依据；
- 不得把 C2 批准扩张为 C3/C4 的隐式批准；
- 每个 Gate 仍须单独获得用户批准。

### 0.3 C0 后已确认的设计决策

| ID | 决策 | 状态 |
|---|---|---|
| C-D01 | 采用“直接全面改名 + 隔离 migration shim”路线，不要求用户先运行中间版本 | Approved 2026-07-22 |
| C-D02 | 新 config/type/key 全部使用 DamageForecast identity | Approved 2026-07-22 |
| C-D03 | `EnablePartyWatchHud` 显式迁为 `EnableDamageForecastHud`；保持其值和语义 | Approved 2026-07-22 |
| C-D04 | 主体代码和玩家可见面旧名为 0；旧 literal 仅允许存在于 shim/rollback/tests/history | Approved 2026-07-22 |
| C-D05 | 最新 Workshop artifact 必须支持旧版直接跳到最新版；Steam 中间版本不可作为迁移前置 | Approved 2026-07-22 |
| C-D06 | Workshop upload、Git 操作和真实迁移仍需各自 Gate 的单独批准 | Standing restriction |
| C-D07 | compatibility 可以容纳版本化 schema、容错、recovery 和未来新增设置，但不得向主体代码扩散 | Approved 2026-07-22 |
| C-D08 | 所有官方版本正常产生的受支持配置必须 100% 精确迁移，否则禁止发布；损坏/未知输入必须保全原件且不得伪称成功 | Approved 2026-07-22 |
| C-D09 | compatibility subsystem 设计为未来可整体删除；按月数不能自动证明可退休，删除前必须明确接受旧版直升支持终止 | Approved 2026-07-22 |

---

## 1. 前置状态：G6 已正式关闭

权威文件：

`docs/task-notes/full-technical-identity-migration-master-task-card.md`

已确认：

- line 3：`Closed / G6-0 through G6-7 Complete`；
- line 632：`Final status: Closed / G6-0 through G6-7 Complete`；
- line 1063：`Final status: G6 Closed`。

G6 已完成并验证：

- active Mod ID：`damage-forecast`；
- active manifest/DLL/install directory：`damage-forecast`；
- active assembly/root namespace：`DamageForecast`；
- BaseLib registration key：`damage-forecast`；
- stable/beta build、publish、runtime 和 rollback 证据已记录；
- G6-7 已完成获批备份清理；
- Workshop 在 G6 中未修改；
- 原先仅记录 Regret/Bad Luck 的 HUD 隐藏问题；2026-07-22 v0.3.0 用户实测已将当前观察范围扩大为所有会造成伤害的状态牌/诅咒牌，见第 22.6 节。该观察与成功的 identity/config migration 分层记录，尚未证明因果或完整根因。

本任务不得重开、重写或否定 G6 closure。Post-G6 变更必须通过新的 C 系列 Gate 记录。

---

## 2. 总目标

评估并实施一次独立于 G6 的全面命名统一，使当前有效工程尽量只使用：

- 玩家名称：`Damage Forecast`
- 技术 slug：`damage-forecast`
- C# 标识：`DamageForecast`

重点包括：

1. 普通 `PartyWatch*` 类名、文件名、内部符号全面改名；
2. `STS2PartyWatch.cfg` 迁移为 `DamageForecast.cfg`；
3. 配置类型改为 `DamageForecast.Settings.DamageForecastBaseLibConfig`；
4. Godot HUD node 名改为 `DamageForecast*`；
5. 清理旧安装目录、旧配置和获批备份；
6. 建立 active-scope 旧名称禁止护栏；
7. 保留所有历史 evidence 原文；
8. Workshop 未经单独批准保持不变。

---

## 3. 全局禁止项

除非相应 Gate 的用户批准明确授权，否则不得：

- 修改生产 Mod ID、manifest stem、assembly、active namespace、安装目录；
- 安装或启动游戏；
- 修改本机配置；
- 移动、删除或覆盖旧配置、旧安装目录或备份；
- 修改 Workshop item、订阅内容、上传 staging 或 `mod_id.txt`；
- stage、commit、push、tag；
- 批量改写历史任务记录；
- 使用 `git reset --hard`、`git checkout --` 或覆盖式恢复；
- 诊断或修复 damage-dealing Status/Curse card 导致完整 HUD 隐藏的问题；
- 将派生 `bin/obj/work` 命中误判成活动源码；
- 在 old/new 配置内容冲突时自动覆盖任一文件。

所有真实安装状态切换必须：

- 在游戏完全关闭时执行；
- 有精确 transaction ID；
- 绑定 staging、安装、备份和配置 SHA256；
- 保持可恢复；
- 由用户负责启动游戏和游戏内验证。

---

## 4. 必须保持的行为

- 玩家名称与功能边界不变；
- stable/beta 行为不变；
- Mod 仍只加载一次；
- BaseLib 中只出现一个 Damage Forecast 配置页；
- HUD 每个 owner/HealthBar 只存在一组节点；
- 18 项配置的数量、语义、类型、默认值、顺序、枚举和值全部保持；
- 已批准唯一 schema key rename：`EnablePartyWatchHud` → `EnableDamageForecastHud`；
- 当前非默认配置值迁移前后保持；
- 颜色四分量保持；
- rollback 有明确且实测的路径；
- 历史 evidence 中旧名称保留原文；
- Workshop 未经单独批准不变；
- 未来 Workshop 最新版必须支持用户从受支持旧版本直接跳过中间版本升级；
- 已知 direct HP-loss HUD 缺陷继续独立记录，不得夹带修复。

---

## 5. C0 只读现场基线

核对日期：2026-07-22，Asia/Shanghai。

### 5.1 游戏与安装

- 游戏进程：0；
- 游戏目录：`C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2`；
- `mods` 中仅有 `damage-forecast`；
- `mods\sts2-party-watch-v2`：不存在；
- `.damage-forecast-backups`：不存在；
- Steam Workshop 本地订阅路径 `C:\Program Files (x86)\Steam\steamapps\workshop\content\2868840\3755598583`：不存在。

当前安装文件：

| 文件 | bytes | SHA256 |
|---|---:|---|
| `damage-forecast.dll` | 135168 | `FB863FF75EA33D6B3F7F9B1471E12E33D5FEBD75595EB680A2D0A49369FDE880` |
| `damage-forecast.json` | 371 | `09D4DC4EFA54BAA11AB3B3B1E2660FF8DB15D5EA153103AB837AAD22DBEB68C5` |

### 5.2 当前配置

唯一相关配置：

`C:\Users\ROG\AppData\Roaming\SlayTheSpire2\mod_configs\STS2PartyWatch.cfg`

- bytes：797；
- key count：18；
- SHA256：`783C0B880EB1851D813801A6FD35F4D266AE4A2FE69BE50C4812FC7BEA714553`。

有序键：

1. `ConfigLanguage`
2. `EnablePartyWatchHud`
3. `ShowAdvancedShieldHeartDetails`
4. `FreezeHudNumbersAfterTurnEnd`
5. `DamageDisplayMode`
6. `IncomingDamagePlacement`
7. `IncludeCurrentBlockInIncomingDamage`
8. `IncludePowerBlockInIncomingDamage`
9. `IncludeRelicBlockInIncomingDamage`
10. `IncludePowerHpLossModifiersInIncomingDamage`
11. `IncludeRelicHpLossModifiersInIncomingDamage`
12. `ShowLocalPlayerHudInMultiplayer`
13. `HudAnchorPreset`
14. `HorizontalOffset`
15. `VerticalOffset`
16. `TotalExpectedLossColor`
17. `ShieldDetailColor`
18. `HeartDetailColor`

当前值：

| Key | Value |
|---|---|
| `ConfigLanguage` | `SimplifiedChinese` |
| `EnablePartyWatchHud` | `True` |
| `ShowAdvancedShieldHeartDetails` | `True` |
| `FreezeHudNumbersAfterTurnEnd` | `True` |
| `DamageDisplayMode` | `Both` |
| `IncomingDamagePlacement` | `RightOfExpectedHpLoss` |
| `IncludeCurrentBlockInIncomingDamage` | `False` |
| `IncludePowerBlockInIncomingDamage` | `False` |
| `IncludeRelicBlockInIncomingDamage` | `False` |
| `IncludePowerHpLossModifiersInIncomingDamage` | `False` |
| `IncludeRelicHpLossModifiersInIncomingDamage` | `False` |
| `ShowLocalPlayerHudInMultiplayer` | `True` |
| `HudAnchorPreset` | `HealthBarRight` |
| `HorizontalOffset` | `0` |
| `VerticalOffset` | `0` |
| `TotalExpectedLossColor` | `[1, 1, 1, 1]` |
| `ShieldDetailColor` | `[0.55, 0.85, 1, 1]` |
| `HeartDetailColor` | `[1, 0.55, 0.62, 1]` |

### 5.3 Git/workspace

- branch：`main`；
- HEAD：`308ffd5`；
- remote：`https://github.com/Niizuma24290/Damage-Forecast.git`；
- 本机仓库目录仍为 `STS2-Party-Watch-v2`；
- 工作树非净，包含尚未提交的 G6 改名结果；
- C0 没有产生任何文件修改；
- C1 必须保留既有 G6 修改，不能用 reset 或 checkout 回滚整个工作树。

本机仓库目录改名不包含在 C1-C4 中。它会影响当前 Codex workspace，应在 C4 closure 后另开批准。

---

## 6. C0 完整命中 inventory

### 6.1 生产源码：18 个命中文件

#### A. C1 普通符号改名：15 个文件

- `src/DamageForecast/Combat/EnemyPreActionSurvivalPreview.cs`
- `src/DamageForecast/Combat/LegacyDiamondDiademDamageForecast.cs`
- `src/DamageForecast/Combat/LocalIncomingDamageReader.cs`
- `src/DamageForecast/Combat/VerifiedEnemyDamageModifier.cs`
- `src/DamageForecast/DamageForecast.csproj`
- `src/DamageForecast/Diagnostics/PartyWatchDiagnostics.cs`
- `src/DamageForecast/Patches/NativeCoveringScreenLifecyclePatch.cs`
- `src/DamageForecast/Patches/PartyWatchBaseLibTitlePatch.cs`
- `src/DamageForecast/Settings/PartyWatchConfigText.cs`
- `src/DamageForecast/Settings/PartyWatchSettingsAdapter.cs`
- `src/DamageForecast/UI/PartyWatchHudDisplay.cs`
- `src/DamageForecast/UI/PartyWatchHudSnapshotStore.cs`
- `src/DamageForecast/UI/PartyWatchHudVisibilityPolicy.cs`
- `src/DamageForecast/UI/PartyWatchNativeCoveringScreenTracker.cs`
- `src/DamageForecast/UI/PartyWatchUiSettings.cs`

#### B. 混合普通符号与 migration compatibility：2 个文件

`src/DamageForecast/MainFile.cs`

- 普通 `PartyWatch*` 引用：C1 改名；
- `LegacyModId = "sts2-party-watch-v2"`：upgrade/rollback compatibility；
- `LegacyDiagnosticPrefix = "[STS2 Party Watch]"`：v0.2.x support marker，建议在 C2 版本切换时到期；
- legacy assembly conflict detector：至少保留到 C3 rollback/re-upgrade 通过。

`src/DamageForecast/Patches/ForecastRefreshPatch.cs`

- 普通 HUD 类型引用：C1 改名；
- 三个旧 Godot node 名：C1 改为 `DamageForecast*`；
- 不建议永久保留旧 node literal，因为安装切换必须在游戏关闭时进行，Godot node 不跨进程持久化。

#### C. C2 persistence identity：1 个文件

`src/DamageForecast/Settings/PartyWatchBaseLibConfig.cs`

当前：

- namespace：`STS2PartyWatch.Settings`；
- type：`PartyWatchBaseLibConfig`；
- storage：`STS2PartyWatch.cfg`。

目标：

- namespace：`DamageForecast.Settings`；
- type：`DamageForecastBaseLibConfig`；
- storage：`DamageForecast.cfg`。

不得在 C1 单独改变该 namespace/type，否则会在没有迁移器的情况下提前改变 BaseLib storage identity。

### 6.2 普通生产符号精确集合

以下普通符号应改为对应 `DamageForecast*`：

- `PartyWatchDiagnostics`
- `PartyWatchBaseLibTitlePatch`
- `PartyWatchBaseLibPageTitlePatch`
- `PartyWatchConfigLanguage`
- `PartyWatchConfigText`
- `PartyWatchSettingsAdapter`
- `PartyWatchHudDisplay`
- `PartyWatchHudSnapshotStore`
- `PartyWatchHudVisibilityPolicy`
- `PartyWatchNativeCoveringScreenTracker`
- `PartyWatchUiSettings`
- `PartyWatchHudAnchor`
- `PartyWatchVisibilityProfiling`
- `PartyWatchDisableGlobalVisibilityPatches`
- `PARTY_WATCH_VISIBILITY_PROFILING`
- `PARTY_WATCH_DISABLE_GLOBAL_VISIBILITY_PATCHES`

建议对应名称：

- `DamageForecastDiagnostics`
- `DamageForecastBaseLibTitlePatch`
- `DamageForecastBaseLibPageTitlePatch`
- `DamageForecastConfigLanguage`
- `DamageForecastConfigText`
- `DamageForecastSettingsAdapter`
- `DamageForecastHudDisplay`
- `DamageForecastHudSnapshotStore`
- `DamageForecastHudVisibilityPolicy`
- `DamageForecastNativeCoveringScreenTracker`
- `DamageForecastUiSettings`
- `DamageForecastHudAnchor`
- `DamageForecastVisibilityProfiling`
- `DamageForecastDisableGlobalVisibilityPatches`
- `DAMAGE_FORECAST_VISIBILITY_PROFILING`
- `DAMAGE_FORECAST_DISABLE_GLOBAL_VISIBILITY_PATCHES`

### 6.3 带旧名的生产文件名：10 个

C1 改名：

- `Diagnostics/PartyWatchDiagnostics.cs` → `Diagnostics/DamageForecastDiagnostics.cs`
- `Patches/PartyWatchBaseLibTitlePatch.cs` → `Patches/DamageForecastBaseLibTitlePatch.cs`
- `Settings/PartyWatchConfigText.cs` → `Settings/DamageForecastConfigText.cs`
- `Settings/PartyWatchSettingsAdapter.cs` → `Settings/DamageForecastSettingsAdapter.cs`
- `UI/PartyWatchHudDisplay.cs` → `UI/DamageForecastHudDisplay.cs`
- `UI/PartyWatchHudSnapshotStore.cs` → `UI/DamageForecastHudSnapshotStore.cs`
- `UI/PartyWatchHudVisibilityPolicy.cs` → `UI/DamageForecastHudVisibilityPolicy.cs`
- `UI/PartyWatchNativeCoveringScreenTracker.cs` → `UI/DamageForecastNativeCoveringScreenTracker.cs`
- `UI/PartyWatchUiSettings.cs` → `UI/DamageForecastUiSettings.cs`

C2 改名：

- `Settings/PartyWatchBaseLibConfig.cs` → `Settings/DamageForecastBaseLibConfig.cs`

### 6.4 Tests：11 个命中文件

C1 编译引用和普通名称同步：

- `tests/DamageForecast.ContractTests/DamageForecast.ContractTests.csproj`
- `tests/DamageForecast.ContractTests/Program.cs`
- `tests/DamageForecast.ContractTests/ProjectionContractCases.cs`

G6 identity/upgrade 历史与活动契约混合，不得盲目替换：

- `tests/DamageForecast.ContractTests/identity-contract.json`
- `tests/DamageForecast.ContractTests/IdentityContractFixture.cs`
- `tests/DamageForecast.ContractTests/IdentityMigrationContractCases.cs`
- `tests/DamageForecast.ContractTests/IdentityPackagingContractCases.cs`
- `tests/DamageForecast.ContractTests/IdentityPublishTreeContractCases.cs`
- `tests/DamageForecast.ContractTests/IdentityUpgradeToolContractCases.cs`

C2 配置 fixtures：

- `tests/DamageForecast.ContractTests/fixtures/settings-default.json`
- `tests/DamageForecast.ContractTests/fixtures/settings-nondefault.json`

处理原则：

- G6 legacy/active/approved identity 记录保持历史意义；
- 不应把 G6-D03/D04/D05/D13 原文改写成“当时已批准新配置文件”；
- 建议新增 Post-G6 naming contract 或 schema v3；
- G6 identity contract 可作为 immutable baseline 继续被测试引用；
- negative migration fixtures 可以保留旧 slug/type/file literal，但必须进入机器可读 allowlist。

### 6.5 Scripts

唯一 active script 命中：

`scripts/Install-LocalMod.ps1`

- `$legacyModId = "sts2-party-watch-v2"` 是 upgrade/rollback compatibility；
- 保留为受支持旧安装的 upgrade/rollback compatibility；
- C4 只能收紧精确 allowlist，不能在仍承诺 Workshop 旧版直升时删除该兼容能力；
- 不得在 C1 直接删除旧安装检测。

### 6.6 Current authority：8 个文件

- `README.md`：2 行旧名命中；
- `docs/architecture.md`：11 行；
- `docs/build-environment.md`：3 行；
- `docs/interface-map.md`：10 行；
- `docs/mechanics-evidence.md`：1 行；
- `docs/project-state.md`：18 行；
- `docs/task-notes/README.md`：1 行；
- `docs/v2-roadmap.md`：4 行。

这些文件当前正确描述 G6 的“永久 config compatibility island”。只有 C2/C3 完成后，才能在 C4 根据真实结果更新。

### 6.7 Historical evidence：39 个文件

`docs/task-notes/` 下除 current index `README.md` 外，共 39 个旧名命中文件，包含：

- documentation/architecture/G6 master task cards；
- Phase 0、1-4、5、6、8、9、9A、9B、11、11A、11B、11C、11E、12A、12B、12C、13A；
- audit、runtime verification、repository closure；
- session handoff；
- Workshop private RC；
- 旧安装路径、旧 DLL/manifest 哈希和旧行为记录。

分类：全部为历史证据，保留原文。未来只允许追加新的 Post-G6 ledger/closure，不批量回写正文。

### 6.8 可删除或需保留的旧 artifacts

仓库忽略区域发现：

- `src/tests` 下 `bin/obj`：46 个旧名 path hit；
- `work/`：212 个旧名 path hit。

`work/` 分类：

| 类别 | path hit 数 | Disposition |
|---|---:|---|
| 旧 `bin/obj/publish` | 44 | 可再生成，C4 获批后精确清理 |
| forecast guardrail 派生输出 | 137 | G6 evidence 已闭合，但仍需获批清理 |
| disabled local mod 备份 | 9 | C3 通过后送回收站 |
| manual install backups | 15 | C3 通过后送回收站 |
| G6 conflict 测试树 | 3 | C3/C4 获批后清理 |
| Workshop upload staging | 4 | Workshop 相关，普通清理不得触碰 |

注意：

- 当前 `STS2PartyWatch.cfg` 不是旧垃圾；在 C3 完成前它是唯一有效配置及 rollback 来源；
- Workshop staging 中存在 item `3755598583` 的 `mod_id.txt` 和旧描述，未经 Workshop 专项批准不得修改或删除；
- 清理必须使用已验证的绝对路径和回收站，不得通过宽泛 wildcard 递归删除。

---

## 7. 已批准架构决策：直接全面改名 + 隔离 migration shim

### 7.1 决策

用户已选择兼顾全面改名与 Workshop 直接升级的路线：

- 新配置类型使用 `DamageForecast.Settings.DamageForecastBaseLibConfig`；
- 新配置文件使用 `DamageForecast.cfg`；
- persisted key `EnablePartyWatchHud` 明确迁为 `EnableDamageForecastHud`；
- 其余 17 项 key 名保持；
- 18 项的数量、顺序、类型、值、枚举、颜色与语义保持；
- 最新 Workshop 版本内置 migration shim，不要求用户先运行某个中间版本；
- 普通业务代码、玩家可见 UI、日志、Godot nodes 和新配置中旧名为 0；
- 旧名称仅存在于隔离 migration shim、installer/rollback compatibility、migration tests 和历史记录。

该决策明确取代此前“永久保留 `EnablePartyWatchHud` persisted member”的推荐方案，但不修改 G6 当时已经批准和完成的历史决策。

### 7.2 为什么不能仅依赖中间版本

Steam Workshop 会把订阅用户更新到当前最新内容，但不保证用户依次运行每个中间 Mod 版本。用户可能从 v0.2.0 直接跳到未来最新版。

因此所有仍承诺支持旧安装直接升级的最新版本，都必须具备：

- 识别 `STS2PartyWatch.cfg`；
- 验证旧 18 项 schema；
- 将 `EnablePartyWatchHud` 映射为 `EnableDamageForecastHud`；
- 原子生成和验证 `DamageForecast.cfg`；
- 重复启动时保持幂等；
- old/new 冲突时 fail-closed。

### 7.3 主体代码与 compatibility subsystem 边界

主体代码只允许使用：

- `DamageForecastBaseLibConfig`；
- `EnableDamageForecastHud`；
- `DamageForecastSettingsAdapter`；
- `DamageForecast*` HUD/node/diagnostic symbols；
- `DamageForecast.cfg`。

compatibility 子系统统一位于：

`src/DamageForecast/Compatibility/**`

允许按职责拆分，而不是强迫全部逻辑塞进一个小文件。建议结构：

- `CompatibilityBootstrap.cs`：唯一主体入口；
- `ConfigSchemaRegistry.cs`：已支持 schema descriptors；
- `ConfigSchemaDetector.cs`：依据 filename、key set、marker 和内容判断 schema；
- `ConfigMigrationPipeline.cs`：按版本执行 pure transforms；
- `ConfigMigrationTransaction.cs`：backup/temp/flush/atomic move/recovery；
- `ConfigMigrationResult.cs`：成功等级和结构化诊断；
- `PreDamageForecastSchemaV1.cs`：旧 18 项 descriptor 与 key map；
- `DamageForecastSchemaV1.cs`：新 18 项 descriptor；
- 未来 `DamageForecastSchemaV2+`：新增配置的 migration steps；
- `LegacyIdentityDescriptor.cs`：旧 filename/key/mod identity 集中数据。

文件/type 自身使用 DamageForecast 或中性 compatibility 命名；旧字符串只存在于受 contract 管理的 descriptors、fixtures 和 diagnostics。compatibility subsystem 不得：

- 参与伤害计算；
- 参与 HUD rendering；
- 参与日常配置保存；
- 持续 dual-write 两份 cfg；
- 把旧 schema DTO/type 暴露给主体代码；
- 自动删除 old cfg；
- 承载与迁移无关的新功能或行为修复。

主体代码只能收到当前 schema 的 typed result 或明确的 migration status，不得收到旧 DTO。

### 7.4 允许的兼容空间

兼容空间不以“代码行必须最少”为目标，而以“旧身份不越界、迁移成功率最大化、未来可整体删除”为目标。

允许 compatibility subsystem 包含：

- 多个旧/新 schema descriptors；
- key aliases、大小写和已知 BaseLib 表示形式兼容；
- bool/enum/float/color 的严格加容错解析；
- future-setting default injection；
- unknown-field archive；
- interrupted-transaction recovery；
- rollback reverse transforms；
- structured diagnostics；
- property-based/fuzz fixtures；
- explicit source-version → target-version migration graph。

初始旧 identity 数据至少包括：

- `STS2PartyWatch.cfg`：migration source/rollback；
- `EnablePartyWatchHud`：old→new key mapping；
- `sts2-party-watch-v2`：旧安装/assembly conflict detection 与 rollback；
- 必要的 G6 legacy type/path literals：仅 compatibility contract/negative fixtures/history。

`[STS2 Party Watch]` startup marker 在 v0.2.x 结束后移除；旧 Godot node 名不进入长期 allowlist。

护栏按目录 ownership + machine-readable token inventory 管理，不要求所有旧 token 固定在单一文件或永远保持相同 occurrence count。任何新增旧 token/schema alias 必须：

1. 只位于 `Compatibility/**`、migration tests/fixtures、installer rollback 或 history；
2. 在 compatibility contract 中登记来源版本、用途、过期条件和测试；
3. 不允许普通 Settings/UI/Combat/Forecast/Patches 引用；
4. 经过 review，防止普通功能进入 compatibility。

### 7.5 完成定义与未来退休

本任务完成时，“全面改名”定义为：

> 玩家可见内容和主体生产代码完全统一为 Damage Forecast；旧身份被隔离在一个幂等、版本化、受自动测试保护且未来可整体删除的 compatibility subsystem。该子系统使长期未上线的 Workshop 用户能够从任何受支持旧版直接升级，并为未来新增设置提供明确的 schema evolution 路径。

如果未来明确放弃所有旧 Workshop 直接升级支持，可另开 C5 删除整个 `Compatibility/**` 子系统，届时才追求整个 active repository 旧字符串绝对 0。C5 不属于当前 C1-C4 授权。

“两个月”或“半年”只能作为观察窗口，不能单独证明所有 Workshop 用户已经运行迁移。C5 删除时必须明确选择以下之一：

- 终止从 pre-migration 版本直接升级的支持；
- 保留独立离线迁移工具和人工支持路径；
- 继续保留 compatibility subsystem。

---

## 8. C1 精确方案：普通编译身份与 Godot node

### 8.1 C1 授权边界

C1 只允许：

- 改普通 `PartyWatch*` 类、枚举、内部符号；
- 改 9 个普通旧文件名；
- 改 MSBuild property 和 conditional define；
- 改 Godot node 名；
- 增加 node ownership/duplicate 防护；
- 同步必要的测试和 Post-G6 contract；
- 运行 stable/beta 编译和测试；
- 写入忽略的 build/test scratch output。

C1 不允许：

- 改 `PartyWatchBaseLibConfig` namespace/type/file identity；
- 创建或迁移真实 `DamageForecast.cfg`；
- 改本机 `STS2PartyWatch.cfg`；
- 改 manifest version；
- 安装 Mod 或启动游戏；
- 修改 Workshop；
- 删除旧配置、备份或 artifacts；
- stage/commit/push/tag；
- 修复 direct HP-loss HUD bug。

### 8.2 Godot node 目标名

当前：

- `STS2PartyWatchForecastLabel`
- `STS2PartyWatchIncomingDamageLabel`
- `STS2PartyWatchForecastDetailsLabel`

推荐目标：

- `DamageForecastExpectedLossLabel`
- `DamageForecastIncomingDamageLabel`
- `DamageForecastDetailsLabel`

### 8.3 Godot duplicate 防护

节点 resolver 必须：

1. 以 `parent + exact name + expected type` 查找；
2. 已存在正确节点时复用；
3. 创建节点后写入 `damage-forecast` ownership metadata 或 group；
4. 同名错误类型不得静默接管，必须 fail-closed 并记录诊断；
5. 若发现多个本 Mod owned 节点，只保留 canonical 实例；
6. covering screen、scene exit、HealthBar 销毁时根据实际引用清理；
7. repeated refresh 不得继续 `AddChild`；
8. 多个玩家 HealthBar 各自最多拥有一组 HUD；
9. 不永久保留旧 node literal。

原因：安装切换必须在游戏关闭时进行，旧 Godot nodes 不跨完整进程持久化。若未来要支持 DLL 热替换，必须另开兼容任务，不能隐式扩大 C1。

### 8.4 C1 rollback

- C1 不接触配置和安装；
- rollback 仅逆向应用明确的文件名/符号映射；
- 不使用 git reset/checkout；
- 必须保护当前工作树已有 G6 修改；
- build scratch 可以重新生成，但不得把 scratch 当作源码恢复依据。

---

## 9. C2 配置迁移设计

### 9.1 目标

- 新类型：`DamageForecast.Settings.DamageForecastBaseLibConfig`；
- 新文件：`DamageForecast.cfg`；
- 新 persisted key：`EnableDamageForecastHud`；
- BaseLib registration key 继续为 `damage-forecast`；
- 将 old key `EnablePartyWatchHud` 精确映射为 `EnableDamageForecastHud`；
- 迁移时保持 18 项数量、顺序、类型、值、枚举、颜色和语义；
- migration 必须在新 config 类型第一次构造/触碰之前完成。
- 最新版本必须能够直接迁移尚未运行过任何中间版本的 Workshop 旧配置。

### 9.2 必须新增的职责

建议新增：

- `Settings/DamageForecastBaseLibConfig.cs`
- `Compatibility/CompatibilityBootstrap.cs`
- `Compatibility/ConfigSchemaRegistry.cs`
- `Compatibility/ConfigSchemaDetector.cs`
- `Compatibility/ConfigMigrationPipeline.cs`
- `Compatibility/ConfigMigrationTransaction.cs`
- `Compatibility/ConfigMigrationResult.cs`
- `Compatibility/PreDamageForecastSchemaV1.cs`
- `Compatibility/DamageForecastSchemaV1.cs`
- `Compatibility/LegacyIdentityDescriptor.cs`
- `Settings/DamageForecastConfigSchema.cs` 或等价 typed validator
- 独立 migration marker/transaction model
- `ConfigMigrationContractCases.cs`
- old/new/default/nondefault/malformed/collision fixtures
- installer rollback 的 new→old reverse sync 支持
- Post-G6 identity/persistence contract schema

主体 Settings/UI/Combat/Patches 不得直接引用 old filename/key。所有旧配置知识集中在 compatibility shim。

### 9.3 启动顺序

严格顺序：

1. 不引用新 BaseLib config 类型的独立 migrator 启动；
2. 定位旧/new/backup/marker 的精确路径；
3. 验证旧/new 文件状态；
4. 完成 backup 和目标文件事务；
5. 重新读取并验证新文件；
6. 写入 completed marker；
7. 才允许构造 `DamageForecastBaseLibConfig`；
8. 调用 `ModConfigRegistry.Register("damage-forecast", config)`；
9. apply config；
10. 初始化 patch/HUD。

不得先构造新 config 再迁移，否则 BaseLib 可能先创建默认 `DamageForecast.cfg`。

### 9.4 状态决策表

| 旧 cfg | 新 cfg | 行为 |
|---|---|---|
| 存在 | 不存在 | 验证 old 18 项→rename one key→CreateNew backup→原子写 new→验证→marker |
| 不存在 | 存在 | 验证 new 后使用；不得伪造旧来源 |
| 存在 | 存在且语义一致 | 使用 new；保留 old 用于 rollback；必要时幂等补 marker |
| 存在 | 存在但不同 | fail-closed；不覆盖任何一方；要求人工决策 |
| 不存在 | 不存在 | fresh install；由新 config 建立默认文件 |
| 损坏 | 任意 | fail-closed；不得从损坏数据生成目标 |
| target 已写 | marker 缺失 | 若 old/new 严格一致，幂等补 marker |

### 9.5 18 项 validator

必须验证：

- 精确 18 项；
- 精确有序 key list；
- 无缺项、未知项、重复项；
- bool 可解析且值不变；
- enum name 与底层 numeric value 稳定；
- float 是有限值，不接受 `NaN`/Infinity；
- color 恰好 4 个分量；
- RGBA 顺序和值保持；
- JSON/UTF-8 有效；
- 原始长度和 SHA256；
- typed semantic digest；
- 基于显式 key map 的 old/new semantic equality。

由于一个 key 被明确改名，old/new 文件不要求 byte-for-byte 或 SHA256 相等。validator 必须以 typed value 和显式 key map 证明语义一致。除 key name 外，不得改变任何值的词法含义；颜色、float 和 enum 必须逐项比较。

固定映射：

| Old | New |
|---|---|
| `EnablePartyWatchHud` | `EnableDamageForecastHud` |

其他 17 项执行 identity mapping。

### 9.5.1 成功等级

不得用一个含糊的 `true/false` 隐藏数据质量。结果至少分为：

- `ExactSuccess`：所有 source 字段均来自受支持官方 schema，18 项逐项精确迁移；这是 release/runtime 验收所要求的正常成功；
- `RecoveredSuccess`：输入是受支持语义，但使用已知可容忍的表示差异，例如 key order、bool casing 或等价 numeric formatting；迁移后 typed values 完全一致；
- `Salvaged`：文件已损坏、缺项或包含无法解释的值；只保全部分有效数据，并完整归档 raw source；不得标记 completed，也不得作为“迁移成功”证据；
- `FailedSafe`：无法安全生成 target；不得写 final target、不得覆盖 source，Mod/配置注册按明确降级策略处理。

“migration 一定要成功”的发布契约定义为：

> 对所有曾由受支持官方版本正常写出的配置，必须达到 `ExactSuccess` 或语义完全等价的 `RecoveredSuccess`。任一官方 fixture、真实基线、随机合法组合或 crash-recovery case 失败，均阻断 build/publish。对于外部损坏或未知文件，不能承诺无损成功，但必须保证原始数据不被覆盖、不被删除、不被伪称为成功。

### 9.5.2 容错策略

允许容错但必须可证明：

- key 顺序变化；
- JSON whitespace/BOM/换行差异；
- 已知 BaseLib bool/enum/numeric 表示；
- old/new enum name 的已批准 alias；
- future schema 缺少后来新增的 optional setting 时注入该版本明确默认值；
- 未知字段保存到 transaction archive/marker 的 extension bag，并产生诊断，不静默丢弃 raw input。

不允许容错为静默猜测：

- 无法解析的 enum 自动选第一个值；
- 无法解析的颜色自动变白；
- old/new 两份冲突时按时间戳自动选边；
- source hash 在 Plan/Execute 之间变化后继续写 target；
- 将 `Salvaged` 当作 completed marker。

### 9.6 原子写入

推荐 backup/marker root：

`C:\Users\ROG\AppData\Roaming\SlayTheSpire2\damage-forecast-migration\transactions\<transaction-id>\`

该目录必须位于 `mod_configs` 外，避免 BaseLib 扫描或误读。

事务步骤：

1. 解析并规范化所有绝对路径；
2. 确认 source、target、backup、temp 均在批准根内；
3. 使用 `FileMode.CreateNew` 写 backup；
4. `Flush(true)`；
5. 重新读取 backup 并比对 source SHA256；
6. 在 `mod_configs` 下创建同卷 sibling temp；
7. 写入、flush、重新读取和验证；
8. 仅当 target 不存在时执行 atomic move；
9. 禁止 overwrite 已存在 target；
10. 验证 final target；
11. marker 使用 temp + atomic move；
12. 清理未提交 temp，但不得删除 source/backup。

### 9.7 marker 内容

至少记录：

- marker schema version；
- transaction ID；
- source/target/backup/temp 路径；
- source/target/backup length；
- SHA256；
- ordered-key digest；
- typed semantic digest；
- migration strategy；
- Mod version；
- game version/commit；
- UTC timestamp；
- status：planned/backed-up/target-written/completed/recovered/rolled-back；
- reverse-sync 状态；
- cleanup eligibility。

### 9.8 C2 版本

建议把真正执行配置身份迁移的发行版设为 `v0.3.0`，使其与 G6 `v0.2.0` persistence island 明确区分。

但 C1 不修改 manifest version；`v0.3.0` 必须在 C2 批准时再次确认。

v0.3.0 可以直接作为全面改名和迁移版本发布，不要求先上传单独桥接版。只要仍承诺支持旧 Workshop 安装，后续最新版本也必须保留同一幂等 shim；不能仅因 v0.3.0 曾经发布过就在下一版本删除。

### 9.9 C2 rollback

C2 只在临时 fixtures 中测试，不触碰本机配置，因此源码 rollback 不影响真实数据。

真实 rollback 逻辑必须提前在 C2 实现：

- old cfg 在 C3 验收前保持原位；
- 如果 new cfg 未发生用户修改，old v0.2.0 可直接读取原 old cfg；
- 如果 new cfg 已修改，rollback 前必须严格验证 new cfg，并在备份 old cfg 后执行 new→old reverse sync；
- reverse sync 同样采用 temp、flush、verify、atomic replace；
- old/new 内容冲突且来源不明确时 fail-closed。

### 9.10 Workshop 长期兼容策略

- Steam 更新成功只代表 Workshop 文件下载/安装成功，不代表 cfg migration 成功；
- migration 成功必须由本 Mod 的 validator、marker 和日志证明；
- compatibility bootstrap 在 `DamageForecast.cfg` 已有效且 marker/schema 状态一致时立即退出；
- 正常启动开销仅限精确文件存在性/marker 检查；
- old cfg 不自动删除，避免 rollback 和长期离线用户数据丢失；
- 不做持续 dual-write；rollback 由显式 reverse-sync transaction 负责；
- compatibility subsystem 的允许旧字符串、调用顺序、schema graph 和行为由 contract tests 冻结；
- 任何新功能或行为修复不得依赖 compatibility DTO；
- 未来新增第 19 项及更多设置时，必须引入版本化 schema：legacy v1=old 18、DamageForecast v1=new 18、未来 schema 按版本补新项默认值；
- migration graph 必须允许从任一受支持 source schema 直接走到 current schema，不能假设用户运行过每一个中间版本；
- compatibility 删除必须是整个目录和入口的一次性 retirement，不允许留下半套 detector/DTO/rollback code。

---

## 10. C3 真实安装、runtime、rollback 与清理

### 10.1 C3 必须拆分事务停点

建议即使用户批准 C3，也保持以下 stop points：

#### C3A：只读 Plan

- 确认游戏已关闭；
- 读取当前安装和配置；
- 确认只有一个 active Mod；
- 生成 transaction ID；
- 绑定 staging、active install、old cfg 的 SHA256；
- 输出计划，不执行写入。

#### C3B：执行安装和首次迁移

- 为当前 v0.2.0 `damage-forecast` 创建新的恢复备份；
- 备份放在游戏根、但位于 Loader 扫描的 `mods` 外；
- 为 old cfg 创建独立 AppData migration backup；
- 安装目标 build；
- 不删除 old cfg；
- 用户启动游戏并完成首次 runtime。

#### C3C：完整 restart、rollback、re-upgrade

- 验证新 cfg；
- 修改一个低风险设置；
- 正常退出并完整重启；
- rollback 到 v0.2.0；
- 必要时执行 new→old reverse sync；
- 用户启动并确认旧版本读取；
- 再次关闭游戏；
- re-upgrade；
- 第二次完整重启并确认无重复。

#### C3D：清理

只有获得单独明确批准后：

- 将获批 old cfg、安装备份和旧 work artifacts 送 Windows Recycle Bin；
- 不永久删除；
- 不触碰 Workshop staging；
- 清理后重新验证 active install 和 new cfg。

### 10.2 新恢复备份是强制项

G6 的 `.damage-forecast-backups` 已在 G6-7 清理，当前不存在。

因此 C3 不得假设旧恢复包可用。替换当前 v0.2.0 前必须重新备份：

- 当前 DLL SHA256：`FB863FF75EA33D6B3F7F9B1471E12E33D5FEBD75595EB680A2D0A49369FDE880`；
- 当前 manifest SHA256：`09D4DC4EFA54BAA11AB3B3B1E2660FF8DB15D5EA153103AB837AAD22DBEB68C5`；
- 当前 cfg SHA256：`783C0B880EB1851D813801A6FD35F4D266AE4A2FE69BE50C4812FC7BEA714553`。

### 10.3 runtime 边界

用户负责：

- 启动游戏；
- 游戏内观察；
- 正常退出游戏；
- 报告配置页、HUD、重复节点和功能状态。

Codex 负责：

- 在启动前确认游戏关闭；
- 执行获批的事务；
- 启动后只读检查新日志、安装、cfg、marker 和哈希；
- 区分 identity/load/config continuity 与已知 direct HP-loss defect。

---

## 11. Godot runtime 验收矩阵

必须覆盖：

| Case | 预期 |
|---|---|
| 无节点首次 refresh | 创建恰好一组新 node |
| 已存在正确 node | 复用，不新增 |
| 同名错误类型 | fail-closed，不接管其他 node |
| 连续 refresh 100 次 | node 数量不增长 |
| covering screen 打开/关闭 | HUD 隐藏/恢复且不重复 |
| combat scene exit/re-entry | 旧引用清理，新 scene 恰好一组 |
| 多人多个 HealthBar | 每个 owner 最多一组，本机边界不变 |
| old/new Mod 冲突 | 只允许一个初始化，禁止双 patch/HUD |
| 完整重启 | 一个配置页、一个初始化、一个 HUD owner |
| rollback + restart | 旧版本正常加载，不留下新 node |
| re-upgrade + restart | 新 node 仍不重复 |

旧 node compatibility 不作为永久要求，因为所有安装切换均要求完整关闭游戏。

---

## 12. Active-scope 零旧名护栏

### 12.1 Layer 1：编译标识

对构建 assembly 进行反射或等价 compiler-aware 检查：

- 普通 `PartyWatch*` type/member 为 0；
- `STS2PartyWatch.*` compiled namespace/type 为 0；
- `PARTY_WATCH_*` define 为 0；
- 新 config 的 compiled persisted member 必须为 `EnableDamageForecastHud`；
- `EnablePartyWatchHud` 只能作为 compatibility shim 内的旧 key string，不得继续是 compiled property/member；
- 不将字符串/comment 命中误判为 compiled symbol。

### 12.2 Layer 2：源码和路径

禁止：

- 旧普通类文件名；
- 旧普通 namespace/type/member；
- 旧 MSBuild property；
- 旧 conditional constant；
- 旧 Godot node 名；
- active manifest/DLL/install identity 回退；
- 通过字符串拼接绕过旧名扫描。

### 12.3 Layer 3：机器可读 compatibility allowlist

每一项旧 literal 必须记录：

- token；
- exact files；
- owner；
- purpose；
- lifetime/expiry；
- rollback behavior；
- Gate decision；
- tests。

预期临时/兼容项：

- `EnablePartyWatchHud`：仅 compatibility shim 的 old→new key mapping 和 migration fixtures；
- `STS2PartyWatch.cfg`：compatibility shim 的 migration source 及 rollback literal；
- `sts2-party-watch-v2`：installer/legacy conflict detection 与 rollback；
- `[STS2 Party Watch]`：只到 v0.2.x，C2 目标版本应移除；
- G6 historical contract 的旧 type/path/slug；
- migration negative fixtures。

Godot旧 node 名不进入永久 allowlist。

每一个 compatibility token 必须限定到批准目录/文件、source schema、用途和测试。允许因新增受支持 schema 扩展 occurrence count，但 contract inventory 的任何增量必须显式 review。普通 Settings/UI/Combat/Forecast/Patches 中出现旧 key/file literal 必须立即失败。

### 12.4 Layer 4：scope

Active scope：

- `src/DamageForecast/`，排除 `bin/obj`；
- active contract code 和 fixtures；
- `scripts/`；
- `README.md`；
- current authority 七个 docs 文件和 `docs/task-notes/README.md`；
- publish trees。

Excluded but reported：

- 39 个 historical task notes；
- migration ledger；
- `work/`；
- `bin/obj`；
- Workshop staging；
- 本机仓库根目录名；
- 外部 Workshop/server 状态。

### 12.5 Publish guard

stable/beta publish tree 必须继续只包含：

- `damage-forecast.dll`
- `damage-forecast.json`

不得出现：

- PDB；
- log；
- migration backup；
- cfg；
- marker；
- legacy DLL/manifest；
- nested backup directory。

---

## 13. 精确 Gate 测试矩阵

### 13.1 C1

- stable v0.107.1 compile；
- beta v0.109.0 compile；
- stable/beta 全部现有 contract，C0 基线为 181 项；
- 普通旧 type/namespace/member 反射结果为 0；
- 9 个 C1 旧文件名不存在；
- config compatibility island 仍精确存在且未改变 storage identity；
- MSBuild property/define 全部迁为 `DamageForecast`；
- Godot node 单例/ownership 测试；
- stable/beta publish 各恰好两个文件；
- stable/beta matching-artifact guard；
- 未批准的产物哈希差异必须停止。

建议命令入口：

- `scripts/Test-ForecastGuardrails.ps1 -Target all`
- `scripts/Build-DualTargets.ps1`
- `scripts/Test-IdentityPublishTrees.ps1`

实际参数和 scratch 路径必须在 C1 开始时再次核对。

### 13.2 C2

#### Schema cases

- all-default 18 项；
- all-nondefault 18 项；
- 当前真实 797-byte fixture；
- old `EnablePartyWatchHud` → new `EnableDamageForecastHud` 精确映射；
- 其他 17 项 identity mapping；
- 新 config/type 中不存在 compiled old property；
- enum name 和 numeric value；
- bool；
- finite float；
- exact RGBA；
- ordered key list。
- official-writer compatibility：收集每个受支持历史版本真实写出的 cfg fixture；
- exhaustive enum combinations 与 bool boundary；
- property-based 随机合法配置，固定 seed 并保存失败样本；
- future schema：old 18 → new 18+N，新字段按版本默认值注入；
- migration graph：每个受支持 source schema 直接到 current schema。

#### State cases

- old-only；
- new-only；
- both-equal；
- both-divergent；
- neither；
- 模拟用户从 v0.2.0 直接跳到当前最新版，未运行任何中间版本；
- 模拟长期离线 Workshop 用户在最新版第一次启动；
- repeated/idempotent migration；
- target exists but marker missing；
- marker exists but target missing；
- backup exists from interrupted transaction。

#### Invalid input

- invalid JSON；
- truncated file；
- missing key；
- extra key；
- duplicate key；
- invalid bool；
- invalid enum；
- `NaN`/Infinity；
- wrong color component count；
- invalid UTF-8；
- source changes between Plan and Execute。
- unknown fields：raw source/extension bag 被保全并产生诊断；
- partially corrupted input：只能得到 `Salvaged`/`FailedSafe`，不得伪称 completed。

#### Transaction failure injection

- backup CreateNew failure；
- backup flush failure；
- temp write failure；
- target move collision；
- target successful but marker failure；
- restart recovery；
- cleanup temp failure；
- reverse sync conflict。
- 进程在每个事务边界被终止后重新启动 recovery；
- disk-full、access-denied、read-only source、target race；
- 10,000+ 次随机合法 migration round-trip/property test 或等价覆盖。

#### Ordering

- migration completion before BaseLib config type construction；
- registration exactly once；
- one config page；
- source/backup/target hash binding；
- marker 完成后的普通启动不再解析 old cfg 内容；
- 除 `Compatibility/**` 子系统外，主体 assembly/code 不引用 old key/file。
- 所有 official fixtures 必须为 `ExactSuccess`/等价 `RecoveredSuccess`；
- `Salvaged` 和 `FailedSafe` 必须阻断 publish acceptance。

### 13.3 C3 stable runtime

1. 游戏关闭；
2. old cfg baseline：797 bytes、18 ordered keys、已知 SHA；
3. current v0.2.0 install backup 完成并复核；
4. approved build 安装；
5. 首次启动只加载一次；
6. 一个 Damage Forecast 配置页；
7. 新 cfg 存在且 18 项逐项一致；
   - `EnableDamageForecastHud` 等于 old `EnablePartyWatchHud` 的值；
   - 其余 17 项值不变；
8. HUD 普通状态正常；
9. 修改一个低风险设置；
10. 正常退出；
11. 完整重启后设置保持；
12. 游戏关闭；
13. rollback 到 v0.2.0；
14. 必要时 reverse sync new→old；
15. 启动确认 old cfg 可读；
16. 游戏关闭；
17. re-upgrade；
18. 第二次完整重启；
19. 无重复 Mod、配置页、patch 或 HUD nodes；
20. 日志 warning/error 边界符合预期。

### 13.4 C3 beta

- beta v0.109.0 contract/build/publish；
- matching artifact；
- config migration smoke；
- one-load/one-page/one-HUD smoke；
- cfg hash/18项 smoke；
- direct HP-loss defect 单独标记，不作为 identity regression。

### 13.5 C4

- active assembly 普通 `PartyWatch*` 标识为 0；
- `STS2PartyWatch.*` compiled type 为 0；
- old Godot node 名为 0；
- old ordinary filenames/path 为 0；
- current authority 不再把旧 config identity 描述为永久 active identity；
- compatibility literals 仅存在于隔离 shim、installer/rollback、migration tests 和机器可读 allowlist；
- compatibility subsystem 的目录边界、token inventory、schema graph 与 contract 一致；
- 普通 Settings/UI/Combat/Forecast/Patches 的旧 key/file literal 为 0；
- historical docs 未被重写；
- publish exact two files；
- installed Mod exact one；
- active cfg only `DamageForecast.cfg`；
- rollback evidence closed；
- Workshop unchanged。

---

## 14. 风险登记

### R1：BaseLib load timing

风险：新 config 类型构造时可能立即读取/创建新文件。

控制：migration 必须先于任何新 config type touch；增加 ordering contract。

### R2：old→new key transform

风险：`EnablePartyWatchHud` → `EnableDamageForecastHud` 转换遗漏、重复或错误时，会让用户设置表现为重置。

控制：该 key transform 已获批准；使用显式一对一 map、old/new typed semantic comparison、真实 797-byte fixture 和 direct-skip Workshop case 覆盖。

### R3：old/new cfg divergence

风险：自动覆盖造成数据丢失。

控制：both-divergent 永远 fail-closed，不自动选边。

### R4：crash consistency

风险：target 写成但 marker 未完成，或 backup 不完整。

控制：CreateNew、flush、verify、same-volume atomic move、幂等 recovery。

### R5：rollback 丢失新设置

风险：用户在新版本修改设置后，旧版本仍读取旧 cfg。

控制：rollback 前 new→old reverse sync，先备份 old，再原子替换。

### R6：Godot duplicate nodes

风险：重复 refresh、scene lifecycle 或双 assembly 创建重复 HUD。

控制：owner metadata、canonical resolver、exact type、single initializer、完整重启。

### R7：旧 assembly/install conflict detector 过早移除

风险：旧本地安装或未来 Workshop 内容与新 Mod 同时加载。

控制：只要仍承诺旧 Workshop 安装直接升级，就继续在精确 compatibility allowlist 中保留；不得因经过一两个版本而自动删除。

### R8：dirty worktree

风险：C1 误覆盖尚未提交的 G6 修改。

控制：增量 diff、精确文件范围、禁止 reset/checkout、每 Gate 报告 diff。

### R9：derived artifact false positives

风险：`bin/obj/work` 旧名导致零扫描误判。

控制：active guard 与 artifact inventory 分开；派生物在 C4 获批后精确清理。

### R10：Workshop 外部状态

风险：本地代码统一但 Workshop 描述、payload 或 item identity 仍旧。

控制：明确标记 `external-unchanged`；未获 Workshop 专项批准不得宣称全面外部统一。未来 upload 前必须用旧 payload/config fixture 模拟用户跳过所有中间版本直接升级。

### R11：本机仓库目录旧名

风险：active source 统一后 workspace 根目录仍为旧名。

控制：不夹带改名；C4 closure 后另开 workspace/repository-path Gate。

### R12：行为修复混入

风险：Regret/Bad Luck/Beckon 修复改变 identity Gate 的行为结果。

控制：严格排除；只保留已知缺陷 baseline，完整改名后另开行为任务。

### R13：Steam 报告成功但配置迁移失败

风险：Steam 只确认 Workshop 内容下载/安装，不理解 BaseLib cfg migration；用户可能看到更新成功，但设置已经回到默认值。

控制：迁移必须在 BaseLib registration 前完成；失败时不得静默创建默认 target 覆盖现场；使用明确日志、marker、fail-closed 和 runtime property-by-property verification。

### R14：compatibility subsystem 存在但不能可靠迁移

风险：兼容代码长期存在，却只能覆盖当前一份 fixture；未来真实用户输入失败，既产生技术债又没有提供兼容价值。

控制：migration 是 release-blocking subsystem。所有 official-writer fixtures、真实 797-byte baseline、property/fuzz 合法组合、direct-skip、crash recovery 和 rollback cases 必须通过；任何受支持合法输入不能达到 `ExactSuccess`/等价 `RecoveredSuccess` 时禁止 build/publish acceptance。

### R15：未来退休过早或留下半套技术债

风险：两个月/半年后直接删除 compatibility，长期离线用户可能从旧版跳到无 migrator 的最新版；或者只删除入口但留下旧 DTO、tests 和 installer branches。

控制：C5 必须以明确的 supported-upgrade cutoff 为依据，而不是仅按时间；删除时一次性移除 bootstrap、schema registry 中旧版本、descriptors、runtime migration paths 和非历史 tests，同时保留独立 migration ledger。若仍支持旧版直升，则不能退休。

---

## 15. Rollback 总策略

### C1 rollback

- 逆向普通文件名/符号映射；
- 不触碰 config/install；
- 不使用 destructive Git 命令。

### C2 rollback

- 在真实执行前只影响源码和临时 fixtures；
- 可逆向代码改名；
- 不产生本机数据变化。

### C3 rollback

- 当前 v0.2.0 install 在替换前重新备份；
- old `STS2PartyWatch.cfg` 保留；
- new settings 若变化则 reverse sync；
- restore 后必须完整启动验证；
- rollback 通过后还要 re-upgrade，不能停在旧版本。

### Cleanup rollback

- 获批旧配置和备份送 Windows Recycle Bin；
- 报告被移动的精确路径；
- 在回收站未清空前可恢复；
- Workshop staging 不在普通 cleanup scope。

---

## 16. 每 Gate 的交付格式

每个 Gate 完成时必须报告：

1. Gate 授权文本；
2. 实际修改文件清单；
3. `git diff --stat` 和关键 diff 摘要；
4. 未触碰范围；
5. stable/beta 测试命令与结果；
6. contract count；
7. artifact 哈希；
8. config 路径、bytes、18项、SHA256；
9. runtime/log 结论；
10. 已知缺陷与新 regression 的区分；
11. 风险变化；
12. rollback 状态；
13. 下一 Gate 精确计划；
14. 停止并请求下一 Gate 批准。

禁止把“计划执行”“code-inferred”“build-verified”和“runtime-verified”混为同一证据等级。

---

## 17. 最终验收目标

- 普通 active source 中 `PartyWatch*` 符号为 0；
- active manifest/DLL/install identity 全部为 `damage-forecast`；
- active普通 C# namespace/type 使用 `DamageForecast`；
- 配置类型为 `DamageForecast.Settings.DamageForecastBaseLibConfig`；
- active 配置文件为 `DamageForecast.cfg`；
- 新 compiled persisted member 为 `EnableDamageForecastHud`；
- old `EnablePartyWatchHud` 仅作为 migration shim 的 key string；
- 18 项设置迁移前后数量、顺序、类型、值、枚举、颜色和语义完全一致；
- 其他 17 项 key 名保持，唯一 key rename 按已批准 map 完成；
- 最新版本可直接接住没有运行过任何中间版本的旧 Workshop 配置；
- 普通 Settings/UI/Combat/Forecast/Patches 中旧名称为 0；
- compatibility subsystem 只含已登记的旧 identity/schema 数据与迁移职责，不承载普通功能；
- 所有受支持官方版本正常写出的配置达到 `ExactSuccess` 或语义完全等价的 `RecoveredSuccess`；
- 损坏/未知输入保全 raw source，且不会被错误标记为 completed；
- schema registry 和 migration graph 能在未来新增配置时从每个受支持旧 schema 直达 current schema；
- compatibility subsystem 可通过未来 C5 整体删除，不与 Combat/UI/Forecast/Settings 日常功能交织；
- 完整重启后新配置可读写；
- rollback、reverse sync 和 re-upgrade 通过；
- Mod 只加载一次；
- BaseLib 只有一个配置页；
- Godot HUD node 不重复；
- old cfg 和获批备份仅在明确批准后送回收站；
- historical docs 和 migration ledger 保留旧名称；
- 自动测试拒绝旧普通名称重新进入 active scope；
- stable/beta guard 均通过；
- Regret/Bad Luck 行为缺陷未夹带修改；
- Workshop 未经单独批准保持不变；
- 未来 Workshop upload 仍需单独批准，并必须发布带 direct-upgrade compatibility subsystem 的已验证 artifact；
- 本机 repository root rename 如仍需要，另开后续 Gate。

---

## 18. 当前停止指令

C2 已完成。

下一 Session 读取本任务卡后，应先：

1. 只读确认 C3A/C3B-Prep closure、当前 Git diff、当前配置和安装哈希仍未变化；
2. 确认第 21 节 transaction/hash Plan 与 stable/beta 证据仍可复核；
3. 确认用户是否已明确批准 C3B；
4. 若没有明确 C3B 批准，停止，不得修改真实安装或配置；
5. 若已批准，只执行第 21.5 节定义的 C3B install transaction；
6. 安装后重新核对 active/backup 哈希并停止，由用户启动游戏；不得把 C3B 扩张为 C3C rollback/re-upgrade。

不得从本任务卡推定 C3C/C3D、C4、Git 或 Workshop 授权。

---

## 19. C1 execution record（2026-07-22）

### 19.1 授权与基线

用户授权文本：`开始c1`。

执行前只读复核：

- G6 三处 closure 标记仍为 Closed；
- branch `main`、HEAD `308ffd5`，工作树仍为任务卡记录的未提交 G6 状态；
- 游戏进程为 0；active Mod 仍仅有 `damage-forecast`；旧安装、安装备份和 Workshop 本地订阅路径均不存在；
- installed DLL/manifest 哈希仍分别为 `FB863FF75EA33D6B3F7F9B1471E12E33D5FEBD75595EB680A2D0A49369FDE880` 与 `09D4DC4EFA54BAA11AB3B3B1E2660FF8DB15D5EA153103AB837AAD22DBEB68C5`；
- 唯一真实配置仍为 797-byte、18-key `STS2PartyWatch.cfg`，SHA256 `783C0B880EB1851D813801A6FD35F4D266AE4A2FE69BE50C4812FC7BEA714553`；`DamageForecast.cfg` 不存在。

### 19.2 实际变更

- 完成第 6.2 节 16 项普通符号/MSBuild define 精确映射；
- 完成第 6.3 节 9 个 C1 普通文件名改名；
- 将三个 Godot node 名改为 `DamageForecastExpectedLossLabel`、`DamageForecastIncomingDamageLabel`、`DamageForecastDetailsLabel`；
- 新增 `DamageForecastHudNodeOwnershipPolicy`，运行时按 exact name + expected type + role ownership group 解析；正确节点复用，同名错类型 fail-closed，本 Mod owned 重复节点收敛到 canonical；
- 新增 `post-g6-naming-contract.json`，把 C1 active naming 与 G6 历史 identity contract 分层；
- 新增 8 项 C1 naming contracts 与 5 项 HUD ownership contracts；最终 contract count 从 C0 基线 181 增至 194；
- `PartyWatchBaseLibConfig.cs` 文件名、`STS2PartyWatch.Settings.PartyWatchBaseLibConfig`、`STS2PartyWatch.cfg`、`EnablePartyWatchHud`、18 项顺序和值语义继续冻结；
- manifest version 继续为 `v0.2.0`。

实际文件清单：

- 9 个改名：
  - `src/DamageForecast/Diagnostics/PartyWatchDiagnostics.cs` → `src/DamageForecast/Diagnostics/DamageForecastDiagnostics.cs`
  - `src/DamageForecast/Patches/PartyWatchBaseLibTitlePatch.cs` → `src/DamageForecast/Patches/DamageForecastBaseLibTitlePatch.cs`
  - `src/DamageForecast/Settings/PartyWatchConfigText.cs` → `src/DamageForecast/Settings/DamageForecastConfigText.cs`
  - `src/DamageForecast/Settings/PartyWatchSettingsAdapter.cs` → `src/DamageForecast/Settings/DamageForecastSettingsAdapter.cs`
  - `src/DamageForecast/UI/PartyWatchHudDisplay.cs` → `src/DamageForecast/UI/DamageForecastHudDisplay.cs`
  - `src/DamageForecast/UI/PartyWatchHudSnapshotStore.cs` → `src/DamageForecast/UI/DamageForecastHudSnapshotStore.cs`
  - `src/DamageForecast/UI/PartyWatchHudVisibilityPolicy.cs` → `src/DamageForecast/UI/DamageForecastHudVisibilityPolicy.cs`
  - `src/DamageForecast/UI/PartyWatchNativeCoveringScreenTracker.cs` → `src/DamageForecast/UI/DamageForecastNativeCoveringScreenTracker.cs`
  - `src/DamageForecast/UI/PartyWatchUiSettings.cs` → `src/DamageForecast/UI/DamageForecastUiSettings.cs`
- 10 个既有 production 文件更新：
  - `src/DamageForecast/Combat/EnemyPreActionSurvivalPreview.cs`
  - `src/DamageForecast/Combat/LegacyDiamondDiademDamageForecast.cs`
  - `src/DamageForecast/Combat/LocalIncomingDamageReader.cs`
  - `src/DamageForecast/Combat/VerifiedEnemyDamageModifier.cs`
  - `src/DamageForecast/DamageForecast.csproj`
  - `src/DamageForecast/Diagnostics/Aud0007VisibilityProfiler.cs`
  - `src/DamageForecast/MainFile.cs`
  - `src/DamageForecast/Patches/ForecastRefreshPatch.cs`
  - `src/DamageForecast/Patches/NativeCoveringScreenLifecyclePatch.cs`
  - `src/DamageForecast/Settings/PartyWatchBaseLibConfig.cs`
- 1 个新增 production 文件：`src/DamageForecast/UI/DamageForecastHudNodeOwnershipPolicy.cs`；
- 6 个既有 contract/test 文件更新：
  - `tests/DamageForecast.ContractTests/DamageForecast.ContractTests.csproj`
  - `tests/DamageForecast.ContractTests/Program.cs`
  - `tests/DamageForecast.ContractTests/ProjectionContractCases.cs`
  - `tests/DamageForecast.ContractTests/IdentityContractFixture.cs`
  - `tests/DamageForecast.ContractTests/IdentityPackagingContractCases.cs`
  - `tests/DamageForecast.ContractTests/IdentityMigrationContractCases.cs`
- 4 个新增 contract/test 文件：
  - `tests/DamageForecast.ContractTests/post-g6-naming-contract.json`
  - `tests/DamageForecast.ContractTests/PostG6NamingContractFixture.cs`
  - `tests/DamageForecast.ContractTests/PostG6NamingContractCases.cs`
  - `tests/DamageForecast.ContractTests/HudNodeOwnershipContractCases.cs`
- 本主任务卡更新为 C1 closure/C2 pending。

### 19.3 验证证据

- `scripts/Test-ForecastGuardrails.ps1 -Target all`：stable/beta 各 `194 discovered / 194 passed / 0 failed / 0 skipped`；
- stable/beta Release build：均 0 warnings / 0 errors；
- `scripts/Build-DualTargets.ps1`：stable/beta build + publish 通过；
- `scripts/Test-IdentityPublishTrees.ps1`：两侧均 exact two files，hash difference 未批准且实际无差异；
- stable/beta `damage-forecast.dll` SHA256：`2F09C7C2681F447F12C0FEE76D5D122FD8E7A6B4D1D57803C5BFB57FF37D2B3A`；
- stable/beta `damage-forecast.json` SHA256：`09D4DC4EFA54BAA11AB3B3B1E2660FF8DB15D5EA153103AB837AAD22DBEB68C5`；
- `git diff --check`、tracked/working forbidden-artifact review 均通过；
- compiler-aware contract 证明普通旧 type/member 为 0；9 个旧普通文件名不存在；旧 Godot node literal 不存在于 active compiler source。

### 19.4 明确未触碰

- 未创建、读取后写回、迁移、覆盖、移动或删除任何真实 cfg；
- 未安装 Mod、未启动游戏、未修改 installed artifact；
- 未修改 Workshop item、订阅内容、staging 或 `mod_id.txt`；
- 未 stage、commit、push 或 tag；
- 未清理 `work/`、旧配置或备份；
- 未修复 Regret/Bad Luck/Beckon direct HP-loss HUD 已知行为缺陷；
- 未改 repository root 目录名；
- runtime/log 证据等级仍为未执行；C1 结论仅为 code/build/contract/publish-tree verified。

### 19.5 Rollback 与下一停点

C1 rollback 仍为普通文件名和符号 mapping 的精确逆向，不需要且不得使用 destructive Git 命令；真实配置和安装未变化，因此不存在数据 rollback 动作。

C1 当时的停点为 `C1 Complete / C2 Approval Pending`；其后用户已单独批准并完成 C2，后续 Gate 结果见第 20.5 节。

---

## 20. C2 execution record（2026-07-22）

### 20.1 授权、版本与基线

用户授权文本：`开始c2`。按第 9.8 节将该授权解释为确认 migration release `v0.3.0`。

执行前及执行后只读复核一致：

- 游戏进程为 0；active Mod 仍仅为已安装的 `damage-forecast` v0.2.0；
- installed DLL/manifest SHA256 仍为 `FB863FF75EA33D6B3F7F9B1471E12E33D5FEBD75595EB680A2D0A49369FDE880` / `09D4DC4EFA54BAA11AB3B3B1E2660FF8DB15D5EA153103AB837AAD22DBEB68C5`；
- 真实 `STS2PartyWatch.cfg` 仍为 797 bytes、18 ordered keys、SHA256 `783C0B880EB1851D813801A6FD35F4D266AE4A2FE69BE50C4812FC7BEA714553`；
- 真实 `DamageForecast.cfg` 仍不存在；Workshop 未扫描写入、未修改。

### 20.2 实际实现

- 当前 persistence identity 改为 `DamageForecast.Settings.DamageForecastBaseLibConfig` / `DamageForecast.cfg` / `EnableDamageForecastHud`，manifest 与 `MainFile.ModVersion` 改为 `v0.3.0`；
- 新增隔离的 `src/DamageForecast/Compatibility/**`：legacy/current schema descriptor、registry、strict UTF-8/JSON typed detector、18 项 snapshot/schema、old→current/current→old pure transform、transaction、marker、recovery、bootstrap 和结构化结果；
- old filename/key/mod-id 集中在 `LegacyIdentityDescriptor`；普通 Settings/UI/Combat/Forecast/Patches 不引用 old cfg/key；
- bootstrap 严格位于新 BaseLib config 第一次构造和唯一一次 `ModConfigRegistry.Register(...)` 之前；invalid/conflict/salvaged 状态 fail-closed；
- old-only 执行 CreateNew backup + flush + SHA verification + same-volume temp + atomic move + final typed verification + marker；old source不删除；
- completed lineage 后只验证 current schema，允许用户后续修改 current cfg，且不再解析 old 内容；
- installer 增加 C3 rollback 只读 config Plan 字段和经 Plan SHA 绑定的 new→old reverse-sync；临时 fixture 中验证 backup、flush、temp、semantic verify、atomic replace；
- 新增 `post-g6-persistence-contract.json`，保留 G6/C1 JSON 为历史证据，不回写其旧结论；
- 官方 old fixture 机械规范化为与真实基线完全一致的 797-byte/相同 SHA256 文件。

新增 production 文件：

- `src/DamageForecast/Compatibility/CompatibilityBootstrap.cs`
- `src/DamageForecast/Compatibility/ConfigMigrationPipeline.cs`
- `src/DamageForecast/Compatibility/ConfigMigrationResult.cs`
- `src/DamageForecast/Compatibility/ConfigMigrationTransaction.cs`
- `src/DamageForecast/Compatibility/ConfigSchemaDetector.cs`
- `src/DamageForecast/Compatibility/ConfigSchemaRegistry.cs`
- `src/DamageForecast/Compatibility/DamageForecastSchemaV1.cs`
- `src/DamageForecast/Compatibility/LegacyIdentityDescriptor.cs`
- `src/DamageForecast/Compatibility/PreDamageForecastSchemaV1.cs`
- `src/DamageForecast/Settings/DamageForecastConfigSchema.cs`

主要 current/installer 更新：

- `src/DamageForecast/MainFile.cs`
- `src/DamageForecast/Settings/DamageForecastBaseLibConfig.cs`
- `src/DamageForecast/Settings/DamageForecastConfigText.cs`
- `src/DamageForecast/Settings/DamageForecastSettingsAdapter.cs`
- `src/DamageForecast/Patches/DamageForecastBaseLibTitlePatch.cs`
- `src/DamageForecast/damage-forecast.json`
- `scripts/Install-LocalMod.ps1`
- `scripts/Test-IdentityPublishTrees.ps1`

新增/更新 C2 tests/contracts/fixtures：

- `tests/DamageForecast.ContractTests/ConfigMigrationContractCases.cs`
- `tests/DamageForecast.ContractTests/PostG6PersistenceContractFixture.cs`
- `tests/DamageForecast.ContractTests/post-g6-persistence-contract.json`
- `tests/DamageForecast.ContractTests/fixtures/config-old-official-v0.2.0.cfg`
- `tests/DamageForecast.ContractTests/fixtures/config-new-default.cfg`
- `tests/DamageForecast.ContractTests/fixtures/config-new-nondefault.cfg`
- `tests/DamageForecast.ContractTests/fixtures/config-invalid-json.cfg`
- `tests/DamageForecast.ContractTests/fixtures/config-truncated.cfg`
- `Program.cs` 及 G6/C1 identity/publish/upgrade contracts 增加 C2 current overlay，不修改历史 `identity-contract.json` 与 `post-g6-naming-contract.json`。

### 20.3 Contract 与失败注入证据

最终 contract count：每个目标 `215 discovered / 215 passed / 0 failed / 0 skipped`。C2 cases 覆盖：

- default/nondefault/真实 797-byte official fixture；唯一 key rename、其余 17 identity mapping、enum/bool/finite float/RGBA/order/typed digest；
- old-only、new-only、both-equal、both-divergent、neither、direct skip、marker missing、target missing、backup/temp interrupted、幂等和用户修改 current 后重启；
- invalid/truncated/missing/extra/duplicate/bool/enum/NaN/Infinity/color/UTF-8/source race；unknown extension bag 保全且不标 completed；
- backup create/flush、temp write、target move/marker/cleanup/reverse、disk-full/access-denied/source-read/target-race failure injection；
- fixed seed 10,000 次合法 old→current→old typed round-trip，覆盖全部 enum 与 bool boundary；
- future 18+N schema 显式默认值、所有 supported source 直达 current、root containment 和 registration ordering；
- 临时 installer rollback 完整 reverse-sync；未在真实 AppData/游戏目录执行。

### 20.4 stable/beta 与产物

- `scripts/Test-ForecastGuardrails.ps1 -Target all`：stable/beta 各 215/215，Release build 均 0 warnings / 0 errors；quality gate PASS；
- `scripts/Build-DualTargets.ps1` 与 publish validator：stable/beta 均 exact two files，manifest `v0.3.0`，两侧产物完全相同；
- stable/beta `damage-forecast.dll` SHA256：`EC22C91A20BE88E2B39D5285769771B5999556534FCEDE5EF000E1A82FB0EE43`；
- stable/beta `damage-forecast.json` SHA256：`FF8D4E07E574F9FC89EDEDF0D569EE8A7CADFE2A6A2907CAA9E3097F476C32DB`；
- `git diff --check` 通过；tracked/working forbidden artifact review 为 0；
- 当前 `git diff --stat` 为 `48 files changed, 742 insertions(+), 5563 deletions(-)`；因 G6 后 renamed tree 仍是 untracked、旧 tree 是 deleted，该标准 stat 不包含 untracked C1/C2 文件，必须与 `git status --short`（55 entries，其中 7 个 untracked roots/files）一并解释。

### 20.5 明确未触碰、证据等级与 rollback

- 未写入、移动、删除真实 old/new cfg；未创建真实 migration backup/marker；
- 未安装 Mod、未启动游戏、未修改 installed artifact；
- 未修改 Workshop、未 stage/commit/push/tag、未清理真实或用户 work；
- 未夹带 Regret/Bad Luck/Beckon 行为修复；未改 repository root；
- C2 证据等级为 code/contract/build/publish-tree verified；真实 migration、BaseLib page、完整重启、日志、rollback/re-upgrade 均仍需 C3，不能标为 runtime-verified；
- 源码 rollback 是逆向 current config identity 并移除整个 compatibility entry；真实数据无变化，因此当前无数据 rollback 动作；installer reverse-sync 只在临时 fixture 验证。

第 20 节完成时的停点为 `C2 Complete / C3A Approval Pending`；其后 C3A 与 C3B-Prep 已完成，后续 Gate 结果见第 21.6 节。

---

## 21. C3A 与 C3B-Prep execution record（2026-07-22）

### 21.1 授权与只读 C3A 基线

用户授权文本依次为：`开始c3 a`、`开始 C3B-Prep`。

C3A 只读绑定：

- 游戏进程 0；唯一 active identity 为 `damage-forecast` v0.2.0；无 legacy/duplicate/orphan identity；
- active DLL：135168 bytes，SHA256 `FB863FF75EA33D6B3F7F9B1471E12E33D5FEBD75595EB680A2D0A49369FDE880`；
- active manifest：371 bytes，SHA256 `09D4DC4EFA54BAA11AB3B3B1E2660FF8DB15D5EA153103AB837AAD22DBEB68C5`；
- staging v0.3.0 DLL：202752 bytes，SHA256 `EC22C91A20BE88E2B39D5285769771B5999556534FCEDE5EF000E1A82FB0EE43`；
- staging manifest：371 bytes，SHA256 `FF8D4E07E574F9FC89EDEDF0D569EE8A7CADFE2A6A2907CAA9E3097F476C32DB`；
- old cfg：797 bytes、18 项，SHA256 `783C0B880EB1851D813801A6FD35F4D266AE4A2FE69BE50C4812FC7BEA714553`；new cfg 不存在；
- `.damage-forecast-backups` 与 AppData migration root 均不存在；
- Workshop 扫描中 Damage Forecast identity 为 0；BaseLib v3.3.8 存在，DLL SHA256 `E92213E9286CB8CB9DB42B83735CC9DDC2D642A7C90C67C5350C983D734407A8`；
- `godot.log` baseline：1224259 bytes，UTC `2026-07-22T10:09:14.4347304Z`，SHA256 `4C8D0EB8FE437A47D0038CAB74B8190728DE20BF1A3DF243DD007E37B91574EE`。

首次 C3A Plan 暴露执行工具缺口：安装器将已使用 target slug 的 v0.2.0 误判为 `target-already-active`，未生成 backup path，且 rollback 只接受旧 slug。C3A 因此安全停止，未执行安装。

### 21.2 C3B-Prep 实现

仅修改 installer 与临时 contract fixtures：

- `scripts/Install-LocalMod.ps1` Plan schema 升为 2，新增 `PlanOperation=Install|Rollback`；
- Plan 同时绑定 staging、active、rollback backup 的 ID、version、manifest SHA256、DLL SHA256；
- active target 与 staging 不同则产生 `target-upgrade`，相同则 `target-already-current` 并拒绝 no-op replacement；
- 为同 identity active target 生成 Loader root 外的 `plannedActiveBackupPath`；
- Install execute 要求 Plan staging/active 四个 SHA256 全部一致，使用 mods-root sibling staging，先移动 previous active 到恢复备份，再 rename 激活 target；
- post-install 验证恰好一个 target identity、exact two files、version 和两个 SHA256；失败时将 failed target 移到备份根并恢复 previous active；
- Rollback Plan 读取精确 backup path，绑定 current active 与 backup 四个 SHA256；支持 `rollback-target` 和历史 `rollback-legacy`；
- 同 identity rollback 恢复到 `mods/damage-forecast`，历史旧 slug 仍恢复到 `mods/sts2-party-watch-v2`；两者都在移动前执行必要的 config reverse-sync，并验证恢复后 version/hash；
- re-upgrade 复用同一 `target-upgrade` 事务；无 delete/overwrite cleanup 分支。

### 21.3 临时 fixture 验证

- 原有 clean install、legacy→target upgrade、legacy rollback、路径/Workshop/execute/hash guards 保持通过；
- 新增 `IU-009`：同 identity v0.2.0→v0.3.0→v0.2.0→v0.3.0 全链，每一步 Loader root 仅一个 `damage-forecast` identity；
- `C2-018` 改为真实拓扑：target v0.3.0 + target v0.2.0 backup，在 rollback 前执行经 old/new cfg SHA 绑定的 reverse-sync；
- stable/beta 各 `216 discovered / 216 passed / 0 failed / 0 skipped`；Release build 均 0 warnings / 0 errors；quality gate PASS；
- production source/manifest 未修改，staging v0.3.0 artifact SHA256 保持第 21.1 节值；
- `git diff --check` 通过。

### 21.4 Prep 后刷新 C3B Plan

最终只读 Plan：

- schema/action：`2 / target-upgrade`；
- transaction ID：`C3B-20260722T125054124Z`；
- active：`damage-forecast` v0.2.0，哈希见第 21.1 节；
- staging：`damage-forecast` v0.3.0，哈希见第 21.1 节；
- planned active backup：`C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\.damage-forecast-backups\C3B-20260722T125054124Z-damage-forecast-v0.2.0`；
- target install：`C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\damage-forecast`；
- Workshop identity count 0；game running false；legacy/target count `0/1`；orphan count 0；
- old cfg SHA 已绑定；new cfg 不存在；config rollback action 在 install 阶段为 `not-applicable`。

Plan 后再次只读复核：active/old cfg 哈希未变；new cfg、backup root、migration root 仍不存在；游戏进程仍为 0。

### 21.5 获批 C3B 后的精确事务

只允许使用第 21.4 节 transaction/hash 执行：

1. 再次确认游戏关闭、Workshop identity 0、active/staging/old cfg 哈希未变；
2. 创建 Loader root 外的 backup root；
3. 在 mods root 下创建同卷 staging，只复制并验证 v0.3.0 DLL/manifest；
4. 将 active v0.2.0 整目录移动到 planned active backup；
5. rename staging 为 `mods/damage-forecast`；
6. 验证唯一 target、version v0.3.0、exact two files 和已批准哈希；
7. 写 recoverable install ledger；
8. 停止并报告 active/backup/config 状态；Codex 不启动游戏；
9. 用户启动游戏，compatibility bootstrap 才创建独立 AppData backup、new cfg 和 completed marker；
10. 首次 runtime 后只读核对新日志/cfg/marker，并由用户确认一个配置页、HUD 和无重复。

### 21.6 当时 Gate snapshot

当时 Gate snapshot：`C3B-Prep Complete / C3B Approval Pending`。

未安装 Mod、未写真实 cfg、未创建真实 backup/marker、未启动游戏、未修改 Workshop、未执行 cleanup 或 Git 发布。只有用户单独说 `开始 C3B` 后，才可执行第 21.5 节；C3C rollback/re-upgrade 仍需再次单独批准。

上述为 C3B-Prep 当时停点；用户随后已明确授权 `开始 C3B`，执行结果见第 22 节。

---

## 22. C3B install execution record（2026-07-22）

### 22.1 授权与执行前重放

用户授权文本：`开始 C3B`。

执行前严格重放第 21.4 节既有 Plan，未生成新事务或替换绑定值。复核结果：

- game running false；Workshop Damage Forecast identity count 0；legacy/target/orphan count `0/1/0`；
- action 为 `target-upgrade`，transaction ID 为 `C3B-20260722T125054124Z`；
- active v0.2.0 manifest/DLL SHA256 分别为 `09D4DC4EFA54BAA11AB3B3B1E2660FF8DB15D5EA153103AB837AAD22DBEB68C5` / `FB863FF75EA33D6B3F7F9B1471E12E33D5FEBD75595EB680A2D0A49369FDE880`；
- staging v0.3.0 manifest/DLL SHA256 分别为 `FF8D4E07E574F9FC89EDEDF0D569EE8A7CADFE2A6A2907CAA9E3097F476C32DB` / `EC22C91A20BE88E2B39D5285769771B5999556534FCEDE5EF000E1A82FB0EE43`；
- old cfg SHA256 仍为 `783C0B880EB1851D813801A6FD35F4D266AE4A2FE69BE50C4812FC7BEA714553`，new cfg 不存在。

### 22.2 真实安装结果

安装器以 `-Mode Install -Execute` 和上述四个 artifact SHA256 执行成功：

- active 路径：`C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\damage-forecast`；
- active identity/version：唯一 `damage-forecast` v0.3.0；
- active exact two files：manifest 371 bytes、SHA256 `FF8D4E07E574F9FC89EDEDF0D569EE8A7CADFE2A6A2907CAA9E3097F476C32DB`；DLL 202752 bytes、SHA256 `EC22C91A20BE88E2B39D5285769771B5999556534FCEDE5EF000E1A82FB0EE43`；
- previous active backup：`C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\.damage-forecast-backups\C3B-20260722T125054124Z-damage-forecast-v0.2.0`；
- backup exact two files：manifest 371 bytes、SHA256 `09D4DC4EFA54BAA11AB3B3B1E2660FF8DB15D5EA153103AB837AAD22DBEB68C5`；DLL 135168 bytes、SHA256 `FB863FF75EA33D6B3F7F9B1471E12E33D5FEBD75595EB680A2D0A49369FDE880`；
- install ledger：`C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\.damage-forecast-backups\C3B-20260722T125054124Z-install-ledger.json`；schema 2、action `target-upgrade`、`createdAtUtc=2026-07-22T12:55:34.6755966Z`，且 staging hashes 与 previous-active backup path 均匹配；
- transaction 临时目录残留为 0。

### 22.3 安装后、首次运行前的配置与日志边界

- 游戏进程仍为 0，Codex 未启动游戏；
- old `STS2PartyWatch.cfg` 仍为 797 bytes、SHA256 `783C0B880EB1851D813801A6FD35F4D266AE4A2FE69BE50C4812FC7BEA714553`；
- `DamageForecast.cfg` 仍不存在，AppData migration root 仍不存在；这正是 bootstrap 尚未运行的预期状态，不能标为 migration success；
- `godot.log` 仍为 1224259 bytes、UTC `2026-07-22T10:09:14.4347304Z`、SHA256 `4C8D0EB8FE437A47D0038CAB74B8190728DE20BF1A3DF243DD007E37B91574EE`，证明尚无本次 runtime 证据；
- Workshop 未修改；未执行 cleanup、stage/commit/push/tag 或 Git 发布。

### 22.4 当时 Gate snapshot

当时状态：`C3B Install Complete / First Runtime Pending`。

下一步只由用户手动启动游戏：等待 Mod 加载到主菜单，确认恰好一个 Damage Forecast 配置页；方便时进入一次战斗观察 HUD，然后正常退出。用户退出后，Codex只读核验 fresh `godot.log`、new cfg、migration backup/marker、单页/无重复与 HUD 反馈。不要在首次运行步骤特意改变设置值；C3C 的设置写入、真实 rollback 与 re-upgrade 仍须用户再次单独批准。

### 22.5 首次 runtime 与 migration 证据

用户于 v0.3.0 首次运行后回复“成功的”，并正常退出游戏。随后只读核验：

- 游戏进程为 0；fresh `godot.log` 为 197786 bytes、UTC `2026-07-22T12:59:01.5970451Z`、SHA256 `9E5EB84CE3838967A14909F6F8CCB209482CF77153CE0D18DE756623F733551C`；
- 日志发现唯一 `mods/damage-forecast/damage-forecast.json`，加载 `damage-forecast.dll`，调用 `DamageForecast.MainFile`，BaseLib 注册 `damage-forecast`，并完成 `Damage Forecast (v0.3.0)` 初始化；
- new `DamageForecast.cfg` 已生成：801 bytes、18 keys、SHA256 `FDED35CCC1B1EC2CFFFEBF0B5EDC2FF0E228F8ADF5B1B647C9BF0B82F752830B`；
- old/new semantic comparison 为 0 mismatch；唯一 key identity 变化为 `EnablePartyWatchHud`→`EnableDamageForecastHud`，其余 17 项名称和值保持一致；
- transaction `20260722T125822561Z-b291524099f1413abe9e554af74ca12f` marker status 为 `completed`，source/target schema 为 `pre-damage-forecast-v1`→`damage-forecast-v1`，strategy 为 `old-v1-to-current-v1`，mod version 为 v0.3.0；
- AppData backup 为 797 bytes、SHA256 `783C0B880EB1851D813801A6FD35F4D266AE4A2FE69BE50C4812FC7BEA714553`，与保留的 old cfg 完全一致；marker target SHA256 与 new cfg 一致；
- 日志未发现 Damage Forecast 初始化或 migration 异常；可见 InputMap 和退出时资源释放提示未归因于本 Mod。

因此 C3B 的安装、首次启动、单 identity/BaseLib 注册和配置迁移已获得 runtime evidence。设置写入、rollback/re-upgrade 尚未执行，不属于本结论。

### 22.6 新发现：damage-dealing Status/Curse card HUD failure

同一次 v0.3.0 runtime 中，用户修正了此前的窄归因：问题并非仅由霉运（Bad Luck）和悔恨（Regret）触发；当前观察是，手牌中所有会造成伤害的状态牌或诅咒牌都会使完整 HUD 失效，而除此之外场景正常。

证据边界：

- 这是当前 v0.3.0 的用户 runtime observation，足以否定“仅 Regret/Bad Luck”这一窄范围；
- 尚未取得逐卡名称、截图/存档或逐卡进入/移除矩阵，因此不把“所有”提升为已完成穷举验证；
- 旧的 strict classifier ordering 仍是 diagnosis candidate，但现有证据不足以确认它能解释整个扩大后的范围；
- 历史 Burn、Beckon、Bad Luck、Regret runtime 证据保留为历史证据，不冒充当前版本复验；
- 不将该行为缺陷归因为 Post-G6 identity/config migration，也不在 C3B/C3C 中夹带修复。

当时 Gate snapshot：`C3B First Runtime Verified / HUD Defect Recorded / C3C Approval Pending`。C3C 与独立 HUD diagnosis/fix 均须用户单独批准。

---

## 23. C3C execution record（2026-07-22）

### 23.1 授权与初始只读预检

用户授权文本：`开始 c3c`。本 Gate 仅覆盖 v0.3 设置持久化、完整重启、v0.3→v0.2 rollback/reverse-sync、旧版启动确认、v0.2→v0.3 re-upgrade 和第二次完整重启；不覆盖 C3D cleanup、C4、Workshop、Git 或 HUD behavior fix。

初始预检结果：

- 游戏进程为 0；Workshop Damage Forecast identity count 0；legacy/target/orphan count `0/1/0`；
- active `damage-forecast` v0.3.0 exact two files：manifest SHA256 `FF8D4E07E574F9FC89EDEDF0D569EE8A7CADFE2A6A2907CAA9E3097F476C32DB`，DLL SHA256 `EC22C91A20BE88E2B39D5285769771B5999556534FCEDE5EF000E1A82FB0EE43`；
- rollback backup `C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\.damage-forecast-backups\C3B-20260722T125054124Z-damage-forecast-v0.2.0` exact two files：manifest SHA256 `09D4DC4EFA54BAA11AB3B3B1E2660FF8DB15D5EA153103AB837AAD22DBEB68C5`，DLL SHA256 `FB863FF75EA33D6B3F7F9B1471E12E33D5FEBD75595EB680A2D0A49369FDE880`；
- old cfg 797 bytes、SHA256 `783C0B880EB1851D813801A6FD35F4D266AE4A2FE69BE50C4812FC7BEA714553`；new cfg 801 bytes、SHA256 `FDED35CCC1B1EC2CFFFEBF0B5EDC2FF0E228F8ADF5B1B647C9BF0B82F752830B`；两者当前 semantic digest 相同；
- migration marker status `completed`、reverse status `not-requested`，marker target SHA 与 new cfg 一致；
- rollback Plan schema/action 为 `2 / rollback-target`，transaction ID `C3C-RB-20260722T132358013Z`；当前 config action 为 `legacy-direct-semantically-equal`。用户修改 new cfg 后必须使用同一 transaction ID 重新生成并审阅 Plan，预期变为 `reverse-sync-required`，不得复用旧 cfg SHA；
- `ShowAdvancedShieldHeartDetails` 在 old/new cfg 中当前均为 `True`。

### 23.2 当时 Gate snapshot：v0.3 设置写入

当时 Gate snapshot：`C3C In Progress / v0.3 Setting Write Pending`。

用户下一步只启动一次当前 v0.3.0，在唯一 Damage Forecast 配置页将 `ShowAdvancedShieldHeartDetails` 从 `True` 切换为 `False`，不改变其他设置，然后正常退出并回复。Codex 随后只读核验 new cfg 唯一值变化和 fresh log；在该证据通过前不得执行 rollback。

### 23.3 v0.3 设置写入证据与重启停点

用户完成设置修改并正常退出后，只读核验通过：

- 游戏进程为 0；本次日志为 66498 bytes、UTC `2026-07-22T13:30:01.3121010Z`、SHA256 `D2B1433E096DC73A89FBD1BA368DB0C6783B7C9A895A21EBC6AEC2FC66165E69`；
- 日志仅发现 active `damage-forecast` manifest，调用 `DamageForecast.MainFile`、BaseLib 注册 `damage-forecast`、完成 `Damage Forecast (v0.3.0)` 初始化，并在正常退出时保存全部 ModConfigs；
- old cfg 保持 797 bytes、SHA256 `783C0B880EB1851D813801A6FD35F4D266AE4A2FE69BE50C4812FC7BEA714553`，时间戳未变；
- new cfg 为 802 bytes、18 keys、SHA256 `B2DB575CF81C8562B65FE85F387E226D33844C97855F51698CFAB12F37AE04F8`；
- old/new 按唯一 identity key mapping 比较后恰好 1 项值差异：`ShowAdvancedShieldHeartDetails` 为 old `True`、new `False`；没有其他 key/value 漂移。

当时 Gate snapshot：`C3C In Progress / v0.3 Restart Persistence Pending`。用户下一步完整重启当前 v0.3.0，确认唯一配置页中的高级明细仍为关闭，不改变任何设置，正常退出。Codex 复核 config hash 和 fresh log 后，才可用同一 transaction ID `C3C-RB-20260722T132358013Z` 生成最终 `reverse-sync-required` rollback Plan。

### 23.4 v0.3 完整重启保持性

用户确认完整重启后高级明细仍为关闭；只读复核通过：

- 游戏进程为 0；new cfg 仍为 802 bytes、18 keys、SHA256 `B2DB575CF81C8562B65FE85F387E226D33844C97855F51698CFAB12F37AE04F8`，`ShowAdvancedShieldHeartDetails=False`；
- old cfg 仍为原始 SHA256 `783C0B880EB1851D813801A6FD35F4D266AE4A2FE69BE50C4812FC7BEA714553`，尚未提前 reverse sync；
- fresh log 为 64532 bytes、UTC `2026-07-22T13:32:10.8754315Z`、SHA256 `4BDC8D814C6664A1075C4101777CB6F3157378801497A7CD19041944D722E1F4`；只发现一次 v0.3.0 manifest/initializer/BaseLib registration/finished initialization，并在退出时保存 ModConfigs。

同一 transaction ID `C3C-RB-20260722T132358013Z` 的最终 rollback Plan 已重放：schema/action `2 / rollback-target`，game false，Workshop identity 0，active/backup 四个 artifact SHA256 与第 23.1 节一致，legacy/current cfg SHA256 分别为 `783C0B880EB1851D813801A6FD35F4D266AE4A2FE69BE50C4812FC7BEA714553` / `B2DB575CF81C8562B65FE85F387E226D33844C97855F51698CFAB12F37AE04F8`，config action 正确变为 `reverse-sync-required`。

### 23.5 v0.3→v0.2 rollback 与 reverse sync

经最终 Plan 哈希绑定执行成功：

- current active 为唯一 `damage-forecast` v0.2.0，exact two files：manifest SHA256 `09D4DC4EFA54BAA11AB3B3B1E2660FF8DB15D5EA153103AB837AAD22DBEB68C5`，DLL SHA256 `FB863FF75EA33D6B3F7F9B1471E12E33D5FEBD75595EB680A2D0A49369FDE880`；
- v0.3.0 recovery：`C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\.damage-forecast-backups\C3C-RB-20260722T132358013Z-target-before-rollback`，manifest/DLL SHA256 分别为 `FF8D4E07E574F9FC89EDEDF0D569EE8A7CADFE2A6A2907CAA9E3097F476C32DB` / `EC22C91A20BE88E2B39D5285769771B5999556534FCEDE5EF000E1A82FB0EE43`；
- rollback ledger：`C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\.damage-forecast-backups\C3C-RB-20260722T132358013Z-rollback-ledger.json`，schema/action `2 / rollback-target`，createdAtUtc `2026-07-22T13:33:38.1505686Z`，restored backup ID `damage-forecast`；
- reverse-synced old cfg 为 798 bytes、SHA256 `5697D2BB9AD6DF4E559FD213ABD93C1A7CDD4EF54A2E837D825099A52833C775`，`ShowAdvancedShieldHeartDetails=False`；与 new cfg 的 18 项 semantic diff 为 0；
- reverse transaction 中 `STS2PartyWatch.cfg.backup` 与 `STS2PartyWatch.cfg.replace-backup` 均为原 797-byte cfg、SHA256 `783C0B880EB1851D813801A6FD35F4D266AE4A2FE69BE50C4812FC7BEA714553`；reverse temp count 0；
- 游戏进程仍为 0，Codex 未启动游戏；未执行 cleanup、Workshop 或 Git 操作。

当时 Gate snapshot：`C3C In Progress / v0.2 Runtime Verification Pending`。用户下一步只启动一次 v0.2.0，确认唯一配置页读取到高级明细关闭，不修改设置并正常退出；通过后才可生成 re-upgrade Plan。

### 23.6 v0.2 runtime 读取证据

用户确认 v0.2.0 配置页中的高级明细依然为关闭，未修改任何设置。只读复核通过：

- 游戏进程为 0；old cfg 仍为 798 bytes、SHA256 `5697D2BB9AD6DF4E559FD213ABD93C1A7CDD4EF54A2E837D825099A52833C775`，`ShowAdvancedShieldHeartDetails=False`；
- new cfg 保持 802 bytes、SHA256 `B2DB575CF81C8562B65FE85F387E226D33844C97855F51698CFAB12F37AE04F8`，同一设置为 `False`；
- fresh log 为 281450 bytes、UTC `2026-07-22T13:44:51.5268612Z`、SHA256 `E375054E56FC36E755156883333C3B965CE16E5FDD96D29FEF3D87269160495C`；只发现一次 `damage-forecast` manifest、`DamageForecast.MainFile`、BaseLib registration 和完成初始化，Mod list 明确为 `Damage Forecast [damage-forecast] (v0.2.0)`，退出时正常保存 ModConfigs。

因此 v0.3 new cfg 的唯一变更已通过 reverse sync 被 v0.2 old config identity 实际读取，旧版 runtime 验证通过。

### 23.7 v0.2→v0.3 re-upgrade

以 v0.3 recovery 目录作为已核验 staging 生成 Plan：

- schema/action `2 / target-upgrade`，transaction ID `C3C-UP-20260722T134526992Z`；
- game false、Workshop identity 0、legacy/target/orphan `0/1/0`；
- staging v0.3 manifest/DLL SHA256 为 `FF8D4E07E574F9FC89EDEDF0D569EE8A7CADFE2A6A2907CAA9E3097F476C32DB` / `EC22C91A20BE88E2B39D5285769771B5999556534FCEDE5EF000E1A82FB0EE43`；
- active v0.2 manifest/DLL SHA256 为 `09D4DC4EFA54BAA11AB3B3B1E2660FF8DB15D5EA153103AB837AAD22DBEB68C5` / `FB863FF75EA33D6B3F7F9B1471E12E33D5FEBD75595EB680A2D0A49369FDE880`；
- planned v0.2 backup：`C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\.damage-forecast-backups\C3C-UP-20260722T134526992Z-damage-forecast-v0.2.0`。

经四个 artifact SHA256 绑定执行成功，安装后复核：

- active 为唯一 `damage-forecast` v0.3.0，exact two files 且哈希与 staging 完全一致；
- 新 v0.2 backup exact two files 且哈希与执行前 active 完全一致；原 v0.3 recovery 仍完整保留；
- install ledger `C3C-UP-20260722T134526992Z-install-ledger.json` 为 schema/action `2 / target-upgrade`，createdAtUtc `2026-07-22T13:45:49.2771039Z`，staging hashes 和 previous active backup path 匹配；
- old/new cfg SHA256 仍分别为 `5697D2BB9AD6DF4E559FD213ABD93C1A7CDD4EF54A2E837D825099A52833C775` / `B2DB575CF81C8562B65FE85F387E226D33844C97855F51698CFAB12F37AE04F8`，两者高级明细均为 `False`；
- Loader-visible Damage Forecast identity count 1，install temp count 0，游戏进程 0。

当时 Gate snapshot：`C3C In Progress / Final v0.3 Restart Pending`。用户下一步完成第二次 v0.3.0 完整重启，确认唯一配置页、设置保持以及普通场景 HUD 无重复；退出后 Codex 只读核验 fresh log/cfg/active identity 并关闭 C3C。

### 23.8 最终 v0.3 restart 与 C3C closure

用户确认 re-upgrade 后的 v0.3.0 最终完整重启正常：高级明细仍为关闭、只有一个 Damage Forecast 配置页，普通无伤害状态/诅咒牌场景 HUD 正常且没有重复。最终只读复核：

- 游戏进程为 0；Loader-visible Damage Forecast identity count 1，active 为 `damage-forecast` v0.3.0；
- active exact two files：manifest 371 bytes、SHA256 `FF8D4E07E574F9FC89EDEDF0D569EE8A7CADFE2A6A2907CAA9E3097F476C32DB`；DLL 202752 bytes、SHA256 `EC22C91A20BE88E2B39D5285769771B5999556534FCEDE5EF000E1A82FB0EE43`；
- old cfg 为 798 bytes、SHA256 `5697D2BB9AD6DF4E559FD213ABD93C1A7CDD4EF54A2E837D825099A52833C775`；new cfg 为 802 bytes、SHA256 `B2DB575CF81C8562B65FE85F387E226D33844C97855F51698CFAB12F37AE04F8`；两者均为 18 keys、`ShowAdvancedShieldHeartDetails=False`，semantic diff count 0；
- fresh log 为 91590 bytes、UTC `2026-07-22T13:48:14.5650921Z`、SHA256 `7DABCA6A1ED2BB8C2A5DEE9C62D04F14E7EFA70ABD704234AF52B5B9ECB74E8E`；
- 日志中 `damage-forecast` manifest、`DamageForecast.MainFile` initializer、BaseLib `damage-forecast` registration、完成初始化、`Damage Forecast [damage-forecast] (v0.3.0)` 和正常退出保存 ModConfigs 均恰好 1 次；Damage Forecast attributable error count 0；
- install temp count 0；rollback/re-upgrade recovery directories、config reverse backups 和 transaction ledgers 保留给 C3D 审核，未清理；
- Workshop 未修改；未执行 cleanup、stage/commit/push/tag；已记录的 damage-dealing Status/Curse HUD defect 未修改，也未作为 identity/config regression。

C3C stable runtime 全链通过：v0.3 setting write→完整重启保持→v0.2 reverse-sync rollback→v0.2 实际读取→v0.3 re-upgrade→第二次完整重启，且全程 single identity/page/load、配置 18 项连续、恢复材料完整。

当时 Gate snapshot：`C3C Complete / C3BR Approval Pending`。`C3BR` 是本任务中 C3 beta runtime 的约定简称；只有用户单独批准后才能开始。C3D cleanup、C4、Workshop、Git 与 HUD behavior fix 仍分别 gated。

---

## 24. C3BR beta runtime execution record（2026-07-22）

### 24.1 授权、scope 与环境预检

用户授权文本：`开始 C3BR`。本简称只覆盖第 13.4 节 beta v0.109.0 contract/build/publish、matching artifact、completed migration/config continuity、one-load/one-page/one-HUD 与 18 项 cfg smoke；不覆盖 C3D、C4、Workshop、Git、配置清理或 HUD behavior fix。

初始环境：

- 游戏进程为 0；当前 Steam appmanifest `BetaKey=public`、buildid `23811903`；
- 当前 `release_info.json` 为 version/branch `v0.107.1`、commit `59260271`，因此真实安装仍是 stable，尚不能产生本次 beta runtime 证据；
- frozen beta snapshot 为 version/branch `v0.109.0`、commit `c12f634d`；
- 当前 active 仍为唯一 `damage-forecast` v0.3.0，manifest/DLL SHA256 为 `FF8D4E07E574F9FC89EDEDF0D569EE8A7CADFE2A6A2907CAA9E3097F476C32DB` / `EC22C91A20BE88E2B39D5285769771B5999556534FCEDE5EF000E1A82FB0EE43`；
- C3C 后 old/new cfg 均为 18 keys、`ShowAdvancedShieldHeartDetails=False`、semantic diff 0；rollback/re-upgrade/config backups 与 ledgers 保留。

### 24.2 beta contracts/build/publish/matching artifact

- `Test-ForecastGuardrails.ps1 -Target beta`：`216 discovered / 216 passed / 0 failed / 0 skipped`；Release build 0 warnings / 0 errors；git diff check 与 tracked/working forbidden artifact review PASS；single-target quality gate PASS；
- `Build-DualTargets.ps1`：frozen stable v0.107.1 / beta v0.109.0 均 Release build 0 warnings / 0 errors；
- stable/beta publish 均 exact two files、assembly/manifest identity `damage-forecast`、manifest v0.3.0；
- stable/beta artifacts byte-identical：DLL SHA256 `EC22C91A20BE88E2B39D5285769771B5999556534FCEDE5EF000E1A82FB0EE43`，manifest SHA256 `FF8D4E07E574F9FC89EDEDF0D569EE8A7CADFE2A6A2907CAA9E3097F476C32DB`；
- 上述哈希与当前 active 安装完全相同，因此 C3BR 不需要也不得执行 no-op reinstall。

### 24.3 beta config smoke 证据边界

C3B 已在 stable 对真实 old-only 状态完成一次 first migration；C3C 又完成 new→old reverse sync、v0.2 实际读取和 re-upgrade。因此 C3BR 不破坏已验收状态去伪造第二次“首次迁移”，而是在 beta 上验证同一 matching artifact 的 completed-lineage/idempotent 路径：

- new `DamageForecast.cfg` 仍为 18 项且值保持；
- completed migration marker 与 backup 保持有效；
- 普通启动不重新覆盖 current cfg、不重复 migration/page/load/HUD；
- fresh beta log 能把实际加载归因到 v0.109.0 上的当前 artifact。

这属于第 13.4 节的 beta config migration smoke 深度，不替代 C3B stable first-migration evidence，也不扩张为第二套 destructive full migration matrix。

当时 Gate snapshot：`C3BR In Progress / Steam Beta Switch Pending`。用户下一步只切换 Steam beta 并等待更新，不启动游戏；Codex 随后只读绑定 release_info、active artifact、cfg/marker 和日志 baseline。

### 24.4 beta switch 后只读绑定

用户切换完成且未启动游戏；只读核验：

- `release_info.json`：version/branch `v0.109.0`、commit `c12f634d`、main assembly hash `1833084275`；
- Steam appmanifest：`BetaKey=public-beta`、buildid `24251656`、StateFlags 4、TargetBuildID 0；游戏进程 0；
- Loader-visible Damage Forecast identity count 1：`damage-forecast` v0.3.0；active manifest/DLL SHA256 仍为 `FF8D4E07E574F9FC89EDEDF0D569EE8A7CADFE2A6A2907CAA9E3097F476C32DB` / `EC22C91A20BE88E2B39D5285769771B5999556534FCEDE5EF000E1A82FB0EE43`，与 beta publish matching artifact 完全一致；
- old cfg：798 bytes、18 keys、SHA256 `5697D2BB9AD6DF4E559FD213ABD93C1A7CDD4EF54A2E837D825099A52833C775`、高级明细 `False`；new cfg：802 bytes、18 keys、SHA256 `B2DB575CF81C8562B65FE85F387E226D33844C97855F51698CFAB12F37AE04F8`、高级明细 `False`；
- first-migration marker 仍为 `completed` / mod v0.3.0，保留 source SHA `783C0B880EB1851D813801A6FD35F4D266AE4A2FE69BE50C4812FC7BEA714553` 和首次 target SHA `FDED35CCC1B1EC2CFFFEBF0B5EDC2FF0E228F8ADF5B1B647C9BF0B82F752830B`；当前 new cfg 后续设置修改产生的 `B2DB575CF81C8562B65FE85F387E226D33844C97855F51698CFAB12F37AE04F8` 不应也不会回写历史 marker；
- Workshop Damage Forecast identity count 0；
- beta 启动前日志 baseline：91590 bytes、UTC `2026-07-22T13:48:14.5650921Z`、SHA256 `7DABCA6A1ED2BB8C2A5DEE9C62D04F14E7EFA70ABD704234AF52B5B9ECB74E8E`。

Steam 分支更新未修改 Mod、配置、marker 或日志。当时 Gate snapshot：`C3BR In Progress / Beta Runtime Smoke Pending`。下一步只由用户启动一次 beta 并完成 one-load/one-page/one-HUD/config continuity smoke；退出后 Codex 检查 fresh beta log 和 cfg/hash。

### 24.5 beta runtime smoke PASS

用户报告 requested beta smoke 一切正常并正常退出。最终 beta 只读证据：

- 游戏进程为 0；环境仍为 version/branch `v0.109.0`、commit `c12f634d`、main assembly hash `1833084275`；
- Loader-visible Damage Forecast identity count 1，active `damage-forecast` v0.3.0 exact two files；manifest/DLL SHA256 仍为 `FF8D4E07E574F9FC89EDEDF0D569EE8A7CADFE2A6A2907CAA9E3097F476C32DB` / `EC22C91A20BE88E2B39D5285769771B5999556534FCEDE5EF000E1A82FB0EE43`；
- 用户确认 Mod list 唯一 v0.3.0、BaseLib 唯一 Damage Forecast 配置页、高级明细保持关闭，普通无伤害状态/诅咒牌场景 HUD 正常且不重复；
- old/new cfg 分别保持 SHA256 `5697D2BB9AD6DF4E559FD213ABD93C1A7CDD4EF54A2E837D825099A52833C775` / `B2DB575CF81C8562B65FE85F387E226D33844C97855F51698CFAB12F37AE04F8`，各 18 keys、高级明细 `False`、semantic diff 0；normal exit 只刷新 new cfg 时间戳；
- first-migration marker 仍为 completed，source/first-target SHA 和 mod version v0.3.0 未变化，证明 beta completed-lineage/idempotent path 未覆盖用户当前配置；
- fresh beta log：45010 bytes、UTC `2026-07-22T15:37:04.0999873Z`、SHA256 `D17C5A7DD9623321FE1B1C93A97410EE4E1CB61861EE545D6A8FD5683DE57366`；
- 日志中 target manifest、`DamageForecast.MainFile` initializer、BaseLib `damage-forecast` registration、完成初始化、`Damage Forecast [damage-forecast] (v0.3.0)` 和退出保存各恰好 1 次；legacy manifest/initializer、legacy-conflict disable 与 attributable error 各 0；
- Workshop Damage Forecast identity count 0；未修改 Workshop、Git、备份或 HUD behavior。

C3BR beta runtime matrix 已实质 PASS。当时 Gate snapshot：`C3BR In Progress / Stable Return Pending`；为恢复 C3C 后的最终环境，用户下一步切回公开 stable v0.107.1、等待完成但不启动，Codex 只读复核后正式关闭 C3BR。

### 24.6 stable return 与 C3BR closure（2026-07-24）

用户切回 stable 且未启动游戏。最终只读复核：

- `release_info.json` 恢复为 version/branch `v0.107.1`、commit `59260271`、main assembly hash `-1555940892`；
- Steam appmanifest 恢复为 `BetaKey=public`、buildid `23811903`、StateFlags 4、TargetBuildID 0；游戏进程 0；
- Loader-visible Damage Forecast identity count 1，active 仍为 `damage-forecast` v0.3.0 exact two files；manifest/DLL SHA256 仍为 `FF8D4E07E574F9FC89EDEDF0D569EE8A7CADFE2A6A2907CAA9E3097F476C32DB` / `EC22C91A20BE88E2B39D5285769771B5999556534FCEDE5EF000E1A82FB0EE43`；
- old/new cfg SHA256 仍为 `5697D2BB9AD6DF4E559FD213ABD93C1A7CDD4EF54A2E837D825099A52833C775` / `B2DB575CF81C8562B65FE85F387E226D33844C97855F51698CFAB12F37AE04F8`，各 18 keys、高级明细 `False`、semantic diff 0；
- first-migration marker 仍为 completed / mod v0.3.0，source/first-target SHA 未变；
- game backup root 保留 5 个顶层项，config migration root 保留 4 个文件，供 C3D 单独审核；
- beta fresh log 仍为 45010 bytes、UTC `2026-07-22T15:37:04.0999873Z`、SHA256 `D17C5A7DD9623321FE1B1C93A97410EE4E1CB61861EE545D6A8FD5683DE57366`，证明 stable return 后没有启动覆盖 beta 证据；
- Workshop Damage Forecast identity count 0；Steam 分支切换未修改 Mod、配置、marker、恢复材料或 Workshop。

C3BR closure：beta v0.109.0 contract/build/publish、matching artifact、completed-migration config continuity、one-load/one-page/one-HUD smoke 和日志归因全部 PASS；最终机器环境已恢复 stable v0.107.1，active 仍为唯一 v0.3.0。

当时 Gate snapshot：`C3BR Complete / C3D Approval Pending`。C3D cleanup、C4、Workshop、Git 与 HUD behavior fix 仍分别 gated。

---

## 25. C3D cleanup execution record（2026-07-24）

### 25.1 授权、依赖核对与精确清单

用户授权文本：`开始 C3D`。本 gate 只覆盖已盘点目标的可恢复清理，不覆盖 C4、Workshop、Git、active Mod/current cfg 变更或 HUD behavior fix。

删除前只读核对确认：

- stable v0.107.1 已恢复且游戏进程为 0；
- active 为唯一 `damage-forecast` v0.3.0，exact two files；
- `CompatibilityBootstrap` 的 completed-lineage 启动路径只读取 completed marker，并校验当前 `DamageForecast.cfg`；marker 中记录的历史 `backupPath` 不会在后续启动被重新读取或校验；
- 因此必须保留 completed marker 与 new cfg，但可以清理它引用的旧配置备份；
- 所有 repo work 目标均位于 `work` 下，且 reparse-point count 为 0。

本次只送入 Windows Recycle Bin 的精确目标：

1. 游戏安装恢复根目录：
   `C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\.damage-forecast-backups`
2. 旧配置：
   `C:\Users\ROG\AppData\Roaming\SlayTheSpire2\mod_configs\STS2PartyWatch.cfg`
3. 首次迁移的旧配置备份：
   `C:\Users\ROG\AppData\Roaming\SlayTheSpire2\damage-forecast-migration\transactions\20260722T125822561Z-b291524099f1413abe9e554af74ca12f\STS2PartyWatch.cfg.backup`
4. C3C reverse-sync transaction 目录：
   `C:\Users\ROG\AppData\Roaming\SlayTheSpire2\damage-forecast-migration\transactions\C3C-RB-20260722T132358013Z-installer-rollback`
5. 三个可再生成的旧 work artifact 目录：
   `work\disabled-local-mods`、`work\manual-install-backups`、`work\g6-6e-conflict-20260722T094644200Z`

明确保留：

- active `damage-forecast` v0.3.0 与 current `DamageForecast.cfg`；
- `20260722T125822561Z-b291524099f1413abe9e554af74ca12f\marker.json` 及其 parent transaction；
- `work\bin`、`work\obj`、`work\publish`、`work\forecast-guardrails`（归 C4）；
- `work\disabled-workshop-mods` 与两个 `work\workshop-upload-*` staging；
- Workshop 内容、源代码、日志和已记录的 HUD behavior defect。

当时 Gate snapshot：`C3D In Progress / Recycle Bin Cleanup Pending`。

### 25.2 Recycle Bin cleanup 与删除后复核

所有目标在删除前再次通过绝对路径、目标类型、无 reparse point、游戏进程为 0 和 keep targets 存在性预检。随后以下 7 个精确目标均成功使用 `SendToRecycleBin`，未永久删除：

- 游戏安装恢复根目录 1 个；
- old cfg 1 个；
- 首次迁移 old cfg backup 1 个；
- C3C reverse-sync transaction 目录 1 个；
- repo 内可再生成的旧 work artifact 目录 3 个。

清理后只读复核：

- 7 个精确目标均 `Exists=False`，在 Windows Recycle Bin 被清空前可恢复；
- stable 环境仍为 v0.107.1 / commit `59260271`，游戏进程为 0，清理后未启动游戏；
- Loader-visible Damage Forecast identity count 1；active 仍为 `damage-forecast` v0.3.0 exact two files：
  - manifest 371 bytes、SHA256 `FF8D4E07E574F9FC89EDEDF0D569EE8A7CADFE2A6A2907CAA9E3097F476C32DB`；
  - DLL 202752 bytes、SHA256 `EC22C91A20BE88E2B39D5285769771B5999556534FCEDE5EF000E1A82FB0EE43`；
- current `DamageForecast.cfg` 仍为 802 bytes、SHA256 `B2DB575CF81C8562B65FE85F387E226D33844C97855F51698CFAB12F37AE04F8`；18 个配置项保持，`ShowAdvancedShieldHeartDetails=False`；
- migration tree 只剩 completed `marker.json`：1511 bytes、SHA256 `ED522CA53E1FEDD202A8960CDF9AC73569309E09A40F5DD338411DEB302911B0`；old cfg、历史 backup 与 reverse transaction 均已移走；
- Workshop Damage Forecast identity count 0，订阅 item `3755598583` 仍不存在；
- `work\bin`、`work\obj`、`work\publish`、`work\forecast-guardrails`、`work\disabled-workshop-mods` 与两个 `work\workshop-upload-*` staging 均保留；
- 未修改 active Mod/new cfg/marker、Workshop、源代码或 HUD behavior；未执行 stage/commit/push/tag。

C3D 已闭环。当时 Gate snapshot：`C3D Complete / C4 Approval Pending`；C4、Workshop、Git 发布动作与 HUD behavior fix 仍分别 gated。

---

## 26. C4 active-scope guard 与 final closure（2026-07-24）

### C4

- Result: Complete；active assembly 普通旧标识、旧 compiled type、旧 Godot node、active old ordinary path 均为 0。
- Changed: legacy runtime token 收口到 non-inlined compatibility descriptor；新增 schema 2 token/file/authority contract 与 10 个 C4 cases；8 份 current authority 已同步。
- Verified: stable/beta 各 `226/226`；两边 Release build 0 warnings / 0 errors；exact-two matching publish PASS，DLL SHA256 `81207FBEDC390E4CB33F706152C8B98AB9FE02097FC0A61F23BC68EB278B31D0`。
- Preserved: C3 L3 evidence仍绑定已安装 `EC22...` DLL；current 18-key `DamageForecast.cfg`、completed marker 与 Workshop unchanged。
- Risks / Pending: C4 artifact 未安装或启动；未来本地安装/Workshop 前须重新绑定届时最终 artifact 的 runtime smoke。HUD behavior、repository-root rename、compatibility retirement 均另行 gated。
- Evidence: §13.5、§25；`post-g6-persistence-contract.json` schema 2；本节记录的 current-session commands。

### Final closure

Result: Post-G6 C1-C4 已完成实现、stable/beta 验证、迁移 runtime matrix、获批清理与 authority reconciliation。
Current state: active install 为唯一 `damage-forecast` v0.3.0；current cfg only `DamageForecast.cfg`；已知 damaging Status/Curse HUD defect 未处理；Workshop unchanged。
Authority: 本任务卡；当前产品事实同步至 `README.md`、`docs/project-state.md`、`docs/architecture.md`、`docs/mechanics-evidence.md` 与相关 current authority。
Repository: Checkpoint Pending；用户已批准 commit、push 与 checkpoint tag。
