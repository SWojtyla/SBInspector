# Menu Collapse Feature

## Overview
This feature allows users to collapse both the left navigation menu and the entities menu horizontally to maximize workspace for viewing messages and entity details.

## Features

### 1. Left Navigation Menu Collapse
- **Location**: Left sidebar containing navigation links (Service Bus Inspector, Message Templates, Theme Customization, Settings)
- **Toggle Button**: Located in the top-right corner of the navigation bar
- **Behavior**:
  - When collapsed: Shows only icons, menu width reduces to 60px, no text overflow
  - When expanded: Shows full text labels, menu width is 250px
  - Smooth transition animation (0.3s)
  - Icons remain visible and functional in both states
  - Connection tree is completely hidden when collapsed

### 2. Entities Menu Collapse
- **Location**: Left panel on the Home page showing queues, topics, and subscriptions
- **Toggle Button**: Located next to the refresh button in the entities panel header
- **Behavior**:
  - When collapsed: Shows only control buttons, panel width reduces to 60px
  - When expanded: Shows full entity tree with search and details
  - Smooth transition animation (0.3s)
  - Refresh button remains accessible in both states
  - Grid layout automatically adjusts: collapsed (1/12) vs expanded (3/12) columns

## How to Use

### Collapsing the Left Navigation Menu
1. Look for the chevron button (←) in the top-right corner of the navigation bar
2. Click the button to collapse the menu to icon-only view (60px wide)
3. Click again (now showing →) to expand it back to full view
4. The main content area automatically expands when menu is collapsed

### Collapsing the Entities Menu
1. Navigate to the Home page (Service Bus Inspector)
2. Connect to a Service Bus namespace
3. Look for the chevron button (←) next to the refresh button in the Entities panel
4. Click the button to collapse the panel (reduces to 60px wide)
5. Click again (now showing →) to expand it back
6. The right panel (messages/details) automatically expands to use the additional space

## Benefits
- **Maximized Horizontal Space**: Both panels collapse horizontally, providing significantly more width for message tables and details
- **No Text Overflow**: Proper overflow handling ensures clean collapsed state
- **Dynamic Grid Layout**: MudGrid columns automatically adjust (3/9 → 1/11 split)
- **Improved Focus**: Reduce visual clutter when you don't need the navigation or entity lists
- **Flexible Layout**: Adapt the interface to your workflow needs
- **Responsive Design**: Works well with the existing responsive layout

## Technical Implementation

### Files Modified
- `SBInspector.Shared/Presentation/Components/Layout/NavMenu.razor` - Added collapse toggle, conditional rendering, and overflow handling
- `SBInspector.Shared/Presentation/Components/Layout/NavMenu.razor.css` - Added collapsed state styles (60px width) with overflow:hidden
- `SBInspector.Shared/Presentation/Components/Layout/MainLayout.razor.css` - Updated sidebar width transition (250px → 60px)
- `SBInspector.Shared/Presentation/Components/UI/EntityTreeView.razor` - Added collapse toggle, conditional rendering, and OnCollapsedChanged callback
- `SBInspector.Shared/Presentation/Components/UI/EntityTreeView.razor.css` - Created with collapsed state styles and overflow:hidden
- `SBInspector.Shared/Presentation/Components/Pages/Home.razor` - Added dynamic grid column sizing based on collapse state

### Key Implementation Details
- Uses CSS transitions for smooth collapse/expand animations
- State is managed locally in each component (not persisted across sessions)
- Responsive design maintained - mobile behavior unchanged
- Icons remain visible and tooltips provide context when collapsed
- Connection tree completely hidden when left nav is collapsed (no overflow)
- Grid columns dynamically adjust: md="3" → md="1" when entities panel is collapsed
- Overflow hidden on both panels prevents any text bleeding through
