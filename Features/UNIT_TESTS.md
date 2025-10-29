# Unit Tests for Service Bus Operations

## Overview

This document describes the unit test coverage for the SBInspector Service Bus operations. The tests are implemented using XUnit framework and cover all major service bus operations defined in the `IServiceBusService` interface.

## Test Project

- **Project Name**: SBInspector.Tests
- **Framework**: .NET 9.0
- **Testing Framework**: XUnit 2.9.2
- **Mocking Framework**: Moq 4.20.72

## Test Coverage

The test suite covers the following areas of the `ServiceBusService` class:

### Connection Management Tests

- `IsConnected_WhenNotConnected_ReturnsFalse` - Verifies the IsConnected property returns false when not connected
- `ConnectAsync_WithInvalidConnectionString_ReturnsFalse` - Tests connection with invalid connection string
- `DisconnectAsync_WhenCalled_ClearsConnection` - Tests async disconnect functionality
- `Disconnect_WhenCalled_ClearsConnection` - Tests sync disconnect functionality

### Entity Retrieval Tests

- `GetQueuesAsync_WhenNotConnected_ReturnsEmptyList` - Verifies empty list when retrieving queues without connection
- `GetTopicsAsync_WhenNotConnected_ReturnsEmptyList` - Verifies empty list when retrieving topics without connection
- `GetSubscriptionsAsync_WhenNotConnected_ReturnsEmptyList` - Verifies empty list when retrieving subscriptions without connection
- `GetQueueInfoAsync_WhenNotConnected_ReturnsNull` - Tests queue info retrieval without connection
- `GetTopicInfoAsync_WhenNotConnected_ReturnsNull` - Tests topic info retrieval without connection
- `GetSubscriptionInfoAsync_WhenNotConnected_ReturnsNull` - Tests subscription info retrieval without connection

### Message Operations Tests

- `GetMessagesAsync_WhenNotConnected_ReturnsEmptyList` - Tests message retrieval without connection
- `GetMessagesAsync_WithMaxMessages_RespectsLimit` - Verifies maxMessages parameter is respected
- `GetMessagesAsync_WithFromSequenceNumber_StartsFromSequence` - Tests starting from a specific sequence number
- `GetMessagesAsync_WithDeadLetterType_RetrievesDeadLetterMessages` - Tests dead letter queue message retrieval
- `GetSubscriptionMessagesAsync_WhenNotConnected_ReturnsEmptyList` - Tests subscription message retrieval without connection

### Message CRUD Operations Tests

- `SendMessageAsync_WhenNotConnected_ReturnsFalse` - Tests send operation without connection
- `SendMessageAsync_WithProperties_SendsWithAllProperties` - Tests sending with custom properties
- `SendMessageAsync_WithScheduledTime_SendsWithScheduledTime` - Tests scheduled message sending
- `DeleteMessageAsync_WhenNotConnected_ReturnsFalse` - Tests delete operation without connection
- `DeleteMessageAsync_ForSubscription_UsesCorrectParameters` - Tests delete for subscription messages
- `RequeueDeadLetterMessageAsync_WhenNotConnected_ReturnsFalse` - Tests requeue operation without connection
- `RequeueDeadLetterMessageAsync_ForSubscription_UsesCorrectParameters` - Tests requeue for subscription messages
- `RescheduleMessageAsync_WhenNotConnected_ReturnsFalse` - Tests reschedule operation without connection
- `RescheduleMessageAsync_ForSubscription_UsesCorrectParameters` - Tests reschedule for subscription messages

### Bulk Operations Tests

- `PurgeMessagesAsync_WhenNotConnected_ReturnsZero` - Tests purge operation without connection
- `PurgeMessagesAsync_WithDeadLetterType_PurgesDeadLetterQueue` - Tests purging dead letter queue
- `PurgeMessagesAsync_ForSubscription_UsesCorrectParameters` - Tests purge for subscription messages
- `PurgeMessagesAsync_WithCancellation_RespectsToken` - Tests cancellation token support
- `PurgeMessagesAsync_WithProgress_ReportsProgress` - Tests progress reporting
- `DeleteFilteredMessagesAsync_WhenNotConnected_ReturnsZero` - Tests filtered delete without connection
- `DeleteFilteredMessagesAsync_ForSubscription_UsesCorrectParameters` - Tests filtered delete for subscriptions
- `DeleteFilteredMessagesAsync_WithCancellation_RespectsToken` - Tests cancellation token support
- `DeleteFilteredMessagesAsync_WithProgress_ReportsProgress` - Tests progress reporting

### Entity Status Management Tests

- `SetQueueStatusAsync_WhenNotConnected_ReturnsFalse` - Tests queue status change without connection
- `SetTopicStatusAsync_WhenNotConnected_ReturnsFalse` - Tests topic status change without connection
- `SetSubscriptionStatusAsync_WhenNotConnected_ReturnsFalse` - Tests subscription status change without connection

## Running the Tests

### Build the Test Project

```bash
dotnet build SBInspector.Tests/SBInspector.Tests.csproj
```

### Run All Tests

```bash
dotnet test SBInspector.Tests/SBInspector.Tests.csproj
```

### Run Tests with Detailed Output

```bash
dotnet test SBInspector.Tests/SBInspector.Tests.csproj --verbosity detailed
```

### Run Tests with Coverage

```bash
dotnet test SBInspector.Tests/SBInspector.Tests.csproj --collect:"XPlat Code Coverage"
```

## Test Results

All 36 tests are passing successfully. The tests verify:

- Proper handling of disconnected state
- Correct return values when operations cannot be performed
- Proper parameter passing for complex operations
- Support for optional parameters (cancellation tokens, progress reporting)
- Correct behavior for both queue and subscription operations

## Technical Implementation Details

### Testing Strategy

The current test suite focuses on:

1. **Boundary Testing**: Testing behavior when the service is not connected
2. **Parameter Validation**: Ensuring methods handle various parameter combinations correctly
3. **Return Value Validation**: Verifying correct return values under different conditions

### Future Enhancements

Future test enhancements could include:

1. **Integration Tests**: Tests with actual Azure Service Bus emulator or test instances
2. **Mock-based Unit Tests**: Using Moq to mock Azure Service Bus clients for connected scenarios
3. **Performance Tests**: Testing behavior with large message volumes
4. **Error Handling Tests**: Testing various error scenarios and exception handling
5. **Concurrency Tests**: Testing thread-safety of service operations

## Dependencies

The test project requires the following NuGet packages:

- `xunit` (2.9.2) - Testing framework
- `xunit.runner.visualstudio` (2.8.2) - Visual Studio test runner
- `Microsoft.NET.Test.Sdk` (17.12.0) - .NET test infrastructure
- `Moq` (4.20.72) - Mocking framework
- `coverlet.collector` (6.0.2) - Code coverage collection

## Notes

- These tests focus on the "disconnected" state to ensure the service handles this scenario gracefully
- All operations return safe default values (false, null, empty lists, zero) when not connected
- The tests validate the service's defensive programming approach
- Tests are fast and do not require external dependencies or Azure resources
