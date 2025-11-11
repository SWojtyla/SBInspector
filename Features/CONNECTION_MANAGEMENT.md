# Connection Management

## Overview

This feature allows users to manage their saved Azure Service Bus connections directly from the navigation menu, including the ability to delete and rename saved environments.

## Features

### Delete Saved Connections

Users can delete saved connections from the nav menu by clicking the delete button (trash icon) next to any saved connection. A confirmation dialog will appear to prevent accidental deletions.

**How to use:**
1. Locate the saved connection in the Connections panel in the navigation menu
2. Click the delete (trash) icon button
3. Confirm the deletion in the dialog that appears

### Rename Saved Connections

Users can rename saved connections from the nav menu by clicking the edit button next to any saved connection. A dialog will appear allowing you to enter a new name.

**How to use:**
1. Locate the saved connection in the Connections panel in the navigation menu  
2. Click the edit (pencil) icon button
3. Enter a new name in the dialog
4. Click "Rename" to save the changes

**Validation:**
- The new name cannot be empty
- The new name must be different from the current name
- The new name cannot conflict with an existing connection name

## Technical Implementation

### Backend Changes

**IStorageService Interface** (`SBInspector.Shared/Core/Interfaces/IStorageService.cs`):
- Added `RenameConnectionAsync(string oldName, string newName)` method

**Storage Implementations**:
- `LocalStorageService`: Implements rename for browser local storage
- `FileStorageService`: Implements rename for file-based storage

Both implementations update the connection name in the stored connections list and persist the changes.

### UI Changes

**ConnectionTreeView Component** (`SBInspector.Shared/Presentation/Components/UI/ConnectionTreeView.razor`):
- Added edit and delete button icons to each connection list item
- Integrated MudBlazor's IDialogService for confirmation and rename dialogs
- Added `ShowDeleteConfirmation` method to handle connection deletion with confirmation
- Added `ShowRenameDialog` method to handle connection renaming with validation

**RenameConnectionDialog Component** (`SBInspector.Shared/Presentation/Components/UI/RenameConnectionDialog.razor`):
- New dialog component for renaming connections
- Validates user input (non-empty, different from current, no conflicts)
- Provides real-time validation feedback

**ConfirmationModal Component** (existing):
- Reused for delete confirmation dialogs
- Consistent with other delete operations in the application (e.g., message templates)

## User Experience

The delete and rename buttons appear next to each saved connection in the navigation menu's Connections panel. The buttons use standard Material Design icons:
- Edit button: Pencil icon for rename operations
- Delete button: Trash icon for delete operations

Both operations maintain consistency with the existing UI patterns used throughout the application, such as the message templates management page.

## Error Handling

- Delete operations silently handle errors to prevent disrupting the user experience
- Rename operations validate input before attempting to save
- The UI refreshes automatically after successful operations
