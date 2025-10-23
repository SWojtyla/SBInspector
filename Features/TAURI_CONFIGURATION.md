# Tauri Configuration for Blazor Server App

## Overview

This document describes how the Tauri desktop application wraps the Blazor Server application to create a native desktop experience.

## Architecture

The SBInspector desktop application uses a hybrid architecture:

1. **Blazor Server Backend**: The ASP.NET Core Blazor Server application runs as a background process, handling all UI logic and server-side operations.
2. **Tauri Frontend**: Tauri creates a native desktop window that connects to the Blazor server via localhost.

## How It Works

### Development Mode

When running in development mode (`npm run tauri dev` or `npx tauri dev`):

1. The `beforeDevCommand` in `tauri.conf.json` starts the Blazor server using `dotnet watch run`
2. The Blazor server listens on `https://localhost:5000`
3. Tauri opens a window pointing to `https://localhost:5000`
4. Hot reload is supported for both Blazor and Tauri changes

### Production Mode (Installed App)

When the app is built and installed:

1. The `beforeBuildCommand` publishes the Blazor app as a self-contained executable to the `dist/` folder
2. All published files are included as resources in the Tauri bundle
3. When the app starts:
   - Tauri's Rust code (in `src-tauri/src/lib.rs`) launches the Blazor server executable as a background process
   - The server starts listening on `https://localhost:5000`
   - After a 3-second delay to allow the server to start, Tauri opens the main window
   - The window displays the Blazor UI from `https://localhost:5000`

## Configuration Files

### tauri.conf.json

Key configuration settings:

- **devUrl**: `https://localhost:5000` - URL for development
- **beforeDevCommand**: Starts the Blazor server in watch mode
- **beforeBuildCommand**: Publishes the Blazor app as self-contained for Windows (x64)
- **app.windows[0].url**: Points to the Blazor server URL
- **bundle.resources**: Includes all published Blazor files in the installer

### src-tauri/src/lib.rs

The Rust code handles:

- Starting the Blazor server executable in production mode
- Setting environment variables (ASPNETCORE_ENVIRONMENT, ASPNETCORE_URLS)
- Suppressing console output (stdout/stderr redirected to null)
- Waiting for the server to start before showing the window

## Building the Application

### Prerequisites

- .NET 9.0 SDK
- Node.js and npm
- Rust (for Tauri)
- Tauri CLI: `npm install -g @tauri-apps/cli`

### Development

```bash
# Start in development mode with hot reload
npm run tauri dev
# or
npx tauri dev
```

### Production Build

```bash
# Build the installer
npm run tauri build
# or
npx tauri build
```

The installer will be created in `src-tauri/target/release/bundle/msi/` (for Windows).

## Advantages of This Approach

1. **Full Blazor Server Capabilities**: All interactive server components work as expected
2. **Native Desktop Experience**: The app looks and feels like a native desktop application
3. **No Browser UI**: No address bar, bookmarks, or other browser chrome
4. **Single Executable**: The installer includes everything needed (including .NET runtime)
5. **Cross-Platform**: Can be built for Windows, macOS, and Linux
6. **Small Bundle Size**: While self-contained, the app is still relatively small

## Troubleshooting

### Server Not Starting

If the window shows a connection error:

1. Check that port 5000 is not in use by another application
2. Verify the Blazor server executable exists in the installation directory
3. Check the Tauri logs for any startup errors

### HTTPS Certificate Issues

The app uses HTTPS locally. If you encounter certificate issues:

1. Ensure the ASP.NET Core development certificate is installed: `dotnet dev-certs https --trust`
2. For production builds, consider using HTTP or implement proper certificate handling

## Future Improvements

Potential enhancements:

1. Use a random port instead of hardcoding 5000
2. Add health check before opening the window
3. Gracefully shutdown the server when the app closes
4. Add error handling and user feedback for startup failures
