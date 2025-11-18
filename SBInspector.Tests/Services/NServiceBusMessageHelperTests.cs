using SBInspector.Shared.Application.Services;
using SBInspector.Shared.Core.Domain;

namespace SBInspector.Tests.Services;

public class NServiceBusMessageHelperTests
{
    [Fact]
    public void IsNServiceBusMessage_WithNServiceBusProperties_ReturnsTrue()
    {
        // Arrange
        var message = new MessageInfo
        {
            MessageId = "test-123",
            Properties = new Dictionary<string, object>
            {
                { "NServiceBus.MessageIntent", "Send" },
                { "NServiceBus.OriginatingEndpoint", "MyService" },
                { "CustomProperty", "value" }
            }
        };

        // Act
        var result = NServiceBusMessageHelper.IsNServiceBusMessage(message);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsNServiceBusMessage_WithoutNServiceBusProperties_ReturnsFalse()
    {
        // Arrange
        var message = new MessageInfo
        {
            MessageId = "test-123",
            Properties = new Dictionary<string, object>
            {
                { "CustomProperty", "value1" },
                { "AnotherProperty", "value2" }
            }
        };

        // Act
        var result = NServiceBusMessageHelper.IsNServiceBusMessage(message);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsNServiceBusMessage_WithEmptyProperties_ReturnsFalse()
    {
        // Arrange
        var message = new MessageInfo
        {
            MessageId = "test-123",
            Properties = new Dictionary<string, object>()
        };

        // Act
        var result = NServiceBusMessageHelper.IsNServiceBusMessage(message);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsNServiceBusMessage_WithNullMessage_ReturnsFalse()
    {
        // Act
        var result = NServiceBusMessageHelper.IsNServiceBusMessage(null!);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsNServiceBusMessage_WithDiagnosticProperties_ReturnsTrue()
    {
        // Arrange
        var message = new MessageInfo
        {
            MessageId = "test-123",
            Properties = new Dictionary<string, object>
            {
                { "$.diagnostics.HostId", "host-123" },
                { "CustomProperty", "value" }
            }
        };

        // Act
        var result = NServiceBusMessageHelper.IsNServiceBusMessage(message);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void GetNServiceBusProperties_ExtractsOnlyNServiceBusProperties()
    {
        // Arrange
        var message = new MessageInfo
        {
            MessageId = "test-123",
            Properties = new Dictionary<string, object>
            {
                { "NServiceBus.MessageIntent", "Send" },
                { "NServiceBus.OriginatingEndpoint", "MyService" },
                { "NServiceBus.CorrelationId", "corr-123" },
                { "CustomProperty", "value" },
                { "AnotherProperty", "value2" }
            }
        };

        // Act
        var result = NServiceBusMessageHelper.GetNServiceBusProperties(message);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.True(result.ContainsKey("NServiceBus.MessageIntent"));
        Assert.True(result.ContainsKey("NServiceBus.OriginatingEndpoint"));
        Assert.True(result.ContainsKey("NServiceBus.CorrelationId"));
        Assert.False(result.ContainsKey("CustomProperty"));
        Assert.False(result.ContainsKey("AnotherProperty"));
    }

    [Fact]
    public void GetNServiceBusProperties_WithNoNServiceBusProperties_ReturnsEmpty()
    {
        // Arrange
        var message = new MessageInfo
        {
            MessageId = "test-123",
            Properties = new Dictionary<string, object>
            {
                { "CustomProperty", "value1" },
                { "AnotherProperty", "value2" }
            }
        };

        // Act
        var result = NServiceBusMessageHelper.GetNServiceBusProperties(message);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetNServiceBusProperties_WithNullMessage_ReturnsEmpty()
    {
        // Act
        var result = NServiceBusMessageHelper.GetNServiceBusProperties(null!);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetNServiceBusProperty_ReturnsCorrectValue()
    {
        // Arrange
        var message = new MessageInfo
        {
            MessageId = "test-123",
            Properties = new Dictionary<string, object>
            {
                { "NServiceBus.MessageIntent", "Send" },
                { "NServiceBus.OriginatingEndpoint", "MyService" }
            }
        };

        // Act
        var result = NServiceBusMessageHelper.GetNServiceBusProperty(message, "NServiceBus.MessageIntent");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Send", result);
    }

    [Fact]
    public void GetNServiceBusProperty_WithNonExistentProperty_ReturnsNull()
    {
        // Arrange
        var message = new MessageInfo
        {
            MessageId = "test-123",
            Properties = new Dictionary<string, object>
            {
                { "NServiceBus.MessageIntent", "Send" }
            }
        };

        // Act
        var result = NServiceBusMessageHelper.GetNServiceBusProperty(message, "NServiceBus.NonExistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetNServiceBusProperty_WithNullMessage_ReturnsNull()
    {
        // Act
        var result = NServiceBusMessageHelper.GetNServiceBusProperty(null!, "NServiceBus.MessageIntent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void IsNServiceBusMessage_IsCaseInsensitive()
    {
        // Arrange
        var message = new MessageInfo
        {
            MessageId = "test-123",
            Properties = new Dictionary<string, object>
            {
                { "nservicebus.messageintent", "Send" }
            }
        };

        // Act
        var result = NServiceBusMessageHelper.IsNServiceBusMessage(message);

        // Assert
        Assert.True(result);
    }
}
