# Subscription Message Counts Feature

## Overview

This feature adds the display of active, scheduled, and dead letter message counts for Azure Service Bus topic subscriptions, similar to how message counts are displayed for queues.

## What Changed

### New Components

- **SubscriptionInfo Domain Model** (`Core/Domain/SubscriptionInfo.cs`): A new domain model that holds subscription information including:
  - `Name`: The subscription name
  - `ActiveMessageCount`: Number of active messages
  - `ScheduledMessageCount`: Number of scheduled messages (retrieved on demand)
  - `DeadLetterMessageCount`: Number of dead letter messages

### Modified Components

1. **IServiceBusService Interface** (`Core/Interfaces/IServiceBusService.cs`):
   - Changed `GetSubscriptionsAsync` return type from `List<string>` to `List<SubscriptionInfo>`
   - Added `GetSubscriptionScheduledMessageCountAsync` method to count scheduled messages on demand

2. **ServiceBusService Implementation** (`Infrastructure/ServiceBus/ServiceBusService.cs`):
   - Updated `GetSubscriptionsAsync` to fetch subscription runtime properties using `GetSubscriptionRuntimePropertiesAsync`
   - Extracts and returns message counts for each subscription
   - Implemented `GetSubscriptionScheduledMessageCountAsync` that peeks messages and counts those with scheduled enqueue times in the future
   - Initial load sets `ScheduledMessageCount` to -1 (not loaded)

3. **SubscriptionListPanel Component** (`Presentation/Components/UI/SubscriptionListPanel.razor`):
   - Updated to accept `List<SubscriptionInfo>` instead of `List<string>`
   - Displays Active and Dead Letter message counts with styled badges (similar to queue display)
   - Shows Scheduled count as "-" when not loaded, with tooltip explaining to click the load button
   - Added "Load Scheduled Counts" button in the header to retrieve scheduled message counts on demand
   - Shows loading spinner for each subscription while counting scheduled messages
   - Message counts use color-coded badges (green for active, yellow for scheduled, red for dead letter)

4. **Home Page** (`Presentation/Components/Pages/Home.razor`):
   - Updated subscriptions field type from `List<string>` to `List<SubscriptionInfo>`
   - Added `HandleLoadScheduledCount` method to handle scheduled count loading

## How to Use

When you view subscriptions for a topic:

1. Click "View Subscriptions" on any topic in the Topics tab
2. The subscription panel now shows:
   - Subscription name
   - **Active message count** (with green badge if > 0)
   - **Scheduled message count** (shows "-" initially, with tooltip)
   - **Dead letter message count** (with red badge if > 0)
   - Action buttons to view messages
3. Click the **"Load Scheduled Counts"** button in the header to retrieve scheduled message counts for all subscriptions
4. Each subscription will show a loading spinner while its scheduled count is being retrieved
5. Once loaded, the actual count will display with appropriate color coding

## Technical Details

### Why Scheduled Count Requires Manual Loading

Azure Service Bus `SubscriptionRuntimeProperties` only provides:
- `ActiveMessageCount`
- `DeadLetterMessageCount`
- `TotalMessageCount`
- `TransferMessageCount`
- `TransferDeadLetterMessageCount`

Unlike queues, subscriptions don't track scheduled message counts in their runtime properties. To get the count, we must:
1. Create a receiver for the subscription
2. Peek all messages in batches
3. Count messages where `ScheduledEnqueueTime > DateTimeOffset.UtcNow`

This is an expensive operation, especially for subscriptions with many messages, which is why it's:
- Not loaded by default
- Triggered only when the user clicks "Load Scheduled Counts"
- Processed sequentially to avoid overwhelming the Service Bus

### Performance Considerations

- **Initial load**: Fast - only retrieves runtime properties (Active and Dead Letter counts)
- **Scheduled count load**: Can be slow for subscriptions with many messages
  - Peeks messages in batches of 100
  - Continues until all messages are counted
  - Processes subscriptions sequentially to minimize Service Bus load
  - Shows individual loading state for each subscription

### API Calls

- **GetSubscriptionsAsync**: One API call per subscription to get runtime properties
- **GetSubscriptionScheduledMessageCountAsync**: Multiple peek operations per subscription (one per 100 messages), only when user requests it

## Benefits

- **Better Visibility**: Users can now see at a glance which subscriptions have messages waiting
- **Consistency**: Subscription display now matches the queue display pattern
- **On-Demand Loading**: Scheduled counts are loaded only when needed, avoiding performance impact
- **Quick Insights**: No need to click into each subscription to see if it has active or dead letter messages
- **Color Coding**: Visual badges make it easy to identify subscriptions with pending or problematic messages
- **User Control**: Users decide when to pay the performance cost of counting scheduled messages
