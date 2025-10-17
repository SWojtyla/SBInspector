using SBInspector.Core.Domain;

namespace SBInspector.Core.Interfaces;

public interface IServiceBusService
{
    bool IsConnected { get; }
    Task<bool> ConnectAsync(string connectionString);
    void Disconnect();
    Task<List<EntityInfo>> GetQueuesAsync();
    Task<List<EntityInfo>> GetTopicsAsync();
    Task<List<MessageInfo>> GetMessagesAsync(string entityName, string messageType, int maxMessages = 100);
    Task<List<string>> GetSubscriptionsAsync(string topicName);
    Task<List<MessageInfo>> GetSubscriptionMessagesAsync(string topicName, string subscriptionName, string messageType, int maxMessages = 100);
}
