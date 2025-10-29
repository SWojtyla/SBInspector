# Send Messages to Dead-Letter Queue

SBInspector now supports sending messages directly to the dead-letter queue, allowing you to test dead-letter message handling and simulate failure scenarios without having to actually fail message processing.

## Feature Overview

This feature allows you to create and send messages directly to the dead-letter queue of any queue or topic subscription. This is useful for:

- Testing dead-letter message handling logic
- Simulating failure scenarios
- Creating test data for dead-letter queue monitoring
- Debugging dead-letter message processing workflows

## How to Use

### Sending a Message to Dead-Letter Queue

1. Navigate to a queue or topic subscription in SBInspector
2. Click the **"Send New"** button (green button with + icon)
3. Fill in the message details:
   - **Message Body** (required): The content of your message
   - **Subject** (optional): A label or title for the message
   - **Content Type** (optional): e.g., `application/json`, `text/plain`
   - **Application Properties** (optional): Key-value pairs in the format `key=value` (one per line)
4. Check the **"Send directly to dead-letter queue"** checkbox
5. Click **"Send"**

The message will be sent to the regular queue/topic first, then immediately moved to the dead-letter queue.

### Important Notes

- **Cannot schedule dead-letter messages**: When you check "Send directly to dead-letter queue", the scheduling option is automatically disabled. Dead-letter messages cannot be scheduled for future delivery.
- **Message identification**: The system uses a unique MessageId to reliably identify and move the message to the dead-letter queue.
- **Reliability**: The implementation retries up to 10 times to ensure the message is properly moved to the dead-letter queue.

## UI Changes

### Send Message Modal

The Send Message modal now includes a new checkbox:

```
☑ Send directly to dead-letter queue
```

When this checkbox is selected:
- The message will be sent to the dead-letter queue
- The scheduling option is disabled (grayed out)
- Success message will indicate: "Message sent to dead-letter queue successfully."

### Validation

The following validation rules apply:
- Message body is required (same as before)
- Cannot schedule messages when sending to dead-letter queue
- All other fields (subject, content type, properties) remain optional

## Technical Implementation

### Service Method

The feature is implemented through a new method in `IServiceBusService`:

```csharp
Task<bool> SendMessageToDeadLetterQueueAsync(
    string entityName, 
    string messageBody, 
    string? subject = null, 
    string? contentType = null, 
    Dictionary<string, object>? properties = null, 
    bool isSubscription = false, 
    string? topicName = null, 
    string? subscriptionName = null);
```

### How It Works

1. **Create Message**: Creates a message with a unique GUID as the MessageId
2. **Send to Regular Queue**: Sends the message to the regular queue/topic first
3. **Receive and Dead-Letter**: 
   - Creates a receiver for the entity
   - Attempts to receive messages (up to 10 attempts)
   - Finds the message by matching the unique MessageId
   - Calls `DeadLetterMessageAsync()` with reason "Manual" and description "Message sent directly to dead-letter queue"
   - Abandons any other messages that were received
4. **Return Success/Failure**: Returns true if the message was successfully dead-lettered, false otherwise

### Azure Service Bus Operations

The implementation uses the following Azure Service Bus SDK methods:
- `ServiceBusSender.SendMessageAsync()` - To send the initial message
- `ServiceBusReceiver.ReceiveMessagesAsync()` - To receive the message back
- `ServiceBusReceiver.DeadLetterMessageAsync()` - To move the message to dead-letter queue
- `ServiceBusReceiver.AbandonMessageAsync()` - To release other messages

### Reliability Features

- **Unique MessageId**: Uses GUID to ensure reliable message identification
- **Multiple Attempts**: Retries up to 10 times with 200ms delay between attempts
- **Abandon Other Messages**: Ensures other messages in the queue are not affected
- **Proper Resource Cleanup**: Disposes sender and receiver in finally blocks

## Use Cases

### Testing Dead-Letter Handlers

```
1. Send a test message to dead-letter queue
2. Run your dead-letter message handler
3. Verify it processes the message correctly
```

### Simulating Failures

```
1. Create messages with various failure scenarios
2. Send them to dead-letter queue
3. Test monitoring and alerting systems
4. Verify retry logic and error handling
```

### Training and Demos

```
1. Populate dead-letter queue with sample data
2. Demonstrate dead-letter message handling
3. Show requeue functionality
4. Explain dead-letter best practices
```

## Limitations

1. **No Scheduling**: Messages sent to dead-letter queue cannot be scheduled. They are delivered immediately.

2. **Two-Step Process**: The message is first sent to the regular queue, then moved to dead-letter. There's a brief moment where it exists in the regular queue.

3. **Race Conditions**: In high-volume scenarios, there's a small possibility that another consumer might process the message before it can be moved to dead-letter. The implementation retries to minimize this risk.

4. **Requires Manage Permissions**: Your Service Bus connection must have "Manage" permissions to dead-letter messages.

5. **Topic Subscriptions**: When sending to a topic subscription's dead-letter queue, the message is sent to the topic first, then moved to the specific subscription's dead-letter queue.

## Error Handling

If sending to dead-letter queue fails, you'll see an error message:

```
"Failed to send message to dead-letter queue."
```

Common reasons for failure:
- Message was processed by another consumer before it could be moved
- Connection issues with Azure Service Bus
- Insufficient permissions (Manage rights required)
- Queue or subscription doesn't exist

## Best Practices

1. **Use Descriptive Content**: Include clear information in the message body about why it's being sent to dead-letter for testing purposes.

2. **Add Custom Properties**: Use application properties to tag test messages:
   ```
   TestMessage=true
   CreatedBy=SBInspector
   Purpose=DeadLetterTesting
   ```

3. **Clean Up After Testing**: Remember to delete test messages from the dead-letter queue after testing.

4. **Document Test Scenarios**: Keep notes on what scenarios you're testing when sending messages to dead-letter.

5. **Use in Development/Test**: This feature is primarily for development and testing. Be cautious when using in production environments.

## Comparison with Other Operations

| Operation | Target | Purpose |
|-----------|--------|---------|
| **Send Message** | Regular Queue/Topic | Normal message delivery |
| **Send to Dead-Letter** | Dead-Letter Queue | Testing/Simulation |
| **Requeue from Dead-Letter** | Regular Queue | Retry failed messages |
| **Delete from Dead-Letter** | N/A | Remove messages |

## Example Workflow

### Testing a Dead-Letter Handler

1. **Create Test Message**:
   - Body: `{"orderId": 12345, "error": "Payment failed"}`
   - Subject: "Test Order Failure"
   - Properties: `TestMessage=true`

2. **Send to Dead-Letter**:
   - Check "Send directly to dead-letter queue"
   - Click "Send"

3. **Navigate to Dead-Letter View**:
   - Select "Dead-letter" messages
   - Verify your message appears

4. **Test Your Handler**:
   - Run your dead-letter processing logic
   - Verify it handles the message correctly

5. **Clean Up**:
   - Delete the test message from dead-letter queue

## Troubleshooting

**Message not appearing in dead-letter queue?**
- Wait a few seconds and refresh the message list
- Check if another consumer processed the message
- Verify you have Manage permissions on the namespace

**"Failed to send message to dead-letter queue" error?**
- Check your connection is still active
- Verify the queue/subscription exists
- Ensure you have appropriate permissions
- Try sending again - may be a temporary issue

**Message appears in regular queue instead of dead-letter?**
- The message briefly exists in regular queue before being moved
- If you see it there, wait a moment - it should move to dead-letter
- If it doesn't move, there may have been an error

## Related Features

- **View Dead-Letter Messages**: Navigate to dead-letter queue to see all dead-lettered messages
- **Requeue Messages**: Move messages from dead-letter back to active queue
- **Delete Messages**: Remove messages from dead-letter queue
- **Message Filtering**: Filter dead-letter messages by properties
- **Export Messages**: Export dead-letter messages for analysis

## Integration with Message Templates

You can save messages as templates and use them when sending to dead-letter queue:

1. Create a message with test data
2. Check "Save as template"
3. Send to dead-letter queue
4. Later, load the template and send to dead-letter again

This is useful for repeatedly testing the same scenarios.
