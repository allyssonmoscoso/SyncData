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
- **Upcoming Features:**
    - Differential synchronization to only copy changed files. 🛑
    - Compression support to reduce data transfer size. 🛑
    - Network synchronization to sync directories over a network. 🛑
    - Preserve file permissions and timestamps. 🛑

## Requirements

- .NET 8.0 SDK or later.
- .NET 8.0 Runtime.

## Parameters

- `-source=<path>`: Specifies the source directory path
- `-target=<path>`: Specifies the target directory path 
- `-v` or `-verbose`: Enables detailed output logging
- `-log-file`: Enables logging to file (syncData.log)
- `-exclude=path` or `-exclude=path1,path2`: Excludes specified paths from synchronization 

## Usage

To run the application, use one of the following commands:

### Using `dotnet run`

```sh
dotnet run --project SyncData/SyncData.csproj <source_directory> <target_directory> [-v | -verbose] [-log-file] [-exclude=<path1,path2>]
```

### Using the compiled executable

```sh
SyncData.exe <source_directory> <target_directory> [-v | -verbose] [-log-file] [-exclude=<path1,path2>]
```

Replace `<source_directory>` and `<target_directory>` with the paths of the directories you want to synchronize. Use `-v` or `-verbose` for verbose output, `-log-file` to log messages to a file, and `-exclude=<path1,path2>` to exclude specific files or directories from synchronization.

## Example

```sh
dotnet run --project SyncData/SyncData.csproj "C:\SourceDir" "C:\TargetDir" -v -log-file-exclude="C:\SourceDir\Temp,C:\SourceDir\Logs"
```

## License

This project is licensed under the GNU General Public License v3.0.