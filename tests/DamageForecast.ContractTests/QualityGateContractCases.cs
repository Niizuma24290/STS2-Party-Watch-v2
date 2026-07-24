internal static class QualityGateContractCases
{
    private const string ScriptPath = "scripts/Test-ForecastGuardrails.ps1";

    public static IEnumerable<ContractCase> Create()
    {
        yield return new(
            "QG-001",
            "QualityGate",
            "QualityGate.DefaultRunsStableAndBetaWithSingleTargetOption",
            assert =>
            {
                var script = IdentityContractFixture.Read(ScriptPath);
                assert.True(
                    script.Contains("[ValidateSet(\"all\", \"stable\", \"beta\")]", StringComparison.Ordinal)
                    && script.Contains("[string]$Target = \"all\"", StringComparison.Ordinal)
                    && script.Contains("v0.107.1", StringComparison.Ordinal)
                    && script.Contains("v0.109.0", StringComparison.Ordinal),
                    "default=all; selectable stable/beta; both frozen versions present",
                    "one or more target-selection requirements are missing");
            });
        yield return new(
            "QG-002",
            "QualityGate",
            "QualityGate.VerifiesDependencyContractsAndReleaseBuilds",
            assert =>
            {
                var script = IdentityContractFixture.Read(ScriptPath);
                assert.True(
                    script.Contains("Restore-BaseLibDependency.ps1", StringComparison.Ordinal)
                    && script.Contains("DamageForecast.ContractTests.csproj", StringComparison.Ordinal)
                    && script.Contains("-Step \"contract\"", StringComparison.Ordinal)
                    && script.Contains("--artifacts-path", StringComparison.Ordinal)
                    && script.Contains("contract-artifacts", StringComparison.Ordinal)
                    && script.Contains("-Step \"release-build\"", StringComparison.Ordinal),
                    "BaseLib verification, work-scoped contract outputs, contract runner, and Release build present",
                    "one or more quality-gate execution stages are missing");
            });
        yield return new(
            "QG-003",
            "QualityGate",
            "QualityGate.DoesNotPublishInstallLaunchOrUpload",
            assert =>
            {
                var script = IdentityContractFixture.Read(ScriptPath);
                var forbidden = new[]
                {
                    "\"publish\", $projectPath",
                    "Install-LocalMod.ps1",
                    "Start-Process",
                    "SlayTheSpire2.exe",
                    "workshop"
                }.Where(token => script.Contains(token, StringComparison.OrdinalIgnoreCase)).ToArray();
                assert.True(
                    forbidden.Length == 0,
                    "no publish/install/game/Workshop operation",
                    string.Join(",", forbidden));
            });
        yield return new(
            "QG-004",
            "QualityGate",
            "QualityGate.ClosesWithGitAndArtifactReview",
            assert =>
            {
                var script = IdentityContractFixture.Read(ScriptPath);
                assert.True(
                    script.Contains("diff --check", StringComparison.Ordinal)
                    && script.Contains("ls-files", StringComparison.Ordinal)
                    && script.Contains("status --short --untracked-files=all", StringComparison.Ordinal)
                    && script.Contains("QUALITY_GATE", StringComparison.Ordinal)
                    && script.Contains("exit_code=0", StringComparison.Ordinal),
                    "git diff, tracked/working artifact review, and final result output present",
                    "one or more closure checks are missing");
            });
    }
}
