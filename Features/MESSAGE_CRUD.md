# Message CRUD Operations

SBInspector now supports full CRUD (Create, Read, Update, Delete) operations for Azure Service Bus messages, including bulk operations, allowing you to manage messages across queues, topics, and subscriptions.

## Features Overview

### 1. Delete Messages
Remove unwanted messages from active queues, dead-letter queues, or topic subscriptions.

**Use Cases:**
- Clean up test messages
- Remove corrupted or invalid messages
- Clear out specific messages that are no longer needed

**How to Use:**
1. Navigate to a queue or subscription
2. Click the **red trash icon** (🗑️) next to the message you want to delete
3. Confirm the deletion in the confirmation dialog
4. The message will be permanently removed and the list will update automatically

**Notes:**
- Deletion is permanent and cannot be undone
- The message must be available (not locked by another receiver)
- Works for Active, Scheduled, and Dead-letter messages
- The message list updates dynamically when a message is deleted

### 2. Purge All Messages (NEW)
Delete all messages from a queue or topic in a single operation.

**Use Cases:**
- Clear out all messages from a test queue
- Clean up dead-letter queue after resolving issues
- Remove all scheduled messages
- Reset a queue to empty state

**How to Use:**
1. Navigate to a queue or subscription
2. Click the **red "Purge All" button** in the message panel header
3. Read the confirmation dialog carefully - it shows the count and type of messages
4. Type 'PURGE' to confirm (case-sensitive)
5. All messages will be deleted and the list will be cleared

**Notes:**
- **WARNING**: This operation is permanent and cannot be undone
- Deletes ALL messages of the selected type (Active, Scheduled, or Dead-letter)
- Uses efficient batch deletion (processes up to 100 messages at a time)
- Returns the total count of deleted messages
- Locked messages will not be deleted
- Works for both queues and topic subscriptions
- The message list is cleared immediately after successful purge

### 3. Requeue Dead-Letter Messages
Move messages from the dead-letter queue back to the active queue for reprocessing.

**Use Cases:**
- Retry messages that failed due to temporary issues
- Reprocess messages after fixing application bugs
- Recover messages that were incorrectly dead-lettered

**How to Use:**
1. Navigate to a queue or subscription's **Dead-letter** messages
2. Click the **green requeue icon** (↻) next to the message
3. Confirm the requeue operation
4. The message will be removed from the dead-letter queue and sent to the active queue
5. The message list will update automatically

**Notes:**
- The original message is removed from the dead-letter queue
- A new message with the same content is sent to the active queue
- All application properties are preserved
- The message will have a new sequence number in the active queue

### 4. Send New Messages
Create and send new messages to queues or topics.

**Use Cases:**
- Test your message processing logic
- Manually inject messages into your workflow
- Create scheduled messages for future processing
- Simulate production scenarios

**How to Use:**
1. Navigate to a queue or subscription
2. Click the **green "Send New" button** (+ Send New)
3. Fill in the message details:
   - **Message Body** (required): The content of your message
   - **Subject** (optional): A label or title for the message
   - **Content Type** (optional): e.g., `application/json`, `text/plain`
   - **Schedule for later delivery** (optional): Check this to schedule the message
   - **Scheduled Enqueue Time** (when scheduling): Select the date and time
   - **Application Properties** (optional): Key-value pairs in the format `key=value` (one per line)
4. Click **Send** to send the message immediately or schedule it for later

**Example Application Properties:**
```
Priority=High
OrderId=12345
CustomerName=John Doe
ProcessingDeadline=2024-01-15T10:00:00
```

**Notes:**
- Scheduled messages will appear in the "Scheduled Messages" view
- Messages sent to topics will be delivered to all subscriptions
- All properties are optional except the message body

### 5. Reschedule Scheduled Messages
Change the scheduled enqueue time for messages that are waiting to be delivered.

**Use Cases:**
- Delay message processing due to maintenance windows
- Adjust timing for scheduled events
- Move messages to a more appropriate time slot

**How to Use:**
1. Navigate to a queue or subscription's **Scheduled** messages
2. Click the **yellow clock icon** (🕐) next to the message
3. Select a new scheduled enqueue time
4. Click **Reschedule**
5. The message will be removed and rescheduled with the new time

**Notes:**
- The original scheduled message is cancelled
- A new message with the same content is scheduled for the new time
- All application properties are preserved
- The message will have a new sequence number

## UI Elements

### Action Buttons
Each message in the message list has action buttons based on its state:

- **View Details** (blue eye icon 👁️): Available for all messages - opens detailed view
- **Delete** (red trash icon 🗑️): Available for all messages - permanently removes the message
- **Requeue** (green requeue icon ↻): Only for dead-letter messages - moves back to active queue
- **Reschedule** (yellow clock icon 🕐): Only for scheduled messages - changes delivery time

### Panel Header Buttons
- **Send New** (green button with +): Create and send a new message to the queue/topic
- **Purge All** (red button): Delete ALL messages from the current view (Active, Scheduled, or Dead-letter)

### Notifications
Success and error notifications appear at the top of the screen:
- **Green notifications**: Operations completed successfully
- **Red notifications**: Operations failed or encountered errors

Notifications automatically disappear after a few seconds.

## Technical Implementation

### Service Methods

The CRUD operations are implemented in `ServiceBusService` class:

```csharp
// Delete a message
Task<bool> DeleteMessageAsync(string entityName, long sequenceNumber, ...)

// Requeue from dead-letter queue
Task<bool> RequeueDeadLetterMessageAsync(string entityName, long sequenceNumber, ...)

// Send a new message
Task<bool> SendMessageAsync(string entityName, string messageBody, ...)

// Reschedule a scheduled message
Task<bool> RescheduleMessageAsync(string entityName, long sequenceNumber, DateTime newScheduledTime, ...)

// Purge all messages
Task<int> PurgeMessagesAsync(string entityName, string messageType, ...)
```

### Azure Service Bus SDK Operations

- **Delete**: Uses `ReceiveMessagesAsync` + `CompleteMessageAsync` to acquire lock and complete (delete) the message
- **Purge**: Uses `ReceiveAndDelete` mode to efficiently delete all messages in batches of 100
- **Requeue**: Uses `ReceiveMessagesAsync` from DLQ + `SendMessageAsync` to new queue + `CompleteMessageAsync` on DLQ message
- **Send**: Uses `SendMessageAsync` or `ScheduleMessageAsync` depending on whether scheduling is required
- **Reschedule**: Similar to requeue - receives old message, schedules new one, completes old one

### Dynamic UI Updates

- Single message deletion: Uses `messages.Remove(messageForOperation)` to update the list
- Purge operation: Uses `messages.Clear()` to empty the list
- Blazor's data binding automatically refreshes the UI when the collection changes

## Limitations and Considerations

1. **Message Locks**: Operations require obtaining a lock on the message. If another receiver has locked the message, the operation may fail.

2. **Sequence Numbers**: Messages are identified by sequence number. After certain operations (requeue, reschedule), messages get new sequence numbers.

3. **PeekLock Mode**: Most operations use PeekLock mode to ensure safe message handling. Purge uses ReceiveAndDelete mode for efficiency.

4. **Batch Operations**: 
   - Single delete works on individual messages
   - Purge operation processes up to 100 messages per batch automatically
   - Locked messages during purge will not be deleted

5. **Scheduled Messages**: When rescheduling or sending scheduled messages, ensure the time is in the future.

6. **Topic Messages**: When sending to a topic, the message is delivered to all subscriptions with matching filters.

7. **Purge Performance**: Large queues may take time to purge as messages are deleted in batches. The operation will continue until no more messages are found.

## Error Handling

If an operation fails, you'll see an error notification with details. Common reasons for failure:

- Message already processed or deleted by another receiver
- Message lock expired (message was being viewed too long)
- Connection issues with Azure Service Bus
- Insufficient permissions on the Service Bus namespace
- Invalid message format or size exceeds limits

## Best Practices

1. **Test First**: Always test CRUD operations in a development environment before using in production
2. **Backup Important Messages**: Before deleting or purging, consider saving message content if you might need it later
3. **Use Purge Carefully**: The purge operation is powerful but destructive - double-check which message type (Active/Scheduled/Dead-letter) you're viewing before purging
4. **Monitor Results**: Watch for success/error notifications to confirm operations completed
5. **Scheduled Time Zones**: Be aware of time zones when scheduling messages - times are in your local time
6. **Application Properties**: Use consistent naming and formatting for application properties
7. **Permissions**: Ensure your Service Bus connection has appropriate permissions (Manage for all operations)
8. **Dynamic Updates**: The message list updates automatically when you delete or purge messages - no need to refresh manually

## Troubleshooting

**"Failed to delete message. It may have already been processed."**
- The message was locked by another receiver or already consumed
- Try refreshing the message list and check if the message still exists

**"Failed to requeue message."**
- Similar to delete - message may be locked or already processed
- Verify the message is still in the dead-letter queue

**"No messages were found to purge."**
- The queue or subscription is already empty
- Ensure you're viewing the correct message type (Active/Scheduled/Dead-letter)

**"Error purging messages: ..."**
- Check your connection to Service Bus is still active
- Verify you have appropriate permissions (Manage rights required)
- Some messages may be locked by other receivers

**"Error sending message: Message size exceeded."**
- Your message body or properties are too large
- Standard tier: 256 KB limit, Premium tier: 1 MB limit
- Reduce message size or upgrade namespace tier

**"Scheduled time must be in the future."**
- Select a date and time that hasn't passed yet
- Check your system time and time zone settings

**Messages not updating in the list:**
- This should not happen as the UI updates dynamically
- Try closing and reopening the message panel
- Verify your connection to Service Bus is still active
