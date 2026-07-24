using DamageForecast.Combat;
using DamageForecast.Forecast;
using DamageForecast.UI;

internal static class ProjectionContractCases
{
    private static readonly ForecastHudSnapshot StandardSnapshot = new(
        ForecastResult.KnownDamage(5, 3),
        IncomingDamageDisplayRead.Known(12));

    public static IReadOnlyList<ContractCase> Create()
    {
        return
        [
            ProjectionCase(
                "PR-001",
                "Projection.ExpectedOnly_ShowsExpected",
                StandardSnapshot,
                DamageDisplayMode.ExpectedHpLossOnly,
                IncomingDamagePlacement.RightOfExpectedHpLoss,
                showExpected: true,
                showIncoming: false,
                expectedTotal: 8,
                blockable: 5,
                direct: 3,
                incoming: 12,
                normalizedPlacement: IncomingDamagePlacement.RightOfExpectedHpLoss),
            ProjectionCase(
                "PR-002",
                "Projection.IncomingOnly_ShowsIncoming",
                StandardSnapshot,
                DamageDisplayMode.IncomingDamageOnly,
                IncomingDamagePlacement.RightOfExpectedHpLoss,
                showExpected: false,
                showIncoming: true,
                expectedTotal: 8,
                blockable: 5,
                direct: 3,
                incoming: 12,
                normalizedPlacement: IncomingDamagePlacement.RightOfExpectedHpLoss),
            ProjectionCase(
                "PR-003",
                "Projection.Both_ShowsExpectedAndIncoming",
                StandardSnapshot,
                DamageDisplayMode.Both,
                IncomingDamagePlacement.RightOfExpectedHpLoss,
                showExpected: true,
                showIncoming: true,
                expectedTotal: 8,
                blockable: 5,
                direct: 3,
                incoming: 12,
                normalizedPlacement: IncomingDamagePlacement.RightOfExpectedHpLoss),
            ProjectionCase(
                "PR-004",
                "Projection.LeftPlacement_PreservesValues",
                StandardSnapshot,
                DamageDisplayMode.Both,
                IncomingDamagePlacement.LeftOfExpectedHpLoss,
                showExpected: true,
                showIncoming: true,
                expectedTotal: 8,
                blockable: 5,
                direct: 3,
                incoming: 12,
                normalizedPlacement: IncomingDamagePlacement.LeftOfExpectedHpLoss),
            ProjectionCase(
                "PR-005",
                "Projection.InvalidMode_FallsBackToExpectedOnly",
                StandardSnapshot,
                (DamageDisplayMode)999,
                IncomingDamagePlacement.RightOfExpectedHpLoss,
                showExpected: true,
                showIncoming: false,
                expectedTotal: 8,
                blockable: 5,
                direct: 3,
                incoming: 12,
                normalizedPlacement: IncomingDamagePlacement.RightOfExpectedHpLoss),
            ProjectionCase(
                "PR-006",
                "Projection.InvalidPlacement_FallsBackToRight",
                StandardSnapshot,
                DamageDisplayMode.Both,
                (IncomingDamagePlacement)999,
                showExpected: true,
                showIncoming: true,
                expectedTotal: 8,
                blockable: 5,
                direct: 3,
                incoming: 12,
                normalizedPlacement: IncomingDamagePlacement.RightOfExpectedHpLoss),
            ProjectionCase(
                "PR-007",
                "Projection.HiddenSnapshot_HasNoDisplayableValue",
                ForecastHudSnapshot.Hidden,
                DamageDisplayMode.Both,
                IncomingDamagePlacement.RightOfExpectedHpLoss,
                showExpected: false,
                showIncoming: false,
                expectedTotal: 0,
                blockable: 0,
                direct: 0,
                incoming: 0,
                normalizedPlacement: IncomingDamagePlacement.RightOfExpectedHpLoss),
            ProjectionCase(
                "PR-008",
                "Projection.UnknownExpectedInExpectedMode_HidesPartialIncoming",
                new ForecastHudSnapshot(
                    ForecastResult.Unknown,
                    IncomingDamageDisplayRead.Known(12)),
                DamageDisplayMode.ExpectedHpLossOnly,
                IncomingDamagePlacement.RightOfExpectedHpLoss,
                showExpected: false,
                showIncoming: false,
                expectedTotal: 0,
                blockable: 0,
                direct: 0,
                incoming: 12,
                normalizedPlacement: IncomingDamagePlacement.RightOfExpectedHpLoss),
            ProjectionCase(
                "PR-009",
                "Projection.UnknownExpectedInIncomingMode_ShowsTrustedIncoming",
                new ForecastHudSnapshot(
                    ForecastResult.Unknown,
                    IncomingDamageDisplayRead.Known(12)),
                DamageDisplayMode.IncomingDamageOnly,
                IncomingDamagePlacement.RightOfExpectedHpLoss,
                showExpected: false,
                showIncoming: true,
                expectedTotal: 0,
                blockable: 0,
                direct: 0,
                incoming: 12,
                normalizedPlacement: IncomingDamagePlacement.RightOfExpectedHpLoss),
            ProjectionCase(
                "PR-010",
                "Projection.DirectOnlyExpected_PreservesDirectBreakdown",
                new ForecastHudSnapshot(
                    ForecastResult.KnownDamage(0, 7),
                    IncomingDamageDisplayRead.Hidden),
                DamageDisplayMode.ExpectedHpLossOnly,
                IncomingDamagePlacement.RightOfExpectedHpLoss,
                showExpected: true,
                showIncoming: false,
                expectedTotal: 7,
                blockable: 0,
                direct: 7,
                incoming: 0,
                normalizedPlacement: IncomingDamagePlacement.RightOfExpectedHpLoss),
            ProjectionCase(
                "PR-011",
                "Projection.Breakdown_EqualsExpectedTotal",
                StandardSnapshot,
                DamageDisplayMode.Both,
                IncomingDamagePlacement.RightOfExpectedHpLoss,
                showExpected: true,
                showIncoming: true,
                expectedTotal: 8,
                blockable: 5,
                direct: 3,
                incoming: 12,
                normalizedPlacement: IncomingDamagePlacement.RightOfExpectedHpLoss),
            ProjectionCase(
                "PR-012",
                "Projection.NonPositiveKnownIncoming_HidesIncoming",
                new ForecastHudSnapshot(
                    ForecastResult.Hidden,
                    new IncomingDamageDisplayRead(IncomingDamageDisplayReadState.Known, -4)),
                DamageDisplayMode.IncomingDamageOnly,
                IncomingDamagePlacement.RightOfExpectedHpLoss,
                showExpected: false,
                showIncoming: false,
                expectedTotal: 0,
                blockable: 0,
                direct: 0,
                incoming: 0,
                normalizedPlacement: IncomingDamagePlacement.RightOfExpectedHpLoss),
            ProjectionCase(
                "PR-013",
                "Projection.NonPositiveKnownExpected_HidesExpected",
                new ForecastHudSnapshot(
                    new ForecastResult(ForecastResultState.KnownDamage, -5, -2),
                    IncomingDamageDisplayRead.Hidden),
                DamageDisplayMode.ExpectedHpLossOnly,
                IncomingDamagePlacement.RightOfExpectedHpLoss,
                showExpected: false,
                showIncoming: false,
                expectedTotal: 0,
                blockable: 0,
                direct: 0,
                incoming: 0,
                normalizedPlacement: IncomingDamagePlacement.RightOfExpectedHpLoss),
            ProjectionCase(
                "PR-014",
                "Projection.OverflowingExpectedTotal_FailsHidden",
                new ForecastHudSnapshot(
                    new ForecastResult(ForecastResultState.KnownDamage, int.MaxValue, 1),
                    IncomingDamageDisplayRead.Hidden),
                DamageDisplayMode.ExpectedHpLossOnly,
                IncomingDamagePlacement.RightOfExpectedHpLoss,
                showExpected: false,
                showIncoming: false,
                expectedTotal: 0,
                blockable: 0,
                direct: 0,
                incoming: 0,
                normalizedPlacement: IncomingDamagePlacement.RightOfExpectedHpLoss),
            new ContractCase(
                "PR-015",
                "Projection",
                "Projection.ExpectedText_UsesNegativeTotal",
                assert => assert.Equal(
                    "-8",
                    DamageForecastHudDisplay.BuildMainHudDisplay(ForecastResult.KnownDamage(5, 3)))),
            new ContractCase(
                "PR-016",
                "Projection",
                "Projection.IncomingText_UsesPositiveTotal",
                assert => assert.Equal(
                    "12",
                    DamageForecastHudDisplay.BuildIncomingHudDisplay(IncomingDamageDisplayRead.Known(12))))
        ];
    }

    private static ContractCase ProjectionCase(
        string id,
        string name,
        ForecastHudSnapshot snapshot,
        DamageDisplayMode displayMode,
        IncomingDamagePlacement placement,
        bool showExpected,
        bool showIncoming,
        int expectedTotal,
        int blockable,
        int direct,
        int incoming,
        IncomingDamagePlacement normalizedPlacement)
    {
        return new ContractCase(
            id,
            "Projection",
            name,
            assert =>
            {
                var actual = ForecastHudProjectionPolicy.Project(snapshot, displayMode, placement);
                var context = $"snapshot={snapshot}; mode={displayMode}; placement={placement}";
                assert.Equal(showExpected, actual.ShowExpectedHpLoss, context);
                assert.Equal(showIncoming, actual.ShowIncomingDamage, context);
                assert.Equal(expectedTotal, actual.ExpectedTotalHpLoss, context);
                assert.Equal(blockable, actual.ExpectedBlockableHpLoss, context);
                assert.Equal(direct, actual.ExpectedDirectHpLoss, context);
                assert.Equal(incoming, actual.IncomingDamage, context);
                assert.Equal(normalizedPlacement, actual.IncomingPlacement, context);
                assert.Equal(
                    (showExpected && expectedTotal > 0) || (showIncoming && incoming > 0),
                    actual.HasDisplayableValue,
                    context);
                assert.Equal(expectedTotal, actual.ExpectedBlockableHpLoss + actual.ExpectedDirectHpLoss, context);
            });
    }
}
