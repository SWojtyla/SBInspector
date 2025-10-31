# Message Details View

## Overview

The message details view has been updated to provide a better user experience when inspecting individual messages from Azure Service Bus queues, topics, and subscriptions.

## Changes

### Previous Behavior
- Clicking on a message in the message list opened a **modal/pop-up** overlay
- The modal displayed message details on top of the message list
- Users had to close the modal to return to the message list

### New Behavior
- Clicking on a message in the message list now **replaces the entire message list** with the message details
- The message details are displayed in the same panel where the message list was shown
- A **"Back to Messages"** button is prominently displayed at the top and bottom of the details view
- Users can click the back button to return to the message list

## Benefits

1. **Better Focus**: The full panel is dedicated to message details, providing more space to view message content
2. **Clearer Navigation**: The back button makes it obvious how to return to the message list
3. **Consistent Layout**: The details view follows the same card layout as other panels in the application
4. **Improved Mobile Experience**: Full-panel views work better on smaller screens than modals

## How to Use

1. Navigate to a queue or subscription and view its messages
2. Click on any message in the table (or click the eye icon in the Actions column)
3. The message details will replace the message table
4. Review all message properties, metadata, and body content
5. Optionally save the message as a template
6. Click **"Back to Messages"** button (at top or bottom) to return to the message list

## Technical Implementation

### Components Modified

- **`MessageDetailsView.razor`** (NEW): A new component that displays message details in a card layout with a back button
- **`MessagesPanel.razor`**: Updated to toggle between list view and details view based on selected message state
- **`Home.razor`** (both Shared and Blazor Server): Removed the `MessageDetailsModal` component

### Key Changes

1. Added `SelectedMessage` state to `MessagesPanel.razor` to track which message is being viewed
2. Created conditional rendering logic to show either the message list or the message details
3. The `HandleViewDetails` method sets the selected message, triggering the details view
4. The `HandleBackToList` method clears the selected message, returning to the list view

## Related Files

- `/SBInspector.Shared/Presentation/Components/UI/MessageDetailsView.razor`
- `/SBInspector.Shared/Presentation/Components/UI/MessagesPanel.razor`
- `/SBInspector.Shared/Presentation/Components/Pages/Home.razor`
- `/SBInspector/Components/Pages/Home.razor`
