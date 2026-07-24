using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using DamageForecast.Settings;
using DamageForecast.UI;

namespace DamageForecast.Compatibility;

internal static class ConfigDigest
{
    public static string Sha256(ReadOnlySpan<byte> bytes) =>
        Convert.ToHexString(SHA256.HashData(bytes));

    public static string Sha256(string value) =>
        Sha256(Encoding.UTF8.GetBytes(value));
}

internal static class ConfigSchemaDetector
{
    private static readonly UTF8Encoding StrictUtf8 = new(false, true);

    public static ConfigValidationResult Validate(byte[] raw, ConfigSchemaDescriptor schema)
    {
        var diagnostics = new List<string>();
        var rawSha = ConfigDigest.Sha256(raw);
        string text;
        var recoveredRepresentation = false;
        try
        {
            text = StrictUtf8.GetString(raw);
        }
        catch (DecoderFallbackException exception)
        {
            return Failure(
                ConfigMigrationGrade.FailedSafe,
                raw.LongLength,
                rawSha,
                "invalid-utf8",
                exception.Message);
        }

        if (text.Length > 0 && text[0] == '\uFEFF')
        {
            text = text[1..];
            recoveredRepresentation = true;
            diagnostics.Add("utf8-bom-normalized");
        }

        JsonDocument document;
        try
        {
            document = JsonDocument.Parse(text, new JsonDocumentOptions
            {
                AllowTrailingCommas = false,
                CommentHandling = JsonCommentHandling.Disallow
            });
        }
        catch (JsonException exception)
        {
            return Failure(
                ConfigMigrationGrade.FailedSafe,
                raw.LongLength,
                rawSha,
                "invalid-json",
                exception.Message);
        }

        using (document)
        {
            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                return Failure(
                    ConfigMigrationGrade.FailedSafe,
                    raw.LongLength,
                    rawSha,
                    "root-not-object",
                    document.RootElement.ValueKind.ToString());
            }

            var properties = document.RootElement.EnumerateObject().ToArray();
            var orderedKeys = properties.Select(property => property.Name).ToArray();
            var orderedKeyDigest = ConfigDigest.Sha256(string.Join("\n", orderedKeys));
            var duplicates = orderedKeys.GroupBy(key => key, StringComparer.Ordinal)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToArray();
            if (duplicates.Length > 0)
            {
                diagnostics.Add($"duplicate-keys:{string.Join(',', duplicates)}");
                return Salvaged(raw.LongLength, rawSha, orderedKeyDigest, orderedKeys, diagnostics);
            }

            var values = properties.ToDictionary(property => property.Name, property => property.Value, StringComparer.Ordinal);
            var expected = schema.OrderedKeys.ToHashSet(StringComparer.Ordinal);
            var missing = schema.OrderedKeys.Where(key => !values.ContainsKey(key)).ToArray();
            var unknown = orderedKeys.Where(key => !expected.Contains(key)).ToArray();
            var extensionBag = unknown.ToDictionary(
                key => key,
                key => values[key].GetRawText(),
                StringComparer.Ordinal);
            if (missing.Length > 0)
            {
                diagnostics.Add($"missing-keys:{string.Join(',', missing)}");
            }
            if (unknown.Length > 0)
            {
                diagnostics.Add($"unknown-keys:{string.Join(',', unknown)}");
            }
            if (missing.Length > 0)
            {
                return Salvaged(raw.LongLength, rawSha, orderedKeyDigest, orderedKeys, diagnostics, extensionBag);
            }

            var parseRecovered = false;
            var parseErrors = new List<string>();

            if (!TryReadEnum(values["ConfigLanguage"], out DamageForecastConfigLanguage configLanguage, ref parseRecovered))
                parseErrors.Add("invalid:ConfigLanguage");
            if (!TryReadBool(values[schema.HudEnabledKey], out var hudEnabled, ref parseRecovered))
                parseErrors.Add($"invalid:{schema.HudEnabledKey}");
            if (!TryReadBool(values["ShowAdvancedShieldHeartDetails"], out var showDetails, ref parseRecovered))
                parseErrors.Add("invalid:ShowAdvancedShieldHeartDetails");
            if (!TryReadBool(values["FreezeHudNumbersAfterTurnEnd"], out var freeze, ref parseRecovered))
                parseErrors.Add("invalid:FreezeHudNumbersAfterTurnEnd");
            if (!TryReadEnum(values["DamageDisplayMode"], out DamageDisplayMode displayMode, ref parseRecovered))
                parseErrors.Add("invalid:DamageDisplayMode");
            if (!TryReadEnum(values["IncomingDamagePlacement"], out IncomingDamagePlacement placement, ref parseRecovered))
                parseErrors.Add("invalid:IncomingDamagePlacement");
            if (!TryReadBool(values["IncludeCurrentBlockInIncomingDamage"], out var currentBlock, ref parseRecovered))
                parseErrors.Add("invalid:IncludeCurrentBlockInIncomingDamage");
            if (!TryReadBool(values["IncludePowerBlockInIncomingDamage"], out var powerBlock, ref parseRecovered))
                parseErrors.Add("invalid:IncludePowerBlockInIncomingDamage");
            if (!TryReadBool(values["IncludeRelicBlockInIncomingDamage"], out var relicBlock, ref parseRecovered))
                parseErrors.Add("invalid:IncludeRelicBlockInIncomingDamage");
            if (!TryReadBool(values["IncludePowerHpLossModifiersInIncomingDamage"], out var powerLoss, ref parseRecovered))
                parseErrors.Add("invalid:IncludePowerHpLossModifiersInIncomingDamage");
            if (!TryReadBool(values["IncludeRelicHpLossModifiersInIncomingDamage"], out var relicLoss, ref parseRecovered))
                parseErrors.Add("invalid:IncludeRelicHpLossModifiersInIncomingDamage");
            if (!TryReadBool(values["ShowLocalPlayerHudInMultiplayer"], out var multiplayer, ref parseRecovered))
                parseErrors.Add("invalid:ShowLocalPlayerHudInMultiplayer");
            if (!TryReadEnum(values["HudAnchorPreset"], out DamageForecastHudAnchor anchor, ref parseRecovered))
                parseErrors.Add("invalid:HudAnchorPreset");
            if (!TryReadFloat(values["HorizontalOffset"], out var horizontal, ref parseRecovered))
                parseErrors.Add("invalid:HorizontalOffset");
            if (!TryReadFloat(values["VerticalOffset"], out var vertical, ref parseRecovered))
                parseErrors.Add("invalid:VerticalOffset");
            if (!TryReadColor(values["TotalExpectedLossColor"], out var totalColor, ref parseRecovered))
                parseErrors.Add("invalid:TotalExpectedLossColor");
            if (!TryReadColor(values["ShieldDetailColor"], out var shieldColor, ref parseRecovered))
                parseErrors.Add("invalid:ShieldDetailColor");
            if (!TryReadColor(values["HeartDetailColor"], out var heartColor, ref parseRecovered))
                parseErrors.Add("invalid:HeartDetailColor");

            if (parseErrors.Count > 0)
            {
                diagnostics.AddRange(parseErrors);
                return Salvaged(raw.LongLength, rawSha, orderedKeyDigest, orderedKeys, diagnostics, extensionBag);
            }

            var snapshot = new DamageForecastConfigSnapshot(
                configLanguage,
                hudEnabled,
                showDetails,
                freeze,
                displayMode,
                placement,
                currentBlock,
                powerBlock,
                relicBlock,
                powerLoss,
                relicLoss,
                multiplayer,
                anchor,
                horizontal,
                vertical,
                totalColor,
                shieldColor,
                heartColor);
            var semanticDigest = ConfigDigest.Sha256(snapshot.ToCanonicalString());
            var exactOrder = orderedKeys.SequenceEqual(schema.OrderedKeys, StringComparer.Ordinal);
            if (!exactOrder)
            {
                diagnostics.Add("key-order-normalized");
            }
            if (parseRecovered)
            {
                diagnostics.Add("value-representation-normalized");
            }

            var grade = unknown.Length > 0
                ? ConfigMigrationGrade.Salvaged
                : exactOrder && !parseRecovered && !recoveredRepresentation
                    ? ConfigMigrationGrade.ExactSuccess
                    : ConfigMigrationGrade.RecoveredSuccess;
            return new ConfigValidationResult(
                grade,
                snapshot,
                new ConfigSourceMetadata(raw.LongLength, rawSha, orderedKeyDigest, semanticDigest, orderedKeys),
                extensionBag,
                diagnostics);
        }
    }

    private static bool TryReadBool(JsonElement element, out bool value, ref bool recovered)
    {
        if (element.ValueKind is JsonValueKind.True or JsonValueKind.False)
        {
            value = element.GetBoolean();
            recovered = true;
            return true;
        }
        if (element.ValueKind == JsonValueKind.String)
        {
            var text = element.GetString();
            if (bool.TryParse(text, out value))
            {
                recovered |= text is not ("True" or "False");
                return true;
            }
        }
        value = default;
        return false;
    }

    private static bool TryReadEnum<T>(JsonElement element, out T value, ref bool recovered)
        where T : struct, Enum
    {
        if (element.ValueKind == JsonValueKind.String)
        {
            var text = element.GetString();
            if (Enum.TryParse<T>(text, ignoreCase: false, out value) && Enum.IsDefined(value))
            {
                return true;
            }
        }
        else if (element.ValueKind == JsonValueKind.Number && element.TryGetInt32(out var numeric))
        {
            var candidate = (T)Enum.ToObject(typeof(T), numeric);
            if (Enum.IsDefined(candidate))
            {
                value = candidate;
                recovered = true;
                return true;
            }
        }
        value = default;
        return false;
    }

    private static bool TryReadFloat(JsonElement element, out float value, ref bool recovered)
    {
        if (element.ValueKind == JsonValueKind.String)
        {
            if (float.TryParse(
                    element.GetString(),
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out value)
                && float.IsFinite(value))
            {
                return true;
            }
        }
        else if (element.ValueKind == JsonValueKind.Number && element.TryGetSingle(out value) && float.IsFinite(value))
        {
            recovered = true;
            return true;
        }
        value = default;
        return false;
    }

    private static bool TryReadColor(JsonElement element, out DamageForecastRgba color, ref bool recovered)
    {
        float[] components;
        if (element.ValueKind == JsonValueKind.String)
        {
            var text = element.GetString()?.Trim();
            if (text is null || text.Length < 2 || text[0] != '[' || text[^1] != ']')
            {
                color = default;
                return false;
            }
            var pieces = text[1..^1].Split(',', StringSplitOptions.TrimEntries);
            if (pieces.Length != 4)
            {
                color = default;
                return false;
            }
            components = new float[4];
            for (var index = 0; index < pieces.Length; index++)
            {
                if (!float.TryParse(pieces[index], NumberStyles.Float, CultureInfo.InvariantCulture, out components[index])
                    || !float.IsFinite(components[index]))
                {
                    color = default;
                    return false;
                }
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            var items = element.EnumerateArray().ToArray();
            if (items.Length != 4)
            {
                color = default;
                return false;
            }
            components = new float[4];
            for (var index = 0; index < items.Length; index++)
            {
                if (!items[index].TryGetSingle(out components[index]) || !float.IsFinite(components[index]))
                {
                    color = default;
                    return false;
                }
            }
            recovered = true;
        }
        else
        {
            color = default;
            return false;
        }
        color = new DamageForecastRgba(components[0], components[1], components[2], components[3]);
        return true;
    }

    private static ConfigValidationResult Failure(
        ConfigMigrationGrade grade,
        long length,
        string rawSha,
        string code,
        string detail)
    {
        return new ConfigValidationResult(
            grade,
            null,
            new ConfigSourceMetadata(length, rawSha, string.Empty, string.Empty, []),
            new Dictionary<string, string>(StringComparer.Ordinal),
            [$"{code}:{detail}"]);
    }

    private static ConfigValidationResult Salvaged(
        long length,
        string rawSha,
        string orderedKeyDigest,
        IReadOnlyList<string> orderedKeys,
        IReadOnlyList<string> diagnostics,
        IReadOnlyDictionary<string, string>? extensionBag = null)
    {
        return new ConfigValidationResult(
            ConfigMigrationGrade.Salvaged,
            null,
            new ConfigSourceMetadata(length, rawSha, orderedKeyDigest, string.Empty, orderedKeys),
            extensionBag ?? new Dictionary<string, string>(StringComparer.Ordinal),
            diagnostics);
    }
}
