using System.Globalization;
using System.Text;
using System.Text.Json;
using DamageForecast.Settings;

namespace DamageForecast.Compatibility;

internal static class ConfigMigrationPipeline
{
    public static byte[] TransformToCurrent(ConfigValidationResult source)
    {
        if (!source.IsSuccessful || source.Snapshot is null)
        {
            throw new InvalidOperationException("Only an exact or recovered supported source can be migrated.");
        }
        return Serialize(source.Snapshot, DamageForecastSchemaV1.Descriptor);
    }

    public static byte[] TransformToLegacy(ConfigValidationResult source)
    {
        if (!source.IsSuccessful || source.Snapshot is null)
        {
            throw new InvalidOperationException("Only an exact or recovered current source can be reverse-synced.");
        }
        return Serialize(source.Snapshot, PreDamageForecastSchemaV1.Descriptor);
    }

    public static bool AreSemanticallyEqual(ConfigValidationResult left, ConfigValidationResult right) =>
        left.IsSuccessful
        && right.IsSuccessful
        && left.Snapshot is not null
        && left.Snapshot == right.Snapshot
        && left.Metadata.TypedSemanticDigest == right.Metadata.TypedSemanticDigest;

    public static byte[] Serialize(DamageForecastConfigSnapshot snapshot, ConfigSchemaDescriptor schema)
        => Serialize(snapshot, schema, new Dictionary<string, string>(StringComparer.Ordinal));

    public static byte[] Serialize(
        DamageForecastConfigSnapshot snapshot,
        ConfigSchemaDescriptor schema,
        IReadOnlyDictionary<string, string> optionalSettingDefaults)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }))
        {
            writer.WriteStartObject();
            foreach (var key in schema.OrderedKeys)
            {
                writer.WriteString(
                    key,
                    optionalSettingDefaults.TryGetValue(key, out var defaultValue)
                        ? defaultValue
                        : ValueFor(key, schema, snapshot));
            }
            writer.WriteEndObject();
        }
        return stream.ToArray();
    }

    private static string ValueFor(
        string key,
        ConfigSchemaDescriptor schema,
        DamageForecastConfigSnapshot snapshot)
    {
        if (key == schema.HudEnabledKey)
        {
            return Bool(snapshot.EnableDamageForecastHud);
        }
        return key switch
        {
            "ConfigLanguage" => snapshot.ConfigLanguage.ToString(),
            "ShowAdvancedShieldHeartDetails" => Bool(snapshot.ShowAdvancedShieldHeartDetails),
            "FreezeHudNumbersAfterTurnEnd" => Bool(snapshot.FreezeHudNumbersAfterTurnEnd),
            "DamageDisplayMode" => snapshot.DamageDisplayMode.ToString(),
            "IncomingDamagePlacement" => snapshot.IncomingDamagePlacement.ToString(),
            "IncludeCurrentBlockInIncomingDamage" => Bool(snapshot.IncludeCurrentBlockInIncomingDamage),
            "IncludePowerBlockInIncomingDamage" => Bool(snapshot.IncludePowerBlockInIncomingDamage),
            "IncludeRelicBlockInIncomingDamage" => Bool(snapshot.IncludeRelicBlockInIncomingDamage),
            "IncludePowerHpLossModifiersInIncomingDamage" => Bool(snapshot.IncludePowerHpLossModifiersInIncomingDamage),
            "IncludeRelicHpLossModifiersInIncomingDamage" => Bool(snapshot.IncludeRelicHpLossModifiersInIncomingDamage),
            "ShowLocalPlayerHudInMultiplayer" => Bool(snapshot.ShowLocalPlayerHudInMultiplayer),
            "HudAnchorPreset" => snapshot.HudAnchorPreset.ToString(),
            "HorizontalOffset" => Float(snapshot.HorizontalOffset),
            "VerticalOffset" => Float(snapshot.VerticalOffset),
            "TotalExpectedLossColor" => Color(snapshot.TotalExpectedLossColor),
            "ShieldDetailColor" => Color(snapshot.ShieldDetailColor),
            "HeartDetailColor" => Color(snapshot.HeartDetailColor),
            _ => throw new InvalidOperationException($"Unsupported config property: {key}")
        };
    }

    private static string Bool(bool value) => value ? "True" : "False";
    private static string Float(float value) => value.ToString("R", CultureInfo.InvariantCulture);
    private static string Color(DamageForecastRgba color) =>
        $"[{string.Join(", ", color.Components.Select(Float))}]";
}
