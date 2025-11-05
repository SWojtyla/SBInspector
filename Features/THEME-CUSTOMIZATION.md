# Theme Customization Feature

## Overview

The Theme Customization feature allows users to personalize the appearance of SBInspector by choosing from preset themes or creating their own custom theme with personalized colors and layout properties.

## User Interface

The Theme Customization page is accessed via the navigation menu and features:

### Layout
- Main content area (left side, 8 columns wide) with theme configuration
- Info panel (right side, 4 columns wide) with help and preview information
- Bootstrap-based responsive design

### Theme Preset Section
- Card with dropdown selector for choosing between Light, Dark, and Custom themes
- Real-time descriptive text that updates based on selected preset
- Immediate application of theme changes across the application

### Custom Theme Editor (when Custom preset is selected)
- **Theme Name Field**: Text input for naming your custom theme
- **Dark Mode Toggle**: Checkbox to switch between light and dark mode bases
- **Color Sections**: Organized groups of color pickers
  - Primary Colors: Primary and Secondary colors
  - Application Bar: AppBar Background and Text colors
  - Background & Surface: Background and Surface colors
  - Navigation Drawer: Drawer Background and Text colors
- **Layout Properties**: Border radius text field
- **Quick Presets**: Button group with Ocean Blue, Forest Green, Sunset Orange, and Monochrome options
- **Action Buttons**: Reset to Default (secondary) and Save Theme (primary) buttons

### Visual Styling
- Uses Bootstrap card components for organization
- Color pickers are full-width for easy interaction
- Icons from Bootstrap Icons for visual clarity
- Consistent spacing and padding throughout
- Success/Error notifications appear as dismissible alerts at the top of the viewport

## How to Use

### Accessing Theme Customization

1. Navigate to the application
2. Open the navigation menu
3. Click on "Theme Customization" menu item

### Selecting a Theme Preset

The theme customization page offers three preset themes:

1. **Light Theme** - Classic light theme with blue accents
2. **Dark Theme** - Dark theme optimized for low-light environments
3. **Custom Theme** - Create your own personalized theme

Simply select the desired preset from the dropdown menu. The theme will be applied immediately across the application.

### Creating a Custom Theme

When you select "Custom Theme", you can customize the following aspects:

#### Theme Settings
- **Theme Name**: Give your custom theme a meaningful name
- **Dark Mode Toggle**: Switch between light and dark mode base themes

#### Color Customization
- **Primary Color**: Main brand color for buttons and accents
- **Secondary Color**: Secondary accent color
- **AppBar Background**: Top navigation bar background color
- **AppBar Text**: Top navigation bar text color
- **Background Color**: Main background color of the application
- **Surface Color**: Card and panel background color
- **Drawer Background**: Side navigation background color
- **Drawer Text**: Side navigation text color

#### Layout Properties
- **Border Radius**: Default border radius for UI elements (e.g., 4px, 8px)

### Quick Presets

To make customization easier, the feature includes quick preset buttons that apply pre-configured color schemes:

- **Ocean Blue**: Blue-themed color palette
- **Forest Green**: Green-themed color palette
- **Sunset Orange**: Orange-themed color palette
- **Monochrome**: Gray-scale color palette

These presets serve as starting points that you can further customize to your liking.

### Saving Your Theme

1. Make your desired changes to the theme settings
2. Click the "Save Theme" button at the bottom of the page
3. Your theme will be saved and applied immediately
4. The theme persists across browser sessions and application restarts

### Resetting to Default

If you want to revert to the original custom theme configuration:

1. Click the "Reset to Default" button
2. This will restore the default custom theme colors (teal/green theme)

## Technical Implementation

### Architecture

The theme customization feature follows the clean architecture pattern used throughout SBInspector:

#### Domain Layer
- **ThemeConfiguration.cs**: Domain model representing theme settings including colors and layout properties
- **ThemePreset enum**: Defines available theme presets (Light, Dark, Custom)

#### Application Layer
- **ThemeService.cs**: Service that manages theme state, persistence, and provides MudBlazor theme objects
  - Loads and saves theme configuration from/to disk
  - Provides theme change notifications via events
  - Generates MudBlazor MudTheme objects for each preset

#### Presentation Layer
- **ThemeCustomization.razor**: Page component for theme editing
- **MainLayout.razor**: Updated to use ThemeService and respond to theme changes
- **NavMenu.razor**: Updated to include Theme Customization menu item

### Data Persistence

Theme settings are persisted to the file system in two files:
- `theme-config.json`: Stores the custom theme configuration
- `theme-preset.txt`: Stores the currently selected preset (Light/Dark/Custom)

Location: `%APPDATA%\SBInspector\` on Windows, `~/.config/SBInspector/` on Linux/Mac

### Real-time Updates

The ThemeService uses the event pattern to notify components when the theme changes:
- Components subscribe to the `ThemeChanged` event
- When theme changes, all subscribed components are notified
- MainLayout re-renders with the new theme applied

### MudBlazor Integration

The feature integrates with MudBlazor's theming system:
- Generates `MudTheme` objects with appropriate `PaletteLight` or `PaletteDark` properties
- Applies theme via `MudThemeProvider` in MainLayout
- Supports all MudBlazor color properties

## Examples

### Example 1: Switching to Dark Theme

```csharp
// Navigate to Theme Customization page
// Select "Dark Theme" from the preset dropdown
// Theme is immediately applied throughout the app
```

### Example 2: Creating a Custom Ocean Theme

```csharp
// Navigate to Theme Customization page
// Select "Custom Theme" preset
// Click "Ocean Blue" quick preset button
// Adjust colors as desired:
// - Primary: #0077be
// - AppBar Background: #005f99
// - Drawer Background: #e6f3f9
// Click "Save Theme"
```

### Example 3: Programmatic Theme Access

```csharp
// In a component that needs the current theme
@inject ThemeService ThemeService

private void SomeMethod()
{
    var currentTheme = ThemeService.GetCurrentTheme();
    var customConfig = ThemeService.GetCustomThemeConfiguration();
    
    // Subscribe to theme changes
    ThemeService.ThemeChanged += OnThemeChanged;
}

private void OnThemeChanged(object? sender, EventArgs e)
{
    // React to theme changes
    StateHasChanged();
}
```

## Browser Compatibility

The theme customization feature uses HTML5 color input controls, which are supported in:
- Chrome/Edge 20+
- Firefox 29+
- Safari 12.1+
- Opera 15+

## Known Limitations

1. Color picker appearance varies by browser and operating system
2. Theme changes apply immediately but may require page refresh for some third-party components
3. Custom themes are stored locally on the device

## Future Enhancements

Potential improvements for future versions:
- Import/Export theme configurations
- Share themes with other users
- More preset themes
- Preview mode to see changes before saving
- Theme scheduling (auto-switch based on time of day)
- Per-component color overrides
