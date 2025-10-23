# SyncData - Object-Oriented Architecture

## Overview
This document describes the object-oriented architecture improvements made to the SyncData application.

## Design Patterns Used

### 1. **Factory Pattern**
- `SynchronizerFactory`: Creates appropriate synchronizer instances based on configuration
- Allows easy extension for new synchronizer types (e.g., FtpSynchronizer, UnidirectionalSynchronizer)

### 2. **Strategy Pattern**
- `FileSynchronizer` (abstract base class) defines the strategy interface
- `BidirectionalSynchronizer` implements the concrete strategy
- Allows different synchronization strategies to be swapped at runtime

### 3. **Composite Pattern**
- `CompositeLogger` can aggregate multiple loggers
- Allows logging to multiple destinations simultaneously

### 4. **Template Method Pattern**
- `FileOperation` defines the structure for file operations
- Subclasses (`FileCopyOperation`, `DirectoryCreateOperation`) implement specific behaviors

## Architecture

### Namespace Structure

```
SyncData/
├── Configuration/          # Configuration and argument parsing
│   ├── AppConstants.cs    # Application-wide constants
│   ├── ArgumentParser.cs  # Command-line argument parsing
│   └── SyncConfiguration.cs # Configuration data model
├── Core/                   # Application orchestration
│   └── SyncApplication.cs # Main application coordinator
├── Logging/               # Logging abstraction
│   ├── Logger.cs          # Abstract logger base class
│   ├── ConsoleLogger.cs   # Console logging implementation
│   ├── FileLogger.cs      # File logging implementation
│   └── CompositeLogger.cs # Composite logger for multiple outputs
├── Synchronization/       # File synchronization logic
│   ├── IFileOperation.cs  # File operation interface
│   ├── FileOperation.cs   # Abstract base for file operations
│   ├── FileCopyOperation.cs # File copy implementation
│   ├── DirectoryCreateOperation.cs # Directory creation implementation
│   ├── FileSynchronizer.cs # Abstract synchronizer base class
│   ├── BidirectionalSynchronizer.cs # Bidirectional sync implementation
│   └── SynchronizerFactory.cs # Factory for creating synchronizers
├── Validation/            # Configuration validation
│   └── ConfigurationValidator.cs # Validates configuration
├── ProgressBar.cs         # Progress reporting
└── Program.cs            # Application entry point
```

## SOLID Principles Applied

### Single Responsibility Principle (SRP)
- Each class has one reason to change:
  - `ArgumentParser`: Only handles argument parsing
  - `ConfigurationValidator`: Only validates configuration
  - `FileCopyOperation`: Only handles file copying
  - `ConsoleLogger`: Only logs to console
  - `FileLogger`: Only logs to file

### Open/Closed Principle (OCP)
- Classes are open for extension but closed for modification:
  - New synchronizer types can be added by extending `FileSynchronizer`
  - New logger types can be added by extending `Logger`
  - New file operations can be added by implementing `IFileOperation`

### Liskov Substitution Principle (LSP)
- Derived classes can substitute their base classes:
  - Any `Logger` subclass can replace `Logger`
  - Any `FileSynchronizer` subclass can replace `FileSynchronizer`

### Interface Segregation Principle (ISP)
- Interfaces are specific and focused:
  - `IFileOperation`: Simple interface with single method
  - `IProgress<double>`: Standard .NET interface for progress reporting

### Dependency Inversion Principle (DIP)
- High-level modules don't depend on low-level modules:
  - `SyncApplication` depends on abstractions (`Logger`, `FileSynchronizer`)
  - Concrete implementations are created at runtime

## Key Classes and Their Responsibilities

### Configuration Layer
- **SyncConfiguration**: Data model for application settings
- **ArgumentParser**: Parses command-line arguments into configuration
- **AppConstants**: Centralizes application constants

### Validation Layer
- **ConfigurationValidator**: Validates configuration before execution

### Logging Layer
- **Logger (abstract)**: Defines logging contract
- **ConsoleLogger**: Logs to console with colors
- **FileLogger**: Logs to file
- **CompositeLogger**: Aggregates multiple loggers

### Synchronization Layer
- **IFileOperation**: Interface for file operations
- **FileOperation (abstract)**: Base implementation for operations
- **FileCopyOperation**: Handles file copying with optional attribute preservation
- **DirectoryCreateOperation**: Handles directory creation
- **FileSynchronizer (abstract)**: Base for synchronization strategies
- **BidirectionalSynchronizer**: Implements bidirectional sync
- **SynchronizerFactory**: Creates appropriate synchronizer instances

### Application Layer
- **SyncApplication**: Orchestrates the entire synchronization process
- **Program**: Entry point, creates dependencies and runs application

## Benefits of the New Architecture

1. **Maintainability**: Each class has a single, clear responsibility
2. **Testability**: Dependencies can be easily mocked/stubbed
3. **Extensibility**: New features can be added without modifying existing code
4. **Reusability**: Components can be reused in different contexts
5. **Readability**: Code is organized logically by responsibility
6. **Type Safety**: Strong typing with interfaces and abstract classes
7. **Separation of Concerns**: Clear boundaries between different parts of the system

## Extension Points

The architecture makes it easy to add:
- New synchronizer types (e.g., `UnidirectionalSynchronizer`, `FtpSynchronizer`)
- New logger types (e.g., `DatabaseLogger`, `CloudLogger`)
- New file operations (e.g., `FileDeleteOperation`, `FileCompressOperation`)
- New validation rules
- New configuration sources (e.g., config file, environment variables)
