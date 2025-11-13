# Menu Collapse Feature

## Overview
This feature allows users to collapse both the left navigation menu and the entities menu to maximize workspace for viewing messages and entity details.

## Features

### 1. Left Navigation Menu Collapse
- **Location**: Left sidebar containing navigation links (Service Bus Inspector, Message Templates, Theme Customization, Settings)
- **Toggle Button**: Located in the top-right corner of the navigation bar
- **Behavior**:
  - When collapsed: Shows only icons, menu width reduces to ~80px
  - When expanded: Shows full text labels, menu width is 250px
  - Smooth transition animation (0.3s)
  - Icons remain visible and functional in both states

### 2. Entities Menu Collapse
- **Location**: Left panel on the Home page showing queues, topics, and subscriptions
- **Toggle Button**: Located next to the refresh button in the entities panel header
- **Behavior**:
  - When collapsed: Shows only control buttons, panel width reduces to ~60px
  - When expanded: Shows full entity tree with search and details
  - Smooth transition animation (0.3s)
  - Refresh button remains accessible in both states

## How to Use

### Collapsing the Left Navigation Menu
1. Look for the chevron button (←) in the top-right corner of the navigation bar
2. Click the button to collapse the menu
3. Click again (now showing →) to expand it back

### Collapsing the Entities Menu
1. Navigate to the Home page (Service Bus Inspector)
2. Connect to a Service Bus namespace
3. Look for the chevron button (←) next to the refresh button in the Entities panel
4. Click the button to collapse the panel
5. Click again (now showing →) to expand it back

## Benefits
- **More Screen Space**: Maximize workspace for viewing message details and entity information
- **Improved Focus**: Reduce visual clutter when you don't need the navigation or entity lists
- **Flexible Layout**: Adapt the interface to your workflow needs
- **Responsive Design**: Works well with the existing responsive layout

## Technical Implementation

### Files Modified
- `SBInspector.Shared/Presentation/Components/Layout/NavMenu.razor` - Added collapse toggle and conditional rendering
- `SBInspector.Shared/Presentation/Components/Layout/NavMenu.razor.css` - Added collapsed state styles and animations
- `SBInspector.Shared/Presentation/Components/Layout/MainLayout.razor.css` - Added sidebar width transition
- `SBInspector.Shared/Presentation/Components/UI/EntityTreeView.razor` - Added collapse toggle and conditional rendering
- `SBInspector.Shared/Presentation/Components/UI/EntityTreeView.razor.css` - Created new file for collapsed state styles

### Key Implementation Details
- Uses CSS transitions for smooth collapse/expand animations
- State is managed locally in each component (not persisted across sessions)
- Responsive design maintained - mobile behavior unchanged
- Icons remain visible and tooltips provide context when collapsed
