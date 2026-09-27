#!/usr/bin/env bash
# Publish SyncData.Gui as a self-contained single-file build for a given RID.
#
# Usage: build/publish.sh <rid> [version]
#   rid     Runtime identifier (linux-x64, linux-arm64, win-x64, osx-x64, osx-arm64)
#   version Application version (default: 1.0.0)
set -euo pipefail

RID="${1:?usage: build/publish.sh <rid> [version]}"
VERSION="${2:-1.0.0}"

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
OUT="$ROOT/artifacts/publish/$RID"

rm -rf "$OUT"
dotnet publish "$ROOT/SyncData.Gui/SyncData.Gui.csproj" \
  -c Release \
  -r "$RID" \
  --self-contained true \
  -p:Version="$VERSION" \
  -p:PublishSingleFile=true \
  -p:IncludeNativeLibrariesForSelfExtract=true \
  -p:DebugType=None \
  -p:DebugSymbols=false \
  -o "$OUT"

echo "Published $RID to $OUT"

# Portable archive
PORTABLE_DIR="$ROOT/artifacts/packages/portable"
mkdir -p "$PORTABLE_DIR"
BASE="SyncData-$VERSION-$RID"
if [[ "$RID" == win-* ]] && command -v zip >/dev/null 2>&1; then
  (cd "$(dirname "$OUT")" && zip -qr "$PORTABLE_DIR/$BASE.zip" "$(basename "$OUT")")
  echo "Portable archive: $PORTABLE_DIR/$BASE.zip"
else
  tar -C "$(dirname "$OUT")" -czf "$PORTABLE_DIR/$BASE.tar.gz" "$(basename "$OUT")"
  echo "Portable archive: $PORTABLE_DIR/$BASE.tar.gz"
fi
