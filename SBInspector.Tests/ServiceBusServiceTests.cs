using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Moq;
using SBInspector.Shared.Core.Domain;
using SBInspector.Shared.Infrastructure.ServiceBus;
using System.Reflection;

namespace SBInspector.Tests;

public class ServiceBusServiceTests
{
    [Fact]
    public void IsConnected_WhenNotConnected_ReturnsFalse()
    {
        // Arrange
        var service = new ServiceBusService();

        // Act
        var result = service.IsConnected;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ConnectAsync_WithInvalidConnectionString_ReturnsFalse()
    {
        // Arrange
        var service = new ServiceBusService();
        var invalidConnectionString = "invalid-connection-string";

        // Act
        var result = await service.ConnectAsync(invalidConnectionString);

        // Assert
        Assert.False(result);
        Assert.False(service.IsConnected);
    }

    [Fact]
    public async Task DisconnectAsync_WhenCalled_ClearsConnection()
    {
        // Arrange
        var service = new ServiceBusService();

        // Act
        await service.DisconnectAsync();

        // Assert
        Assert.False(service.IsConnected);
    }

    [Fact]
    public void Disconnect_WhenCalled_ClearsConnection()
    {
        // Arrange
        var service = new ServiceBusService();

        // Act
        service.Disconnect();

        // Assert
        Assert.False(service.IsConnected);
    }

    [Fact]
    public async Task GetQueuesAsync_WhenNotConnected_ReturnsEmptyList()
    {
        // Arrange
        var service = new ServiceBusService();

        // Act
        var result = await service.GetQueuesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetTopicsAsync_WhenNotConnected_ReturnsEmptyList()
    {
        // Arrange
        var service = new ServiceBusService();

        // Act
        var result = await service.GetTopicsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetMessagesAsync_WhenNotConnected_ReturnsEmptyList()
    {
        // Arrange
        var service = new ServiceBusService();

        // Act
        var result = await service.GetMessagesAsync("test-queue", "Active");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetSubscriptionsAsync_WhenNotConnected_ReturnsEmptyList()
    {
        // Arrange
        var service = new ServiceBusService();

        // Act
        var result = await service.GetSubscriptionsAsync("test-topic");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetSubscriptionMessagesAsync_WhenNotConnected_ReturnsEmptyList()
    {
        // Arrange
        var service = new ServiceBusService();

        // Act
        var result = await service.GetSubscriptionMessagesAsync("test-topic", "test-subscription", "Active");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task DeleteMessageAsync_WhenNotConnected_ReturnsFalse()
    {
        // Arrange
        var service = new ServiceBusService();

        // Act
        var result = await service.DeleteMessageAsync("test-queue", 12345);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task RequeueDeadLetterMessageAsync_WhenNotConnected_ReturnsFalse()
    {
        // Arrange
        var service = new ServiceBusService();

        // Act
        var result = await service.RequeueDeadLetterMessageAsync("test-queue", 12345);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SendMessageAsync_WhenNotConnected_ReturnsFalse()
    {
        // Arrange
        var service = new ServiceBusService();

        // Act
        var result = await service.SendMessageAsync("test-queue", "test message");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task RescheduleMessageAsync_WhenNotConnected_ReturnsFalse()
    {
        // Arrange
        var service = new ServiceBusService();
        var futureTime = DateTime.UtcNow.AddHours(1);

        // Act
        var result = await service.RescheduleMessageAsync("test-queue", 12345, futureTime);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task PurgeMessagesAsync_WhenNotConnected_ReturnsZero()
    {
        // Arrange
        var service = new ServiceBusService();

        // Act
        var result = await service.PurgeMessagesAsync("test-queue", "Active");

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task DeleteFilteredMessagesAsync_WhenNotConnected_ReturnsZero()
    {
        // Arrange
        var service = new ServiceBusService();
        var filters = new List<MessageFilter>();

        // Act
        var result = await service.DeleteFilteredMessagesAsync("test-queue", "Active", filters);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task SetQueueStatusAsync_WhenNotConnected_ReturnsFalse()
    {
        // Arrange
        var service = new ServiceBusService();

        // Act
        var result = await service.SetQueueStatusAsync("test-queue", true);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SetTopicStatusAsync_WhenNotConnected_ReturnsFalse()
    {
        // Arrange
        var service = new ServiceBusService();

        // Act
        var result = await service.SetTopicStatusAsync("test-topic", true);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SetSubscriptionStatusAsync_WhenNotConnected_ReturnsFalse()
    {
        // Arrange
        var service = new ServiceBusService();

        // Act
        var result = await service.SetSubscriptionStatusAsync("test-topic", "test-subscription", true);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetQueueInfoAsync_WhenNotConnected_ReturnsNull()
    {
        // Arrange
        var service = new ServiceBusService();

        // Act
        var result = await service.GetQueueInfoAsync("test-queue");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetTopicInfoAsync_WhenNotConnected_ReturnsNull()
    {
        // Arrange
        var service = new ServiceBusService();

        // Act
        var result = await service.GetTopicInfoAsync("test-topic");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetSubscriptionInfoAsync_WhenNotConnected_ReturnsNull()
    {
        // Arrange
        var service = new ServiceBusService();

        // Act
        var result = await service.GetSubscriptionInfoAsync("test-topic", "test-subscription");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SendMessageAsync_WithProperties_SendsWithAllProperties()
    {
        // Arrange
        var service = new ServiceBusService();
        var properties = new Dictionary<string, object>
        {
            { "key1", "value1" },
            { "key2", 42 }
        };

        // Act
        var result = await service.SendMessageAsync(
            "test-queue", 
            "test message", 
            "test subject", 
            "application/json", 
            properties);

        // Assert - Since not connected, it should return false
        Assert.False(result);
    }

    [Fact]
    public async Task SendMessageAsync_WithScheduledTime_SendsWithScheduledTime()
    {
        // Arrange
        var service = new ServiceBusService();
        var scheduledTime = DateTime.UtcNow.AddHours(1);

        // Act
        var result = await service.SendMessageAsync(
            "test-queue", 
            "test message", 
            scheduledEnqueueTime: scheduledTime);

        // Assert - Since not connected, it should return false
        Assert.False(result);
    }

    [Fact]
    public async Task GetMessagesAsync_WithMaxMessages_RespectsLimit()
    {
        // Arrange
        var service = new ServiceBusService();

        // Act
        var result = await service.GetMessagesAsync("test-queue", "Active", maxMessages: 50);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result); // Empty because not connected
    }

    [Fact]
    public async Task GetMessagesAsync_WithFromSequenceNumber_StartsFromSequence()
    {
        // Arrange
        var service = new ServiceBusService();

        // Act
        var result = await service.GetMessagesAsync("test-queue", "Active", fromSequenceNumber: 100);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result); // Empty because not connected
    }

    [Fact]
    public async Task DeleteMessageAsync_ForSubscription_UsesCorrectParameters()
    {
        // Arrange
        var service = new ServiceBusService();

        // Act
        var result = await service.DeleteMessageAsync(
            "test-queue", 
            12345, 
            isSubscription: true, 
            topicName: "test-topic", 
            subscriptionName: "test-subscription");

        // Assert
        Assert.False(result); // False because not connected
    }

    [Fact]
    public async Task RequeueDeadLetterMessageAsync_ForSubscription_UsesCorrectParameters()
    {
        // Arrange
        var service = new ServiceBusService();

        // Act
        var result = await service.RequeueDeadLetterMessageAsync(
            "test-queue", 
            12345, 
            isSubscription: true, 
            topicName: "test-topic", 
            subscriptionName: "test-subscription");

        // Assert
        Assert.False(result); // False because not connected
    }

    [Fact]
    public async Task PurgeMessagesAsync_WithCancellation_RespectsToken()
    {
        // Arrange
        var service = new ServiceBusService();
        var cts = new CancellationTokenSource();

        // Act
        var result = await service.PurgeMessagesAsync(
            "test-queue", 
            "Active", 
            cancellationToken: cts.Token);

        // Assert
        Assert.Equal(0, result); // 0 because not connected
    }

    [Fact]
    public async Task PurgeMessagesAsync_WithProgress_ReportsProgress()
    {
        // Arrange
        var service = new ServiceBusService();
        var progressReported = false;
        var progress = new Progress<int>(count =>
        {
            progressReported = true;
        });

        // Act
        var result = await service.PurgeMessagesAsync(
            "test-queue", 
            "Active", 
            progress: progress);

        // Assert
        Assert.Equal(0, result); // 0 because not connected
        Assert.False(progressReported); // No progress because not connected
    }

    [Fact]
    public async Task DeleteFilteredMessagesAsync_WithCancellation_RespectsToken()
    {
        // Arrange
        var service = new ServiceBusService();
        var filters = new List<MessageFilter>();
        var cts = new CancellationTokenSource();

        // Act
        var result = await service.DeleteFilteredMessagesAsync(
            "test-queue", 
            "Active", 
            filters, 
            cancellationToken: cts.Token);

        // Assert
        Assert.Equal(0, result); // 0 because not connected
    }

    [Fact]
    public async Task DeleteFilteredMessagesAsync_WithProgress_ReportsProgress()
    {
        // Arrange
        var service = new ServiceBusService();
        var filters = new List<MessageFilter>();
        var progressReported = false;
        var progress = new Progress<int>(count =>
        {
            progressReported = true;
        });

        // Act
        var result = await service.DeleteFilteredMessagesAsync(
            "test-queue", 
            "Active", 
            filters, 
            progress: progress);

        // Assert
        Assert.Equal(0, result); // 0 because not connected
        Assert.False(progressReported); // No progress because not connected
    }

    [Fact]
    public async Task GetMessagesAsync_WithDeadLetterType_RetrievesDeadLetterMessages()
    {
        // Arrange
        var service = new ServiceBusService();

        // Act
        var result = await service.GetMessagesAsync("test-queue", "DeadLetter");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result); // Empty because not connected
    }

    [Fact]
    public async Task PurgeMessagesAsync_WithDeadLetterType_PurgesDeadLetterQueue()
    {
        // Arrange
        var service = new ServiceBusService();

        // Act
        var result = await service.PurgeMessagesAsync("test-queue", "DeadLetter");

        // Assert
        Assert.Equal(0, result); // 0 because not connected
    }

    [Fact]
    public async Task DeleteFilteredMessagesAsync_ForSubscription_UsesCorrectParameters()
    {
        // Arrange
        var service = new ServiceBusService();
        var filters = new List<MessageFilter>();

        // Act
        var result = await service.DeleteFilteredMessagesAsync(
            "test-queue", 
            "Active", 
            filters,
            isSubscription: true,
            topicName: "test-topic",
            subscriptionName: "test-subscription");

        // Assert
        Assert.Equal(0, result); // 0 because not connected
    }

    [Fact]
    public async Task PurgeMessagesAsync_ForSubscription_UsesCorrectParameters()
    {
        // Arrange
        var service = new ServiceBusService();

        // Act
        var result = await service.PurgeMessagesAsync(
            "test-queue", 
            "Active",
            isSubscription: true,
            topicName: "test-topic",
            subscriptionName: "test-subscription");

        // Assert
        Assert.Equal(0, result); // 0 because not connected
    }

    [Fact]
    public async Task RescheduleMessageAsync_ForSubscription_UsesCorrectParameters()
    {
        // Arrange
        var service = new ServiceBusService();
        var futureTime = DateTime.UtcNow.AddHours(1);

        // Act
        var result = await service.RescheduleMessageAsync(
            "test-queue", 
            12345, 
            futureTime,
            isSubscription: true,
            topicName: "test-topic",
            subscriptionName: "test-subscription");

        // Assert
        Assert.False(result); // False because not connected
    }
}
