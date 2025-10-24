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

    public void Disconnect()
    {
        _adminClient = null;
        _client?.DisposeAsync();
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

            var receivedMessages = await receiver.PeekMessagesAsync(maxMessages, fromSequenceNumber);
            
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

            var receivedMessages = await receiver.PeekMessagesAsync(maxMessages, fromSequenceNumber);
            
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

            // Receive messages and find the one with matching sequence number
            var messages = await receiver.ReceiveMessagesAsync(maxMessages: 100);
            var messageToDelete = messages.FirstOrDefault(m => m.SequenceNumber == sequenceNumber);

            if (messageToDelete != null)
            {
                await receiver.CompleteMessageAsync(messageToDelete);
                return true;
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

            // Receive messages from dead-letter queue
            var messages = await dlqReceiver.ReceiveMessagesAsync(maxMessages: 100);
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
                return true;
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

            // Find the scheduled message
            var messages = await receiver.ReceiveMessagesAsync(maxMessages: 100);
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
                return true;
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
            while (true)
            {
                // Check for cancellation
                cancellationToken.ThrowIfCancellationRequested();
                
                var messages = await receiver.ReceiveMessagesAsync(maxMessages: 100, maxWaitTime: TimeSpan.FromSeconds(1));
                
                if (messages.Count == 0)
                {
                    break; // No more messages to delete
                }

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

            // Keep receiving and filtering messages in batches
            while (true)
            {
                // Check for cancellation
                cancellationToken.ThrowIfCancellationRequested();
                
                // Receive messages with PeekLock mode
                var receivedMessages = await receiver.ReceiveMessagesAsync(maxMessages: 100, maxWaitTime: TimeSpan.FromSeconds(1));
                
                if (receivedMessages.Count == 0)
                {
                    break; // No more messages to process
                }

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
                
                // Add a small delay to avoid overwhelming the service
                if (receivedMessages.Count == 100)
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
