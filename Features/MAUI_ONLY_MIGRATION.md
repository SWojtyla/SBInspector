# MAUI-Only Migration

## Feature Overview

SBInspector now runs as a MAUI-only Windows desktop application. The shared Razor class library and the Blazor Server app have been removed, and all shared logic and UI components live directly in the MAUI project.

## How to Use

1. Open `SBInspector.sln`.
2. Set `SEBInspector.Maui` as the startup project.
3. Build and run the Windows target:
   ```bash
   dotnet build -f net9.0-windows10.0.19041.0
   dotnet run -f net9.0-windows10.0.19041.0
   ```
4. Use the app to connect to Azure Service Bus and manage entities/messages.

## Examples

### Build the MAUI app

```bash
cd SEBInspector.Maui
dotnet build -f net9.0-windows10.0.19041.0
```

### Run tests

```bash
cd SBInspector
dotnet test SBInspector.Tests
```

## Technical Implementation Details

- All code is hosted in `SEBInspector.Maui` under clean architecture folders: `Core`, `Application`, `Infrastructure`, and `Presentation`.
- Namespaces were updated to the MAUI root: `SEBInspector.Maui.*`.
- The test project now references `SEBInspector.Maui` directly.
- The solution contains only `SEBInspector.Maui` and `SBInspector.Tests`.
