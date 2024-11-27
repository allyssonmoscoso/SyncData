# SyncData

SyncData is a console application that synchronizes files and directories between two specified paths. It ensures that both directories have the same content by copying files and creating directories as needed.

## Features

- Synchronizes files from source to target directory and vice versa.
- Creates missing directories in both source and target directories.
- Displays progress of synchronization with a progress bar.
- Verbose mode for detailed logging.
- Option to log messages to a file.
- **Upcoming Features:**
    - Differential synchronization to only copy changed files.
    - Compression support to reduce data transfer size.
    - Network synchronization to sync directories over a network.
    - Exclude specific files or directories from synchronization.
    - Preserve file permissions and timestamps.

## Requirements

- .NET 8.0 SDK or later.
- .NET 8.0 Runtime.

## Usage

To run the application, use one of the following commands:

### Using `dotnet run`

```sh
dotnet run --project SyncData/SyncData.csproj <source_directory> <target_directory> [-v | -verbose] [-log-file]
```

### Using the compiled executable

```sh
SyncData.exe <source_directory> <target_directory> [-v | -verbose] [-log-file]
```

Replace `<source_directory>` and `<target_directory>` with the paths of the directories you want to synchronize. Use `-v` or `-verbose` for verbose output and `-log-file` to log messages to a file.

## Example

```sh
dotnet run --project SyncData/SyncData.csproj "C:\SourceDir" "C:\TargetDir" -v -log-file
```

## Project Structure

- `SyncData/Program.cs`: Main application logic for synchronizing directories and files.
- `SyncData/SyncData.csproj`: Project file containing configuration and dependencies.

## License

This project is licensed under the GNU General Public License v3.0.