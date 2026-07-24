using System.Reflection;
using System.Text.Json;
using DamageForecast;
using DamageForecast.Settings;

internal static class IdentityPackagingContractCases
{
    public static IEnumerable<ContractCase> Create()
    {
        yield return new(
            "IP-001",
            "IdentityPackaging",
            "Identity.ContractSchema_ActiveMigrationIsAtomic",
            assert =>
            {
                var contract = IdentityContractFixture.Contract;
                assert.True(
                    contract.SchemaVersion == 2
                    && contract.Status == "active-migrated"
                    && contract.ActiveIdentity == contract.ApprovedTargetIdentity,
                    "schema=2; status=active-migrated; active identity equals approved target",
                    $"schema={contract.SchemaVersion}; status={contract.Status}");
            });
        yield return new(
            "IP-002",
            "IdentityPackaging",
            "Identity.PermanentProductNames_RemainDamageForecast",
            assert =>
            {
                var behavior = IdentityContractFixture.Contract.PermanentBehavior;
                assert.True(
                    behavior.EnglishProductName == DamageForecastConfigText.EnglishProductName
                    && behavior.SimplifiedChineseProductName == DamageForecastConfigText.SimplifiedChineseProductName,
                    "English=Damage Forecast; Chinese=伤害预测",
                    $"English={DamageForecastConfigText.EnglishProductName}; Chinese={DamageForecastConfigText.SimplifiedChineseProductName}");
            });
        yield return new(
            "IP-003",
            "IdentityPackaging",
            "Identity.MultiplayerBoundary_RemainsLocalPlayerOnly",
            assert =>
            {
                var source = IdentityContractFixture.Read("src/DamageForecast/UI/DamageForecastHudVisibilityPolicy.cs");
                var boundary = IdentityContractFixture.Contract.PermanentBehavior.MultiplayerBoundary;
                assert.True(
                    boundary == "local-player-only"
                    && source.Contains("ShowLocalHudInMultiplayer", StringComparison.Ordinal)
                    && source.Contains("creature.Player.NetId != localPlayer.NetId", StringComparison.Ordinal),
                    "contract=local-player-only; setting gate and local NetId gate present",
                    $"contract={boundary}; settingGate={source.Contains("ShowLocalHudInMultiplayer", StringComparison.Ordinal)}; localGate={source.Contains("creature.Player.NetId != localPlayer.NetId", StringComparison.Ordinal)}");
            });
        yield return new(
            "IP-004",
            "IdentityPackaging",
            "Identity.ManifestJson_ValidAndIdentityAligned",
            assert =>
            {
                using var manifest = IdentityContractFixture.ReadManifest();
                var root = manifest.RootElement;
                var identity = IdentityContractFixture.Contract.ActiveIdentity;
                var manifestPath = IdentityContractFixture.Contract.Packaging.ManifestPath;
                assert.True(
                    root.GetProperty("id").GetString() == identity.ModId
                    && Path.GetFileNameWithoutExtension(manifestPath) == identity.ManifestStem,
                    $"id={identity.ModId}; stem={identity.ManifestStem}",
                    $"id={root.GetProperty("id").GetString()}; stem={Path.GetFileNameWithoutExtension(manifestPath)}");
            });
        yield return new(
            "IP-005",
            "IdentityPackaging",
            "Identity.ManifestCapabilitiesAndDependency_RemainExpected",
            assert =>
            {
                using var manifest = IdentityContractFixture.ReadManifest();
                var root = manifest.RootElement;
                var packaging = IdentityContractFixture.Contract.Packaging;
                var dependency = root.GetProperty("dependencies").EnumerateArray().Single();
                assert.True(
                    root.GetProperty("has_dll").GetBoolean() == packaging.HasDll
                    && root.GetProperty("has_pck").GetBoolean() == packaging.HasPck
                    && root.GetProperty("min_game_version").GetString() == packaging.MinGameVersion
                    && dependency.GetProperty("id").GetString() == packaging.DependencyId
                    && dependency.GetProperty("min_version").GetString() == packaging.DependencyMinVersion,
                    "has_dll=true; has_pck=false; BaseLib=3.3.4; min_game=0.107.1",
                    root.ToString());
            });
        yield return new(
            "IP-006",
            "IdentityPackaging",
            "Identity.ProjectAssemblyNamespaceAndManifest_AgreeWithContract",
            assert =>
            {
                var project = IdentityContractFixture.ReadProject();
                var identity = IdentityContractFixture.Contract.ActiveIdentity;
                var manifestFile = Path.GetFileName(IdentityContractFixture.Contract.Packaging.ManifestPath);
                var assembly = project.Descendants("AssemblyName").Single().Value;
                var rootNamespace = project.Descendants("RootNamespace").Single().Value;
                var includedManifest = project.Descendants("None").Single(element =>
                    string.Equals((string?)element.Attribute("Include"), manifestFile, StringComparison.Ordinal)).Attribute("Include")?.Value;
                assert.True(
                    assembly == identity.AssemblyName
                    && rootNamespace == identity.RootNamespace
                    && includedManifest == manifestFile,
                    $"assembly={identity.AssemblyName}; namespace={identity.RootNamespace}; manifest={manifestFile}",
                    $"assembly={assembly}; namespace={rootNamespace}; manifest={includedManifest}");
            });
        yield return new(
            "IP-007",
            "IdentityPackaging",
            "Identity.BaseLibHarmonyAndDiagnostics_AreTraceable",
            assert =>
            {
                var identity = IdentityContractFixture.Contract.ActiveIdentity;
                var source = IdentityContractFixture.Read("src/DamageForecast/MainFile.cs");
                assert.True(
                    MainFile.ModId == identity.ModId
                    && identity.BaseLibRegistrationKey == identity.ModId
                    && identity.HarmonyOwner == identity.ModId
                    && identity.PersistenceFileName == "STS2PartyWatch.cfg"
                    && identity.PersistenceFileName != identity.BaseLibRegistrationKey
                    && identity.PersistenceOwnerRootNamespace == "STS2PartyWatch"
                    && source.Contains("ModConfigRegistry.Register(ModId, config)", StringComparison.Ordinal)
                    && source.Contains("new Harmony(ModId)", StringComparison.Ordinal)
                    && source.Contains(identity.DiagnosticPrefix, StringComparison.Ordinal),
                    "ModId=BaseLib registration=Harmony; persistence remains separate STS2PartyWatch.cfg; diagnostic prefix present",
                    $"ModId={MainFile.ModId}; BaseLib={identity.BaseLibRegistrationKey}; Harmony={identity.HarmonyOwner}; persistence={identity.PersistenceFileName}");
            });
        yield return new(
            "IP-008",
            "IdentityPackaging",
            "Identity.BuildAndInstallScripts_UseOneProjectAndActiveIdentity",
            assert =>
            {
                var contract = IdentityContractFixture.Contract;
                var build = IdentityContractFixture.Read(contract.Packaging.BuildScriptPath);
                var install = IdentityContractFixture.Read(contract.Packaging.InstallScriptPath);
                var identity = contract.ActiveIdentity;
                assert.True(
                    build.Contains(identity.ProjectPath, StringComparison.Ordinal)
                    && install.Contains("work/publish/damage-forecast", StringComparison.Ordinal)
                    && build.Contains($"stable/{identity.InstallDirectoryName}", StringComparison.Ordinal)
                    && build.Contains($"beta/{identity.InstallDirectoryName}", StringComparison.Ordinal)
                    && install.Contains($"$modId = \"{identity.ModId}\"", StringComparison.Ordinal),
                    "build project and staged installer active identity aligned",
                    "one or more active build/install identity references differ");
            });
        yield return new(
            "IP-009",
            "IdentityPackaging",
            "Identity.PublishWhitelist_IsDllPlusManifestOnly",
            assert =>
            {
                var contract = IdentityContractFixture.Contract;
                var expected = new[] { contract.ActiveIdentity.DllName, $"{contract.ActiveIdentity.ManifestStem}.json" };
                var actual = contract.Packaging.PublishWhitelist.Order(StringComparer.Ordinal).ToArray();
                assert.True(
                    actual.SequenceEqual(expected.Order(StringComparer.Ordinal), StringComparer.Ordinal),
                    string.Join(",", expected),
                    string.Join(",", actual));
            });
        yield return new(
            "IP-010",
            "IdentityPackaging",
            "Identity.PublishAndInstallScripts_EnforceWhitelistAndForbiddenOutputs",
            assert =>
            {
                var packaging = IdentityContractFixture.Contract.Packaging;
                var combined = IdentityContractFixture.Read(packaging.BuildScriptPath)
                    + IdentityContractFixture.Read(packaging.InstallScriptPath);
                var allRequired = packaging.PublishWhitelist.All(file => combined.Contains(file, StringComparison.Ordinal));
                assert.True(
                    allRequired
                    && combined.Contains(".deps.json", StringComparison.Ordinal)
                    && combined.Contains(".pdb", StringComparison.Ordinal)
                    && combined.Contains(".pck", StringComparison.Ordinal)
                    && combined.Contains(".log", StringComparison.Ordinal),
                    "required DLL/manifest and all forbidden output guards present",
                    $"required={allRequired}");
            });
        yield return new(
            "IP-011",
            "IdentityPackaging",
            "Identity.PersistenceContract_CoversAllOrderedSettings",
            assert =>
            {
                var field = typeof(DamageForecastBaseLibConfig).GetField(
                    "PropertyOrder",
                    BindingFlags.NonPublic | BindingFlags.Static);
                var actual = (string[]?)field?.GetValue(null) ?? [];
                var expected = IdentityContractFixture.Contract.PersistentSettings
                    .Select(name => name == "EnablePartyWatchHud" ? "EnableDamageForecastHud" : name)
                    .ToArray();
                assert.True(
                    actual.Length == 18 && actual.SequenceEqual(expected, StringComparer.Ordinal),
                    string.Join(",", expected),
                    string.Join(",", actual),
                    "settings upgrade continuity is tracked property by property");
            });
        yield return new(
            "IP-012",
            "IdentityPackaging",
            "Identity.ActiveSourceContainsOneManifestIdentity",
            assert =>
            {
                var sourceDirectory = Path.Combine(IdentityContractFixture.RepositoryRoot, "src", "DamageForecast");
                var ids = Directory.EnumerateFiles(sourceDirectory, "*.json", SearchOption.TopDirectoryOnly)
                    .Select(path => JsonDocument.Parse(File.ReadAllText(path)))
                    .Select(document =>
                    {
                        using (document)
                        {
                            return document.RootElement.TryGetProperty("id", out var id) ? id.GetString() : null;
                        }
                    })
                    .Where(id => id is not null)
                    .ToArray();
                assert.True(
                    ids.Length == 1 && ids[0] == IdentityContractFixture.Contract.ActiveIdentity.ModId,
                    $"one active manifest id={IdentityContractFixture.Contract.ActiveIdentity.ModId}",
                    string.Join(",", ids));
            });
        yield return new(
            "IP-013",
            "IdentityPackaging",
            "Identity.RepositoryTracksNoForbiddenBuildOrGameArtifacts",
            assert =>
            {
                var forbidden = IdentityContractFixture.GetTrackedFiles()
                    .Where(IsForbiddenTrackedArtifact)
                    .ToArray();
                assert.True(
                    forbidden.Length == 0,
                    "no tracked DLL/PDB/PCK/log/bin/obj/publish/work/uploader/game artifacts",
                    string.Join(",", forbidden));
            });
        yield return new(
            "IP-014",
            "IdentityPackaging",
            "Identity.PostG6ActiveScanScope_ExcludesHistoricalEvidence",
            assert =>
            {
                var contract = IdentityContractFixture.Contract;
                var postG6 = PostG6NamingContractFixture.Contract;
                var excluded = contract.L0ScanScope.Any(path =>
                    contract.ExcludedScanPrefixes.Any(prefix => path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)));
                var allCurrentFilesExist = postG6.FileMappings.All(mapping =>
                    File.Exists(PostG6NamingContractFixture.Resolve(mapping.Current)));
                assert.True(
                    !excluded
                    && allCurrentFilesExist
                    && contract.PermanentBehavior.HistoricalEvidenceDisposition == "excluded-from-l0-scan",
                    "historical docs excluded; all Post-G6 current ordinary files exist",
                    $"excludedHit={excluded}; allCurrentFilesExist={allCurrentFilesExist}; disposition={contract.PermanentBehavior.HistoricalEvidenceDisposition}");
            });
        yield return new(
            "IP-015",
            "IdentityPackaging",
            "Identity.MigrationContractStructure_IsCompleteAndApproved",
            assert =>
            {
                var errors = IdentityContractFixture.ValidateMigrationStructure();
                assert.True(
                    errors.Count == 0,
                    "all identity fields, approved targets, decisions, and migration evidence columns complete",
                    string.Join(" | ", errors));
            });
        yield return new(
            "IP-016",
            "IdentityPackaging",
            "Identity.WorkshopDisposition_RemainsExternalAndUnchanged",
            assert => assert.Equal(
                "external-unchanged",
                IdentityContractFixture.Contract.PermanentBehavior.WorkshopDisposition));
        yield return new(
            "IP-017",
            "IdentityPackaging",
            "Identity.LegacyConflictDetection_RefusesDuplicateWithoutDeletion",
            assert =>
            {
                var install = IdentityContractFixture.Read(IdentityContractFixture.Contract.Packaging.InstallScriptPath);
                var startup = IdentityContractFixture.Read("src/DamageForecast/Identity/LegacyIdentityConflictDetector.cs");
                assert.True(
                    install.Contains("$legacyModId = \"sts2-party-watch-v2\"", StringComparison.Ordinal)
                    && install.Contains("Duplicate identity state", StringComparison.Ordinal)
                    && !install.Contains("Remove-Item", StringComparison.OrdinalIgnoreCase)
                    && startup.Contains("AppDomain.CurrentDomain.GetAssemblies()", StringComparison.Ordinal),
                    "installer refuses legacy directory without deletion; startup scans loaded assemblies",
                    "legacy conflict preflight is incomplete");
            });
    }

    private static bool IsForbiddenTrackedArtifact(string path)
    {
        var normalized = path.Replace('\\', '/');
        var extension = Path.GetExtension(normalized);
        if (extension.Equals(".dll", StringComparison.OrdinalIgnoreCase)
            || extension.Equals(".pdb", StringComparison.OrdinalIgnoreCase)
            || extension.Equals(".pck", StringComparison.OrdinalIgnoreCase)
            || extension.Equals(".log", StringComparison.OrdinalIgnoreCase)
            || extension.Equals(".exe", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var segments = normalized.Split('/');
        return segments.Any(segment => segment.Equals("bin", StringComparison.OrdinalIgnoreCase)
            || segment.Equals("obj", StringComparison.OrdinalIgnoreCase)
            || segment.Equals("publish", StringComparison.OrdinalIgnoreCase)
            || segment.Equals("work", StringComparison.OrdinalIgnoreCase)
            || segment.Equals("uploader", StringComparison.OrdinalIgnoreCase));
    }
}
