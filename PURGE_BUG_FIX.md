# Purge Messages Bug Fix

## Problem Description

When purging "Active" messages from a queue or subscription, the purge operation was only deleting messages that were immediately available for delivery. Scheduled messages (messages with a future `ScheduledEnqueueTime`) were left behind, resulting in an incomplete purge.

### Example Scenario

If a queue had:
- 50 active messages (ready for delivery)
- 150 scheduled messages (scheduled for future delivery)

When purging "Active" messages, only the 50 active messages were deleted, leaving the 150 scheduled messages untouched.

## Root Cause

The `PurgeMessagesAsync` method used `ReceiveMessagesAsync` with `ReceiveAndDelete` mode to efficiently delete messages in batches. However, this approach has a fundamental limitation:

**`ReceiveMessagesAsync` only returns messages that are ready for immediate delivery.**

In Azure Service Bus:
- **Active messages**: Messages ready to be delivered now
- **Scheduled messages**: Messages with a future `ScheduledEnqueueTime` that remain hidden until that time arrives

The original implementation only handled the first type, causing scheduled messages to be skipped during purge operations.

## Solution

The fix enhances the `PurgeMessagesAsync` method to operate in two phases when purging "Active" or "Scheduled" messages:

### Phase 1: Delete Ready-to-Deliver Messages
```csharp
// Use ReceiveAndDelete mode for efficient deletion
var messages = await receiver.ReceiveMessagesAsync(maxMessages: 100, maxWaitTime: TimeSpan.FromSeconds(1));
// Messages are automatically deleted
```

### Phase 2: Cancel Scheduled Messages
```csharp
// Peek to find scheduled messages
var peekedMessages = await peekReceiver.PeekMessagesAsync(maxMessages: 100, fromSequenceNumber: fromSequenceNumber);

// Filter for scheduled messages
var scheduledMessages = peekedMessages
    .Where(m => m.ScheduledEnqueueTime > DateTimeOffset.UtcNow)
    .ToList();

// Cancel them using sender
await sender.CancelScheduledMessagesAsync(sequenceNumbers, cancellationToken);
```

## Technical Implementation

### Key Changes

1. **Added scheduled message handling**: When `messageType` is "Active" or "Scheduled", the method now performs an additional pass to find and cancel scheduled messages.

2. **Used PeekMessagesAsync**: This method can see all messages, including scheduled ones, without removing them from the queue.

3. **Used CancelScheduledMessagesAsync**: This is the proper way to delete scheduled messages before their scheduled time arrives.

4. **Maintained progress reporting**: Both phases report their progress, giving users accurate feedback on the total number of deleted messages.

5. **Preserved cancellation support**: The cancellation token is checked in both phases, allowing users to cancel the operation at any time.

### Azure Service Bus SDK Methods Used

- **ServiceBusReceiver.ReceiveMessagesAsync**: Receives and deletes ready-to-deliver messages
- **ServiceBusReceiver.PeekMessagesAsync**: Views all messages without removing them
- **ServiceBusSender.CancelScheduledMessagesAsync**: Cancels scheduled messages in batches

### Code Structure

```csharp
public async Task<int> PurgeMessagesAsync(...)
{
    // Phase 1: Delete ready-to-deliver messages
    while (true)
    {
        var messages = await receiver.ReceiveMessagesAsync(...);
        if (messages.Count == 0) break;
        totalDeleted += messages.Count;
        progress?.Report(totalDeleted);
    }

    // Phase 2: Cancel scheduled messages (for Active/Scheduled types)
    if (messageType == "Active" || messageType == "Scheduled")
    {
        while (true)
        {
            var peekedMessages = await peekReceiver.PeekMessagesAsync(...);
            if (peekedMessages.Count == 0) break;
            
            var scheduledMessages = peekedMessages
                .Where(m => m.ScheduledEnqueueTime > DateTimeOffset.UtcNow)
                .ToList();
            
            if (scheduledMessages.Count > 0)
            {
                await sender.CancelScheduledMessagesAsync(sequenceNumbers, ...);
                totalDeleted += scheduledMessages.Count;
                progress?.Report(totalDeleted);
            }
        }
    }

    return totalDeleted;
}
```

## Testing Recommendations

To verify the fix works correctly, test the following scenarios:

### Test 1: Queue with Mixed Messages
1. Create a queue
2. Send 50 regular messages
3. Send 50 scheduled messages (scheduled for 1 hour in the future)
4. Verify queue shows 50 active, 50 scheduled
5. Purge "Active" messages
6. Verify ALL 100 messages are deleted

### Test 2: Only Scheduled Messages
1. Create a queue
2. Send only scheduled messages (e.g., 100 messages scheduled for future)
3. Purge "Active" or "Scheduled" messages
4. Verify all messages are deleted

### Test 3: Progress Reporting
1. Start a purge with many messages (active + scheduled)
2. Observe the progress counter
3. Verify it counts both active and scheduled deletions

### Test 4: Cancellation
1. Start purging a large number of messages
2. Cancel mid-operation
3. Verify partial count is reported correctly
4. Verify operation stops cleanly

### Test 5: Subscription Purge
1. Repeat above tests for topic subscriptions
2. Verify the fix works for subscriptions as well

## Impact

### Before the Fix
- Purging "Active" messages left scheduled messages behind
- Users had to wait for scheduled messages to arrive before purging them
- Confusion about why message counts didn't reach zero after purge

### After the Fix
- Purging "Active" messages deletes ALL messages (both active and scheduled)
- Complete cleanup in a single operation
- Accurate progress reporting
- Better user experience

## Backward Compatibility

This fix maintains full backward compatibility:
- No changes to the method signature
- No changes to the interface
- Progress reporting continues to work as before
- Cancellation support unchanged
- Same behavior for "DeadLetter" message type

## Performance Considerations

- The fix adds a second phase that peeks and cancels scheduled messages
- For queues with few or no scheduled messages, the impact is minimal
- For queues with many scheduled messages, the total operation time increases proportionally
- Cancellation remains responsive throughout both phases
- Batching (100 messages at a time) maintains good performance

## Related Files

- `SBInspector/Infrastructure/ServiceBus/ServiceBusService.cs` - The main fix
- `SBInspector/Core/Interfaces/IServiceBusService.cs` - Interface (unchanged)
- `SBInspector/Presentation/Components/Pages/Home.razor` - UI that calls purge (unchanged)

## Security Review

- No security vulnerabilities introduced
- CodeQL analysis passed with 0 alerts
- Proper resource disposal maintained
- Exception handling preserved
