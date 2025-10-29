using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using SBInspector.Shared.Core.Domain;
using SBInspector.Shared.Core.Interfaces;
using System.Text;

namespace SBInspector.Shared.Infrastructure.ServiceBus;

public class ServiceBusService : IServiceBusService
{
    private ServiceBusAdministrationClient? _adminClient;
    private ServiceBusClient? _client;
    private string? _connectionString;

    public bool IsConnected => _adminClient != null && _client != null;

    public async Task<bool> ConnectAsync(string connectionString)
    {
        try
        {
            _connectionString = connectionString;
            _adminClient = new ServiceBusAdministrationClient(connectionString);
            _client = new ServiceBusClient(connectionString);
            
            // Test connection by attempting to list queues
            await _adminClient.GetQueuesAsync().AsPages().GetAsyncEnumerator().MoveNextAsync();
            
            return true;
        }
        catch
        {
            _adminClient = null;
            _client = null;
            _connectionString = null;
            return false;
        }
    }

    public async Task DisconnectAsync()
    {
        _adminClient = null;
        
        if (_client != null)
        {
            await _client.DisposeAsync();
            _client = null;
        }
        
        _connectionString = null;
    }
    
    public void Disconnect()
    {
        _adminClient = null;
        _client?.DisposeAsync().AsTask().Wait();
        _client = null;
        _connectionString = null;
    }

    public async Task<List<EntityInfo>> GetQueuesAsync()
    {
        if (_adminClient == null) return new List<EntityInfo>();

        var queues = new List<EntityInfo>();
        await foreach (var queue in _adminClient.GetQueuesAsync())
        {
            // Get runtime properties to access message counts
            var runtimeProps = await _adminClient.GetQueueRuntimePropertiesAsync(queue.Name);
            
            queues.Add(new EntityInfo
            {
                Name = queue.Name,
                Type = "Queue",
                ActiveMessageCount = runtimeProps.Value.ActiveMessageCount,
                ScheduledMessageCount = runtimeProps.Value.ScheduledMessageCount,
                DeadLetterMessageCount = runtimeProps.Value.DeadLetterMessageCount,
                Status = queue.Status.ToString()
            });
        }
        return queues;
    }

    public async Task<List<EntityInfo>> GetTopicsAsync()
    {
        if (_adminClient == null) return new List<EntityInfo>();

        var topics = new List<EntityInfo>();
        await foreach (var topic in _adminClient.GetTopicsAsync())
        {
            topics.Add(new EntityInfo
            {
                Name = topic.Name,
                Type = "Topic",
                // Note: Topics don't have message counts directly, subscriptions do
                ActiveMessageCount = 0,
                ScheduledMessageCount = 0,
                DeadLetterMessageCount = 0,
                Status = topic.Status.ToString()
            });
        }
        return topics;
    }

    public async Task<List<MessageInfo>> GetMessagesAsync(string entityName, string messageType, int maxMessages = 100, long? fromSequenceNumber = null)
    {
        if (_client == null) return new List<MessageInfo>();

        var messages = new List<MessageInfo>();
        ServiceBusReceiver? receiver = null;

        try
        {
            var options = new ServiceBusReceiverOptions
            {
                ReceiveMode = ServiceBusReceiveMode.PeekLock
            };

            if (messageType == "DeadLetter")
            {
                options.SubQueue = SubQueue.DeadLetter;
                receiver = _client.CreateReceiver(entityName, options);
            }
            else
            {
                receiver = _client.CreateReceiver(entityName, options);
            }

            // Azure Service Bus PeekMessagesAsync has a maximum limit of 256 messages per call
            // To fetch more than 256 messages, we need to make multiple calls
            const int maxMessagesPerPeek = 256;
            int remainingMessages = maxMessages;
            long? currentSequenceNumber = fromSequenceNumber;
            
            while (remainingMessages > 0)
            {
                int messagesToFetch = Math.Min(remainingMessages, maxMessagesPerPeek);
                var receivedMessages = await receiver.PeekMessagesAsync(messagesToFetch, currentSequenceNumber);
                
                if (receivedMessages.Count == 0)
                {
                    break; // No more messages available
                }
                
                foreach (var message in receivedMessages)
                {
                    var body = message.Body.ToString();
                    
                    var messageInfo = new MessageInfo
                    {
                        MessageId = message.MessageId ?? string.Empty,
                        Subject = message.Subject ?? string.Empty,
                        ContentType = message.ContentType ?? string.Empty,
                        Body = body,
                        EnqueuedTime = message.EnqueuedTime.DateTime,
                        ScheduledEnqueueTime = message.ScheduledEnqueueTime == DateTimeOffset.MinValue ? null : message.ScheduledEnqueueTime.DateTime,
                        SequenceNumber = message.SequenceNumber,
                        DeliveryCount = message.DeliveryCount,
                        State = messageType,
                        Properties = new Dictionary<string, object>()
                    };

                    foreach (var prop in message.ApplicationProperties)
                    {
                        messageInfo.Properties[prop.Key] = prop.Value;
                    }

                    messages.Add(messageInfo);
                }
                
                remainingMessages -= receivedMessages.Count;
                
                // Set the next sequence number to continue from
                if (receivedMessages.Count > 0)
                {
                    currentSequenceNumber = receivedMessages.Last().SequenceNumber + 1;
                }
            }
        }
        finally
        {
            if (receiver != null)
            {
                await receiver.DisposeAsync();
            }
        }

        return messages;
    }

    public async Task<List<SubscriptionInfo>> GetSubscriptionsAsync(string topicName)
    {
        if (_adminClient == null) return new List<SubscriptionInfo>();

        var subscriptions = new List<SubscriptionInfo>();
        await foreach (var subscription in _adminClient.GetSubscriptionsAsync(topicName))
        {
            // Get runtime properties to access message counts
            var runtimeProps = await _adminClient.GetSubscriptionRuntimePropertiesAsync(topicName, subscription.SubscriptionName);
            
            subscriptions.Add(new SubscriptionInfo
            {
                Name = subscription.SubscriptionName,
                ActiveMessageCount = runtimeProps.Value.ActiveMessageCount,
                DeadLetterMessageCount = runtimeProps.Value.DeadLetterMessageCount,
                Status = subscription.Status.ToString()
            });
        }
        return subscriptions;
    }

    public async Task<List<MessageInfo>> GetSubscriptionMessagesAsync(string topicName, string subscriptionName, string messageType, int maxMessages = 100, long? fromSequenceNumber = null)
    {
        if (_client == null) return new List<MessageInfo>();

        var messages = new List<MessageInfo>();
        ServiceBusReceiver? receiver = null;

        try
        {
            var options = new ServiceBusReceiverOptions
            {
                ReceiveMode = ServiceBusReceiveMode.PeekLock
            };

            if (messageType == "DeadLetter")
            {
                options.SubQueue = SubQueue.DeadLetter;
            }

            receiver = _client.CreateReceiver(topicName, subscriptionName, options);

            // Azure Service Bus PeekMessagesAsync has a maximum limit of 256 messages per call
            // To fetch more than 256 messages, we need to make multiple calls
            const int maxMessagesPerPeek = 256;
            int remainingMessages = maxMessages;
            long? currentSequenceNumber = fromSequenceNumber;
            
            while (remainingMessages > 0)
            {
                int messagesToFetch = Math.Min(remainingMessages, maxMessagesPerPeek);
                var receivedMessages = await receiver.PeekMessagesAsync(messagesToFetch, currentSequenceNumber);
                
                if (receivedMessages.Count == 0)
                {
                    break; // No more messages available
                }
                
                foreach (var message in receivedMessages)
                {
                    var body = message.Body.ToString();
                    
                    var messageInfo = new MessageInfo
                    {
                        MessageId = message.MessageId ?? string.Empty,
                        Subject = message.Subject ?? string.Empty,
                        ContentType = message.ContentType ?? string.Empty,
                        Body = body,
                        EnqueuedTime = message.EnqueuedTime.DateTime,
                        ScheduledEnqueueTime = message.ScheduledEnqueueTime == DateTimeOffset.MinValue ? null : message.ScheduledEnqueueTime.DateTime,
                        SequenceNumber = message.SequenceNumber,
                        DeliveryCount = message.DeliveryCount,
                        State = messageType,
                        Properties = new Dictionary<string, object>()
                    };

                    foreach (var prop in message.ApplicationProperties)
                    {
                        messageInfo.Properties[prop.Key] = prop.Value;
                    }

                    messages.Add(messageInfo);
                }
                
                remainingMessages -= receivedMessages.Count;
                
                // Set the next sequence number to continue from
                if (receivedMessages.Count > 0)
                {
                    currentSequenceNumber = receivedMessages.Last().SequenceNumber + 1;
                }
            }
        }
        finally
        {
            if (receiver != null)
            {
                await receiver.DisposeAsync();
            }
        }

        return messages;
    }

    public async Task<bool> DeleteMessageAsync(string entityName, long sequenceNumber, bool isSubscription = false, string? topicName = null, string? subscriptionName = null)
    {
        if (_client == null) return false;

        ServiceBusReceiver? receiver = null;

        try
        {
            var options = new ServiceBusReceiverOptions
            {
                ReceiveMode = ServiceBusReceiveMode.PeekLock
            };

            // Create the appropriate receiver
            if (isSubscription && !string.IsNullOrEmpty(topicName) && !string.IsNullOrEmpty(subscriptionName))
            {
                receiver = _client.CreateReceiver(topicName, subscriptionName, options);
            }
            else
            {
                receiver = _client.CreateReceiver(entityName, options);
            }

            // First, use Peek to verify the message exists
            bool messageExists = false;
            long? startSequence = null;
            const int peekBatchSize = 256;
            int maxPeekBatches = 20; // Check up to 5120 messages
            
            for (int i = 0; i < maxPeekBatches; i++)
            {
                var peekedMessages = await receiver.PeekMessagesAsync(peekBatchSize, startSequence);
                if (peekedMessages.Count == 0) break;
                
                if (peekedMessages.Any(m => m.SequenceNumber == sequenceNumber))
                {
                    messageExists = true;
                    break;
                }
                
                startSequence = peekedMessages.Last().SequenceNumber + 1;
            }
            
            if (!messageExists)
            {
                return false; // Message not found in queue
            }

            // Now receive messages until we find the target one
            // Track sequence numbers we've already seen to avoid infinite loops
            var seenSequenceNumbers = new HashSet<long>();
            const int receiveBatchSize = 100;
            int maxReceiveBatches = 100; // Increased limit
            int emptyBatchCount = 0;
            const int maxEmptyBatches = 3;
            
            for (int i = 0; i < maxReceiveBatches; i++)
            {
                var messages = await receiver.ReceiveMessagesAsync(
                    maxMessages: receiveBatchSize, 
                    maxWaitTime: TimeSpan.FromSeconds(5)
                );
                
                if (messages.Count == 0)
                {
                    emptyBatchCount++;
                    if (emptyBatchCount >= maxEmptyBatches)
                    {
                        return false; // Message not available after multiple attempts
                    }
                    await Task.Delay(1000);
                    continue;
                }
                
                emptyBatchCount = 0;
                
                // Check if we've seen all these messages before (infinite loop detection)
                bool allSeen = messages.All(m => seenSequenceNumbers.Contains(m.SequenceNumber));
                if (allSeen)
                {
                    // We're stuck in a loop, abandon all and return false
                    foreach (var msg in messages)
                    {
                        await receiver.AbandonMessageAsync(msg);
                    }
                    return false;
                }
                
                var messageToDelete = messages.FirstOrDefault(m => m.SequenceNumber == sequenceNumber);

                if (messageToDelete != null)
                {
                    // Found the message, delete it and abandon the rest
                    await receiver.CompleteMessageAsync(messageToDelete);
                    
                    // Abandon other messages so they're available again
                    foreach (var msg in messages.Where(m => m.SequenceNumber != sequenceNumber))
                    {
                        await receiver.AbandonMessageAsync(msg);
                    }
                    
                    return true;
                }
                
                // Track these sequence numbers and abandon all
                foreach (var msg in messages)
                {
                    seenSequenceNumbers.Add(msg.SequenceNumber);
                    await receiver.AbandonMessageAsync(msg);
                }
                
                // Add a delay to allow messages to become available again
                await Task.Delay(500);
            }

            return false;
        }
        catch
        {
            return false;
        }
        finally
        {
            if (receiver != null)
            {
                await receiver.DisposeAsync();
            }
        }
    }

    public async Task<bool> RequeueDeadLetterMessageAsync(string entityName, long sequenceNumber, bool isSubscription = false, string? topicName = null, string? subscriptionName = null)
    {
        if (_client == null) return false;

        ServiceBusReceiver? dlqReceiver = null;
        ServiceBusSender? sender = null;

        try
        {
            var options = new ServiceBusReceiverOptions
            {
                ReceiveMode = ServiceBusReceiveMode.PeekLock,
                SubQueue = SubQueue.DeadLetter
            };

            // Create dead-letter queue receiver
            if (isSubscription && !string.IsNullOrEmpty(topicName) && !string.IsNullOrEmpty(subscriptionName))
            {
                dlqReceiver = _client.CreateReceiver(topicName, subscriptionName, options);
                sender = _client.CreateSender(topicName);
            }
            else
            {
                dlqReceiver = _client.CreateReceiver(entityName, options);
                sender = _client.CreateSender(entityName);
            }

            // First, use Peek to verify the message exists
            bool messageExists = false;
            long? startSequence = null;
            const int peekBatchSize = 256;
            int maxPeekBatches = 20;
            
            for (int i = 0; i < maxPeekBatches; i++)
            {
                var peekedMessages = await dlqReceiver.PeekMessagesAsync(peekBatchSize, startSequence);
                if (peekedMessages.Count == 0) break;
                
                if (peekedMessages.Any(m => m.SequenceNumber == sequenceNumber))
                {
                    messageExists = true;
                    break;
                }
                
                startSequence = peekedMessages.Last().SequenceNumber + 1;
            }
            
            if (!messageExists)
            {
                return false;
            }

            // Now receive messages until we find the target one
            // Track sequence numbers to avoid infinite loops
            var seenSequenceNumbers = new HashSet<long>();
            const int batchSize = 100;
            int maxBatchesToSearch = 100;
            int emptyBatchCount = 0;
            const int maxEmptyBatches = 3;
            
            for (int i = 0; i < maxBatchesToSearch; i++)
            {
                var messages = await dlqReceiver.ReceiveMessagesAsync(
                    maxMessages: batchSize, 
                    maxWaitTime: TimeSpan.FromSeconds(5)
                );
                
                if (messages.Count == 0)
                {
                    emptyBatchCount++;
                    if (emptyBatchCount >= maxEmptyBatches)
                    {
                        return false;
                    }
                    await Task.Delay(1000);
                    continue;
                }
                
                emptyBatchCount = 0;
                
                // Check for infinite loop
                bool allSeen = messages.All(m => seenSequenceNumbers.Contains(m.SequenceNumber));
                if (allSeen)
                {
                    foreach (var msg in messages)
                    {
                        await dlqReceiver.AbandonMessageAsync(msg);
                    }
                    return false;
                }
                
                var messageToRequeue = messages.FirstOrDefault(m => m.SequenceNumber == sequenceNumber);

                if (messageToRequeue != null)
                {
                    // Create a new message with the same content
                    var newMessage = new ServiceBusMessage(messageToRequeue.Body)
                    {
                        Subject = messageToRequeue.Subject,
                        ContentType = messageToRequeue.ContentType,
                        MessageId = messageToRequeue.MessageId
                    };

                    // Copy application properties
                    foreach (var prop in messageToRequeue.ApplicationProperties)
                    {
                        newMessage.ApplicationProperties[prop.Key] = prop.Value;
                    }

                    // Send to main queue and complete the DLQ message
                    await sender.SendMessageAsync(newMessage);
                    await dlqReceiver.CompleteMessageAsync(messageToRequeue);
                    
                    // Abandon other messages so they're available again
                    foreach (var msg in messages.Where(m => m.SequenceNumber != sequenceNumber))
                    {
                        await dlqReceiver.AbandonMessageAsync(msg);
                    }
                    
                    return true;
                }
                
                // Track and abandon all messages
                foreach (var msg in messages)
                {
                    seenSequenceNumbers.Add(msg.SequenceNumber);
                    await dlqReceiver.AbandonMessageAsync(msg);
                }
                
                await Task.Delay(500);
            }

            return false;
        }
        catch
        {
            return false;
        }
        finally
        {
            if (dlqReceiver != null)
            {
                await dlqReceiver.DisposeAsync();
            }
            if (sender != null)
            {
                await sender.DisposeAsync();
            }
        }
    }

    public async Task<bool> SendMessageAsync(string entityName, string messageBody, string? subject = null, string? contentType = null, Dictionary<string, object>? properties = null, DateTime? scheduledEnqueueTime = null)
    {
        if (_client == null) return false;

        ServiceBusSender? sender = null;

        try
        {
            sender = _client.CreateSender(entityName);

            var message = new ServiceBusMessage(messageBody)
            {
                Subject = subject ?? string.Empty,
                ContentType = contentType ?? "text/plain"
            };

            // Add application properties
            if (properties != null)
            {
                foreach (var prop in properties)
                {
                    message.ApplicationProperties[prop.Key] = prop.Value;
                }
            }

            // Send with or without scheduling
            if (scheduledEnqueueTime.HasValue)
            {
                await sender.ScheduleMessageAsync(message, scheduledEnqueueTime.Value);
            }
            else
            {
                await sender.SendMessageAsync(message);
            }

            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            if (sender != null)
            {
                await sender.DisposeAsync();
            }
        }
    }

    public async Task<bool> SendMessageToDeadLetterQueueAsync(string entityName, string messageBody, string? subject = null, string? contentType = null, Dictionary<string, object>? properties = null, bool isSubscription = false, string? topicName = null, string? subscriptionName = null)
    {
        if (_client == null) return false;

        ServiceBusSender? sender = null;

        try
        {
            // Create sender for the entity (queue or topic)
            if (isSubscription && !string.IsNullOrEmpty(topicName) && !string.IsNullOrEmpty(subscriptionName))
            {
                sender = _client.CreateSender(topicName);
            }
            else
            {
                sender = _client.CreateSender(entityName);
            }

            // Create the message
            var message = new ServiceBusMessage(messageBody)
            {
                Subject = subject ?? string.Empty,
                ContentType = contentType ?? "text/plain"
            };

            // Add application properties
            if (properties != null)
            {
                foreach (var prop in properties)
                {
                    message.ApplicationProperties[prop.Key] = prop.Value;
                }
            }

            // Send the message to the regular queue/topic first
            await sender.SendMessageAsync(message);

            // Now move it to the dead letter queue
            // We need to receive it and dead-letter it
            ServiceBusReceiver? receiver = null;
            try
            {
                var receiverOptions = new ServiceBusReceiverOptions
                {
                    ReceiveMode = ServiceBusReceiveMode.PeekLock
                };

                if (isSubscription && !string.IsNullOrEmpty(topicName) && !string.IsNullOrEmpty(subscriptionName))
                {
                    receiver = _client.CreateReceiver(topicName, subscriptionName, receiverOptions);
                }
                else
                {
                    receiver = _client.CreateReceiver(entityName, receiverOptions);
                }

                // Wait a bit for the message to be available
                await Task.Delay(100);

                // Receive messages to find the one we just sent
                var receivedMessages = await receiver.ReceiveMessagesAsync(maxMessages: 10, maxWaitTime: TimeSpan.FromSeconds(5));
                
                // Find our message by matching the body and subject
                var targetMessage = receivedMessages.FirstOrDefault(m => 
                    m.Body.ToString() == messageBody && 
                    m.Subject == (subject ?? string.Empty));

                if (targetMessage != null)
                {
                    // Dead-letter the message
                    await receiver.DeadLetterMessageAsync(targetMessage);
                    
                    // Abandon other messages
                    foreach (var msg in receivedMessages.Where(m => m.MessageId != targetMessage.MessageId))
                    {
                        await receiver.AbandonMessageAsync(msg);
                    }
                    
                    return true;
                }
                else
                {
                    // If we can't find the message, abandon all and return false
                    foreach (var msg in receivedMessages)
                    {
                        await receiver.AbandonMessageAsync(msg);
                    }
                    return false;
                }
            }
            finally
            {
                if (receiver != null)
                {
                    await receiver.DisposeAsync();
                }
            }
        }
        catch
        {
            return false;
        }
        finally
        {
            if (sender != null)
            {
                await sender.DisposeAsync();
            }
        }
    }

    public async Task<bool> RescheduleMessageAsync(string entityName, long sequenceNumber, DateTime newScheduledTime, bool isSubscription = false, string? topicName = null, string? subscriptionName = null)
    {
        if (_client == null) return false;

        ServiceBusReceiver? receiver = null;
        ServiceBusSender? sender = null;

        try
        {
            var options = new ServiceBusReceiverOptions
            {
                ReceiveMode = ServiceBusReceiveMode.PeekLock
            };

            // Create receiver and sender
            if (isSubscription && !string.IsNullOrEmpty(topicName) && !string.IsNullOrEmpty(subscriptionName))
            {
                receiver = _client.CreateReceiver(topicName, subscriptionName, options);
                sender = _client.CreateSender(topicName);
            }
            else
            {
                receiver = _client.CreateReceiver(entityName, options);
                sender = _client.CreateSender(entityName);
            }

            // First, use Peek to verify the message exists
            bool messageExists = false;
            long? startSequence = null;
            const int peekBatchSize = 256;
            int maxPeekBatches = 20;
            
            for (int i = 0; i < maxPeekBatches; i++)
            {
                var peekedMessages = await receiver.PeekMessagesAsync(peekBatchSize, startSequence);
                if (peekedMessages.Count == 0) break;
                
                if (peekedMessages.Any(m => m.SequenceNumber == sequenceNumber))
                {
                    messageExists = true;
                    break;
                }
                
                startSequence = peekedMessages.Last().SequenceNumber + 1;
            }
            
            if (!messageExists)
            {
                return false;
            }

            // Now receive messages until we find the target one
            // Track sequence numbers to avoid infinite loops
            var seenSequenceNumbers = new HashSet<long>();
            const int batchSize = 100;
            int maxBatchesToSearch = 100;
            int emptyBatchCount = 0;
            const int maxEmptyBatches = 3;
            
            for (int i = 0; i < maxBatchesToSearch; i++)
            {
                var messages = await receiver.ReceiveMessagesAsync(
                    maxMessages: batchSize, 
                    maxWaitTime: TimeSpan.FromSeconds(5)
                );
                
                if (messages.Count == 0)
                {
                    emptyBatchCount++;
                    if (emptyBatchCount >= maxEmptyBatches)
                    {
                        return false;
                    }
                    await Task.Delay(1000);
                    continue;
                }
                
                emptyBatchCount = 0;
                
                // Check for infinite loop
                bool allSeen = messages.All(m => seenSequenceNumbers.Contains(m.SequenceNumber));
                if (allSeen)
                {
                    foreach (var msg in messages)
                    {
                        await receiver.AbandonMessageAsync(msg);
                    }
                    return false;
                }
                
                var messageToReschedule = messages.FirstOrDefault(m => m.SequenceNumber == sequenceNumber);

                if (messageToReschedule != null)
                {
                    // Create a new message with the same content
                    var newMessage = new ServiceBusMessage(messageToReschedule.Body)
                    {
                        Subject = messageToReschedule.Subject,
                        ContentType = messageToReschedule.ContentType,
                        MessageId = messageToReschedule.MessageId
                    };

                    // Copy application properties
                    foreach (var prop in messageToReschedule.ApplicationProperties)
                    {
                        newMessage.ApplicationProperties[prop.Key] = prop.Value;
                    }

                    // Schedule with new time and complete the old message
                    await sender.ScheduleMessageAsync(newMessage, newScheduledTime);
                    await receiver.CompleteMessageAsync(messageToReschedule);
                    
                    // Abandon other messages so they're available again
                    foreach (var msg in messages.Where(m => m.SequenceNumber != sequenceNumber))
                    {
                        await receiver.AbandonMessageAsync(msg);
                    }
                    
                    return true;
                }
                
                // Track and abandon all messages
                foreach (var msg in messages)
                {
                    seenSequenceNumbers.Add(msg.SequenceNumber);
                    await receiver.AbandonMessageAsync(msg);
                }
                
                await Task.Delay(500);
            }

            return false;
        }
        catch
        {
            return false;
        }
        finally
        {
            if (receiver != null)
            {
                await receiver.DisposeAsync();
            }
            if (sender != null)
            {
                await sender.DisposeAsync();
            }
        }
    }

    public async Task<int> PurgeMessagesAsync(string entityName, string messageType, bool isSubscription = false, string? topicName = null, string? subscriptionName = null, CancellationToken cancellationToken = default, IProgress<int>? progress = null)
    {
        if (_client == null) return 0;

        ServiceBusReceiver? receiver = null;
        int totalDeleted = 0;

        try
        {
            var options = new ServiceBusReceiverOptions
            {
                ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete // Use ReceiveAndDelete for efficient bulk deletion
            };

            // Handle dead-letter queue
            if (messageType == "DeadLetter")
            {
                options.SubQueue = SubQueue.DeadLetter;
            }

            // Create the appropriate receiver
            if (isSubscription && !string.IsNullOrEmpty(topicName) && !string.IsNullOrEmpty(subscriptionName))
            {
                receiver = _client.CreateReceiver(topicName, subscriptionName, options);
            }
            else
            {
                receiver = _client.CreateReceiver(entityName, options);
            }

            // Keep receiving and deleting messages in batches until no more messages
            int emptyBatchCount = 0;
            const int maxEmptyBatches = 3;
            
            while (true)
            {
                // Check for cancellation
                cancellationToken.ThrowIfCancellationRequested();
                
                var messages = await receiver.ReceiveMessagesAsync(maxMessages: 100, maxWaitTime: TimeSpan.FromSeconds(5));
                
                if (messages.Count == 0)
                {
                    emptyBatchCount++;
                    if (emptyBatchCount >= maxEmptyBatches)
                    {
                        break; // No more messages to delete after multiple attempts
                    }
                    await Task.Delay(1000, cancellationToken); // Wait a bit before retrying
                    continue;
                }

                emptyBatchCount = 0; // Reset counter when we receive messages
                totalDeleted += messages.Count;
                
                // Report progress
                progress?.Report(totalDeleted);
                
                // Messages are automatically deleted when using ReceiveAndDelete mode
                // Add a small delay to avoid overwhelming the service
                if (messages.Count == 100)
                {
                    await Task.Delay(100, cancellationToken);
                }
            }

            return totalDeleted;
        }
        catch (OperationCanceledException)
        {
            // Return partial count if cancelled
            return totalDeleted;
        }
        catch
        {
            return totalDeleted; // Return partial count if error occurs
        }
        finally
        {
            if (receiver != null)
            {
                await receiver.DisposeAsync();
            }
        }
    }

    public async Task<int> DeleteFilteredMessagesAsync(string entityName, string messageType, List<MessageFilter> filters, bool isSubscription = false, string? topicName = null, string? subscriptionName = null, CancellationToken cancellationToken = default, IProgress<int>? progress = null)
    {
        if (_client == null) return 0;

        ServiceBusReceiver? receiver = null;
        int totalDeleted = 0;

        try
        {
            var options = new ServiceBusReceiverOptions
            {
                ReceiveMode = ServiceBusReceiveMode.PeekLock
            };

            // Handle dead-letter queue
            if (messageType == "DeadLetter")
            {
                options.SubQueue = SubQueue.DeadLetter;
            }

            // Create the appropriate receiver
            if (isSubscription && !string.IsNullOrEmpty(topicName) && !string.IsNullOrEmpty(subscriptionName))
            {
                receiver = _client.CreateReceiver(topicName, subscriptionName, options);
            }
            else
            {
                receiver = _client.CreateReceiver(entityName, options);
            }

            // Create a filter service to check messages
            var filterService = new Application.Services.MessageFilterService();

            // Keep receiving and filtering messages in batches until no more messages
            int emptyBatchCount = 0;
            const int maxEmptyBatches = 3;
            
            while (true)
            {
                // Check for cancellation
                cancellationToken.ThrowIfCancellationRequested();
                
                // Receive messages with PeekLock mode - increased timeout
                var receivedMessages = await receiver.ReceiveMessagesAsync(maxMessages: 100, maxWaitTime: TimeSpan.FromSeconds(5));
                
                if (receivedMessages.Count == 0)
                {
                    emptyBatchCount++;
                    if (emptyBatchCount >= maxEmptyBatches)
                    {
                        break; // No more messages to process after multiple attempts
                    }
                    await Task.Delay(1000, cancellationToken); // Wait before retrying
                    continue;
                }

                emptyBatchCount = 0; // Reset counter when we receive messages

                // Convert to MessageInfo for filtering
                var messageInfos = new List<MessageInfo>();
                var messageMap = new Dictionary<long, ServiceBusReceivedMessage>();
                
                foreach (var msg in receivedMessages)
                {
                    var messageInfo = new MessageInfo
                    {
                        MessageId = msg.MessageId ?? string.Empty,
                        Subject = msg.Subject ?? string.Empty,
                        ContentType = msg.ContentType ?? string.Empty,
                        Body = msg.Body.ToString(),
                        EnqueuedTime = msg.EnqueuedTime.DateTime,
                        ScheduledEnqueueTime = msg.ScheduledEnqueueTime == DateTimeOffset.MinValue ? null : msg.ScheduledEnqueueTime.DateTime,
                        SequenceNumber = msg.SequenceNumber,
                        DeliveryCount = msg.DeliveryCount,
                        State = messageType,
                        Properties = new Dictionary<string, object>()
                    };

                    foreach (var prop in msg.ApplicationProperties)
                    {
                        messageInfo.Properties[prop.Key] = prop.Value;
                    }

                    messageInfos.Add(messageInfo);
                    messageMap[msg.SequenceNumber] = msg;
                }

                // Apply filters to find matching messages
                var matchingMessages = filterService.ApplyFilters(messageInfos, filters).ToList();
                
                // Delete (complete) only the matching messages, abandon the rest
                foreach (var matchingMsg in matchingMessages)
                {
                    if (messageMap.TryGetValue(matchingMsg.SequenceNumber, out var serviceBusMessage))
                    {
                        await receiver.CompleteMessageAsync(serviceBusMessage);
                        totalDeleted++;
                        
                        // Report progress after each deletion
                        progress?.Report(totalDeleted);
                    }
                }
                
                // Abandon non-matching messages so they can be received again
                foreach (var msg in receivedMessages)
                {
                    if (!matchingMessages.Any(m => m.SequenceNumber == msg.SequenceNumber))
                    {
                        await receiver.AbandonMessageAsync(msg);
                    }
                }
                
                // Add a delay to allow abandoned messages to become available again
                await Task.Delay(500, cancellationToken);
            }

            return totalDeleted;
        }
        catch (OperationCanceledException)
        {
            // Return partial count if cancelled
            return totalDeleted;
        }
        catch
        {
            return totalDeleted; // Return partial count if error occurs
        }
        finally
        {
            if (receiver != null)
            {
                await receiver.DisposeAsync();
            }
        }
    }

    public async Task<bool> SetQueueStatusAsync(string queueName, bool enabled)
    {
        if (_adminClient == null) return false;

        try
        {
            var queue = await _adminClient.GetQueueAsync(queueName);
            queue.Value.Status = enabled 
                ? Azure.Messaging.ServiceBus.Administration.EntityStatus.Active 
                : Azure.Messaging.ServiceBus.Administration.EntityStatus.Disabled;
            await _adminClient.UpdateQueueAsync(queue.Value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SetTopicStatusAsync(string topicName, bool enabled)
    {
        if (_adminClient == null) return false;

        try
        {
            var topic = await _adminClient.GetTopicAsync(topicName);
            topic.Value.Status = enabled 
                ? Azure.Messaging.ServiceBus.Administration.EntityStatus.Active 
                : Azure.Messaging.ServiceBus.Administration.EntityStatus.Disabled;
            await _adminClient.UpdateTopicAsync(topic.Value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SetSubscriptionStatusAsync(string topicName, string subscriptionName, bool enabled)
    {
        if (_adminClient == null) return false;

        try
        {
            var subscription = await _adminClient.GetSubscriptionAsync(topicName, subscriptionName);
            subscription.Value.Status = enabled 
                ? Azure.Messaging.ServiceBus.Administration.EntityStatus.Active 
                : Azure.Messaging.ServiceBus.Administration.EntityStatus.Disabled;
            await _adminClient.UpdateSubscriptionAsync(subscription.Value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<EntityInfo?> GetQueueInfoAsync(string queueName)
    {
        if (_adminClient == null) return null;

        try
        {
            var queue = await _adminClient.GetQueueAsync(queueName);
            var runtimeProps = await _adminClient.GetQueueRuntimePropertiesAsync(queueName);
            
            return new EntityInfo
            {
                Name = queue.Value.Name,
                Type = "Queue",
                ActiveMessageCount = runtimeProps.Value.ActiveMessageCount,
                ScheduledMessageCount = runtimeProps.Value.ScheduledMessageCount,
                DeadLetterMessageCount = runtimeProps.Value.DeadLetterMessageCount,
                Status = queue.Value.Status.ToString()
            };
        }
        catch
        {
            return null;
        }
    }

    public async Task<EntityInfo?> GetTopicInfoAsync(string topicName)
    {
        if (_adminClient == null) return null;

        try
        {
            var topic = await _adminClient.GetTopicAsync(topicName);
            
            return new EntityInfo
            {
                Name = topic.Value.Name,
                Type = "Topic",
                ActiveMessageCount = 0,
                ScheduledMessageCount = 0,
                DeadLetterMessageCount = 0,
                Status = topic.Value.Status.ToString()
            };
        }
        catch
        {
            return null;
        }
    }

    public async Task<SubscriptionInfo?> GetSubscriptionInfoAsync(string topicName, string subscriptionName)
    {
        if (_adminClient == null) return null;

        try
        {
            var subscription = await _adminClient.GetSubscriptionAsync(topicName, subscriptionName);
            var runtimeProps = await _adminClient.GetSubscriptionRuntimePropertiesAsync(topicName, subscriptionName);
            
            return new SubscriptionInfo
            {
                Name = subscription.Value.SubscriptionName,
                ActiveMessageCount = runtimeProps.Value.ActiveMessageCount,
                DeadLetterMessageCount = runtimeProps.Value.DeadLetterMessageCount,
                Status = subscription.Value.Status.ToString()
            };
        }
        catch
        {
            return null;
        }
    }
}
