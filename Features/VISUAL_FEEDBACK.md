# Visual Feedback Feature

## Overview

This feature ensures that users receive clear visual feedback whenever they perform actions in the SBInspector application. Visual feedback includes loading spinners, progress indicators, and status overlays that inform users their action is being processed.

## Purpose

The visual feedback feature was implemented to:
- Improve user experience by providing clear indication that an action is in progress
- Prevent confusion about whether an action was triggered
- Reduce user anxiety during long-running operations
- Prevent duplicate actions by disabling controls during processing

## Components with Visual Feedback

### 1. Send Message Modal (`SendMessageModal.razor`)

#### Send Button
- **Visual Feedback**: Spinner with "Sending..." text
- **Trigger**: When user clicks "Send" or "Reschedule" button
- **Duration**: Until message is successfully sent or an error occurs
- **Implementation**: Uses `isSending` state flag and `MudProgressCircular` component

#### Load Template Button
- **Visual Feedback**: Spinner with "Loading..." text
- **Trigger**: When user clicks "Load Template" button
- **Duration**: Until template is loaded from storage
- **Implementation**: Uses `isLoadingTemplate` state flag and `MudProgressCircular` component
- **Added in**: This PR

#### Delete Template Button
- **Visual Feedback**: Spinner with "Deleting..." text
- **Trigger**: When user clicks "Delete" button for a template
- **Duration**: Until template is deleted from storage
- **Implementation**: Uses `isDeletingTemplate` state flag and `MudProgressCircular` component
- **Added in**: This PR

### 2. Connection Form (`ConnectionForm.razor`)

#### Connect Button
- **Visual Feedback**: Spinner with "Connecting..." text
- **Trigger**: When user clicks "Connect" button
- **Duration**: Until connection to Azure Service Bus is established or fails
- **Implementation**: Uses `IsConnecting` state flag and `MudProgressCircular` component

### 3. Home Page Operations (`Home.razor`)

#### Send New Message Operation
- **Visual Feedback**: Full-screen `OperationLoadingOverlay` with "Sending Message" title
- **Trigger**: After user confirms message in `SendMessageModal` and the actual send operation begins
- **Duration**: Until message is sent and message list is refreshed
- **Implementation**: Uses `isOperationInProgress` flag with `OperationLoadingOverlay` component
- **Added in**: This PR

#### Export Single Message
- **Visual Feedback**: Full-screen `OperationLoadingOverlay` with "Exporting Message" title
- **Trigger**: When user clicks export button for a single message
- **Duration**: Until message is serialized and file is exported
- **Implementation**: Uses `isOperationInProgress` flag with `OperationLoadingOverlay` component
- **Added in**: This PR

#### Export Filtered Messages
- **Visual Feedback**: Full-screen `OperationLoadingOverlay` with "Exporting Messages" title
- **Trigger**: When user clicks "Export Filtered" button
- **Duration**: Until filtered messages are serialized and file is exported
- **Implementation**: Uses `isOperationInProgress` flag with `OperationLoadingOverlay` component
- **Added in**: This PR

#### Delete Message
- **Visual Feedback**: Full-screen `OperationLoadingOverlay` with "Deleting Message" title
- **Trigger**: After user confirms deletion in confirmation dialog
- **Duration**: Until message is deleted from Service Bus
- **Implementation**: Uses `isOperationInProgress` flag with `OperationLoadingOverlay` component

#### Requeue Message
- **Visual Feedback**: Full-screen `OperationLoadingOverlay` with "Requeuing Message" title
- **Trigger**: After user confirms requeue in confirmation dialog
- **Duration**: Until message is moved from dead-letter queue to active queue
- **Implementation**: Uses `isOperationInProgress` flag with `OperationLoadingOverlay` component

#### Move to Dead-Letter
- **Visual Feedback**: Full-screen `OperationLoadingOverlay` with "Moving to Dead-Letter" title
- **Trigger**: After user confirms move in confirmation dialog
- **Duration**: Until message is moved to dead-letter queue
- **Implementation**: Uses `isOperationInProgress` flag with `OperationLoadingOverlay` component

#### Purge Messages
- **Visual Feedback**: `BackgroundOperationPanel` with progress bar and message count
- **Trigger**: After user confirms purge in confirmation dialog
- **Duration**: Until all messages are deleted (can be cancelled by user)
- **Implementation**: Uses `activeOperations` dictionary with `BackgroundOperationPanel` component
- **Special**: Supports multiple concurrent purge operations on different entities

#### Delete Filtered Messages
- **Visual Feedback**: `BackgroundOperationPanel` with progress bar and message count
- **Trigger**: After user confirms deletion in confirmation dialog
- **Duration**: Until all filtered messages are deleted (can be cancelled by user)
- **Implementation**: Uses `activeOperations` dictionary with `BackgroundOperationPanel` component
- **Special**: Supports multiple concurrent delete operations on different entities

### 4. Entity Tree View (`EntityTreeView.razor`)

#### Refresh Entities
- **Visual Feedback**: Spinner in entity tree area, refresh button disabled
- **Trigger**: When user clicks refresh button in entity tree
- **Duration**: Until queues and topics are reloaded from Service Bus
- **Implementation**: Uses `IsLoading` parameter with `MudProgressCircular` component

#### Load Subscriptions
- **Visual Feedback**: Loading indicator on topic node being expanded
- **Trigger**: When user expands a topic to view subscriptions
- **Duration**: Until subscriptions are loaded for the topic
- **Implementation**: Uses `LoadingTopics` parameter to track loading state per topic

### 5. Messages Panel (`MessagesPanel.razor`)

#### Refresh Messages
- **Visual Feedback**: Loading state in messages panel, refresh button disabled
- **Trigger**: When user clicks refresh button in messages panel
- **Duration**: Until messages are reloaded from Service Bus
- **Implementation**: Uses `IsLoading` parameter with loading indicator

#### Load More Messages
- **Visual Feedback**: Spinner on "Load More" button with "Loading..." text
- **Trigger**: When user clicks "Load More" button at bottom of message list
- **Duration**: Until next page of messages is loaded
- **Implementation**: Uses `IsLoadingMore` parameter with `MudProgressCircular` component

## Implementation Patterns

### Pattern 1: Button Spinner (Small Operations)
Used for quick operations that complete within a few seconds:

```csharp
// State variable
private bool isProcessing = false;

// Button markup
<MudButton Disabled="@isProcessing" OnClick="HandleAction">
    @if (isProcessing)
    {
        <MudProgressCircular Indeterminate="true" Size="Size.Small" Class="mr-2" />
        <span>Processing...</span>
    }
    else
    {
        <span>Action Name</span>
    }
</MudButton>

// Handler
private async Task HandleAction()
{
    isProcessing = true;
    try
    {
        // Perform action
    }
    finally
    {
        isProcessing = false;
    }
}
```

### Pattern 2: Operation Loading Overlay (Medium Operations)
Used for operations that may take several seconds and should block the UI:

```csharp
// State variables
private bool isOperationInProgress = false;
private string operationLoadingTitle = string.Empty;
private string operationLoadingMessage = string.Empty;

// Component markup
<OperationLoadingOverlay 
    IsVisible="@isOperationInProgress"
    Title="@operationLoadingTitle"
    Message="@operationLoadingMessage" />

// Handler
private async Task HandleAction()
{
    isOperationInProgress = true;
    operationLoadingTitle = "Processing";
    operationLoadingMessage = "Please wait...";
    
    try
    {
        // Perform action
    }
    finally
    {
        isOperationInProgress = false;
    }
}
```

### Pattern 3: Background Operation Panel (Long Operations)
Used for long-running operations that can be cancelled:

```csharp
// Operation class
private class Operation
{
    public string OperationId { get; set; }
    public string ProgressMessage { get; set; }
    public int Progress { get; set; }
    public CancellationTokenSource CancellationTokenSource { get; set; }
}

// State variable
private Dictionary<string, Operation> activeOperations = new();

// Component markup
@foreach (var operation in activeOperations.Values)
{
    <BackgroundOperationPanel
        IsVisible="true"
        Title="@operation.Title"
        Message="@operation.ProgressMessage"
        Progress="@operation.Progress"
        OnCancel="@(() => CancelOperation(operation.OperationId))" />
}

// Handler
private async Task HandleAction()
{
    var operationId = Guid.NewGuid().ToString();
    var operation = new Operation
    {
        OperationId = operationId,
        CancellationTokenSource = new CancellationTokenSource()
    };
    
    activeOperations[operationId] = operation;
    
    try
    {
        var progress = new Progress<int>(count =>
        {
            operation.Progress = count;
            InvokeAsync(StateHasChanged);
        });
        
        // Perform action with progress reporting
    }
    finally
    {
        activeOperations.Remove(operationId);
    }
}
```

## User Experience Benefits

1. **Clarity**: Users know their action was received and is being processed
2. **Confidence**: Visual feedback reduces uncertainty and user anxiety
3. **Control**: Users can see progress and cancel long-running operations
4. **Prevention**: Disabled buttons prevent duplicate actions during processing
5. **Feedback**: Error messages and success notifications complement loading states

## Testing

To verify visual feedback is working correctly:

1. **Template Operations**
   - Open Send Message modal
   - Select a template and click "Load Template" - should see spinner
   - Click "Delete" on a template - should see spinner

2. **Message Operations**
   - Send a new message - should see spinner in modal, then overlay after closing modal
   - Export a single message - should see loading overlay
   - Export filtered messages - should see loading overlay

3. **Connection**
   - Enter connection string and click "Connect" - should see spinner

4. **Entity Operations**
   - Click refresh in entity tree - should see spinner
   - Expand a topic - should see loading indicator

5. **Message List Operations**
   - Click refresh in messages panel - should see loading state
   - Click "Load More" - should see spinner on button
   - Delete, requeue, or move message - should see loading overlay
   - Purge or delete filtered - should see background operation panel with progress

## Notes

- All visual feedback implementations use MudBlazor components for consistency
- Spinners are shown in-place for button actions
- Full-screen overlays are used for operations that modify data
- Background panels are used for long operations that report progress
- All operations properly handle errors and restore UI state in finally blocks
