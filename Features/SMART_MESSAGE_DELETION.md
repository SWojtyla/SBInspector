# Smart Message Deletion

## Overview

SBInspector implements an intelligent message deletion system that automatically selects the best deletion strategy based on the message's position in the queue. This feature addresses the performance challenges of deleting messages from large Service Bus queues.

## The Problem

Azure Service Bus does not provide direct message deletion by sequence number for active messages. To delete a specific message, the application must:

1. Receive messages in batches using `ReceiveMessagesAsync`
2. Check each batch for the target message
3. Handle non-target messages appropriately
4. Complete (delete) the target message when found

**Initial Challenge:** When using `AbandonMessageAsync`, messages return to the front of the queue, causing `ReceiveMessagesAsync` to return the same messages repeatedly in an infinite loop.

**Solution:** Use `DeferMessageAsync` to temporarily set aside non-target messages. Deferred messages don't return via regular receive operations, allowing the search to progress forward through the queue. After finding and deleting the target message, deferred messages are retrieved using `ReceiveDeferredMessagesAsync` and abandoned to restore them to their original state.

For messages deep in a large queue (e.g., position 10,000+), this process can still take time, which is why background operations with progress tracking are used.

## The Solution

SBInspector implements a **smart hybrid deletion strategy** that automatically chooses between foreground and background operations based on the message's estimated position, and uses **message deferral** to efficiently navigate through the queue.

**Note:** The deferral approach is used for both single message deletion and filtered message deletion to avoid infinite loops and ensure all messages in the queue are properly examined.

### Architecture

```
User clicks Delete
        ↓
EstimateMessagePosition()
        ↓
    Position?
    ↙     ↘
≤ 1000      > 1000
    ↓           ↓
Foreground  Background
Operation   Operation
    ↓           ↓
Quick       Shows Progress
Delete      Can Cancel
    ↓           ↓
Defer non-target messages
Find & delete target
Restore deferred messages
```

### Components

#### 1. Message Position Estimation

**Method:** `EstimateMessagePositionAsync(MessageOperationOptions options)`

- Peeks up to 2,560 messages (10 batches × 256 messages)
- Returns exact position if message is found
- Returns estimated position if message is beyond the peek range
- Returns -1 on error

**Performance:** Very fast (~1-2 seconds) as it only peeks without locking messages.

#### 2. Foreground Deletion (Position ≤ 1000)

**Method:** `DeleteMessageAsync(MessageOperationOptions options)`

**When Used:**
- Messages in the first ~1,000 positions
- Expected completion time: < 10 seconds

**User Experience:**
- Shows loading overlay: "Deleting message at position ~X..."
- Blocks UI during operation
- Quick feedback on success/failure

**Technical Details:**
- Searches up to 10,000 messages (100 batches × 100 messages)
- Uses `DeferMessageAsync` to set aside non-target messages (prevents infinite loops)
- No delay needed between batches (deferred messages don't return)
- After deletion, restores deferred messages using `ReceiveDeferredMessagesAsync` and `AbandonMessageAsync`

**Deferral Approach:**
1. Receive batch of messages
2. Check for target message
3. If found: Complete (delete) it, defer others
4. If not found: Defer all messages
5. Continue to next batch (deferred messages won't be received again)
6. Once target is found/deleted, retrieve all deferred messages and abandon them

#### 3. Background Deletion (Position > 1000)

**Method:** `DeleteMessageWithProgressAsync(MessageOperationOptions options, CancellationToken, IProgress)`

**When Used:**
- Messages beyond position 1,000
- Messages where position estimation failed
- Expected completion time: Variable (could be minutes)

**User Experience:**
- Shows background operation panel in bottom-right corner
- Live progress: "Searched X batches (Y messages)..."
- Can cancel at any time
- Does not block UI
- User can continue working while deletion runs

**Technical Details:**
- Searches up to 50,000 messages (500 batches × 100 messages)
- Uses `DeferMessageAsync` to set aside non-target messages (prevents infinite loops)
- No delay needed between batches (deferred messages don't return)
- Reports progress after each batch
- Supports cancellation via CancellationToken
- After deletion, restores deferred messages using `ReceiveDeferredMessagesAsync` and `AbandonMessageAsync`
- Cleans up properly on cancellation or completion

**Deferral Approach (same as foreground):**
1. Receive batch of messages
2. Check for target message
3. If found: Complete (delete) it, defer others, break
4. If not found: Defer all messages
5. Report progress and continue to next batch
6. Once target is found/deleted or search exhausted, retrieve all deferred messages and abandon them

#### 4. Filtered Message Deletion

**Method:** `DeleteFilteredMessagesAsync(string entityName, string messageType, List<MessageFilter> filters, ...)`

**When Used:**
- Deleting multiple messages that match specific filter criteria
- User applies filters (e.g., property value, enqueued time) and clicks "Delete Filtered"
- Background operation with progress tracking

**Technical Details:**
- Uses the same defer/restore pattern as single message deletion
- Receives batches of 100 messages
- Applies filters to identify matching messages
- Deletes (completes) matching messages
- Defers non-matching messages to continue search
- After processing all messages, restores all deferred messages

**Deferral Approach:**
1. Receive batch of messages
2. Apply filters to identify matches
3. Complete (delete) all matching messages
4. Defer all non-matching messages
5. Continue to next batch (deferred messages won't be received again)
6. Once search is exhausted, retrieve all deferred messages and abandon them

**Why Deferral is Critical:**
Without deferral, using `AbandonMessageAsync` on non-matching messages causes them to return to the front of the queue, creating an infinite loop where the same non-matching messages are received repeatedly, preventing progress through the queue.

### Progress Reporting

The background deletion reports progress as a tuple:
```csharp
Progress<(int batchesSearched, int messagesProcessed)>
```

Example: `(5, 486)` means "Searched 5 batches, processed 486 messages"

This is displayed to the user as:
```
Searched 5 batches (486 messages)...
```

### Cancellation

Background deletions can be cancelled at any time:

1. User clicks "Cancel" button on the background operation panel
2. CancellationToken is triggered
3. Current batch completes processing (deferred messages will still be restored)
4. Operation terminates cleanly
5. User is notified of cancellation

## Configuration

The threshold for choosing deletion strategy is currently hardcoded:

```csharp
const int POSITION_THRESHOLD = 1000;
bool useBackgroundOperation = estimatedPosition > POSITION_THRESHOLD || estimatedPosition == -1;
```

### Customization

To adjust the threshold, modify the constant in `Home.razor` at line ~805:

```csharp
// Use background operation if message is beyond position 1000 or estimation failed
bool useBackgroundOperation = estimatedPosition > 1000 || estimatedPosition == -1;
```

**Recommendations:**
- **Lower threshold (e.g., 500)**: More operations run in background, better for large queues
- **Higher threshold (e.g., 2000)**: More operations run in foreground, faster for messages near front
- **Consider message size**: Larger messages may need lower threshold due to network latency

## API Usage

### Service Layer

```csharp
// Estimate position (fast, read-only)
int position = await serviceBusService.EstimateMessagePositionAsync(options);

if (position <= 1000)
{
    // Foreground deletion
    bool success = await serviceBusService.DeleteMessageAsync(options);
}
else
{
    // Background deletion with progress
    var progress = new Progress<(int batches, int messages)>(tuple =>
    {
        Console.WriteLine($"Searched {tuple.batches} batches ({tuple.messages} messages)");
    });
    
    var cts = new CancellationTokenSource();
    bool success = await serviceBusService.DeleteMessageWithProgressAsync(
        options,
        cancellationToken: cts.Token,
        progress: progress);
}
```

### UI Integration

The UI automatically handles strategy selection in `Home.razor`:

```csharp
private async Task ConfirmDelete()
{
    // ...
    int estimatedPosition = await ServiceBusService.EstimateMessagePositionAsync(options);
    bool useBackgroundOperation = estimatedPosition > 1000 || estimatedPosition == -1;
    
    if (useBackgroundOperation)
    {
        // Set up background operation with BackgroundOperationPanel
    }
    else
    {
        // Use foreground operation with OperationLoadingOverlay
    }
}
```

## Performance Characteristics

### Position Estimation
- **Time:** 1-2 seconds
- **API Calls:** Up to 10 peek operations
- **Messages Checked:** Up to 2,560
- **Impact:** Minimal (peek is lightweight)

### Foreground Deletion
- **Best Case:** 1-2 seconds (message in first batch)
- **Average Case:** 5-10 seconds (position 100-500)
- **Worst Case:** 30-60 seconds (position 1000)
- **Batch Size:** 100 messages
- **Max Search:** 10,000 messages
- **Delay Between Batches:** 500ms

### Background Deletion
- **Best Case:** 30 seconds (position 1001-1500)
- **Average Case:** 2-5 minutes (position 5000-10000)
- **Worst Case:** 10+ minutes (position 25000+)
- **Batch Size:** 100 messages
- **Max Search:** 50,000 messages
- **Delay Between Batches:** 300ms

## Limitations

### Azure Service Bus SDK Limitations

1. **No direct sequence number deletion**: Active messages cannot be deleted by sequence number directly
2. **ReceiveDeferredMessageAsync doesn't help**: Only works for explicitly deferred messages
3. **Batch receive limitations**: Maximum 100 messages per receive operation
4. **Lock expiration**: Messages have a lock timeout (default 60 seconds)

### Application Limitations

1. **Search limit**: Won't find messages beyond position 50,000 in background mode
2. **Concurrent access**: Other applications receiving messages can interfere with position
3. **Message ordering**: Service Bus doesn't guarantee FIFO, affecting position accuracy
4. **Network latency**: Performance depends on network speed to Azure

## Best Practices

### For Users

1. **Check message position**: Look at the message's position in the list before deleting
2. **Use filters**: For bulk deletions, use filtered deletion instead of individual deletions
3. **Background operations**: Monitor the background panel for progress
4. **Cancellation**: Don't hesitate to cancel if it's taking too long
5. **Verify after deletion**: Refresh the message list to confirm deletion

### For Developers

1. **Always estimate first**: Use `EstimateMessagePositionAsync` before deletion
2. **Respect thresholds**: Don't hardcode deletion strategy without checking position
3. **Handle cancellation**: Always support CancellationToken in long operations
4. **Report progress**: Keep users informed during long operations
5. **Clean up resources**: Properly dispose receivers and abandon messages
6. **Test with large queues**: Test performance with queues containing 10,000+ messages

## Troubleshooting

### "Failed to delete message. It may have already been processed."

**Causes:**
- Message was processed by another application
- Message lock expired during search
- Message was auto-deleted by TTL
- Search limit was reached without finding the message

**Solutions:**
- Refresh the message list to verify the message still exists
- Increase the search limit in the code
- Check if other applications are processing the queue
- Use background operation for deep messages

### "Deletion cancelled" message appears unexpectedly

**Causes:**
- User clicked cancel
- Application lost connection to Service Bus
- CancellationToken was triggered externally

**Solutions:**
- Check network connectivity
- Retry the deletion
- Verify Service Bus connection status

### Background deletion is too slow

**Causes:**
- Message is very deep in the queue
- Large batch size causing network delays
- Service Bus throttling

**Solutions:**
- Reduce the batch size (trade-off: more API calls)
- Reduce the delay between batches (trade-off: more throttling risk)
- Consider using filtered deletion for bulk operations
- Increase the foreground/background threshold

## Future Enhancements

Potential improvements to consider:

1. **Adaptive batch sizing**: Dynamically adjust batch size based on message size and network latency
2. **Parallel searching**: Use multiple receivers to search different ranges concurrently
3. **Position caching**: Cache message positions after initial list load
4. **Smart retry**: Exponential backoff for transient failures
5. **Bulk operations**: Optimize deletion of multiple messages in one operation
6. **Telemetry**: Track deletion performance metrics for tuning
7. **Configurable thresholds**: Allow users to set their own position threshold
8. **Position indicator**: Show estimated position in the message list UI

## Related Features

- [Filtered Message Deletion](./FILTERING.md) - For bulk deletions based on filters
- [Purge Operations](./PURGE.md) - For deleting all messages
- [Background Operations](./BACKGROUND_OPERATIONS.md) - General background operation framework

## See Also

- [Azure Service Bus Receiver API](https://learn.microsoft.com/en-us/dotnet/api/azure.messaging.servicebus.servicebusreceiver)
- [Message Browsing in Service Bus](https://learn.microsoft.com/en-us/azure/service-bus-messaging/message-browsing)
- [Service Bus Best Practices](https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-performance-improvements)
