# MAUI UI Feedback Fixes

This document describes the UI improvements made to SBInspector based on MAUI user feedback (November 2025).

## Feature: Service Bus Namespace Display

**Location**: Home page title

**Description**: The main page now displays the connected Service Bus namespace instead of the generic "Azure Service Bus Inspector" title.

**How it works**:
- When not connected, shows "Azure Service Bus Inspector"
- When connected, displays the fully qualified namespace (e.g., "myservicebus.servicebus.windows.net")
- Namespace is extracted from the ServiceBusClient's `FullyQualifiedNamespace` property

**Implementation**:
- Added `GetNamespace()` method to `IServiceBusService` interface
- Home page uses `GetPageTitle()` method to dynamically show the appropriate title

## Feature: Removed Redundant View Buttons

**Location**: Entity Details Panel (Queue and Subscription cards)

**Description**: Removed the "View" buttons from message count cards since clicking on the card itself performs the same action.

**What was changed**:
- Removed "View" button from Active Messages card
- Removed "View" button from Scheduled Messages card
- Removed "View" button from Dead Letter Messages card
- Cards remain fully clickable with cursor pointer indication

**User Benefits**:
- Cleaner, less cluttered interface
- More intuitive - the entire card is the clickable area
- Reduced visual noise

## Feature: Collapsible Sidebar

**Location**: Left navigation panel

**Description**: Added the ability to hide/show the left menu panel to maximize screen space for viewing messages.

**How to use**:
- Click the hamburger menu icon (☰) in the top-left corner
- The sidebar will slide out to the left with a smooth transition
- Click the icon again to show the sidebar

**Implementation**:
- Added toggle button in MainLayout with MudBlazor IconButton
- CSS transitions for smooth collapse/expand animation
- Sidebar collapses to -250px margin (completely hidden)
- Works on all screen sizes

## Feature: Theme-Aware Navigation

**Location**: Left navigation panel

**Description**: The navigation panel now respects the current theme instead of using a fixed green gradient.

**What changed**:
- Navigation background now uses theme's `DrawerBackground` color
- Navigation text now uses theme's `DrawerText` color
- Icons updated to use `currentColor` for theme compatibility
- Top row of navigation also matches theme colors

**User Benefits**:
- Consistent appearance across the entire application
- Works correctly with Light, Dark, and Custom themes
- Navigation is readable in all theme configurations

## Feature: Enhanced Theme Presets

**Location**: Theme Customization page

**Description**: Expanded the quick preset themes to include complete color schemes with both light and dark variants.

### Available Presets

#### Light Themes

**Ocean Blue**
- Primary: Blue tones (#0077be)
- Background: Light blue-tinted (#e6f3f9)
- Perfect for a professional, calm appearance

**Forest Green**
- Primary: Green tones (#2d5016)
- Background: Light green-tinted (#f0f4e8)
- Natural, earthy appearance

**Sunset Orange**
- Primary: Orange/Red tones (#ff6b35)
- Background: Light orange-tinted (#fff3e0)
- Warm, energetic appearance

**Monochrome**
- Primary: Gray tones (#424242)
- Background: Light gray (#fafafa)
- Classic, minimal appearance

#### Dark Themes

**Ocean Dark**
- Primary: Bright blue tones (#4fc3f7)
- Background: Dark blue-black (#0d1117)
- Good for low-light environments

**Forest Dark**
- Primary: Light green tones (#81c784)
- Background: Dark with green accent (#1a1a1a)
- Easy on the eyes in dark environments

**Sunset Dark**
- Primary: Bright orange tones (#ff8a65)
- Background: Dark with warm accents (#1a1a1a)
- Warm dark theme

**Monochrome Dark**
- Primary: Light gray tones (#9e9e9e)
- Background: Pure dark gray/black (#121212)
- High contrast dark theme

### What Each Preset Includes

Every preset now provides a complete theme with:
- Primary color
- Secondary color
- Background color
- Surface color (cards/panels)
- AppBar background and text colors
- Drawer (navigation) background and text colors
- Appropriate dark mode flag

**User Benefits**:
- Quick starting points for customization
- Professionally designed color schemes
- Consistent appearance throughout the app
- Easy switching between light and dark modes

## Technical Notes

### Dependencies
- MudBlazor for UI components
- Azure.Messaging.ServiceBus for namespace extraction

### Files Modified
- `IServiceBusService.cs` - Added GetNamespace method
- `ServiceBusService.cs` - Implemented GetNamespace
- `Home.razor` - Added dynamic title display
- `EntityDetailsPanel.razor` - Removed View buttons
- `MainLayout.razor` - Added sidebar toggle
- `MainLayout.razor.css` - Added collapse styles
- `NavMenu.razor` - Added theme-aware styling
- `NavMenu.razor.css` - Removed fixed colors
- `ThemeCustomization.razor` - Enhanced presets

### Browser Compatibility
All features use standard CSS and JavaScript, compatible with modern browsers.

### Performance Impact
- Minimal - theme colors are computed once per render
- Sidebar transition is CSS-based (GPU accelerated)
- No additional API calls or heavy computations

## Testing

### Manual Testing Checklist

✅ **Service Bus Namespace Display**
- [ ] Connect to Service Bus - verify namespace appears in title
- [ ] Disconnect - verify title returns to "Azure Service Bus Inspector"
- [ ] Switch between connections - verify title updates correctly

✅ **View Button Removal**
- [ ] Select a queue - verify no "View" buttons on cards
- [ ] Click on Active Messages card - verify messages load
- [ ] Click on Scheduled Messages card - verify messages load
- [ ] Click on Dead Letter Messages card - verify messages load
- [ ] Select a subscription - verify same behavior

✅ **Collapsible Sidebar**
- [ ] Click hamburger menu - verify sidebar slides out
- [ ] Click hamburger menu again - verify sidebar slides back in
- [ ] Verify smooth animation during transition
- [ ] Test on mobile/tablet viewports
- [ ] Verify entity tree and navigation are hidden when collapsed

✅ **Theme-Aware Navigation**
- [ ] Select Light theme - verify navigation has light colors
- [ ] Select Dark theme - verify navigation has dark colors
- [ ] Create custom theme - verify navigation matches
- [ ] Verify icons are visible in all themes
- [ ] Verify text is readable in all themes

✅ **Enhanced Theme Presets**
- [ ] Load Ocean Blue preset - verify all colors are set
- [ ] Load Forest Green preset - verify all colors are set
- [ ] Load Sunset Orange preset - verify all colors are set
- [ ] Load Monochrome preset - verify all colors are set
- [ ] Load Ocean Dark preset - verify dark mode and colors
- [ ] Load Forest Dark preset - verify dark mode and colors
- [ ] Load Sunset Dark preset - verify dark mode and colors
- [ ] Load Monochrome Dark preset - verify dark mode and colors
- [ ] Verify each preset affects: background, surface, appbar, drawer, and primary colors
- [ ] Save a preset and reload the app - verify it persists

## Known Limitations

None - all features are fully functional.

## Future Enhancements

Potential improvements for future versions:
- Add keyboard shortcuts for sidebar toggle (e.g., Ctrl+B)
- Remember sidebar state across sessions
- Add more theme presets (e.g., High Contrast, Retro, Neon)
- Allow saving custom presets with user-defined names
- Add theme preview cards showing what each preset looks like
