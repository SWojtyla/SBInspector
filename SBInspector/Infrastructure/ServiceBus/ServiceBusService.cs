using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using SBInspector.Core.Domain;
using SBInspector.Core.Interfaces;
using System.Text;

namespace SBInspector.Infrastructure.ServiceBus;

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
                DeadLetterMessageCount = runtimeProps.Value.DeadLetterMessageCount
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
                DeadLetterMessageCount = 0
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
                ScheduledMessageCount = -1, // -1 indicates not loaded yet
                DeadLetterMessageCount = runtimeProps.Value.DeadLetterMessageCount
            });
        }
        return subscriptions;
    }

    public async Task<long> GetSubscriptionScheduledMessageCountAsync(string topicName, string subscriptionName)
    {
        if (_client == null) return 0;

        ServiceBusReceiver? receiver = null;
        try
        {
            var options = new ServiceBusReceiverOptions
            {
                ReceiveMode = ServiceBusReceiveMode.PeekLock
            };

            receiver = _client.CreateReceiver(topicName, subscriptionName, options);

            // Peek scheduled messages to count them
            // We peek in batches to count all scheduled messages
            long count = 0;
            long? fromSequenceNumber = null;
            int batchSize = 100;
            
            while (true)
            {
                var messages = await receiver.PeekMessagesAsync(batchSize, fromSequenceNumber);
                
                if (messages.Count == 0)
                    break;

                // Count only scheduled messages (messages with ScheduledEnqueueTime in the future)
                var scheduledMessages = messages.Where(m => 
                    m.ScheduledEnqueueTime > DateTimeOffset.UtcNow).ToList();
                
                count += scheduledMessages.Count;
                
                // If we got fewer messages than the batch size, we've reached the end
                if (messages.Count < batchSize)
                    break;
                    
                // Move to the next batch
                fromSequenceNumber = messages.Max(m => m.SequenceNumber) + 1;
            }

            return count;
        }
        finally
        {
            if (receiver != null)
            {
                await receiver.DisposeAsync();
            }
        }
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
}
