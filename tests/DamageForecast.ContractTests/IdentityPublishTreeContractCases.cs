using System.Diagnostics;
using System.Text.Json;
using DamageForecast;

internal static class IdentityPublishTreeContractCases
{
    public static IEnumerable<ContractCase> Create()
    {
        yield return new("PT-001", "IdentityPublishTree", "PublishValidator.IdenticalTwoFileTrees_PassWithFullIdentity", assert =>
        {
            using var fixture = PublishTreeFixture.Create();
            var result = fixture.Run();
            using var json = JsonDocument.Parse(result.Output);
            var root = json.RootElement;
            assert.True(result.ExitCode == 0
                && root.GetProperty("artifactsIdentical").GetBoolean()
                && root.GetProperty("differentFiles").GetArrayLength() == 0
                && root.GetProperty("stable").GetProperty("fileCount").GetInt32() == 2
                && root.GetProperty("beta").GetProperty("fileCount").GetInt32() == 2
                && root.GetProperty("stable").GetProperty("assemblyName").GetString() == "damage-forecast",
                "stable/beta trees contain exactly manifest+DLL; identity and SHA256 values match",
                $"exit={result.ExitCode}; stdout={result.Output}; stderr={result.Error}");
        });
        yield return new("PT-002", "IdentityPublishTree", "PublishValidator.ArbitraryExtraFile_IsRejected", assert =>
        {
            using var fixture = PublishTreeFixture.Create(stableExtra: true);
            var result = fixture.Run();
            assert.True(result.ExitCode != 0
                && result.Error.Contains("must contain exactly damage-forecast.dll, damage-forecast.json", StringComparison.Ordinal),
                "any third file is rejected, regardless of extension",
                $"exit={result.ExitCode}; stderr={result.Error}");
        });
        yield return new("PT-003", "IdentityPublishTree", "PublishValidator.LegacyManifestIdentity_IsRejected", assert =>
        {
            using var fixture = PublishTreeFixture.Create(legacyStableManifest: true);
            var result = fixture.Run();
            assert.True(result.ExitCode != 0
                && result.Error.Contains("stable manifest id mismatch", StringComparison.Ordinal),
                "legacy Mod ID cannot enter an active publish tree",
                $"exit={result.ExitCode}; stderr={result.Error}");
        });
        yield return new("PT-004", "IdentityPublishTree", "PublishValidator.HashDifference_RequiresExplicitApproval", assert =>
        {
            using var fixture = PublishTreeFixture.Create(betaManifestDifference: true);
            var result = fixture.Run();
            assert.True(result.ExitCode != 0
                && result.Output.Contains("\"artifactsIdentical\":false", StringComparison.Ordinal)
                && result.Error.Contains("explicit -ApproveHashDifference is required", StringComparison.Ordinal),
                "stable/beta hash difference is reported and rejected by default",
                $"exit={result.ExitCode}; stdout={result.Output}; stderr={result.Error}");
        });
        yield return new("PT-005", "IdentityPublishTree", "PublishValidator.ContractAndScriptRemainReadOnlyAndSeparatelyApproved", assert =>
        {
            var contract = IdentityContractFixture.Contract.PublishValidationContract;
            var script = IdentityContractFixture.Read(contract.ValidatorScriptPath);
            var build = IdentityContractFixture.Read(IdentityContractFixture.Contract.Packaging.BuildScriptPath);
            var forbiddenMutationTokens = new[] { "Remove-Item", "Move-Item", "Copy-Item", "Set-Content", "Out-File", "New-Item" };
            var mutations = forbiddenMutationTokens.Where(token => script.Contains(token, StringComparison.OrdinalIgnoreCase)).ToArray();
            assert.True(contract.Targets.SequenceEqual(["stable", "beta"], StringComparer.Ordinal)
                && contract.ExactFileCount == 2 && contract.HashAlgorithm == "SHA256"
                && contract.HashDifferencePolicy == "reject-unless-explicitly-approved"
                && contract.ActualPublishRequiresSeparateApproval && mutations.Length == 0
                && build.Contains("Test-IdentityPublishTrees.ps1", StringComparison.Ordinal)
                && build.Contains("must contain exactly", StringComparison.Ordinal)
                && build.Contains("$validatorArguments = @{", StringComparison.Ordinal)
                && build.Contains("& $publishValidator @validatorArguments", StringComparison.Ordinal)
                && build.Contains("$validatorArguments.ApproveHashDifference = $true", StringComparison.Ordinal),
                "validator is read-only; dual publish invokes exact-tree/hash validation; approval gates remain",
                "mutationTokens=" + string.Join(",", mutations));
        });
    }

    private sealed class PublishTreeFixture : IDisposable
    {
        private PublishTreeFixture(string root, string stableTree, string betaTree)
        {
            Root = root;
            StableTree = stableTree;
            BetaTree = betaTree;
        }

        public string Root { get; }
        public string StableTree { get; }
        public string BetaTree { get; }

        public static PublishTreeFixture Create(bool stableExtra = false, bool legacyStableManifest = false,
            bool betaManifestDifference = false)
        {
            var root = Path.Combine(IdentityContractFixture.RepositoryRoot, "work", "identity-publish-tree-contracts", Guid.NewGuid().ToString("N"));
            var stable = Path.Combine(root, "stable", "damage-forecast");
            var beta = Path.Combine(root, "beta", "damage-forecast");
            Directory.CreateDirectory(stable);
            Directory.CreateDirectory(beta);
            var sourceManifest = IdentityContractFixture.Read(IdentityContractFixture.Contract.Packaging.ManifestPath);
            var stableManifest = legacyStableManifest
                ? sourceManifest.Replace("\"id\": \"damage-forecast\"", "\"id\": \"sts2-party-watch-v2\"", StringComparison.Ordinal)
                : sourceManifest;
            var betaManifest = betaManifestDifference
                ? sourceManifest.Replace("Read-only local-player combat damage forecast HUD.", "Read-only local-player damage forecast HUD.", StringComparison.Ordinal)
                : sourceManifest;
            File.WriteAllText(Path.Combine(stable, "damage-forecast.json"), stableManifest);
            File.WriteAllText(Path.Combine(beta, "damage-forecast.json"), betaManifest);
            var assemblyPath = typeof(MainFile).Assembly.Location;
            File.Copy(assemblyPath, Path.Combine(stable, "damage-forecast.dll"));
            File.Copy(assemblyPath, Path.Combine(beta, "damage-forecast.dll"));
            if (stableExtra) File.WriteAllText(Path.Combine(stable, "notes.txt"), "forbidden third file");
            return new(root, stable, beta);
        }

        public ProcessResult Run()
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
                Path.Combine(IdentityContractFixture.RepositoryRoot, "scripts", "Test-IdentityPublishTrees.ps1"),
                "-StableTree", StableTree, "-BetaTree", BetaTree
            }) start.ArgumentList.Add(argument);
            using var process = Process.Start(start) ?? throw new InvalidOperationException("Unable to start publish-tree validator.");
            var output = process.StandardOutput.ReadToEnd().Trim();
            var error = process.StandardError.ReadToEnd().Trim();
            process.WaitForExit();
            return new(process.ExitCode, output, error);
        }

        public void Dispose()
        {
            if (Directory.Exists(Root)) Directory.Delete(Root, recursive: true);
        }
    }

    private sealed record ProcessResult(int ExitCode, string Output, string Error);
}
