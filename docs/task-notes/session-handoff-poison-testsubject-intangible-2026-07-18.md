# 专项交接任务卡｜Poison × 实验体第三阶段 Intangible

> Historical / Superseded handoff. Preserve as dated evidence.
> Current task and authority: [`README.md`](README.md) + [`docs/project-state.md`](../project-state.md).

日期：2026-07-18

## 1. 专项目标

下一位只处理一件事：

```text
在当前 v0.109 王冠兼容基线上，重新接入敌方 Intangible 的窄范围 Poison 行动前存活预测，并完成实验体第三阶段运行时验证。
```

该专项只决定：实验体是否会在当前回合开始时被 Poison 杀死，从而是否应从 Party Watch 的 `-N` 中移除它当前的 Attack Intent。

不做完整回合模拟，不执行真实伤害、死亡、复活、转阶段、敌方行动、RNG 或网络行为。

---

## 2. 已确认事实

- 敌方 `IntangiblePower` 生效时，每个 Poison tick 最多造成 1 HP loss。
- Intangible 不像 `SlipperyPower` 那样按受伤次数消耗层数；只要当前无实体生效，本轮每个 Poison tick 都按最多 1 点处理。
- Poison 触发总次数为：

```text
triggerCount = min(currentPoison, 1 + alive opponents' AccelerantPower amount sum)
```

- 因此 Intangible 下可确定的 Poison HP loss 为：

```text
intangiblePoisonHpLoss = min(currentHp, triggerCount)
```

- 只有 `triggerCount >= currentHp` 时，才能预测敌人在行动前死亡并移除当前 Intent。
- 用户已确认：实验体死亡后复活或转阶段时不会立即执行原来的攻击，因此确定被 Poison 杀死时可以移除当前 Intent。
- `TestSubject` / `AdaptablePower` 生命周期阻挡已经放开；恢复前唯一阻挡实验体第三阶段 Poison 预测的是敌方 `IntangiblePower` 的保守回滚分支。

---

## 3. 上次失败调查结论

上次“实验体有攻击 Intent 但 Party Watch 不显示伤害”的画面不能作为 Intangible 算法失败证据：

- 该画面中的实验体没有 Poison 图标，代码会在 `CurrentPoison <= 0` 时提前返回，根本不会进入 Intangible Poison 分支。
- 当天游戏从 `v0.107.1` 更新到 `v0.109.0`。
- 故障 DLL 仍引用旧王冠 `DiamondDiademPower` / `CardsPlayedThisTurn` 接口。
- 每次读取敌方攻击都会经过旧王冠修正；接口异常被转换为 unsupported / Unknown，导致 HUD 隐藏数字。
- 王冠现已改为能力路由，未知或新机制保持原生攻击，不再拖垮整个 HUD。

结论：此前测试被版本更新和旧王冠 API 污染，没有证明记录下来的 Intangible 触发次数算法错误。

---

## 4. 恢复前代码状态（历史）

当前 [EnemyPreActionSurvivalPreview.cs](../../src/STS2PartyWatchCode/Combat/EnemyPreActionSurvivalPreview.cs) 中的有效逻辑是：

```csharp
if (state.NativeEnemy.GetPower<IntangiblePower>()?.Amount > 0)
{
    return PoisonTickPreviewResult.CreateSupported(0, willRemainAliveBeforeAction: true);
}
```

效果：只要中毒敌人有 Intangible，Party Watch 就完全禁用该敌人的 Poison survival override，并保留基础 Intent。

此前实现过、随后回滚的代码完整记录在：

- `docs/task-notes/phase-11c-poison-runtime-verification-safe-expansion.md`
- `Enemy Intangible / TestSubject phase 3`
- `Rolled-back code shape`

当前安装基线：

```text
游戏：v0.109.0
DLL：C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\sts2-party-watch-v2\sts2-party-watch-v2.dll
SHA256：A9D4BFC675F48DDC87774EBCC52008027185D70C06ACF43ECCE5B30E7621A296
状态：王冠能力路由版本已构建并安装；王冠本身的专项运行时验证仍待完成。
```

Git 基线：

```text
分支：main
最新提交：4febf61d68dd5bb0000955d75aa9fe6b99c3d692
工作区：dirty；相关源码和 task note 均有未提交改动，不得 reset 或 checkout 回滚。
```

---

## 5. 已执行的恢复方案（历史计划）

只恢复记录中的窄规则：

```text
if CurrentPoison <= 0:
  keep current Intent

if active enemy Intangible:
  if combined with HardToKill, Slippery, or HardenedShell:
    keep current Intent
  else:
    triggerCount = min(CurrentPoison, 1 + OpponentAccelerant)
    previewedHpLoss = min(CurrentHp, triggerCount)
    remove current Intent only when previewedHpLoss >= CurrentHp
```

建议恢复形态就是 Phase 11C 已记录的：

- `TryPreviewIntangiblePoisonDamage(...)`
- 复用现有 `PreviewPoisonTriggerCount(...)`
- 返回 `CreateSupported(...)`
- 无法确定时保持基础 Intent，不返回 Unknown。

不要改用通用敌方 Power 模拟器，不要扩展到玩家 Intangible，也不要重写已验证的 HardToKill、Slippery 或 HardenedShell 路径。

---

## 6. 实施顺序

1. 先用当前回滚 DLL 做无 Poison 基线：实验体第三阶段有 Intangible 和攻击 Intent 时，确认基础攻击数字正常显示。
2. 保存基线截图和当次 `godot.log`，确认加载 DLL 哈希为当前基线。
3. 只修改 `EnemyPreActionSurvivalPreview.cs`，恢复记录中的 Intangible 窄算法。
4. Release build、publish、`git diff --check`。
5. 安装新 DLL，记录安装 SHA256。
6. 按第 7 节矩阵测试实验体第三阶段。
7. 成功后更新 Phase 11C、`docs/project-state.md`、`docs/mechanics-evidence.md` 和 task-note 索引。

如果基线第 1 步仍不显示基础攻击，不得继续恢复 Intangible；先调查当前通用伤害读取或版本兼容问题。

---

## 7. 必测矩阵

### TSI-01：无 Poison 基线

```text
条件：实验体第三阶段；Intangible 生效；Poison = 0；有攻击 Intent。
预期：完整保留该攻击，HUD 正常显示，不得消失。
目的：证明基础攻击读取和王冠兼容基线正常，Intangible Poison 分支没有执行。
```

### TSI-02：Poison 不致死，无额外触发

```text
条件：Intangible 生效；Poison > 0；currentHp > 1；OpponentAccelerant = 0。
预期：本轮只有 1 个 Poison tick，预览 HP loss = 1，保留当前攻击。
```

### TSI-03：有多段触发但仍不致死

```text
条件：Intangible 生效；triggerCount < currentHp。
预期：保留当前攻击；HUD 不得 Unknown 或隐藏。
```

### TSI-04：触发次数刚好覆盖 HP

```text
条件：Intangible 生效；triggerCount == currentHp。
预期：预测行动前死亡，移除实验体当前 Intent。
```

### TSI-05：触发次数超过 HP

```text
条件：Intangible 生效；triggerCount > currentHp，且 currentPoison 足以支持这些触发。
预期：预测行动前死亡，移除实验体当前 Intent。
```

### TSI-06：实际结束回合

```text
条件：使用 TSI-04 或 TSI-05 的确定致死场景并真实结束回合。
预期：实验体由 Poison 死亡并复活 / 转阶段；不会执行被 Party Watch 移除的旧攻击。
```

### TSI-07：复数敌人隔离

```text
条件：实验体确定被 Intangible Poison ticks 杀死；另一个敌人仍有攻击 Intent。
预期：只移除实验体贡献，其他敌人的攻击继续计入 `-N`。
```

### TSI-08：特殊 Power 组合保守回退

```text
条件：Intangible 与 HardToKill、Slippery 或 HardenedShell 任一组合存在。
预期：不套用本专项算法，保留基础 Intent；不得返回 Unknown。
```

每张验证截图至少要同时看见：

- 实验体当前 HP；
- Poison 层数；
- Intangible 图标 / 层数；
- Accelerant / 触媒层数；
- 实验体当前攻击 Intent；
- Party Watch 的 `-N` 或高级伤害数字。

---

## 8. 通过标准

本专项只有同时满足以下条件才可收口：

```text
- TSI-01 基础攻击正常显示。
- TSI-02 / TSI-03 不致死时保留攻击。
- TSI-04 / TSI-05 确定致死时移除攻击。
- TSI-06 真实结算确认转阶段 / 复活后不会执行旧攻击。
- TSI-07 复数敌人贡献彼此隔离。
- 任一不确定或组合场景保持原生 Intent，不让完整 HUD 变成 Unknown。
- Build / publish 通过，0 errors；安装 DLL 哈希已记录。
- 最新游戏日志无 Party Watch 异常。
```

在 TSI-06 完成前，只能标记 `Implemented / Built / Installed`，不能标记 `RuntimeVerified`。

---

## 9. 禁止事项

- 不修改 `VerifiedEnemyDamageModifier`、`LegacyDiamondDiademDamageForecast` 或王冠机制。
- 不开始整个 Mod 的 stable / beta 双版本基础设施任务。
- 不修改波纹水盆的 Stampede 修正。
- 不改外骨骼虫、墨宝、珊瑚现有已验证算法。
- 不输出 `Unknown` 作为敌方 Intangible Poison 的正常结果。
- 不根据原生毒血条颜色直接判定 Intent；毒血条只能作为旁证。
- 不把其他 Mod 的文字或贴图错误归因于 Party Watch。
- 不提交 DLL、PDB、日志、游戏程序集、`work/` 或 publish 目录。
- 不清理或回滚当前脏工作区中的未知改动。

---

## 10. 启动必读文件

下一位开始时依次读取：

1. `docs/task-notes/session-handoff-poison-testsubject-intangible-2026-07-18.md`
2. `docs/task-notes/phase-11c-poison-runtime-verification-safe-expansion.md`
3. `docs/task-notes/phase-11d-diamond-diadem-mechanism-compatibility.md`
4. `docs/project-state.md`
5. `src/STS2PartyWatchCode/Combat/EnemyPreActionSurvivalPreview.cs`
6. `src/STS2PartyWatchCode/Combat/LocalIncomingDamageReader.cs`
7. `src/STS2PartyWatchCode/Combat/VerifiedEnemyDamageModifier.cs`，只读，不修改

随后先执行 `git status --short`，不得重复已验证的 HardToKill、Slippery 和 HardenedShell 实现工作。

---

## 11. 简短摘要

```text
当前状态：实验体敌方 Intangible Poison 窄预测已恢复、构建、安装，并完成代表性 exact-lethal 运行时验证。

历史结论：上次伤害消失由 v0.109 更新后的旧王冠接口失配造成，不是 Intangible 算法已被证明错误。

收口结论：9 HP / 20 Poison / Intangible(1) / Accelerant(8) / 原生 45 Intent 场景中，`triggerCount = 9`，Party Watch 正确移除旧攻击贡献；用户确认运行成功。

最重要边界：不确定就保留原生攻击；绝不能让 Intangible Poison 专项使整个 HUD 变成 Unknown。
```

---

## 12. 2026-07-18 收口记录

- 用户先确认 TSI-01 无 Poison 基线正常：实验体第三阶段的基础攻击数字可正常显示。
- 只在 `EnemyPreActionSurvivalPreview.cs` 恢复交接卡记录的 `TryPreviewIntangiblePoisonDamage(...)` 窄规则；未修改王冠、HardToKill、Slippery 或 HardenedShell 路径。
- Release build / publish 通过，0 warnings、0 errors；`git diff --check` 通过，仅有工作区既有换行符提示。
- 发布 DLL 与安装 DLL SHA256 一致：`B8E5ACB84580BCE901CA51623522B86560CFF178A07CD0F33619E1E49FAB7E8A`。
- TSI-04 exact-lethal 等号边界由用户 Steam 截图确认：实验体 9 HP、20 Poison、Intangible(1)、玩家 Accelerant(8)、原生 45 Attack Intent；`min(20, 1 + 8) = 9`，Party Watch 正确移除该旧攻击贡献。
- 用户报告运行成功；结合此前已确认的实验体死亡后复活 / 转阶段不会立即执行旧攻击，本专项按代表性第三阶段致死边界标记 RuntimeVerified。
- 证据截图：`docs/task-notes/assets/phase-11c-testsubject-intangible-tsi-04-2026-07-18.png`；SHA256 `900852D903DE607AE69ECEAF039C48C8CE89DCEB2A4417358BE79B7EE22586F4`。
- 未在此前已知的 app-userdata 路径找到本次新 `godot.log`，因此不声明日志验证；本次运行时证据为用户确认和保存的截图。
- TSI-02 / 03 / 05 / 07 / 08 没有逐项保存独立截图，不得表述为完整八项矩阵均已单独验证。

最终状态：`Closed / Implemented / Built / Installed / RuntimeVerified (representative exact-lethal TestSubject phase-3 boundary)`。
