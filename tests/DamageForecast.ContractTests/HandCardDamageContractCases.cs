using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using DamageForecast.Combat;

internal static class HandCardDamageContractCases
{
    public static IReadOnlyList<ContractCase> Create()
    {
        return
        [
            PolicyCase(
                "HC-001",
                "HandCard.NoDamage_RemainsSupported",
                new HandCardDamageClassificationInput(
                    GenericDamageInspectionAccepted: true,
                    HasVerifiedBlockableDamage: false,
                    HasVerifiedDirectHpLoss: false),
                HandCardDamageClassification.NoDamage),
            PolicyCase(
                "HC-002",
                "HandCard.VerifiedBlockable_RemainsSupported",
                new HandCardDamageClassificationInput(
                    GenericDamageInspectionAccepted: true,
                    HasVerifiedBlockableDamage: true,
                    HasVerifiedDirectHpLoss: false),
                HandCardDamageClassification.VerifiedBlockable),
            PolicyCase(
                "HC-003",
                "HandCard.VerifiedDirect_BypassesGenericUnsupportedRejection",
                new HandCardDamageClassificationInput(
                    GenericDamageInspectionAccepted: false,
                    HasVerifiedBlockableDamage: false,
                    HasVerifiedDirectHpLoss: true),
                HandCardDamageClassification.VerifiedDirect),
            PolicyCase(
                "HC-004",
                "HandCard.TrulyUnsupportedDamage_RemainsUnsupported",
                new HandCardDamageClassificationInput(
                    GenericDamageInspectionAccepted: false,
                    HasVerifiedBlockableDamage: false,
                    HasVerifiedDirectHpLoss: false),
                HandCardDamageClassification.UnsupportedDamage),
            BuiltInCase(
                "HC-005",
                "HandCard.Burn_ClassifiesAsVerifiedBlockable",
                new Burn(),
                handCount: 1,
                nativeExecutionOrder: 0,
                expectedClassification: HandCardDamageClassification.VerifiedBlockable,
                expectedDirectAmount: null),
            BuiltInCase(
                "HC-006",
                "HandCard.Beckon_ClassifiesAsVerifiedDirect",
                new Beckon(),
                handCount: 1,
                nativeExecutionOrder: 0,
                expectedClassification: HandCardDamageClassification.VerifiedDirect,
                expectedDirectAmount: 6),
            BuiltInCase(
                "HC-007",
                "HandCard.BadLuck_ClassifiesAsVerifiedDirect",
                new BadLuck(),
                handCount: 1,
                nativeExecutionOrder: 0,
                expectedClassification: HandCardDamageClassification.VerifiedDirect,
                expectedDirectAmount: 13),
            BuiltInCase(
                "HC-008",
                "HandCard.Regret_ClassifiesAsVerifiedDirect",
                new Regret(),
                handCount: 5,
                nativeExecutionOrder: 0,
                expectedClassification: HandCardDamageClassification.VerifiedDirect,
                expectedDirectAmount: 5),
            new ContractCase(
                "HC-009",
                "HandCardDamage",
                "HandCard.MultipleSupportedCards_PreserveOrderAndDoNotDuplicate",
                assert =>
                {
                    CardModel[] cards = [new Burn(), new Beckon(), new BadLuck()];
                    var classifications = new List<HandCardDamageClassification>();
                    var directEvents = new List<UpcomingHpLossEvent>();

                    for (var index = 0; index < cards.Length; index++)
                    {
                        var result = ClassifyBuiltIn(cards[index], cards.Length, index);
                        classifications.Add(result.Classification);
                        if (result.HasVerifiedDirectHpLoss)
                        {
                            directEvents.Add(result.DirectHpLossEvent);
                        }
                    }

                    assert.Equal(3, classifications.Count, "one classification per source card");
                    assert.Equal(
                        HandCardDamageClassification.VerifiedBlockable,
                        classifications[0],
                        "Burn remains first and blockable");
                    assert.Equal(2, directEvents.Count, "two direct sources produce exactly two events");
                    assert.Equal(1, directEvents[0].NativeExecutionOrder, "Beckon native order");
                    assert.Equal(2, directEvents[1].NativeExecutionOrder, "Bad Luck native order");
                    assert.Equal(
                        HandCardDamageClassification.VerifiedDirect,
                        classifications[1],
                        "Beckon remains second and direct");
                    assert.Equal(
                        HandCardDamageClassification.VerifiedDirect,
                        classifications[2],
                        "Bad Luck remains third and direct");
                }),
            new ContractCase(
                "HC-010",
                "HandCardDamage",
                "HandCard.SupportedPlusUnsupported_FailsClosed",
                assert =>
                {
                    var supported = HandCardDamageClassificationPolicy.Classify(
                        new HandCardDamageClassificationInput(false, false, true));
                    var unsupported = HandCardDamageClassificationPolicy.Classify(
                        new HandCardDamageClassificationInput(false, false, false));
                    var aggregateSupported = supported != HandCardDamageClassification.UnsupportedDamage
                        && unsupported != HandCardDamageClassification.UnsupportedDamage;

                    assert.Equal(false, aggregateSupported, "an unsupported source keeps the aggregate conservative");
                }),
            new ContractCase(
                "HC-011",
                "HandCardDamage",
                "HandCard.ExpectedAndIncoming_UseSameSourceClassification",
                assert =>
                {
                    var input = new HandCardDamageClassificationInput(false, false, true);
                    var expectedPath = HandCardDamageClassificationPolicy.Classify(input);
                    var incomingPath = HandCardDamageClassificationPolicy.Classify(input);

                    assert.Equal(expectedPath, incomingPath, "N and -N use the same source classification");
                    assert.Equal(
                        HandCardDamageClassification.VerifiedDirect,
                        expectedPath,
                        "verified direct remains supported in both paths");
                }),
            BuiltInCase(
                "HC-012",
                "HandCard.Slimed_ClassifiesAsNoDamage",
                new Slimed(),
                handCount: 1,
                nativeExecutionOrder: 0,
                expectedClassification: HandCardDamageClassification.NoDamage,
                expectedDirectAmount: null),
            VerifiedBlockableBuiltInCase(
                "HC4R-001",
                "TurnEndStructure.Burn_OfficialSingleSelfDamageVar_Accepted",
                new Burn(),
                expectedBaseValue: 2),
            VerifiedBlockableBuiltInCase(
                "HC4R-002",
                "TurnEndStructure.Decay_OfficialSingleSelfDamageVar_Accepted",
                new Decay(),
                expectedBaseValue: 2),
            VerifiedBlockableBuiltInCase(
                "HC4R-003",
                "TurnEndStructure.Infection_OfficialSingleSelfDamageVar_Accepted",
                new Infection(),
                expectedBaseValue: 3),
            VerifiedBlockableBuiltInCase(
                "HC4R-004",
                "TurnEndStructure.Toxic_OfficialSingleSelfDamageVar_Accepted",
                new Toxic(),
                expectedBaseValue: 5),
            VerifiedBlockableBuiltInCase(
                "HC4R-005",
                "TurnEndStructure.Wither_OfficialSingleSelfDamageVar_Accepted",
                new Wither(),
                expectedBaseValue: 3),
            VerifiedNoDamageBuiltInCase(
                "HC4R-006",
                "TurnEndStructure.Debt_ParsedWithoutDamage_RemainsNoDamage",
                new Debt()),
            VerifiedNoDamageBuiltInCase(
                "HC4R-007",
                "TurnEndStructure.Doubt_ParsedWithoutDamage_RemainsNoDamage",
                new Doubt()),
            VerifiedNoDamageBuiltInCase(
                "HC4R-008",
                "TurnEndStructure.Shame_ParsedWithoutDamage_RemainsNoDamage",
                new Shame()),
            DescriptorPolicyCase(
                "HC4R-009",
                "TurnEndDescriptor.OfficialAsyncStableShape_Accepted",
                VerifiedDescriptor(TurnEndDamageOverloadKind.DamageVarStable),
                HandCardDamageClassification.VerifiedBlockable),
            DescriptorPolicyCase(
                "HC4R-010",
                "TurnEndDescriptor.OfficialAsyncBetaNullCardPlayShape_Accepted",
                VerifiedDescriptor(TurnEndDamageOverloadKind.DamageVarBeta),
                HandCardDamageClassification.VerifiedBlockable),
            DescriptorPolicyCase(
                "HC4R-011",
                "TurnEndDescriptor.OfficialSyncEquivalentShape_Accepted",
                VerifiedDescriptor(TurnEndDamageOverloadKind.DamageVarStable) with
                {
                    BodyResolution = TurnEndDamageBodyResolution.ParsedSync
                },
                HandCardDamageClassification.VerifiedBlockable),
            DescriptorPolicyCase(
                "HC4R-012",
                "TurnEndDescriptor.ExternalDamagingType_Rejected",
                VerifiedDescriptor(TurnEndDamageOverloadKind.DamageVarStable) with
                {
                    IsOfficialCurrentGameAssembly = false
                },
                HandCardDamageClassification.UnsupportedDamage),
            DescriptorPolicyCase(
                "HC4R-013",
                "TurnEndDescriptor.InheritedOrForeignTurnEndMethod_Rejected",
                VerifiedDescriptor(TurnEndDamageOverloadKind.DamageVarStable) with
                {
                    MethodDeclaredOnConcreteType = false
                },
                HandCardDamageClassification.UnsupportedDamage),
            DescriptorPolicyCase(
                "HC4R-014",
                "TurnEndDescriptor.UnreadableMethodBody_Rejected",
                VerifiedDescriptor(TurnEndDamageOverloadKind.DamageVarStable) with
                {
                    BodyResolution = TurnEndDamageBodyResolution.Unreadable
                },
                HandCardDamageClassification.UnsupportedDamage),
            DescriptorPolicyCase(
                "HC4R-015",
                "TurnEndDescriptor.MultipleDamageCalls_Rejected",
                VerifiedDescriptor(TurnEndDamageOverloadKind.DamageVarStable) with
                {
                    DamageCallCount = 2
                },
                HandCardDamageClassification.UnsupportedDamage),
            DescriptorPolicyCase(
                "HC4R-016",
                "TurnEndDescriptor.DecimalDamageOverload_RejectedByGenericPolicy",
                VerifiedDescriptor(TurnEndDamageOverloadKind.Decimal),
                HandCardDamageClassification.UnsupportedDamage),
            DescriptorPolicyCase(
                "HC4R-017",
                "TurnEndDescriptor.UnknownTargetProvenance_Rejected",
                VerifiedDescriptor(TurnEndDamageOverloadKind.DamageVarStable) with
                {
                    TargetIsSameCardOwnerCreature = false
                },
                HandCardDamageClassification.UnsupportedDamage),
            DescriptorPolicyCase(
                "HC4R-018",
                "TurnEndDescriptor.UnknownAmountProvenance_Rejected",
                VerifiedDescriptor(TurnEndDamageOverloadKind.DamageVarStable) with
                {
                    AmountIsSameCardDynamicVarsDamage = false
                },
                HandCardDamageClassification.UnsupportedDamage),
            DescriptorPolicyCase(
                "HC4R-019",
                "TurnEndDescriptor.UnknownSourceCardProvenance_Rejected",
                VerifiedDescriptor(TurnEndDamageOverloadKind.DamageVarStable) with
                {
                    SourceCardIsSameCard = false
                },
                HandCardDamageClassification.UnsupportedDamage),
            DescriptorPolicyCase(
                "HC4R-020",
                "TurnEndDescriptor.BetaNonNullCardPlay_Rejected",
                VerifiedDescriptor(TurnEndDamageOverloadKind.DamageVarBeta) with
                {
                    BetaCardPlayIsNull = false
                },
                HandCardDamageClassification.UnsupportedDamage),
            DescriptorPolicyCase(
                "HC4R-021",
                "TurnEndDescriptor.UnsupportedControlFlow_Rejected",
                VerifiedDescriptor(TurnEndDamageOverloadKind.DamageVarStable) with
                {
                    ControlFlowIsSupported = false
                },
                HandCardDamageClassification.UnsupportedDamage),
            DescriptorPolicyCase(
                "HC4R-022",
                "TurnEndDescriptor.MultipleDamageVars_Rejected",
                VerifiedDescriptor(TurnEndDamageOverloadKind.DamageVarStable) with
                {
                    DamageVarCount = 2
                },
                HandCardDamageClassification.UnsupportedDamage),
            DescriptorPolicyCase(
                "HC4R-023",
                "TurnEndDescriptor.UnsupportedDamageVarValue_Rejected",
                VerifiedDescriptor(TurnEndDamageOverloadKind.DamageVarStable) with
                {
                    DamageVarBaseValueIsSupported = false
                },
                HandCardDamageClassification.UnsupportedDamage),
            DescriptorPolicyCase(
                "HC4R-024",
                "TurnEndDescriptor.UnsupportedDamageVarProps_Rejected",
                VerifiedDescriptor(TurnEndDamageOverloadKind.DamageVarStable) with
                {
                    DamageVarPropsAreExactUnpoweredMove = false
                },
                HandCardDamageClassification.UnsupportedDamage),
            DescriptorPolicyCase(
                "HC4R-025",
                "TurnEndDescriptor.ExternalParsedNoDamage_RemainsNoDamage",
                VerifiedDescriptor(TurnEndDamageOverloadKind.None) with
                {
                    IsOfficialCurrentGameAssembly = false,
                    DamageCallCount = 0,
                    TargetIsSameCardOwnerCreature = false,
                    AmountIsSameCardDynamicVarsDamage = false,
                    SourceCardIsSameCard = false,
                    DamageVarCount = 0,
                    DamageVarBaseValueIsSupported = false,
                    DamageVarPropsAreExactUnpoweredMove = false
                },
                HandCardDamageClassification.NoDamage),
            DescriptorPolicyCase(
                "HC4R-026",
                "TurnEndDescriptor.NoTurnEndEffect_RemainsNoDamage",
                VerifiedDescriptor(TurnEndDamageOverloadKind.None) with
                {
                    BodyResolution = TurnEndDamageBodyResolution.NoTurnEndEffect,
                    DamageCallCount = 0,
                    TargetIsSameCardOwnerCreature = false,
                    AmountIsSameCardDynamicVarsDamage = false,
                    SourceCardIsSameCard = false,
                    DamageVarCount = 0,
                    DamageVarBaseValueIsSupported = false,
                    DamageVarPropsAreExactUnpoweredMove = false
                },
                HandCardDamageClassification.NoDamage),
            DescriptorPolicyCase(
                "HC4R-027",
                "TurnEndDescriptor.VerifiedDirect_PreservesPriority",
                VerifiedDescriptor(TurnEndDamageOverloadKind.Decimal) with
                {
                    HasVerifiedDirectHpLoss = true
                },
                HandCardDamageClassification.VerifiedDirect),
            new ContractCase(
                "HC4R-028",
                "HandCardDamage",
                "TurnEndDescriptor.FailClosedReasons_AreStable",
                assert =>
                {
                    var verified = VerifiedDescriptor(TurnEndDamageOverloadKind.DamageVarStable);
                    var cases = new[]
                    {
                        (
                            verified with
                            {
                                BodyResolution = TurnEndDamageBodyResolution.NoTurnEndEffect,
                                DamageCallCount = 0
                            },
                            TurnEndDamageSupportReason.NoTurnEndEffect),
                        (
                            verified with
                            {
                                DamageCallCount = 0,
                                OverloadKind = TurnEndDamageOverloadKind.None
                            },
                            TurnEndDamageSupportReason.NoDamageCall),
                        (
                            verified with { IsOfficialCurrentGameAssembly = false },
                            TurnEndDamageSupportReason.ExternalDamagingType),
                        (
                            verified with { MethodDeclaredOnConcreteType = false },
                            TurnEndDamageSupportReason.InheritedOrForeignTurnEndMethod),
                        (
                            verified with { BodyResolution = TurnEndDamageBodyResolution.Unreadable },
                            TurnEndDamageSupportReason.UnreadableMethodBody),
                        (
                            verified with { BodyResolution = (TurnEndDamageBodyResolution)(-1) },
                            TurnEndDamageSupportReason.UnreadableMethodBody),
                        (
                            verified with { DamageCallCount = 2 },
                            TurnEndDamageSupportReason.DamageCallCountMismatch),
                        (
                            verified with { OverloadKind = TurnEndDamageOverloadKind.Decimal },
                            TurnEndDamageSupportReason.UnsupportedDamageOverload),
                        (
                            verified with { TargetIsSameCardOwnerCreature = false },
                            TurnEndDamageSupportReason.UnknownTargetProvenance),
                        (
                            verified with { AmountIsSameCardDynamicVarsDamage = false },
                            TurnEndDamageSupportReason.UnknownAmountProvenance),
                        (
                            verified with { SourceCardIsSameCard = false },
                            TurnEndDamageSupportReason.UnknownSourceCardProvenance),
                        (
                            VerifiedDescriptor(TurnEndDamageOverloadKind.DamageVarBeta) with
                            {
                                BetaCardPlayIsNull = false
                            },
                            TurnEndDamageSupportReason.UnexpectedBetaCardPlay),
                        (
                            verified with { ControlFlowIsSupported = false },
                            TurnEndDamageSupportReason.UnsupportedControlFlow),
                        (
                            verified with { DamageVarCount = 2 },
                            TurnEndDamageSupportReason.DamageVarCountMismatch),
                        (
                            verified with { DamageVarBaseValueIsSupported = false },
                            TurnEndDamageSupportReason.UnsupportedDamageVarValue),
                        (
                            verified with { DamageVarPropsAreExactUnpoweredMove = false },
                            TurnEndDamageSupportReason.UnsupportedDamageVarProps),
                        (
                            verified,
                            TurnEndDamageSupportReason.VerifiedSingleOfficialBlockableDamage)
                    };

                    foreach (var item in cases)
                    {
                        assert.Equal(
                            item.Item2,
                            ReadDescriptorPolicyResult(item.Item1).Reason,
                            item.Item1.ToString());
                    }
                })
        ];
    }

    private static ContractCase VerifiedBlockableBuiltInCase(
        string id,
        string name,
        CardModel card,
        decimal expectedBaseValue)
    {
        return new ContractCase(
            id,
            "HandCardDamage",
            name,
            assert =>
            {
                var accepted =
                    CardTurnEndDamageInspector.TryGetVerifiedSingleBlockableDamageVar(card, out var damageVar);

                assert.Equal(true, accepted, $"{card.GetType().Name} generic inspection");
                assert.Equal(true, damageVar is not null, $"{card.GetType().Name} verified DamageVar");
                assert.Equal(expectedBaseValue, damageVar!.BaseValue, $"{card.GetType().Name} DamageVar.BaseValue");
                assert.Equal(
                    ValueProp.Unpowered | ValueProp.Move,
                    damageVar.Props,
                    $"{card.GetType().Name} DamageVar.Props");
                assert.Equal(
                    typeof(CardModel).Assembly,
                    card.GetType().Assembly,
                    $"{card.GetType().Name} official game assembly");
            });
    }

    private static ContractCase VerifiedNoDamageBuiltInCase(string id, string name, CardModel card)
    {
        return new ContractCase(
            id,
            "HandCardDamage",
            name,
            assert =>
            {
                var accepted =
                    CardTurnEndDamageInspector.TryGetVerifiedSingleBlockableDamageVar(card, out var damageVar);

                assert.Equal(true, accepted, $"{card.GetType().Name} parsed no-damage inspection");
                assert.Equal(true, damageVar is null, $"{card.GetType().Name} has no verified DamageVar");
            });
    }

    private static ContractCase DescriptorPolicyCase(
        string id,
        string name,
        TurnEndDamageDescriptorContractInput input,
        HandCardDamageClassification expected)
    {
        return new ContractCase(
            id,
            "HandCardDamage",
            name,
            assert => assert.Equal(expected, ClassifyDescriptorContract(input), input.ToString()));
    }

    private static TurnEndDamageDescriptorContractInput VerifiedDescriptor(
        TurnEndDamageOverloadKind overloadKind)
    {
        return new TurnEndDamageDescriptorContractInput(
            HasVerifiedDirectHpLoss: false,
            IsOfficialCurrentGameAssembly: true,
            MethodDeclaredOnConcreteType: true,
            BodyResolution: TurnEndDamageBodyResolution.ParsedAsync,
            DamageCallCount: 1,
            OverloadKind: overloadKind,
            TargetIsSameCardOwnerCreature: true,
            AmountIsSameCardDynamicVarsDamage: true,
            SourceCardIsSameCard: true,
            BetaCardPlayIsNull: true,
            ControlFlowIsSupported: true,
            DamageVarCount: 1,
            DamageVarBaseValueIsSupported: true,
            DamageVarPropsAreExactUnpoweredMove: true);
    }

    private static HandCardDamageClassification ClassifyDescriptorContract(
        TurnEndDamageDescriptorContractInput input)
    {
        if (input.HasVerifiedDirectHpLoss)
        {
            return HandCardDamageClassificationPolicy.Classify(
                new HandCardDamageClassificationInput(
                    GenericDamageInspectionAccepted: false,
                    HasVerifiedBlockableDamage: false,
                    HasVerifiedDirectHpLoss: true));
        }

        var result = ReadDescriptorPolicyResult(input);
        return result.Decision switch
        {
            TurnEndDamageDecision.NoDamage => HandCardDamageClassification.NoDamage,
            TurnEndDamageDecision.VerifiedBlockable => HandCardDamageClassification.VerifiedBlockable,
            _ => HandCardDamageClassification.UnsupportedDamage
        };
    }

    private static TurnEndDamagePolicyResult ReadDescriptorPolicyResult(
        TurnEndDamageDescriptorContractInput input)
    {
        var descriptor = new TurnEndDamageShapeDescriptor(
            input.IsOfficialCurrentGameAssembly,
            input.MethodDeclaredOnConcreteType,
            input.BodyResolution,
            input.DamageCallCount,
            input.OverloadKind,
            input.TargetIsSameCardOwnerCreature,
            input.AmountIsSameCardDynamicVarsDamage,
            input.SourceCardIsSameCard,
            input.BetaCardPlayIsNull,
            input.ControlFlowIsSupported);
        var instance = new TurnEndDamageInstanceShape(
            input.DamageVarCount,
            input.DamageVarBaseValueIsSupported,
            input.DamageVarPropsAreExactUnpoweredMove);
        return VerifiedTurnEndDamagePolicy.ClassifyDescriptor(descriptor, instance);
    }

    private static ContractCase PolicyCase(
        string id,
        string name,
        HandCardDamageClassificationInput input,
        HandCardDamageClassification expected)
    {
        return new ContractCase(
            id,
            "HandCardDamage",
            name,
            assert => assert.Equal(expected, HandCardDamageClassificationPolicy.Classify(input), input.ToString()));
    }

    private static ContractCase BuiltInCase(
        string id,
        string name,
        CardModel card,
        int handCount,
        int nativeExecutionOrder,
        HandCardDamageClassification expectedClassification,
        int? expectedDirectAmount)
    {
        return new ContractCase(
            id,
            "HandCardDamage",
            name,
            assert =>
            {
                var actual = ClassifyBuiltIn(card, handCount, nativeExecutionOrder);
                if (expectedDirectAmount is { } directAmount)
                {
                    assert.Equal(true, actual.HasVerifiedDirectHpLoss, $"{card.GetType().Name} fixed reader");
                    assert.Equal(directAmount, actual.DirectHpLossEvent.VerifiedHpLoss, card.GetType().Name);
                    assert.Equal(HpLossDisplayLane.DirectHpLoss, actual.DirectHpLossEvent.DisplayLane, card.GetType().Name);
                    assert.Equal(nativeExecutionOrder, actual.DirectHpLossEvent.NativeExecutionOrder, card.GetType().Name);
                }

                assert.Equal(expectedClassification, actual.Classification, card.GetType().Name);
            });
    }

    private static BuiltInClassificationResult ClassifyBuiltIn(
        CardModel card,
        int handCount,
        int nativeExecutionOrder)
    {
        var genericAccepted =
            CardTurnEndDamageInspector.TryGetVerifiedSingleBlockableDamageVar(card, out var damageVar);
        var hasVerifiedDirectHpLoss = VerifiedFixedTurnEndHpLossReader.TryReadEvent(
            card,
            handCount,
            nativeExecutionOrder,
            out var directHpLossEvent);
        var classification = HandCardDamageClassificationPolicy.Classify(
            new HandCardDamageClassificationInput(
                genericAccepted,
                damageVar is not null,
                hasVerifiedDirectHpLoss));

        return new BuiltInClassificationResult(
            classification,
            hasVerifiedDirectHpLoss,
            directHpLossEvent);
    }

    private readonly record struct BuiltInClassificationResult(
        HandCardDamageClassification Classification,
        bool HasVerifiedDirectHpLoss,
        UpcomingHpLossEvent DirectHpLossEvent);

    private readonly record struct TurnEndDamageDescriptorContractInput(
        bool HasVerifiedDirectHpLoss,
        bool IsOfficialCurrentGameAssembly,
        bool MethodDeclaredOnConcreteType,
        TurnEndDamageBodyResolution BodyResolution,
        int DamageCallCount,
        TurnEndDamageOverloadKind OverloadKind,
        bool TargetIsSameCardOwnerCreature,
        bool AmountIsSameCardDynamicVarsDamage,
        bool SourceCardIsSameCard,
        bool BetaCardPlayIsNull,
        bool ControlFlowIsSupported,
        int DamageVarCount,
        bool DamageVarBaseValueIsSupported,
        bool DamageVarPropsAreExactUnpoweredMove);
}
