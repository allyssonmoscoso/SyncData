#!/usr/bin/env bash
# Assemble a macOS .app bundle from a published osx-* build (run on macOS).
#
# Usage: build/macos/build-app.sh <rid> [version]
set -euo pipefail

RID="${1:?usage: build/macos/build-app.sh <rid> [version]}"
VERSION="${2:-1.0.0}"

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
PUBLISH_DIR="$ROOT/artifacts/publish/$RID"
APP="$ROOT/artifacts/app/$RID/SyncData.app"

if [ ! -d "$PUBLISH_DIR" ]; then
  echo "No publish output at $PUBLISH_DIR. Run: build/publish.sh $RID $VERSION" >&2
  exit 1
fi

if [ ! -f "$ROOT/build/macos/icon.icns" ]; then
  "$ROOT/build/macos/make-icns.sh"
fi

rm -rf "$APP"
mkdir -p "$APP/Contents/MacOS" "$APP/Contents/Resources"
cp -r "$PUBLISH_DIR/." "$APP/Contents/MacOS/"
cp "$ROOT/build/macos/Info.plist" "$APP/Contents/Info.plist"
cp "$ROOT/build/macos/icon.icns" "$APP/Contents/Resources/AppIcon.icns"
chmod +x "$APP/Contents/MacOS/SyncData.Gui"

echo "Built $APP"
