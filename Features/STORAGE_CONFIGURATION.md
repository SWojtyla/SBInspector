# Storage Configuration Feature

## Overview

SBInspector now supports two storage options for saving connection strings and message templates:
1. **Browser Local Storage** - Default for web deployments
2. **File System Storage** - Recommended for Tauri desktop applications

## Why Two Storage Options?

- **Browser Local Storage**: Perfect for web-based deployments where data is stored in the browser
- **File System Storage**: Ideal for desktop applications running with Tauri, providing persistent storage on the user's Desktop

## How to Use

### Accessing Storage Settings

When you first open SBInspector (before connecting to Service Bus), you'll see a "Storage Settings" panel on the right side of the screen.

### Changing Storage Type

1. Select your preferred storage type from the dropdown:
   - **Browser Local Storage (Web)**: Stores data in browser localStorage
   - **File System (Desktop - Recommended for Tauri)**: Stores data in files on your Desktop

2. Click the dropdown to change the setting
3. The application will show a success message
4. **Important**: Restart the application for changes to take full effect

### Storage Locations

#### Browser Local Storage
- Data is stored in the browser's localStorage
- Keys used:
  - `sbinspector_connections` - Saved Service Bus connections
  - `sbinspector_templates` - Saved message templates
- Data persists in the browser but is not accessible from other browsers or devices

#### File System Storage
- Data is saved to your Desktop in a folder called `SBInspector`
- Files created:
  - `connections.json` - Contains all saved connections
  - `templates.json` - Contains all message templates
- Files are stored in JSON format with pretty-printing for easy reading
- Location: `~/Desktop/SBInspector/` (or equivalent on Windows: `C:\Users\<username>\Desktop\SBInspector\`)

### Configuration Persistence

Your storage type preference is saved in:
- Linux/Mac: `~/.config/SBInspector/storage-config.json`
- Windows: `%APPDATA%\SBInspector\storage-config.json`

This means your storage preference is remembered even after restarting the application.

## Technical Implementation

### Architecture

The storage system follows clean architecture principles:

1. **Interface Layer** (`IStorageService`): Defines the contract for storage operations
2. **Implementation Layer**:
   - `LocalStorageService`: Browser-based storage using JavaScript interop
   - `FileStorageService`: File system-based storage using .NET file I/O
3. **Factory Pattern** (`StorageServiceFactory`): Selects the appropriate implementation based on configuration
4. **Configuration Service** (`StorageConfigurationService`): Manages storage type preference persistence

### Files Involved

- `Core/Domain/StorageType.cs` - Enum defining storage types
- `Core/Domain/StorageConfiguration.cs` - Configuration model
- `Core/Interfaces/IStorageService.cs` - Storage service interface
- `Infrastructure/Storage/LocalStorageService.cs` - Browser localStorage implementation
- `Infrastructure/Storage/FileStorageService.cs` - File system implementation
- `Infrastructure/Storage/StorageServiceFactory.cs` - Factory for creating storage instances
- `Application/Services/StorageConfigurationService.cs` - Configuration management
- `Presentation/Components/UI/StorageSettings.razor` - UI component for settings

### Dependency Injection

The storage service is registered in `Program.cs` using a factory function that:
1. Retrieves the current storage configuration
2. Creates the appropriate storage service implementation
3. Returns it as `IStorageService` for dependency injection

## Switching Between Storage Types

### From Browser Storage to File System

1. Select "File System" in the Storage Settings
2. Restart the application
3. Your existing connections and templates in browser storage will remain there
4. New connections and templates will be saved to the Desktop folder
5. To migrate data, manually copy it from browser storage (use browser dev tools) to the JSON files

### From File System to Browser Storage

1. Select "Browser Local Storage" in the Storage Settings
2. Restart the application
3. Your existing files on the Desktop will remain there
4. New connections and templates will be saved to browser localStorage
5. To migrate data, manually import the JSON files into localStorage (use browser dev tools)

## Best Practices

1. **For Tauri Desktop Apps**: Use File System storage for better data persistence and easier backup
2. **For Web Deployments**: Use Browser Local Storage for better browser integration
3. **Data Backup**: When using File System storage, you can easily backup the `~/Desktop/SBInspector/` folder
4. **Security**: Both storage types store connection strings. Ensure proper file permissions on Desktop folder for sensitive data

## Screenshots

### Storage Settings UI
![Storage Settings - Browser Storage](https://github.com/user-attachments/assets/6001636c-13d0-4eac-ab1d-6bdb86ef6bb4)

### File System Storage Selected
![Storage Settings - File System](https://github.com/user-attachments/assets/cc6a97d5-e5c8-4e19-80b0-d30e05622a52)

## Future Enhancements

Possible future improvements:
- Data migration wizard between storage types
- Custom storage path selection
- Cloud storage integration
- Encryption for sensitive data
- Export/import functionality
