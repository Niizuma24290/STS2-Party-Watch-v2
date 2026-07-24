using System.Diagnostics;
using System.Text.Json;

internal static class IdentityUpgradeToolContractCases
{
    public static IEnumerable<ContractCase> Create()
    {
        yield return new("IU-001", "IdentityUpgradeTool", "UpgradePlan.CleanInstall_IsReadOnlyAndTargeted", assert =>
        {
            using var fixture = UpgradeFixture.Create();
            var result = fixture.Run("Plan");
            using var json = JsonDocument.Parse(result.Output);
            var root = json.RootElement;
            assert.True(result.ExitCode == 0 && root.GetProperty("action").GetString() == "clean-install"
                && root.GetProperty("legacyActiveCount").GetInt32() == 0
                && root.GetProperty("targetActiveCount").GetInt32() == 0
                && !Directory.Exists(fixture.TargetInstallPath),
                "clean-install plan; legacy=0; target=0; no mutation",
                $"exit={result.ExitCode}; stdout={result.Output}; stderr={result.Error}");
        });
        yield return new("IU-002", "IdentityUpgradeTool", "UpgradePlan.LegacyManifest_ProducesRecoverableUpgradePlan", assert =>
        {
            using var fixture = UpgradeFixture.Create(legacyCopies: 1);
            var result = fixture.Run("Plan");
            using var json = JsonDocument.Parse(result.Output);
            var root = json.RootElement;
            var backup = root.GetProperty("plannedLegacyBackupPath").GetString() ?? "";
            assert.True(result.ExitCode == 0 && root.GetProperty("action").GetString() == "upgrade"
                && root.GetProperty("legacyActiveCount").GetInt32() == 1
                && backup.StartsWith(fixture.BackupRoot, StringComparison.OrdinalIgnoreCase)
                && !backup.StartsWith(fixture.ModsRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
                && root.GetProperty("backupOutsideLoaderRoot").GetBoolean()
                && Directory.Exists(fixture.LegacyInstallPaths[0]),
                "upgrade plan retains legacy directory and resolves backup outside the Loader-scanned mods root",
                $"exit={result.ExitCode}; backup={backup}; stderr={result.Error}");
        });
        yield return new("IU-003", "IdentityUpgradeTool", "UpgradePlan.DuplicateLegacyIdentities_AreRejected", assert =>
        {
            using var fixture = UpgradeFixture.Create(legacyCopies: 2);
            var result = fixture.Run("Plan");
            assert.True(result.ExitCode != 0 && result.Error.Contains("Duplicate identity state", StringComparison.Ordinal)
                && fixture.LegacyInstallPaths.All(Directory.Exists),
                "duplicate identities rejected without mutation",
                $"exit={result.ExitCode}; stderr={result.Error}");
        });
        yield return new("IU-004", "IdentityUpgradeTool", "UpgradePlan.StagingExtras_AreRejected", assert =>
        {
            using var fixture = UpgradeFixture.Create(stagingExtra: true);
            var result = fixture.Run("Plan");
            assert.True(result.ExitCode != 0 && result.Error.Contains("Staging tree must contain exactly", StringComparison.Ordinal)
                && !Directory.Exists(fixture.TargetInstallPath),
                "extra staging file rejected without install",
                $"exit={result.ExitCode}; stderr={result.Error}");
        });
        yield return new("IU-005", "IdentityUpgradeTool", "UpgradeExecution.InstallWithoutExecute_IsRejected", assert =>
        {
            using var fixture = UpgradeFixture.Create();
            var result = fixture.Run("Install");
            assert.True(result.ExitCode != 0 && result.Error.Contains("requires explicit -Execute", StringComparison.Ordinal)
                && !Directory.Exists(fixture.TargetInstallPath),
                "Install requires -Execute and leaves mods root unchanged",
                $"exit={result.ExitCode}; stderr={result.Error}");
        });
        yield return new("IU-006", "IdentityUpgradeTool", "UpgradeTool.StaticGuards_CoverProcessPathsWorkshopBackupAndRestore", assert =>
        {
            var script = IdentityContractFixture.Read("scripts/Install-LocalMod.ps1");
            var contract = IdentityContractFixture.Contract.InstallToolContract;
            var required = new[]
            {
                "Get-Process -Name \"SlayTheSpire2\"", "Read-ManifestIdentity", "Get-StagingIdentity",
                "Assert-PathWithinRoot", "Assert-PathOutsideRoot", "plannedLegacyBackupPath", "plannedActiveBackupPath",
                "ExpectedActiveManifestSha256", "ExpectedBackupManifestSha256", "PlanOperation", "activate-target-by-rename",
                "keep-backups-outside-loader-scan", "Get-IdentityRecords -Root $modsRoot -Source \"local\" -Recursive",
                "restore-previous-active-on-failure", "Post-install identity verification failed",
                "Post-rollback identity verification failed", "-IncludeWorkshop requires an explicit -WorkshopRoot",
                "without a separately approved Workshop disposition"
            };
            var missing = required.Where(token => !script.Contains(token, StringComparison.Ordinal)).ToArray();
            assert.True(missing.Length == 0 && !script.Contains("Remove-Item", StringComparison.OrdinalIgnoreCase)
                && contract.DefaultMode == "Plan" && contract.RealExecutionRequiresSeparateApproval
                && contract.PlanBinding == "transaction-and-staging-sha256"
                && contract.BackupRootPolicy == "inside-game-root-outside-loader-mods-root"
                && contract.LoaderScanModel == "recursive-json-under-mods-root"
                && contract.UnsafeBackupDisposition == "reject"
                && contract.WorkshopDefault == "excluded" && contract.BackupRetention == "through-runtime-acceptance",
                "all process/path/staging/backup/restore/Workshop guards present; no deletion",
                "missing=" + string.Join(",", missing));
        });
        yield return new("IU-007", "IdentityUpgradeTool", "UpgradeExecution.TempUpgradeAndRollback_RestoreSingleIdentity", assert =>
        {
            using var fixture = UpgradeFixture.Create(legacyCopies: 1);
            var planResult = fixture.Run("Plan");
            using var planJson = JsonDocument.Parse(planResult.Output);
            var backupPath = planJson.RootElement.GetProperty("plannedLegacyBackupPath").GetString()
                ?? throw new InvalidOperationException("Upgrade plan did not provide a backup path.");
            var transactionId = planJson.RootElement.GetProperty("transactionId").GetString()
                ?? throw new InvalidOperationException("Upgrade plan did not provide a transaction id.");
            var manifestSha256 = planJson.RootElement.GetProperty("stagingManifestSha256").GetString()
                ?? throw new InvalidOperationException("Upgrade plan did not provide the manifest hash.");
            var dllSha256 = planJson.RootElement.GetProperty("stagingDllSha256").GetString()
                ?? throw new InvalidOperationException("Upgrade plan did not provide the DLL hash.");
            var activeManifestSha256 = planJson.RootElement.GetProperty("activeManifestSha256").GetString()
                ?? throw new InvalidOperationException("Upgrade plan did not provide the active manifest hash.");
            var activeDllSha256 = planJson.RootElement.GetProperty("activeDllSha256").GetString()
                ?? throw new InvalidOperationException("Upgrade plan did not provide the active DLL hash.");
            var install = fixture.Run("Install", execute: true, transactionId: transactionId,
                expectedManifestSha256: manifestSha256, expectedDllSha256: dllSha256,
                expectedActiveManifestSha256: activeManifestSha256, expectedActiveDllSha256: activeDllSha256);
            var installed = install.ExitCode == 0 && Directory.Exists(fixture.TargetInstallPath)
                && !Directory.Exists(fixture.LegacyInstallPaths[0]) && Directory.Exists(backupPath)
                && !backupPath.StartsWith(fixture.ModsRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
                && fixture.LoaderVisibleJsonFiles().SequenceEqual(["damage-forecast/damage-forecast.json"], StringComparer.Ordinal);
            var rollbackPlan = fixture.Run("Plan", planOperation: "Rollback", backupPath: backupPath);
            using var rollbackJson = JsonDocument.Parse(rollbackPlan.Output);
            var rollbackRoot = rollbackJson.RootElement;
            var rollback = fixture.Run("Rollback", execute: true, backupPath: backupPath,
                transactionId: rollbackRoot.GetProperty("transactionId").GetString(),
                expectedActiveManifestSha256: rollbackRoot.GetProperty("activeManifestSha256").GetString(),
                expectedActiveDllSha256: rollbackRoot.GetProperty("activeDllSha256").GetString(),
                expectedBackupManifestSha256: rollbackRoot.GetProperty("rollbackBackupManifestSha256").GetString(),
                expectedBackupDllSha256: rollbackRoot.GetProperty("rollbackBackupDllSha256").GetString());
            var restored = rollback.ExitCode == 0 && !Directory.Exists(fixture.TargetInstallPath)
                && Directory.Exists(fixture.LegacyInstallPaths[0])
                && File.Exists(Path.Combine(fixture.LegacyInstallPaths[0], "sts2-party-watch-v2.json"))
                && File.Exists(Path.Combine(fixture.LegacyInstallPaths[0], "sts2-party-watch-v2.dll"))
                && fixture.LoaderVisibleJsonFiles().SequenceEqual(["sts2-party-watch-v2/sts2-party-watch-v2.json"], StringComparer.Ordinal);
            assert.True(installed && restored,
                "temporary upgrade activates one target; rollback restores one legacy identity",
                $"installExit={install.ExitCode}; installError={install.Error}; rollbackExit={rollback.ExitCode}; rollbackError={rollback.Error}");
        });
        yield return new("IU-008", "IdentityUpgradeTool", "UpgradePlan.BackupInsideLoaderRoot_IsRejected", assert =>
        {
            using var fixture = UpgradeFixture.Create(legacyCopies: 1);
            var unsafeRoot = Path.Combine(fixture.ModsRoot, ".damage-forecast-backups");
            var result = fixture.Run("Plan", backupRootOverride: unsafeRoot);
            assert.True(result.ExitCode != 0
                && result.Error.Contains("backup root must remain outside Loader-scanned root", StringComparison.Ordinal)
                && Directory.Exists(fixture.LegacyInstallPaths[0]) && !Directory.Exists(unsafeRoot),
                "backup roots inside recursively scanned mods are rejected without mutation",
                $"exit={result.ExitCode}; stderr={result.Error}");
        });
        yield return new("IU-009", "IdentityUpgradeTool", "TargetUpgrade.TempUpgradeRollbackAndReupgrade_PreserveSingleIdentity", assert =>
        {
            using var fixture = UpgradeFixture.Create(targetActive: true);
            var plan = fixture.Run("Plan", transactionId: "same-id-upgrade");
            using var planJson = JsonDocument.Parse(plan.Output);
            var root = planJson.RootElement;
            var backup = root.GetProperty("plannedActiveBackupPath").GetString()
                ?? throw new InvalidOperationException("Target upgrade Plan has no active backup path.");
            var install = fixture.Run("Install", execute: true, transactionId: root.GetProperty("transactionId").GetString(),
                expectedManifestSha256: root.GetProperty("stagingManifestSha256").GetString(),
                expectedDllSha256: root.GetProperty("stagingDllSha256").GetString(),
                expectedActiveManifestSha256: root.GetProperty("activeManifestSha256").GetString(),
                expectedActiveDllSha256: root.GetProperty("activeDllSha256").GetString());
            var upgraded = install.ExitCode == 0 && UpgradeFixture.ReadVersion(fixture.TargetInstallPath) == "v0.3.0"
                && UpgradeFixture.ReadVersion(backup) == "v0.2.0" && fixture.LoaderVisibleJsonFiles()
                    .SequenceEqual(["damage-forecast/damage-forecast.json"], StringComparer.Ordinal);

            var rollbackPlan = fixture.Run("Plan", planOperation: "Rollback", backupPath: backup,
                transactionId: "same-id-rollback");
            using var rollbackJson = JsonDocument.Parse(rollbackPlan.Output);
            var rollbackRoot = rollbackJson.RootElement;
            var rollback = fixture.Run("Rollback", execute: true, backupPath: backup,
                transactionId: rollbackRoot.GetProperty("transactionId").GetString(),
                expectedActiveManifestSha256: rollbackRoot.GetProperty("activeManifestSha256").GetString(),
                expectedActiveDllSha256: rollbackRoot.GetProperty("activeDllSha256").GetString(),
                expectedBackupManifestSha256: rollbackRoot.GetProperty("rollbackBackupManifestSha256").GetString(),
                expectedBackupDllSha256: rollbackRoot.GetProperty("rollbackBackupDllSha256").GetString());
            var rolledBack = rollback.ExitCode == 0 && UpgradeFixture.ReadVersion(fixture.TargetInstallPath) == "v0.2.0"
                && fixture.LoaderVisibleJsonFiles().SequenceEqual(["damage-forecast/damage-forecast.json"], StringComparer.Ordinal);

            var replan = fixture.Run("Plan", transactionId: "same-id-reupgrade");
            using var replanJson = JsonDocument.Parse(replan.Output);
            var reRoot = replanJson.RootElement;
            var reupgrade = fixture.Run("Install", execute: true, transactionId: reRoot.GetProperty("transactionId").GetString(),
                expectedManifestSha256: reRoot.GetProperty("stagingManifestSha256").GetString(),
                expectedDllSha256: reRoot.GetProperty("stagingDllSha256").GetString(),
                expectedActiveManifestSha256: reRoot.GetProperty("activeManifestSha256").GetString(),
                expectedActiveDllSha256: reRoot.GetProperty("activeDllSha256").GetString());
            var reupgraded = reupgrade.ExitCode == 0 && UpgradeFixture.ReadVersion(fixture.TargetInstallPath) == "v0.3.0"
                && fixture.LoaderVisibleJsonFiles().SequenceEqual(["damage-forecast/damage-forecast.json"], StringComparer.Ordinal);

            assert.True(plan.ExitCode == 0 && root.GetProperty("action").GetString() == "target-upgrade"
                && root.GetProperty("activeVersion").GetString() == "v0.2.0"
                && root.GetProperty("stagingVersion").GetString() == "v0.3.0"
                && rollbackPlan.ExitCode == 0 && rollbackRoot.GetProperty("action").GetString() == "rollback-target"
                && upgraded && rolledBack && reupgraded,
                "same identity v0.2.0 upgrades to v0.3.0, rolls back to v0.2.0, then re-upgrades with one active identity",
                $"plan={plan.Error}; install={install.Error}; rollbackPlan={rollbackPlan.Error}; rollback={rollback.Error}; reupgrade={reupgrade.Error}");
        });
        yield return new("IU-010", "IdentityUpgradeTool", "TargetRollback.CurrentSchemaOnlyConfig_DoesNotRequireLegacyConfig", assert =>
        {
            using var fixture = UpgradeFixture.Create(
                targetActive: true,
                targetVersion: "v0.3.0",
                currentConfigOnly: true);
            var configBefore = File.ReadAllBytes(fixture.CurrentConfigPath);
            var activeDllBefore = File.ReadAllBytes(Path.Combine(fixture.TargetInstallPath, "damage-forecast.dll"));
            var plan = fixture.Run("Plan", transactionId: "current-schema-upgrade");
            using var planJson = JsonDocument.Parse(plan.Output);
            var root = planJson.RootElement;
            var backup = root.GetProperty("plannedActiveBackupPath").GetString()
                ?? throw new InvalidOperationException("Current-schema target upgrade Plan has no active backup path.");
            var install = fixture.Run("Install", execute: true, transactionId: root.GetProperty("transactionId").GetString(),
                expectedManifestSha256: root.GetProperty("stagingManifestSha256").GetString(),
                expectedDllSha256: root.GetProperty("stagingDllSha256").GetString(),
                expectedActiveManifestSha256: root.GetProperty("activeManifestSha256").GetString(),
                expectedActiveDllSha256: root.GetProperty("activeDllSha256").GetString());

            var rollbackPlan = fixture.Run("Plan", planOperation: "Rollback", backupPath: backup,
                transactionId: "current-schema-rollback");
            using var rollbackJson = JsonDocument.Parse(rollbackPlan.Output);
            var rollbackRoot = rollbackJson.RootElement;
            var rollback = fixture.Run("Rollback", execute: true, backupPath: backup,
                transactionId: rollbackRoot.GetProperty("transactionId").GetString(),
                expectedActiveManifestSha256: rollbackRoot.GetProperty("activeManifestSha256").GetString(),
                expectedActiveDllSha256: rollbackRoot.GetProperty("activeDllSha256").GetString(),
                expectedBackupManifestSha256: rollbackRoot.GetProperty("rollbackBackupManifestSha256").GetString(),
                expectedBackupDllSha256: rollbackRoot.GetProperty("rollbackBackupDllSha256").GetString());

            assert.True(plan.ExitCode == 0 && root.GetProperty("action").GetString() == "target-upgrade"
                && root.GetProperty("activeVersion").GetString() == "v0.3.0"
                && root.GetProperty("stagingVersion").GetString() == "v0.3.0"
                && install.ExitCode == 0
                && rollbackPlan.ExitCode == 0 && rollbackRoot.GetProperty("action").GetString() == "rollback-target"
                && rollbackRoot.GetProperty("configRollbackAction").GetString() == "target-direct"
                && rollback.ExitCode == 0
                && File.ReadAllBytes(fixture.CurrentConfigPath).SequenceEqual(configBefore)
                && File.ReadAllBytes(Path.Combine(fixture.TargetInstallPath, "damage-forecast.dll")).SequenceEqual(activeDllBefore)
                && fixture.LoaderVisibleJsonFiles().SequenceEqual(["damage-forecast/damage-forecast.json"], StringComparer.Ordinal),
                "current-schema same-version binary upgrade rolls back directly with one active identity and unchanged current config",
                $"plan={plan.Error}; install={install.Error}; rollbackPlan={rollbackPlan.Error}; rollback={rollback.Error}");
        });
    }

    private sealed class UpgradeFixture : IDisposable
    {
        private UpgradeFixture(string root, string gameRoot, string stagingRoot, string backupRoot, IReadOnlyList<string> legacyInstallPaths)
        {
            Root = root;
            GameRoot = gameRoot;
            StagingRoot = stagingRoot;
            BackupRoot = backupRoot;
            LegacyInstallPaths = legacyInstallPaths;
        }

        public string Root { get; }
        public string GameRoot { get; }
        public string StagingRoot { get; }
        public string BackupRoot { get; }
        public string ModsRoot => Path.Combine(GameRoot, "mods");
        public IReadOnlyList<string> LegacyInstallPaths { get; }
        public string TargetInstallPath => Path.Combine(GameRoot, "mods", "damage-forecast");
        public string CurrentConfigPath => Path.Combine(Root, "appdata", "mod_configs", "DamageForecast.cfg");

        public static UpgradeFixture Create(
            int legacyCopies = 0,
            bool stagingExtra = false,
            bool targetActive = false,
            string targetVersion = "v0.2.0",
            bool currentConfigOnly = false)
        {
            var root = Path.Combine(IdentityContractFixture.RepositoryRoot, "work", "identity-upgrade-contracts", Guid.NewGuid().ToString("N"));
            var gameRoot = Path.Combine(root, "game");
            var modsRoot = Path.Combine(gameRoot, "mods");
            var stagingRoot = Path.Combine(root, "staging");
            var backupRoot = Path.Combine(gameRoot, ".damage-forecast-backups");
            Directory.CreateDirectory(modsRoot);
            Directory.CreateDirectory(stagingRoot);
            File.WriteAllText(Path.Combine(stagingRoot, "damage-forecast.json"), Manifest("damage-forecast", "v0.3.0"));
            File.WriteAllBytes(Path.Combine(stagingRoot, "damage-forecast.dll"), [0x44, 0x46, 0x00, 0x02]);
            if (stagingExtra) File.WriteAllText(Path.Combine(stagingRoot, "unexpected.txt"), "forbidden");

            var legacyPaths = new List<string>();
            for (var index = 0; index < legacyCopies; index++)
            {
                var legacyPath = Path.Combine(modsRoot, index == 0 ? "sts2-party-watch-v2" : $"legacy-copy-{index}");
                Directory.CreateDirectory(legacyPath);
                File.WriteAllText(Path.Combine(legacyPath, "sts2-party-watch-v2.json"), Manifest("sts2-party-watch-v2", "v0.1.0"));
                File.WriteAllBytes(Path.Combine(legacyPath, "sts2-party-watch-v2.dll"), [0x50, 0x57, 0x00, 0x01]);
                legacyPaths.Add(legacyPath);
            }
            if (targetActive)
            {
                var targetPath = Path.Combine(modsRoot, "damage-forecast");
                Directory.CreateDirectory(targetPath);
                File.WriteAllText(Path.Combine(targetPath, "damage-forecast.json"), Manifest("damage-forecast", targetVersion));
                File.WriteAllBytes(Path.Combine(targetPath, "damage-forecast.dll"), [0x44, 0x46, 0x00, 0x01]);
            }
            if (currentConfigOnly)
            {
                Directory.CreateDirectory(Path.Combine(root, "appdata", "mod_configs"));
                File.Copy(
                    Path.Combine(
                        IdentityContractFixture.RepositoryRoot,
                        "tests",
                        "DamageForecast.ContractTests",
                        "fixtures",
                        "config-new-default.cfg"),
                    Path.Combine(root, "appdata", "mod_configs", "DamageForecast.cfg"));
            }
            return new(root, gameRoot, stagingRoot, backupRoot, legacyPaths);
        }

        public ProcessResult Run(string mode, bool execute = false, string? backupPath = null, string? transactionId = null,
            string? expectedManifestSha256 = null, string? expectedDllSha256 = null, string? backupRootOverride = null,
            string? planOperation = null, string? expectedActiveManifestSha256 = null,
            string? expectedActiveDllSha256 = null, string? expectedBackupManifestSha256 = null,
            string? expectedBackupDllSha256 = null)
        {
            var start = new ProcessStartInfo("powershell")
            {
                WorkingDirectory = IdentityContractFixture.RepositoryRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            foreach (var argument in new[]
            {
                "-NoProfile", "-ExecutionPolicy", "Bypass", "-File",
                Path.Combine(IdentityContractFixture.RepositoryRoot, "scripts", "Install-LocalMod.ps1"),
                "-Mode", mode, "-StagingDir", StagingRoot, "-Sts2GameRoot", GameRoot,
                "-BackupRoot", backupRootOverride ?? BackupRoot,
                "-ConfigRoot", Path.Combine(Root, "appdata", "mod_configs"),
                "-ConfigMigrationRoot", Path.Combine(Root, "appdata", "damage-forecast-migration")
            }) start.ArgumentList.Add(argument);
            if (execute) start.ArgumentList.Add("-Execute");
            if (!string.IsNullOrWhiteSpace(planOperation))
            {
                start.ArgumentList.Add("-PlanOperation");
                start.ArgumentList.Add(planOperation);
            }
            if (!string.IsNullOrWhiteSpace(backupPath))
            {
                start.ArgumentList.Add("-BackupPath");
                start.ArgumentList.Add(backupPath);
            }
            if (!string.IsNullOrWhiteSpace(transactionId))
            {
                start.ArgumentList.Add("-TransactionId");
                start.ArgumentList.Add(transactionId);
            }
            if (!string.IsNullOrWhiteSpace(expectedManifestSha256))
            {
                start.ArgumentList.Add("-ExpectedManifestSha256");
                start.ArgumentList.Add(expectedManifestSha256);
            }
            if (!string.IsNullOrWhiteSpace(expectedDllSha256))
            {
                start.ArgumentList.Add("-ExpectedDllSha256");
                start.ArgumentList.Add(expectedDllSha256);
            }
            foreach (var pair in new[]
            {
                (Name: "-ExpectedActiveManifestSha256", Value: expectedActiveManifestSha256),
                (Name: "-ExpectedActiveDllSha256", Value: expectedActiveDllSha256),
                (Name: "-ExpectedBackupManifestSha256", Value: expectedBackupManifestSha256),
                (Name: "-ExpectedBackupDllSha256", Value: expectedBackupDllSha256)
            })
            {
                if (string.IsNullOrWhiteSpace(pair.Value)) continue;
                start.ArgumentList.Add(pair.Name);
                start.ArgumentList.Add(pair.Value);
            }
            using var process = Process.Start(start) ?? throw new InvalidOperationException("Unable to start PowerShell upgrade plan.");
            var output = process.StandardOutput.ReadToEnd().Trim();
            var error = process.StandardError.ReadToEnd().Trim();
            process.WaitForExit();
            return new(process.ExitCode, output, error);
        }

        public void Dispose()
        {
            if (Directory.Exists(Root)) Directory.Delete(Root, recursive: true);
        }

        public IReadOnlyList<string> LoaderVisibleJsonFiles() => Directory
            .EnumerateFiles(ModsRoot, "*.json", SearchOption.AllDirectories)
            .Select(path => Path.GetRelativePath(ModsRoot, path).Replace('\\', '/'))
            .Order(StringComparer.Ordinal)
            .ToArray();

        public static string ReadVersion(string directory)
        {
            using var json = JsonDocument.Parse(File.ReadAllText(Path.Combine(directory, "damage-forecast.json")));
            return json.RootElement.GetProperty("version").GetString() ?? "";
        }

        private static string Manifest(string id, string version) => JsonSerializer.Serialize(new
        {
            id,
            name = "Damage Forecast",
            version,
            has_dll = true,
            has_pck = false
        });
    }

    private sealed record ProcessResult(int ExitCode, string Output, string Error);
}
