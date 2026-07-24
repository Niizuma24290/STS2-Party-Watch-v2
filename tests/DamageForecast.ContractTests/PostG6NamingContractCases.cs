using System.Reflection;
using DamageForecast;
using DamageForecast.Patches;
using DamageForecast.Settings;

internal static class PostG6NamingContractCases
{
    public static IEnumerable<ContractCase> Create()
    {
        yield return new(
            "C1-001",
            "PostG6Naming",
            "C1.Contract_IsExplicitlyApprovedAndComplete",
            assert =>
            {
                var contract = PostG6NamingContractFixture.Contract;
                var uniqueLegacyTokens = contract.OrdinarySymbolMappings
                    .Concat(contract.MsbuildMappings)
                    .Select(mapping => mapping.Legacy)
                    .Distinct(StringComparer.Ordinal)
                    .Count();
                assert.True(
                    contract.SchemaVersion == 1
                    && contract.Status == "c1-complete"
                    && contract.Approval.Gate == "C1"
                    && contract.FileMappings.Count == 9
                    && contract.GodotNodeMappings.Count == 3
                    && uniqueLegacyTokens == contract.OrdinarySymbolMappings.Count + contract.MsbuildMappings.Count,
                    "schema=1; status=c1-complete; gate=C1; mappings complete and unique",
                    $"schema={contract.SchemaVersion}; status={contract.Status}; files={contract.FileMappings.Count}; nodes={contract.GodotNodeMappings.Count}; unique={uniqueLegacyTokens}");
            });

        yield return new(
            "C1-002",
            "PostG6Naming",
            "C1.OrdinaryFiles_UseDamageForecastNamesOnly",
            assert =>
            {
                var contract = PostG6NamingContractFixture.Contract;
                var missingCurrent = contract.FileMappings
                    .Where(mapping => !File.Exists(PostG6NamingContractFixture.Resolve(mapping.Current)))
                    .Select(mapping => mapping.Current)
                    .ToArray();
                var remainingLegacy = contract.FileMappings
                    .Where(mapping => File.Exists(PostG6NamingContractFixture.Resolve(mapping.Legacy)))
                    .Select(mapping => mapping.Legacy)
                    .ToArray();
                assert.True(
                    missingCurrent.Length == 0 && remainingLegacy.Length == 0,
                    "all nine current files exist; all nine legacy files are absent",
                    $"missing={string.Join(',', missingCurrent)}; legacy={string.Join(',', remainingLegacy)}");
            });

        yield return new(
            "C1-003",
            "PostG6Naming",
            "C1.ActiveCompilerSources_ContainNoOrdinaryLegacyTokens",
            assert =>
            {
                var contract = PostG6NamingContractFixture.Contract;
                var forbidden = contract.OrdinarySymbolMappings.Select(mapping => mapping.Legacy)
                    .Concat(contract.MsbuildMappings.Select(mapping => mapping.Legacy))
                    .Concat(contract.GodotNodeMappings.Select(mapping => mapping.Legacy))
                    .ToArray();
                var hits = PostG6NamingContractFixture.ActiveCompilerFiles()
                    .SelectMany(path => forbidden
                        .Where(token => File.ReadAllText(path).Contains(token, StringComparison.Ordinal))
                        .Select(token => $"{Path.GetRelativePath(IdentityContractFixture.RepositoryRoot, path)}:{token}"))
                    .ToArray();
                assert.True(
                    hits.Length == 0,
                    "ordinary legacy symbols, MSBuild switches, defines, and Godot node names are absent",
                    string.Join(',', hits));
            });

        yield return new(
            "C1-004",
            "PostG6Naming",
            "C1.CompiledAssembly_ContainsNoOrdinaryPartyWatchTypesOrMembers",
            assert =>
            {
                var forbidden = PostG6NamingContractFixture.Contract.OrdinarySymbolMappings
                    .Select(mapping => mapping.Legacy)
                    .ToHashSet(StringComparer.Ordinal);
                var types = typeof(MainFile).Assembly.GetTypes();
                var hits = types
                    .Select(type => type.Name)
                    .Concat(types.SelectMany(type => type.GetMembers(
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                        .Select(member => member.Name)))
                    .Where(forbidden.Contains)
                    .Distinct(StringComparer.Ordinal)
                    .ToArray();
                assert.True(hits.Length == 0, "no ordinary legacy type/member names", string.Join(',', hits));
            });

        yield return new(
            "C1-005",
            "PostG6Naming",
            "C1.PersistenceIdentity_HistoricalFreezeIsSupersededOnlyByApprovedC2",
            assert =>
            {
                var frozen = PostG6NamingContractFixture.Contract.FrozenPersistence;
                var c2 = PostG6PersistenceContractFixture.Contract;
                var configType = typeof(DamageForecastBaseLibConfig);
                var propertyOrder = (string[]?)configType.GetField(
                    "PropertyOrder",
                    BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null) ?? [];
                assert.True(
                    c2.Approval.Gate == "C2"
                    && configType.FullName == c2.Current.Type
                    && $"{configType.Namespace?.Split('.')[0]}.cfg" == c2.Current.File
                    && configType.GetProperty(c2.Current.HudKey) is not null
                    && configType.GetProperty(frozen.PersistedMember) is null
                    && propertyOrder.Length == frozen.PropertyCount
                    && File.Exists(PostG6NamingContractFixture.Resolve("src/DamageForecast/Settings/DamageForecastBaseLibConfig.cs")),
                    "C1 historical freeze is retained while approved C2 owns the current 18-property identity",
                    $"type={configType.FullName}; file={configType.Namespace?.Split('.')[0]}.cfg; member={c2.Current.HudKey}; count={propertyOrder.Length}");
            });

        yield return new(
            "C1-006",
            "PostG6Naming",
            "C1.MSBuildSwitchesAndDefines_UseDamageForecastNames",
            assert =>
            {
                var contract = PostG6NamingContractFixture.Contract;
                var sourceProject = PostG6NamingContractFixture.Read("src/DamageForecast/DamageForecast.csproj");
                var testProject = PostG6NamingContractFixture.Read("tests/DamageForecast.ContractTests/DamageForecast.ContractTests.csproj");
                var combined = sourceProject + testProject;
                var currentMissing = contract.MsbuildMappings
                    .Where(mapping => !combined.Contains(mapping.Current, StringComparison.Ordinal))
                    .Select(mapping => mapping.Current)
                    .ToArray();
                var legacyRemaining = contract.MsbuildMappings
                    .Where(mapping => combined.Contains(mapping.Legacy, StringComparison.Ordinal))
                    .Select(mapping => mapping.Legacy)
                    .ToArray();
                assert.True(
                    currentMissing.Length == 0 && legacyRemaining.Length == 0,
                    "all current MSBuild switches/defines present; no legacy switch/define remains",
                    $"missing={string.Join(',', currentMissing)}; legacy={string.Join(',', legacyRemaining)}");
            });

        yield return new(
            "C1-007",
            "PostG6Naming",
            "C1.GodotNodeNamesAndOwnershipGroups_AreCurrent",
            assert =>
            {
                var expected = PostG6NamingContractFixture.Contract.GodotNodeMappings;
                var source = PostG6NamingContractFixture.Read("src/DamageForecast/Patches/ForecastRefreshPatch.cs");
                var actualNames = new[]
                {
                    ForecastRefreshPatch.MainLabelName,
                    ForecastRefreshPatch.IncomingLabelName,
                    ForecastRefreshPatch.DetailLabelName
                };
                assert.True(
                    actualNames.SequenceEqual(expected.Select(mapping => mapping.Current), StringComparer.Ordinal)
                    && expected.All(mapping => source.Contains(mapping.OwnershipGroup, StringComparison.Ordinal))
                    && expected.All(mapping => !source.Contains(mapping.Legacy, StringComparison.Ordinal)),
                    "three current node names and three ownership groups; no legacy node literal",
                    $"actual={string.Join(',', actualNames)}");
            });

        yield return new(
            "C1-008",
            "PostG6Naming",
            "C1.G6IdentityContract_RemainsHistoricalAndSeparate",
            assert =>
            {
                var postG6 = PostG6NamingContractFixture.Contract;
                var g6 = IdentityContractFixture.Contract;
                assert.True(
                    postG6.ExcludedHistoricalContracts.Contains(
                        "tests/DamageForecast.ContractTests/identity-contract.json",
                        StringComparer.Ordinal)
                    && g6.ApprovedDecisions.Any(decision => decision.Id == "G6-D03")
                    && g6.ActiveIdentity.PersistenceFileName == postG6.FrozenPersistence.File,
                    "G6 contract preserved as a separate historical baseline; C1 freezes persistence",
                    $"G6={g6.Status}; persistence={g6.ActiveIdentity.PersistenceFileName}");
            });
    }
}
