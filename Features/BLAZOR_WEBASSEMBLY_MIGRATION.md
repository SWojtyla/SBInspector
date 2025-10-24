# Blazor WebAssembly Migration

## Overview

The application has been successfully migrated from **Blazor Server** to **Blazor WebAssembly**. This change enables better deployment options, including running as a standalone desktop application with Tauri, and improved client-side performance.

## What Changed

### Architecture
- **Before**: Blazor Server (runs on the server with SignalR for UI updates)
- **After**: Blazor WebAssembly (runs entirely in the browser, downloaded as .NET assemblies compiled to WebAssembly)

### Key Benefits
1. **Tauri-Ready**: The WebAssembly architecture works seamlessly with Tauri for desktop app packaging
2. **Offline Capable**: Once loaded, the app can run without a server connection (except for Azure Service Bus operations)
3. **Better Scalability**: No server-side state or SignalR connections to manage
4. **Easier Deployment**: Can be deployed to any static file hosting (CDN, GitHub Pages, etc.)

## Technical Changes

### Project Structure
- Changed SDK from `Microsoft.NET.Sdk.Web` to `Microsoft.NET.Sdk.BlazorWebAssembly`
- Added WebAssembly-specific NuGet packages:
  - `Microsoft.AspNetCore.Components.WebAssembly`
  - `Microsoft.AspNetCore.Components.WebAssembly.DevServer`

### Program.cs
Updated to use WebAssembly hosting model:
```csharp
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
// ... service registrations ...
await builder.Build().RunAsync();
```

### Entry Point
- Created `wwwroot/index.html` as the application entry point
- Updated `App.razor` to be a router component instead of an HTML document
- Changed script reference from `blazor.web.js` to `blazor.webassembly.js`

### Render Modes
Updated all interactive components to use `InteractiveWebAssembly` render mode:
- `Home.razor`: Changed from `InteractiveServerRenderMode` to `InteractiveWebAssembly`
- `Templates.razor`: Changed from `InteractiveServerRenderMode` to `InteractiveWebAssembly`

### Storage Compatibility
Enhanced storage services to work in both browser and Tauri environments:

#### StorageConfigurationService
- Detects file system availability at runtime
- Falls back to LocalStorage when file system is not available (pure browser deployment)
- Supports FileSystem storage when running in Tauri (desktop app)

#### FileStorageService
- Gracefully handles browser WASM environment where file system is unavailable
- Silently fails file operations in browser, returning empty collections
- Works fully when running in Tauri with file system access

#### LocalStorageService
- Works in all environments (browser and Tauri)
- Uses browser's localStorage API via JavaScript interop
- Default storage option for web deployments

## How to Use

### Running in Development
```bash
cd SBInspector
dotnet run
```

The app will start on `https://localhost:5000` or `http://localhost:5000`

### Building for Production
```bash
dotnet publish -c Release -o dist
```

The output will be in `dist/wwwroot/` - this directory contains all the static files needed to deploy the app.

### Running with Tauri
```bash
# Development mode
npx tauri dev

# Build desktop app
npx tauri build
```

## Storage Options

### Browser Local Storage (Default)
- Works in all environments
- Data persists in browser localStorage
- Best for web deployments
- No file system access needed

### File System Storage
- **Browser Only**: Not available - automatically falls back to LocalStorage
- **Tauri Desktop App**: Fully functional - saves to Desktop/SBInspector folder
- Recommended for Tauri deployments where users want direct file access

## Deployment Options

### Static Web Hosting
Deploy the contents of `dist/wwwroot/` to:
- Azure Static Web Apps
- GitHub Pages
- Netlify
- Any CDN or static file host

### Tauri Desktop Application
Use `npx tauri build` to create:
- Windows: `.msi` installer
- macOS: `.dmg` installer  
- Linux: `.AppImage` or `.deb`

## Compatibility

- **Browsers**: All modern browsers supporting WebAssembly
  - Chrome/Edge 88+
  - Firefox 78+
  - Safari 14.1+
- **Desktop**: Windows, macOS, Linux (via Tauri)
- **.NET Version**: .NET 9.0

## Migration Impact

### What Stayed the Same
- All features work identically
- Clean architecture structure unchanged
- Service Bus integration unchanged
- UI/UX identical to before
- All existing features (CRUD operations, filtering, sorting, pagination, etc.)

### What's Different
- App downloads once and runs in browser (faster subsequent loads)
- No server-side state management
- Storage automatically adapts to environment
- Smaller server resource footprint (if hosting the static files)

## Future Considerations

The WebAssembly architecture opens up new possibilities:
1. **Progressive Web App (PWA)**: Can be enhanced to work offline completely
2. **Better Caching**: Static files can be cached aggressively
3. **Cross-Platform Desktop**: Single codebase for all platforms via Tauri
4. **Reduced Hosting Costs**: Static files are cheaper to host than server applications
