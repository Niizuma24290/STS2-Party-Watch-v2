# Decisions

- D-001: v2 is a separate repository with no copied v1 source code.
- D-002: v2.0 supports only local single-player forecast HUD.
- D-003: use native preview APIs and verified shipped mechanisms rather than custom combat formulas.
- D-004: combine blockable damage with verified EffectiveBlock exactly once in the forecast layer.
- D-005: show total expected HP loss as the primary and default HUD value.
- D-006: do not depend on or parse Minty.
- D-007: do not hand-write special damage formulas without a reproducible mismatch.
- D-008: unknown is safer than an incorrect prediction.
- D-009: multiplayer is deferred until a separate future version.
- D-010: if blockable damage is not trustworthy but direct HP loss remains trustworthy, let the display total equal trusted `♥`; optional details omit `🛡`.
- D-011: the display total is not a new mechanics algorithm; it is only `ForecastResult.OutDamage + ForecastResult.DirectHpLoss`.
- D-012: `🛡` / `♥` breakdown details are an advanced UI display option and are disabled by default.
