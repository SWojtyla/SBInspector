# SBInspector

Azure Service Bus Inspector - A .NET MAUI Blazor Hybrid cross-platform desktop application for inspecting Azure Service Bus queues, topics, and messages.

## Features

- Connect to Azure Service Bus using a connection string
## Storage Configuration

SBInspector uses file-based storage on the local device by default. Connection strings and message templates are stored in the application's data directory:
- **Desktop folder**: Data is saved to `Desktop/SBInspector/` for easy access
- Connections are stored in `connections.json`
- Templates are stored in `templates.json`

For detailed information, see [STORAGE_CONFIGURATION.md](Features/STORAGE_CONFIGURATION.md).
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
- For Android: Android SDK
- For Windows: Windows 10 version 1809 or higher
- For iOS/macOS: Xcode (macOS only)

## Platform Support

SBInspector runs on:
- **Windows** (Windows 10 1809+)
- **Android** (API 24+)
- **iOS** (iOS 14.2+)
- **macOS** (macOS 11+)

## Getting Started

### Installing .NET MAUI Workloads

First, ensure you have the required .NET MAUI workloads installed:

```bash
# Install MAUI workloads for your platform
dotnet workload install maui-android    # For Android
dotnet workload install maui-windows    # For Windows (Windows only)
```

### Building and Running

1. Clone the repository
2. Navigate to the SBInspector directory
3. Build and run the application for your platform:

   **For Android:**
   ```bash
   cd SBInspector
   dotnet build -f net9.0-android
   dotnet run -f net9.0-android
   ```

   **For Windows:**
   ```bash
   cd SBInspector
   dotnet build -f net9.0-windows10.0.19041.0
   dotnet run -f net9.0-windows10.0.19041.0
   ```

   **For iOS (macOS only):**
   ```bash
   cd SBInspector
   dotnet build -f net9.0-ios
   dotnet run -f net9.0-ios
   ```

   **For macOS (macOS only):**
   ```bash
   cd SBInspector
   dotnet build -f net9.0-maccatalyst
   dotnet run -f net9.0-maccatalyst
   ```

4. Enter your Azure Service Bus connection string and click "Connect"
5. Browse queues, topics, and inspect messages
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

- **Azure.Messaging.ServiceBus** - Azure Service Bus SDK for .NET
- **Microsoft.Maui.Controls** - .NET MAUI UI framework
- **Microsoft.AspNetCore.Components.WebView.Maui** - Blazor integration for MAUI

## Message Filtering

The application supports filtering messages by their application properties (attributes). See [FILTERING.md](FILTERING.md) for detailed documentation on how to use this feature.

## Migration from Tauri

Previous versions of SBInspector used Tauri for desktop deployment. The application has been refactored to use .NET MAUI, which provides better integration with .NET and cross-platform capabilities. The Tauri configuration (`src-tauri/`, `package.json`) is no longer used but kept for reference.

For the MAUI version, use the build instructions above. The old Tauri-based version can be found in earlier commits.

## License

MIT