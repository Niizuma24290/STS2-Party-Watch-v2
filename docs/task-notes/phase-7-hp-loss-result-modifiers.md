# Phase 7｜HP Loss 结果修正机制

## 状态

待做

## 目标

接入 TungstenRod、BeatingRemnant 等会修正 HP Loss 结果的机制。

## 当前范围

- TungstenRod
- BeatingRemnant
- 其他已证明会修正 HP Loss 结果、但不属于 Block 的机制

## 本任务允许做的事

- 只读确认 shipped 机制与实际时序。
- 在证据充分时，把结果修正纳入 `♥ -N`。
- 保持 `🛡 -N` 既有公式不变。
- 记录代码确认与 Steam 运行时验证状态。

## 本任务禁止做的事

- 接入 DiamondDiadem / DiamondDiademPower。
- 开发多人 HUD。
- 创建完整回合模拟器。
- 调用真实结算、命令、RNG、存档或网络入口。

## 下一步唯一任务

- Phase 8：补 DiamondDiademPower 等“改变实际承伤、但不属于 Block”的伤害修正机制
