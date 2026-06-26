# Phase 1–4｜单人攻击 HUD 基线

## 状态

进行中

## 目标

空 Mod 可加载
→ 只读读取单人战斗数据
→ HUD 可显示
→ 使用原生 AttackIntent 预览显示 🛡 -N

## 本任务允许做的事

- 建立最小可加载 Mod。
- 只读读取单人战斗数据。
- 建立 HUD 显示路径。
- 使用原生 AttackIntent 预览计算并显示 🛡 -N。

## 本任务禁止做的事

- Frost / 覆甲 Block 修正。
- ♥ -N。
- 多人 HUD。
- 完整回合模拟。
- 通用伤害引擎。

## 完成标准

- Mod 可加载。
- 可只读读取单人战斗数据。
- HUD 可显示 `🛡 -N`。
- 使用原生 AttackIntent 预览作为攻击伤害来源。
- 已记录运行时验证结果和未验证内容。

## 已验证事实

- `C:\sts2\dotnet\dotnet.exe restore src/STS2PartyWatchCode/STS2PartyWatchCode.csproj` 成功。
- `C:\sts2\dotnet\dotnet.exe build src/STS2PartyWatchCode/STS2PartyWatchCode.csproj -c Release --no-restore` 成功，0 warning，0 error。
- `C:\sts2\dotnet\dotnet.exe publish src/STS2PartyWatchCode/STS2PartyWatchCode.csproj -c Release --no-build -o work/publish/STS2PartyWatchCode` 成功。
- 发布包包含 `manifest.json`、`STS2PartyWatchCode.dll`、`STS2PartyWatchCode.deps.json`。
- 发布包曾被复制到本机 STS2 `mods/STS2PartyWatchCode` 目录用于验证；验证受阻后该临时安装目录已清理。
- 本机直接启动 `SlayTheSpire2.exe` 被 Steam 初始化错误阻止，未到达 Mod 加载或战斗阶段。

## 仅代码确认、尚未运行时验证

- Mod 初始化入口使用 `[ModInitializer(nameof(Initialize))]`。
- 初始化时输出一次 `[STS2 Party Watch] Loaded`，并注册 Harmony patch。
- `NCombatUi.Activate` patch 创建/刷新 HUD controller。
- `NCombatUi.Deactivate` patch 清理 HUD。
- 单人路径通过 `LocalContext.GetMe(ICombatState)` 读取本机玩家。
- 当前本机 Block 来自 `Creature.Block`，每次刷新只读取一次。
- 敌人攻击 RAW 来自 `AttackIntent.GetTotalDamage(new[] { localCreature }, enemy)`。
- 多个敌人的可读攻击 Intent 会先累计 RAW，再统一减当前 Block。
- HUD 文本严格为 `🛡 -N`。
- HUD 尝试锚定在本机玩家血条右侧，并留出额外水平间距；若血条节点无法定位，则回退到玩家节点右侧偏移。
- 非单人、无战斗、无本机玩家、无本机 Creature、无攻击 Intent、RAW 不可读、OUT <= 0 时隐藏。

## 未解决问题

- 游戏必须通过 Steam 或等价可初始化 Steamworks 的方式启动后，才能验证 Mod 加载。
- 尚未验证日志中出现 `[STS2 Party Watch] Loaded`。
- 尚未验证非战斗 HUD 隐藏。
- 尚未验证进入单人战斗后 HUD 正常工作。
- 尚未验证单个敌人单段攻击。
- 尚未验证多段攻击。
- 尚未验证多个敌人攻击累计。
- 尚未验证玩家 Block 改变后 N 会刷新。
- 尚未验证敌人非攻击 Intent 不显示。
- 尚未验证 OUT = 0 时不显示。
- 尚未验证无可读战斗实体时不崩溃、不显示猜测值。

## 实际改动文件

- `src/STS2PartyWatchCode/STS2PartyWatchCode.csproj`
- `src/STS2PartyWatchCode/manifest.json`
- `src/STS2PartyWatchCode/MainFile.cs`
- `src/STS2PartyWatchCode/Combat/IncomingDamageRead.cs`
- `src/STS2PartyWatchCode/Combat/LocalIncomingDamageReader.cs`
- `src/STS2PartyWatchCode/Forecast/ForecastResult.cs`
- `src/STS2PartyWatchCode/Forecast/LocalDamageForecast.cs`
- `src/STS2PartyWatchCode/Patches/ForecastRefreshPatch.cs`
- `src/STS2PartyWatchCode/UI/ForecastHudController.cs`
- `src/STS2PartyWatchCode/UI/ForecastHudView.cs`
- `src/STS2PartyWatchCode/UI/HealthBarLocator.cs`
- `docs/project-state.md`
- `docs/interface-map.md`
- `docs/mechanics-evidence.md`
- `docs/architecture.md`
- `docs/task-notes/README.md`
- `docs/task-notes/phase-1-4-singleplayer-baseline.md`

## 下一步唯一任务

- Phase 1–4：Steam 启动后的单人攻击 HUD 运行时验证

## 预期提交文件

- `src/STS2PartyWatchCode/STS2PartyWatchCode.csproj`
- `src/STS2PartyWatchCode/manifest.json`
- `src/STS2PartyWatchCode/MainFile.cs`
- `src/STS2PartyWatchCode/Combat/IncomingDamageRead.cs`
- `src/STS2PartyWatchCode/Combat/LocalIncomingDamageReader.cs`
- `src/STS2PartyWatchCode/Forecast/ForecastResult.cs`
- `src/STS2PartyWatchCode/Forecast/LocalDamageForecast.cs`
- `src/STS2PartyWatchCode/Patches/ForecastRefreshPatch.cs`
- `src/STS2PartyWatchCode/UI/ForecastHudController.cs`
- `src/STS2PartyWatchCode/UI/ForecastHudView.cs`
- `src/STS2PartyWatchCode/UI/HealthBarLocator.cs`
- `docs/project-state.md`
- `docs/interface-map.md`
- `docs/mechanics-evidence.md`
- `docs/architecture.md`
- `docs/task-notes/README.md`
- `docs/task-notes/phase-1-4-singleplayer-baseline.md`

## 提交记录

- 待提交
