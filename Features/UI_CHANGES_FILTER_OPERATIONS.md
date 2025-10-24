# UI Changes Summary - Filter-Based Operations

This document describes the user interface changes made to support the new filter-based operations.

## Messages Panel Updates

The Messages Panel (displayed when viewing messages from a queue or subscription) now has three new buttons:

### Button Layout (Top Right of Messages Panel)

```
┌─────────────────────────────────────────────────────────────────┐
│ Queue/Subscription Messages Panel                               │
├─────────────────────────────────────────────────────────────────┤
│ [🔄 Refresh] [➕ Send New] [💾 Export Filtered]              │
│              [🗑️ Delete Filtered] [🗑️🗑️ Purge All] [✖️ Close]  │
└─────────────────────────────────────────────────────────────────┘
```

### Button Colors and Icons

1. **Export Filtered** (Primary/Blue button - more visible!)
   - Icon: 💾 (download icon)
   - Color: Primary (bright blue)
   - Tooltip: "Download messages matching current filters to Downloads folder"
   - Action: 
     - **Web**: Immediately downloads JSON file
     - **MAUI**: Saves to Downloads folder and shows location in dialog

2. **Delete Filtered** (Warning/Yellow button)
   - Icon: 🗑️ (single trash icon)
   - Color: Warning (yellow/orange)
   - Tooltip: "Delete messages matching current filters"
   - Action: Opens confirmation dialog for filtered deletion

3. **Purge All** (Danger/Red button - existing)
   - Icon: 🗑️🗑️ (double trash icon)
   - Color: Danger (red)
   - Tooltip: "Delete all messages"
   - Action: Opens confirmation dialog for full purge

## Filter Panel Updates

The Message Filter Panel now includes the "Not Contains" operator:

### Filter Operator Dropdown (for Application Properties)

```
┌─────────────────────────────┐
│ Operator ▼                  │
├─────────────────────────────┤
│ Contains                    │
│ Not Contains          ← NEW │
│ Equals                      │
│ Not Equals                  │
│ Regex                       │
│ Greater Than                │
│ Less Than                   │
│ Greater Than or Equal       │
│ Less Than or Equal          │
└─────────────────────────────┘
```

## Confirmation Dialog Updates

### Delete Filtered Confirmation

When clicking "Delete Filtered", a confirmation dialog appears:

```
┌────────────────────────────────────────────────────┐
│  ⚠️  Delete Filtered Messages                     │
├────────────────────────────────────────────────────┤
│                                                    │
│  Are you sure you want to DELETE ALL active       │
│  messages from 'myqueue' that match the current   │
│  filters?                                          │
│                                                    │
│  This will permanently delete matching messages   │
│  and cannot be undone.                            │
│                                                    │
│  Type 'DELETE' to confirm:                        │
│  [___________________]                             │
│                                                    │
│  [Cancel] [Delete Filtered]                       │
└────────────────────────────────────────────────────┘
```

**Note**: User must type "DELETE" (case-insensitive) to enable the confirmation button.

### Purge All Confirmation (existing - for comparison)

```
┌────────────────────────────────────────────────────┐
│  ⚠️  Purge All Messages                           │
├────────────────────────────────────────────────────┤
│                                                    │
│  Are you sure you want to PURGE ALL active        │
│  messages from 'myqueue'?                         │
│                                                    │
│  This will permanently delete ALL messages        │
│  and cannot be undone.                            │
│                                                    │
│  Type 'PURGE' to confirm:                         │
│  [___________________]                             │
│                                                    │
│  [Cancel] [Purge All]                             │
└────────────────────────────────────────────────────┘
```

**Note**: User must type "PURGE" (case-insensitive) to enable the confirmation button.

## Export Success Dialog (MAUI App Only)

When exporting in the MAUI desktop app, a success dialog shows the file location:

```
┌────────────────────────────────────────────────────┐
│  ✅  Export Successful                             │
├────────────────────────────────────────────────────┤
│                                                    │
│  File saved to:                                    │
│  C:\Users\YourName\Downloads\                      │
│  myqueue_active_messages_20241024_153045.json      │
│                                                    │
│                     [OK]                           │
└────────────────────────────────────────────────────┘
```

**Note**: This dialog only appears in the MAUI desktop app. In the web version, files download normally through the browser.

## Progress Overlay

When deleting filtered messages, a progress overlay is shown:

```
┌────────────────────────────────────────────────────┐
│                                                    │
│              Deleting Messages                     │
│                                                    │
│  Deleting filtered active messages from 'myqueue' │
│                                                    │
│           ⏳ Deleted: 42 messages                  │
│                                                    │
│              [Cancel Operation]                    │
│                                                    │
└────────────────────────────────────────────────────┘
```

**Features**:
- Real-time progress counter
- Cancellation button
- Full-screen semi-transparent overlay
- Blocks interaction during operation

## Success/Error Messages

### Success Messages (Top of page, green alert)

- **Export (Web)**: "Successfully exported 25 message(s) to myqueue_active_messages_20241024_153045.json."
- **Export (MAUI)**: File location shown in dialog box (see above)
- **Delete**: "Successfully deleted 42 filtered message(s)."
- **Cancel**: "Delete cancelled. Deleted 10 message(s) before cancellation."

Auto-dismisses after 3 seconds (web only).

### Error Messages (Top of page, red alert)

- **No Filters**: "No filters applied. Use 'Purge All' to delete all messages, or apply filters first."
- **No Matches**: "No messages matched the filters."
- **Export Error**: "Error exporting messages: [error message]"
- **Delete Error**: "Error deleting filtered messages: [error message]"

Auto-dismisses after 5 seconds.

## User Workflows

### Workflow 1: Export Filtered Messages

1. Apply one or more filters to narrow down messages
2. Verify filtered message count at bottom of filter panel
3. Click "Export Filtered" button (bright blue - now more visible!)
4. **Web**: File downloads automatically to browser's download folder
5. **MAUI**: File saves to Downloads folder, dialog shows exact location
6. Success message appears (web) or dialog with file path (MAUI)

**No confirmation required** - Export is non-destructive.

### Workflow 2: Delete Filtered Messages

1. Apply one or more filters to narrow down messages
2. Verify filtered message count at bottom of filter panel
3. Click "Delete Filtered" button (yellow)
4. Confirmation dialog appears
5. Type "DELETE" in the confirmation box
6. Click "Delete Filtered" button in dialog
7. Progress overlay shows deletion progress
8. Can cancel at any time
9. Message list refreshes automatically when complete
10. Entity counts update in tree view

### Workflow 3: Backup Before Delete

1. Apply filters to identify messages to delete
2. Click "Export Filtered" to backup (blue button)
3. Wait for download to complete
4. Click "Delete Filtered" (yellow button)
5. Confirm deletion by typing "DELETE"
6. Messages are deleted, but you have the backup

## Visual Hierarchy

### Button Importance (Left to Right)

```
Primary Actions: [Refresh] [Send New] [Export Filtered]  ← Now more prominent!
Warning Actions: [Delete Filtered]  ← Destructive but selective
Danger Actions:  [Purge All]        ← Most destructive
Close Action:    [Close]
```

The color coding helps users understand the risk level:
- **Bright Blue (Primary)**: Important but safe action (Export)
- **Green**: Add new items (Send New)
- **Yellow**: Caution, selective destructive action (Delete Filtered)
- **Red**: Danger, full destructive action (Purge All)

## Responsive Design

All buttons remain functional on mobile devices:
- Buttons wrap to multiple rows on small screens
- Touch targets are appropriately sized
- Confirmation dialogs are mobile-friendly

## Accessibility

- All buttons have descriptive tooltips
- Keyboard navigation supported
- Screen reader friendly labels
- Confirmation dialogs prevent accidental actions
- Progress indicators visible to all users

## Integration with Existing Features

The new buttons work seamlessly with:
- **Filters**: All filter types (Application Property, Enqueued Time, etc.)
- **Sorting**: Export includes sorted order
- **Pagination**: Delete/export work on all loaded messages
- **Message Types**: Active, Scheduled, Dead-Letter all supported
- **Entity Types**: Works for both queues and subscriptions
