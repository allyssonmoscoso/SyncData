# SyncData

SyncData is a console application that synchronizes files and directories between two specified paths. It ensures that both directories have the same content by copying files and creating directories as needed.

## Features

- Synchronizes files from source to target directory and vice versa.
- Creates missing directories in both source and target directories.
- Displays progress of synchronization with a progress bar.

## Requirements

- .NET 8.0 SDK or later.
- .NET 8.0 Runtime.

## Usage

To run the application, use one of the following commands:

### Using `dotnet run`

```sh
dotnet run --project SyncData/SyncData.csproj <source_directory> <target_directory>
```

### Using the compiled executable

```sh
SyncData.exe <source_directory> <target_directory>
```

Replace `<source_directory>` and `<target_directory>` with the paths of the directories you want to synchronize.

## Example

```sh
dotnet run --project SyncData/SyncData.csproj "C:\SourceDir" "C:\TargetDir"
```

## Project Structure

- `SyncData/Program.cs`: Main application logic for synchronizing directories and files.
- `SyncData/SyncData.csproj`: Project file containing configuration and dependencies.

## License

This project is licensed under the GNU General Public License v3.0.