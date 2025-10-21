# Scheduled Messages Feature

## Overview

This feature enables users to view and manage scheduled messages in Azure Service Bus topic subscriptions. Scheduled messages are messages that have been sent with a future delivery time and will be delivered to the subscription only when that time arrives.

## Feature Details

### Subscription Scheduled Messages

**Viewing Scheduled Messages:**
- When you select a subscription from the tree view, the details panel now displays three message type cards:
  1. **Active Messages** - Shows count and allows viewing currently available messages
  2. **Scheduled Messages** - Allows viewing messages scheduled for future delivery (count shown as "-" since Azure SDK doesn't provide this count without peeking all messages)
  3. **Dead Letter Messages** - Shows count and allows viewing messages that failed processing

**How to View Scheduled Messages:**
1. Connect to your Azure Service Bus namespace
2. Navigate to a topic in the tree view on the left
3. Expand the topic to see its subscriptions
4. Click on a subscription
5. In the details panel, click the "View" button on the **Scheduled Messages** card
6. The messages panel will display all scheduled messages for that subscription

### Topic Aggregated View

**Message Count Summary:**
When you select a topic (without drilling into a subscription), the details panel shows aggregated message counts across all subscriptions:
- **Active Messages** - Total active messages across all subscriptions
- **Dead Letter Messages** - Total dead-letter messages across all subscriptions

Note: Scheduled message counts are not shown at the topic level because Azure Service Bus SDK does not provide `ScheduledMessageCount` for subscriptions (only for queues). To view scheduled messages, you must select individual subscriptions.

### Managing Scheduled Messages

Once you view scheduled messages, you can:

1. **View Details** - Click on any message row or the eye icon to see full message details including:
   - Message ID, Subject, Content Type
   - Message body
   - **Scheduled Enqueue Time** - When the message will be delivered
   - Sequence number and delivery count
   - Application properties

2. **Reschedule Message** - Click the clock icon to change when the message will be delivered
   - Opens a modal dialog where you can set a new scheduled delivery time
   - The message will be removed from the current queue and re-sent with the new schedule

3. **Delete Message** - Click the trash icon to permanently remove a scheduled message
   - Useful for canceling messages that are no longer needed

4. **Send New Scheduled Message** - Click "Send New" button to create a new scheduled message
   - You can specify the message content, properties, and scheduled delivery time

5. **Purge All Scheduled Messages** - Click "Purge All" to delete all scheduled messages at once
   - Requires confirmation to prevent accidental deletion

## Technical Implementation

### Architecture

The implementation follows the existing clean architecture pattern:

**Domain Layer:**
- `SubscriptionInfo` - Contains subscription metadata (Active and DeadLetter counts only, as ScheduledMessageCount is not available in Azure SDK for subscriptions)

**Infrastructure Layer:**
- `ServiceBusService.GetSubscriptionMessagesAsync()` - Retrieves messages from subscription, including scheduled messages
- Messages are retrieved using `PeekMessagesAsync()` which returns all message types based on the receiver configuration

**Presentation Layer:**
- `EntityDetailsPanel.razor` - Updated to show the Scheduled Messages card for subscriptions
- `MessagesPanel.razor` - Displays scheduled messages with reschedule functionality
- `MessageListTable.razor` - Shows reschedule button for scheduled messages
- `Home.razor` - Coordinates the message viewing workflow

### Key Design Decisions

1. **No Count for Subscription Scheduled Messages**: Unlike queues, Azure Service Bus's `SubscriptionRuntimeProperties` does not include a `ScheduledMessageCount` property. Therefore, the count is shown as "-" and users must click "View" to see the actual messages.

2. **Consistent with Queue Behavior**: The implementation mirrors how scheduled messages work for queues, ensuring a consistent user experience.

3. **Aggregated Topic View**: Topics show aggregate counts for Active and DeadLetter messages across all subscriptions, but not scheduled messages (since counts aren't available per subscription).

## Usage Examples

### Example 1: Viewing Scheduled Messages

```
1. Select a topic (e.g., "orders-topic")
2. Expand to see subscriptions (e.g., "processing-subscription")
3. Click on "processing-subscription"
4. Click "View" on the Scheduled Messages card
5. View list of all scheduled messages with their scheduled delivery times
```

### Example 2: Rescheduling a Message

```
1. View scheduled messages (as above)
2. Click the clock icon on a scheduled message
3. In the modal, enter a new scheduled time
4. Click "Reschedule"
5. The message will be removed and re-sent with the new schedule
```

### Example 3: Canceling a Scheduled Message

```
1. View scheduled messages
2. Click the trash icon on the message you want to cancel
3. Confirm deletion
4. The message will be permanently removed
```

## Limitations

- **No real-time count**: The Scheduled Messages card shows "-" instead of a count because the Azure SDK doesn't provide this information without peeking all messages
- **Peek-based retrieval**: Scheduled messages are retrieved using peek operations, which means very large numbers of scheduled messages may require pagination
- **Subscription-level only**: You must view scheduled messages per subscription; there's no topic-level aggregated view of scheduled messages

## Related Features

- **Queue Scheduled Messages**: Queues also support scheduled messages, with the advantage that the count is displayed (via `QueueRuntimeProperties.ScheduledMessageCount`)
- **Message Filtering**: You can apply filters to scheduled messages based on their application properties
- **Message Pagination**: Large sets of scheduled messages can be loaded incrementally using the "Load More" button

## See Also

- [MESSAGE_CRUD.md](MESSAGE_CRUD.md) - Documentation on message CRUD operations including rescheduling
- [TREE_VIEW_NAVIGATION.md](TREE_VIEW_NAVIGATION.md) - How to navigate the tree view to access topics and subscriptions
- [Azure Service Bus Scheduled Messages](https://learn.microsoft.com/en-us/azure/service-bus-messaging/message-sequencing) - Official Microsoft documentation
