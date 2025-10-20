# Subscription Message Counts Feature

## Overview

This feature adds the display of active and dead letter message counts for Azure Service Bus topic subscriptions, along with scheduled message count at the topic level.

## What Changed

### New Components

- **SubscriptionInfo Domain Model** (`Core/Domain/SubscriptionInfo.cs`): A new domain model that holds subscription information including:
  - `Name`: The subscription name
  - `ActiveMessageCount`: Number of active messages per subscription
  - `DeadLetterMessageCount`: Number of dead letter messages per subscription

### Modified Components

1. **IServiceBusService Interface** (`Core/Interfaces/IServiceBusService.cs`):
   - Changed `GetSubscriptionsAsync` return type from `List<string>` to `List<SubscriptionInfo>`
   - Added `GetTopicScheduledMessageCountAsync` method to count scheduled messages at the topic level

2. **ServiceBusService Implementation** (`Infrastructure/ServiceBus/ServiceBusService.cs`):
   - Updated `GetSubscriptionsAsync` to fetch subscription runtime properties using `GetSubscriptionRuntimePropertiesAsync`
   - Extracts and returns message counts for each subscription (active and dead letter)
   - Implemented `GetTopicScheduledMessageCountAsync` that peeks messages from the topic and counts those with scheduled enqueue times in the future
   - Uses the first subscription to peek messages since scheduled messages exist at the topic level

3. **SubscriptionListPanel Component** (`Presentation/Components/UI/SubscriptionListPanel.razor`):
   - Updated to accept `List<SubscriptionInfo>` instead of `List<string>`
   - Displays Active and Dead Letter message counts per subscription with styled badges
   - Shows Topic-level Scheduled count in the panel header
   - Added "Load Scheduled Count" button in the header to retrieve the topic's scheduled message count on demand
   - Shows loading spinner while counting scheduled messages
   - Message counts use color-coded badges (green for active, yellow for scheduled, red for dead letter)

4. **Home Page** (`Presentation/Components/Pages/Home.razor`):
   - Updated subscriptions field type from `List<string>` to `List<SubscriptionInfo>`
   - Added `topicScheduledMessageCount` field to track scheduled messages at topic level
   - Added `HandleLoadTopicScheduledCount` method to load scheduled count for the topic

## How to Use

When you view subscriptions for a topic:

1. Click "View Subscriptions" on any topic in the Topics tab
2. The subscription panel shows:
   - **Topic header** with:
     - Topic name
     - Subscription count
     - **Topic Scheduled message count** (shows "-" initially, with tooltip)
     - "Load Scheduled Count" button
   - **Each subscription** shows:
     - Subscription name
     - **Active message count** (with green badge if > 0)
     - **Dead letter message count** (with red badge if > 0)
     - Action buttons to view messages
3. Click the **"Load Scheduled Count"** button in the header to retrieve the scheduled message count for the entire topic
4. Once loaded, the scheduled count displays with a color-coded badge (yellow if > 0, gray if 0)

## Technical Details

### Why Scheduled Messages Are at Topic Level

In Azure Service Bus architecture:
- **Topics** receive messages that may have scheduled enqueue times
- **Scheduled messages** wait at the topic level until their scheduled time
- When the scheduled time arrives, the message is delivered to subscriptions based on their filters
- Therefore, all subscriptions see the same scheduled messages - they exist at the topic level, not per subscription

### Implementation Details

**Active and Dead Letter Counts (Per Subscription):**
- Retrieved from `SubscriptionRuntimeProperties` API
- Fast and efficient
- Available immediately on subscription load

**Scheduled Count (Topic Level):**
- Not available in runtime properties for topics or subscriptions
- Retrieved by peeking messages from any subscription (uses first subscription)
- Counts messages where `ScheduledEnqueueTime > DateTimeOffset.UtcNow`
- Peeks in batches of 100 messages
- On-demand to avoid performance impact

### Performance Considerations

- **Initial load**: Fast - only retrieves runtime properties (Active and Dead Letter counts per subscription)
- **Scheduled count load**: Can be slow for topics with many messages
  - Peeks messages in batches of 100
  - Continues until all messages are counted
  - Only done when user clicks "Load Scheduled Count"
  - Shows loading indicator for transparency

### API Calls

- **GetSubscriptionsAsync**: One API call per subscription to get runtime properties
- **GetTopicScheduledMessageCountAsync**: Multiple peek operations (one per 100 messages), only when user requests it

## Benefits

- **Better Visibility**: Users can see at a glance which subscriptions have active or dead letter messages
- **Topic-Level Scheduled Count**: Correctly shows scheduled messages at the topic level where they actually exist
- **Consistency**: Subscription display matches the queue display pattern for active and dead letter counts
- **On-Demand Loading**: Scheduled count loaded only when needed, avoiding performance impact
- **Quick Insights**: No need to click into each subscription to see active or dead letter message counts
- **Color Coding**: Visual badges make it easy to identify subscriptions or topics with pending or problematic messages
- **User Control**: Users decide when to pay the performance cost of counting scheduled messages
- **Architecturally Correct**: Reflects the actual Azure Service Bus architecture where scheduled messages exist at topic level
