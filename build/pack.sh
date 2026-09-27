#!/usr/bin/env bash
# Package the published app with Velopack (vpk).
# Run on the target OS: it produces the native installer for that platform
# (Windows: Setup.exe + portable, macOS: .app/.dmg, Linux: AppImage).
#
# Usage: build/pack.sh <rid> [version]
set -euo pipefail

RID="${1:?usage: build/pack.sh <rid> [version]}"
VERSION="${2:-1.0.0}"

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PUBLISH_DIR="$ROOT/artifacts/publish/$RID"
OUT_DIR="$ROOT/artifacts/packages/$RID"

if [ ! -d "$PUBLISH_DIR" ]; then
  echo "No publish output at $PUBLISH_DIR. Run: build/publish.sh $RID $VERSION" >&2
  exit 1
fi

export PATH="$PATH:$HOME/.dotnet/tools"

case "$(uname -s)" in
  Linux)  ICON="$ROOT/SyncData.Gui/Assets/icon.png" ;;
  Darwin) ICON="$ROOT/build/macos/icon.icns" ;;
  *)      ICON="$ROOT/SyncData.Gui/Assets/icon.ico" ;;
esac

mkdir -p "$OUT_DIR"

vpk pack \
  --packId SyncData \
  --packTitle SyncData \
  --packVersion "$VERSION" \
  --packAuthors "allyssonmoscoso" \
  --packDir "$PUBLISH_DIR" \
  --mainExe SyncData.Gui \
  --icon "$ICON" \
  --runtime "$RID" \
  --outputDir "$OUT_DIR"

echo "Packaged $RID into $OUT_DIR"
