# MudBlazor UI Refactoring

## Overview
The SBInspector UI has been refactored to use MudBlazor instead of Bootstrap, with dark mode support and a cleaner layout without a left sidebar.

## Changes Made

### 1. MudBlazor Integration
- **Package Added**: MudBlazor v8.13.0
- **Configuration**: Added `builder.Services.AddMudServices()` in `Program.cs`
- **Resources**: Updated `App.razor` to include MudBlazor CSS, fonts, and JavaScript
- **Imports**: Added `@using MudBlazor` to `_Imports.razor`

### 2. Layout Refactoring

#### MainLayout.razor
Completely redesigned the main layout:
- **Removed**: Left sidebar with `NavMenu` component
- **Added**: MudBlazor `MudAppBar` with top navigation
- **Components**:
  - `MudThemeProvider` - Manages theme and dark mode
  - `MudAppBar` - Top navigation bar
  - `MudIconButton` - Home button and theme toggle
  - `MudMainContent` - Main content container

#### Navigation
- **Home Link**: Now in the top app bar as an icon button
- **Dark Mode Toggle**: Sun/moon icon button in the top right
- **Menu Button**: Hamburger menu icon (reserved for future use)

### 3. Theme Configuration

Custom theme with light and dark palettes:

```csharp
PaletteLight:
- Primary: #1b6ec2 (Azure blue)
- Secondary: #006bb7
- AppbarBackground: #1b6ec2
- Background: #ffffff
- Surface: #ffffff

PaletteDark:
- Primary: #3a8bd8 (Lighter blue)
- Secondary: #2d9cdb
- AppbarBackground: #1e1e1e
- Background: #1e1e1e
- Surface: #252525
```

### 4. Bug Fixes
- **FilterOperator Conflict**: Resolved naming conflict between `SBInspector.Core.Domain.FilterOperator` and `MudBlazor.FilterOperator` by using type alias `DomainFilterOperator`
- **File Cleanup**: Removed unused `NavMenu.razor` and `NavMenu.razor.css`
- **Style Cleanup**: Removed obsolete sidebar and page styles from `MainLayout.razor.css`

## How to Use

### Dark Mode Toggle
Click the sun/moon icon in the top right corner of the app bar to switch between light and dark modes.

### Navigation
- **Home**: Click the home icon in the top app bar to return to the main page
- **App Title**: "SBInspector" text in the top left always visible

## Technical Implementation

### Theme Provider
```razor
<MudThemeProvider @ref="@_mudThemeProvider" IsDarkMode="@_isDarkMode" Theme="_theme" />
```

### Dark Mode State Management
```csharp
private bool _isDarkMode = true; // Default to dark mode
private async Task ToggleDarkMode()
{
    _isDarkMode = !_isDarkMode;
    StateHasChanged();
}
```

## Future Enhancements

### Recommended Component Migrations
To achieve full theme consistency, consider migrating these Bootstrap components to MudBlazor:

1. **ConnectionForm** (Home.razor)
   - `<input>` → `<MudTextField>`
   - `<checkbox>` → `<MudCheckBox>`
   - `<button>` → `<MudButton>`

2. **Message Components**
   - Tables → `<MudTable>`
   - Cards → `<MudCard>`
   - Alerts → `<MudAlert>`

3. **Modals**
   - Bootstrap modals → `<MudDialog>`

4. **Forms**
   - Form inputs → `<MudTextField>`, `<MudSelect>`, etc.
   - Validation → MudBlazor's built-in form validation

## Benefits

### ✅ Achieved
- Modern, professional UI with Material Design
- Built-in dark mode support with smooth transitions
- No left sidebar clutter - cleaner, more spacious layout
- Consistent navigation in top bar
- Better mobile responsiveness with MudBlazor components

### 🎯 Architecture
- Maintains clean architecture principles
- All business logic unchanged
- UI layer properly isolated
- Easy to extend and customize themes

## Known Limitations

- Some components (like the connection form) still use Bootstrap styling with hardcoded colors
- These Bootstrap components don't automatically follow the MudBlazor theme
- For complete theme consistency, all form controls should be migrated to MudBlazor equivalents

## Screenshots

### Dark Mode
![Dark Mode](https://github.com/user-attachments/assets/e91ae6b6-fae4-40c9-a07b-363946e152de)

The UI shows:
- Clean top navigation bar
- Dark theme with comfortable contrast
- No sidebar - all space devoted to content
- Theme toggle and home button easily accessible
