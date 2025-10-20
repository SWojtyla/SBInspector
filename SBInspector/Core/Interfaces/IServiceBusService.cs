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
    Task<long> GetSubscriptionScheduledMessageCountAsync(string topicName, string subscriptionName);
    Task<List<MessageInfo>> GetSubscriptionMessagesAsync(string topicName, string subscriptionName, string messageType, int maxMessages = 100, long? fromSequenceNumber = null);
}
