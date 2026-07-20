using STS2PartyWatch.Combat;
using STS2PartyWatch.Diagnostics;
using STS2PartyWatch.Forecast;
using STS2PartyWatch.Patches;
using STS2PartyWatch.Settings;

var failures = new List<string>();

Check(
    VerifiedTurnEndDamagePolicy.IsVerifiedSingleBlockableDamageShape(
        typeof(VerifiedCard),
        typeof(VerifiedCard),
        1,
        false),
    "exact verified card with one blockable DamageVar is accepted");
Check(
    !VerifiedTurnEndDamagePolicy.IsVerifiedSingleBlockableDamageShape(
        typeof(DerivedVerifiedCard),
        typeof(VerifiedCard),
        1,
        false),
    "derived card cannot inherit built-in verification");
Check(
    !VerifiedTurnEndDamagePolicy.IsVerifiedSingleBlockableDamageShape(
        typeof(UnverifiedCard),
        typeof(VerifiedCard),
        1,
        false),
    "unverified card type is rejected");
Check(
    !VerifiedTurnEndDamagePolicy.IsVerifiedSingleBlockableDamageShape(
        typeof(VerifiedCard),
        typeof(VerifiedCard),
        2,
        false),
    "multiple DamageVars are rejected");
Check(
    !VerifiedTurnEndDamagePolicy.IsVerifiedSingleBlockableDamageShape(
        typeof(VerifiedCard),
        typeof(VerifiedCard),
        1,
        true),
    "unblockable DamageVar is rejected");

var forecast = ForecastResult.KnownDamage(-5, 7);
Check(forecast.OutDamage == 0 && forecast.DirectHpLoss == 7, "forecast result clamps negative lanes");

var incoming = IncomingDamageDisplayRead.Known(-4);
Check(incoming.State == IncomingDamageDisplayReadState.Hidden && incoming.Damage == 0, "incoming zero or negative result is hidden");
Check(IncomingDamageDisplayRead.Known(4).Damage == 4, "positive incoming result remains known");

Check(PartyWatchDiagnostics.TryMarkReported("contract-test.once"), "diagnostic code reports the first time");
Check(!PartyWatchDiagnostics.TryMarkReported("contract-test.once"), "diagnostic code is rate-limited after the first report");

Check(
    PartyWatchConfigText.Setting(
        nameof(PartyWatchBaseLibConfig.EnablePartyWatchHud),
        PartyWatchConfigLanguage.English) == "Enable Damage Forecast HUD",
    "English HUD enable label uses the current player-facing name");
Check(
    PartyWatchConfigText.Setting(
        nameof(PartyWatchBaseLibConfig.EnablePartyWatchHud),
        PartyWatchConfigLanguage.SimplifiedChinese) == "启用伤害预测 HUD",
    "Simplified Chinese HUD enable label uses the current player-facing name");
Check(
    PartyWatchConfigText.Setting(
        nameof(PartyWatchBaseLibConfig.ShowLocalPlayerHudInMultiplayer),
        PartyWatchConfigLanguage.English) == "Show local-player Damage Forecast HUD in Multiplayer",
    "English multiplayer label states the local-player boundary");
Check(
    PartyWatchConfigText.Setting(
        nameof(PartyWatchBaseLibConfig.ShowLocalPlayerHudInMultiplayer),
        PartyWatchConfigLanguage.SimplifiedChinese) == "在多人模式中显示本机伤害预测 HUD",
    "Simplified Chinese multiplayer label states the local-player boundary");
Check(
    PartyWatchBaseLibTitlePatch.ResolveListTitle(
        "sts2-party-watch-v2",
        "STS2PartyWatch") == "Damage Forecast",
    "BaseLib list identity uses the stable English player-facing name for this Mod ID");
Check(
    PartyWatchBaseLibTitlePatch.ResolveListTitle(
        "sts2-party-watch-v2",
        "STS2PartyWatch") == "Damage Forecast",
    "BaseLib list identity stays English when the config language changes");
Check(
    PartyWatchBaseLibTitlePatch.ResolveListTitle(
        "another-mod",
        "AnotherMod") == "AnotherMod",
    "BaseLib list identity preserves every other Mod title");
Check(
    PartyWatchBaseLibTitlePatch.ResolvePageTitle(
        "sts2-party-watch-v2",
        PartyWatchConfigLanguage.English,
        "STS2PartyWatch") == "Damage Forecast",
    "BaseLib page title uses the English player-facing name for this Mod ID");
Check(
    PartyWatchBaseLibTitlePatch.ResolvePageTitle(
        "sts2-party-watch-v2",
        PartyWatchConfigLanguage.SimplifiedChinese,
        "STS2PartyWatch") == "伤害预测",
    "BaseLib page title follows the Simplified Chinese config language");
Check(
    PartyWatchBaseLibTitlePatch.ResolvePageTitle(
        "another-mod",
        PartyWatchConfigLanguage.SimplifiedChinese,
        "AnotherMod") == "AnotherMod",
    "BaseLib page title preserves every other Mod title");
Check(
    PartyWatchBaseLibTitlePatch.HasCompatibleTarget(),
    "BaseLib 3.3.4 exposes the expected title resolver, page-load hook, and title field");

var visibilityAccumulator = new VisibilityPerformanceAccumulator();
visibilityAccumulator.Record(VisibilityPostfixKind.Show, false, false, 10, 100, false, 0, 0);
visibilityAccumulator.Record(VisibilityPostfixKind.Hide, true, true, 20, 200, true, 30, 2);
visibilityAccumulator.Record(VisibilityPostfixKind.SetVisible, true, false, 30, 300, true, 40, 3);
var visibilitySnapshot = visibilityAccumulator.Snapshot(2_000_000);
Check(
    visibilitySnapshot.ShowCalls == 1
    && visibilitySnapshot.HideCalls == 1
    && visibilitySnapshot.SetVisibleCalls == 1
    && visibilitySnapshot.TotalCalls == 3,
    "visibility profiling aggregates entry points");
Check(
    visibilitySnapshot.UnmatchedCalls == 1 && visibilitySnapshot.MatchedCalls == 2,
    "visibility profiling aggregates match outcomes");
Check(
    visibilitySnapshot.Transitions == 1 && visibilitySnapshot.Duplicates == 1,
    "visibility profiling separates transitions from duplicates");
Check(
    visibilitySnapshot.Refreshes == 2 && visibilitySnapshot.BarsVisited == 5,
    "visibility profiling aggregates refresh work");
Check(
    visibilitySnapshot.TypeCheckTotalTicks == 60
    && visibilitySnapshot.TypeCheckMaxTicks == 30
    && visibilitySnapshot.CallbackTotalTicks == 600
    && visibilitySnapshot.CallbackMaxTicks == 300
    && visibilitySnapshot.RefreshTotalTicks == 70
    && visibilitySnapshot.RefreshMaxTicks == 40,
    "visibility profiling aggregates total and maximum timings");
var visibilitySummary = Aud0007VisibilitySummaryFormatter.Format(
    "contract-test",
    visibilitySnapshot,
    1_000_000);
Check(
    visibilitySummary.StartsWith("[STS2 Party Watch][AUD-0007] reason=contract-test window_s=2.000", StringComparison.Ordinal)
    && visibilitySummary.Contains("calls_per_s=1.500", StringComparison.Ordinal)
    && visibilitySummary.Contains("match_pct=66.667", StringComparison.Ordinal)
    && visibilitySummary.Contains("duplicate_pct=50.000", StringComparison.Ordinal)
    && visibilitySummary.Contains("refreshes_per_transition=2.000", StringComparison.Ordinal),
    "visibility profiling summary formatting is stable and invariant");
visibilityAccumulator.Reset();
Check(visibilityAccumulator.Snapshot(0).TotalCalls == 0, "visibility profiling reset clears counters");

if (failures.Count > 0)
{
    foreach (var failure in failures)
    {
        Console.Error.WriteLine($"FAIL: {failure}");
    }

    return 1;
}

Console.WriteLine("28 contract tests passed.");
return 0;

void Check(bool condition, string description)
{
    if (!condition)
    {
        failures.Add(description);
    }
}

internal class VerifiedCard;

internal sealed class DerivedVerifiedCard : VerifiedCard;

internal sealed class UnverifiedCard;
