# MudBlazor Migration

## Overview

This document tracks the migration from Bootstrap to MudBlazor UI framework. The migration is being done in phases to ensure stability and minimize disruption.

## Phase 1: Package Installation and Registration (COMPLETED)

### What Was Done

1. **Added MudBlazor NuGet Package** to all three projects:
   - `SBInspector.Shared` (version 8.13.0)
   - `SBInspector` (Blazor Server - version 8.13.0)
   - `SEBInspector.Maui` (MAUI Desktop - version 8.13.0)

2. **Registered MudBlazor Services**:
   - Added `builder.Services.AddMudServices();` to `SBInspector/Program.cs` (Blazor Server)
   - Added `builder.Services.AddMudServices();` to `SEBInspector.Maui/MauiProgram.cs` (MAUI)

3. **Added MudBlazor CSS and JavaScript References**:
   - Updated `SBInspector/Components/App.razor`
   - Updated `SBInspector.Shared/Presentation/Components/App.razor`
   - Added Roboto font: `<link href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap" rel="stylesheet" />`
   - Added MudBlazor CSS: `<link href="_content/MudBlazor/MudBlazor.min.css" rel="stylesheet" />`
   - Added MudBlazor JS: `<script src="_content/MudBlazor/MudBlazor.min.js"></script>`

4. **Added MudBlazor Using Statements**:
   - Updated `SBInspector.Shared/_Imports.razor`
   - Updated `SBInspector.Shared/Presentation/Components/_Imports.razor`
   - Added `@using MudBlazor`

5. **Resolved Naming Conflicts**:
   - Fixed `FilterOperator` ambiguity between `SBInspector.Shared.Core.Domain.FilterOperator` and `MudBlazor.FilterOperator`
   - Added explicit using alias in `MessageFilterPanel.razor`: `@using FilterOperator = SBInspector.Shared.Core.Domain.FilterOperator`

### Verification

- ✅ Both Blazor Server and MAUI projects build successfully
- ✅ Blazor Server application runs without errors
- ✅ MudBlazor CSS and JS files are loaded correctly
- ✅ No component functionality was changed (phase 1 requirement)

### Technical Notes

- **Bootstrap and Bootstrap Icons are still referenced** - They will be removed in later phases as components are migrated
- **No visual changes** - All UI components still use Bootstrap styling
- **MAUI package reference** was added manually to the `.csproj` file because the MAUI project only builds on Windows

## Next Phases (To Be Implemented)

### Phase 2: Core Layout Components
- Migrate main layout (MainLayout.razor)
- Migrate navigation components
- Add MudThemeProvider, MudDialogProvider, MudSnackbarProvider to App.razor

### Phase 3: Form Components
- Migrate input fields to MudTextField
- Migrate buttons to MudButton
- Migrate checkboxes to MudCheckBox
- Migrate select elements to MudSelect

### Phase 4: Data Display Components
- Migrate tables to MudTable
- Migrate cards to MudCard
- Migrate alerts to MudAlert

### Phase 5: Final Cleanup
- Remove Bootstrap CSS and JS references
- Remove Bootstrap-specific styling
- Update custom CSS to work with MudBlazor
- Remove Bootstrap Icons (if not needed)
- Final testing and validation

## Resources

- [MudBlazor Documentation](https://mudblazor.com/)
- [MudBlazor Components](https://mudblazor.com/components)
- [MudBlazor GitHub](https://github.com/MudBlazor/MudBlazor)
