# SBInspector

Azure Service Bus Inspector - A Blazor web application for inspecting Azure Service Bus queues, topics, and messages.

## Features

- Connect to Azure Service Bus using a connection string
- View all queues and topics in the namespace
- Inspect messages in queues:
  - Active messages
  - Scheduled messages
  - Dead-letter messages
- View topic subscriptions
- Inspect messages in topic subscriptions:
  - Active messages
  - Scheduled messages
  - Dead-letter messages
- View detailed message information including:
  - Message ID, Subject, Content Type
  - Enqueued time and scheduled enqueue time
  - Sequence number and delivery count
  - Message body
  - Application properties
- Filter messages by application properties (attributes):
  - Filter by attribute name and/or value
  - Support for literal (case-insensitive) or regex pattern matching
  - Real-time filtering with message count display

## Prerequisites

- .NET 9.0 SDK or later
- Azure Service Bus namespace

## Getting Started

1. Clone the repository
2. Navigate to the SBInspector directory
3. Run the application:
   ```bash
   cd SBInspector
   dotnet run
   ```
4. Open your browser and navigate to the URL shown in the console (typically https://localhost:5001 or http://localhost:5000)
5. Enter your Azure Service Bus connection string and click "Connect"
6. Browse queues, topics, and inspect messages

## Azure Service Bus Connection String

You can find your connection string in the Azure Portal:
1. Navigate to your Service Bus namespace
2. Go to "Settings" > "Shared access policies"
3. Select a policy (e.g., RootManageSharedAccessKey)
4. Copy the "Primary Connection String"

The connection string format is:
```
Endpoint=sb://<namespace>.servicebus.windows.net/;SharedAccessKeyName=<keyname>;SharedAccessKey=<key>
```

## Project Structure

- `Models/` - Data models for entities and messages
- `Services/` - Service Bus client management and operations
- `Components/Pages/` - Blazor UI components
- `Components/Layout/` - Layout components

## Dependencies

- Azure.Messaging.ServiceBus - Azure Service Bus SDK for .NET

## Message Filtering

The application supports filtering messages by their application properties (attributes). See [FILTERING.md](FILTERING.md) for detailed documentation on how to use this feature.

## License

MIT