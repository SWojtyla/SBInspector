# Column Sorting via Drag-and-Drop

## Feature Overview

Users can now reorder columns in the messages overview by dragging and dropping them within the Column Configuration Modal. This feature allows for a more intuitive and visual way to customize the column order beyond just showing/hiding columns.

## How to Use

1. **Open the Column Configuration Modal**
   - Navigate to a message list (Queue or Topic)
   - Click the "Configure Columns" button (gear icon ⚙️)

2. **Reorder Columns**
   - You'll see a drag indicator icon (≡) next to each column
   - Click and hold on any column item
   - Drag the column to your desired position
   - Release to drop the column in its new position
   - The order is updated automatically

3. **Column Groups**
   - **Default Columns**: System-defined columns that can be reordered within their group
   - **Custom Columns**: User-defined columns that can be reordered separately
   - Columns can only be reordered within their own group (system vs custom)

4. **Save Changes**
   - Click the "Save" button to persist your column order
   - The new order will be reflected in the messages table
   - The order is saved to your local configuration

## User Interface

- Each column now displays a **drag indicator icon** (≡) on the left side
- An info message at the top explains the drag-and-drop feature
- The cursor changes to a "move" cursor when hovering over columns
- Columns maintain their visibility checkbox and other controls while being draggable

## Technical Implementation

### Components Used

- **MudBlazor MudDropContainer**: Container component that manages drag-and-drop state
- **MudBlazor MudDropZone**: Drop zones for system and custom columns
- **ItemRenderer**: Template for rendering individual draggable column items

### Drag-and-Drop Behavior

1. **Separate Drop Zones**: System and custom columns have separate drop zones to prevent mixing
2. **Order Update**: When a column is dropped, the `Order` property is recalculated for all items in that group
3. **Persistence**: The updated order is saved when the user clicks "Save"

### Code Changes

**File Modified**: `SBInspector.Shared/Presentation/Components/UI/ColumnConfigurationModal.razor`

**Key Changes**:
- Added `MudDropContainer` and `MudDropZone` components
- Implemented `OnSystemColumnDropped` and `OnCustomColumnDropped` event handlers
- Added drag indicator icons to each column item
- Added helper properties `SystemColumns` and `CustomColumns` for filtering
- Maintained visual styling with `cursor: move` and `user-select: none`

### Algorithm

The reordering algorithm:
1. Get the current ordered list of columns in the drop zone
2. Find the old index of the dragged item
3. Remove the item from its old position
4. Insert the item at the new position
5. Recalculate the `Order` property for all items (0, 1, 2, ...)
6. For custom columns, offset the order by the number of system columns
7. Trigger a UI refresh with `StateHasChanged()`

## Example

**Before** (initial order):
- Message ID (Order: 0)
- Originating Endpoint (Order: 1)
- Enqueued Time (Order: 2)
- Delivery Count (Order: 3)

**After** (drag "Message ID" to position 2):
- Originating Endpoint (Order: 0)
- Enqueued Time (Order: 1)
- Message ID (Order: 2)
- Delivery Count (Order: 3)

## Benefits

1. **More Intuitive**: Visual drag-and-drop is more intuitive than numeric order fields
2. **Faster**: Users can quickly rearrange columns without typing
3. **Visual Feedback**: Users see the reordering happen in real-time
4. **Maintains Existing Features**: All existing features (show/hide, add/remove, reset) still work

## Compatibility

- Works in both **Blazor Server** and **.NET MAUI** applications
- Requires MudBlazor 8.13.0 or higher
- No breaking changes to existing functionality

## Testing

To test the drag-and-drop functionality:

1. Build and run the application
2. Connect to a Service Bus namespace
3. Navigate to a queue or topic with messages
4. Click the "Configure Columns" button
5. Try dragging columns to different positions
6. Verify the order updates correctly
7. Click "Save" and check that the message table reflects the new order
8. Reopen the modal and verify the order persists

## Future Enhancements

Potential future improvements:
- Allow dragging columns between system and custom groups
- Add keyboard shortcuts for reordering (Alt+Up/Down)
- Add visual highlighting during drag operations
- Support touch gestures on mobile devices
