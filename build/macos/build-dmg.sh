#!/usr/bin/env bash
# Create a .dmg from a built SyncData.app (run on macOS).
#
# Usage: build/macos/build-dmg.sh <rid> [version]
set -euo pipefail

RID="${1:?usage: build/macos/build-dmg.sh <rid> [version]}"
VERSION="${2:-1.0.0}"

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
APP="$ROOT/artifacts/app/$RID/SyncData.app"
OUT="$ROOT/artifacts/packages/dmg/SyncData-$VERSION-$RID.dmg"

if [ ! -d "$APP" ]; then
  echo "No app bundle at $APP. Run: build/macos/build-app.sh $RID $VERSION" >&2
  exit 1
fi

mkdir -p "$(dirname "$OUT")"
rm -f "$OUT"

hdiutil create -volname "SyncData" -srcfolder "$APP" -ov -format UDZO "$OUT"
echo "Built $OUT"
