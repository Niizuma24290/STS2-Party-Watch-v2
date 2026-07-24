using DamageForecast.Combat;

internal static class PoisonPolicyContractCases
{
    public static IEnumerable<ContractCase> Create()
    {
        yield return Case("PO-001", "Poison.None_PreservesIntent", new(10, 0, 0), 0, true);
        yield return Case("PO-002", "Poison.OrdinaryNoKill_PreservesIntent", new(10, 4, 0), 4, true);
        yield return Case("PO-003", "Poison.OrdinaryExactLethal_RemovesIntent", new(7, 4, 1), 7, false);
        yield return Case("PO-004", "Poison.OrdinaryOverkill_RemovesIntent", new(6, 4, 1), 7, false);

        yield return new ContractCase(
            "PO-005",
            "PoisonPolicy",
            "Poison.AccelerantTriggerCount_CapsAtPoison",
            assert => assert.Equal(2, PoisonTickPolicy.PreviewTriggerCount(2, 99)));
        yield return new ContractCase(
            "PO-006",
            "PoisonPolicy",
            "Poison.TickDamage_DecrementsBeforeNextTick",
            assert => assert.Equal(9, PoisonTickPolicy.PreviewOrdinaryDamage(4, 2)));

        yield return Case(
            "PO-007",
            "Poison.IntangibleBelowLethal_PreservesIntent",
            new(3, 5, 1, HasIntangible: true),
            2,
            true);
        yield return Case(
            "PO-008",
            "Poison.IntangibleExactLethal_RemovesIntent",
            new(3, 5, 2, HasIntangible: true),
            3,
            false);
        yield return Case(
            "PO-009",
            "Poison.IntangibleWithHardToKill_IsConservative",
            new(1, 9, 9, HasIntangible: true, HasHardToKill: true),
            0,
            true);
        yield return Case(
            "PO-010",
            "Poison.IntangibleWithSlippery_IsConservative",
            new(1, 9, 9, HasIntangible: true, HasSlippery: true, SlipperyAmount: 1),
            0,
            true);
        yield return Case(
            "PO-011",
            "Poison.IntangibleWithHardenedShell_IsConservative",
            new(1, 9, 9, HasIntangible: true, HasHardenedShell: true, HardenedShellRemainingBudget: 9),
            0,
            true);

        yield return Case(
            "PO-012",
            "Poison.Slippery_ConsumesOneLayerPerPositiveTick",
            new(4, 4, 2, HasSlippery: true, SlipperyAmount: 2, ModifiedTickDamages: [4, 3, 2]),
            4,
            false);
        yield return Case(
            "PO-013",
            "Poison.Slippery_LeavesLaterTicksUncapped",
            new(6, 4, 2, HasSlippery: true, SlipperyAmount: 1, ModifiedTickDamages: [4, 3, 2]),
            6,
            false);
        yield return Case(
            "PO-014",
            "Poison.SlipperyUnavailable_FallsBackToNativePreview",
            new(5, 9, 0, HasSlippery: true, SlipperyAmount: 1, NativePreviewedDamage: 5),
            5,
            false);

        yield return Case(
            "PO-015",
            "Poison.HardenedShellBudgetBelowHp_PreservesIntent",
            new(8, 20, 0, HasHardenedShell: true, HardenedShellRemainingBudget: 7, NativePreviewedDamage: 20),
            0,
            true);
        yield return Case(
            "PO-016",
            "Poison.HardenedShellBudgetEqualHp_ExactLethal",
            new(8, 20, 0, HasHardenedShell: true, HardenedShellRemainingBudget: 8, NativePreviewedDamage: 20),
            8,
            false);
        yield return Case(
            "PO-017",
            "Poison.HardenedShellBudgetEnoughForOverkill_RemovesIntent",
            new(8, 20, 0, HasHardenedShell: true, HardenedShellRemainingBudget: 12, NativePreviewedDamage: 10),
            10,
            false);
        yield return Case(
            "PO-018",
            "Poison.HardenedShellBudgetUnavailable_PreservesIntent",
            new(1, 20, 0, HasHardenedShell: true),
            0,
            true);

        yield return Case(
            "PO-019",
            "Poison.HardToKillNativePreviewAvailable_UsesNativeDamage",
            new(9, 54, 0, HasHardToKill: true, NativePreviewedDamage: 9),
            9,
            false);
        yield return Case(
            "PO-020",
            "Poison.HardToKillNativePreviewUnavailable_PreservesIntent",
            new(9, 54, 0, HasHardToKill: true),
            0,
            true);
        yield return Case(
            "PO-021",
            "Poison.NativePreviewAvailable_TakesPriorityOverOrdinaryMath",
            new(4, 20, 0, NativePreviewedDamage: 3),
            3,
            true);

        yield return new ContractCase(
            "PO-022",
            "PoisonPolicy",
            "Poison.IntentRemoval_OnlyMatchesPreviewedStableIdentity",
            assert =>
            {
                var matching = PoisonIntentIdentityPolicy.ShouldRetainCurrentIntent("enemy-a", "enemy-a", false);
                var otherEnemy = PoisonIntentIdentityPolicy.ShouldRetainCurrentIntent("enemy-b", "enemy-a", false);
                assert.True(
                    !matching && otherEnemy,
                    "enemy-a=false; enemy-b=true",
                    $"enemy-a={matching}; enemy-b={otherEnemy}",
                    "a lethal preview removes only the matching stable enemy identity contribution");
            });
        yield return new ContractCase(
            "PO-023",
            "PoisonPolicy",
            "Poison.NonLethalPreview_RetainsMatchingStableIdentity",
            assert => assert.Equal(
                true,
                PoisonIntentIdentityPolicy.ShouldRetainCurrentIntent("enemy-a", "enemy-a", true)));
        yield return Case(
            "PO-024",
            "Poison.SlipperyLethalPartialSequence_DoesNotRequireLaterHookReads",
            new(3, 4, 2, HasSlippery: true, SlipperyAmount: 1, ModifiedTickDamages: [4, 3]),
            4,
            false);
    }

    private static ContractCase Case(
        string id,
        string name,
        PoisonTickPolicyInput input,
        int expectedDamage,
        bool expectedIntentWillExecute) =>
        new(
            id,
            "PoisonPolicy",
            name,
            assert =>
            {
                var actual = PoisonTickPolicy.Preview(input);
                assert.True(
                    actual.PreviewedPoisonDamage == expectedDamage
                    && actual.WillRemainAliveBeforeAction == expectedIntentWillExecute
                    && actual.WillExecuteCurrentIntent == expectedIntentWillExecute,
                    $"damage={expectedDamage}; remainsAlive={expectedIntentWillExecute}; executesIntent={expectedIntentWillExecute}",
                    $"damage={actual.PreviewedPoisonDamage}; remainsAlive={actual.WillRemainAliveBeforeAction}; executesIntent={actual.WillExecuteCurrentIntent}");
            });
}
