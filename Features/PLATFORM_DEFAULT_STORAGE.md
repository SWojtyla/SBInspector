# Platform-Specific Default Storage

## Overview

The application now automatically selects the appropriate storage backend based on the platform, eliminating the need for manual storage configuration.

## Platform Defaults

### Blazor Server Application
- **Default Storage**: Browser LocalStorage
- **Location**: Browser's localStorage API
- **Best For**: Web deployments where data is stored per browser session

### MAUI Desktop Application
- **Default Storage**: File System
- **Location**: Local disk in the user's AppData directory under `SBInspector` folder
- **Best For**: Desktop deployments where persistent file-based storage is preferred

## Technical Implementation

### Storage Configuration Service

The `StorageConfigurationService` now accepts a platform-specific default storage type in its constructor:

```csharp
public StorageConfigurationService(StorageType defaultStorageType)
{
    _defaultStorageType = defaultStorageType;
    // ... initialization
}
```

### Platform Registration

**Blazor Server** (`Program.cs`):
```csharp
builder.Services.AddSingleton<StorageConfigurationService>(sp => 
    new StorageConfigurationService(StorageType.LocalStorage));
```

**MAUI** (`MauiProgram.cs`):
```csharp
builder.Services.AddSingleton<StorageConfigurationService>(sp => 
    new StorageConfigurationService(StorageType.FileSystem));
```

## User Interface Changes

The storage selection UI component (`StorageSettings`) has been removed from the home page. Users no longer need to manually select their storage preference as the application automatically uses the optimal storage type for their platform.

## Benefits

1. **Simplified User Experience**: No configuration needed - works out of the box
2. **Platform Optimized**: Each platform uses the storage type best suited for its environment
3. **Reduced Complexity**: Eliminates potential user confusion about which storage type to select
4. **Maintains Flexibility**: The underlying infrastructure still supports both storage types, allowing for future enhancements if needed

## Migration from Previous Versions

Users who previously had a storage configuration file will continue to use their configured storage type. Only new installations or users without an existing configuration will use the platform defaults.

## Storage Locations

### Browser LocalStorage (Blazor Server)
- Data is stored in the browser's localStorage
- Persists across browser sessions
- Limited to approximately 5-10MB depending on browser
- Cleared when browser data is cleared

### File System (MAUI)
- Windows: `%APPDATA%\SBInspector\`
- Files are stored as JSON on disk
- No storage size limitations (other than available disk space)
- Persists until manually deleted
