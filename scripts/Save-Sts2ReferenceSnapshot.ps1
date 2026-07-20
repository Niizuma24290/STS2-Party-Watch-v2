param(
    [string]$GameRoot = "C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2",
    [string]$ReferenceRoot = "",
    [string]$ReleaseInfoPath = "",
    [string]$SnapshotRoot = "C:\Users\ROG\Documents\Codex\STS2-reference-snapshots",
    [string]$SnapshotName = "",
    [string]$Version = "",
    [string]$Branch = "",
    [string]$Commit = "",
    [string]$ReleaseDate = ""
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($ReferenceRoot)) {
    $managedDir = Join-Path $GameRoot "data_sts2_windows_x86_64"
}
else {
    $managedDir = $ReferenceRoot
}

if ([string]::IsNullOrWhiteSpace($ReleaseInfoPath) -and [string]::IsNullOrWhiteSpace($ReferenceRoot)) {
    $ReleaseInfoPath = Join-Path $GameRoot "release_info.json"
}

if (-not (Test-Path -LiteralPath $managedDir)) {
    throw "Managed directory not found: $managedDir"
}

if (-not [string]::IsNullOrWhiteSpace($ReleaseInfoPath) -and -not (Test-Path -LiteralPath $ReleaseInfoPath)) {
    throw "release_info.json not found: $ReleaseInfoPath"
}

if (-not [string]::IsNullOrWhiteSpace($ReleaseInfoPath)) {
    $releaseInfo = Get-Content -Raw -Encoding UTF8 -LiteralPath $ReleaseInfoPath | ConvertFrom-Json
}
else {
    if ([string]::IsNullOrWhiteSpace($Version) -or [string]::IsNullOrWhiteSpace($Branch) -or [string]::IsNullOrWhiteSpace($Commit)) {
        throw "Provide -ReleaseInfoPath or provide -Version, -Branch, and -Commit."
    }

    $releaseInfo = [pscustomobject]@{
        version = $Version
        branch = $Branch
        commit = $Commit
        date = $ReleaseDate
    }
}

if ([string]::IsNullOrWhiteSpace($SnapshotName)) {
    $safeVersion = ($releaseInfo.version -replace "[^A-Za-z0-9._-]", "-")
    $safeBranch = ($releaseInfo.branch -replace "[^A-Za-z0-9._-]", "-")
    $safeCommit = ($releaseInfo.commit -replace "[^A-Za-z0-9._-]", "-")
    $SnapshotName = "$safeVersion-$safeBranch-$safeCommit"
}

$snapshotPath = Join-Path $SnapshotRoot $SnapshotName
$snapshotManagedDir = Join-Path $snapshotPath "data_sts2_windows_x86_64"
New-Item -ItemType Directory -Force -Path $snapshotManagedDir | Out-Null

$releaseInfoDestination = Join-Path $snapshotPath "release_info.json"
if (-not [string]::IsNullOrWhiteSpace($ReleaseInfoPath)) {
    Copy-Item -LiteralPath $ReleaseInfoPath -Destination $releaseInfoDestination -Force
}
else {
    $releaseInfo | ConvertTo-Json -Depth 5 | Set-Content -Encoding UTF8 -LiteralPath $releaseInfoDestination
}

$files = @(
    @{ Source = $releaseInfoDestination; Destination = $releaseInfoDestination },
    @{ Source = (Join-Path $managedDir "sts2.dll"); Destination = (Join-Path $snapshotManagedDir "sts2.dll") },
    @{ Source = (Join-Path $managedDir "sts2.xml"); Destination = (Join-Path $snapshotManagedDir "sts2.xml") },
    @{ Source = (Join-Path $managedDir "GodotSharp.dll"); Destination = (Join-Path $snapshotManagedDir "GodotSharp.dll") },
    @{ Source = (Join-Path $managedDir "0Harmony.dll"); Destination = (Join-Path $snapshotManagedDir "0Harmony.dll") }
)

foreach ($file in $files) {
    if (-not (Test-Path -LiteralPath $file.Source)) {
        throw "Required snapshot source missing: $($file.Source)"
    }

    if ($file.Source -ne $file.Destination) {
        Copy-Item -LiteralPath $file.Source -Destination $file.Destination -Force
    }
}

$hashRows = foreach ($file in $files) {
    $hash = Get-FileHash -Algorithm SHA256 -LiteralPath $file.Destination
    [pscustomobject]@{
        Path = Resolve-Path -Relative -LiteralPath $file.Destination
        SHA256 = $hash.Hash
        Length = (Get-Item -LiteralPath $file.Destination).Length
    }
}

$manifest = [pscustomobject]@{
    captured_at = (Get-Date).ToString("o")
    source_game_root = $GameRoot
    source_reference_root = $managedDir
    reference_root = $snapshotManagedDir
    version = $releaseInfo.version
    branch = $releaseInfo.branch
    commit = $releaseInfo.commit
    files = $hashRows
}

$manifest | ConvertTo-Json -Depth 5 | Set-Content -Encoding UTF8 -LiteralPath (Join-Path $snapshotPath "manifest.json")
$hashRows |
    ForEach-Object { "$($_.SHA256)  $($_.Path)" } |
    Set-Content -Encoding ASCII -LiteralPath (Join-Path $snapshotPath "SHA256SUMS.txt")

Write-Host "Saved STS2 reference snapshot to $snapshotPath"
Write-Host "Reference root: $snapshotManagedDir"
