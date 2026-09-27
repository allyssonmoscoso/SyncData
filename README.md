# SyncData

[![CI](https://github.com/allyssonmoscoso/SyncData/actions/workflows/ci.yml/badge.svg)](https://github.com/allyssonmoscoso/SyncData/actions/workflows/ci.yml)
[![Release](https://github.com/allyssonmoscoso/SyncData/actions/workflows/release.yml/badge.svg)](https://github.com/allyssonmoscoso/SyncData/actions/workflows/release.yml)

SyncData is a cross-platform tool (desktop GUI + console) that synchronizes files and directories between two specified paths. It ensures that both directories have the same content by copying files and creating directories as needed.

## Features

|||
| -------- | ------- |
|  Added | ✅    |
| Partially added | ⚠️    |
| Not yet implemented    | 🛑   |

- Synchronizes files from source to target directory and vice versa. ✅
- Creates missing directories in both source and target directories. ✅
- Displays progress of synchronization with a progress bar. ✅
- Verbose mode for detailed logging. ✅
- Option to log messages to a file. ✅
- Exclude specific files or directories from synchronization. ✅
- Preserve file permissions and timestamps. ✅
- Auto-update: installed builds check GitHub Releases and offer to update (Windows/macOS). ⚠️
- Bilingual UI: English (default) and Spanish, switchable at runtime and remembered. ✅
- **Upcoming Features:**
    - Differential synchronization to only copy changed files. 🛑
    - Compression support to reduce data transfer size. 🛑
    - Network synchronization to sync directories over a network. 🛑

## Requirements

- .NET 8.0 SDK or later.
- .NET 8.0 Runtime.

## Parameters

- `-source=<path>`: Specifies the source directory path
- `-target=<path>`: Specifies the target directory path 
- `-v` or `-verbose`: Enables detailed output logging
- `-log-file`: Enables logging to file (syncData.log)
- `-exclude=path` or `-exclude=path1,path2`: Excludes specified paths from synchronization
-  `-preserve`: Preserve file permissions and timestamps.

## Project structure

- `SyncData.Core` — library with the shared synchronization logic (UI-agnostic).
- `SyncData.Cli` — console front-end.
- `SyncData.Gui` — desktop front-end built with [Avalonia UI](https://avaloniaui.net/) (Linux, Windows and macOS).
- `SyncData.Tests` — xUnit test suite for `SyncData.Core`.

## Languages

The GUI ships in **English (default)** and **Spanish**. Pick the language from the
selector in the window footer; the change applies instantly and is saved to
`settings.json` in the user config folder (`~/.config`, `%APPDATA%` or
`~/Library/Application Support`).

Messages are also localized inside the core library (validation and log output),
so the log panel follows the selected language too.

## Usage

### Graphical interface (Avalonia)

```sh
dotnet run --project SyncData.Gui
```

Pick the source and target folders, choose the options and press **Sincronizar**. You can cancel a running synchronization with **Cancelar**.

### Console

```sh
dotnet run --project SyncData.Cli -- -source=<source_directory> -target=<target_directory> [-v | -verbose] [-log-file] [-exclude=<path1,path2>] [-preserve]
```

### Using the compiled executable

```sh
SyncData.Cli -source=<source_directory> -target=<target_directory> [-v | -verbose] [-log-file] [-exclude=<path1,path2>] [-preserve]
```

Replace `<source_directory>` and `<target_directory>` with the paths of the directories you want to synchronize. Use `-v` or `-verbose` for verbose output, `-log-file` to log messages to a file, `-exclude=<path1,path2>` to exclude specific files or directories from synchronization, and `-preserve` to maintain file permissions and timestamps during synchronization.

## Tests

```sh
dotnet test SyncData.sln
```

## Distribution / packaging

Requirements: the .NET 8 SDK, the Velopack CLI (`dotnet tool restore`, pinned in
`.config/dotnet-tools.json`) and, on Linux, `dpkg-deb`.

```sh
# 1) Publish a self-contained single-file build and create the portable archive
./build/publish.sh linux-x64 1.0.0      # or win-x64, osx-arm64, osx-x64, linux-arm64

# 2) Native installer with Velopack (run on the target OS)
./build/pack.sh linux-x64 1.0.0         # Linux: AppImage · Windows: Setup.exe · macOS: .pkg (+ .app)

# 3) (Linux) .deb package
./build/linux/build-deb.sh 1.0.0

# macOS (optional): also produce a manual .app bundle and .dmg
./build/macos/make-icns.sh
./build/macos/build-app.sh osx-arm64 1.0.0
./build/macos/build-dmg.sh osx-arm64 1.0.0
```

On Windows use `build/publish.ps1` and `build/pack.ps1`.

Artifacts are written to `artifacts/` (git-ignored):
- `artifacts/packages/portable/` — `.zip` (Windows) / `.tar.gz` (Linux/macOS)
- `artifacts/packages/<rid>/` — Velopack output (AppImage / Setup.exe / `.pkg`+`.app` + update feed)
- `artifacts/packages/deb/` — Linux `.deb`

> Note: Windows `Setup.exe` and macOS `.dmg` must be built on their own OS (run the
> scripts there). Code signing and Apple notarization are out of scope for now.

## Auto-update

Installed builds (via Velopack) check the latest GitHub Release on startup and
show a banner with an **Actualizar** button when a newer version is available.
This relies on the update feed published by the release workflow
(`RELEASES-*`, `releases.*.json`, `.nupkg`).

- Works on **Windows and macOS** installed builds; portable builds and the Linux
  `.deb`/AppImage do not self-update.
- On macOS, updates require the app to be signed/notarized (planned for a later
  phase).
- Architecture: `IUpdateService` (abstraction) + `VelopackUpdateService`
  (Velopack `UpdateManager` over `GithubSource`) + `UpdateViewModel`.

## CI / Releases

- **CI** (`.github/workflows/ci.yml`): on every push/PR to `main`/`develop` it builds and runs the test suite on Linux.
- **Release** (`.github/workflows/release.yml`): triggered by a `v*` tag or manually (`workflow_dispatch`). It packages the app on Linux, Windows and macOS with Velopack and publishes a GitHub Release with the artifacts.

To cut a release:

```sh
git tag v1.0.0
git push origin v1.0.0
```

## Example

```sh
dotnet run --project SyncData.Cli -- -source="/home/user/Source" -target="/home/user/Target" -v -log-file -exclude="node_modules,.git" -preserve
```

## License

This project is licensed under the GNU General Public License v3.0.