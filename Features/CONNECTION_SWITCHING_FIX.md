# Connection Switching Bug Fix

## Issue

When switching from one connection to another using the connection tree view in the navigation menu, the UI did not refresh to show the queues and topics from the new connection. The old connection's data remained visible, causing confusion.

## Root Cause

The Home page component was not subscribed to connection state changes from the `ConnectionStateService`. When connections were switched via the tree view in the navigation:

1. `MainLayout.HandleConnectionSelected` connected to the new Service Bus namespace
2. `ConnectionStateService.CurrentConnectionString` was updated
3. The navigation tree updated to show the new active connection
4. **BUT** the Home page never received notification of the change
5. The queues and topics from the previous connection remained displayed

## Solution

### 1. Home Page Subscription to Connection State Changes

Updated `Home.razor` to subscribe to `ConnectionStateService.OnChange` events:

```csharp
protected override async Task OnInitializedAsync()
{
    // Subscribe to connection state changes
    ConnectionState.OnChange += HandleConnectionStateChanged;
    
    // If already connected (e.g., after page refresh), reload entities
    if (ServiceBusService.IsConnected)
    {
        await LoadAllEntitiesAsync();
    }
}
```

### 2. Clear and Reload on Connection Change

When a connection state change is detected, the Home page now:
- Clears all existing data (queues, topics, subscriptions, messages)
- Resets all view state (selections, filters, pagination)
- Loads new entities from the newly connected Service Bus namespace

```csharp
private async void HandleConnectionStateChanged()
{
    await InvokeAsync(async () =>
    {
        if (ServiceBusService.IsConnected && !string.IsNullOrEmpty(ConnectionState.CurrentConnectionString))
        {
            // Connection switched - clear old data and load new entities
            await ClearAllDataAndReload();
        }
        StateHasChanged();
    });
}
```

### 3. Proper Resource Cleanup

Implemented `IDisposable` on the Home page to properly unsubscribe from events:

```csharp
@implements IDisposable

public void Dispose()
{
    ConnectionState.OnChange -= HandleConnectionStateChanged;
}
```

### 4. Smooth Transition Enhancement

Updated `MainLayout.HandleConnectionSelected` to:
- Explicitly disconnect from current connection before connecting to new one
- Add a small 100ms delay between disconnect and connect for smoother visual transition

```csharp
// First disconnect from current connection if connected
if (ServiceBusService.IsConnected)
{
    await ServiceBusService.DisconnectAsync();
    // Small delay for smoother transition
    await Task.Delay(100);
}
```

## User Experience Improvements

### Before Fix
1. User clicks "Connection B" in the tree view
2. Navigation highlights "Connection B" as active
3. **Problem**: Main panel still shows queues/topics from "Connection A"
4. User is confused about which connection they're actually using

### After Fix
1. User clicks "Connection B" in the tree view
2. Brief disconnect (100ms) for smooth transition
3. Connects to "Connection B"
4. Navigation highlights "Connection B" as active
5. **Main panel immediately clears and shows loading state**
6. **New queues/topics from "Connection B" load and display**
7. User sees correct data matching the selected connection

## Files Modified

1. **Home.razor**
   - Added `@implements IDisposable`
   - Subscribed to `ConnectionState.OnChange` in `OnInitializedAsync`
   - Added `HandleConnectionStateChanged` method
   - Added `ClearAllDataAndReload` method
   - Added `Dispose` method for cleanup

2. **MainLayout.razor**
   - Updated `HandleConnectionSelected` to disconnect before connecting
   - Added 100ms delay for smoother transition

## Testing Validation

✅ Build succeeds with no errors
✅ Connection switching clears old data
✅ New entities load automatically after switch
✅ Event subscriptions properly cleaned up
✅ Smooth transition between connections

## Impact

This fix ensures that:
- The displayed data always matches the selected connection
- Users have visual confirmation that the connection changed (brief clear/reload)
- No memory leaks from event subscriptions
- Smooth UX with intentional 100ms delay
