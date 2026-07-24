using DamageForecast.Combat;
using DamageForecast.Forecast;

internal static class ForecastResultContractCases
{
    public static IReadOnlyList<ContractCase> Create()
    {
        return
        [
            Case(
                "FR-002",
                "Forecast.HiddenInput_ReturnsHidden",
                IncomingDamageRead.Hidden,
                ForecastResultState.Hidden,
                0,
                0),
            Case(
                "FR-003",
                "Forecast.UnknownWithoutTrustedDirect_ReturnsUnknown",
                IncomingDamageRead.Unknown,
                ForecastResultState.Unknown,
                0,
                0),
            Case(
                "FR-004",
                "Forecast.UnknownWithTrustedDirect_ReturnsDirectOnly",
                IncomingDamageRead.UnknownDirect(7),
                ForecastResultState.KnownDamage,
                0,
                7),
            Case(
                "FR-005",
                "Forecast.UnknownWithNegativeDirect_ReturnsUnknown",
                IncomingDamageRead.UnknownDirect(-7),
                ForecastResultState.Unknown,
                0,
                0),
            Case(
                "FR-006",
                "Forecast.KnownRawAboveBlock_ReturnsKnownOutDamage",
                IncomingDamageRead.Known(12, 5, 0),
                ForecastResultState.KnownDamage,
                7,
                0),
            Case(
                "FR-007",
                "Forecast.KnownRawEqualToBlockWithoutDirect_ReturnsHidden",
                IncomingDamageRead.Known(5, 5, 0),
                ForecastResultState.Hidden,
                0,
                0),
            Case(
                "FR-008",
                "Forecast.KnownRawBelowBlockWithoutDirect_ReturnsHidden",
                IncomingDamageRead.Known(4, 5, 0),
                ForecastResultState.Hidden,
                0,
                0),
            Case(
                "FR-009",
                "Forecast.KnownFullyBlockedWithDirect_ReturnsDirectOnly",
                IncomingDamageRead.Known(4, 5, 3),
                ForecastResultState.KnownDamage,
                0,
                3),
            Case(
                "FR-010",
                "Forecast.KnownNegativeRawWithoutDirect_ReturnsHidden",
                IncomingDamageRead.Known(-5, 0, 0),
                ForecastResultState.Hidden,
                0,
                0),
            Case(
                "FR-011",
                "Forecast.KnownNegativeBlock_PreservesCurrentArithmetic",
                IncomingDamageRead.Known(5, -3, 0),
                ForecastResultState.KnownDamage,
                8,
                0),
            Case(
                "FR-012",
                "Forecast.KnownNegativeDirect_ClampsDirectLane",
                IncomingDamageRead.Known(5, 0, -4),
                ForecastResultState.KnownDamage,
                5,
                0),
            Case(
                "FR-013",
                "Forecast.InvalidReadState_ReturnsUnknown",
                new IncomingDamageRead((IncomingDamageReadState)999, 8, 2, 3),
                ForecastResultState.Unknown,
                0,
                0),
            Case(
                "FR-014",
                "Forecast.OverflowingSubtraction_FailsHidden",
                IncomingDamageRead.Known(int.MaxValue, -1, 0),
                ForecastResultState.Hidden,
                0,
                0)
        ];
    }

    private static ContractCase Case(
        string id,
        string name,
        IncomingDamageRead input,
        ForecastResultState expectedState,
        int expectedOutDamage,
        int expectedDirectHpLoss)
    {
        return new ContractCase(
            id,
            "ForecastResult",
            name,
            assert =>
            {
                var actual = new LocalDamageForecast().Calculate(input);
                assert.Equal(expectedState, actual.State, $"input={input}");
                assert.Equal(expectedOutDamage, actual.OutDamage, $"input={input}");
                assert.Equal(expectedDirectHpLoss, actual.DirectHpLoss, $"input={input}");
            });
    }
}
