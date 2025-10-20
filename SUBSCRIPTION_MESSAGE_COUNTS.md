# Subscription Message Counts Feature

## Overview

This feature adds the display of active and dead letter message counts for Azure Service Bus topic subscriptions, similar to how message counts are displayed for queues.

## What Changed

### New Components

- **SubscriptionInfo Domain Model** (`Core/Domain/SubscriptionInfo.cs`): A new domain model that holds subscription information including:
  - `Name`: The subscription name
  - `ActiveMessageCount`: Number of active messages
  - `ScheduledMessageCount`: Number of scheduled messages (always 0 for subscriptions as Azure doesn't track this)
  - `DeadLetterMessageCount`: Number of dead letter messages

### Modified Components

1. **IServiceBusService Interface** (`Core/Interfaces/IServiceBusService.cs`):
   - Changed `GetSubscriptionsAsync` return type from `List<string>` to `List<SubscriptionInfo>`

2. **ServiceBusService Implementation** (`Infrastructure/ServiceBus/ServiceBusService.cs`):
   - Updated `GetSubscriptionsAsync` to fetch subscription runtime properties using `GetSubscriptionRuntimePropertiesAsync`
   - Extracts and returns message counts for each subscription

3. **SubscriptionListPanel Component** (`Presentation/Components/UI/SubscriptionListPanel.razor`):
   - Updated to accept `List<SubscriptionInfo>` instead of `List<string>`
   - Displays Active and Dead Letter message counts with styled badges (similar to queue display)
   - Message counts use color-coded badges (green for active, red for dead letter)
   - Kept all three action buttons (Active, Scheduled, DLQ) for viewing messages

4. **Home Page** (`Presentation/Components/Pages/Home.razor`):
   - Updated subscriptions field type from `List<string>` to `List<SubscriptionInfo>`

## How to Use

When you view subscriptions for a topic:

1. Click "View Subscriptions" on any topic in the Topics tab
2. The subscription panel now shows:
   - Subscription name
   - **Active message count** (with green badge if > 0)
   - **Scheduled message count** (shows "N/A" - not available from Azure API)
   - **Dead letter message count** (with red badge if > 0)
   - Action buttons to view messages

## Technical Details

### Why Scheduled Count Shows "N/A"

Azure Service Bus `SubscriptionRuntimeProperties` only provides:
- `ActiveMessageCount`
- `DeadLetterMessageCount`
- `TotalMessageCount`
- `TransferMessageCount`
- `TransferDeadLetterMessageCount`

Unlike queues, subscriptions don't track scheduled message counts in their runtime properties. The scheduled messages exist but are not counted separately by the Azure API. 

The UI displays "N/A" for the scheduled count to:
1. Maintain consistency with the queue display layout
2. Make it clear that this metric is not available for subscriptions
3. Provide a tooltip explaining the Azure API limitation

Users can still view scheduled messages using the "View Scheduled" button, but the count is not available at the subscription level.

### API Calls

Each subscription now makes an additional API call to `GetSubscriptionRuntimePropertiesAsync` to retrieve the message counts. This provides real-time data but may slightly increase loading time when viewing subscriptions for topics with many subscriptions.

## Benefits

- **Better Visibility**: Users can now see at a glance which subscriptions have messages waiting
- **Consistency**: Subscription display now matches the queue display pattern
- **Quick Insights**: No need to click into each subscription to see if it has messages
- **Color Coding**: Visual badges make it easy to identify subscriptions with pending or problematic messages
