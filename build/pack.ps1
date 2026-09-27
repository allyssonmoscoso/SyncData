# Package the published app with Velopack (vpk). Run on the target OS.
#
# Usage: pwsh build/pack.ps1 -Rid win-x64 [-Version 1.0.0]
param(
    [Parameter(Mandatory = $true)][string]$Rid,
    [string]$Version = "1.0.0"
)

$ErrorActionPreference = "Stop"

$Root = Split-Path -Parent $PSScriptRoot
$PublishDir = Join-Path $Root "artifacts/publish/$Rid"
$OutDir = Join-Path $Root "artifacts/packages/$Rid"

if (-not (Test-Path $PublishDir)) {
    throw "No publish output at $PublishDir. Run: pwsh build/publish.ps1 -Rid $Rid -Version $Version"
}

New-Item -ItemType Directory -Force -Path $OutDir | Out-Null

# Resolve vpk: prefer a global tool, otherwise use the local tool manifest.
if (Get-Command vpk -ErrorAction SilentlyContinue) {
    $VpkExe = "vpk"
    $VpkPrefix = @()
} else {
    $VpkExe = "dotnet"
    $VpkPrefix = @("tool", "run", "vpk")
}

& $VpkExe @VpkPrefix pack `
    --packId SyncData `
    --packTitle SyncData `
    --packVersion $Version `
    --packAuthors "allyssonmoscoso" `
    --packDir $PublishDir `
    --mainExe "SyncData.Gui.exe" `
    --icon (Join-Path $Root "SyncData.Gui/Assets/icon.ico") `
    --runtime $Rid `
    --outputDir $OutDir

if ($LASTEXITCODE -ne 0) {
    throw "vpk pack failed with exit code $LASTEXITCODE"
}

Write-Host "Packaged $Rid into $OutDir"
