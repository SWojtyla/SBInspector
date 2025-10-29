# Connection Tree View in Navigation

## Overview

This feature adds a tree view of saved connections in the left navigation menu. Users can quickly connect to or disconnect from saved connections by clicking on them directly from the navigation panel.

## Features

### Connection Tree View
- **Location**: Top of the left navigation menu (sidebar)
- **Display**: Shows all saved connections with their names
- **Sorting**: Connections are sorted by last used date (most recent first), then alphabetically by name
- **Visual Indicators**: 
  - Active connection is highlighted with a blue background
  - Connected status is shown with a pulsing green indicator dot
  - Different icons for connected vs disconnected state

### User Interactions

#### Connect to a Saved Connection
1. Click on any saved connection in the tree view
2. The application will automatically connect to that Service Bus namespace
3. The connection will be highlighted and show a green status indicator
4. The last used timestamp for that connection will be updated

#### Disconnect from Current Connection
1. Click on the currently connected (highlighted) connection
2. The application will disconnect from the Service Bus
3. All entity data will be cleared
4. The highlight and status indicator will be removed

### Empty State
- When no connections are saved, displays "No saved connections" message
- Encourages users to save a connection using the connection form

## Technical Implementation

### Components Created

#### 1. `ConnectionTreeView.razor`
- Location: `/SBInspector.Shared/Presentation/Components/UI/ConnectionTreeView.razor`
- Displays the list of saved connections
- Handles click events for connect/disconnect operations
- Uses `ConnectionStateService` to track current connection
- Subscribes to connection state changes for real-time updates

#### 2. `ConnectionStateService.cs`
- Location: `/SBInspector.Shared/Application/Services/ConnectionStateService.cs`
- Singleton service to manage and share connection state across components
- Raises events when connection state changes
- Allows different parts of the application to stay synchronized

#### 3. `ConnectionTreeView.razor.css`
- Location: `/SBInspector.Shared/Presentation/Components/UI/ConnectionTreeView.razor.css`
- Scoped CSS styles for the connection tree view
- Includes hover effects, active state highlighting, and pulsing animation for the status indicator

### Updated Components

#### 1. `NavMenu.razor`
- Added `ConnectionTreeView` component at the top of navigation
- Passes event callbacks for connect/disconnect actions
- Added border separator between connections and main navigation items

#### 2. `MainLayout.razor`
- Handles connection selected events from the navigation
- Manages connection and disconnection logic
- Updates connection timestamps when connections are used
- Refreshes the connection list after successful operations

#### 3. `Home.razor`
- Injects `ConnectionStateService`
- Updates connection state when connecting via the connection form
- Updates connection state when disconnecting via the disconnect button

#### 4. Service Registration
- **`Program.cs`** (Blazor Server): Registers `ConnectionStateService` as singleton
- **`MauiProgram.cs`** (MAUI): Registers `ConnectionStateService` as singleton

## Styling Details

### Color Scheme
- **Default state**: Light gray text (#d7d7d7)
- **Hover state**: White text with subtle background (rgba(255, 255, 255, 0.1))
- **Active/Connected state**: White text with blue background (rgba(0, 123, 255, 0.25))
- **Status indicator**: Green dot (#28a745) with pulsing animation

### Visual Effects
- Smooth transitions for hover and active states
- Pulsing animation for the connected status indicator
- Consistent spacing and sizing with existing navigation items
- Bootstrap icon integration (bi-plug and bi-plug-fill)

## Usage Example

### For Users

1. **Save a connection** (if you haven't already):
   - Enter your Service Bus connection string
   - Check "Save this connection for later use"
   - Enter a friendly name (e.g., "Dev Environment", "Production")
   - Click Connect

2. **Use saved connections**:
   - Look at the top of the left sidebar
   - You'll see all your saved connections under "Connections"
   - Click any connection name to connect to it
   - Click the active (highlighted) connection to disconnect

3. **Visual feedback**:
   - The active connection shows with a blue background
   - A green pulsing dot appears next to the active connection
   - Recently used connections appear at the top of the list

## Benefits

- **Quick Switching**: Switch between different Service Bus namespaces with a single click
- **No Copy-Paste**: No need to copy and paste connection strings
- **Visual Confirmation**: Clear indication of which connection is active
- **Persistent**: Connection list persists across sessions using platform storage (Browser LocalStorage for web, File System for MAUI)
- **Always Visible**: Connections are always accessible in the navigation menu

## Compatibility

- ✅ **Blazor Server Application**: Fully supported
- ✅ **MAUI Desktop Application**: Fully supported
- Both applications share the same codebase through `SBInspector.Shared`

## Future Enhancements

Potential improvements for future versions:
- Edit connection names directly from the tree view
- Delete connections from the tree view context menu
- Group connections by environment or tags
- Search/filter connections when the list gets long
- Import/export connection configurations
