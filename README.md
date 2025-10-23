# SBInspector

Azure Service Bus Inspector - A Blazor web application for inspecting Azure Service Bus queues, topics, and messages.

## Features

- Connect to Azure Service Bus using a connection string
- **Storage Configuration**:
  - Choose between Browser Local Storage or File System storage
  - Save connection strings and message templates persistently
  - File System storage saves to Desktop folder (recommended for Tauri desktop app)
  - Browser storage for web deployments
  - See [STORAGE_CONFIGURATION.md](Features/STORAGE_CONFIGURATION.md) for detailed documentation
- **Refresh Functionality**:
  - Refresh button to reload all queues, topics, and subscriptions with updated counts
  - Refresh button on messages panel to reload current messages
  - Auto-reload entities after browser page refresh (fixes disappearing entities bug)
  - See [REFRESH_FUNCTIONALITY.md](REFRESH_FUNCTIONALITY.md) for detailed documentation
- **Tree View Navigation**:
  - Hierarchical tree view showing all queues, topics, and subscriptions
  - Split-panel layout with tree on left and details on right
  - Expandable topics to view subscriptions
  - Visual selection highlighting and message count badges
  - See [TREE_VIEW_NAVIGATION.md](TREE_VIEW_NAVIGATION.md) for detailed documentation
- View all queues and topics in the namespace
- **Enable/Disable Entities**:
  - View the status (Active/Disabled) of queues, topics, and subscriptions
  - Toggle entities between Active and Disabled states
  - Visual status indicators with color-coded badges
  - See [ENABLE_DISABLE.md](ENABLE_DISABLE.md) for detailed documentation
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
- **CRUD Operations on Messages**:
  - **Delete** messages from active queues, dead-letter queues, or subscriptions
  - **Requeue** messages from dead-letter queue back to active queue
  - **Send** new messages to queues or topics (with optional scheduling)
  - **Reschedule** scheduled messages to a different delivery time
  - **Purge All** messages from a queue or topic in one operation
  - Dynamic list updates when messages are deleted or purged
  - Automatic refresh of message counts after operations
  - **Visual Operation Feedback**: Full-screen loading overlay during delete/purge operations
  - See [MESSAGE_CRUD.md](MESSAGE_CRUD.md) for detailed documentation
- Filter messages by application properties (attributes):
  - Add multiple filters with AND logic
  - Filter by attribute name and/or value
  - Support for literal (case-insensitive) or regex pattern matching
  - Real-time filtering with message count display
- Sortable tables:
  - Click on column headers to sort queues by name or message counts
  - Click on column headers to sort messages by ID, subject, enqueued time, or delivery count
  - Visual indicators show current sort column and direction
- Message pagination:
  - Customizable page size (50, 100, 200, or 500 messages)
  - Load more messages on demand with "Load More" button
  - Efficient browsing of large message collections
  - See [PAGINATION.md](PAGINATION.md) for details

## Prerequisites

- .NET 9.0 SDK or later
- Azure Service Bus namespace
- For Tauri desktop app:
  - Node.js and npm
  - Rust toolchain
  - Tauri CLI: `npm install`

## Getting Started

### Running as Web Application

1. Clone the repository
2. Navigate to the SBInspector directory
3. Run the application:
   ```bash
   cd SBInspector
   dotnet run
   ```
4. Open your browser and navigate to the URL shown in the console (typically https://localhost:5000)
5. Enter your Azure Service Bus connection string and click "Connect"
6. Browse queues, topics, and inspect messages

### Running as Desktop Application (Tauri)

1. Clone the repository
2. Install dependencies:
   ```bash
   npm install
   ```
3. Run in development mode:
   ```bash
   npm run tauri dev
   # or
   npx tauri dev
   ```
4. Build installer:
   ```bash
   npm run tauri build
   # or
   npx tauri build
   ```
   The installer will be created in `src-tauri/target/release/bundle/`

For detailed information about the Tauri configuration, see [TAURI_CONFIGURATION.md](Features/TAURI_CONFIGURATION.md).

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

The project follows Clean Architecture principles:

- `Core/Domain/` - Domain models (EntityInfo, MessageInfo, MessageFilter)
- `Core/Interfaces/` - Service interfaces
- `Application/Services/` - Application services (MessageFilterService)
- `Infrastructure/ServiceBus/` - Service Bus implementation
- `Presentation/Components/Pages/` - Blazor UI pages
- `Presentation/Components/Layout/` - Layout components
- `Presentation/Components/UI/` - Reusable UI components

For detailed information about the UI component structure, see [UI_COMPONENTS.md](UI_COMPONENTS.md).

## Dependencies

- Azure.Messaging.ServiceBus - Azure Service Bus SDK for .NET

## Message Filtering

The application supports filtering messages by their application properties (attributes). See [FILTERING.md](FILTERING.md) for detailed documentation on how to use this feature.

## License

MIT