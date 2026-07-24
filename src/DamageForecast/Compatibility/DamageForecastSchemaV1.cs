using DamageForecast.Settings;

namespace DamageForecast.Compatibility;

internal static class DamageForecastSchemaV1
{
    public const string SchemaId = "damage-forecast-v1";
    public const string ConfigFileName = "DamageForecast.cfg";
    public const string HudEnabledKey = "EnableDamageForecastHud";

    public static readonly ConfigSchemaDescriptor Descriptor = new(
        Id: SchemaId,
        ConfigFileName: ConfigFileName,
        OrderedKeys: DamageForecastConfigSchema.PropertyOrder,
        HudEnabledKey: HudEnabledKey,
        IsCurrent: true);
}
