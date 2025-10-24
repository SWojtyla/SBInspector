# Testing the MAUI Implementation

This document provides instructions for testing the MAUI implementation on your local machine.

## Prerequisites

Before testing, ensure you have:

1. **.NET 9.0 SDK** installed
   ```bash
   dotnet --version
   # Should show 9.0.x or later
   ```

2. **MAUI workload** installed
   ```bash
   dotnet workload install maui
   ```

3. For specific platforms:
   - **Windows**: Windows 10 SDK 10.0.19041.0 or later
   - **Android**: Android SDK and emulator or physical device
   - **iOS/macOS**: Xcode on macOS

## Testing Steps

### 1. Test Blazor Server (Web) Application

```bash
# Navigate to the solution directory
cd /path/to/SBInspector

# Build the Blazor Server project
cd SBInspector
dotnet build

# Run the Blazor Server project
dotnet run

# Open browser to https://localhost:5001 or http://localhost:5000
```

**Expected Result**: The application should start, and you should be able to connect to Azure Service Bus and perform all operations.

### 2. Test Shared Library

```bash
# Navigate to shared library
cd /path/to/SBInspector/SBInspector.Shared

# Build the shared library
dotnet build

# Expected: Build should succeed with no errors
```

### 3. Test MAUI Application

#### Option A: Using Visual Studio 2022

1. Open `SBInspector.sln` in Visual Studio 2022
2. In Solution Explorer, right-click on `SEBInspector.Maui` and select "Set as Startup Project"
3. Select your target platform from the dropdown (Windows, Android, iOS, etc.)
4. Press F5 or click the "Run" button

**Expected Result**: The MAUI application should launch with the same UI as the Blazor Server version.

#### Option B: Using Command Line

**Windows:**
```bash
cd /path/to/SBInspector/SEBInspector.Maui
dotnet build -f net9.0-windows10.0.19041.0
dotnet run -f net9.0-windows10.0.19041.0
```

**Android:**
```bash
cd /path/to/SBInspector/SEBInspector.Maui
dotnet build -f net9.0-android

# Deploy to connected device or emulator
# (Requires Android device/emulator to be running)
```

**iOS/macOS (requires Mac):**
```bash
cd /path/to/SBInspector/SEBInspector.Maui

# iOS
dotnet build -f net9.0-ios

# macOS
dotnet build -f net9.0-maccatalyst
```

## Functional Testing

Once the application is running (either Blazor Server or MAUI), test the following features:

### 1. Connection
- [ ] Enter a valid Azure Service Bus connection string
- [ ] Click "Connect"
- [ ] Verify that queues and topics are loaded

### 2. Queue Operations
- [ ] Select a queue from the tree view
- [ ] View queue details (message counts, status)
- [ ] Click "View Active Messages"
- [ ] Verify messages are displayed

### 3. Message Operations
- [ ] Select a message from the list
- [ ] Click to view message details
- [ ] Test "Delete" operation
- [ ] Test "Send New Message" operation
- [ ] Test "Purge All" (on a test queue with few messages)

### 4. Topic/Subscription Operations
- [ ] Select a topic from the tree view
- [ ] Expand the topic to see subscriptions
- [ ] Select a subscription
- [ ] View subscription messages
- [ ] Test message operations on subscription messages

### 5. Filtering and Sorting
- [ ] Add a message filter by property
- [ ] Verify filtered results
- [ ] Sort messages by different columns
- [ ] Test pagination (Load More)

### 6. Storage
- [ ] Save a connection string (checkbox "Save this connection")
- [ ] Close and reopen the application
- [ ] Verify saved connection appears in dropdown
- [ ] Test message templates (Templates page)

### 7. Entity Status
- [ ] Select a queue
- [ ] Toggle its status (Enable/Disable)
- [ ] Verify status change is reflected

## Comparing Blazor Server and MAUI

Both applications should behave identically. Test the same features in both to verify:

| Feature | Blazor Server | MAUI | Notes |
|---------|---------------|------|-------|
| Connection | ✓ | ✓ | |
| View Queues | ✓ | ✓ | |
| View Topics | ✓ | ✓ | |
| View Messages | ✓ | ✓ | |
| Delete Message | ✓ | ✓ | |
| Send Message | ✓ | ✓ | |
| Requeue | ✓ | ✓ | |
| Purge | ✓ | ✓ | |
| Filtering | ✓ | ✓ | |
| Sorting | ✓ | ✓ | |
| Pagination | ✓ | ✓ | |
| Templates | ✓ | ✓ | |
| Storage | ✓ | ✓ | File location differs |

## Storage Location Testing

### Blazor Server
- Browser Local Storage: Use browser dev tools (F12) → Application → Local Storage
- File System: Check Desktop folder for `sbinspector_connections.json` and `sbinspector_templates.json`

### MAUI
Check platform-specific locations:

**Windows:**
```
%LOCALAPPDATA%\SBInspector\
```

**macOS:**
```
~/Library/Application Support/SBInspector/
```

**Android:**
Use Android Device File Explorer or adb:
```bash
adb shell
cd /data/data/com.companyname.sebinspector.maui/files
ls
```

## Troubleshooting

### MAUI Build Fails

**Error: "NETSDK1147: To build this project, the following workloads must be installed: maui"**

**Solution:**
```bash
dotnet workload install maui
```

### Missing Dependencies

**Error: Package not found**

**Solution:**
```bash
dotnet restore
```

### Platform-Specific Issues

**Windows:**
- Enable Developer Mode: Settings → Update & Security → For developers → Developer Mode

**Android:**
- Ensure Android SDK is installed
- Start an emulator or connect a physical device
- Enable USB debugging on physical device

**iOS/macOS:**
- Ensure Xcode is installed
- Accept Xcode license: `sudo xcodebuild -license accept`
- For physical devices, configure provisioning profiles

## Performance Testing

Test with different message volumes:

1. **Small Queue** (< 100 messages)
   - Should load instantly
   - All operations should be fast

2. **Medium Queue** (100-1000 messages)
   - Should load within 1-2 seconds
   - Pagination should work smoothly

3. **Large Queue** (1000+ messages)
   - Initial load should be paginated (default 100)
   - Load More should work incrementally
   - Filtering should not timeout

## Automated Testing

While manual testing is important, you can also run automated tests:

```bash
# If unit tests are available
dotnet test
```

## Reporting Issues

When reporting issues, please include:

1. Platform (Blazor Server / MAUI - Windows / Android / iOS / macOS)
2. .NET version: `dotnet --version`
3. Steps to reproduce
4. Expected vs actual behavior
5. Screenshots if applicable
6. Any error messages from console/logs

## Success Criteria

The implementation is successful if:

- [x] Blazor Server builds and runs without errors
- [x] Shared library builds without errors
- [ ] MAUI project builds on a machine with MAUI workload installed
- [ ] MAUI application runs and displays the same UI as Blazor Server
- [ ] All features work identically in both applications
- [ ] Storage works correctly in both applications
- [ ] No code duplication between Blazor Server and MAUI projects

## Next Steps

After successful testing:

1. Test on multiple platforms (Windows, Android, iOS, macOS)
2. Test on different screen sizes
3. Consider platform-specific UI optimizations
4. Add automated UI tests
5. Set up CI/CD for MAUI builds
6. Create distribution packages (installers, app store packages)

## Notes

- The CI environment (GitHub Actions on Linux) cannot build MAUI projects because the MAUI workload is not available on Linux
- All testing must be done on local developer machines
- For CI/CD, consider using Windows runners for MAUI builds
