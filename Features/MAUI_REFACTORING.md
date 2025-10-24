# MAUI Refactoring Documentation

## Overview

SBInspector has been refactored from a Blazor Server web application to a .NET MAUI Blazor Hybrid desktop application for Windows. This change enables the application to run as a native Windows desktop application while maintaining the same Blazor-based UI and business logic.

## What is .NET MAUI Blazor Hybrid?

.NET MAUI (Multi-platform App UI) Blazor Hybrid combines the best of both worlds:
- **Native Application**: Runs as a native Windows desktop app
- **Blazor UI**: Uses the same Blazor components, Razor syntax, and component model
- **File System Access**: Direct access to the device file system without browser limitations
- **Native APIs**: Access to platform-specific features and APIs

## Key Changes Made

### 1. Project Structure

**Before (Blazor Server):**
- SDK: `Microsoft.NET.Sdk.Web`
- Target Framework: `net9.0`
- Hosting: ASP.NET Core web server

**After (.NET MAUI):**
- SDK: `Microsoft.NET.Sdk.Razor`
- Target Framework: `net9.0-windows10.0.19041.0`
- Hosting: Native MAUI application

### 2. Application Entry Point

**Before:**
- `Program.cs` with `WebApplication.CreateBuilder()`
- ASP.NET Core middleware pipeline
- Blazor Server render mode

**After:**
- `MauiProgram.cs` with `MauiApp.CreateBuilder()`
- MAUI application lifecycle
- `BlazorWebView` component for Blazor hosting

### 3. Platform-Specific Code

Added platform-specific folder:
- `Platforms/Windows/` - Windows app package manifest

### 4. Resources

Added MAUI resource structure:
- `Resources/AppIcon/` - Application icon
- `Resources/Splash/` - Splash screen
- `Resources/Fonts/` - Custom fonts
- `Resources/Raw/` - Raw assets

### 5. Storage Implementation

**Before:**
- Storage configuration with choice between browser localStorage (via JSInterop) and file system
- Factory pattern to create appropriate storage service

**After:**
- File system storage by default (no browser limitations)
- Direct file system access without JSInterop
- Files saved to Desktop/SBInspector folder

### 6. UI Components

**No Changes Required:**
- All Razor components remain the same
- All CSS styles remain the same
- All Blazor component logic remains the same
- Removed `@rendermode` directives (not needed in MAUI)

### 7. Service Registration

**Before:**
```csharp
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
```

**After:**
```csharp
builder.Services.AddMauiBlazorWebView();
```

### 8. Business Logic and Infrastructure

**No Changes Required:**
- Service Bus integration (`ServiceBusService`)
- Domain models
- Application services
- All business logic remains unchanged

## Architecture Compatibility

The clean architecture of SBInspector made this refactoring seamless:

```
Presentation Layer (UI)
    ↓ (depends on)
Application Layer (Services)
    ↓ (depends on)
Domain Layer (Models, Interfaces)
    ↑ (implemented by)
Infrastructure Layer (Service Bus, Storage)
```

Only the **Presentation Layer hosting mechanism** changed:
- Before: Hosted in ASP.NET Core web server
- After: Hosted in MAUI BlazorWebView

All other layers remained completely unchanged!

## Benefits of MAUI

1. **Native Performance**: Runs as a native Windows application with better performance
2. **Offline Capability**: No web server required, runs fully offline
3. **File System Access**: Direct access to device storage without browser restrictions
4. **Distribution**: Can be distributed via Microsoft Store or direct installation
5. **No Browser Required**: Standalone application without browser dependencies

## Backward Compatibility

The refactoring maintains:
- ✅ All existing features
- ✅ All UI components and layouts
- ✅ All business logic and services
- ✅ All Azure Service Bus operations
- ✅ Connection and template storage
- ✅ Message filtering, sorting, and pagination

## Future Enhancements

Now that SBInspector is a MAUI Windows application, future possibilities include:
- Platform-specific features (notifications, system tray)
- Offline queue browsing with sync capability
- Biometric authentication for stored connections
- Additional platform support (Android, iOS, macOS) if needed

## Technical Notes

### Render Modes

Blazor Server used `@rendermode @(new InteractiveServerRenderMode(prerender: false))`. In MAUI, render modes are not needed because:
- Components are always interactive
- No prerendering concept in MAUI
- Direct component-to-UI binding

### JSInterop

- Browser-specific JavaScript (localStorage) is no longer used
- Platform-specific features available through MAUI APIs
- Native file system access replaces browser storage

### Build Targets

The project targets Windows only:
- **Windows**: Requires Windows SDK (Windows 10 version 1809+)

## Migration Summary

This was a **minimal-change refactoring** that converted the hosting model while preserving all application functionality. The well-designed clean architecture made this transition smooth with zero changes to business logic, domain models, or Azure Service Bus integration.

## Testing Recommendations

To test the MAUI Windows application:
1. Test on Windows 10 (version 1809+) or Windows 11
2. Verify file storage works correctly in Desktop/SBInspector folder
3. Test all Azure Service Bus operations
4. Verify UI components render correctly across different window sizes
5. Test application lifecycle (minimize, restore, close)

## Conclusion

The refactoring successfully transforms SBInspector from a web application to a native Windows desktop application while maintaining all existing functionality and code quality. The clean architecture principles followed in the original design made this transformation straightforward and low-risk.
