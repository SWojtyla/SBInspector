# Dark Mode Feature

## Overview
This document describes the dark mode feature implementation for the SBInspector application.

## How to Use
1. Look for the theme toggle button in the top-right corner of the application (next to the "About" link)
2. Click the button to switch between light and dark modes
3. The button displays a moon icon (🌙) in light mode and a sun icon (☀️) in dark mode
4. The theme persists during your session

## Features
- **Theme Toggle Button**: Conveniently located in the top navigation bar
- **Smooth Transitions**: All elements transition smoothly between themes
- **Comprehensive Coverage**: Dark mode affects:
  - Background colors
  - Text colors
  - Form inputs
  - Buttons
  - Cards and panels
  - Tables
  - Modals
  - Navigation sidebar
  - All UI components

## Color Palette

### Light Theme
- Primary background: `#ffffff`
- Secondary background: `#f8f9fa`
- Text: `#212529`
- Links: `#006bb7`

### Dark Theme
- Primary background: `#1a1a1a`
- Secondary background: `#2d2d2d`
- Text: `#e0e0e0`
- Links: `#4da3ff`

## Technical Implementation

The dark mode feature is implemented using:

1. **ThemeService**: A singleton service that manages the current theme state
2. **ThemeToggle Component**: A Blazor component that provides the UI toggle
3. **CSS Variables**: Theme-aware CSS custom properties that change based on the active theme
4. **JavaScript Interop**: Direct DOM manipulation to apply theme classes

### Architecture

- **Service Layer** (`Application/Services/ThemeService.cs`): Manages theme state
- **UI Component** (`Presentation/Components/UI/ThemeToggle.razor`): Provides toggle button
- **Styling** (`wwwroot/app.css`): CSS variables and dark theme styles

### CSS Variables

All colors are defined as CSS variables in `:root` for light theme and `.dark-theme` for dark mode. This makes it easy to maintain and extend the theming system.

## Browser Compatibility

The dark mode feature works in all modern browsers that support:
- CSS Custom Properties (CSS Variables)
- CSS classList manipulation
- Blazor Server

## Future Enhancements

Possible future improvements:
- Persist theme preference to local storage
- System theme detection (prefer-color-scheme)
- Additional theme options (high contrast, custom colors)
- Theme transition animations
