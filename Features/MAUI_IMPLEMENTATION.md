# MAUI Implementation

This document describes the MAUI-only implementation of SBInspector.

## Overview

SBInspector now ships as a Windows MAUI desktop application using a BlazorWebView host. All domain logic, services, and UI components live inside the MAUI project.

## Architecture

### Project Structure

```
SBInspector/
├── SEBInspector.Maui/              # .NET MAUI App
│   ├── App.xaml                    # MAUI app definition
│   ├── MainPage.xaml               # Main MAUI page with BlazorWebView
│   ├── MauiProgram.cs              # MAUI app startup and DI configuration
│   ├── Core/                       # Domain models and interfaces
│   ├── Application/                # Application services
│   ├── Infrastructure/             # Service Bus + storage implementations
│   ├── Presentation/Components/    # Blazor UI components
│   ├── Platforms/                  # Platform-specific code
│   ├── Resources/                  # MAUI resources (icons, fonts, etc.)
│   ├── wwwroot/                    # Blazor WebView assets
│   └── SEBInspector.Maui.csproj
│
└── SBInspector.Tests/              # bUnit + service tests
```

### Clean Architecture in MAUI

The MAUI project follows clean architecture principles:

- **Core**: Domain entities and interfaces
- **Application**: Application services and business logic
- **Infrastructure**: Service Bus and storage implementations
- **Presentation**: Blazor components and pages

### Dependency Injection

Services are registered in `MauiProgram.cs`. Keep business logic inside services and keep components thin.

## Building the Project

### Prerequisites

- .NET 9.0 SDK
- MAUI workload: `dotnet workload install maui`
- Windows 10 SDK 10.0.19041.0 or later

### Build (Windows)

```bash
cd SEBInspector.Maui
dotnet build -f net9.0-windows10.0.19041.0
```

### Run (Windows)

```bash
cd SEBInspector.Maui
dotnet run -f net9.0-windows10.0.19041.0
```

## Development Notes

### Adding New Features

1. Add domain models to `SEBInspector.Maui/Core/Domain/`
2. Add service interfaces to `SEBInspector.Maui/Core/Interfaces/`
3. Implement services in `SEBInspector.Maui/Application/Services/` or `SEBInspector.Maui/Infrastructure/`
4. Create UI components in `SEBInspector.Maui/Presentation/Components/`
5. Register services in `MauiProgram.cs`

### Platform-Specific Code

Add platform-specific code under `SEBInspector.Maui/Platforms/{Platform}/`.

## Troubleshooting

### MAUI Workload Issues

```bash
dotnet workload install maui
```

### Windows SDK Issues

Ensure Windows 10 SDK 10.0.19041.0 or later is installed and Developer Mode is enabled.

## License

MIT