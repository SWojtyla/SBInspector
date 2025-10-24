# MAUI Implementation

This document describes the implementation of the .NET MAUI version of SBInspector.

## Overview

The SBInspector application is now available in two flavors:
1. **Blazor Server** - Web-based application (SBInspector project)
2. **.NET MAUI** - Cross-platform desktop and mobile application (SEBInspector.Maui project)

Both applications share the same UI components and business logic through a shared Razor Class Library.

## Architecture

### Project Structure

```
SBInspector/
├── SBInspector/                    # Blazor Server Web App
│   ├── Program.cs                  # Web app startup and DI configuration
│   ├── Properties/
│   ├── wwwroot/                    # Web-specific static assets
│   └── SBInspector.csproj
│
├── SEBInspector.Maui/             # .NET MAUI App
│   ├── App.xaml                    # MAUI app definition
│   ├── MainPage.xaml               # Main MAUI page with BlazorWebView
│   ├── MauiProgram.cs              # MAUI app startup and DI configuration
│   ├── Platforms/                  # Platform-specific code
│   ├── Resources/                  # MAUI resources (icons, fonts, etc.)
│   ├── wwwroot/
│   │   └── index.html              # Blazor WebView HTML host
│   └── SEBInspector.Maui.csproj
│
└── SBInspector.Shared/            # Shared Razor Class Library
    ├── Core/
    │   ├── Domain/                 # Domain models
    │   └── Interfaces/             # Service interfaces
    ├── Application/
    │   └── Services/               # Application services
    ├── Infrastructure/
    │   ├── ServiceBus/             # Azure Service Bus implementation
    │   └── Storage/                # Storage implementations
    ├── Presentation/
    │   └── Components/
    │       ├── Pages/              # Page components
    │       ├── Layout/             # Layout components
    │       ├── UI/                 # Reusable UI components
    │       ├── App.razor           # Root component for Blazor Server
    │       └── Routes.razor        # Routing configuration
    ├── wwwroot/                    # Shared static assets
    │   ├── app.css
    │   └── lib/                    # Bootstrap and other libraries
    └── SBInspector.Shared.csproj
```

### Key Design Decisions

1. **Shared Razor Class Library (RCL)**
   - All UI components, business logic, and domain models are in the shared library
   - Both Blazor Server and MAUI projects reference this shared library
   - Ensures consistency and reduces code duplication

2. **Clean Architecture**
   - The shared library follows clean architecture principles:
     - **Domain**: Business entities and value objects
     - **Application**: Business logic and services
     - **Infrastructure**: External service implementations (Azure Service Bus, Storage)
     - **Presentation**: UI components and pages

3. **Dependency Injection**
   - Both applications register the same services in their respective startup files
   - Services are configured in `Program.cs` (Blazor Server) and `MauiProgram.cs` (MAUI)

## Building the Projects

### Prerequisites

- .NET 9.0 SDK
- For MAUI development:
  - Visual Studio 2022 (Windows/Mac) with MAUI workload, OR
  - Visual Studio Code with .NET MAUI extension
  - MAUI workload: `dotnet workload install maui`

### Blazor Server

```bash
cd SBInspector
dotnet run
```

The Blazor Server app will be available at `https://localhost:5001` (or as indicated in the console).

### MAUI App

#### Windows
```bash
cd SEBInspector.Maui
dotnet build -f net9.0-windows10.0.19041.0
```

#### Android
```bash
cd SEBInspector.Maui
dotnet build -f net9.0-android
```

#### iOS
```bash
cd SEBInspector.Maui
dotnet build -f net9.0-ios
```

#### macOS
```bash
cd SEBInspector.Maui
dotnet build -f net9.0-maccatalyst
```

## Running the MAUI App

### Visual Studio
1. Open `SBInspector.sln`
2. Set `SEBInspector.Maui` as the startup project
3. Select your target platform (Windows, Android, iOS, etc.)
4. Press F5 to run

### Command Line

#### Windows
```bash
cd SEBInspector.Maui
dotnet run -f net9.0-windows10.0.19041.0
```

## Features

Both the Blazor Server and MAUI applications support the same features:

- Connect to Azure Service Bus using connection strings
- Browse queues, topics, and subscriptions
- View and manage messages (active, scheduled, dead-letter)
- Send new messages
- Delete, requeue, and reschedule messages
- Purge messages
- Filter messages by properties
- Sort and paginate message lists
- Enable/disable queues, topics, and subscriptions
- Persistent storage for connection strings and message templates

## Storage Differences

### Blazor Server
- Can use **Browser Local Storage** for web deployments
- Can use **File System Storage** when running locally or with Tauri

### MAUI
- Uses **File System Storage** by default
- Stores data in the application's local app data folder
- Location varies by platform:
  - **Windows**: `%LOCALAPPDATA%\SBInspector`
  - **macOS**: `~/Library/Application Support/SBInspector`
  - **Android**: `/data/data/com.companyname.sebinspector.maui/files`
  - **iOS**: Application's Documents directory

## Troubleshooting

### MAUI Workload Issues

If you encounter errors about missing MAUI workloads:

```bash
dotnet workload install maui
```

### Platform-Specific Issues

#### Windows
- Ensure Windows 10 SDK 10.0.19041.0 or later is installed
- May require enabling Developer Mode in Windows Settings

#### Android
- Requires Android SDK and emulator or physical device
- Update Android SDK to latest version if build fails

#### iOS/macOS
- Requires Xcode on macOS
- May require provisioning profiles for physical devices

## Development Notes

### Adding New Features

1. Add domain models to `SBInspector.Shared/Core/Domain/`
2. Add service interfaces to `SBInspector.Shared/Core/Interfaces/`
3. Implement services in `SBInspector.Shared/Application/Services/` or `SBInspector.Shared/Infrastructure/`
4. Create UI components in `SBInspector.Shared/Presentation/Components/`
5. Register services in both `Program.cs` (Blazor Server) and `MauiProgram.cs` (MAUI)

### Updating Shared Components

Any changes to components in `SBInspector.Shared` automatically affect both applications.

### Platform-Specific Code

If platform-specific code is needed:
- For MAUI: Add to `SEBInspector.Maui/Platforms/{Platform}/`
- For Blazor Server: Add directly to the `SBInspector` project

## Future Enhancements

Potential improvements for the MAUI app:
- Native notifications for message arrivals
- Background message monitoring
- Platform-specific UI optimizations
- Support for mobile-specific gestures
- Offline mode with local caching
- Biometric authentication for saved connections

## Contributing

When contributing:
1. Keep shared code in `SBInspector.Shared`
2. Only add platform-specific code when absolutely necessary
3. Test changes in both Blazor Server and MAUI
4. Update this documentation for significant changes

## License

MIT
