# .NET 10 Migration Guide

## Overview

This document describes the migration of SBInspector from .NET 9 to .NET 10.

## Migration Date

November 20, 2025

## What Was Changed

### 1. SDK Version

Created `global.json` to specify .NET 10 SDK:
```json
{
  "sdk": {
    "version": "10.0.100",
    "rollForward": "latestMinor"
  }
}
```

### 2. Target Framework Updates

All projects updated from `net9.0` to `net10.0`:

- **SEBInspector.Maui**: `net9.0-windows10.0.19041.0` → `net10.0-windows10.0.19041.0`
- **SBInspector** (Blazor Server): `net9.0` → `net10.0`
- **SBInspector.Shared**: `net9.0` → `net10.0`
- **SBInspector.Tests**: `net9.0` → `net10.0`

### 3. Package Updates

#### MAUI Project (SEBInspector.Maui)
- `Microsoft.Maui.Controls`: `$(MauiVersion)` → `10.0.0`
- `Microsoft.AspNetCore.Components.WebView.Maui`: `$(MauiVersion)` → `10.0.0`
- `Microsoft.Extensions.Logging.Debug`: `9.0.8` → `10.0.0`
- `Azure.Messaging.ServiceBus`: `7.20.1` (unchanged)

#### Shared Project (SBInspector.Shared)
- `Microsoft.AspNetCore.Components.Web`: `9.0.9` → `10.0.0`
- `Microsoft.AspNetCore.DataProtection`: `9.0.10` → `10.0.0`
- `Azure.Messaging.ServiceBus`: `7.20.1` (unchanged)
- `MudBlazor`: `8.13.0` (unchanged)

### 4. Bug Fixes

#### TargetFrameworks Condition Fix
The original project had a bug where `TargetFrameworks` would append to an empty variable:
```xml
<!-- Before (broken) -->
<TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">$(TargetFrameworks);net9.0-windows10.0.19041.0</TargetFrameworks>
```

This was fixed to:
```xml
<!-- After (working) -->
<TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">net10.0-windows10.0.19041.0</TargetFrameworks>
<!-- For CI/non-Windows builds, set a default target framework -->
<TargetFrameworks Condition="'$(TargetFrameworks)' == ''">net10.0-windows10.0.19041.0</TargetFrameworks>
<!-- Enable Windows targeting on non-Windows platforms -->
<EnableWindowsTargeting>true</EnableWindowsTargeting>
```

## Key Differences from .NET 9 Template

### WindowsPackageType
- **.NET 10 default template**: `<WindowsPackageType>None</WindowsPackageType>` (unpackaged)
- **SBInspector**: `<WindowsPackageType>MSIX</WindowsPackageType>` (packaged for Microsoft Store)

**Note**: If experiencing startup issues (loading screen that doesn't progress), consider temporarily changing to `None` for debugging purposes. However, MSIX packaging is required for Microsoft Store submission.

## Comparison with Fresh .NET 10 MAUI Template

A fresh .NET 10 MAUI Blazor app was created for comparison. Key findings:

1. **No breaking changes in MauiProgram.cs structure** - the app initialization pattern remains the same
2. **Package versions** - MAUI 10.0.0 packages are used consistently
3. **WindowsPackageType** - new template defaults to unpackaged (`None`)
4. **TargetFrameworks** - new template uses comma-separated list, not append syntax

## Verification

### Blazor Server Application
✅ Successfully builds and runs on .NET 10
- Tested with `dotnet run` 
- Application starts without errors
- Listening on https://localhost:5000

### MAUI Application
⚠️ **Requires Windows for full testing**
- Cannot be fully compiled on Linux due to XAML compilation step
- Builds framework and dependencies successfully
- Final verification must be done on Windows platform

## Addressing the Loading Screen Issue

The problem statement mentioned the app "stays on loading screen and crashes" after upgrade. Based on the comparison with a fresh .NET 10 MAUI template, potential causes include:

1. **Package version mismatches** - Fixed by explicitly setting all MAUI packages to 10.0.0
2. **WindowsPackageType compatibility** - New template uses `None` instead of `MSIX`
3. **Missing EnableWindowsTargeting** - Added for cross-platform builds

### Recommended Testing Steps on Windows

1. Build the MAUI project: `dotnet build SEBInspector.Maui`
2. Run the application and observe startup behavior
3. If loading screen still hangs:
   - Check Windows Event Viewer for crash details
   - Try changing `WindowsPackageType` from `MSIX` to `None` temporarily
   - Verify all NuGet packages restored correctly
   - Check for any breaking changes in MAUI 10.0.0 release notes

## Known Issues

1. **Linux Build Limitation**: MAUI Windows apps cannot be fully compiled on Linux due to XAML compiler being Windows-specific
2. **Test Project Errors**: Pre-existing test errors in ConfirmationModalTests (unrelated to .NET 10 migration)

## Breaking Changes from .NET 9 to .NET 10

Based on comparison of fresh templates and Microsoft documentation:

### Deprecated APIs (Still Work but Generate Warnings)
- **MessagingCenter**: Made internal in .NET 10. Replace with `WeakReferenceMessenger` from CommunityToolkit.Mvvm
- **ListView**: Deprecated - use CollectionView instead
- **TableView**: Deprecated - use CollectionView instead
- **DisplayAlert/DisplayActionSheet**: Deprecated - use DisplayAlertAsync/DisplayActionSheetAsync instead

### Structural Changes
- No breaking changes in MAUI app structure
- No changes required to MauiProgram.cs initialization pattern
- Package version updates are straightforward
- The `$(MauiVersion)` variable usage was replaced with explicit version numbers

### New Features in .NET 10 MAUI
- **XAML Source Generator**: Improved build performance (opt-in with `<MauiXamlInflator>SourceGen</MauiXamlInflator>`)
- **HybridWebView**: Web request interception support
- **Diagnostics**: Comprehensive metrics tracking for layout performance
- **SafeArea Enhancements**: Improved SafeArea management across platforms
- **CollectionView/CarouselView**: New handlers are now default on iOS/Mac Catalyst

## Rollback Instructions

If needed to rollback to .NET 9:

1. Delete or modify `global.json` to specify .NET 9 SDK:
   ```json
   {"sdk": {"version": "9.0.307"}}
   ```

2. Revert all `net10.0` references to `net9.0` in .csproj files

3. Revert package versions:
   - MAUI packages: `10.0.0` → `9.0.111` (or use `$(MauiVersion)`)
   - Microsoft.Extensions.Logging.Debug: `10.0.0` → `9.0.8`
   - Microsoft.AspNetCore packages: `10.0.0` → `9.0.9` or `9.0.10`

## References

- [.NET 10 Release Notes](https://github.com/dotnet/core/tree/main/release-notes/10.0)
- [.NET MAUI 10.0.0 Release Notes](https://github.com/dotnet/maui/releases)
- [Microsoft Store Submission Guide](./MICROSOFT_STORE_SUBMISSION_GUIDE.md)
