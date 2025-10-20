# Enable/Disable Entity Status Feature

## Overview

This feature allows you to view and toggle the status (Active/Disabled) of Azure Service Bus queues, topics, and subscriptions directly from the SBInspector UI.

## Features

### Status Display

- **Visual Indicators**: Each queue, topic, and subscription displays its current status with a color-coded badge:
  - 🟢 **Active** - Green badge indicating the entity is active and can send/receive messages
  - ⚫ **Disabled** - Gray badge indicating the entity is disabled

### Status Toggle Actions

- **Enable/Disable Queues**: Toggle individual queues between Active and Disabled states
- **Enable/Disable Topics**: Toggle individual topics between Active and Disabled states  
- **Enable/Disable Subscriptions**: Toggle individual subscriptions between Active and Disabled states

## User Interface

### Queues Table

The Queues table now includes:
- A **Status** column showing the current state (Active/Disabled)
- **Enable/Disable buttons** in the Actions column:
  - When Active: Shows a "Disable" button with a pause icon
  - When Disabled: Shows an "Enable" button with a play icon

### Topics Table

The Topics table now includes:
- A **Status** column showing the current state (Active/Disabled)
- **Enable/Disable buttons** in the Actions column:
  - When Active: Shows a "Disable" button with a pause icon
  - When Disabled: Shows an "Enable" button with a play icon

### Subscriptions Panel

The Subscriptions panel now includes:
- A **Status badge** next to each subscription name
- **Enable/Disable buttons** in each subscription card:
  - When Active: Shows a "Disable" button with a pause icon
  - When Disabled: Shows an "Enable" button with a play icon

## How to Use

### Disabling an Entity

1. Navigate to the Queues or Topics tab
2. Locate the entity you want to disable
3. Click the **Disable** button in the Actions column
4. The entity status will be updated to "Disabled" and the badge will change to gray
5. A success message will confirm the operation

### Enabling an Entity

1. Navigate to the Queues or Topics tab
2. Locate the disabled entity you want to enable
3. Click the **Enable** button in the Actions column
4. The entity status will be updated to "Active" and the badge will change to green
5. A success message will confirm the operation

### For Subscriptions

1. Navigate to the Topics tab
2. Click "View Subscriptions" for the desired topic
3. Locate the subscription you want to enable/disable
4. Click the **Enable** or **Disable** button next to the subscription
5. The status badge will update accordingly

## What Happens When You Disable an Entity

### Disabled Queue
- Messages cannot be sent to the queue
- Messages cannot be received from the queue
- Existing messages remain in the queue
- The queue can still be viewed and managed

### Disabled Topic
- Messages cannot be published to the topic
- Existing subscriptions are not affected directly
- The topic can still be viewed and managed

### Disabled Subscription
- Messages are not delivered to the subscription
- Messages continue to be published to the topic
- Existing messages in the subscription remain
- The subscription can still be viewed and managed

## Azure Service Bus Entity Status Values

The Azure Service Bus supports the following status values:
- **Active**: The entity is fully operational
- **Disabled**: The entity is disabled for all operations
- **SendDisabled**: The entity cannot receive new messages (send operations are disabled)
- **ReceiveDisabled**: The entity cannot deliver messages (receive operations are disabled)

*Note: The current implementation focuses on Active and Disabled states. SendDisabled and ReceiveDisabled states are displayed as-is if encountered.*

## Technical Implementation

### Domain Models

- `EntityInfo` class includes a `Status` property (string)
- `SubscriptionInfo` class includes a `Status` property (string)

### Service Interface

New methods in `IServiceBusService`:
- `SetQueueStatusAsync(string queueName, bool enabled)`: Enable or disable a queue
- `SetTopicStatusAsync(string topicName, bool enabled)`: Enable or disable a topic
- `SetSubscriptionStatusAsync(string topicName, string subscriptionName, bool enabled)`: Enable or disable a subscription
- `GetQueueInfoAsync(string queueName)`: Refresh queue information including status
- `GetTopicInfoAsync(string topicName)`: Refresh topic information including status
- `GetSubscriptionInfoAsync(string topicName, string subscriptionName)`: Refresh subscription information including status

### Implementation Details

The implementation uses the Azure Service Bus Administration Client to:
1. Retrieve entity properties using `GetQueueAsync`, `GetTopicAsync`, or `GetSubscriptionAsync`
2. Update the `Status` property to `EntityStatus.Active` or `EntityStatus.Disabled`
3. Update the entity using `UpdateQueueAsync`, `UpdateTopicAsync`, or `UpdateSubscriptionAsync`

## Message Count Refresh Bug Fix

As part of this feature, a bug was fixed where message counts in the queue/topic table were not being updated after deleting messages or performing a purge operation.

### Fix Details

- After message deletion: The entity's message counts are refreshed using the new `GetQueueInfoAsync` or `GetSubscriptionInfoAsync` methods
- After message purge: The entity's message counts are refreshed to reflect the empty state
- After message requeue: Both dead-letter and active message counts are updated

This ensures that the table above the message panel always displays accurate, up-to-date message counts.

## Error Handling

If an enable/disable operation fails:
- An error message is displayed at the top of the page
- The entity status remains unchanged in the UI
- Common reasons for failure:
  - Network connectivity issues
  - Insufficient permissions
  - The entity was deleted or modified by another user

## Permissions Required

To enable or disable entities, your Azure Service Bus connection must have **Manage** permissions on the namespace or specific entity. Read-only connections cannot modify entity status.

## Related Features

- [Message CRUD Operations](MESSAGE_CRUD.md)
- [Filtering Messages](FILTERING.md)
- [Pagination](PAGINATION.md)
