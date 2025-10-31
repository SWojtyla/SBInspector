# App Icon in Left Menu

## Feature Overview

This feature adds the application's favicon icon to the bottom of the left navigation menu, providing better branding and visual appeal.

## Visual Design

- **Icon Size**: 48x48 pixels
- **Opacity**: 70% by default, increases to 100% on hover
- **Position**: Bottom of the left navigation menu
- **Animation**: Smooth opacity transition (0.3s ease)

## Implementation

### Files Modified

1. **NavMenu.razor**
   - Added an `app-icon-container` div at the bottom of the menu
   - Uses the favicon image with absolute path (`/favicon.png`)

2. **NavMenu.razor.css**
   - Added `.app-icon-container` styling with `margin-top: auto` to push it to the bottom
   - Added `.app-icon` styling for size, opacity, and hover effects
   - Changed `.nav-scrollable` to use flexbox layout for proper bottom positioning

### Technical Details

The implementation uses CSS Flexbox to ensure the icon stays at the bottom of the menu:
- `.nav-scrollable` uses `display: flex` with `flex-direction: column`
- `.app-icon-container` uses `margin-top: auto` to push itself to the bottom

This approach ensures the icon remains at the bottom regardless of the number of menu items above it.

## Responsive Behavior

- **Desktop (≥641px)**: Icon is always visible at the bottom of the left sidebar
- **Mobile (<641px)**: Icon appears at the bottom when the hamburger menu is expanded

## User Experience

The icon provides:
- Visual branding reinforcement
- A subtle interactive element (hover effect)
- Consistent presence across all pages
- Professional appearance

## Accessibility

- The icon has an `alt` attribute set to "SBInspector" for screen readers
- The hover effect provides visual feedback for mouse users

## Screenshots

See the pull request for before and after screenshots demonstrating the feature on both desktop and mobile views.
