namespace DamageForecast.UI;

internal readonly record struct DamageForecastHudNodeCandidate(
    bool HasExpectedName,
    bool HasExpectedType,
    bool IsOwned);

internal readonly record struct DamageForecastHudNodeResolution(
    int CanonicalIndex,
    bool CreateNew,
    bool FailClosed,
    IReadOnlyList<int> DuplicateOwnedIndexes);

internal static class DamageForecastHudNodeOwnershipPolicy
{
    public static DamageForecastHudNodeResolution Resolve(
        IReadOnlyList<DamageForecastHudNodeCandidate> candidates)
    {
        for (var index = 0; index < candidates.Count; index++)
        {
            var candidate = candidates[index];
            if (candidate.HasExpectedName && !candidate.HasExpectedType)
            {
                return new DamageForecastHudNodeResolution(
                    CanonicalIndex: -1,
                    CreateNew: false,
                    FailClosed: true,
                    DuplicateOwnedIndexes: []);
            }
        }

        var canonicalIndex = -1;
        for (var index = 0; index < candidates.Count; index++)
        {
            var candidate = candidates[index];
            if (candidate.HasExpectedName && candidate.HasExpectedType)
            {
                canonicalIndex = index;
                break;
            }
        }

        if (canonicalIndex < 0)
        {
            for (var index = 0; index < candidates.Count; index++)
            {
                var candidate = candidates[index];
                if (candidate.IsOwned && candidate.HasExpectedType)
                {
                    canonicalIndex = index;
                    break;
                }
            }
        }

        var duplicateOwnedIndexes = new List<int>();
        for (var index = 0; index < candidates.Count; index++)
        {
            var candidate = candidates[index];
            if (index != canonicalIndex && candidate.IsOwned)
            {
                duplicateOwnedIndexes.Add(index);
            }
        }

        return new DamageForecastHudNodeResolution(
            CanonicalIndex: canonicalIndex,
            CreateNew: canonicalIndex < 0,
            FailClosed: false,
            DuplicateOwnedIndexes: duplicateOwnedIndexes);
    }
}
