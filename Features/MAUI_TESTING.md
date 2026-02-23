# Testing the MAUI Implementation

This document provides instructions for testing the MAUI-only implementation on Windows.

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

3. **Windows 10 SDK 10.0.19041.0 or later**

## Testing Steps

### 1. Build the MAUI App (Windows)

```bash
# Navigate to the MAUI project
cd /path/to/SBInspector/SEBInspector.Maui

# Build the MAUI project
dotnet build -f net9.0-windows10.0.19041.0
```

### 2. Run the MAUI App (Windows)

```bash
cd /path/to/SBInspector/SEBInspector.Maui
dotnet run -f net9.0-windows10.0.19041.0
```

### 3. Run Tests

```bash
cd /path/to/SBInspector
dotnet test SBInspector.Tests
```

## Functional Testing

Once the application is running, test the following features:

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

## Storage Location Testing (Windows)

Check the MAUI app data folder:

```
%LOCALAPPDATA%\SBInspector\
```

## Troubleshooting

### MAUI Build Fails

**Error: "NETSDK1147: To build this project, the following workloads must be installed: maui"**

**Solution:**

```bash
dotnet workload install maui
```
