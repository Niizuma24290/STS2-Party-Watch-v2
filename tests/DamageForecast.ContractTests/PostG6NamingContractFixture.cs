using System.Text.Json;

internal static class PostG6NamingContractFixture
{
    private static readonly Lazy<PostG6NamingContract> Loaded = new(Load);

    public static PostG6NamingContract Contract => Loaded.Value;

    public static string Read(string relativePath) => IdentityContractFixture.Read(relativePath);

    public static string Resolve(string relativePath) => Path.Combine(
        IdentityContractFixture.RepositoryRoot,
        relativePath.Replace('/', Path.DirectorySeparatorChar));

    public static IReadOnlyList<string> ActiveCompilerFiles()
    {
        var roots = new[]
        {
            Resolve("src/DamageForecast"),
            Resolve("tests/DamageForecast.ContractTests")
        };
        return roots.SelectMany(root => Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
            .Where(path => Path.GetExtension(path) is ".cs" or ".csproj")
            .ToArray();
    }

    private static PostG6NamingContract Load() =>
        JsonSerializer.Deserialize<PostG6NamingContract>(
            Read("tests/DamageForecast.ContractTests/post-g6-naming-contract.json"),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
        ?? throw new InvalidOperationException("Post-G6 naming contract deserialized to null.");
}

internal sealed record PostG6NamingContract(
    int SchemaVersion,
    string Status,
    PostG6NamingApproval Approval,
    IReadOnlyList<PostG6NameMapping> OrdinarySymbolMappings,
    IReadOnlyList<PostG6PathMapping> FileMappings,
    IReadOnlyList<PostG6NameMapping> MsbuildMappings,
    IReadOnlyList<PostG6GodotNodeMapping> GodotNodeMappings,
    PostG6FrozenPersistence FrozenPersistence,
    IReadOnlyList<string> ExcludedHistoricalContracts);

internal sealed record PostG6NamingApproval(string Gate, string ApprovedAt, string Scope);
internal sealed record PostG6NameMapping(string Legacy, string Current);
internal sealed record PostG6PathMapping(string Legacy, string Current);
internal sealed record PostG6GodotNodeMapping(string Legacy, string Current, string OwnershipGroup);
internal sealed record PostG6FrozenPersistence(
    string Namespace,
    string Type,
    string File,
    string PersistedMember,
    int PropertyCount);
