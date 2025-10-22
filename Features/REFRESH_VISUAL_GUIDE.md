# Refresh Functionality - Visual Guide

## Overview

This document provides a visual representation of the refresh functionality added to the SBInspector application.

## UI Changes

### 1. Entity Tree View (Left Panel)

**Before:**
```
┌───────────────────────────────────────────┐
│ 🗂️  Service Bus Entities                  │
│                                           │
│ [Search queues and topics...]             │
│                                           │
│ ▼ 📥 Queues (3)                           │
│   📥 queue1  [Active] [10] [5] [2]       │
│   📥 queue2  [Active] [0]                 │
│   📥 queue3  [Disabled] [100]             │
│                                           │
│ ▼ 📡 Topics (2)                           │
│   📡 topic1  [Active] ▼                   │
│     ✉️  subscription1 [Active] [5]        │
│   📡 topic2  [Active] ▼                   │
└───────────────────────────────────────────┘
```

**After:**
```
┌───────────────────────────────────────────┐
│ 🗂️  Service Bus Entities  [🔄 Refresh]    │
│                                           │
│ [Search queues and topics...]             │
│                                           │
│ ▼ 📥 Queues (3)                           │
│   📥 queue1  [Active] [10] [5] [2]       │
│   📥 queue2  [Active] [0]                 │
│   📥 queue3  [Disabled] [100]             │
│                                           │
│ ▼ 📡 Topics (2)                           │
│   📡 topic1  [Active] ▼                   │
│     ✉️  subscription1 [Active] [5]        │
│   📡 topic2  [Active] ▼                   │
└───────────────────────────────────────────┘
```

**Key Change**: Added blue "Refresh" button with clockwise arrow icon (🔄) in the header

### 2. Messages Panel (Right Panel)

**Before:**
```
┌─────────────────────────────────────────────────────────────┐
│ 📧 queue1 - [Active Messages]                               │
│                                                             │
│          [✉️ Send New] [🗑️ Purge All] [Page Size: 100] [✖️ Close] │
├─────────────────────────────────────────────────────────────┤
│ Total Messages: 10      Filtered: 10                       │
│                                                             │
│ [Filters section]                                           │
│                                                             │
│ ┌─────────────────────────────────────────────────────┐   │
│ │ MessageId │ Subject │ Enqueued Time │ Delivery │ ▲ │   │
│ ├─────────────────────────────────────────────────────┤   │
│ │ msg-001   │ Test    │ 2024-01-01... │    1     │ ... │   │
│ │ msg-002   │ Data    │ 2024-01-01... │    1     │ ... │   │
│ └─────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
```

**After:**
```
┌─────────────────────────────────────────────────────────────┐
│ 📧 queue1 - [Active Messages]                               │
│                                                             │
│ [🔄 Refresh] [✉️ Send New] [🗑️ Purge All] [Page Size: 100] [✖️ Close] │
├─────────────────────────────────────────────────────────────┤
│ Total Messages: 10      Filtered: 10                       │
│                                                             │
│ [Filters section]                                           │
│                                                             │
│ ┌─────────────────────────────────────────────────────┐   │
│ │ MessageId │ Subject │ Enqueued Time │ Delivery │ ▲ │   │
│ ├─────────────────────────────────────────────────────┤   │
│ │ msg-001   │ Test    │ 2024-01-01... │    1     │ ... │   │
│ │ msg-002   │ Data    │ 2024-01-01... │    1     │ ... │   │
│ └─────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
```

**Key Change**: Added blue "Refresh" button as the first action button in the header

## User Workflows

### Workflow 1: Refreshing All Entities

```
User Action                    System Response
───────────────────────────    ──────────────────────────────────
1. Click "Refresh" button   → Shows loading spinner
   in Entity Tree View        
                               
2. System loads data from    → Queries Service Bus for:
   Azure Service Bus             - All queues
                                 - All topics
                                 - Expanded subscriptions
                               
3. Display updated data      → Updates UI with:
                                 - New message counts
                                 - Updated statuses
                                 - Refreshed subscription lists
                               
4. Hide loading spinner      → Tree view shows updated data
```

### Workflow 2: Refreshing Current Messages

```
User Action                    System Response
───────────────────────────    ──────────────────────────────────
1. Click "Refresh" button   → Shows loading indicator
   in Messages Panel          
                               
2. System loads messages     → Queries Service Bus for:
   from Service Bus              - Messages from current queue/sub
                                 - First page (respects page size)
                               
3. Update counts             → Refreshes entity counts in tree
                               
4. Display updated data      → Updates UI with:
                                 - New message list
                                 - Updated counts
                                 - Maintains filters/sort
                               
5. Hide loading indicator    → Messages panel shows updated data
```

### Workflow 3: Browser Page Refresh (Bug Fix)

**Before (Buggy Behavior):**
```
User Action                    System Response (OLD)
───────────────────────────    ──────────────────────────────────
1. Browse queues/topics     → Shows all entities
                               
2. Press F5 or Ctrl+R       → Component reinitializes
                               
3. Page reloads             → ❌ Entity tree is EMPTY
                               ❌ Connection still active
                               ❌ User must reconnect to see data
```

**After (Fixed Behavior):**
```
User Action                    System Response (NEW)
───────────────────────────    ──────────────────────────────────
1. Browse queues/topics     → Shows all entities
                               
2. Press F5 or Ctrl+R       → Component reinitializes
                               
3. Component checks if      → ✅ Connection detected as active
   already connected          
                               
4. Auto-reload entities     → ✅ Automatically loads queues/topics
                               
5. Page fully loaded        → ✅ Tree view shows all entities
                               ✅ User can continue working
```

## Loading States

### Entity Tree Refresh
```
┌───────────────────────────────────────────┐
│ 🗂️  Service Bus Entities  [🔄 Refresh]    │
│                                           │
│        ⏳ Loading...                       │
│                                           │
└───────────────────────────────────────────┘
```

### Message Panel Refresh
```
┌─────────────────────────────────────────────────┐
│ 📧 queue1 - [Active Messages]                   │
│                                                 │
│ [🔄 Refresh] [✉️ Send New] [🗑️ Purge All] [✖️ Close] │
├─────────────────────────────────────────────────┤
│                                                 │
│        ⏳ Loading messages...                    │
│                                                 │
└─────────────────────────────────────────────────┘
```

## Button Styles

Both refresh buttons use consistent styling:

- **Color**: Primary (Bootstrap blue)
- **Size**: Small (`btn-sm`)
- **Icon**: `bi-arrow-clockwise` (Bootstrap Icons)
- **Text**: "Refresh"
- **Tooltip**: 
  - Entity Tree: "Refresh all entities"
  - Messages Panel: "Refresh messages"

## Responsive Behavior

The buttons maintain their visibility and functionality across different screen sizes:

- **Desktop**: Buttons show full text "Refresh" with icon
- **Tablet**: Same as desktop
- **Mobile**: May wrap to new line if needed (flexbox with gap)

## Accessibility

Both refresh buttons include:
- Descriptive text labels ("Refresh")
- `title` attributes for tooltips
- Semantic button elements
- Icon + text for better understanding
- Disabled state during loading

## Integration Points

### Entity Tree Refresh
```
EntityTreeView Component
    ↓ @onclick="HandleRefresh"
    ↓ OnRefresh.InvokeAsync()
    ↓
Home Component
    ↓ HandleRefreshEntities()
    ↓
ServiceBusService
    ↓ GetQueuesAsync()
    ↓ GetTopicsAsync()
    ↓ GetSubscriptionsAsync()
    ↓
Azure Service Bus
```

### Message Panel Refresh
```
MessagesPanel Component
    ↓ @onclick="HandleRefresh"
    ↓ OnRefresh.InvokeAsync()
    ↓
Home Component
    ↓ HandleRefreshMessages()
    ↓
ServiceBusService
    ↓ GetMessagesAsync()
    ↓ or GetSubscriptionMessagesAsync()
    ↓
Azure Service Bus
```

## State Preservation

During refresh operations, the following state is preserved:

✅ **Preserved:**
- Current entity selection
- Expanded/collapsed sections
- Search filter text
- Message filters
- Sort column and direction
- Page size setting
- Connection state

❌ **Reset:**
- Message list (reloads from page 1)
- Loading states (temporarily set during operation)

## Error Handling

If refresh operations fail:

1. Loading indicator is hidden
2. Entity/message lists remain unchanged
3. User can retry the refresh operation
4. Connection remains active (no auto-disconnect)

## Performance Considerations

- Both refresh operations are asynchronous
- UI remains responsive during refresh
- Loading indicators provide feedback
- Network calls are made in parallel where possible
- Entity counts are updated efficiently

## Summary

The refresh functionality provides:

✅ Two strategically placed refresh buttons
✅ Consistent visual design and behavior  
✅ Clear loading states
✅ Preserved user context
✅ Fixed browser refresh bug
✅ Improved user experience
