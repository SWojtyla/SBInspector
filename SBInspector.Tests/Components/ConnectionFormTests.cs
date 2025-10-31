using Bunit;
using FakeItEasy;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using SBInspector.Shared.Application.Services;
using SBInspector.Shared.Core.Domain;
using SBInspector.Shared.Core.Interfaces;
using SBInspector.Shared.Presentation.Components.UI;

namespace SBInspector.Tests.Components;

public class ConnectionFormTests : TestContext
{
    private readonly IServiceBusService _mockServiceBusService;
    private readonly IStorageService _mockStorageService;
    private readonly ConnectionStateService _connectionState;
    private readonly ConnectionStringEncryptionService _encryptionService;

    public ConnectionFormTests()
    {
        _mockServiceBusService = A.Fake<IServiceBusService>();
        _mockStorageService = A.Fake<IStorageService>();
        _connectionState = new ConnectionStateService();
        
        // Create a stub implementation of IDataProtector for testing
        var mockDataProtectionProvider = A.Fake<IDataProtectionProvider>();
        var stubDataProtector = new StubDataProtector();
        
        A.CallTo(() => mockDataProtectionProvider.CreateProtector(A<string>._))
            .Returns(stubDataProtector);
        
        _encryptionService = new ConnectionStringEncryptionService(mockDataProtectionProvider);

        // Register services
        Services.AddSingleton(_mockServiceBusService);
        Services.AddSingleton(_mockStorageService);
        Services.AddSingleton(_connectionState);
        Services.AddSingleton(_encryptionService);
    }

    // Stub implementation of IDataProtector for testing
    private class StubDataProtector : IDataProtector
    {
        public IDataProtector CreateProtector(string purpose) => this;

        public byte[] Protect(byte[] plaintext)
        {
            // Simple encoding for testing
            var encrypted = new byte[plaintext.Length + 10];
            Array.Copy(plaintext, 0, encrypted, 10, plaintext.Length);
            return encrypted;
        }

        public byte[] Unprotect(byte[] protectedData)
        {
            // Simple decoding for testing
            var decrypted = new byte[protectedData.Length - 10];
            Array.Copy(protectedData, 10, decrypted, 0, decrypted.Length);
            return decrypted;
        }
    }

    [Fact]
    public void ConnectionForm_RendersInitially()
    {
        // Arrange
        A.CallTo(() => _mockStorageService.GetSavedConnectionsAsync())
            .Returns(Task.FromResult(new List<SavedConnection>()));

        // Act
        var cut = RenderComponent<ConnectionForm>();

        // Assert
        Assert.Contains("Connect to Service Bus", cut.Markup);
        Assert.Contains("Connection String", cut.Markup);
        Assert.Contains("Connect", cut.Markup);
    }

    [Fact]
    public void ConnectionForm_WithSavedConnections_RendersDropdown()
    {
        // Arrange
        var savedConnections = new List<SavedConnection>
        {
            new SavedConnection
            {
                Name = "Test Connection",
                ConnectionString = "encrypted_test",
                IsEncrypted = true,
                LastUsedAt = DateTime.UtcNow
            }
        };

        A.CallTo(() => _mockStorageService.GetSavedConnectionsAsync())
            .Returns(Task.FromResult(savedConnections));

        // Act
        var cut = RenderComponent<ConnectionForm>();
        cut.WaitForAssertion(() => cut.Find("select"));

        // Assert
        Assert.Contains("Saved Connections", cut.Markup);
        Assert.Contains("Test Connection", cut.Markup);
    }

    [Fact]
    public void ConnectionForm_EmptyConnectionString_ShowsPlaceholder()
    {
        // Arrange
        A.CallTo(() => _mockStorageService.GetSavedConnectionsAsync())
            .Returns(Task.FromResult(new List<SavedConnection>()));

        // Act
        var cut = RenderComponent<ConnectionForm>();

        // Assert
        var input = cut.Find("input[type='password']");
        Assert.Contains("Endpoint=sb://...", input.GetAttribute("placeholder"));
    }

    [Fact]
    public void ConnectionForm_WithErrorMessage_DisplaysError()
    {
        // Arrange
        const string errorMessage = "Connection failed";
        A.CallTo(() => _mockStorageService.GetSavedConnectionsAsync())
            .Returns(Task.FromResult(new List<SavedConnection>()));

        // Act
        var cut = RenderComponent<ConnectionForm>(parameters => parameters
            .Add(p => p.ErrorMessage, errorMessage));

        // Assert
        Assert.Contains("alert-danger", cut.Markup);
        Assert.Contains(errorMessage, cut.Markup);
    }

    [Fact]
    public async Task ConnectionForm_SuccessfulConnection_InvokesOnConnected()
    {
        // Arrange
        var onConnectedCalled = false;
        const string connectionString = "Endpoint=sb://test.servicebus.windows.net/;";

        A.CallTo(() => _mockServiceBusService.ConnectAsync(A<string>._))
            .Returns(Task.FromResult(true));
        A.CallTo(() => _mockStorageService.GetSavedConnectionsAsync())
            .Returns(Task.FromResult(new List<SavedConnection>()));

        var cut = RenderComponent<ConnectionForm>(parameters => parameters
            .Add(p => p.ConnectionString, connectionString)
            .Add(p => p.OnConnected, () => { onConnectedCalled = true; }));

        // Act
        var connectButton = cut.Find("button.btn-primary");
        await cut.InvokeAsync(() => connectButton.Click());

        // Assert
        Assert.True(onConnectedCalled);
        A.CallTo(() => _mockServiceBusService.ConnectAsync(connectionString))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ConnectionForm_FailedConnection_SetsErrorMessage()
    {
        // Arrange
        const string connectionString = "invalid-connection-string";
        string? capturedErrorMessage = null;

        A.CallTo(() => _mockServiceBusService.ConnectAsync(A<string>._))
            .Returns(Task.FromResult(false));
        A.CallTo(() => _mockStorageService.GetSavedConnectionsAsync())
            .Returns(Task.FromResult(new List<SavedConnection>()));

        var cut = RenderComponent<ConnectionForm>(parameters => parameters
            .Add(p => p.ConnectionString, connectionString)
            .Add(p => p.ErrorMessageChanged, (string msg) => { capturedErrorMessage = msg; }));

        // Act
        var connectButton = cut.Find("button.btn-primary");
        await cut.InvokeAsync(() => connectButton.Click());

        // Assert
        Assert.NotNull(capturedErrorMessage);
        Assert.Contains("Failed to connect", capturedErrorMessage);
    }

    [Fact]
    public void ConnectionForm_SaveConnectionCheckbox_ShowsNameInput()
    {
        // Arrange
        A.CallTo(() => _mockStorageService.GetSavedConnectionsAsync())
            .Returns(Task.FromResult(new List<SavedConnection>()));

        var cut = RenderComponent<ConnectionForm>();

        // Act
        var checkbox = cut.Find("input[type='checkbox']#saveConnection");
        checkbox.Change(true);

        // Assert
        cut.WaitForAssertion(() =>
        {
            var nameInput = cut.Find("input[type='text'][placeholder*='name']");
            Assert.NotNull(nameInput);
        });
    }

    [Fact]
    public async Task ConnectionForm_SaveConnection_CallsStorageService()
    {
        // Arrange
        const string connectionString = "Endpoint=sb://test.servicebus.windows.net/;";
        const string connectionName = "My Test Connection";

        A.CallTo(() => _mockServiceBusService.ConnectAsync(A<string>._))
            .Returns(Task.FromResult(true));
        A.CallTo(() => _mockStorageService.GetSavedConnectionsAsync())
            .Returns(Task.FromResult(new List<SavedConnection>()));

        var cut = RenderComponent<ConnectionForm>(parameters => parameters
            .Add(p => p.ConnectionString, connectionString));

        // Enable save connection checkbox
        var checkbox = cut.Find("input[type='checkbox']#saveConnection");
        checkbox.Change(true);

        // Enter connection name
        cut.WaitForAssertion(() =>
        {
            var nameInput = cut.Find("input[type='text'][placeholder*='name']");
            nameInput.Change(connectionName);
        });

        // Act
        var connectButton = cut.Find("button.btn-primary");
        await cut.InvokeAsync(() => connectButton.Click());

        // Assert
        A.CallTo(() => _mockStorageService.SaveConnectionAsync(
            A<SavedConnection>.That.Matches(c =>
                c.Name == connectionName &&
                c.IsEncrypted == true)))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void ConnectionForm_ConnectButton_DisabledWhileConnecting()
    {
        // Arrange
        A.CallTo(() => _mockStorageService.GetSavedConnectionsAsync())
            .Returns(Task.FromResult(new List<SavedConnection>()));

        var cut = RenderComponent<ConnectionForm>(parameters => parameters
            .Add(p => p.IsConnecting, true));

        // Assert
        var connectButton = cut.Find("button.btn-primary");
        Assert.True(connectButton.HasAttribute("disabled"));
        Assert.Contains("Connecting...", cut.Markup);
    }

    [Fact]
    public async Task ConnectionForm_DeleteConnection_RemovesFromStorage()
    {
        // Arrange
        const string connectionName = "Test Connection";
        var savedConnections = new List<SavedConnection>
        {
            new SavedConnection
            {
                Name = connectionName,
                ConnectionString = "encrypted_test",
                IsEncrypted = true
            }
        };

        A.CallTo(() => _mockStorageService.GetSavedConnectionsAsync())
            .Returns(Task.FromResult(savedConnections));

        var cut = RenderComponent<ConnectionForm>();
        cut.WaitForAssertion(() => cut.Find("select"));

        // Select the connection
        var select = cut.Find("select");
        await cut.InvokeAsync(() => select.Change(connectionName));

        // Act
        var deleteButton = cut.Find("button.btn-outline-danger");
        await cut.InvokeAsync(() => deleteButton.Click());

        // Assert
        A.CallTo(() => _mockStorageService.DeleteConnectionAsync(connectionName))
            .MustHaveHappenedOnceExactly();
    }
}
