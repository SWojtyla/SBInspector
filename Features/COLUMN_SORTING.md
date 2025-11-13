# Column Sorting via Drag-and-Drop

## Feature Overview

Users can now reorder columns in the messages table by dragging and dropping column headers directly in the table view. This provides an intuitive and immediate way to customize the column order without needing to open a settings dialog.

## How to Use

1. **View the Messages Table**
   - Navigate to a message list (Queue or Topic)
   - Look at the column headers in the table

2. **Reorder Columns by Dragging Headers**
   - Notice the drag indicator icon (≡) at the start of each column header
   - Click and hold on any column header
   - Drag the column to your desired position
   - You'll see visual feedback:
     - The dragged column becomes semi-transparent
     - The drop target column highlights with a blue border
   - Release to drop the column in its new position
   - The order is updated and saved automatically

3. **Additional Column Configuration**
   - Click the settings icon (⚙️) above the table to open the Column Configuration Modal
   - Use the modal to:
     - Show/hide columns with checkboxes
     - Add custom property columns
     - Remove custom columns
     - Reset to default configuration

## User Interface

### Table Header Features
- Each column header displays a **drag indicator icon** (≡) on the left side
- Cursor changes to "move" when hovering over column headers
- **During drag:**
  - Dragged column appears semi-transparent (opacity: 0.5)
  - Drop target highlights with blue border and light blue background
  - Visual feedback makes it clear where the column will be dropped

### Column Configuration Modal
- Click the settings icon to access additional options
- Show/hide columns with checkboxes
- Add custom property columns from message metadata
- Remove custom columns you no longer need
- Reset to default column configuration

## Technical Implementation

### Drag-and-Drop in Table Headers

The primary reordering mechanism uses HTML5 drag-and-drop API directly on table column headers:

**Components:**
- **MessageListTable.razor** - Main table component with draggable headers
- **MudTh elements** - Table header cells with `draggable="true"` attribute
- **Event handlers:**
  - `@ondragstart` - Captures which column is being dragged
  - `@ondragenter` - Tracks which column the drag is over
  - `@ondrop` - Handles the drop and reordering logic
  - `@ondragover:preventDefault` - Allows dropping

**Visual Feedback:**
- `GetColumnHeaderStyle()` method provides dynamic styling:
  - Dragged column: `opacity: 0.5`
  - Drop target: Blue border and background highlight
  - Default: `cursor: move` to indicate draggability

**Algorithm:**
1. User drags a column header
2. `HandleDragStart` captures the dragged column reference
3. `HandleDragEnter` tracks the current drop target
4. `HandleDrop` executes when user releases:
   - Gets list of visible columns ordered by Order property
   - Finds old and new index positions
   - Removes column from old position
   - Inserts column at new position
   - Recalculates Order property for all columns (0, 1, 2, ...)
   - Saves configuration to ColumnConfigurationService
   - Triggers UI refresh with StateHasChanged()

### Column Configuration Modal

The modal dialog provides additional configuration options:

- **MudDropContainer** and **MudDropZone** - For drag-and-drop within modal (legacy feature)
- **GetDropZoneForColumn()** - Determines zone based on IsSystemColumn
- **OnColumnDropped** - Event handler for modal drag-and-drop
- **Show/hide checkboxes** - Toggle column visibility
- **Add/remove custom columns** - Manage custom property columns

## Example

### Drag-and-Drop in Table Headers

**Before** (initial column order):
```
| ≡ Message ID | ≡ Originating Endpoint | ≡ Enqueued Time | ≡ Delivery | Actions |
```

**Action**: User drags "Message ID" column header to the right, drops it between "Enqueued Time" and "Delivery"

**During drag**:
- "Message ID" column appears semi-transparent
- Drop target position highlights with blue border

**After** (new order):
```
| ≡ Originating Endpoint | ≡ Enqueued Time | ≡ Message ID | ≡ Delivery | Actions |
```

Order properties automatically updated:
- Originating Endpoint: Order = 0
- Enqueued Time: Order = 1
- Message ID: Order = 2
- Delivery: Order = 3

Changes are saved immediately to local configuration.

### Using the Configuration Modal

You can still use the modal for additional options:
1. Click the settings icon (⚙️)
2. Check/uncheck columns to show/hide them
3. Add custom property columns from message metadata
4. Remove custom columns you no longer need
5. Click "Save" to apply changes

## Benefits

1. **Immediate Feedback**: Drag column headers directly in the table - no need to open a settings modal
2. **Intuitive UX**: Natural interaction pattern familiar from file managers and other applications
3. **Visual Guidance**: Clear drag indicator icons and visual feedback during drag operations
4. **Auto-save**: Changes are saved automatically upon drop
5. **Maintains Existing Features**: Column configuration modal still available for show/hide and custom columns
6. **Flexible**: Reorder any visible column to any position
7. **Persistent**: Column order is saved to local configuration and restored on next session

## Compatibility

- Works in both **Blazor Server** and **.NET MAUI** applications
- Uses standard HTML5 drag-and-drop API for maximum compatibility
- Requires MudBlazor 8.13.0 or higher for table components
- No breaking changes to existing functionality

## Testing

To test the drag-and-drop functionality:

1. Build and run the application
2. Connect to a Service Bus namespace
3. Navigate to a queue or topic with messages
4. Look at the column headers - you should see drag indicators (≡)
5. Try dragging a column header left or right
6. Verify visual feedback (semi-transparent dragged column, blue highlight on drop target)
7. Drop the column in a new position
8. Verify the column order changes immediately
9. Refresh the page and verify the order persists
10. Try the settings icon to test show/hide functionality

## Future Enhancements

Potential future improvements:
- Drag columns between visible and hidden states
- Keyboard shortcuts for reordering (Alt+Left/Right Arrow)
- Touch gesture support for mobile devices
- Column width adjustment by dragging borders
- Save multiple column layout presets
