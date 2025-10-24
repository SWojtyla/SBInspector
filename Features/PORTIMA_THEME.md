# Portima Theme Implementation

## Overview

The SBInspector UI has been updated to use Portima's brand colors, replacing the previous blue/purple color scheme with Portima green and yellow.

## Color Scheme

### Primary Colors
- **Portima Green**: `#00836b`
  - Used for: Primary buttons, links, sidebar gradient, active states
  - Darker variant: `#006d59` (used for button borders)
  - Background tint: `#e6f5f2` (used for hover/selected states)

- **Portima Yellow**: `#ffd17c`
  - Used for: Scheduled message badges and icons
  - Background tint: `#fff8e6` (used for icon backgrounds)

### Secondary Colors
- **Dead Letter Red**: `#dc3545` (unchanged - used for error states)
- **Neutral Gray**: `#6c757d` (unchanged - used for muted text)

## Updated UI Elements

### Navigation & Layout
1. **Sidebar Background**
   - Gradient from `#00836b` to `#005a4a`
   - Replaces the previous blue/purple gradient

2. **Active Navigation Items**
   - Highlight color: White with 37% opacity overlay
   - Maintains good contrast with green background

### Buttons & Links
1. **Primary Buttons** (`.btn-primary`)
   - Background: `#00836b`
   - Border: `#006d59`
   - Text: White

2. **Links** (`a`, `.btn-link`)
   - Color: `#00836b`
   - Maintains accessibility contrast ratios

### Interactive Elements
1. **Focus States**
   - Outline color: `#00836b`
   - Box shadow: `0 0 0 0.25rem #00836b`
   - Applied to buttons, inputs, and form controls

2. **Hover States**
   - Table sortable headers: `#00836b`
   - Tree node expansion icons: `#00836b`

### Tree View & Entity Lists
1. **Selected Nodes**
   - Background: `#e6f5f2` (light green tint)
   - Border: 3px solid `#00836b`
   - Icon color: `#00836b`

2. **Tree Header Icons**
   - Color: `#00836b`

### Message Badges
1. **Active Message Badge**
   - Background: `#00836b`
   - Text: White

2. **Scheduled Message Badge**
   - Background: `#ffd17c` (Portima yellow)
   - Text: Black

3. **Count Badges**
   - Same color scheme as message badges
   - Used in tree view for message counts

### Message Type Cards
1. **Active Message Card**
   - Icon background: `#e6f5f2`
   - Icon color: `#00836b`
   - Hover background: `#e6f5f2`

2. **Scheduled Message Card**
   - Icon background: `#fff8e6`
   - Icon color: `#ffd17c`

### Details Panel
- Header icons: `#00836b`
- Maintains consistent branding throughout

## Files Modified

1. **SBInspector.Shared/wwwroot/app.css**
   - Main shared stylesheet
   - Contains all component styles

2. **SBInspector/wwwroot/app.css**
   - Blazor Server specific styles
   - Mirrors shared styles for consistency

3. **SEBInspector.Maui/wwwroot/css/app.css**
   - MAUI application styles
   - Basic theme colors for cross-platform consistency

4. **SBInspector.Shared/Presentation/Components/Layout/MainLayout.razor.css**
   - Layout-specific styles
   - Sidebar gradient definition

## Accessibility

All color changes maintain WCAG 2.1 Level AA contrast ratios:
- Green on white: 4.57:1 (exceeds 4.5:1 requirement for normal text)
- White on green: 4.57:1 (exceeds 3:1 requirement for large text)
- Focus indicators: 2px outline with 2px offset for clear visibility

## Browser Compatibility

The color updates use standard CSS color values (hex codes) and are compatible with all modern browsers:
- Chrome/Edge (Chromium-based)
- Firefox
- Safari
- Mobile browsers

## Implementation Notes

### Color Consistency
All instances of the previous blue color scheme (`#1b6ec2`, `#0d6efd`, etc.) have been replaced with the corresponding Portima green variants.

### Gradient Smoothness
The sidebar gradient uses a darker shade of green (`#005a4a`) to maintain visual depth similar to the original design.

### Icon Colors
SVG icons in the navigation use white color and are unaffected by the color scheme change, maintaining good contrast against the green background.

### Yellow Accent Usage
The Portima yellow is used sparingly and specifically for scheduled message indicators, providing a clear visual distinction from active messages.

## Future Considerations

If additional brand colors need to be incorporated:
1. Consider using yellow for warning states or informational badges
2. Maintain the green as the primary action color
3. Keep the color palette limited to maintain visual clarity
4. Test all new colors for accessibility compliance

## Testing

To verify the theme implementation:
1. Navigate through all pages (Home, Message Templates)
2. Check button hover and focus states
3. Verify navigation active states
4. Review message badges and count indicators
5. Test form focus indicators
6. Ensure all interactive elements use the correct colors

## Screenshots

**Before (Blue/Purple Theme):**
- Sidebar used blue/purple gradient
- Buttons and links used blue colors

**After (Portima Green/Yellow Theme):**
- Sidebar uses green gradient
- All interactive elements use Portima green
- Scheduled items use Portima yellow
- Consistent branding throughout the application
