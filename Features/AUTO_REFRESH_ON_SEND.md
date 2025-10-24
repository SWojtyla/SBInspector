# Auto-Refresh on Message Send

## Feature Overview

This feature ensures that the message list automatically refreshes after successfully sending a new message to a queue or topic. This provides immediate visual feedback to users and ensures they can see the newly sent message in the list without manually clicking the refresh button.

## How to Use

1. **Send a New Message**:
   - Connect to a Service Bus instance
   - Select a queue or topic/subscription
   - View messages (Active, Scheduled, or Dead-Letter)
   - Click "Send New" button
   - Fill in message details and click "Send"

2. **Automatic Refresh**:
   - After successful message send, the message list will automatically refresh
   - The new message will appear in the list
   - Entity counts in the sidebar will be updated
   - A success message will be displayed temporarily

3. **Reschedule Messages**:
   - When rescheduling a scheduled message, the entity counts are automatically refreshed
   - The message is removed from the current view
   - Sidebar counts are updated to reflect the change

## Technical Implementation

### Changes Made

1. **Home.razor - HandleSendMessage Method**:
   - Added call to `HandleRefreshMessages()` after successful message send
   - This reloads the message list from Service Bus
   - Also refreshes entity counts to update the sidebar

2. **Home.razor - HandleSendMessage Method (Reschedule)**:
   - Added call to `RefreshEntityCounts()` after successful reschedule
   - Updates the sidebar to show correct message counts

### Code Changes

**File**: `SBInspector.Shared/Presentation/Components/Pages/Home.razor`

```csharp
// After sending a new message successfully
if (success)
{
    if (data.ScheduledTime.HasValue)
    {
        successMessage = $"Message scheduled for {data.ScheduledTime.Value:yyyy-MM-dd HH:mm:ss}.";
    }
    else
    {
        successMessage = "Message sent successfully.";
    }
    
    // Refresh the message list to show the new message
    await HandleRefreshMessages();
}
```

```csharp
// After rescheduling a message successfully
if (success)
{
    messages.Remove(messageForOperation);
    successMessage = $"Message rescheduled for {data.ScheduledTime.Value:yyyy-MM-dd HH:mm:ss}.";
    
    // Refresh entity counts to update the sidebar
    await RefreshEntityCounts();
}
```

## Benefits

1. **Immediate Feedback**: Users can immediately see the message they just sent in the list
2. **Improved UX**: No need to manually click refresh after sending a message
3. **Consistency**: Matches user expectations from similar applications
4. **Accurate Counts**: Sidebar entity counts are always up-to-date after operations

## Notes

- The refresh happens automatically only after successful message send
- If the send operation fails, no refresh occurs
- The refresh includes both the message list and entity counts
- Users can still manually refresh using the "Refresh" button if needed
