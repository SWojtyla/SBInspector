# UI Component Refactoring

This document describes the UI component structure of SBInspector after the refactoring to separate concerns and improve maintainability.

## Overview

The UI layer has been refactored from a single monolithic `Home.razor` page into multiple reusable components. This follows the Single Responsibility Principle and makes the code more maintainable, testable, and reusable.

## Component Structure

All UI components are located in `Presentation/Components/UI/` directory.

### 1. ConnectionForm
**File:** `ConnectionForm.razor`

**Purpose:** Handles Azure Service Bus connection string input and connection establishment.

**Key Features:**
- Password input field for connection string
- Loading state with spinner
- Error message display
- Connection button with disabled state

**Parameters:**
- `ConnectionString` (string, two-way binding) - The connection string value
- `ErrorMessage` (string, two-way binding) - Error message to display
- `IsConnecting` (bool, two-way binding) - Connection state indicator
- `OnConnected` (EventCallback) - Called when connection is successful

**Usage:**
```razor
<ConnectionForm 
    @bind-ConnectionString="@connectionString"
    @bind-ErrorMessage="@errorMessage"
    @bind-IsConnecting="@isConnecting"
    OnConnected="@HandleConnected" />
```

### 2. QueueListTable
**File:** `QueueListTable.razor`

**Purpose:** Displays a sortable table of Service Bus queues with message counts.

**Key Features:**
- Sortable columns (Name, Active Messages, Scheduled Messages, Dead Letter Messages)
- Sort indicators (▲/▼/⇅)
- Action buttons for viewing different message types
- Responsive table design

**Parameters:**
- `Queues` (List<EntityInfo>) - List of queues to display
- `SortColumn` (string, two-way binding) - Current sort column
- `SortAscending` (bool, two-way binding) - Sort direction
- `OnViewMessages` (EventCallback<(string, string)>) - Called when user wants to view messages

**Usage:**
```razor
<QueueListTable 
    Queues="@queues" 
    @bind-SortColumn="@queueSortColumn"
    @bind-SortAscending="@queueSortAscending"
    OnViewMessages="@HandleViewQueueMessages" />
```

### 3. TopicListTable
**File:** `TopicListTable.razor`

**Purpose:** Displays a table of Service Bus topics with subscription viewing capability.

**Key Features:**
- Simple table layout
- Action button for viewing subscriptions
- Empty state message

**Parameters:**
- `Topics` (List<EntityInfo>) - List of topics to display
- `OnViewSubscriptions` (EventCallback<string>) - Called when user wants to view subscriptions

**Usage:**
```razor
<TopicListTable 
    Topics="@topics"
    OnViewSubscriptions="@ViewTopicSubscriptions" />
```

### 4. MessageFilterPanel
**File:** `MessageFilterPanel.razor`

**Purpose:** Provides filtering interface for messages based on their application properties.

**Key Features:**
- Multiple filter support
- Attribute name and value filtering
- Regex pattern matching option
- Add/Remove filter buttons
- Clear all filters button
- Message count display (filtered vs total)

**Parameters:**
- `Filters` (List<MessageFilter>, two-way binding) - Current filter collection
- `FilteredCount` (int) - Number of filtered messages
- `TotalCount` (int) - Total number of messages

**Usage:**
```razor
<MessageFilterPanel 
    @bind-Filters="@messageFilters"
    FilteredCount="@FilteredMessages.Count()"
    TotalCount="@messages.Count" />
```

### 5. MessageListTable
**File:** `MessageListTable.razor`

**Purpose:** Displays a sortable table of Service Bus messages.

**Key Features:**
- Sortable columns (Message ID, Subject, Enqueued Time, Delivery Count)
- Sort indicators (▲/▼/⇅)
- Details button for each message
- Compact table design

**Parameters:**
- `Messages` (IEnumerable<MessageInfo>) - Messages to display (should be pre-filtered)
- `SortColumn` (string, two-way binding) - Current sort column
- `SortAscending` (bool, two-way binding) - Sort direction
- `OnViewDetails` (EventCallback<MessageInfo>) - Called when user wants to view message details

**Usage:**
```razor
<MessageListTable 
    Messages="@FilteredMessages"
    @bind-SortColumn="@messageSortColumn"
    @bind-SortAscending="@messageSortAscending"
    OnViewDetails="@OnViewMessageDetails" />
```

### 6. MessageDetailsModal
**File:** `MessageDetailsModal.razor`

**Purpose:** Shows detailed information about a selected message in a modal dialog.

**Key Features:**
- Full-screen modal overlay
- Displays all message properties
- Application properties display
- Scrollable message body
- Close button

**Parameters:**
- `Message` (MessageInfo?) - The message to display (null hides the modal)
- `OnClose` (EventCallback) - Called when user closes the modal

**Usage:**
```razor
<MessageDetailsModal 
    Message="@selectedMessage"
    OnClose="@CloseMessageDetails" />
```

### 7. SubscriptionListPanel
**File:** `SubscriptionListPanel.razor`

**Purpose:** Displays subscriptions for a selected topic with message viewing actions.

**Key Features:**
- Card-based layout
- Loading state indicator
- Action buttons for viewing different message types
- Close button

**Parameters:**
- `TopicName` (string) - Name of the parent topic
- `Subscriptions` (List<string>) - List of subscription names
- `IsLoading` (bool) - Loading state indicator
- `OnClose` (EventCallback) - Called when user closes the panel
- `OnViewMessages` (EventCallback<(string, string, string)>) - Called when user wants to view messages

**Usage:**
```razor
<SubscriptionListPanel 
    TopicName="@selectedTopic"
    Subscriptions="@subscriptions"
    IsLoading="@isLoadingSubscriptions"
    OnClose="@CloseSubscriptions"
    OnViewMessages="@HandleViewSubscriptionMessages" />
```

### 8. MessagesPanel
**File:** `MessagesPanel.razor`

**Purpose:** Container component that combines message filtering and message list display.

**Key Features:**
- Integrates MessageFilterPanel and MessageListTable
- Handles filter application using MessageFilterService
- Card-based layout
- Loading state indicator
- Close button

**Parameters:**
- `EntityName` (string) - Name of the queue or topic/subscription
- `MessageType` (string) - Type of messages (Active, Scheduled, DeadLetter)
- `Messages` (List<MessageInfo>) - All messages
- `IsLoading` (bool) - Loading state indicator
- `Filters` (List<MessageFilter>, two-way binding) - Current filter collection
- `SortColumn` (string, two-way binding) - Current sort column
- `SortAscending` (bool, two-way binding) - Sort direction
- `OnClose` (EventCallback) - Called when user closes the panel
- `OnViewMessageDetails` (EventCallback<MessageInfo>) - Called when user wants to view message details

**Usage:**
```razor
<MessagesPanel 
    EntityName="@selectedEntity"
    MessageType="@selectedMessageType"
    Messages="@messages"
    IsLoading="@isLoadingMessages"
    @bind-Filters="@messageFilters"
    @bind-SortColumn="@messageSortColumn"
    @bind-SortAscending="@messageSortAscending"
    OnClose="@CloseMessages"
    OnViewMessageDetails="@ViewMessageDetails" />
```

## Benefits of the Refactoring

### 1. **Separation of Concerns**
Each component has a single, well-defined responsibility:
- Connection management
- Queue/Topic listing
- Message filtering
- Message display
- Detail viewing
- Subscription management

### 2. **Reusability**
Components can be used in different contexts:
- QueueListTable and TopicListTable can be reused in other pages
- MessageFilterPanel can be used wherever message filtering is needed
- MessageDetailsModal can display any message, regardless of source

### 3. **Maintainability**
- Smaller, focused files are easier to understand and modify
- Changes to one component don't affect others
- Reduced complexity in Home.razor (from 660 lines to ~150 lines)

### 4. **Testability**
- Each component can be tested independently
- Mock parameters and callbacks for unit testing
- Easier to write integration tests

### 5. **Consistency**
- UI patterns are standardized across components
- Sorting logic is encapsulated in table components
- Filtering logic is centralized in MessagesPanel

## Architecture Alignment

The refactoring maintains alignment with Clean Architecture principles:

- **Presentation Layer:** All components remain in `Presentation/Components/`
- **Application Layer:** MessageFilterService is used for filtering logic
- **Domain Layer:** Components use domain models (EntityInfo, MessageInfo, MessageFilter)
- **Infrastructure Layer:** ServiceBusService is injected where needed

## Component Communication

Components communicate through:
- **Parameters:** Data flows down from parent to child
- **EventCallbacks:** Events flow up from child to parent
- **Two-way Binding:** Using `@bind-` syntax for synchronized state

This follows Blazor's component model and ensures predictable data flow.

## Future Enhancements

Potential improvements for the component structure:

1. **State Management:** Consider using a state container for complex state
2. **Validation:** Add input validation in ConnectionForm
3. **Loading Strategies:** Implement skeleton loaders instead of spinners
4. **Error Boundaries:** Add error handling components
5. **Accessibility:** Enhance ARIA labels and keyboard navigation
6. **Responsive Design:** Add mobile-specific layouts
7. **Component Library:** Extract common UI patterns (buttons, cards) into shared components

## Migration Guide

For developers familiar with the old structure:

### Old (Monolithic)
All logic in `Home.razor`:
- 660 lines of code
- Mixing presentation and logic
- Difficult to navigate

### New (Component-based)
Logic distributed across 8 components:
- Home.razor: ~150 lines (orchestration)
- UI Components: ~2,400 characters each (focused logic)
- Clear separation of concerns
- Easy to locate and modify

### Breaking Changes
None - the refactoring maintains the same functionality and user experience.

## Component Dependencies

```
Home.razor
├── ConnectionForm
├── QueueListTable
├── TopicListTable
├── MessagesPanel
│   ├── MessageFilterPanel
│   └── MessageListTable
├── MessageDetailsModal
└── SubscriptionListPanel
```

All components depend on:
- Core domain models (EntityInfo, MessageInfo, MessageFilter)
- Blazor framework components
