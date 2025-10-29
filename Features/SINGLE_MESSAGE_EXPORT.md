# Single Message Export Feature

## Overview

This feature adds the ability to export an individual message from the message list. Previously, users could only export multiple messages using the "Export Filtered" button, which exports all messages matching the current filter criteria.

## How to Use

### Exporting a Single Message

1. Navigate to a queue, subscription, or dead-letter queue in the SBInspector application
2. View the message list for the selected entity
3. Locate the message you want to export in the table
4. Click the **Export** button (download icon) in the Actions column for that specific message
5. The message will be downloaded as a JSON file to your Downloads folder

The exported file will be named using the pattern:
```
{entity-name}_{message-type}_message_{message-id}_{timestamp}.json
```

**Note:** Special characters (such as `/`) in entity names are automatically replaced with underscores (`_`) to ensure filesystem compatibility.

For example:
- `my-queue_active_message_abc123def456_20250129_143000.json`
- `my-topic_my-subscription_deadletter_message_xyz789abc123_20250129_143015.json`

### Export Format

The exported file contains a single message in JSON format with the following structure:

```json
{
  "messageId": "abc123def456...",
  "subject": "OrderCreated",
  "contentType": "application/json",
  "body": "{ ... message body content ... }",
  "enqueuedTime": "2025-01-29T14:30:00Z",
  "scheduledEnqueueTime": null,
  "sequenceNumber": 12345,
  "deliveryCount": 0,
  "properties": {
    "customProperty1": "value1",
    "customProperty2": "value2"
  },
  "state": "Active"
}
```

## User Interface Changes

The message actions in the message list table now include:
- **View** (eye icon) - View message details
- **Export** (download icon) - **NEW** - Export this single message
- **Requeue** (arrow icon) - Available for dead-letter messages
- **Reschedule** (clock icon) - Available for scheduled messages
- **Delete** (trash icon) - Delete the message

## Use Cases

This feature is particularly useful for:

1. **Debugging**: Export a specific problematic message for detailed analysis
2. **Message Archival**: Save individual important messages for record-keeping
3. **Testing**: Extract a single message to use as test data
4. **Integration**: Export a message to share with other teams or systems
5. **Documentation**: Save example messages for documentation purposes

## Comparison with Export Filtered

| Feature | Single Message Export | Export Filtered |
|---------|----------------------|-----------------|
| Button Location | Per-message in the table | Top panel toolbar |
| Number of Messages | Exports 1 message | Exports all filtered messages |
| File Naming | Includes message ID | Generic timestamp |
| Use Case | Specific message analysis | Bulk export/backup |

## Technical Implementation

### Components Modified

1. **MessageListTable.razor**
   - Added Export button to the actions column
   - Added `OnExport` EventCallback parameter

2. **MessagesPanel.razor**
   - Added `OnExportMessage` EventCallback parameter
   - Wired the callback to MessageListTable

3. **Home.razor**
   - Implemented `HandleExportSingleMessage(MessageInfo message)` method
   - Reuses existing `IFileExportService` infrastructure
   - Serializes single message to JSON with proper formatting

### Export Process

1. User clicks Export button for a message
2. Event propagates through: MessageListTable → MessagesPanel → Home.razor
3. `HandleExportSingleMessage()` serializes the message to JSON
4. Filename is generated with message ID (truncated to 20 characters if longer for filesystem compatibility and readability while maintaining uniqueness)
5. `IFileExportService.ExportToFileAsync()` handles platform-specific download
6. Success/error message displayed to user for 3 seconds

### Platform Compatibility

The export functionality works on both:
- **Blazor Server** (web application) - Uses JavaScript interop to trigger browser download
- **.NET MAUI** (Windows desktop) - Uses native file save dialog

Both implementations use the same `IFileExportService` interface, ensuring consistent behavior across platforms.
