# Refresh Button Loading Indicators

## Overview
Added consistent loading spinner indicators to refresh buttons across the application to provide better user feedback during data refresh operations.

## Problem
The refresh buttons in EntityTreeView and MessagesPanel had inconsistent behavior:
- EntityTreeView refresh button disappeared during loading
- MessagesPanel refresh button had no visual loading indicator
- No clear feedback to users that a refresh operation was in progress

## Solution
Standardized both refresh buttons to show loading spinners while data is being refreshed, providing consistent and clear user feedback.

## Files Modified
- `SBInspector.Shared/Presentation/Components/UI/EntityTreeView.razor` - Added spinner to refresh button
- `SBInspector.Shared/Presentation/Components/UI/MessagesPanel.razor` - Added spinner to refresh button

## Technical Details

### EntityTreeView Changes
```razor
@if (!IsLoading)
{
    <button class="btn btn-sm btn-primary" @onclick="HandleRefresh" title="Refresh all entities" disabled="@IsLoading">
        @if (IsLoading)
        {
            <span class="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span>
        }
        <i class="bi bi-arrow-clockwise"></i> Refresh
    </button>
}
else
{
    <button class="btn btn-sm btn-primary" disabled title="Refreshing...">
        <span class="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span>
        <i class="bi bi-arrow-clockwise"></i> Refresh
    </button>
}
```

### MessagesPanel Changes
```razor
<button class="btn btn-sm btn-primary" @onclick="HandleRefresh" title="Refresh messages" disabled="@IsLoading">
    @if (IsLoading)
    {
        <span class="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span>
    }
    <i class="bi bi-arrow-clockwise"></i> Refresh
</button>
```

## Behavior
Both refresh buttons now:
1. Display a spinning animation next to the refresh icon when loading
2. Become disabled during the refresh operation (preventing multiple simultaneous requests)
3. Return to normal state once the refresh completes
4. Provide consistent visual feedback across the application

## User Experience Improvements
- **Clear Feedback**: Users can see that their refresh request is being processed
- **Prevents Double-Clicks**: Disabled state prevents accidental multiple refresh requests
- **Consistency**: Both refresh buttons behave the same way, creating a cohesive user experience
- **Professional Appearance**: Loading indicators are a standard UI pattern that users expect

## Testing
1. Connect to a Service Bus namespace
2. Click the refresh button in the EntityTreeView (left sidebar)
   - Observe the spinner appears during the refresh
3. Navigate to view messages in a queue or subscription
4. Click the refresh button in the MessagesPanel
   - Observe the spinner appears during the refresh
5. Verify both buttons are disabled while loading
