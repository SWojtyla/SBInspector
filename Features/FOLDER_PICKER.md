# Folder Picker Feature

## Overview

This feature adds convenient folder picker buttons to the Settings page, allowing users to browse and select folder paths directly instead of typing them manually.

## What Was Added

### 1. User Interface Changes

Three folder picker buttons were added to the Settings page (`Settings.razor`), one for each path configuration:

- **Export Path** - For exported messages location
- **Template Path** - For message templates location  
- **Connections Path** - For saved connection strings location

Each section now has two buttons:
- **Browse** button (with folder icon) - Opens the folder picker dialog
- **Reset** button - Clears the custom path to use the default

### 2. Architecture

Following the clean architecture pattern already established in the project:

#### Interface Layer (`SBInspector.Shared/Core/Interfaces/`)
- `IFolderPickerService.cs` - Interface defining the folder picker contract
  - Single method: `Task<string?> PickFolderAsync()`

#### Implementation Layer

**Blazor Server** (`SBInspector/Services/`)
- `BlazorFolderPickerService.cs` - Web implementation using JavaScript interop
  - Uses the File System Access API (modern browser feature)
  - Falls back gracefully if API is not supported

**MAUI** (`SEBInspector.Maui/Services/`)
- `MauiFolderPickerService.cs` - Desktop implementation for Windows
  - Uses Windows.Storage.Pickers.FolderPicker
  - Platform-specific implementation with conditional compilation

### 3. JavaScript Enhancement

Added to `app.js`:
```javascript
window.pickFolder = async function () {
    // Uses File System Access API
    const directoryHandle = await window.showDirectoryPicker();
    return directoryHandle.name;
}
```

### 4. Service Registration

Both `Program.cs` files updated to register the appropriate implementation:
- **Blazor Server**: Registers `BlazorFolderPickerService` as scoped service
- **MAUI**: Registers `MauiFolderPickerService` as singleton service

## How It Works

### For Blazor Server (Web)

1. User clicks the "Browse" button
2. Service calls JavaScript `pickFolder()` function
3. Browser shows native directory picker dialog (if supported)
4. Selected folder name is returned and populated in the text input
5. User can save the settings

**Note**: Due to browser security restrictions, the web version can only get the folder name, not the full path. This is a browser limitation of the File System Access API.

### For MAUI (Windows Desktop)

1. User clicks the "Browse" button
2. Service uses Windows.Storage.Pickers.FolderPicker
3. Native Windows folder picker dialog appears
4. Full folder path is returned and populated in the text input
5. User can save the settings

## Browser Support

The File System Access API used in the web version is supported in:
- Chrome/Edge 86+
- Opera 72+
- Safari 15.2+ (partial support)

Firefox does not currently support this API. The feature degrades gracefully - the button will simply do nothing if the API is not available.

## UI Preview

The Settings page now looks like this:

```
┌─────────────────────────────────────────────────────────┐
│ Export Path                                             │
│ Exported Messages Location                              │
│ ┌───────────────────────┬─────────┬──────────┐         │
│ │ [text input field]    │ Browse  │  Reset   │         │
│ └───────────────────────┴─────────┴──────────┘         │
│ Path where exported messages will be saved...           │
└─────────────────────────────────────────────────────────┘
```

Each of the three path sections (Export, Template, Connections) follows the same pattern.

## Benefits

1. **Improved UX** - No need to manually type folder paths
2. **Reduced Errors** - Less chance of typos in paths
3. **Cross-Platform** - Works in both web and desktop versions
4. **Clean Architecture** - Follows established patterns in the codebase
5. **Graceful Degradation** - Feature works when supported, doesn't break when not

## Technical Implementation Details

- **Language**: C# 12 with .NET 9.0
- **UI Framework**: Blazor with Bootstrap 5 icons
- **Dependency Injection**: Uses built-in DI container
- **Platform Detection**: Conditional compilation for MAUI
- **Error Handling**: Try-catch blocks with null returns on failure

## Future Enhancements

Possible improvements for future versions:
1. Add validation to ensure selected path exists and is writable
2. Show more helpful error messages if folder picker fails
3. Add recent folders dropdown for quick selection
4. Add "Open Folder" button to launch file explorer at the configured path
