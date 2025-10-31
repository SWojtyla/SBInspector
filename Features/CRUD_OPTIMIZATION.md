# CRUD Operations Optimization

## Overview

This feature optimizes CRUD (Create, Read, Update, Delete) operations on Service Bus messages by:
1. Eliminating redundant peek operations when sequence numbers are already known
2. Properly handling dead-letter queue operations by specifying the SubQueue parameter
3. Using a clean parameter object pattern instead of many individual parameters

## Problem Statement

Previously, there were two main issues:

1. **Redundant Peek Operations**: When performing CRUD operations like delete, requeue, move to dead-letter, or reschedule, the system would:
   - Peek messages to display them in the UI (getting sequence numbers)
   - When a user clicked an operation button, peek again to verify the message still exists
   - Then receive and perform the actual operation
   - This resulted in unnecessary network calls and slower operations

2. **Dead-Letter Queue Issues**: The deletion of messages in the dead-letter queue was not working correctly because the `SubQueue` parameter wasn't being specified

3. **Code Complexity**: Methods had too many parameters (6-7 parameters each), making them hard to use and maintain

## Solution

### 1. Parameter Object Pattern

Created a `MessageOperationOptions` class to encapsulate all operation parameters:

```csharp
public class MessageOperationOptions
{
    public string EntityName { get; set; }
    public long SequenceNumber { get; set; }
    public bool IsSubscription { get; set; }
    public string? TopicName { get; set; }
    public string? SubscriptionName { get; set; }
    public bool IsDeadLetterQueue { get; set; }  // NEW: Properly handles DLQ operations
    public bool SkipPeekVerification { get; set; }
}
```

This class includes a helper factory method:
```csharp
MessageOperationOptions.FromMessageInfo(message, entityName, isSubscription, topicName, subscriptionName)
```

### 2. Fixed Dead-Letter Queue Operations

The CRUD operations now properly set the `SubQueue` parameter when operating on dead-letter messages:

```csharp
var receiverOptions = new ServiceBusReceiverOptions
{
    ReceiveMode = ServiceBusReceiveMode.PeekLock
};

// Set SubQueue if it's a dead-letter queue
if (options.IsDeadLetterQueue)
{
    receiverOptions.SubQueue = SubQueue.DeadLetter;
}
```

The `IsDeadLetterQueue` flag is automatically determined from the message's `State` property when using the factory method.

### 3. Skip Peek Verification

The optimization adds skip peek verification capability that eliminates the verification peek when the sequence number is already known to be valid (from the UI display).

### Modified Methods

The following methods now use the `MessageOperationOptions` parameter object:

- `DeleteMessageAsync(MessageOperationOptions options)`
- `RequeueDeadLetterMessageAsync(MessageOperationOptions options)`
- `MoveActiveMessageToDeadLetterAsync(MessageOperationOptions options)`
- `RescheduleMessageAsync(MessageOperationOptions options, DateTime newScheduledTime)`

### How It Works

1. **UI Display**: Messages are fetched using `GetMessagesAsync()` or `GetSubscriptionMessagesAsync()` which performs a peek operation and stores information in `MessageInfo` objects (including the `State` which indicates if it's a dead-letter message)

2. **User Action**: When a user clicks delete, requeue, move to dead-letter, or reschedule on a message:
   ```csharp
   var options = MessageOperationOptions.FromMessageInfo(
       messageForOperation,
       selectedEntity,
       !string.IsNullOrEmpty(selectedSubscriptionName),
       selectedTopic,
       selectedSubscriptionName);
   
   success = await ServiceBusService.DeleteMessageAsync(options);
   ```

3. **Optimized Operation**: The service:
   - Automatically detects if it's a dead-letter message and sets `SubQueue` accordingly
   - Skips the verification peek (since `SkipPeekVerification` defaults to `true` in the factory method)
   - Directly attempts to receive and process the message using the known sequence number

## Performance Benefits

- **Reduced Network Calls**: Eliminates one peek operation per CRUD action (can save 5-20 peek API calls depending on message position)
- **Faster Operations**: Operations complete more quickly by skipping unnecessary verification
- **Lower Latency**: Reduced round-trips to Azure Service Bus improve user experience
- **Cleaner Code**: Reduced from 6-7 parameters to a single options object per method

## Usage Example

### Before (many parameters, no DLQ support)
```csharp
// Hard to read, error-prone, doesn't work for dead-letter messages
await ServiceBusService.DeleteMessageAsync(
    "my-queue", 
    message.SequenceNumber,
    isSubscription: false,
    topicName: null,
    subscriptionName: null,
    skipPeekVerification: true);
```

### After (clean, works for DLQ)
```csharp
// Clean, easy to read, automatically handles dead-letter queues
var options = MessageOperationOptions.FromMessageInfo(
    message, 
    "my-queue",
    isSubscription: false);
    
await ServiceBusService.DeleteMessageAsync(options);
```

## Implementation Details

### Code Changes

1. **New Domain Class** (`MessageOperationOptions.cs`): Parameter object with factory method
2. **Interface** (`IServiceBusService.cs`): Updated CRUD method signatures to use parameter object
3. **Implementation** (`ServiceBusService.cs`): 
   - Updated methods to use options parameter
   - Added SubQueue handling for dead-letter operations
   - Wrapped peek verification loops in conditional checks
4. **UI** (`Home.razor` in both Blazor Server and Shared): Updated all CRUD operation calls to use the factory method
5. **Tests** (`ServiceBusServiceTests.cs`): Updated all tests to use the new parameter object pattern

### Safety Considerations

While skipping peek verification is safe for UI-driven operations (where messages were just displayed), the operation may still fail if:
- The message was processed by another consumer
- The message expired or was moved
- Network issues occur

In these cases, the receive operation will simply return no results, and the UI will show an appropriate error message. This is the same behavior as before, just reached more quickly.

## Testing

All 40 unit tests pass, including tests that verify:
- Parameter object pattern works correctly
- Dead-letter queue flag is respected
- Skip peek verification is honored
- Subscription parameters are properly used

## Technical Notes

- The optimization is particularly beneficial when messages are deep in the queue, as it avoids paging through up to 5,120 messages (20 batches × 256 messages)
- The receive operation already handles cases where the message doesn't exist, making peek verification redundant in most scenarios
- This follows the principle of "fail fast" - try the operation and handle failure rather than pre-verifying
- The `IsDeadLetterQueue` flag is automatically determined from the message's `State` property, ensuring correct SubQueue handling

