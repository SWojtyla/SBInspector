# SBInspector

A .NET console application for inspecting Azure Service Bus queues and topics, including their messages (active, scheduled, and dead-letter).

## Features

- **List Queues**: View all queues in your Azure Service Bus namespace with message counts
- **List Topics**: View all topics with subscription and message counts
- **Inspect Queue Messages**: 
  - View active messages
  - View scheduled messages
  - View dead-letter messages
- **Inspect Topic Subscription Messages**:
  - View active messages in subscriptions
  - View dead-letter messages in subscriptions

## Prerequisites

- .NET 9.0 SDK or later
- Azure Service Bus connection string with appropriate permissions

## Building the Application

```bash
dotnet build
```

## Running the Application

### Option 1: Provide connection string as command-line argument

```bash
dotnet run --project SBInspector/SBInspector.csproj -- "your-connection-string"
```

### Option 2: Enter connection string interactively

```bash
dotnet run --project SBInspector/SBInspector.csproj
```

The application will prompt you to enter the connection string.

## Usage

Once the application starts, you'll see a menu with the following options:

1. **List Queues** - Displays all queues with their message counts
2. **List Topics** - Displays all topics with subscription and message counts
3. **Inspect Queue Messages** - Select a queue and view its messages
4. **Inspect Topic Subscription Messages** - Select a topic and subscription to view messages
5. **Exit** - Close the application

### Viewing Messages

When inspecting messages, you can choose to view:
- **Active Messages**: Messages currently in the queue/subscription
- **Scheduled Messages**: Messages scheduled for future delivery
- **Dead Letter Messages**: Messages that failed processing and were moved to the dead-letter queue

Messages are displayed in "peek" mode, meaning they are not removed from the queue.

## Example Output

```
=== Azure Service Bus Inspector ===

=== Main Menu ===
1. List Queues
2. List Topics
3. Inspect Queue Messages
4. Inspect Topic Subscription Messages
5. Exit
Select an option: 1

=== Queues ===
  - my-queue
    Active Messages: 5
    Scheduled Messages: 2
    Dead Letter Messages: 1
```

## Connection String Format

Your Azure Service Bus connection string should be in the following format:

```
Endpoint=sb://your-namespace.servicebus.windows.net/;SharedAccessKeyName=your-key-name;SharedAccessKey=your-key-value
```

## Security Note

Never commit connection strings to source control. Use environment variables or secure configuration management systems to store sensitive credentials.

## License

This project is provided as-is for demonstration and utility purposes.