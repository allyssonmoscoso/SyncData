# SyncData

SyncData is a console application that synchronizes files and directories between two specified paths. It ensures that both directories have the same content by copying files and creating directories as needed.

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

## Example

```sh
dotnet run --project SyncData.Cli -- -source="/home/user/Source" -target="/home/user/Target" -v -log-file -exclude="node_modules,.git" -preserve
```

## License

This project is licensed under the GNU General Public License v3.0.