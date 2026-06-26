param(
    [string]$Configuration = "Release",
    [string]$Project = "src/STS2PartyWatchCode/STS2PartyWatchCode.csproj",
    [string]$PublishDir = "work/publish/sts2-party-watch-v2",
    [string]$Sts2GameRoot = "C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2",
    [switch]$SkipPublish
)

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$projectPath = Join-Path $repoRoot $Project
$publishPath = Join-Path $repoRoot $PublishDir
$dotnet = "C:\sts2\dotnet\dotnet.exe"
$modId = "sts2-party-watch-v2"
$manifestName = "$modId.json"
$dllName = "$modId.dll"
$modsRoot = Join-Path $Sts2GameRoot "mods"
$installPath = Join-Path $modsRoot $modId

if (-not (Test-Path -LiteralPath $dotnet)) {
    throw "Expected local dotnet at $dotnet"
}

if (-not (Test-Path -LiteralPath $projectPath)) {
    throw "Project not found: $projectPath"
}

if (-not $SkipPublish) {
    & $dotnet restore $projectPath
    & $dotnet build $projectPath -c $Configuration --no-restore
    & $dotnet publish $projectPath -c $Configuration --no-build -o $publishPath
}

$requiredFiles = @($manifestName, $dllName)
foreach ($file in $requiredFiles) {
    $path = Join-Path $publishPath $file
    if (-not (Test-Path -LiteralPath $path)) {
        throw "Publish output missing required file: $path"
    }
}

$forbidden = Get-ChildItem -LiteralPath $publishPath -File -Recurse |
    Where-Object { $_.Name.EndsWith(".deps.json", [System.StringComparison]::OrdinalIgnoreCase) -or $_.Extension -in @(".pdb", ".pck", ".log") }

if ($forbidden) {
    $names = ($forbidden | ForEach-Object FullName) -join [Environment]::NewLine
    throw "Publish output contains forbidden files:$([Environment]::NewLine)$names"
}

$resolvedModsRoot = [System.IO.Path]::GetFullPath($modsRoot)
$resolvedInstallPath = [System.IO.Path]::GetFullPath($installPath)
if (-not $resolvedInstallPath.StartsWith($resolvedModsRoot, [System.StringComparison]::OrdinalIgnoreCase)) {
    throw "Refusing to install outside mods root: $resolvedInstallPath"
}

New-Item -ItemType Directory -Force -Path $installPath | Out-Null
foreach ($file in $requiredFiles) {
    Copy-Item -LiteralPath (Join-Path $publishPath $file) -Destination (Join-Path $installPath $file) -Force
}

Write-Host "Installed $modId to $installPath"
