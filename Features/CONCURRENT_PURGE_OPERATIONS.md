# Concurrent Purge Operations Fix

## Overview
Fixed a critical null reference exception and added support for multiple concurrent purge operations in the Service Bus Inspector application.

## Problem Statement
When two purge operations were initiated quickly (e.g., one for dead-letter messages and one for active messages on the same queue), the application would crash with a null reference exception. Additionally, only one background operation panel would be shown, not multiple panels for concurrent operations.

## Root Cause
The original implementation used a single `purgeCancellationTokenSource` field to track purge operations. When a second purge was initiated before the first completed:
1. The cancellation token source would be overwritten, causing the first operation to lose its reference
2. When the first operation completed and tried to check `purgeCancellationTokenSource.Token.IsCancellationRequested`, the reference could be null or point to a different operation's token
3. Only one `BackgroundOperationPanel` component was rendered, limiting visibility to a single operation

## Solution
Implemented a robust multi-operation tracking system that supports concurrent purge operations:

### Key Changes

#### 1. PurgeOperation Data Structure
Created a dedicated class to encapsulate all state for a single purge operation:
```csharp
private class PurgeOperation
{
    public string OperationId { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string MessageType { get; set; } = string.Empty;
    public string ProgressMessage { get; set; } = string.Empty;
    public int Progress { get; set; }
    public CancellationTokenSource CancellationTokenSource { get; set; } = new();
    public bool IsDeleteFiltered { get; set; }
}
```

#### 2. Multiple Operation Tracking
Replaced single-operation state with a dictionary to track multiple operations:
```csharp
private Dictionary<string, PurgeOperation> activeOperations = new();
```

Each operation gets a unique ID based on:
- Entity name (queue/topic/subscription)
- Message type (Active/DeadLetter/Scheduled)
- Timestamp (to ensure uniqueness)

#### 3. Multiple Background Panels
Updated the UI to render one `BackgroundOperationPanel` for each active operation:
```razor
@foreach (var operation in activeOperations.Values)
{
    var operationId = operation.OperationId;
    <BackgroundOperationPanel
        IsVisible="true"
        Title="@(operation.IsDeleteFiltered ? "Deleting Filtered Messages" : "Purging Messages")"
        Message="@operation.ProgressMessage"
        Progress="@operation.Progress"
        OnCancel="@(() => CancelPurgeOperation(operationId))" />
}
```

#### 4. Null-Safe Cancellation Checks
All cancellation token checks now use null-safe operators:
```csharp
bool wasCancelled = operation.CancellationTokenSource?.Token.IsCancellationRequested ?? false;
```

#### 5. Proper Cleanup
Enhanced cleanup in multiple locations:
- `Dispose()` - Cleans up all active operations when component is disposed
- `Disconnect()` - Cancels and cleans up all operations when disconnecting
- `ClearAllDataAndReload()` - Cancels operations during connection switching

### Backward Compatibility
Maintained legacy single-operation state fields for backward compatibility:
- `isPurgeInProgress`
- `purgeProgress`
- `purgeCancellationTokenSource`
- `purgeProgressMessage`

These are synchronized with the new multi-operation tracking system.

## Benefits

### 1. No More Crashes
Null reference exceptions are completely eliminated through:
- Null-safe operator usage (`?.`)
- Proper null checks before accessing cancellation tokens
- Defensive programming practices

### 2. Multiple Concurrent Operations
Users can now:
- Start multiple purge operations simultaneously
- See separate progress panels for each operation
- Cancel individual operations independently

### 3. Better UX
- Each operation is clearly identified in its own panel
- Operation-specific messages show which queue/topic and message type is being purged
- Multiple panels appear in the bottom-right corner, stacked vertically

### 4. Robust State Management
- Each operation maintains isolated state
- Entity context is captured at operation start to prevent issues when user switches views
- Proper cleanup ensures no memory leaks

## Testing

### Manual Testing Scenarios
1. **Concurrent Purge Test**
   - Select a queue with both active and dead-letter messages
   - Click "View Active Messages" → "Purge All"
   - Quickly click "View Dead-Letter Messages" → "Purge All"
   - Expected: Two separate background panels appear, each showing its own progress

2. **Null Safety Test**
   - Start a purge operation
   - Let it complete
   - Verify no null reference exceptions occur

3. **Cancellation Test**
   - Start multiple purge operations
   - Cancel one or more individually
   - Verify correct operations are cancelled
   - Verify remaining operations continue

4. **Cleanup Test**
   - Start multiple purge operations
   - Disconnect from Service Bus
   - Verify all operations are cancelled and cleaned up

### Automated Testing
All existing 36 unit tests continue to pass, confirming no regressions.

## Files Modified
- `/SBInspector.Shared/Presentation/Components/Pages/Home.razor` - Main implementation
- `/SBInspector/Components/Pages/Home.razor` - Blazor Server app version

## Future Enhancements
Potential improvements for future consideration:
1. Add visual grouping or collapsing for multiple operation panels
2. Add a "Cancel All" button when multiple operations are running
3. Add operation history/log to track completed operations
4. Persist operation state to survive page refreshes
5. Add unit tests specifically for the PurgeOperation tracking logic
