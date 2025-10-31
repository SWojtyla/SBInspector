# CRUD Operations Optimization

## Overview

This feature optimizes CRUD (Create, Read, Update, Delete) operations on Service Bus messages by eliminating redundant peek operations. When messages are already displayed in the UI (from an initial peek operation), their sequence numbers are known and can be used directly for operations without re-peeking.

## Problem Statement

Previously, when performing CRUD operations like delete, requeue, move to dead-letter, or reschedule, the system would:
1. Peek messages to display them in the UI (getting sequence numbers)
2. When a user clicked an operation button, peek again to verify the message still exists
3. Then receive and perform the actual operation

This resulted in unnecessary network calls and slower operations.

## Solution

The optimization adds an optional `skipPeekVerification` parameter to CRUD methods that allows bypassing the verification peek when the sequence number is already known to be valid (from the UI display).

### Modified Methods

The following methods in `IServiceBusService` and `ServiceBusService` now support the optimization:

- `DeleteMessageAsync(..., bool skipPeekVerification = false)`
- `RequeueDeadLetterMessageAsync(..., bool skipPeekVerification = false)`
- `MoveActiveMessageToDeadLetterAsync(..., bool skipPeekVerification = false)`
- `RescheduleMessageAsync(..., bool skipPeekVerification = false)`

### How It Works

1. **UI Display**: Messages are fetched using `GetMessagesAsync()` or `GetSubscriptionMessagesAsync()` which performs a peek operation and stores sequence numbers in `MessageInfo` objects
2. **User Action**: When a user clicks delete, requeue, move to dead-letter, or reschedule on a message, the UI passes `skipPeekVerification: true`
3. **Optimized Operation**: The service skips the verification peek and directly attempts to receive and process the message using the known sequence number

### Backward Compatibility

The parameter defaults to `false`, preserving the original behavior for any code that doesn't explicitly set it. This ensures:
- Existing integrations continue to work unchanged
- New code can opt-in to the optimization
- Direct API usage (not from UI) can still use verification if needed

## Performance Benefits

- **Reduced Network Calls**: Eliminates one peek operation per CRUD action (can save 5-20 peek API calls depending on message position)
- **Faster Operations**: Operations complete more quickly by skipping unnecessary verification
- **Lower Latency**: Reduced round-trips to Azure Service Bus improve user experience

## Usage Example

### Before (with peek verification)
```csharp
// Performs peek verification, then receive and delete
await ServiceBusService.DeleteMessageAsync(
    "my-queue", 
    message.SequenceNumber);
```

### After (optimized)
```csharp
// Skips peek verification, directly receives and deletes
await ServiceBusService.DeleteMessageAsync(
    "my-queue", 
    message.SequenceNumber,
    skipPeekVerification: true);
```

## Implementation Details

### Code Changes

1. **Interface** (`IServiceBusService.cs`): Added `skipPeekVerification` parameter to CRUD method signatures
2. **Implementation** (`ServiceBusService.cs`): Wrapped peek verification loops in conditional checks
3. **UI** (`Home.razor`): Updated all CRUD operation calls to pass `skipPeekVerification: true`
4. **Tests** (`ServiceBusServiceTests.cs`): Added 4 new tests to verify the parameter is accepted correctly

### Safety Considerations

While skipping peek verification is safe for UI-driven operations (where messages were just displayed), the operation may still fail if:
- The message was processed by another consumer
- The message expired or was moved
- Network issues occur

In these cases, the receive operation will simply return no results, and the UI will show an appropriate error message. This is the same behavior as before, just reached more quickly.

## Testing

New unit tests verify:
- `DeleteMessageAsync_WithSkipPeekVerification_SkipsPeekStep`
- `RequeueDeadLetterMessageAsync_WithSkipPeekVerification_SkipsPeekStep`
- `MoveActiveMessageToDeadLetterAsync_WithSkipPeekVerification_SkipsPeekStep`
- `RescheduleMessageAsync_WithSkipPeekVerification_SkipsPeekStep`

All 40 tests pass (36 existing + 4 new).

## Technical Notes

- The optimization is particularly beneficial when messages are deep in the queue, as it avoids paging through up to 5,120 messages (20 batches × 256 messages)
- The receive operation already handles cases where the message doesn't exist, making peek verification redundant in most scenarios
- This follows the principle of "fail fast" - try the operation and handle failure rather than pre-verifying
