# Tauri Configuration Update - Summary

## Problem

The Tauri configuration was not properly set up to work with the Blazor Server application. When installing the app, the application would not work because:

1. Tauri was configured to serve static files from `frontendDist`
2. The Blazor app is a server-side application that requires a running .NET process
3. There was no mechanism to launch and manage the Blazor server process

## Solution

Updated the Tauri configuration to properly launch and manage the Blazor Server application as a background process:

### Changes Made

1. **Updated `tauri.conf.json`**:
   - Removed `frontendDist` configuration
   - Added `url` to window configuration pointing to `https://localhost:5000`
   - Updated `beforeBuildCommand` to publish as self-contained Windows executable
   - Added `resources` to bundle all published .NET files with the installer

2. **Enhanced `src-tauri/src/lib.rs`**:
   - Added code to spawn the Blazor server process in production mode
   - Set appropriate environment variables (ASPNETCORE_ENVIRONMENT, ASPNETCORE_URLS)
   - Implemented proper process lifecycle management
   - Server process is killed when the Tauri window closes
   - Used conditional compilation to only run in release builds

3. **Updated `.gitignore`**:
   - Added exclusions for build artifacts (`dist/`, `src-tauri/target/`, `src-tauri/binaries/`)

4. **Documentation**:
   - Created `Features/TAURI_CONFIGURATION.md` with detailed architecture explanation
   - Updated `README.md` with Tauri usage instructions

## Architecture

### Development Mode
- Tauri runs `dotnet watch run` to start the Blazor server
- Tauri window connects to `https://localhost:5000`
- Hot reload works for both Blazor and Tauri

### Production Mode (Installed App)
- Blazor app is published as a self-contained executable (includes .NET runtime)
- All files are bundled in the installer as resources
- On startup, Tauri:
  1. Spawns the Blazor server executable
  2. Waits 3 seconds for server to start
  3. Opens window pointing to `https://localhost:5000`
- On shutdown, Tauri kills the server process

## Benefits

1. **No Static File Serving**: The Blazor Server app works as intended with full interactive capabilities
2. **Self-Contained**: No need for .NET runtime to be pre-installed
3. **Clean Lifecycle**: Server starts with the app and stops when the app closes
4. **Native Desktop Experience**: No browser UI, looks like a native application
5. **Cross-Platform Ready**: Can be built for Windows, macOS, and Linux

## Testing

To test the changes:

```bash
# Development mode
npm run tauri dev

# Build installer
npm run tauri build
```

The installer will be in `src-tauri/target/release/bundle/msi/` (for Windows).

## Future Improvements

Potential enhancements:
- Use a random port instead of hardcoding 5000
- Add health check before opening window
- Better error handling for server startup failures
- Add retry logic if server fails to start
