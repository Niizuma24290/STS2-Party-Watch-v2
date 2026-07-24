param(
    [ValidateSet("Plan", "Install", "Rollback")]
    [string]$Mode = "Plan",
    [ValidateSet("Install", "Rollback")]
    [string]$PlanOperation = "Install",
    [string]$StagingDir = "work/publish/damage-forecast",
    [string]$Sts2GameRoot = "C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2",
    [string]$BackupRoot = "",
    [string]$BackupPath = "",
    [string]$WorkshopRoot = "",
    [string]$TransactionId = "",
    [string]$ExpectedManifestSha256 = "",
    [string]$ExpectedDllSha256 = "",
    [string]$ExpectedActiveManifestSha256 = "",
    [string]$ExpectedActiveDllSha256 = "",
    [string]$ExpectedBackupManifestSha256 = "",
    [string]$ExpectedBackupDllSha256 = "",
    [string]$ConfigRoot = "",
    [string]$ConfigMigrationRoot = "",
    [string]$ExpectedLegacyConfigSha256 = "",
    [string]$ExpectedCurrentConfigSha256 = "",
    [switch]$IncludeWorkshop,
    [switch]$Execute
)

$ErrorActionPreference = "Stop"

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$modId = "damage-forecast"
$legacyModId = "sts2-party-watch-v2"
$manifestName = "$modId.json"
$dllName = "$modId.dll"
$legacyManifestName = "$legacyModId.json"
$legacyDllName = "$legacyModId.dll"
$resolvedGameRoot = [System.IO.Path]::GetFullPath($Sts2GameRoot)
$modsRoot = [System.IO.Path]::GetFullPath((Join-Path $resolvedGameRoot "mods"))
$installPath = [System.IO.Path]::GetFullPath((Join-Path $modsRoot $modId))
$legacyInstallPath = [System.IO.Path]::GetFullPath((Join-Path $modsRoot $legacyModId))
$resolvedStagingPath = if ([System.IO.Path]::IsPathRooted($StagingDir)) {
    [System.IO.Path]::GetFullPath($StagingDir)
} else {
    [System.IO.Path]::GetFullPath((Join-Path $repoRoot $StagingDir))
}
if ([string]::IsNullOrWhiteSpace($BackupRoot)) {
    $BackupRoot = Join-Path $resolvedGameRoot ".damage-forecast-backups"
}
$resolvedBackupRoot = [System.IO.Path]::GetFullPath($BackupRoot)
$configRootWasExplicit = -not [string]::IsNullOrWhiteSpace($ConfigRoot)
if (-not $configRootWasExplicit) {
    $ConfigRoot = Join-Path ([Environment]::GetFolderPath([Environment+SpecialFolder]::ApplicationData)) "SlayTheSpire2\mod_configs"
}
if ([string]::IsNullOrWhiteSpace($ConfigMigrationRoot)) {
    $ConfigMigrationRoot = Join-Path ([Environment]::GetFolderPath([Environment+SpecialFolder]::ApplicationData)) "SlayTheSpire2\damage-forecast-migration"
}
$resolvedConfigRoot = [System.IO.Path]::GetFullPath($ConfigRoot)
$resolvedConfigMigrationRoot = [System.IO.Path]::GetFullPath($ConfigMigrationRoot)
$legacyConfigPath = [System.IO.Path]::GetFullPath((Join-Path $resolvedConfigRoot "STS2PartyWatch.cfg"))
$currentConfigPath = [System.IO.Path]::GetFullPath((Join-Path $resolvedConfigRoot "DamageForecast.cfg"))

function Assert-PathWithinRoot {
    param([string]$Root, [string]$Candidate, [string]$Label)
    $rootPath = [System.IO.Path]::GetFullPath($Root).TrimEnd([System.IO.Path]::DirectorySeparatorChar)
    $candidatePath = [System.IO.Path]::GetFullPath($Candidate)
    $rootPrefix = $rootPath + [System.IO.Path]::DirectorySeparatorChar
    if (-not $candidatePath.StartsWith($rootPrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "$Label must remain within $rootPath`: $candidatePath"
    }
    return $candidatePath
}

function Assert-PathOutsideRoot {
    param([string]$Root, [string]$Candidate, [string]$Label)
    $rootPath = [System.IO.Path]::GetFullPath($Root).TrimEnd([System.IO.Path]::DirectorySeparatorChar)
    $candidatePath = [System.IO.Path]::GetFullPath($Candidate).TrimEnd([System.IO.Path]::DirectorySeparatorChar)
    $rootPrefix = $rootPath + [System.IO.Path]::DirectorySeparatorChar
    if ($candidatePath.Equals($rootPath, [System.StringComparison]::OrdinalIgnoreCase) -or
        $candidatePath.StartsWith($rootPrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "$Label must remain outside Loader-scanned root $rootPath`: $candidatePath"
    }
    return $candidatePath
}

function Get-StrictConfigSnapshot {
    param([string]$Path, [ValidateSet("Legacy", "Current")][string]$Schema)

    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) { return $null }
    $hudKey = if ($Schema -eq "Legacy") { "EnablePartyWatchHud" } else { "EnableDamageForecastHud" }
    $keys = @("ConfigLanguage", $hudKey, "ShowAdvancedShieldHeartDetails", "FreezeHudNumbersAfterTurnEnd",
        "DamageDisplayMode", "IncomingDamagePlacement", "IncludeCurrentBlockInIncomingDamage",
        "IncludePowerBlockInIncomingDamage", "IncludeRelicBlockInIncomingDamage",
        "IncludePowerHpLossModifiersInIncomingDamage", "IncludeRelicHpLossModifiersInIncomingDamage",
        "ShowLocalPlayerHudInMultiplayer", "HudAnchorPreset", "HorizontalOffset", "VerticalOffset",
        "TotalExpectedLossColor", "ShieldDetailColor", "HeartDetailColor")
    $raw = [System.IO.File]::ReadAllText($Path, [System.Text.UTF8Encoding]::new($false, $true))
    try { $json = $raw | ConvertFrom-Json } catch { throw "Invalid $Schema config JSON: $Path" }
    $properties = @($json.PSObject.Properties)
    $names = @($properties | ForEach-Object Name)
    if ($names.Count -ne 18 -or (($names -join "`n") -ne ($keys -join "`n"))) {
        throw "$Schema config must contain the exact ordered 18-key schema: $Path"
    }
    foreach ($key in $keys) {
        $occurrences = [regex]::Matches($raw, '"' + [regex]::Escape($key) + '"\s*:').Count
        if ($occurrences -ne 1) { throw "$Schema config key must occur exactly once: $key" }
        if ($json.$key -isnot [string]) { throw "$Schema config values must use official BaseLib strings: $key" }
    }
    $boolKeys = @($hudKey, "ShowAdvancedShieldHeartDetails", "FreezeHudNumbersAfterTurnEnd",
        "IncludeCurrentBlockInIncomingDamage", "IncludePowerBlockInIncomingDamage", "IncludeRelicBlockInIncomingDamage",
        "IncludePowerHpLossModifiersInIncomingDamage", "IncludeRelicHpLossModifiersInIncomingDamage", "ShowLocalPlayerHudInMultiplayer")
    foreach ($key in $boolKeys) { if ($json.$key -notin @("True", "False")) { throw "Invalid bool: $key" } }
    if ($json.ConfigLanguage -notin @("English", "SimplifiedChinese")) { throw "Invalid ConfigLanguage" }
    if ($json.DamageDisplayMode -notin @("ExpectedHpLossOnly", "IncomingDamageOnly", "Both")) { throw "Invalid DamageDisplayMode" }
    if ($json.IncomingDamagePlacement -notin @("LeftOfExpectedHpLoss", "RightOfExpectedHpLoss")) { throw "Invalid IncomingDamagePlacement" }
    if ($json.HudAnchorPreset -notin @("HealthBarRight", "HealthBarLeft", "HealthBarAbove", "HealthBarBelow")) { throw "Invalid HudAnchorPreset" }
    foreach ($key in @("HorizontalOffset", "VerticalOffset")) {
        $number = 0.0
        if (-not [double]::TryParse($json.$key, [Globalization.NumberStyles]::Float,
                [Globalization.CultureInfo]::InvariantCulture, [ref]$number) -or [double]::IsNaN($number) -or [double]::IsInfinity($number)) {
            throw "Invalid finite float: $key"
        }
    }
    foreach ($key in @("TotalExpectedLossColor", "ShieldDetailColor", "HeartDetailColor")) {
        $text = $json.$key.Trim()
        if (-not ($text.StartsWith("[") -and $text.EndsWith("]"))) { throw "Invalid color: $key" }
        $parts = @($text.Substring(1, $text.Length - 2).Split(',') | ForEach-Object { $_.Trim() })
        if ($parts.Count -ne 4) { throw "Color must have four components: $key" }
        foreach ($part in $parts) {
            $number = 0.0
            if (-not [double]::TryParse($part, [Globalization.NumberStyles]::Float,
                    [Globalization.CultureInfo]::InvariantCulture, [ref]$number) -or [double]::IsNaN($number) -or [double]::IsInfinity($number)) {
                throw "Invalid color component: $key"
            }
        }
    }
    $values = foreach ($key in $keys) { [string]$json.$key }
    return [pscustomobject]@{
        Path = $Path
        Sha256 = (Get-FileHash -Algorithm SHA256 -LiteralPath $Path).Hash
        Length = (Get-Item -LiteralPath $Path).Length
        SemanticDigest = [BitConverter]::ToString(
            [Security.Cryptography.SHA256]::Create().ComputeHash([Text.Encoding]::UTF8.GetBytes(($values -join "`n")))).Replace("-", "")
        Raw = $raw
    }
}

function Invoke-ConfigReverseSync {
    param([object]$Legacy, [object]$Current, [string]$Transaction)

    if ($Legacy.SemanticDigest -eq $Current.SemanticDigest) { return $null }
    if ([string]::IsNullOrWhiteSpace($ExpectedLegacyConfigSha256) -or [string]::IsNullOrWhiteSpace($ExpectedCurrentConfigSha256)) {
        throw "Rollback reverse-sync requires reviewed old/new config SHA256 values"
    }
    if ($Legacy.Sha256 -ne $ExpectedLegacyConfigSha256 -or $Current.Sha256 -ne $ExpectedCurrentConfigSha256) {
        throw "Config SHA256 changed after Plan review"
    }
    $transactionRoot = Assert-PathWithinRoot -Root $resolvedConfigMigrationRoot -Candidate (Join-Path $resolvedConfigMigrationRoot "transactions\$Transaction-installer-rollback") -Label "config transaction root"
    [System.IO.Directory]::CreateDirectory($transactionRoot) | Out-Null
    $backup = Assert-PathWithinRoot -Root $transactionRoot -Candidate (Join-Path $transactionRoot "STS2PartyWatch.cfg.backup") -Label "legacy config backup"
    $replaceBackup = Assert-PathWithinRoot -Root $transactionRoot -Candidate (Join-Path $transactionRoot "STS2PartyWatch.cfg.replace-backup") -Label "atomic replace recovery"
    $temp = Assert-PathWithinRoot -Root $resolvedConfigRoot -Candidate (Join-Path $resolvedConfigRoot ".STS2PartyWatch.cfg.$Transaction.reverse.tmp") -Label "legacy config temp"
    $bytes = [System.IO.File]::ReadAllBytes($legacyConfigPath)
    $stream = [System.IO.FileStream]::new($backup, [System.IO.FileMode]::CreateNew, [System.IO.FileAccess]::Write,
        [System.IO.FileShare]::None, 4096, [System.IO.FileOptions]::WriteThrough)
    try { $stream.Write($bytes, 0, $bytes.Length); $stream.Flush($true) } finally { $stream.Dispose() }
    if ((Get-FileHash -Algorithm SHA256 -LiteralPath $backup).Hash -ne $Legacy.Sha256) { throw "Legacy config backup verification failed" }
    $legacyRaw = $Current.Raw.Replace('"EnableDamageForecastHud"', '"EnablePartyWatchHud"')
    $targetBytes = [Text.UTF8Encoding]::new($false).GetBytes($legacyRaw)
    $stream = [System.IO.FileStream]::new($temp, [System.IO.FileMode]::CreateNew, [System.IO.FileAccess]::Write,
        [System.IO.FileShare]::None, 4096, [System.IO.FileOptions]::WriteThrough)
    try { $stream.Write($targetBytes, 0, $targetBytes.Length); $stream.Flush($true) } finally { $stream.Dispose() }
    $tempSnapshot = Get-StrictConfigSnapshot -Path $temp -Schema Legacy
    if ($tempSnapshot.SemanticDigest -ne $Current.SemanticDigest) { throw "Reverse-sync temp semantic verification failed" }
    [System.IO.File]::Replace($temp, $legacyConfigPath, $replaceBackup, $true)
    $final = Get-StrictConfigSnapshot -Path $legacyConfigPath -Schema Legacy
    if ($final.SemanticDigest -ne $Current.SemanticDigest) { throw "Reverse-sync final semantic verification failed" }
    return [pscustomobject]@{ backupPath = $backup; legacySha256 = $final.Sha256; semanticDigest = $final.SemanticDigest }
}

function Read-ManifestIdentity {
    param([string]$ManifestPath)
    try {
        $manifest = Get-Content -Raw -Encoding UTF8 -LiteralPath $ManifestPath | ConvertFrom-Json
    }
    catch {
        throw "Invalid manifest JSON: $ManifestPath`n$($_.Exception.Message)"
    }
    if ([string]::IsNullOrWhiteSpace($manifest.id)) {
        throw "Manifest has no id: $ManifestPath"
    }
    return $manifest
}

function Get-IdentityRecords {
    param([string]$Root, [string]$Source, [switch]$Recursive)
    if (-not (Test-Path -LiteralPath $Root -PathType Container)) {
        return @()
    }
    $manifests = if ($Recursive) {
        Get-ChildItem -LiteralPath $Root -File -Filter *.json -Recurse
    } else {
        Get-ChildItem -LiteralPath $Root -Directory | Where-Object {
            [System.IO.Path]::GetFullPath($Root) -ne $modsRoot -or
                -not $_.FullName.StartsWith($resolvedBackupRoot, [System.StringComparison]::OrdinalIgnoreCase)
        } | ForEach-Object { Get-ChildItem -LiteralPath $_.FullName -File -Filter *.json }
    }
    $records = foreach ($manifestFile in $manifests) {
        try {
            $manifest = Read-ManifestIdentity -ManifestPath $manifestFile.FullName
        }
        catch {
            if ($manifestFile.Name -in @($legacyManifestName, $manifestName)) {
                throw
            }
            continue
        }
        if ($manifest.id -notin @($legacyModId, $modId)) {
            continue
        }
        $directory = $manifestFile.Directory.FullName
        $expectedDll = if ($manifest.id -eq $legacyModId) { $legacyDllName } else { $dllName }
        [pscustomobject]@{
            Source = $Source
            Id = [string]$manifest.id
            Version = [string]$manifest.version
            Directory = $directory
            ManifestPath = $manifestFile.FullName
            DllPath = Join-Path $directory $expectedDll
            DllExists = Test-Path -LiteralPath (Join-Path $directory $expectedDll) -PathType Leaf
        }
    }
    return @($records)
}

function Get-OrphanIdentityArtifacts {
    param([string]$Root, [object[]]$Records)
    if (-not (Test-Path -LiteralPath $Root -PathType Container)) {
        return @()
    }
    $owned = @($Records | ForEach-Object { $_.ManifestPath; $_.DllPath })
    return @(Get-ChildItem -LiteralPath $Root -File -Recurse | Where-Object {
        $_.Name -in @($legacyManifestName, $legacyDllName, $manifestName, $dllName) -and
        $_.FullName -notin $owned -and
        -not $_.FullName.StartsWith($resolvedBackupRoot, [System.StringComparison]::OrdinalIgnoreCase)
    } | Select-Object -ExpandProperty FullName)
}

function Get-StagingIdentity {
    param([string]$Path, [string]$ExpectedId, [string]$ExpectedManifest, [string]$ExpectedDll)
    if (-not (Test-Path -LiteralPath $Path -PathType Container)) {
        throw "Staging directory not found: $Path"
    }
    $files = @(Get-ChildItem -LiteralPath $Path -File -Recurse)
    $stagingRootPath = [System.IO.Path]::GetFullPath($Path).TrimEnd([System.IO.Path]::DirectorySeparatorChar)
    $stagingPrefix = $stagingRootPath + [System.IO.Path]::DirectorySeparatorChar
    $relative = @($files | ForEach-Object {
        $fullName = [System.IO.Path]::GetFullPath($_.FullName)
        if (-not $fullName.StartsWith($stagingPrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
            throw "Staging file resolved outside staging root: $fullName"
        }
        $fullName.Substring($stagingPrefix.Length).Replace('\', '/')
    } | Sort-Object)
    $expected = @($ExpectedDll, $ExpectedManifest) | Sort-Object
    if ($relative.Count -ne 2 -or (Compare-Object -ReferenceObject $expected -DifferenceObject $relative)) {
        throw "Staging tree must contain exactly $ExpectedManifest and $ExpectedDll at its root; actual=$($relative -join ',')"
    }
    $manifestPath = Join-Path $Path $ExpectedManifest
    $manifest = Read-ManifestIdentity -ManifestPath $manifestPath
    if ($manifest.id -ne $ExpectedId) {
        throw "Staging manifest id mismatch: expected=$ExpectedId; actual=$($manifest.id)"
    }
    return [pscustomobject]@{
        Root = $Path
        ManifestPath = $manifestPath
        DllPath = Join-Path $Path $ExpectedDll
        Id = [string]$manifest.id
        Version = [string]$manifest.version
        ManifestSha256 = (Get-FileHash -Algorithm SHA256 -LiteralPath $manifestPath).Hash
        DllSha256 = (Get-FileHash -Algorithm SHA256 -LiteralPath (Join-Path $Path $ExpectedDll)).Hash
    }
}

function Assert-SingleIdentityState {
    param([object[]]$Records)
    $legacy = @($Records | Where-Object Id -eq $legacyModId)
    $target = @($Records | Where-Object Id -eq $modId)
    if ($legacy.Count -gt 1 -or $target.Count -gt 1 -or ($legacy.Count -gt 0 -and $target.Count -gt 0)) {
        throw "Duplicate identity state: legacy=$($legacy.Count); target=$($target.Count)"
    }
    foreach ($record in @($legacy + $target)) {
        if (-not $record.DllExists) {
            throw "Manifest $($record.ManifestPath) is missing its matching DLL: $($record.DllPath)"
        }
    }
    return [pscustomobject]@{ Legacy = $legacy; Target = $target }
}

function New-Ledger {
    param(
        [string]$Action,
        [object]$Staging,
        [string]$PreviousActiveBackup,
        [string]$TargetRecovery,
        [string]$RestoredBackupId)
    return [ordered]@{
        schemaVersion = 2
        action = $Action
        createdAtUtc = [DateTime]::UtcNow.ToString("o")
        legacyId = $legacyModId
        targetId = $modId
        stagingManifestSha256 = if ($null -eq $Staging) { $null } else { $Staging.ManifestSha256 }
        stagingDllSha256 = if ($null -eq $Staging) { $null } else { $Staging.DllSha256 }
        previousActiveBackupPath = $PreviousActiveBackup
        targetRecoveryPath = $TargetRecovery
        restoredBackupId = $RestoredBackupId
    }
}

$effectiveOperation = if ($Mode -eq "Plan") { $PlanOperation } else { $Mode }
$modsRoot = Assert-PathWithinRoot -Root $resolvedGameRoot -Candidate $modsRoot -Label "mods root"
$installPath = Assert-PathWithinRoot -Root $modsRoot -Candidate $installPath -Label "target install path"
$legacyInstallPath = Assert-PathWithinRoot -Root $modsRoot -Candidate $legacyInstallPath -Label "legacy install path"
$resolvedBackupRoot = Assert-PathWithinRoot -Root $resolvedGameRoot -Candidate $resolvedBackupRoot -Label "backup root"
$resolvedBackupRoot = Assert-PathOutsideRoot -Root $modsRoot -Candidate $resolvedBackupRoot -Label "backup root"
$legacyConfigPath = Assert-PathWithinRoot -Root $resolvedConfigRoot -Candidate $legacyConfigPath -Label "legacy config path"
$currentConfigPath = Assert-PathWithinRoot -Root $resolvedConfigRoot -Candidate $currentConfigPath -Label "current config path"
$resolvedConfigMigrationRoot = Assert-PathOutsideRoot -Root $resolvedConfigRoot -Candidate $resolvedConfigMigrationRoot -Label "config migration root"
$gameProcesses = @(Get-Process -Name "SlayTheSpire2" -ErrorAction SilentlyContinue)
$localRecords = @(Get-IdentityRecords -Root $modsRoot -Source "local" -Recursive)
$localState = Assert-SingleIdentityState -Records $localRecords
$orphanArtifacts = @(Get-OrphanIdentityArtifacts -Root $modsRoot -Records $localRecords)
if ($orphanArtifacts.Count -gt 0) {
    throw "Orphan legacy/target identity artifacts detected: $($orphanArtifacts -join ',')"
}
$workshopRecords = @()
if ($IncludeWorkshop) {
    if ([string]::IsNullOrWhiteSpace($WorkshopRoot)) {
        throw "-IncludeWorkshop requires an explicit -WorkshopRoot"
    }
    $resolvedWorkshopRoot = [System.IO.Path]::GetFullPath($WorkshopRoot)
    $workshopRecords = @(Get-IdentityRecords -Root $resolvedWorkshopRoot -Source "workshop" -Recursive)
}

$activeIdentity = if ($localState.Target.Count -eq 1) {
    Get-StagingIdentity -Path $localState.Target[0].Directory -ExpectedId $modId -ExpectedManifest $manifestName -ExpectedDll $dllName
} elseif ($localState.Legacy.Count -eq 1) {
    Get-StagingIdentity -Path $localState.Legacy[0].Directory -ExpectedId $legacyModId -ExpectedManifest $legacyManifestName -ExpectedDll $legacyDllName
} else { $null }

$staging = if ($effectiveOperation -eq "Install") {
    Get-StagingIdentity -Path $resolvedStagingPath -ExpectedId $modId -ExpectedManifest $manifestName -ExpectedDll $dllName
} else { $null }

$timestamp = if ([string]::IsNullOrWhiteSpace($TransactionId)) {
    [DateTime]::UtcNow.ToString("yyyyMMddTHHmmssfffZ")
} else {
    if ($TransactionId -notmatch '^[A-Za-z0-9._-]+$') {
        throw "TransactionId contains unsupported characters"
    }
    $TransactionId
}
$plannedLegacyBackup = Assert-PathWithinRoot -Root $resolvedBackupRoot -Candidate (Join-Path $resolvedBackupRoot "$timestamp-$legacyModId") -Label "legacy backup path"
$activeVersionToken = if ($null -eq $activeIdentity) { "none" } else {
    [regex]::Replace($activeIdentity.Version, '[^A-Za-z0-9._-]', '_')
}
$plannedActiveBackup = Assert-PathWithinRoot -Root $resolvedBackupRoot -Candidate (Join-Path $resolvedBackupRoot "$timestamp-$modId-$activeVersionToken") -Label "active target backup path"

$resolvedBackupPath = $null
$rollbackBackup = $null
if ($effectiveOperation -eq "Rollback") {
    if ([string]::IsNullOrWhiteSpace($BackupPath)) {
        throw "Rollback Plan/execute requires -BackupPath"
    }
    $candidateBackupPath = if ([System.IO.Path]::IsPathRooted($BackupPath)) { $BackupPath } else { Join-Path $resolvedBackupRoot $BackupPath }
    $resolvedBackupPath = Assert-PathWithinRoot -Root $resolvedBackupRoot -Candidate $candidateBackupPath -Label "rollback backup path"
    if (-not (Test-Path -LiteralPath $resolvedBackupPath -PathType Container)) {
        throw "Rollback backup not found: $resolvedBackupPath"
    }
    $backupRecords = @(Get-IdentityRecords -Root (Split-Path $resolvedBackupPath -Parent) -Source "backup" |
        Where-Object Directory -eq $resolvedBackupPath)
    if ($backupRecords.Count -ne 1) {
        throw "Rollback backup must contain exactly one supported identity manifest and DLL: $resolvedBackupPath"
    }
    $backupRecord = $backupRecords[0]
    $rollbackBackup = if ($backupRecord.Id -eq $modId) {
        Get-StagingIdentity -Path $resolvedBackupPath -ExpectedId $modId -ExpectedManifest $manifestName -ExpectedDll $dllName
    } else {
        Get-StagingIdentity -Path $resolvedBackupPath -ExpectedId $legacyModId -ExpectedManifest $legacyManifestName -ExpectedDll $legacyDllName
    }
    if ($localState.Legacy.Count -ne 0 -or $localState.Target.Count -ne 1) {
        throw "Rollback requires legacy=0 and target=1; actual legacy=$($localState.Legacy.Count), target=$($localState.Target.Count)"
    }
}

$planAction = if ($effectiveOperation -eq "Rollback") {
    if ($rollbackBackup.Id -eq $modId) { "rollback-target" } else { "rollback-legacy" }
} elseif ($localState.Legacy.Count -eq 1) {
    "upgrade"
} elseif ($localState.Target.Count -eq 1) {
    if ($activeIdentity.ManifestSha256 -eq $staging.ManifestSha256 -and $activeIdentity.DllSha256 -eq $staging.DllSha256) {
        "target-already-current"
    } else {
        "target-upgrade"
    }
} else {
    "clean-install"
}
$inspectConfigs = $effectiveOperation -eq "Rollback" -or $configRootWasExplicit
$legacyConfig = if ($inspectConfigs) { Get-StrictConfigSnapshot -Path $legacyConfigPath -Schema Legacy } else { $null }
$currentConfig = if ($inspectConfigs) { Get-StrictConfigSnapshot -Path $currentConfigPath -Schema Current } else { $null }
$configRollbackAction = if ($effectiveOperation -ne "Rollback") {
    "not-applicable"
} elseif ($null -eq $currentConfig -and $null -eq $legacyConfig) {
    "no-config"
} elseif ($null -eq $currentConfig -and $null -ne $legacyConfig) {
    "legacy-direct"
} elseif ($null -eq $currentConfig -or $null -eq $legacyConfig) {
    "blocked-missing-config"
} elseif ($legacyConfig.SemanticDigest -eq $currentConfig.SemanticDigest) {
    "legacy-direct-semantically-equal"
} else {
    "reverse-sync-required"
}
$plan = [ordered]@{
    schemaVersion = 2
    mode = $Mode
    operation = $effectiveOperation
    action = $planAction
    executeRequested = [bool]$Execute
    transactionId = $timestamp
    gameRunning = $gameProcesses.Count -gt 0
    modsRoot = $modsRoot
    stagingRoot = if ($null -eq $staging) { $null } else { $staging.Root }
    stagingVersion = if ($null -eq $staging) { $null } else { $staging.Version }
    stagingManifestSha256 = if ($null -eq $staging) { $null } else { $staging.ManifestSha256 }
    stagingDllSha256 = if ($null -eq $staging) { $null } else { $staging.DllSha256 }
    activeId = if ($null -eq $activeIdentity) { $null } else { $activeIdentity.Id }
    activeVersion = if ($null -eq $activeIdentity) { $null } else { $activeIdentity.Version }
    activeManifestSha256 = if ($null -eq $activeIdentity) { $null } else { $activeIdentity.ManifestSha256 }
    activeDllSha256 = if ($null -eq $activeIdentity) { $null } else { $activeIdentity.DllSha256 }
    legacyActiveCount = $localState.Legacy.Count
    targetActiveCount = $localState.Target.Count
    workshopScanned = [bool]$IncludeWorkshop
    workshopIdentityCount = $workshopRecords.Count
    orphanArtifactCount = $orphanArtifacts.Count
    loaderScanRoot = $modsRoot
    backupOutsideLoaderRoot = $true
    targetInstallPath = $installPath
    plannedLegacyBackupPath = if ($effectiveOperation -eq "Install" -and $localState.Legacy.Count -eq 1) { $plannedLegacyBackup } else { $null }
    plannedActiveBackupPath = if ($effectiveOperation -eq "Install" -and $localState.Target.Count -eq 1 -and $planAction -eq "target-upgrade") { $plannedActiveBackup } else { $null }
    rollbackBackupPath = $resolvedBackupPath
    rollbackBackupId = if ($null -eq $rollbackBackup) { $null } else { $rollbackBackup.Id }
    rollbackBackupVersion = if ($null -eq $rollbackBackup) { $null } else { $rollbackBackup.Version }
    rollbackBackupManifestSha256 = if ($null -eq $rollbackBackup) { $null } else { $rollbackBackup.ManifestSha256 }
    rollbackBackupDllSha256 = if ($null -eq $rollbackBackup) { $null } else { $rollbackBackup.DllSha256 }
    configRoot = $resolvedConfigRoot
    configMigrationRoot = $resolvedConfigMigrationRoot
    legacyConfigPath = $legacyConfigPath
    legacyConfigSha256 = if ($null -eq $legacyConfig) { $null } else { $legacyConfig.Sha256 }
    currentConfigPath = $currentConfigPath
    currentConfigSha256 = if ($null -eq $currentConfig) { $null } else { $currentConfig.Sha256 }
    configRollbackAction = $configRollbackAction
    operations = @(
        "verify-game-not-running",
        "verify-staging-active-and-backup-hashes",
        "verify-resolved-paths",
        "keep-backups-outside-loader-scan",
        "stage-under-mods-root",
        "move-previous-active-to-recoverable-backup",
        "activate-target-by-rename",
        "verify-one-active-identity-and-exact-hashes",
        "reverse-sync-current-config-before-rollback-when-required",
        "restore-previous-active-on-failure"
    )
}

if ($Mode -eq "Plan") {
    $plan | ConvertTo-Json -Depth 8
    return
}
if (-not $Execute) {
    throw "$Mode mode requires explicit -Execute after review of Plan output"
}
if ($gameProcesses.Count -gt 0) {
    throw "Slay the Spire 2 is running; refusing identity mutation"
}
if ($workshopRecords.Count -gt 0) {
    throw "Workshop identity copies were detected; refusing local mutation without a separately approved Workshop disposition"
}

if ($Mode -eq "Install") {
    if ([string]::IsNullOrWhiteSpace($ExpectedManifestSha256) -or [string]::IsNullOrWhiteSpace($ExpectedDllSha256)) {
        throw "Install execution requires the reviewed Plan staging SHA256 values"
    }
    if ($staging.ManifestSha256 -ne $ExpectedManifestSha256 -or $staging.DllSha256 -ne $ExpectedDllSha256) {
        throw "Staging SHA256 changed after Plan review"
    }
    if ($null -ne $activeIdentity) {
        if ([string]::IsNullOrWhiteSpace($ExpectedActiveManifestSha256) -or [string]::IsNullOrWhiteSpace($ExpectedActiveDllSha256)) {
            throw "Install execution with an active identity requires reviewed active SHA256 values"
        }
        if ($activeIdentity.ManifestSha256 -ne $ExpectedActiveManifestSha256 -or $activeIdentity.DllSha256 -ne $ExpectedActiveDllSha256) {
            throw "Active install SHA256 changed after Plan review"
        }
    }
    if ($planAction -eq "target-already-current") {
        throw "Target staging is already active; refusing a no-op replacement"
    }
}

if ($Mode -eq "Rollback") {
    if ($configRollbackAction -eq "blocked-missing-config") {
        throw "Rollback config preflight is blocked because the required old/new config state is incomplete"
    }
    if ([string]::IsNullOrWhiteSpace($ExpectedActiveManifestSha256) -or
        [string]::IsNullOrWhiteSpace($ExpectedActiveDllSha256) -or
        [string]::IsNullOrWhiteSpace($ExpectedBackupManifestSha256) -or
        [string]::IsNullOrWhiteSpace($ExpectedBackupDllSha256)) {
        throw "Rollback execution requires reviewed active and backup SHA256 values"
    }
    if ($activeIdentity.ManifestSha256 -ne $ExpectedActiveManifestSha256 -or $activeIdentity.DllSha256 -ne $ExpectedActiveDllSha256) {
        throw "Active install SHA256 changed after rollback Plan review"
    }
    if ($rollbackBackup.ManifestSha256 -ne $ExpectedBackupManifestSha256 -or $rollbackBackup.DllSha256 -ne $ExpectedBackupDllSha256) {
        throw "Rollback backup SHA256 changed after Plan review"
    }
}

New-Item -ItemType Directory -Force -Path $resolvedBackupRoot | Out-Null

if ($Mode -eq "Install") {
    $transactionPath = Assert-PathWithinRoot -Root $modsRoot -Candidate (Join-Path $modsRoot ".damage-forecast-staging-$timestamp") -Label "transaction path"
    $failedPath = Assert-PathWithinRoot -Root $resolvedBackupRoot -Candidate (Join-Path $resolvedBackupRoot "$timestamp-failed-target") -Label "failed target path"
    $previousBackupPath = if ($localState.Target.Count -eq 1) { $plannedActiveBackup } elseif ($localState.Legacy.Count -eq 1) { $plannedLegacyBackup } else { $null }
    $previousInstallPath = if ($localState.Target.Count -eq 1) { $installPath } elseif ($localState.Legacy.Count -eq 1) { $legacyInstallPath } else { $null }
    $previousMoved = $false
    $targetActivated = $false
    try {
        New-Item -ItemType Directory -Path $transactionPath | Out-Null
        Copy-Item -LiteralPath $staging.ManifestPath -Destination (Join-Path $transactionPath $manifestName)
        Copy-Item -LiteralPath $staging.DllPath -Destination (Join-Path $transactionPath $dllName)
        $stagedTransaction = Get-StagingIdentity -Path $transactionPath -ExpectedId $modId -ExpectedManifest $manifestName -ExpectedDll $dllName
        if ($stagedTransaction.ManifestSha256 -ne $staging.ManifestSha256 -or $stagedTransaction.DllSha256 -ne $staging.DllSha256) {
            throw "Staged transaction SHA256 differs from the reviewed staging tree"
        }
        if ($null -ne $previousInstallPath) {
            Move-Item -LiteralPath $previousInstallPath -Destination $previousBackupPath
            $previousMoved = $true
        }
        Move-Item -LiteralPath $transactionPath -Destination $installPath
        $targetActivated = $true
        $after = Assert-SingleIdentityState -Records @(Get-IdentityRecords -Root $modsRoot -Source "local" -Recursive)
        if ($after.Legacy.Count -ne 0 -or $after.Target.Count -ne 1) {
            throw "Post-install identity verification failed: legacy=$($after.Legacy.Count); target=$($after.Target.Count)"
        }
        $installed = Get-StagingIdentity -Path $installPath -ExpectedId $modId -ExpectedManifest $manifestName -ExpectedDll $dllName
        if ($installed.Version -ne $staging.Version -or $installed.ManifestSha256 -ne $staging.ManifestSha256 -or $installed.DllSha256 -ne $staging.DllSha256) {
            throw "Post-install version/hash verification failed"
        }
        $ledgerPath = Join-Path $resolvedBackupRoot "$timestamp-install-ledger.json"
        New-Ledger -Action $planAction -Staging $staging -PreviousActiveBackup $previousBackupPath -TargetRecovery $null -RestoredBackupId $null |
            ConvertTo-Json -Depth 6 | Set-Content -Encoding UTF8 -LiteralPath $ledgerPath
        Write-Host "Activated $modId $($staging.Version) at $installPath; previous=$previousBackupPath; ledger=$ledgerPath"
    }
    catch {
        if ($targetActivated -and (Test-Path -LiteralPath $installPath)) {
            Move-Item -LiteralPath $installPath -Destination $failedPath
        } elseif (Test-Path -LiteralPath $transactionPath) {
            Move-Item -LiteralPath $transactionPath -Destination $failedPath
        }
        if ($previousMoved -and (Test-Path -LiteralPath $previousBackupPath) -and -not (Test-Path -LiteralPath $previousInstallPath)) {
            Move-Item -LiteralPath $previousBackupPath -Destination $previousInstallPath
        }
        throw
    }
    return
}

$targetRecoveryPath = Assert-PathWithinRoot -Root $resolvedBackupRoot -Candidate (Join-Path $resolvedBackupRoot "$timestamp-target-before-rollback") -Label "target recovery path"
$restorePath = if ($rollbackBackup.Id -eq $modId) { $installPath } else { $legacyInstallPath }
$targetMoved = $false
$backupRestored = $false
try {
    $configReverseSync = if ($configRollbackAction -eq "reverse-sync-required") {
        Invoke-ConfigReverseSync -Legacy $legacyConfig -Current $currentConfig -Transaction $timestamp
    } else { $null }
    Move-Item -LiteralPath $localState.Target[0].Directory -Destination $targetRecoveryPath
    $targetMoved = $true
    Move-Item -LiteralPath $resolvedBackupPath -Destination $restorePath
    $backupRestored = $true
    $after = Assert-SingleIdentityState -Records @(Get-IdentityRecords -Root $modsRoot -Source "local" -Recursive)
    if ($rollbackBackup.Id -eq $modId) {
        if ($after.Legacy.Count -ne 0 -or $after.Target.Count -ne 1) {
            throw "Post-rollback identity verification failed: legacy=$($after.Legacy.Count); target=$($after.Target.Count)"
        }
        $restored = Get-StagingIdentity -Path $installPath -ExpectedId $modId -ExpectedManifest $manifestName -ExpectedDll $dllName
    } else {
        if ($after.Legacy.Count -ne 1 -or $after.Target.Count -ne 0) {
            throw "Post-rollback identity verification failed: legacy=$($after.Legacy.Count); target=$($after.Target.Count)"
        }
        $restored = Get-StagingIdentity -Path $legacyInstallPath -ExpectedId $legacyModId -ExpectedManifest $legacyManifestName -ExpectedDll $legacyDllName
    }
    if ($restored.Version -ne $rollbackBackup.Version -or $restored.ManifestSha256 -ne $rollbackBackup.ManifestSha256 -or $restored.DllSha256 -ne $rollbackBackup.DllSha256) {
        throw "Post-rollback version/hash verification failed"
    }
    $ledgerPath = Join-Path $resolvedBackupRoot "$timestamp-rollback-ledger.json"
    New-Ledger -Action $planAction -Staging $null -PreviousActiveBackup $resolvedBackupPath -TargetRecovery $targetRecoveryPath -RestoredBackupId $rollbackBackup.Id |
        ConvertTo-Json -Depth 6 | Set-Content -Encoding UTF8 -LiteralPath $ledgerPath
    Write-Host "Restored $($rollbackBackup.Id) $($rollbackBackup.Version) at $restorePath; target recovery=$targetRecoveryPath; ledger=$ledgerPath"
    if ($null -ne $configReverseSync) {
        Write-Host "Reverse-synced DamageForecast.cfg to STS2PartyWatch.cfg; backup=$($configReverseSync.backupPath)"
    }
}
catch {
    if ($backupRestored -and -not (Test-Path -LiteralPath $resolvedBackupPath) -and (Test-Path -LiteralPath $restorePath)) {
        Move-Item -LiteralPath $restorePath -Destination $resolvedBackupPath
    }
    if ($targetMoved -and -not (Test-Path -LiteralPath $installPath) -and (Test-Path -LiteralPath $targetRecoveryPath)) {
        Move-Item -LiteralPath $targetRecoveryPath -Destination $installPath
    }
    throw
}
