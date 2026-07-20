# Session 交接卡｜Mod 双版本适配

## 1. 项目定位

```text
项目：STS2 Party Watch v2
仓库路径：C:\Users\ROG\Documents\Codex\STS2-Party-Watch-v2
当前分支：main
远程仓库：https://github.com/Niizuma24290/STS2-Party-Watch-v2.git
当前阶段：Phase 11D 王冠机制兼容已完成代码接入；整个 Mod 的 stable / beta 双版本适配尚未开始实施
```

一句话说明当前项目在做什么：

```text
只读战斗预测 HUD Mod；当前已完成 v0.109 王冠机制路由，下一步建立整个 Mod 的正式版 / Beta 双目标构建与兼容验证基础设施。
```

---

## 2. 本次 Session 原始任务

```text
本次要解决：保留旧王冠减伤与旧惊涛修正，接入 v0.109 首回合 Block + Blur 王冠，并为整个 Mod 的正式版 / Beta 双版本适配制定可执行方案。
```

任务结果：

```text
部分完成：王冠机制兼容、旧源码保留、构建、发布和本地安装已完成；整个 Mod 的双版本构建、二进制验证和双分支运行时矩阵尚未实施。
```

---

## 3. 本次实际完成内容

只写已经实际完成的事实：

```text
- 反编译并确认 v0.109 DiamondDiadem：第一回合开始获得 20 Block，并施加 1 层 BlurPower；不再统计出牌数，不再生成 DiamondDiademPower，也不再修改敌方攻击伤害。
- 将旧王冠出牌阈值、逐段 0.5 减伤、取整和旧王冠专属 Stampede 修正保留为独立正式源码。
- VerifiedEnemyDamageModifier 已按运行时能力识别旧王冠、v0.109 王冠和未知机制。
- 当前 / 未知王冠机制保持原生攻击值；王冠读取失败不再让整个伤害结果变成 Unknown。
- 编译产物已确认没有 DiamondDiadem 或 DiamondDiademPower 硬类型引用；剩余 StampedePower 硬引用属于波纹水盆原有逻辑。
- 王冠版本已完成 Release build、publish、本地安装和产物哈希核对。
- 已形成整个 Mod 双版本适配计划，但尚未创建双参考程序集目录、双目标构建脚本、统一能力档案或分组 Harmony 启动器。
```

---

## 4. 实际修改文件

本次王冠与交接工作：

```text
新增：
- src/STS2PartyWatchCode/Combat/LegacyDiamondDiademDamageForecast.cs
- docs/task-notes/phase-11d-diamond-diadem-mechanism-compatibility.md
- docs/task-notes/session-handoff-dual-version-compatibility-2026-07-17.md

修改：
- src/STS2PartyWatchCode/Combat/VerifiedEnemyDamageModifier.cs
- README.md
- docs/architecture.md
- docs/interface-map.md
- docs/mechanics-evidence.md
- docs/project-state.md
- docs/task-notes/README.md

删除：
- 本次没有删除文件。
```

每个重要文件一句话说明作用：

```text
src/STS2PartyWatchCode/Combat/VerifiedEnemyDamageModifier.cs：检测王冠能力表面并路由 Legacy / FirstTurnBlockAndBlur / NativeOnlyFallback。
src/STS2PartyWatchCode/Combat/LegacyDiamondDiademDamageForecast.cs：保留旧王冠减伤、阈值、逐段取整和旧王冠专属惊涛修正；旧 API 通过类型名和反射读取。
docs/task-notes/phase-11d-diamond-diadem-mechanism-compatibility.md：记录 v0.109 原生证据、旧机制边界、失败回退政策、安装哈希和待验证矩阵。
docs/project-state.md：将王冠状态从 SkippedPendingRemap 更新为能力路由的 Implemented, Conditional。
```

工作区还有大量此前 Session 留下的设置、HUD、Poison 和兼容性改动。它们不是本次王冠任务新建的，不得回滚。当前 `git status` 还包括 `PartyWatchSettingsPatch.cs` 的既有删除，以及多个既有未跟踪源码和 task note。

---

## 5. 验证结果

### 已完成检查

```text
Build：Release build 通过，0 warnings，0 errors。
Publish：通过，输出到 bin/Release/net9.0/publish 和 work/publish。
Lint / diff check：git diff --check 无空白错误；只有现有 LF -> CRLF 警告。
Git status：dirty；有大量已修改、已删除和未跟踪文件，详见第 7 节并在下一 Session 重新执行 git status --short。
运行时验证：本轮新王冠能力路由尚未进行游戏内专项验证。
```

### 运行时验证记录

```text
测试条件：当前安装游戏为 v0.109.0；本地模组 DLL 已更新。
操作：仅完成构建、发布、安装和 DLL 哈希比较。
预期：游戏内 v0.109 王冠只使用原生 20 Block + Blur，不读取旧惊涛修正；普通攻击 HUD 保持显示。
实际结果：Runtime verification not completed.
是否通过：尚未判定。
```

已安装 DLL：

```text
C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\sts2-party-watch-v2\sts2-party-watch-v2.dll
SHA256：A9D4BFC675F48DDC87774EBCC52008027185D70C06ACF43ECCE5B30E7621A296
```

发布目录 DLL 与游戏目录 DLL 的 SHA256 已确认一致。

### 未验证项

```text
- v0.109 王冠第一回合 20 Block + Blur 的 Party Watch 实际显示。
- v0.109 王冠与 Stampede 共存时，Stampede 不介入王冠计算。
- 旧正式版王冠 Legacy 路由重新激活后的阈值、逐段减伤和旧惊涛修正。
- 同一个最终 DLL 在正式版和 Beta 两边加载与运行。
- 整个 Mod 的正式版 / Beta 双目标构建与二进制成员引用扫描。
```

---

## 6. 当前稳定状态

当前已经确认可用、不要随意改坏的内容：

```text
- Exoskeleton / HardToKill Poison 预测已由用户运行时验证。
- 墨宝 / Slippery Poison 逐段、逐层预测已由用户运行时验证。
- SewerClam / HardenedShell 剩余限伤预算预测已由用户运行时验证。
- 敌方 Intangible Poison 专项算法当前已经回滚；有敌方无实体时保留基础 Intent。
- HookDamageCompat 已保留 v0.107.1 10 参数与 v0.108+ 11 参数 ModifyDamage 兼容路径。
- ForecastRefreshPatch 已有 stable BeforeTurnEnd / beta BeforeSideTurnEnd 可选目标的历史修复。
- 波纹水盆自己的 Stampede 修正仍在 VerifiedPreAttackBlockReader，不能与旧王冠 Stampede 修正合并。
- 当前王冠未知机制和读取异常会保持原生攻击值，不再隐藏完整 HUD。
```

当前已知限制、风险或不支持情形：

```text
- 当前项目仍只从正在安装的 Steam 游戏目录编译，没有冻结的 stable / beta 两套参考程序集。
- 当前机器未找到保存下来的 v0.107.1 stable sts2.dll；正式版参考需要后续切换分支或单独获取。
- 当前发布 DLL 是针对已安装 v0.109 构建，不能据此宣称完整双版本支持。
- MainFile 仍先初始化 BaseLib，再一次性 Harmony.PatchAll；任一未来 API / Patch 类型失配仍可能拖垮整个初始化。
- HookDamageCompat 只识别已知 10 / 11 参数签名；未来新签名需要扩展。
- 编译通过不代表机制正确；王冠已经证明机制变化必须单独做运行时验证。
- 整个工作区未提交改动很多，后续不能使用 reset / checkout 回滚未知改动。
```

---

## 7. Git 与产物状态

```text
最新提交 hash：4febf61d68dd5bb0000955d75aa9fe6b99c3d692（4febf61 docs: record HUD cleanup smoke success）
本次提交 hash：无，本次没有提交。
是否已 push：否。
git status：dirty；main 分支上有大量 modified、1 个既有 deleted，以及多个 untracked 文件。
```

明确确认未提交的内容：

```text
- 构建产物：bin/、obj/、work/publish 产物未出现在 git status，保持忽略状态。
- 日志：游戏日志位于 C:\Users\ROG\AppData\Roaming\SlayTheSpire2\logs，未提交。
- 临时目录：本次未创建需要提交的临时目录；现有 work/ 保持忽略。
- 发布文件：已安装到游戏 mods 目录，未提交到 Git。
- 源码与文档：王冠源码、Phase 11D task note、本交接卡及大量此前改动仍未提交。
```

当前远程：

```text
origin https://github.com/Niizuma24290/STS2-Party-Watch-v2.git
```

---

## 8. 下一步唯一任务

下一步只做：

```text
建立双版本编译基线：在仓库外捕获当前 v0.109 Beta 参考程序集和哈希清单，并让 STS2PartyWatchCode.csproj + 新构建脚本支持显式 Sts2ReferenceRoot 和独立 stable / beta 输出目录；先用当前 Beta 快照跑通，不改任何预测机制。
```

下一步禁止做：

```text
- 不切换或重写王冠、Poison、HUD 显示逻辑。
- 不立即创建 Bootstrap + 双载荷，也不复制两套业务源码。
- 不宣称已经支持正式版和 Beta；stable 参考和运行时验证尚未完成。
- 不提交游戏 DLL、XML、BaseLib DLL、构建产物或日志。
- 不清理、reset、checkout 或回滚当前工作区中的既有改动。
```

---

## 9. 给下一个 Session 的启动指令

下一个 Session 开始时必须：

```text
1. 先读本交接卡。
2. 再读：
   - docs/task-notes/phase-11d-diamond-diadem-mechanism-compatibility.md
   - docs/task-notes/phase-11a-v108-quick-compatibility.md
   - docs/task-notes/phase-11b-stable-dll-initialization.md
   - docs/project-state.md
   - src/STS2PartyWatchCode/STS2PartyWatchCode.csproj
   - src/STS2PartyWatchCode/MainFile.cs
   - src/STS2PartyWatchCode/Combat/HookDamageCompat.cs
   - src/STS2PartyWatchCode/Combat/VerifiedEnemyDamageModifier.cs
   - src/STS2PartyWatchCode/Combat/LegacyDiamondDiademDamageForecast.cs
   - src/STS2PartyWatchCode/Patches/ForecastRefreshPatch.cs
3. 先确认当前 git status。
4. 不重复已完成或已验证的任务。
5. 只处理“下一步唯一任务”。
6. 遇到机制证据不足时，不猜；先汇报阻塞点。
7. 完成后继续按本模板生成新的交接卡。
```

---

## 10. 给下一位执行者的简短摘要

```text
当前项目已经完成：旧 / 新王冠能力路由已接入，旧减伤与旧惊涛修正保留为正式源码，v0.109 构建和本地安装完成。

当前正在做：准备把现有零散 stable / beta 兼容修复升级为整个 Mod 的双版本构建与验证体系。

下一步唯一任务：冻结当前 v0.109 Beta 参考程序集，并实现显式参考根目录和双目标独立输出的构建基础设施。

最重要边界：能力检测优先于版本名；任何可选机制失配只回退原生值，不能让完整 HUD 消失，也不能回滚当前脏工作区。
```
