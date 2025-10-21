# Background Purge Operation

## Overview

The purge messages feature now runs in the background, allowing users to continue using the application while messages are being deleted. Users can also cancel the operation at any time.

## Features

### Non-Blocking UI

- **Background Processing**: The purge operation runs in the background without blocking the UI
- **Continue Working**: Users can navigate to other entities or perform other operations while purging is in progress
- **Real-time Progress**: A progress panel shows the number of messages deleted in real-time

### Cancellation Support

- **Cancel Anytime**: Users can cancel the purge operation at any point during execution
- **Partial Results**: When cancelled, the operation reports how many messages were deleted before cancellation
- **Clean Cancellation**: Resources are properly cleaned up when the operation is cancelled

## User Experience

### Starting a Purge Operation

1. Navigate to a queue or subscription and view messages (Active, Scheduled, or Dead-Letter)
2. Click the **"Purge All"** button in the messages panel
3. A confirmation dialog appears requiring you to type 'PURGE' to confirm
4. After confirmation, the purge operation starts in the background

### During Purge

- A **background operation panel** appears in the bottom-right corner of the screen
- The panel shows:
  - A spinning icon indicating the operation is in progress
  - The number of messages deleted so far
  - A **Cancel** button to stop the operation

### Completion

- Upon completion, a success message shows the total number of messages deleted
- The message list is automatically cleared
- Entity counts in the tree view are refreshed

### Cancellation

- Click the **Cancel** button in the background operation panel
- The operation stops gracefully
- A message displays how many messages were deleted before cancellation

## Technical Implementation

### CancellationToken Support

The `PurgeMessagesAsync` method in `IServiceBusService` now accepts a `CancellationToken`:

```csharp
Task<int> PurgeMessagesAsync(
    string entityName, 
    string messageType, 
    bool isSubscription = false, 
    string? topicName = null, 
    string? subscriptionName = null, 
    CancellationToken cancellationToken = default, 
    IProgress<int>? progress = null);
```

### Progress Reporting

The method also accepts an `IProgress<int>` callback to report the number of messages deleted:

```csharp
var progress = new Progress<int>(count =>
{
    purgeProgress = count;
    InvokeAsync(StateHasChanged);
});
```

### Background Execution

The operation runs in a background task using `Task.Run`:

```csharp
_ = Task.Run(async () =>
{
    // Purge operation with cancellation and progress reporting
    int deletedCount = await ServiceBusService.PurgeMessagesAsync(
        entityName,
        messageType,
        cancellationToken: purgeCancellationTokenSource.Token,
        progress: progress);
});
```

## Component Architecture

### BackgroundOperationPanel

A new UI component (`BackgroundOperationPanel.razor`) displays background operations:

- **Position**: Fixed bottom-right corner of the screen
- **Appearance**: Card-style with shadow, slides in from the right
- **Content**: Shows title, progress message, current count, and cancel button
- **Animation**: Smooth slide-in animation and spinning icon

### Integration

The component is integrated into `Home.razor` alongside other UI elements:

```razor
<BackgroundOperationPanel
    IsVisible="@isPurgeInProgress"
    Title="Purging Messages"
    Message="@purgeProgressMessage"
    Progress="@purgeProgress"
    OnCancel="@CancelPurge" />
```

## Benefits

1. **Better User Experience**: Users are not blocked by long-running operations
2. **Transparency**: Real-time progress feedback keeps users informed
3. **Control**: Users can cancel operations they no longer need
4. **Flexibility**: Users can perform other tasks while purging continues
5. **Reliability**: Proper error handling and resource cleanup

## Future Enhancements

Potential improvements for future versions:

- Support for multiple concurrent background operations
- Detailed progress percentage or time estimates
- Pause/resume functionality
- Background operation history/log
- Configurable batch sizes for performance tuning
