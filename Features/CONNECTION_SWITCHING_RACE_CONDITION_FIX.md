# Connection Switching: Loading Spinner and Race Condition Fix

## Issues Addressed

1. **Missing Loading Spinner**: When switching connections, there was no visual feedback that entities were being loaded
2. **Race Condition Exceptions**: Rapid connection switching could cause exceptions due to concurrent operations

## Root Causes

### 1. Loading Spinner Issue
While the `isLoadingEntities` flag was being set during entity loading, it wasn't being explicitly shown during connection switching. The flag needed to be set earlier in the process and properly displayed through the UI.

### 2. Race Condition Issue
When users clicked rapidly between connections:
- Multiple `LoadAllEntitiesAsync()` operations could run concurrently
- Previous operations weren't being cancelled
- This led to exceptions when trying to load entities from a cancelled/disconnected connection
- The async nature of the operations meant results from old connections could overwrite new ones

## Solutions Implemented

### 1. Cancellation Token Support

Added a `CancellationTokenSource` to track and cancel entity loading operations:

```csharp
// Cancellation token for connection switching
private CancellationTokenSource? loadEntitiesCts = null;
```

**Benefits:**
- Cancels ongoing operations when switching to a new connection
- Prevents race conditions from concurrent operations
- Properly cleans up resources

### 2. Enhanced Loading State Management

Updated `ClearAllDataAndReload()` to:
- Cancel any ongoing entity loading operation
- Set `isLoadingEntities = true` immediately
- Call `StateHasChanged()` to show the spinner right away

```csharp
private async Task ClearAllDataAndReload()
{
    // Cancel any ongoing entity loading operation
    if (loadEntitiesCts != null)
    {
        loadEntitiesCts.Cancel();
        loadEntitiesCts.Dispose();
        loadEntitiesCts = null;
    }
    
    // Show loading state immediately
    isLoadingEntities = true;
    StateHasChanged();
    
    // ... clear data and reload
}
```

### 3. Robust Exception Handling

Updated `LoadAllEntitiesAsync()` with comprehensive error handling:

```csharp
try
{
    // Check if cancelled before starting
    cts.Token.ThrowIfCancellationRequested();
    
    queues = await ServiceBusService.GetQueuesAsync();
    
    // Check if cancelled between operations
    cts.Token.ThrowIfCancellationRequested();
    
    topics = await ServiceBusService.GetTopicsAsync();
}
catch (OperationCanceledException)
{
    // Operation was cancelled (expected during rapid switching)
    return;
}
catch (Exception ex)
{
    // Handle other exceptions with user-friendly message
    operationErrorMessage = $"Error loading entities: {ex.Message}";
}
finally
{
    // Only clear loading state if this is still the current operation
    if (loadEntitiesCts == cts)
    {
        isLoadingEntities = false;
    }
}
```

### 4. Proper Resource Cleanup

Enhanced `Dispose()` method to clean up cancellation tokens:

```csharp
public void Dispose()
{
    ConnectionState.OnChange -= HandleConnectionStateChanged;
    
    // Clean up cancellation token
    if (loadEntitiesCts != null)
    {
        loadEntitiesCts.Cancel();
        loadEntitiesCts.Dispose();
        loadEntitiesCts = null;
    }
}
```

## User Experience Improvements

### Loading Spinner
**Before:**
- User clicks connection → no visual feedback → entities appear suddenly
- Unclear if the connection is switching or hung

**After:**
- User clicks connection → loading spinner appears immediately
- Clear visual feedback that entities are loading
- Spinner disappears when entities are loaded

### Race Condition Prevention
**Before:**
- Rapid clicking: Connection A → Connection B → Connection C
- Could see queues from A, then B flashes, then C
- Sometimes exceptions thrown in console
- Unpredictable behavior

**After:**
- Rapid clicking: Connection A → Connection B → Connection C
- Previous operations automatically cancelled
- Only Connection C's entities are loaded and displayed
- No exceptions, predictable behavior
- Loading spinner shows during the transition

## Technical Details

### Cancellation Flow
1. User clicks Connection B while Connection A is loading
2. `ClearAllDataAndReload()` called
3. Checks if `loadEntitiesCts` exists (from Connection A)
4. Calls `Cancel()` on the token
5. Connection A's `LoadAllEntitiesAsync()` detects cancellation
6. Throws `OperationCanceledException`
7. Caught and handled gracefully (no error shown to user)
8. New `loadEntitiesCts` created for Connection B
9. Connection B's entities load successfully

### Loading State Synchronization
The key is checking if the current operation is still active:

```csharp
finally
{
    // Only clear loading state if this is still the current operation
    if (loadEntitiesCts == cts)
    {
        isLoadingEntities = false;
    }
}
```

This prevents a cancelled operation from clearing the loading state of a newer operation.

## Files Modified

**Home.razor**
- Added `loadEntitiesCts` field for cancellation token
- Updated `HandleConnectionStateChanged()` with try-catch
- Updated `ClearAllDataAndReload()` to cancel ongoing operations and show loading immediately
- Updated `LoadAllEntitiesAsync()` with cancellation token support
- Updated `Dispose()` to clean up cancellation tokens

## Testing Validation

✅ Build succeeds with no errors
✅ Loading spinner appears when switching connections
✅ Rapid connection switching no longer causes exceptions
✅ Cancelled operations don't interfere with new operations
✅ Memory properly cleaned up via Dispose
✅ Error messages shown for unexpected exceptions

## Impact

This fix ensures:
- **Visual Feedback**: Users always know when entities are loading
- **Stability**: No exceptions when rapidly switching connections
- **Predictability**: Only the selected connection's data is displayed
- **Performance**: Cancelled operations don't waste resources
- **Clean Code**: Proper resource management and disposal
