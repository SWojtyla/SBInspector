using SBInspector.Shared.Core.Domain;
using Xunit;

namespace SBInspector.Tests;

/// <summary>
/// Tests for message body formatting logic
/// </summary>
public class MessageBodyFormattingTests
{
    [Fact]
    public void IsJsonContent_WithJsonContentType_ReturnsTrue()
    {
        // Arrange
        var message = new MessageInfo
        {
            MessageId = "test",
            Body = "{\"key\": \"value\"}",
            ContentType = "application/json",
            EnqueuedTime = DateTime.UtcNow,
            SequenceNumber = 1,
            DeliveryCount = 0
        };

        // Act
        bool isJson = IsJsonContentHelper(message);

        // Assert
        Assert.True(isJson);
    }

    [Fact]
    public void IsJsonContent_WithJsonObject_ReturnsTrue()
    {
        // Arrange
        var message = new MessageInfo
        {
            MessageId = "test",
            Body = "{\"key\": \"value\"}",
            ContentType = string.Empty,
            EnqueuedTime = DateTime.UtcNow,
            SequenceNumber = 1,
            DeliveryCount = 0
        };

        // Act
        bool isJson = IsJsonContentHelper(message);

        // Assert
        Assert.True(isJson);
    }

    [Fact]
    public void IsJsonContent_WithJsonArray_ReturnsTrue()
    {
        // Arrange
        var message = new MessageInfo
        {
            MessageId = "test",
            Body = "[{\"key\": \"value\"}]",
            ContentType = string.Empty,
            EnqueuedTime = DateTime.UtcNow,
            SequenceNumber = 1,
            DeliveryCount = 0
        };

        // Act
        bool isJson = IsJsonContentHelper(message);

        // Assert
        Assert.True(isJson);
    }

    [Fact]
    public void IsJsonContent_WithPlainText_ReturnsFalse()
    {
        // Arrange
        var message = new MessageInfo
        {
            MessageId = "test",
            Body = "This is plain text",
            ContentType = "text/plain",
            EnqueuedTime = DateTime.UtcNow,
            SequenceNumber = 1,
            DeliveryCount = 0
        };

        // Act
        bool isJson = IsJsonContentHelper(message);

        // Assert
        Assert.False(isJson);
    }

    [Fact]
    public void IsJsonContent_WithEmptyBody_ReturnsFalse()
    {
        // Arrange
        var message = new MessageInfo
        {
            MessageId = "test",
            Body = "",
            ContentType = string.Empty,
            EnqueuedTime = DateTime.UtcNow,
            SequenceNumber = 1,
            DeliveryCount = 0
        };

        // Act
        bool isJson = IsJsonContentHelper(message);

        // Assert
        Assert.False(isJson);
    }

    [Fact]
    public void GetDisplayBody_WithValidJson_FormatsCorrectly()
    {
        // Arrange
        var compactJson = "{\"name\":\"John\",\"age\":30}";
        
        // Act
        var formatted = FormatJsonHelper(compactJson);

        // Assert
        Assert.Contains("\"name\"", formatted);
        Assert.Contains("\"John\"", formatted);
        Assert.Contains("\n", formatted); // Should have line breaks
    }

    [Fact]
    public void GetDisplayBody_WithInvalidJson_ReturnsOriginal()
    {
        // Arrange
        var invalidJson = "{invalid json";
        
        // Act
        var result = FormatJsonHelper(invalidJson);

        // Assert
        Assert.Equal(invalidJson, result);
    }

    // Helper methods that replicate the logic from MessageDetailsModal
    private bool IsJsonContentHelper(MessageInfo message)
    {
        if (message == null || string.IsNullOrWhiteSpace(message.Body))
            return false;

        var contentType = message.ContentType?.ToLower() ?? string.Empty;
        if (contentType.Contains("json"))
            return true;

        var body = message.Body.Trim();
        return (body.StartsWith("{") && body.EndsWith("}")) || 
               (body.StartsWith("[") && body.EndsWith("]"));
    }

    private string FormatJsonHelper(string jsonBody)
    {
        try
        {
            var jsonElement = System.Text.Json.JsonDocument.Parse(jsonBody);
            return System.Text.Json.JsonSerializer.Serialize(jsonElement, new System.Text.Json.JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
        }
        catch (System.Text.Json.JsonException)
        {
            // Invalid JSON, return raw body
            return jsonBody;
        }
    }
}
