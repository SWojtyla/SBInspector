# Refresh Functionality

## Overview

The SBInspector application now includes comprehensive refresh functionality to reload queues, topics, subscriptions, and messages with updated information from Azure Service Bus.

## Features

### 1. Auto-Reload on Page Refresh

**Problem Fixed**: Previously, when you refreshed the browser page, all queues and topics would disappear even though the connection remained active.

**Solution**: The application now automatically reloads all entities when the page is refreshed. The `OnInitializedAsync` lifecycle method checks if the Service Bus connection is still active and automatically loads all queues and topics.

**User Experience**:
- When you refresh the browser (F5 or Ctrl+R), the application will:
  - Detect that you're already connected
  - Automatically reload all queues and topics
  - Maintain your connection state
  - No need to reconnect or re-enter the connection string

### 2. Entity Tree Refresh Button

**Location**: In the left panel's Entity Tree View header, next to the title "Service Bus Entities"

**What it does**:
- Refreshes all queues with updated message counts (Active, Scheduled, Dead Letter)
- Refreshes all topics
- Reloads all subscriptions for topics that are currently expanded
- Updates the selected entity's information if one is currently selected

**When to use**:
- After messages have been added or removed by external processes
- To see updated message counts without navigating away
- To refresh the entire view of your Service Bus namespace

**Button**: 
- Icon: 🔄 (bi-arrow-clockwise)
- Label: "Refresh"
- Color: Primary (blue)

### 3. Messages Panel Refresh Button

**Location**: In the messages panel header, next to the "Send New" button

**What it does**:
- Reloads all messages from the current queue/subscription
- Updates message counts in the entity tree
- Resets the message list to the first page (respects current page size setting)
- Maintains current filters and sort settings

**When to use**:
- After processing messages externally
- To see newly arrived messages
- After sending new messages
- To refresh the current view without changing queues/subscriptions

**Button**:
- Icon: 🔄 (bi-arrow-clockwise)
- Label: "Refresh"
- Color: Primary (blue)

## How to Use

### Refreshing All Entities

1. Connect to your Service Bus namespace
2. Browse queues and topics in the left panel
3. Click the **"Refresh"** button at the top of the entity tree (left panel)
4. The application will reload:
   - All queues with updated message counts
   - All topics
   - All subscriptions for expanded topics
   - Currently selected entity information

### Refreshing Messages

1. Open a queue or subscription to view messages
2. Click the **"Refresh"** button in the messages panel header
3. The application will:
   - Reload messages from the queue/subscription
   - Update message counts in the entity tree
   - Reset to the first page of messages
   - Preserve your filter and sort settings

### After Browser Refresh

1. If you refresh the browser page (F5, Ctrl+R, or clicking refresh)
2. The application automatically:
   - Detects your existing connection
   - Reloads all queues and topics
   - Maintains the connection state
3. No action needed - it works automatically!

## Implementation Details

### Component Changes

**EntityTreeView.razor**:
- Added refresh button in the header
- Added `OnRefresh` event callback
- Button is only visible when not loading

**MessagesPanel.razor**:
- Added refresh button in the header
- Added `OnRefresh` event callback
- Button appears alongside existing action buttons

**Home.razor**:
- Added `OnInitializedAsync` lifecycle method to auto-reload on page refresh
- Implemented `HandleRefreshEntities` method to refresh all queues, topics, and subscriptions
- Implemented `HandleRefreshMessages` method to refresh current messages view
- Both refresh methods update message counts in the entity tree

### Key Methods

**HandleRefreshEntities**:
```csharp
- Reloads queues and topics from Service Bus
- Refreshes subscriptions for all expanded topics
- Updates selected entity information
- Shows loading indicator during refresh
```

**HandleRefreshMessages**:
```csharp
- Reloads messages for current queue/subscription
- Respects current page size setting
- Refreshes entity counts in the tree
- Shows loading indicator during refresh
```

**OnInitializedAsync**:
```csharp
- Checks if already connected on component initialization
- Automatically loads all entities if connection exists
- Fixes the browser refresh bug
```

## Benefits

1. **Better User Experience**: Always have up-to-date information without navigating away
2. **Fix Browser Refresh Bug**: Entities no longer disappear when refreshing the page
3. **Flexible Refresh Options**: Refresh everything or just the current messages view
4. **Maintains Context**: Refresh doesn't lose your current selection or filters
5. **Real-time Updates**: See changes made by external processes immediately

## Technical Notes

- Refresh operations are asynchronous and show loading indicators
- Entity counts are automatically updated after refresh
- Subscriptions are only reloaded for topics that were already expanded
- Message filters and sort settings are preserved during message refresh
- Connection state is maintained across browser refreshes through the singleton service
