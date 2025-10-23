# Testing Checklist for Tauri Configuration

This document provides a comprehensive testing checklist to verify that the Tauri configuration works correctly.

## Prerequisites

Before testing, ensure you have:
- [ ] Windows machine (for MSI installer testing)
- [ ] .NET 9.0 SDK installed
- [ ] Node.js and npm installed
- [ ] Rust toolchain installed
- [ ] Tauri CLI installed: `npm install`

## Development Mode Testing

### 1. Basic Development Run

```bash
cd /path/to/SBInspector
npm run tauri dev
```

**Expected Results:**
- [ ] Blazor server starts automatically
- [ ] Tauri window opens within a few seconds
- [ ] Application UI is visible in the window
- [ ] No browser chrome (address bar, bookmarks, etc.)
- [ ] Console shows Blazor server running on https://localhost:5000

### 2. Hot Reload Testing

While `npm run tauri dev` is running:

**Test Blazor Hot Reload:**
- [ ] Edit a Blazor component (e.g., `SBInspector/Presentation/Components/Pages/Home.razor`)
- [ ] Save the file
- [ ] Verify the UI updates automatically without restarting

**Test Tauri Hot Reload:**
- [ ] Edit `src-tauri/src/lib.rs`
- [ ] Save the file
- [ ] Verify the Tauri window restarts

### 3. Functionality Testing

- [ ] Enter an Azure Service Bus connection string
- [ ] Click "Connect"
- [ ] Verify queues and topics are displayed
- [ ] Test message inspection
- [ ] Test all CRUD operations (create, read, delete messages)
- [ ] Verify all existing features work as expected

## Production Build Testing

### 1. Build the Installer

```bash
cd /path/to/SBInspector
npm run tauri build
```

**Expected Results:**
- [ ] Build completes without errors
- [ ] MSI installer is created in `src-tauri/target/release/bundle/msi/`
- [ ] Installer file size is reasonable (will be larger due to self-contained .NET)

### 2. Install the Application

**Installation:**
- [ ] Run the MSI installer
- [ ] Follow installation wizard
- [ ] Choose installation location
- [ ] Complete installation

**Verify Installation:**
- [ ] Application shortcut is created
- [ ] Application appears in Windows Start menu
- [ ] Application appears in "Programs and Features"

### 3. First Launch Testing

**Launch the installed app:**
- [ ] Double-click the application icon
- [ ] Application window opens within 5-10 seconds
- [ ] No console window appears
- [ ] No browser opens
- [ ] Application UI is displayed correctly

**Check Logs (if needed):**
On Windows, Tauri logs are typically in: `%APPDATA%/SBInspector/logs/`

### 4. Functionality Testing (Installed App)

- [ ] Enter an Azure Service Bus connection string
- [ ] Click "Connect"
- [ ] Verify connection works
- [ ] Test all CRUD operations
- [ ] Test filtering and sorting
- [ ] Test message inspection
- [ ] Verify storage persistence (connection strings saved)

### 5. Process Management Testing

**Verify Server Process:**
- [ ] Open Task Manager
- [ ] Launch the app
- [ ] Verify `SBInspector.exe` process appears (this is the Blazor server)
- [ ] Verify the Tauri window process appears

**Verify Process Cleanup:**
- [ ] Close the application window
- [ ] Check Task Manager
- [ ] Verify both processes are terminated
- [ ] No lingering `SBInspector.exe` or Tauri processes

### 6. Network Testing

**Port Usage:**
- [ ] Launch the app
- [ ] Open Command Prompt and run: `netstat -ano | findstr :5000`
- [ ] Verify port 5000 is in use by the Blazor server
- [ ] Close the app
- [ ] Verify port 5000 is released

### 7. Uninstall Testing

**Uninstallation:**
- [ ] Go to "Programs and Features"
- [ ] Uninstall SBInspector
- [ ] Verify uninstallation completes successfully
- [ ] Verify application files are removed
- [ ] Verify shortcuts are removed

## Edge Cases and Error Testing

### 1. Port Conflict

**Test:**
- [ ] Start another application on port 5000
- [ ] Launch SBInspector
- [ ] Verify error message or graceful handling

### 2. Missing Executable (Simulated)

This would require modifying the installation, but:
- [ ] If server executable is missing, app should show error message
- [ ] Error should be logged

### 3. Multiple Instances

**Test:**
- [ ] Launch the app
- [ ] Try to launch a second instance
- [ ] Verify behavior (should either open new window or focus existing)

### 4. Server Crash Recovery

**Test:**
- [ ] Launch the app
- [ ] Manually kill the Blazor server process via Task Manager
- [ ] Try to interact with the app
- [ ] Verify error handling

## Performance Testing

### 1. Startup Time

- [ ] Measure time from launch to UI visible
- [ ] Should be under 10 seconds on modern hardware

### 2. Memory Usage

- [ ] Check memory usage in Task Manager
- [ ] Should be reasonable (likely 100-200 MB for the combo)

### 3. Resource Cleanup

- [ ] Launch and close the app multiple times
- [ ] Verify no memory leaks
- [ ] Verify all processes are properly terminated each time

## Cross-Platform Testing (Future)

When building for other platforms:

### macOS
- [ ] DMG installer works
- [ ] App launches correctly
- [ ] Code signing is valid (if signed)

### Linux
- [ ] AppImage/DEB installer works
- [ ] App launches correctly
- [ ] Desktop integration works

## Troubleshooting Common Issues

| Issue | Possible Cause | Solution |
|-------|---------------|----------|
| Window shows "Connection failed" | Server didn't start | Check logs, verify port 5000 is free |
| Installer fails | Missing dependencies | Ensure build is self-contained |
| Server process doesn't terminate | Lifecycle bug | Check Rust code, verify on_window_event handler |
| Port already in use | Another app on port 5000 | Change port or close conflicting app |

## Reporting Issues

When reporting issues, include:
- [ ] Operating system version
- [ ] Application version
- [ ] Steps to reproduce
- [ ] Expected vs actual behavior
- [ ] Log files from `%APPDATA%/SBInspector/logs/`
- [ ] Screenshots if relevant
