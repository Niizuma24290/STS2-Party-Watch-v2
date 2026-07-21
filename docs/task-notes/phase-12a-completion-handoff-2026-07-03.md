# Phase 12A Completion Handoff - 2026-07-03

> Historical / Superseded handoff. Preserve as dated evidence.
> Current task and authority: [`README.md`](README.md) + [`docs/project-state.md`](../project-state.md).

## 当前结论

Phase 12A 已完成：Party Watch 设置已经迁移到 BaseLib 自动生成的 `SimpleModConfig` 配置页，并完成主菜单路径的用户侧运行时验证。

不要把 Phase 12A 扩展成 Phase 12B。当前只声明“BaseLib 自动 UI 基线完成”，不声明自定义右侧 UI、友好标签、本地化、完整 controller/focus 体验或战斗内置入口可用。

## 已完成

- 添加 `Alchyr.Sts2.BaseLib` `3.3.4` compile-only PackageReference。
- 在 manifest 中添加 BaseLib dependency：`{"id":"BaseLib","min_version":"3.3.4"}`。
- 新增 `PartyWatchBaseLibConfig : SimpleModConfig`，用静态属性定义现有 Party Watch 设置。
- 新增 `PartyWatchSettingsAdapter`，订阅 BaseLib config changed，并同步到 `PartyWatchUiSettings`。
- `MainFile.Initialize()` 中注册 BaseLib config，并在 Harmony patch 前应用当前设置。
- `PartyWatchUiSettings` 新增批量 `Apply(...)`，避免一次同步触发多次 changed。
- 删除旧的 Party Watch 自建 `PartyWatchSettingsPatch.cs` 设置入口。
- Release build 通过，0 warning，0 error。
- Publish 通过。
- 本地 mods 目录已安装 BaseLib v3.3.4 runtime 和本次 Party Watch build。
- 用户从游戏主菜单进入 Mod Configuration，确认 `STS2PartyWatch` 可见、可打开、各项自动控件可操作。

## 运行时验证证据

记录文件：

```text
docs/task-notes/phase-12a-runtime-verification-2026-07-03.md
```

截图：

```text
docs/task-notes/assets/phase-12a-runtime/main-menu-mod-config.png
docs/task-notes/assets/phase-12a-runtime/in-combat-built-in-entry.png
```

用户反馈：

```text
图1 从游戏主菜单进去可以操作，各种操作也可以用。
图2 是游戏内置的通道，但使用不了这个先不管，记录下来。
```

## 已知限制

- 主菜单 Mod Configuration 路径可用，是 Phase 12A 的支持路径。
- 战斗内置设置页中的 `模组配置（BaseLib）` / `打开配置` 通道可见但目前不可用，已记录，Phase 12A 不修。
- 自动 UI 仍显示代码属性名和枚举名，例如 `EnablePartyWatchHud`、`HealthBarRight`；友好文案、本地化、hover tip、布局 polish 属于后续 UI 工作。
- 未做 Phase 12B 自定义 `SetupConfigUI`。
- 未改 Poison 逻辑。
- 未重新打开 formal multiplayer HUD / teammate HUD / shared party HUD。

## 当前文档状态

- `docs/project-state.md` 已更新为 Phase 12A 完成口径。
- `docs/task-notes/phase-12a-baselib-auto-config-evaluation.md` 末尾已补后续运行时结果链接。
- 本文件是干净 UTF-8 的 Phase 12A 完成交接卡，用于替代早前乱码 handoff 的阅读入口。

## 下一步建议

Phase 12A 不需要继续做。

下一项应单独开范围：

- 修 v1.08 covering-screen HUD hide regression；或
- 开 Phase 12B 做 BaseLib 配置页友好标签/本地化/自定义 UI polish。

继续保持 Poison 扩展和 formal multiplayer HUD 冻结，除非用户明确重新打开。
