# .NET 10 Upgrade Feature

## Overview
This document describes the .NET 10 upgrade feature implemented for SBInspector, migrating the entire solution from .NET 9 to .NET 10.

## Feature Date
November 20, 2025

## Problem Statement
The user attempted to migrate to .NET 10 themselves by updating NuGet packages, but the MAUI application would "stay on loading screen and crash." The task was to create a fresh .NET 10 MAUI project, compare it with the .NET 9 version, and identify the needed modifications beyond just package upgrades.

## Solution Approach

### 1. Analysis Phase
- Installed .NET 10 MAUI workload
- Created a fresh .NET 10 MAUI Blazor Hybrid app for comparison
- Identified key differences between .NET 9 and .NET 10 templates
- Documented breaking changes and deprecated APIs

### 2. Implementation Phase
- Created `global.json` to pin SDK to .NET 10.0.100
- Updated all project target frameworks from `net9.0` to `net10.0`
- Fixed critical bug in MAUI project's TargetFrameworks condition
- Updated all package versions to .NET 10 compatible versions
- Addressed deprecated API usage

### 3. Documentation Phase
- Created comprehensive migration guide (NET10_MIGRATION.md)
- Documented all changes, known issues, and rollback procedures
- Included references to breaking changes and new features

## Technical Changes

### Project Files Updated
1. **SEBInspector.Maui.csproj**
   - Target framework: `net9.0-windows10.0.19041.0` → `net10.0-windows10.0.19041.0`
   - Fixed TargetFrameworks condition (was appending to empty variable)
   - Added EnableWindowsTargeting for cross-platform builds
   - MAUI packages: Explicit version 10.0.0
   - Microsoft.Extensions.Logging.Debug: 9.0.8 → 10.0.0

2. **SBInspector.Shared.csproj**
   - Target framework: `net9.0` → `net10.0`
   - Microsoft.AspNetCore.Components.Web: 9.0.9 → 10.0.0
   - Microsoft.AspNetCore.DataProtection: 9.0.10 → 10.0.0

3. **SBInspector.csproj** (Blazor Server)
   - Target framework: `net9.0` → `net10.0`

4. **SBInspector.Tests.csproj**
   - Target framework: `net9.0` → `net10.0`

### Code Updates
- **MauiFileExportService.cs**: Replaced `DisplayAlert` with `DisplayAlertAsync` (deprecated API)

### Configuration Files
- **global.json**: Created to specify .NET 10.0.100 SDK with latestMinor rollForward

## Key Findings

### Root Cause of Original Issue
The loading screen crash was likely caused by:
1. **Missing MauiVersion variable**: The project used `$(MauiVersion)` but never defined it
2. **TargetFrameworks bug**: The condition appended to an empty variable, causing build failures
3. **Package mismatches**: Attempting to use .NET 9 packages with .NET 10 SDK

### Critical Fix: TargetFrameworks Condition
**Before (Broken):**
```xml
<TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">$(TargetFrameworks);net9.0-windows10.0.19041.0</TargetFrameworks>
```

**After (Fixed):**
```xml
<TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">net10.0-windows10.0.19041.0</TargetFrameworks>
<TargetFrameworks Condition="'$(TargetFrameworks)' == ''">net10.0-windows10.0.19041.0</TargetFrameworks>
<EnableWindowsTargeting>true</EnableWindowsTargeting>
```

### Comparison with Fresh .NET 10 Template
- **WindowsPackageType**: New template uses `None` (unpackaged), existing project uses `MSIX` (for Store)
- **Package versions**: New template uses explicit 10.0.0 versions, not variables
- **MauiProgram.cs**: No structural changes required
- **App initialization**: Pattern remains compatible

## Breaking Changes & Deprecations

### Deprecated in .NET 10 (Still Work)
- **MessagingCenter**: Made internal, use CommunityToolkit.Mvvm's WeakReferenceMessenger
- **ListView**: Use CollectionView instead
- **TableView**: Use CollectionView instead
- **DisplayAlert/DisplayActionSheet**: Use DisplayAlertAsync/DisplayActionSheetAsync

### New Features in .NET 10 MAUI
- XAML Source Generator (opt-in)
- HybridWebView with request interception
- Comprehensive diagnostics and metrics
- Improved SafeArea management
- MediaPicker enhancements

## Testing & Verification

### Verified Working ✅
- Blazor Server application builds successfully
- Blazor Server application runs without errors
- All projects compile without errors (except XAML on Linux)
- Shared libraries load correctly

### Requires Windows Testing ⚠️
- MAUI application cannot be fully tested on Linux (XAML compilation limitation)
- Final verification must be done on Windows platform
- App startup and loading screen behavior needs Windows validation

## How to Use

### Building the Project
```bash
# Ensure .NET 10 SDK is installed
dotnet --version  # Should show 10.0.100 or higher

# Build the entire solution
dotnet build

# Run Blazor Server app
cd SBInspector
dotnet run

# Build MAUI app (Windows only)
cd SEBInspector.Maui
dotnet build
```

### Rollback (if needed)
If rollback to .NET 9 is needed:
1. Modify or delete `global.json`
2. Revert all `net10.0` to `net9.0` in .csproj files
3. Revert package versions (see NET10_MIGRATION.md for details)

## Documentation
- **NET10_MIGRATION.md**: Comprehensive migration guide with all details
- **MICROSOFT_STORE_SUBMISSION_GUIDE.md**: Store submission remains compatible

## Future Considerations

### Optional Enhancements
1. Enable XAML Source Generator: Add `<MauiXamlInflator>SourceGen</MauiXamlInflator>`
2. Adopt full trimming: Set `<TrimMode>full</TrimMode>`
3. Consider Native AOT for iOS/Mac Catalyst
4. Replace remaining DisplayAlert usages in other components

### Known Limitations
- XAML compilation requires Windows platform
- MAUI app is Windows-only (by design)
- Some test failures exist (pre-existing, unrelated to .NET 10)

## References
- [.NET 10 Release Notes](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10/overview)
- [What's new in .NET MAUI 10](https://learn.microsoft.com/en-us/dotnet/maui/whats-new/dotnet-10)
- [Migration Guide](./NET10_MIGRATION.md)
