using System.Text.Json;

internal static class PostG6PersistenceContractFixture
{
    public static PostG6PersistenceContract Contract { get; } =
        JsonSerializer.Deserialize<PostG6PersistenceContract>(
            IdentityContractFixture.Read("tests/DamageForecast.ContractTests/post-g6-persistence-contract.json"),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
        ?? throw new InvalidOperationException("Post-G6 persistence contract deserialized to null.");
}

internal sealed record PostG6PersistenceContract(
    int SchemaVersion,
    string Status,
    PostG6PersistenceApproval Approval,
    PostG6Closure Closure,
    PostG6PersistenceRelease Release,
    PostG6PersistenceIdentity Legacy,
    PostG6PersistenceIdentity Current,
    int PropertyCount,
    IReadOnlyDictionary<string, string> KeyMap,
    IReadOnlyList<string> SuccessGrades,
    IReadOnlyList<string> FailureGrades,
    IReadOnlyDictionary<string, string> SchemaGraph,
    string CompatibilityRoot,
    IReadOnlyList<string> CompatibilityFiles,
    IReadOnlyList<string> OrdinaryLegacyMarkers,
    IReadOnlyList<PostG6RuntimeToken> RuntimeTokenInventory,
    IReadOnlyList<string> OrdinaryLayerRoots,
    IReadOnlyList<PostG6AuthorityRule> CurrentAuthorityRules,
    IReadOnlyList<string> HistoricalEvidencePrefixes,
    string MigrationRootName,
    PostG6RealConfigBaseline RealBaseline,
    bool RuntimeExecutedInC2);

internal sealed record PostG6PersistenceApproval(string Gate, string ApprovedAt, string Scope);
internal sealed record PostG6Closure(
    string Gate,
    string ApprovedAt,
    string Scope,
    string RollbackEvidence,
    string InstalledIdentity,
    string ActiveConfig,
    string WorkshopDisposition,
    string HistoricalEvidenceDisposition);
internal sealed record PostG6PersistenceRelease(string Version, string ModId, string WorkshopDisposition);
internal sealed record PostG6PersistenceIdentity(string Schema, string Type, string File, string HudKey);
internal sealed record PostG6RuntimeToken(
    string Token,
    string OriginVersion,
    string Purpose,
    string RetirementGate,
    IReadOnlyList<string> Locations,
    IReadOnlyList<string> VerificationIds);
internal sealed record PostG6AuthorityRule(
    string Path,
    IReadOnlyList<string> RequiredTokens,
    IReadOnlyList<string> ForbiddenTokens);
internal sealed record PostG6RealConfigBaseline(
    string File,
    long Length,
    string Sha256,
    int PropertyCount,
    bool MutationAllowedInC2);
