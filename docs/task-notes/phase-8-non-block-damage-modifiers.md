# Phase 8｜非 Block 承伤修正机制

## 状态

待做

## 目标

补 DiamondDiademPower 等“改变实际承伤、但不属于 Block”的伤害修正机制。

## 当前范围

- DiamondDiadem
- DiamondDiademPower
- 其他已证明改变实际承伤、但不属于 Block 的机制

## 本任务允许做的事

- 只读确认 shipped 机制与实际时序。
- 在证据充分时，把非 Block 承伤修正纳入预测。
- 保持已验证的 `🛡 -N` 与 `♥ -N` 来源规则清晰分离。

## 本任务禁止做的事

- 接入 TungstenRod / BeatingRemnant。
- 开发多人 HUD。
- 创建完整回合模拟器。
- 调用真实结算、命令、RNG、存档或网络入口。

## 下一步唯一任务

- Phase 9：做单人正式版收口
