using DamageForecast.Settings;

namespace DamageForecast.Compatibility;

internal enum ConfigMigrationGrade
{
    ExactSuccess,
    RecoveredSuccess,
    Salvaged,
    FailedSafe
}

internal enum ConfigMigrationStatus
{
    FreshInstall,
    ExistingCurrent,
    AlreadyCompleted,
    Completed,
    Recovered,
    Conflict,
    Salvaged,
    FailedSafe,
    RolledBack
}

[Flags]
internal enum ConfigMigrationFailurePoint
{
    None = 0,
    BackupCreate = 1 << 0,
    BackupFlush = 1 << 1,
    TempWrite = 1 << 2,
    TargetMove = 1 << 3,
    MarkerWrite = 1 << 4,
    CleanupTemp = 1 << 5,
    ReverseMove = 1 << 6,
    DiskFull = 1 << 7,
    AccessDenied = 1 << 8,
    SourceRead = 1 << 9,
    TargetRace = 1 << 10
}

internal sealed record ConfigSourceMetadata(
    long Length,
    string Sha256,
    string OrderedKeyDigest,
    string TypedSemanticDigest,
    IReadOnlyList<string> OrderedKeys);

internal sealed record ConfigValidationResult(
    ConfigMigrationGrade Grade,
    DamageForecastConfigSnapshot? Snapshot,
    ConfigSourceMetadata Metadata,
    IReadOnlyDictionary<string, string> ExtensionBag,
    IReadOnlyList<string> Diagnostics)
{
    public bool IsSuccessful => Grade is ConfigMigrationGrade.ExactSuccess or ConfigMigrationGrade.RecoveredSuccess;
}

internal sealed record ConfigMigrationResult(
    ConfigMigrationGrade Grade,
    ConfigMigrationStatus Status,
    bool MayRegisterCurrentConfig,
    string Message,
    ConfigMigrationMarker? Marker = null)
{
    public bool IsSuccessful => Grade is ConfigMigrationGrade.ExactSuccess or ConfigMigrationGrade.RecoveredSuccess;
}

internal sealed record ConfigMigrationMarker(
    int SchemaVersion,
    string TransactionId,
    string SourceSchema,
    string TargetSchema,
    string SourcePath,
    string TargetPath,
    string BackupPath,
    string TempPath,
    long SourceLength,
    long TargetLength,
    long BackupLength,
    string SourceSha256,
    string TargetSha256,
    string BackupSha256,
    string OrderedKeyDigest,
    string TypedSemanticDigest,
    string MigrationStrategy,
    string ModVersion,
    string GameVersion,
    DateTimeOffset TimestampUtc,
    string Status,
    string ReverseSyncStatus,
    bool CleanupEligible);

internal sealed record ConfigMigrationOptions(
    string ConfigRoot,
    string MigrationRoot,
    string TransactionId,
    string ModVersion,
    string GameVersion,
    DateTimeOffset TimestampUtc,
    ConfigMigrationFailurePoint FailurePoints = ConfigMigrationFailurePoint.None,
    Action? BeforeExecute = null)
{
    public string LegacyConfigPath => Path.Combine(ConfigRoot, LegacyIdentityDescriptor.ConfigFileName);
    public string CurrentConfigPath => Path.Combine(ConfigRoot, DamageForecastSchemaV1.ConfigFileName);
    public string TransactionsRoot => Path.Combine(MigrationRoot, "transactions");
    public string TransactionRoot => Path.Combine(TransactionsRoot, TransactionId);
    public string BackupPath => Path.Combine(TransactionRoot, LegacyIdentityDescriptor.ConfigFileName + ".backup");
    public string MarkerPath => Path.Combine(TransactionRoot, "marker.json");
    public string MarkerTempPath => Path.Combine(TransactionRoot, "marker.json.tmp");
    public string TargetTempPath => Path.Combine(ConfigRoot, $".{DamageForecastSchemaV1.ConfigFileName}.{TransactionId}.tmp");
    public string ReverseTempPath => Path.Combine(ConfigRoot, $".{LegacyIdentityDescriptor.ConfigFileName}.{TransactionId}.reverse.tmp");

    public static ConfigMigrationOptions CreateDefault(string modVersion, string gameVersion)
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var gameDataRoot = Path.Combine(appData, "SlayTheSpire2");
        return new ConfigMigrationOptions(
            ConfigRoot: Path.Combine(gameDataRoot, "mod_configs"),
            MigrationRoot: Path.Combine(gameDataRoot, "damage-forecast-migration"),
            TransactionId: $"{DateTime.UtcNow:yyyyMMddTHHmmssfffZ}-{Guid.NewGuid():N}",
            ModVersion: modVersion,
            GameVersion: gameVersion,
            TimestampUtc: DateTimeOffset.UtcNow);
    }
}
