namespace SBInspector.Shared.Core.Domain;

/// <summary>
/// Options for performing operations on Service Bus messages
/// </summary>
public class MessageOperationOptions
{
    /// <summary>
    /// The entity name (queue or topic name)
    /// </summary>
    public string EntityName { get; set; } = string.Empty;

    /// <summary>
    /// The sequence number of the message to operate on
    /// </summary>
    public long SequenceNumber { get; set; }

    /// <summary>
    /// Whether the entity is a subscription (true) or queue (false)
    /// </summary>
    public bool IsSubscription { get; set; }

    /// <summary>
    /// The topic name (required if IsSubscription is true)
    /// </summary>
    public string? TopicName { get; set; }

    /// <summary>
    /// The subscription name (required if IsSubscription is true)
    /// </summary>
    public string? SubscriptionName { get; set; }

    /// <summary>
    /// Whether the message is in the dead-letter queue
    /// </summary>
    public bool IsDeadLetterQueue { get; set; }

    /// <summary>
    /// Whether to skip peek verification before performing the operation
    /// </summary>
    public bool SkipPeekVerification { get; set; }

    /// <summary>
    /// Creates options from a MessageInfo object
    /// </summary>
    public static MessageOperationOptions FromMessageInfo(MessageInfo message, string entityName, bool isSubscription = false, string? topicName = null, string? subscriptionName = null)
    {
        return new MessageOperationOptions
        {
            EntityName = entityName,
            SequenceNumber = message.SequenceNumber,
            IsSubscription = isSubscription,
            TopicName = topicName,
            SubscriptionName = subscriptionName,
            IsDeadLetterQueue = message.State == "DeadLetter",
            SkipPeekVerification = true // Default to true for UI operations where we already have the sequence number
        };
    }
}
