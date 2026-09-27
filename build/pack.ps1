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

vpk pack `
    --packId SyncData `
    --packTitle SyncData `
    --packVersion $Version `
    --packAuthors "allyssonmoscoso" `
    --packDir $PublishDir `
    --mainExe SyncData.Gui `
    --icon (Join-Path $Root "SyncData.Gui/Assets/icon.ico") `
    --runtime $Rid `
    --outputDir $OutDir

Write-Host "Packaged $Rid into $OutDir"
