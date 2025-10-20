# Subscription Message Counts Feature

## Overview

This feature adds the display of active and dead letter message counts for Azure Service Bus topic subscriptions.

## What Changed

### New Components

- **SubscriptionInfo Domain Model** (`Core/Domain/SubscriptionInfo.cs`): A new domain model that holds subscription information including:
  - `Name`: The subscription name
  - `ActiveMessageCount`: Number of active messages per subscription
  - `DeadLetterMessageCount`: Number of dead letter messages per subscription

### Modified Components

1. **IServiceBusService Interface** (`Core/Interfaces/IServiceBusService.cs`):
   - Changed `GetSubscriptionsAsync` return type from `List<string>` to `List<SubscriptionInfo>`

2. **ServiceBusService Implementation** (`Infrastructure/ServiceBus/ServiceBusService.cs`):
   - Updated `GetSubscriptionsAsync` to fetch subscription runtime properties using `GetSubscriptionRuntimePropertiesAsync`
   - Extracts and returns message counts for each subscription (active and dead letter)

3. **SubscriptionListPanel Component** (`Presentation/Components/UI/SubscriptionListPanel.razor`):
   - Updated to accept `List<SubscriptionInfo>` instead of `List<string>`
   - Displays Active and Dead Letter message counts per subscription with styled badges
   - Message counts use color-coded badges (green for active, red for dead letter)

4. **Home Page** (`Presentation/Components/Pages/Home.razor`):
   - Updated subscriptions field type from `List<string>` to `List<SubscriptionInfo>`

## How to Use

When you view subscriptions for a topic:

1. Click "View Subscriptions" on any topic in the Topics tab
2. The subscription panel shows each subscription with:
   - Subscription name
   - **Active message count** (with green badge if > 0)
   - **Dead letter message count** (with red badge if > 0)
   - Action buttons to view messages

## Technical Details

### Message Counts

**Active and Dead Letter Counts (Per Subscription):**
- Retrieved from `SubscriptionRuntimeProperties` API
- Fast and efficient
- Available immediately on subscription load

**Note on Scheduled Messages:**
Scheduled message counts are not displayed due to performance concerns. Retrieving scheduled message counts requires peeking through all messages in the subscription, which can be extremely slow for topics with many messages and may result in infinite loops in certain scenarios.

### Performance Considerations

- **Initial load**: Fast - only retrieves runtime properties (Active and Dead Letter counts per subscription)
- **No scheduled count**: Scheduled message count functionality has been removed for performance reasons

### API Calls

- **GetSubscriptionsAsync**: One API call per subscription to get runtime properties

## Benefits

- **Better Visibility**: Users can see at a glance which subscriptions have active or dead letter messages
- **Fast Performance**: No expensive peek operations that could cause infinite loops or delays
- **Quick Insights**: No need to click into each subscription to see active or dead letter message counts
- **Color Coding**: Visual badges make it easy to identify subscriptions with pending or problematic messages
- **Reliable**: Avoids potential infinite loop issues with scheduled message counting
