# Tauri + Blazor Server Setup

## Overview

This document explains how the SBInspector application is configured to work with Tauri as a desktop application while using Blazor Server mode.

## The Challenge

Blazor Server applications are not static websites - they require a running .NET server to function. However, Tauri's `frontendDist` configuration expects static HTML/CSS/JS files. This creates a mismatch that needed to be resolved.

## The Solution: Sidecar Pattern

We've implemented the "sidecar pattern" where:

1. The .NET server runs alongside the Tauri application
2. The Tauri window loads content from the local server via `http://localhost:5000`
3. The server is automatically started when the app launches and stopped when it closes

## Implementation Details

### 1. Index.html

A minimal `index.html` file is placed in `wwwroot/`:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta http-equiv="refresh" content="0; url=http://localhost:5000" />
    <title>Redirecting...</title>
</head>
<body>
    <p>Redirecting to SB Inspector...</p>
</body>
</html>
```

This file:
- Satisfies Tauri's requirement for an index.html in `frontendDist`
- Redirects to the local server running on port 5000

### 2. Tauri Configuration (`src-tauri/tauri.conf.json`)

```json
{
  "build": {
    "frontendDist": "../dist/wwwroot",
    "beforeBuildCommand": "dotnet publish -c release SBInspector/SBInspector.csproj -o dist"
  },
  "bundle": {
    "resources": [
      "../dist/*"
    ]
  }
}
```

Key points:
- `frontendDist` points to the published wwwroot folder
- `beforeBuildCommand` publishes the .NET application
- `resources` bundles all server files with the installer

### 3. Tauri Rust Code (`src-tauri/src/lib.rs`)

The Rust code:
- Starts the .NET server process in production builds
- Sets the server to listen on `http://localhost:5000`
- Redirects the webview to the server URL
- Cleans up the server process when the app closes

```rust
// In production builds only
#[cfg(not(debug_assertions))]
{
    let resource_path = app.path().resource_dir()?;
    let server_path = resource_path.join("SBInspector");
    
    let server_process = Command::new(&server_path)
        .current_dir(&resource_path)
        .env("ASPNETCORE_URLS", "http://localhost:5000")
        .spawn()?;
    
    // ... manage the process lifecycle
}
```

## Development vs. Production

### Development Mode
- Uses `devUrl: "https://localhost:5000"`
- You run `dotnet watch run` separately
- Tauri connects to the already-running server
- Hot reload works as expected

### Production Mode (MSI Installer)
- Tauri automatically starts the .NET server
- Server runs as a child process
- Webview connects to `http://localhost:5000`
- Server shuts down when the app closes

## Why This Approach?

We chose this approach because:

1. **Minimal Changes**: No need to convert to Blazor WebAssembly
2. **Full Functionality**: Azure SDK and server-side features work perfectly
3. **Simple**: The sidecar pattern is straightforward and well-documented
4. **Performance**: Server-side Blazor can be faster for some operations

## Alternative Approaches (Not Used)

### Convert to Blazor WebAssembly
- ❌ Major code refactoring required
- ❌ Azure SDK might not work in WASM
- ❌ Loss of server-side capabilities
- ✅ Would create a true static app

### External Server
- ❌ User must manually start the server
- ❌ Poor user experience
- ✅ Simpler Tauri configuration

## Building the MSI Installer

To build the installer:

```bash
# From the repository root
npm install  # Install Tauri CLI
npm run tauri build
```

The build process:
1. Runs `dotnet publish` to create the server executable
2. Copies all server files to the bundle
3. Creates the MSI installer with everything included

## Troubleshooting

### "Index.html not found" error
- ✅ Fixed by adding `index.html` to `wwwroot/`

### Server doesn't start
- Check that `dist/SBInspector` executable has execute permissions
- Verify port 5000 is not already in use
- Check application logs

### Blank window in production
- Verify the redirect in index.html is correct
- Check that the server process started successfully
- Inspect console logs in the Tauri window

## Summary

This setup allows SBInspector to run as a desktop application while maintaining its Blazor Server architecture. The sidecar pattern provides the best balance of simplicity, functionality, and user experience.
