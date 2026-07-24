namespace DamageForecast.Compatibility;

internal sealed record ConfigSchemaDescriptor(
    string Id,
    string ConfigFileName,
    IReadOnlyList<string> OrderedKeys,
    string HudEnabledKey,
    bool IsCurrent);

internal static class ConfigSchemaRegistry
{
    private static readonly IReadOnlyDictionary<string, ConfigSchemaDescriptor> Schemas =
        new[] { PreDamageForecastSchemaV1.Descriptor, DamageForecastSchemaV1.Descriptor }
            .ToDictionary(schema => schema.Id, StringComparer.Ordinal);

    private static readonly IReadOnlyDictionary<string, string> DirectMigrationTargets =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [LegacyIdentityDescriptor.SchemaId] = DamageForecastSchemaV1.SchemaId,
            [DamageForecastSchemaV1.SchemaId] = DamageForecastSchemaV1.SchemaId
        };

    public static IReadOnlyCollection<ConfigSchemaDescriptor> SupportedSchemas => Schemas.Values.ToArray();
    public static ConfigSchemaDescriptor Current => DamageForecastSchemaV1.Descriptor;

    public static ConfigSchemaDescriptor Get(string schemaId) =>
        Schemas.TryGetValue(schemaId, out var schema)
            ? schema
            : throw new KeyNotFoundException($"Unsupported config schema: {schemaId}");

    public static bool CanMigrateDirectlyToCurrent(string sourceSchemaId) =>
        DirectMigrationTargets.TryGetValue(sourceSchemaId, out var target)
        && target == Current.Id;
}
