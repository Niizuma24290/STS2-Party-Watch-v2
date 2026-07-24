param(
    [ValidateSet("all", "stable", "beta")]
    [string]$Target = "all",
    [string]$SnapshotRoot = "",
    [string]$StableReferenceRoot = "",
    [string]$BetaReferenceRoot = "",
    [string]$BaseLibReferencePath = ""
)

$ErrorActionPreference = "Stop"

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$dotnet = "C:\sts2\dotnet\dotnet.exe"
$projectPath = Join-Path $repoRoot "src/DamageForecast/DamageForecast.csproj"
$contractProjectPath = Join-Path $repoRoot "tests/DamageForecast.ContractTests/DamageForecast.ContractTests.csproj"
$baseLibBootstrap = Join-Path $PSScriptRoot "Restore-BaseLibDependency.ps1"
$scratchRoot = Join-Path $repoRoot "work/forecast-guardrails"
$results = @()

function Resolve-SnapshotReferenceRoot {
    param(
        [string]$Root,
        [string]$TargetName,
        [string]$ExpectedVersion
    )

    if (-not (Test-Path -LiteralPath $Root)) {
        throw "Snapshot root not found: $Root"
    }

    $snapshotMatches = @(foreach ($snapshot in Get-ChildItem -LiteralPath $Root -Directory) {
        $releaseInfoPath = Join-Path $snapshot.FullName "release_info.json"
        if (-not (Test-Path -LiteralPath $releaseInfoPath)) {
            continue
        }

        $releaseInfo = Get-Content -Raw -Encoding UTF8 -LiteralPath $releaseInfoPath | ConvertFrom-Json
        if ($releaseInfo.version -eq $ExpectedVersion) {
            Join-Path $snapshot.FullName "data_sts2_windows_x86_64"
        }
    })

    if ($snapshotMatches.Count -ne 1) {
        throw "Expected exactly one $TargetName snapshot for $ExpectedVersion under $Root; found $($snapshotMatches.Count)."
    }

    return $snapshotMatches[0]
}

function Invoke-DotnetStep {
    param(
        [string]$TargetName,
        [string]$Step,
        [string[]]$Arguments
    )

    $timer = [System.Diagnostics.Stopwatch]::StartNew()
    & $dotnet @Arguments | ForEach-Object { Write-Host $_ }
    $exitCode = $LASTEXITCODE
    $timer.Stop()
    Write-Host "QUALITY target=$TargetName step=$Step duration_ms=$($timer.ElapsedMilliseconds) exit_code=$exitCode"
    if ($exitCode -ne 0) {
        throw "$TargetName $Step failed with exit code $exitCode."
    }

    return $timer.ElapsedMilliseconds
}

function Test-ForbiddenPath {
    param([string]$Path)

    $normalized = $Path.Replace("\", "/").Trim()
    $extension = [System.IO.Path]::GetExtension($normalized)
    if ($extension -in @(".dll", ".pdb", ".pck", ".log", ".exe")) {
        return $true
    }

    return ($normalized -split "/") | Where-Object {
        $_ -in @("bin", "obj", "publish", "work", "uploader")
    } | Select-Object -First 1
}

if (-not (Test-Path -LiteralPath $dotnet)) {
    throw "Expected local dotnet at $dotnet"
}

foreach ($requiredPath in @($projectPath, $contractProjectPath, $baseLibBootstrap)) {
    if (-not (Test-Path -LiteralPath $requiredPath)) {
        throw "Required quality-gate input not found: $requiredPath"
    }
}

if ([string]::IsNullOrWhiteSpace($SnapshotRoot)) {
    $SnapshotRoot = Join-Path (Split-Path -Parent $repoRoot) "STS2-reference-snapshots"
}

if ([string]::IsNullOrWhiteSpace($BaseLibReferencePath)) {
    $BaseLibReferencePath = Join-Path $repoRoot "work/dependencies/BaseLib/3.3.4/BaseLib.dll"
}

& $baseLibBootstrap -Destination $BaseLibReferencePath

$targets = @()
if ($Target -in @("all", "stable")) {
    if ([string]::IsNullOrWhiteSpace($StableReferenceRoot)) {
        $StableReferenceRoot = Resolve-SnapshotReferenceRoot -Root $SnapshotRoot -TargetName "stable" -ExpectedVersion "v0.107.1"
    }
    $targets += [pscustomobject]@{ Name = "stable"; ReferenceRoot = $StableReferenceRoot }
}
if ($Target -in @("all", "beta")) {
    if ([string]::IsNullOrWhiteSpace($BetaReferenceRoot)) {
        $BetaReferenceRoot = Resolve-SnapshotReferenceRoot -Root $SnapshotRoot -TargetName "beta" -ExpectedVersion "v0.109.0"
    }
    $targets += [pscustomobject]@{ Name = "beta"; ReferenceRoot = $BetaReferenceRoot }
}

foreach ($targetInput in $targets) {
    foreach ($file in @("sts2.dll", "GodotSharp.dll", "0Harmony.dll")) {
        $reference = Join-Path $targetInput.ReferenceRoot $file
        if (-not (Test-Path -LiteralPath $reference)) {
            throw "$($targetInput.Name) reference root missing ${file}: $reference"
        }
    }

    $contractArtifactsPath = Join-Path $scratchRoot "contract-artifacts/$($targetInput.Name)/"
    $contractDuration = Invoke-DotnetStep -TargetName $targetInput.Name -Step "contract" -Arguments @(
        "run",
        "--project", $contractProjectPath,
        "-c", "Release",
        "--artifacts-path", $contractArtifactsPath,
        "-p:Sts2ReferenceRoot=$($targetInput.ReferenceRoot)",
        "-p:BaseLibReferencePath=$BaseLibReferencePath"
    )

    $buildOutput = Join-Path $scratchRoot "bin/$($targetInput.Name)/"
    $intermediateOutput = Join-Path $scratchRoot "obj/$($targetInput.Name)/"
    Invoke-DotnetStep -TargetName $targetInput.Name -Step "restore" -Arguments @(
        "restore", $projectPath,
        "-p:Sts2ReferenceRoot=$($targetInput.ReferenceRoot)",
        "-p:BaseLibReferencePath=$BaseLibReferencePath",
        "-p:OutputPath=$buildOutput",
        "-p:BaseIntermediateOutputPath=$intermediateOutput"
    ) | Out-Null
    $buildDuration = Invoke-DotnetStep -TargetName $targetInput.Name -Step "release-build" -Arguments @(
        "build", $projectPath,
        "-c", "Release",
        "--no-restore",
        "-p:Sts2ReferenceRoot=$($targetInput.ReferenceRoot)",
        "-p:BaseLibReferencePath=$BaseLibReferencePath",
        "-p:OutputPath=$buildOutput",
        "-p:BaseIntermediateOutputPath=$intermediateOutput"
    )

    $results += [pscustomobject]@{
        Target = $targetInput.Name
        ContractDurationMs = $contractDuration
        BuildDurationMs = $buildDuration
    }
}

& git -C $repoRoot diff --check
if ($LASTEXITCODE -ne 0) {
    throw "git diff --check failed with exit code $LASTEXITCODE."
}
Write-Host "QUALITY step=git-diff-check exit_code=0"

$trackedForbidden = @(& git -C $repoRoot ls-files) | Where-Object { Test-ForbiddenPath $_ }
if ($LASTEXITCODE -ne 0) {
    throw "git ls-files failed with exit code $LASTEXITCODE."
}

$statusLines = @(& git -C $repoRoot status --short --untracked-files=all)
if ($LASTEXITCODE -ne 0) {
    throw "git status failed with exit code $LASTEXITCODE."
}
$workingForbidden = $statusLines |
    ForEach-Object { if ($_.Length -gt 3) { $_.Substring(3).Trim('"') } } |
    Where-Object { $_ -and (Test-ForbiddenPath $_) }

if ($trackedForbidden -or $workingForbidden) {
    $forbidden = @($trackedForbidden) + @($workingForbidden) | Sort-Object -Unique
    throw "Forbidden tracked/working-tree artifacts detected:`n$($forbidden -join [Environment]::NewLine)"
}
Write-Host "QUALITY step=artifact-review tracked_forbidden=0 working_forbidden=0 exit_code=0"

foreach ($result in $results) {
    Write-Host "QUALITY_RESULT target=$($result.Target) contract_ms=$($result.ContractDurationMs) build_ms=$($result.BuildDurationMs) exit_code=0"
}
Write-Host "QUALITY_GATE targets=$($results.Count) status=PASS exit_code=0"
