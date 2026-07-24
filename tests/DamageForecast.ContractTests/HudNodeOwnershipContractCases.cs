using DamageForecast.UI;

internal static class HudNodeOwnershipContractCases
{
    public static IEnumerable<ContractCase> Create()
    {
        yield return new(
            "HN-001",
            "HudNodeOwnership",
            "HudNode.NoCandidate_CreatesExactlyOneCanonicalNode",
            assert =>
            {
                var resolution = DamageForecastHudNodeOwnershipPolicy.Resolve([]);
                assert.True(
                    resolution.CreateNew
                    && !resolution.FailClosed
                    && resolution.CanonicalIndex == -1
                    && resolution.DuplicateOwnedIndexes.Count == 0,
                    "create=true; failClosed=false; canonical=-1; duplicates=0",
                    resolution.ToString());
            });

        yield return new(
            "HN-002",
            "HudNodeOwnership",
            "HudNode.ExactCorrectNode_IsReusedAcrossRepeatedRefreshes",
            assert =>
            {
                var candidate = new[] { new DamageForecastHudNodeCandidate(true, true, true) };
                var allReused = Enumerable.Range(0, 100)
                    .Select(_ => DamageForecastHudNodeOwnershipPolicy.Resolve(candidate))
                    .All(resolution => !resolution.CreateNew
                        && !resolution.FailClosed
                        && resolution.CanonicalIndex == 0
                        && resolution.DuplicateOwnedIndexes.Count == 0);
                assert.Equal(true, allReused, "100 repeated resolutions reuse the same canonical node");
            });

        yield return new(
            "HN-003",
            "HudNodeOwnership",
            "HudNode.ExactWrongType_FailsClosed",
            assert =>
            {
                var resolution = DamageForecastHudNodeOwnershipPolicy.Resolve(
                    [new DamageForecastHudNodeCandidate(true, false, false)]);
                assert.True(
                    resolution.FailClosed && !resolution.CreateNew && resolution.CanonicalIndex == -1,
                    "failClosed=true; create=false; canonical=-1",
                    resolution.ToString());
            });

        yield return new(
            "HN-004",
            "HudNodeOwnership",
            "HudNode.MultipleOwnedNodes_KeepCanonicalAndRetireDuplicates",
            assert =>
            {
                var resolution = DamageForecastHudNodeOwnershipPolicy.Resolve(
                [
                    new DamageForecastHudNodeCandidate(false, true, true),
                    new DamageForecastHudNodeCandidate(true, true, true),
                    new DamageForecastHudNodeCandidate(false, true, true),
                    new DamageForecastHudNodeCandidate(false, true, false)
                ]);
                assert.True(
                    !resolution.CreateNew
                    && !resolution.FailClosed
                    && resolution.CanonicalIndex == 1
                    && resolution.DuplicateOwnedIndexes.SequenceEqual([0, 2]),
                    "canonical=1; duplicate owned nodes=0,2; unowned node untouched",
                    $"canonical={resolution.CanonicalIndex}; duplicates={string.Join(',', resolution.DuplicateOwnedIndexes)}");
            });

        yield return new(
            "HN-005",
            "HudNodeOwnership",
            "HudNode.OwnedWrongTypeWithDifferentName_IsRetiredWithoutTakingOverUnownedNode",
            assert =>
            {
                var resolution = DamageForecastHudNodeOwnershipPolicy.Resolve(
                [
                    new DamageForecastHudNodeCandidate(true, true, false),
                    new DamageForecastHudNodeCandidate(false, false, true)
                ]);
                assert.True(
                    !resolution.CreateNew
                    && !resolution.FailClosed
                    && resolution.CanonicalIndex == 0
                    && resolution.DuplicateOwnedIndexes.SequenceEqual([1]),
                    "reuse the exact correct node; retire the separately named owned stale node",
                    $"canonical={resolution.CanonicalIndex}; duplicates={string.Join(',', resolution.DuplicateOwnedIndexes)}");
            });
    }
}
