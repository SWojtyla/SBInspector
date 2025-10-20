using SBInspector.Core.Domain;

namespace SBInspector.Core.Interfaces;

public interface IServiceBusService
{
    bool IsConnected { get; }
    Task<bool> ConnectAsync(string connectionString);
    void Disconnect();
    Task<List<EntityInfo>> GetQueuesAsync();
    Task<List<EntityInfo>> GetTopicsAsync();
    Task<List<MessageInfo>> GetMessagesAsync(string entityName, string messageType, int maxMessages = 100, long? fromSequenceNumber = null);
    Task<List<SubscriptionInfo>> GetSubscriptionsAsync(string topicName);
    Task<List<MessageInfo>> GetSubscriptionMessagesAsync(string topicName, string subscriptionName, string messageType, int maxMessages = 100, long? fromSequenceNumber = null);
    
    // CRUD operations
    Task<bool> DeleteMessageAsync(string entityName, long sequenceNumber, bool isSubscription = false, string? topicName = null, string? subscriptionName = null);
    Task<bool> RequeueDeadLetterMessageAsync(string entityName, long sequenceNumber, bool isSubscription = false, string? topicName = null, string? subscriptionName = null);
    Task<bool> SendMessageAsync(string entityName, string messageBody, string? subject = null, string? contentType = null, Dictionary<string, object>? properties = null, DateTime? scheduledEnqueueTime = null);
    Task<bool> RescheduleMessageAsync(string entityName, long sequenceNumber, DateTime newScheduledTime, bool isSubscription = false, string? topicName = null, string? subscriptionName = null);
    Task<int> PurgeMessagesAsync(string entityName, string messageType, bool isSubscription = false, string? topicName = null, string? subscriptionName = null);
    
    // Enable/Disable operations
    Task<bool> SetQueueStatusAsync(string queueName, bool enabled);
    Task<bool> SetTopicStatusAsync(string topicName, bool enabled);
    Task<bool> SetSubscriptionStatusAsync(string topicName, string subscriptionName, bool enabled);
    
    // Refresh counts
    Task<EntityInfo?> GetQueueInfoAsync(string queueName);
    Task<EntityInfo?> GetTopicInfoAsync(string topicName);
    Task<SubscriptionInfo?> GetSubscriptionInfoAsync(string topicName, string subscriptionName);
}
