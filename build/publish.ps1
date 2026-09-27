# Publish SyncData.Gui as a self-contained single-file build for a given RID.
#
# Usage: pwsh build/publish.ps1 -Rid win-x64 [-Version 1.0.0]
param(
    [Parameter(Mandatory = $true)][string]$Rid,
    [string]$Version = "1.0.0"
)

$ErrorActionPreference = "Stop"

$Root = Split-Path -Parent $PSScriptRoot
$Out = Join-Path $Root "artifacts/publish/$Rid"

if (Test-Path $Out) { Remove-Item -Recurse -Force $Out }

dotnet publish (Join-Path $Root "SyncData.Gui/SyncData.Gui.csproj") `
    -c Release `
    -r $Rid `
    --self-contained true `
    -p:Version=$Version `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:DebugType=None `
    -p:DebugSymbols=false `
    -o $Out

Write-Host "Published $Rid to $Out"

# Portable archive
$PortableDir = Join-Path $Root "artifacts/packages/portable"
New-Item -ItemType Directory -Force -Path $PortableDir | Out-Null
$Base = "SyncData-$Version-$Rid"
$Archive = Join-Path $PortableDir "$Base.zip"
if (Test-Path $Archive) { Remove-Item -Force $Archive }
Compress-Archive -Path (Join-Path $Out "*") -DestinationPath $Archive
Write-Host "Portable archive: $Archive"
