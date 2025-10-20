# Message Pagination

## Overview

The message pagination feature allows you to view more than 100 messages from Azure Service Bus queues and subscriptions. You can customize the page size and load additional batches of messages as needed.

## Features

### Customizable Page Size

You can configure how many messages to load at a time by selecting from the page size dropdown in the message panel header:
- 50 messages
- 100 messages (default)
- 200 messages
- 500 messages

The page size setting is applied when you first view messages from a queue or subscription.

### Load More Messages

After the initial batch of messages is loaded, if there are potentially more messages available, a "Load More Messages" button will appear at the bottom of the message list.

Click this button to fetch the next batch of messages. The new messages will be appended to the existing list, allowing you to browse through large message collections without overwhelming the UI.

### How It Works

The pagination feature uses Azure Service Bus's sequence numbers to efficiently retrieve messages:

1. When you first view messages, the system fetches the specified page size worth of messages
2. The system tracks the sequence number of the last message retrieved
3. When you click "Load More", it fetches the next batch starting from the next sequence number
4. This process continues until all available messages are loaded or you close the panel

### Visual Indicators

- **Loading spinner**: Displayed when the initial batch of messages is being fetched
- **Load More button**: Shows when additional messages may be available
- **"Loading more messages..." spinner**: Displayed while fetching additional batches
- **"All available messages loaded"**: Shown when no more messages are available

## Usage Example

1. Connect to your Azure Service Bus namespace
2. Navigate to a queue or topic subscription
3. Click to view Active, Scheduled, or Dead-letter messages
4. (Optional) Change the page size using the dropdown in the header
5. Browse through the loaded messages
6. If the "Load More Messages" button appears, click it to fetch additional messages
7. Repeat step 6 as needed to view all messages

## Technical Details

- Uses Azure Service Bus SDK's `PeekMessagesAsync` with `fromSequenceNumber` parameter
- Implements efficient pagination without loading all messages at once
- Preserves all existing features like filtering, sorting, and viewing message details
- Page size can be changed at any time (takes effect on next message load)

## Benefits

- **Performance**: Load only what you need, reducing initial load time
- **Memory efficiency**: Avoid loading thousands of messages at once
- **Flexibility**: Choose the page size that works best for your use case
- **User control**: Decide when to load more messages based on your needs
