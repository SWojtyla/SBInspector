using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;

namespace SBInspector;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Azure Service Bus Inspector ===");
        Console.WriteLine();

        // Get connection string
        string? connectionString = null;
        if (args.Length > 0)
        {
            connectionString = args[0];
        }
        else
        {
            Console.Write("Enter Azure Service Bus connection string: ");
            connectionString = Console.ReadLine();
        }

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            Console.WriteLine("Error: Connection string is required.");
            return;
        }

        try
        {
            var inspector = new ServiceBusInspector(connectionString);
            await inspector.RunAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

class ServiceBusInspector
{
    private readonly string _connectionString;
    private readonly ServiceBusAdministrationClient _adminClient;

    public ServiceBusInspector(string connectionString)
    {
        _connectionString = connectionString;
        _adminClient = new ServiceBusAdministrationClient(connectionString);
    }

    public async Task RunAsync()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=== Main Menu ===");
            Console.WriteLine("1. List Queues");
            Console.WriteLine("2. List Topics");
            Console.WriteLine("3. Inspect Queue Messages");
            Console.WriteLine("4. Inspect Topic Subscription Messages");
            Console.WriteLine("5. Exit");
            Console.Write("Select an option: ");

            var choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        await ListQueuesAsync();
                        break;
                    case "2":
                        await ListTopicsAsync();
                        break;
                    case "3":
                        await InspectQueueMessagesAsync();
                        break;
                    case "4":
                        await InspectTopicSubscriptionMessagesAsync();
                        break;
                    case "5":
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private async Task ListQueuesAsync()
    {
        Console.WriteLine();
        Console.WriteLine("=== Queues ===");

        var queues = _adminClient.GetQueuesAsync();
        var queueList = new List<string>();

        await foreach (var queue in queues)
        {
            queueList.Add(queue.Name);
            
            // Get runtime properties to access message counts
            var runtimeProperties = await _adminClient.GetQueueRuntimePropertiesAsync(queue.Name);
            
            Console.WriteLine($"  - {queue.Name}");
            Console.WriteLine($"    Active Messages: {runtimeProperties.Value.ActiveMessageCount}");
            Console.WriteLine($"    Scheduled Messages: {runtimeProperties.Value.ScheduledMessageCount}");
            Console.WriteLine($"    Dead Letter Messages: {runtimeProperties.Value.DeadLetterMessageCount}");
        }

        if (queueList.Count == 0)
        {
            Console.WriteLine("  No queues found.");
        }
    }

    private async Task ListTopicsAsync()
    {
        Console.WriteLine();
        Console.WriteLine("=== Topics ===");

        var topics = _adminClient.GetTopicsAsync();
        var topicList = new List<string>();

        await foreach (var topic in topics)
        {
            topicList.Add(topic.Name);
            
            // Get runtime properties to access message counts
            var runtimeProperties = await _adminClient.GetTopicRuntimePropertiesAsync(topic.Name);
            
            Console.WriteLine($"  - {topic.Name}");
            Console.WriteLine($"    Subscriptions: {runtimeProperties.Value.SubscriptionCount}");
            Console.WriteLine($"    Scheduled Messages: {runtimeProperties.Value.ScheduledMessageCount}");
        }

        if (topicList.Count == 0)
        {
            Console.WriteLine("  No topics found.");
        }
    }

    private async Task InspectQueueMessagesAsync()
    {
        Console.Write("Enter queue name: ");
        var queueName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(queueName))
        {
            Console.WriteLine("Queue name is required.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"=== Inspecting Queue: {queueName} ===");
        Console.WriteLine("1. View Active Messages");
        Console.WriteLine("2. View Scheduled Messages");
        Console.WriteLine("3. View Dead Letter Messages");
        Console.Write("Select message type: ");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                await ViewMessagesAsync(queueName, null, SubQueue.None);
                break;
            case "2":
                Console.WriteLine("\nNote: Scheduled messages are included in active messages.");
                Console.WriteLine("They will be delivered at their scheduled time.");
                await ViewMessagesAsync(queueName, null, SubQueue.None, showScheduled: true);
                break;
            case "3":
                await ViewMessagesAsync(queueName, null, SubQueue.DeadLetter);
                break;
            default:
                Console.WriteLine("Invalid option.");
                break;
        }
    }

    private async Task InspectTopicSubscriptionMessagesAsync()
    {
        Console.Write("Enter topic name: ");
        var topicName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(topicName))
        {
            Console.WriteLine("Topic name is required.");
            return;
        }

        // List subscriptions
        Console.WriteLine($"\n=== Subscriptions for Topic: {topicName} ===");
        var subscriptions = _adminClient.GetSubscriptionsAsync(topicName);
        var subscriptionList = new List<string>();

        await foreach (var subscription in subscriptions)
        {
            subscriptionList.Add(subscription.SubscriptionName);
            
            // Get runtime properties to access message counts
            var runtimeProperties = await _adminClient.GetSubscriptionRuntimePropertiesAsync(topicName, subscription.SubscriptionName);
            
            Console.WriteLine($"  - {subscription.SubscriptionName}");
            Console.WriteLine($"    Active Messages: {runtimeProperties.Value.ActiveMessageCount}");
            Console.WriteLine($"    Dead Letter Messages: {runtimeProperties.Value.DeadLetterMessageCount}");
        }

        if (subscriptionList.Count == 0)
        {
            Console.WriteLine("  No subscriptions found.");
            return;
        }

        Console.Write("\nEnter subscription name: ");
        var subscriptionName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(subscriptionName))
        {
            Console.WriteLine("Subscription name is required.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"=== Inspecting Subscription: {subscriptionName} ===");
        Console.WriteLine("1. View Active Messages");
        Console.WriteLine("2. View Dead Letter Messages");
        Console.Write("Select message type: ");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                await ViewMessagesAsync(topicName, subscriptionName, SubQueue.None);
                break;
            case "2":
                await ViewMessagesAsync(topicName, subscriptionName, SubQueue.DeadLetter);
                break;
            default:
                Console.WriteLine("Invalid option.");
                break;
        }
    }

    private async Task ViewMessagesAsync(string entityPath, string? subscriptionName, SubQueue subQueue, bool showScheduled = false)
    {
        await using var client = new ServiceBusClient(_connectionString);
        
        ServiceBusReceiver receiver;
        
        if (string.IsNullOrEmpty(subscriptionName))
        {
            // It's a queue
            receiver = client.CreateReceiver(entityPath, new ServiceBusReceiverOptions
            {
                SubQueue = subQueue,
                ReceiveMode = ServiceBusReceiveMode.PeekLock
            });
        }
        else
        {
            // It's a topic subscription
            receiver = client.CreateReceiver(entityPath, subscriptionName, new ServiceBusReceiverOptions
            {
                SubQueue = subQueue,
                ReceiveMode = ServiceBusReceiveMode.PeekLock
            });
        }

        try
        {
            Console.WriteLine("\nFetching messages (peek mode - messages will not be removed)...");
            
            var messages = new List<ServiceBusReceivedMessage>();
            
            // Peek up to 100 messages
            for (int i = 0; i < 100; i += 10)
            {
                var batch = await receiver.PeekMessagesAsync(maxMessages: 10, fromSequenceNumber: i);
                if (batch.Count == 0)
                    break;
                messages.AddRange(batch);
            }

            if (messages.Count == 0)
            {
                Console.WriteLine("No messages found.");
                return;
            }

            Console.WriteLine($"\nFound {messages.Count} message(s):\n");

            int messageNumber = 1;
            foreach (var message in messages)
            {
                // Filter scheduled messages if needed
                if (showScheduled && message.ScheduledEnqueueTime == DateTimeOffset.MinValue)
                    continue;
                
                if (!showScheduled && message.ScheduledEnqueueTime != DateTimeOffset.MinValue)
                    continue;

                Console.WriteLine($"--- Message {messageNumber} ---");
                Console.WriteLine($"Message ID: {message.MessageId}");
                Console.WriteLine($"Sequence Number: {message.SequenceNumber}");
                Console.WriteLine($"Enqueued Time: {message.EnqueuedTime}");
                
                if (message.ScheduledEnqueueTime != DateTimeOffset.MinValue)
                {
                    Console.WriteLine($"Scheduled Enqueue Time: {message.ScheduledEnqueueTime}");
                }
                
                Console.WriteLine($"Delivery Count: {message.DeliveryCount}");
                Console.WriteLine($"Content Type: {message.ContentType}");
                Console.WriteLine($"Subject: {message.Subject}");
                
                if (message.ApplicationProperties.Count > 0)
                {
                    Console.WriteLine("Application Properties:");
                    foreach (var prop in message.ApplicationProperties)
                    {
                        Console.WriteLine($"  {prop.Key}: {prop.Value}");
                    }
                }

                // Display message body
                try
                {
                    var body = message.Body.ToString();
                    Console.WriteLine($"Body: {body}");
                }
                catch
                {
                    Console.WriteLine("Body: [Binary data]");
                }

                Console.WriteLine();
                messageNumber++;
            }

            Console.WriteLine($"Total messages displayed: {messageNumber - 1}");
        }
        finally
        {
            await receiver.CloseAsync();
        }
    }
}
