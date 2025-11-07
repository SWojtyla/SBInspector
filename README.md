# SBInspector

Azure Service Bus Inspector - Available as both a **Blazor web application** and a **.NET MAUI cross-platform desktop/mobile app** for inspecting Azure Service Bus queues, topics, and messages.

## Available Applications

1. **Blazor Server Web App** (`SBInspector`) - Run in a web browser
2. **.NET MAUI App** (`SEBInspector.Maui`) - Cross-platform desktop and mobile application

Both applications share the same codebase and features through a shared Razor Class Library. See [MAUI_IMPLEMENTATION.md](Features/MAUI_IMPLEMENTATION.md) for details on the MAUI version.

## Features

- Connect to Azure Service Bus using a connection string
- **Storage Configuration**:
  - Platform-specific automatic storage selection
  - Blazor Server: Uses Browser Local Storage automatically
  - MAUI Desktop: Uses File System storage automatically
  - Saves connection strings and message templates persistently
  - No manual configuration required
  - See [PLATFORM_DEFAULT_STORAGE.md](Features/PLATFORM_DEFAULT_STORAGE.md) for detailed documentation
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
- **Advanced Message Filtering**:
  - Filter by application properties, enqueued time, delivery count, or sequence number
  - Multiple operators: Contains, Not Contains, Equals, Not Equals, Greater Than, Less Than, Regex, etc.
  - Add multiple filters with AND logic
  - **Enable/Disable Individual Filters**: Toggle filters on/off without removing them
  - Real-time filtering with message count display
  - **Delete Filtered**: Delete only messages that match your filters
  - **Export Filtered**: Download filtered messages as JSON for backup or analysis
  - See [FILTERING.md](FILTERING.md), [FILTER_BASED_OPERATIONS.md](FILTER_BASED_OPERATIONS.md), and [FILTER_ENABLE_DISABLE.md](Features/FILTER_ENABLE_DISABLE.md) for detailed documentation
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
- For MAUI development: MAUI workload (`dotnet workload install maui`)

## Getting Started

### Blazor Server Web App

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

### .NET MAUI App

1. Clone the repository
2. Install MAUI workload (if not already installed):
   ```bash
   dotnet workload install maui
   ```
3. Navigate to the MAUI project:
   ```bash
   cd SEBInspector.Maui
   ```
4. Run for your target platform:
   ```bash
   # Windows
   dotnet run -f net9.0-windows10.0.19041.0
   
   # Android (requires emulator or device)
   dotnet build -f net9.0-android
   
   # iOS (requires Mac)
   dotnet build -f net9.0-ios
   
   # macOS
   dotnet build -f net9.0-maccatalyst
   ```

Alternatively, open `SBInspector.sln` in Visual Studio 2022 and run the `SEBInspector.Maui` project.

For more details on the MAUI implementation, see [MAUI_IMPLEMENTATION.md](Features/MAUI_IMPLEMENTATION.md).

### Tauri Desktop App

To run the app with Tauri:
```bash
npx tauri dev
```
If needed: `npm install -g @tauri-apps/cli`
5. Enter your Azure Service Bus connection string and click "Connect"
6. Browse queues, topics, and inspect messages
7. To run the Tauri desktop app: `npx tauri dev` (if needed: `npm install -g @tauri-apps/cli`)

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

The solution consists of three projects following Clean Architecture principles:

### SBInspector (Blazor Server Web App)
- `Program.cs` - Application startup and DI configuration
- `wwwroot/` - Web-specific static assets

### SEBInspector.Maui (.NET MAUI App)
- `App.xaml` / `App.xaml.cs` - MAUI application definition
- `MainPage.xaml` - Main page with BlazorWebView
- `MauiProgram.cs` - MAUI startup and DI configuration
- `Platforms/` - Platform-specific code
- `Resources/` - App icons, fonts, and resources
- `wwwroot/index.html` - Blazor WebView HTML host

### SBInspector.Shared (Razor Class Library)
Shared code used by both Blazor Server and MAUI:

- `Core/Domain/` - Domain models (EntityInfo, MessageInfo, MessageFilter)
- `Core/Interfaces/` - Service interfaces (IServiceBusService, IStorageService)
- `Application/Services/` - Application services (MessageFilterService, StorageConfigurationService)
- `Infrastructure/ServiceBus/` - Azure Service Bus implementation
- `Infrastructure/Storage/` - Storage implementations (LocalStorage, FileSystem)
- `Presentation/Components/Pages/` - Blazor UI pages
- `Presentation/Components/Layout/` - Layout components
- `Presentation/Components/UI/` - Reusable UI components
- `wwwroot/` - Shared static assets (CSS, JavaScript, Bootstrap)

For detailed information about the UI component structure, see [UI_COMPONENTS.md](UI_COMPONENTS.md).
For details on the MAUI implementation, see [MAUI_IMPLEMENTATION.md](Features/MAUI_IMPLEMENTATION.md).

## Dependencies

- **Azure.Messaging.ServiceBus** - Azure Service Bus SDK for .NET
- **Microsoft.AspNetCore.Components.Web** - Blazor components
- **Microsoft.AspNetCore.Components.WebView.Maui** - Blazor WebView for MAUI (MAUI project only)
- **Microsoft.Maui.Controls** - MAUI UI framework (MAUI project only)

## Message Filtering

The application supports advanced filtering of messages by application properties, enqueued time, delivery count, and sequence number. You can also perform bulk operations on filtered messages including delete and export. See [FILTERING.md](FILTERING.md) and [FILTER_BASED_OPERATIONS.md](Features/FILTER_BASED_OPERATIONS.md) for detailed documentation.

## Privacy

SBInspector respects your privacy. All data (connection strings and templates) is stored locally on your device in encrypted form. No data is collected, tracked, or transmitted to third parties. For complete details, see [PRIVACY_POLICY.md](PRIVACY_POLICY.md).

## License

MIT