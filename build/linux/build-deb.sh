#!/usr/bin/env bash
# Build a .deb package from a published linux-x64 build.
#
# Usage: build/linux/build-deb.sh [version] [arch]
#   version  Application version (default: 1.0.0)
#   arch     Debian architecture (default: amd64)
set -euo pipefail

VERSION="${1:-1.0.0}"
ARCH="${2:-amd64}"

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
PUBLISH_DIR="$ROOT/artifacts/publish/linux-x64"
PKG="syncdata_${VERSION}_${ARCH}"
STAGE="$ROOT/artifacts/deb/$PKG"
OUT_DIR="$ROOT/artifacts/packages/deb"

if [ ! -d "$PUBLISH_DIR" ]; then
  echo "No publish output at $PUBLISH_DIR. Run: build/publish.sh linux-x64 $VERSION" >&2
  exit 1
fi

rm -rf "$STAGE"
mkdir -p "$STAGE/DEBIAN" \
         "$STAGE/opt/syncdata" \
         "$STAGE/usr/bin" \
         "$STAGE/usr/share/applications" \
         "$STAGE/usr/share/icons/hicolor/512x512/apps"

cp -r "$PUBLISH_DIR/." "$STAGE/opt/syncdata/"
chmod +x "$STAGE/opt/syncdata/SyncData.Gui"
rm -f "$STAGE/opt/syncdata"/*.pdb
ln -s /opt/syncdata/SyncData.Gui "$STAGE/usr/bin/syncdata"

cp "$ROOT/build/linux/syncdata.desktop" "$STAGE/usr/share/applications/syncdata.desktop"
cp "$ROOT/SyncData.Gui/Assets/icon.png" "$STAGE/usr/share/icons/hicolor/512x512/apps/syncdata.png"

INSTALLED_SIZE="$(du -sk "$STAGE/opt" | cut -f1)"
cat > "$STAGE/DEBIAN/control" <<EOF
Package: syncdata
Version: $VERSION
Section: utils
Priority: optional
Architecture: $ARCH
Installed-Size: $INSTALLED_SIZE
Maintainer: allyssonmoscoso
Homepage: https://github.com/allyssonmoscoso/SyncData
Description: SyncData - bidirectional directory synchronization tool
 A cross-platform GUI and CLI to keep two directories in sync.
EOF

mkdir -p "$OUT_DIR"
dpkg-deb --build --root-owner-group "$STAGE" "$OUT_DIR/${PKG}.deb"
echo "Built $OUT_DIR/${PKG}.deb"
