using DamageForecast.Combat;

internal static class HpLossModifierContractCases
{
    public static IReadOnlyList<ContractCase> Create()
    {
        return
        [
            Case(
                "HM-001",
                "HpLoss.NoModifiers_AggregatesLanesAndClampsNegative",
                [Event("block", 0, HpLossDisplayLane.Blockable, 5), Event("direct", 1, HpLossDisplayLane.DirectHpLoss, 3), Event("negative", 2, HpLossDisplayLane.Blockable, -8)],
                Input(),
                HpLossResultModificationState.Supported,
                5,
                3),
            Case(
                "HM-002",
                "HpLoss.NativeOrder_AppliesBeforeBudget",
                [Event("later", 2, HpLossDisplayLane.Blockable, 5), Event("earlier", 1, HpLossDisplayLane.DirectHpLoss, 4)],
                Input(relics: [Remnant(6)]),
                HpLossResultModificationState.Supported,
                2,
                4),
            Case(
                "HM-003",
                "HpLoss.EqualOrder_PreservesInputOrder",
                [Event("first", 1, HpLossDisplayLane.DirectHpLoss, 4), Event("second", 1, HpLossDisplayLane.Blockable, 4)],
                Input(relics: [Remnant(5)]),
                HpLossResultModificationState.Supported,
                1,
                4),
            Case(
                "HM-004",
                "HpLoss.Intangible.SingleDirectEvent_ClampsToOne",
                [Event("direct", 0, HpLossDisplayLane.DirectHpLoss, 9)],
                Input(intangible: true),
                HpLossResultModificationState.Supported,
                0,
                1),
            Case(
                "HM-005",
                "HpLoss.Intangible.AggregateDirectEvent_IsUnsupported",
                [Event("aggregate", 0, HpLossDisplayLane.DirectHpLoss, 9, single: false)],
                Input(intangible: true),
                HpLossResultModificationState.UnsupportedBecauseAggregateDirectHpLossWithIntangible,
                0,
                0),
            Case(
                "HM-006",
                "HpLoss.Intangible.DoesNotModifyBlockableLane",
                [Event("blockable", 0, HpLossDisplayLane.Blockable, 9)],
                Input(intangible: true),
                HpLossResultModificationState.Supported,
                9,
                0),
            Case(
                "HM-007",
                "HpLoss.TungstenRod_ReducesEachVerifiedEvent",
                [Event("one", 0, HpLossDisplayLane.Blockable, 5), Event("two", 1, HpLossDisplayLane.DirectHpLoss, 3)],
                Input(relics: [Tungsten(1)]),
                HpLossResultModificationState.Supported,
                4,
                2),
            Case(
                "HM-008",
                "HpLoss.TungstenRod.AggregateEvent_IsUnsupported",
                [Event("aggregate", 0, HpLossDisplayLane.Blockable, 8, single: false)],
                Input(relics: [Tungsten(1)]),
                HpLossResultModificationState.UnsupportedBecauseAggregateEnemyHpLossWithTungstenRod,
                0,
                0),
            Case(
                "HM-009",
                "HpLoss.BeatingRemnant_UsesObservedSpentBudget",
                [Event("damage", 0, HpLossDisplayLane.Blockable, 5)],
                Input(observed: 18, relics: [Remnant(20)]),
                HpLossResultModificationState.Supported,
                2,
                0),
            Case(
                "HM-010",
                "HpLoss.BeatingRemnant.ZeroRemainingBudget_ReturnsZero",
                [Event("damage", 0, HpLossDisplayLane.Blockable, 5)],
                Input(observed: 20, relics: [Remnant(20)]),
                HpLossResultModificationState.Supported,
                0,
                0),
            Case(
                "HM-011",
                "HpLoss.BeatingRemnant.InvalidRemainingBudget_IsUnsupported",
                [Event("damage", 0, HpLossDisplayLane.Blockable, 5)],
                Input(observed: 21, relics: [Remnant(20)]),
                HpLossResultModificationState.UnsupportedBecauseInvalidRemainingBudget,
                0,
                0),
            Case(
                "HM-012",
                "HpLoss.TungstenBeforeRemnant_PreservesCurrentOrder",
                [Event("damage", 0, HpLossDisplayLane.DirectHpLoss, 5)],
                Input(observed: 19, relics: [Remnant(20), Tungsten(1)]),
                HpLossResultModificationState.Supported,
                0,
                1),
            Case(
                "HM-013",
                "HpLoss.IntangibleThenTungstenThenRemnant_UsesDeclaredOrder",
                [Event("damage", 0, HpLossDisplayLane.DirectHpLoss, 9)],
                Input(intangible: true, observed: 19, relics: [Remnant(20), Tungsten(1)]),
                HpLossResultModificationState.Supported,
                0,
                0),
            Case(
                "HM-014",
                "HpLoss.PowerModifiersDisabled_SkipsIntangible",
                [Event("damage", 0, HpLossDisplayLane.DirectHpLoss, 9)],
                Input(intangible: false),
                HpLossResultModificationState.Supported,
                0,
                9),
            Case(
                "HM-015",
                "HpLoss.RelicModifiersDisabled_SkipsRelics",
                [Event("damage", 0, HpLossDisplayLane.DirectHpLoss, 9)],
                Input(relics: []),
                HpLossResultModificationState.Supported,
                0,
                9),
            Case(
                "HM-016",
                "HpLoss.UnsupportedEvent_ReturnsCompletedLanes",
                [Event("completed", 0, HpLossDisplayLane.Blockable, 2), Event("unsupported", 1, HpLossDisplayLane.DirectHpLoss, 9, single: false)],
                Input(intangible: true),
                HpLossResultModificationState.UnsupportedBecauseAggregateDirectHpLossWithIntangible,
                2,
                0),
            Case(
                "HM-017",
                "HpLoss.NegativeObservedBudget_ClampsToZero",
                [Event("damage", 0, HpLossDisplayLane.Blockable, 5)],
                Input(observed: -7, relics: [Remnant(20)]),
                HpLossResultModificationState.Supported,
                5,
                0),
            Case(
                "HM-018",
                "HpLoss.UnknownRelicModifier_IsUnsupported",
                [Event("damage", 0, HpLossDisplayLane.Blockable, 5)],
                Input(relics: [new HpLossRelicModifierInput((HpLossRelicModifierKind)999, 1)]),
                HpLossResultModificationState.UnsupportedBecauseEventGranularityUnknown,
                0,
                0)
        ];
    }

    private static ContractCase Case(
        string id,
        string name,
        IReadOnlyList<UpcomingHpLossEvent> events,
        HpLossModifierPolicyInput input,
        HpLossResultModificationState expectedState,
        int expectedBlockable,
        int expectedDirect)
    {
        return new ContractCase(
            id,
            "HpLossModifier",
            name,
            assert =>
            {
                var actual = VerifiedHpLossResultModifier.ApplyPolicy(events, input);
                var context = $"events={string.Join(',', events.Select(e => e.Source))}; input={input}";
                assert.Equal(expectedState, actual.State, context);
                assert.Equal(expectedBlockable, actual.BlockableHpLoss, context);
                assert.Equal(expectedDirect, actual.DirectHpLoss, context);
            });
    }

    private static HpLossModifierPolicyInput Input(
        bool intangible = false,
        int observed = 0,
        IReadOnlyList<HpLossRelicModifierInput>? relics = null)
    {
        return new HpLossModifierPolicyInput(
            intangible,
            relics ?? Array.Empty<HpLossRelicModifierInput>(),
            observed);
    }

    private static HpLossRelicModifierInput Tungsten(decimal reduction)
    {
        return new HpLossRelicModifierInput(HpLossRelicModifierKind.TungstenRod, reduction);
    }

    private static HpLossRelicModifierInput Remnant(decimal maximum)
    {
        return new HpLossRelicModifierInput(HpLossRelicModifierKind.BeatingRemnant, maximum);
    }

    private static UpcomingHpLossEvent Event(
        string source,
        int order,
        HpLossDisplayLane lane,
        int amount,
        bool single = true)
    {
        return new UpcomingHpLossEvent(source, order, lane, amount, single);
    }
}
