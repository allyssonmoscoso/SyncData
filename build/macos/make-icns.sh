#!/usr/bin/env bash
# Generate build/macos/icon.icns from the PNG icon.
# Requires macOS tools: sips + iconutil.
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
PNG="$ROOT/SyncData.Gui/Assets/icon.png"
ICONSET="$ROOT/build/macos/AppIcon.iconset"
OUT="$ROOT/build/macos/icon.icns"

if ! command -v iconutil >/dev/null 2>&1; then
  echo "iconutil not found; run this script on macOS." >&2
  exit 1
fi

rm -rf "$ICONSET"
mkdir -p "$ICONSET"

for size in 16 32 128 256 512; do
  sips -z "$size" "$size" "$PNG" --out "$ICONSET/icon_${size}x${size}.png" >/dev/null
  double=$((size * 2))
  sips -z "$double" "$double" "$PNG" --out "$ICONSET/icon_${size}x${size}@2x.png" >/dev/null
done

iconutil -c icns "$ICONSET" -o "$OUT"
rm -rf "$ICONSET"
echo "Wrote $OUT"
