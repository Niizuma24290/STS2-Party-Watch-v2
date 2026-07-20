param(
    [string]$Destination = "work/dependencies/BaseLib/3.3.4/BaseLib.dll"
)

$ErrorActionPreference = "Stop"

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$version = "3.3.4"
$downloadUrl = "https://github.com/Alchyr/BaseLib-StS2/releases/download/v$version/BaseLib.dll"
$expectedSha256 = "C593F14EAAB504FC1D31C89DA7C029116D269F65706D9612D6F71A048E504235"
$destinationPath = if ([System.IO.Path]::IsPathRooted($Destination)) {
    [System.IO.Path]::GetFullPath($Destination)
}
else {
    [System.IO.Path]::GetFullPath((Join-Path $repoRoot $Destination))
}

$destinationDirectory = Split-Path -Parent $destinationPath
$temporaryPath = "$destinationPath.download"

function Get-Sha256 {
    param([string]$Path)

    return (Get-FileHash -Algorithm SHA256 -LiteralPath $Path).Hash.ToUpperInvariant()
}

if (Test-Path -LiteralPath $destinationPath) {
    $currentHash = Get-Sha256 -Path $destinationPath
    if ($currentHash -ne $expectedSha256) {
        throw "Existing BaseLib dependency has an unexpected SHA256: $destinationPath`nExpected: $expectedSha256`nActual:   $currentHash"
    }

    Write-Host "BaseLib $version dependency already verified: $destinationPath"
    return
}

New-Item -ItemType Directory -Force -Path $destinationDirectory | Out-Null
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12

try {
    Invoke-WebRequest -UseBasicParsing -Uri $downloadUrl -OutFile $temporaryPath
    $downloadHash = Get-Sha256 -Path $temporaryPath
    if ($downloadHash -ne $expectedSha256) {
        throw "Downloaded BaseLib dependency failed SHA256 verification.`nExpected: $expectedSha256`nActual:   $downloadHash"
    }

    Move-Item -LiteralPath $temporaryPath -Destination $destinationPath
    Write-Host "Downloaded and verified BaseLib ${version}: $destinationPath"
}
finally {
    if (Test-Path -LiteralPath $temporaryPath) {
        Remove-Item -LiteralPath $temporaryPath -Force
    }
}
