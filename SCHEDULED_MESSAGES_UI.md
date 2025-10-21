# Scheduled Messages UI Guide

## Subscription Details View

When you select a subscription from the tree view, the details panel displays three message type cards:

```
┌─────────────────────────────────────────────────────────────┐
│ 🏷️ TopicName / SubscriptionName                   [Active] │
│                                                  [Enable/Disable]
├─────────────────────────────────────────────────────────────┤
│ Message Counts                                              │
│                                                             │
│ ┌─────────────────┐ ┌─────────────────┐ ┌──────────────┐  │
│ │ 📥 Active Msgs  │ │ ⏰ Scheduled    │ │ ⚠️ Dead Lttr │  │
│ │                 │ │                 │ │              │  │
│ │ Count: 42       │ │ Count: -        │ │ Count: 3     │  │
│ │                 │ │                 │ │              │  │
│ │   [View]        │ │   [View]        │ │   [View]     │  │
│ └─────────────────┘ └─────────────────┘ └──────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

**Note**: The Scheduled Messages card shows "-" for count because Azure Service Bus SDK doesn't provide `ScheduledMessageCount` for subscriptions (only for queues).

## Topic Aggregated View

When you select a topic (without drilling into a subscription), you see aggregated counts:

```
┌─────────────────────────────────────────────────────────────┐
│ 📡 TopicName                                       [Active]  │
│                                                  [Enable/Disable]
├─────────────────────────────────────────────────────────────┤
│ Aggregated Message Counts (All Subscriptions)              │
│                                                             │
│ ┌─────────────────┐ ┌──────────────────────────────┐       │
│ │ 📥 Active Msgs  │ │ ⚠️ Dead Letter Messages      │       │
│ │                 │ │                              │       │
│ │ Count: 156      │ │ Count: 7                     │       │
│ │                 │ │                              │       │
│ └─────────────────┘ └──────────────────────────────┘       │
│                                                             │
│ ℹ️ Select a subscription from the tree to view and manage  │
│   its messages, including scheduled messages.              │
└─────────────────────────────────────────────────────────────┘
```

## Scheduled Messages List View

After clicking "View" on the Scheduled Messages card:

```
┌─────────────────────────────────────────────────────────────────────┐
│ 📧 TopicName/SubscriptionName        [Scheduled Messages]          │
│                                                                     │
│ [+ Send New] [🗑️ Purge All]  Page Size: [100▼]    [X Close]       │
├─────────────────────────────────────────────────────────────────────┤
│ Total Messages: 23    Filtered: 23                                 │
│                                                                     │
│ ┌─ Message Filter ────────────────────────────────────────────┐   │
│ │ [+ Add Filter]                                              │   │
│ └─────────────────────────────────────────────────────────────┘   │
│                                                                     │
│ ┌─────────────────────────────────────────────────────────────┐   │
│ │ Message ID ▲    │ Subject       │ Enqueued Time │ Actions  │   │
│ ├─────────────────┼───────────────┼───────────────┼──────────┤   │
│ │ msg-123         │ Order Process │ 2025-10-22    │ 👁️ 🔄 🗑️ │   │
│ │ msg-456         │ Email Notify  │ 2025-10-23    │ 👁️ 🔄 🗑️ │   │
│ │ msg-789         │ Report Gen    │ 2025-10-24    │ 👁️ 🔄 🗑️ │   │
│ └─────────────────────────────────────────────────────────────┘   │
│                                                                     │
│ [Load More Messages]                                               │
└─────────────────────────────────────────────────────────────────────┘

Action Buttons:
- 👁️ View Details: View full message content and properties
- 🔄 Reschedule: Change the scheduled delivery time (clock icon)
- 🗑️ Delete: Permanently remove the scheduled message
```

## Reschedule Modal

When you click the reschedule button (🔄):

```
┌─────────────────────────────────────────────────┐
│ Reschedule Message                        ✕     │
├─────────────────────────────────────────────────┤
│                                                 │
│ Message ID: msg-123                             │
│ Current Scheduled Time: 2025-10-22 10:00:00     │
│                                                 │
│ New Scheduled Time:                             │
│ ┌─────────────────────────────────────────┐    │
│ │ 2025-10-25T14:30:00                     │    │
│ └─────────────────────────────────────────┘    │
│                                                 │
│               [Cancel]  [Reschedule]            │
└─────────────────────────────────────────────────┘
```

## Key UI Features

### 1. Subscription Card
- Displays "-" for scheduled message count (not available in Azure SDK)
- "View" button always visible to allow viewing scheduled messages
- Click anywhere on the card or the View button to open the messages panel

### 2. Messages Panel
- Shows scheduled messages with their scheduled enqueue time
- **Send New** button - Create new scheduled messages for the topic
- **Purge All** button - Delete all scheduled messages (with confirmation)
- **Reschedule button** (clock icon) - Only visible for scheduled messages
- **Filter panel** - Filter scheduled messages by application properties

### 3. Message Details
- Click any message row to see full details
- Scheduled Enqueue Time is prominently displayed
- View message body and all application properties

### 4. Visual Indicators
- Scheduled Messages card uses clock icon (⏰) and warning color (yellow/orange)
- Badge shows "Scheduled Messages" in the messages panel header
- Reschedule button uses clock-history icon (🕐)

## Workflow Example

1. **Navigate**: Tree View → Select Topic → Expand → Click Subscription
2. **View**: Click "View" on Scheduled Messages card
3. **Inspect**: Review list of scheduled messages with delivery times
4. **Manage**: 
   - Reschedule: Click 🔄 icon → Set new time → Confirm
   - Delete: Click 🗑️ icon → Confirm
   - View Details: Click 👁️ icon or click message row
5. **Close**: Click "Close" button to return to details panel
