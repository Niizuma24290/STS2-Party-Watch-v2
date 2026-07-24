using DamageForecast.Compatibility;

namespace DamageForecast.Identity;

internal static class LegacyIdentityConflictDetector
{
    public static IReadOnlyList<string> FindLoadedLegacyAssemblyNames()
    {
        var currentAssemblyName = typeof(LegacyIdentityConflictDetector).Assembly.GetName().Name ?? string.Empty;
        var loadedAssemblyNames = AppDomain.CurrentDomain.GetAssemblies()
            .Select(assembly => assembly.GetName().Name);
        return FindConflicts(loadedAssemblyNames, currentAssemblyName, LegacyIdentityDescriptor.ModId);
    }

    internal static IReadOnlyList<string> FindConflicts(
        IEnumerable<string?> loadedAssemblyNames,
        string currentAssemblyName,
        string legacyAssemblyName)
    {
        return loadedAssemblyNames
            .Where(name => !string.IsNullOrWhiteSpace(name)
                && !string.Equals(name, currentAssemblyName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(name, legacyAssemblyName, StringComparison.OrdinalIgnoreCase))
            .Select(name => name!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
