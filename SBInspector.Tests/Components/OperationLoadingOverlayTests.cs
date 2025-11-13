using Bunit;
using SBInspector.Shared.Presentation.Components.UI;

namespace SBInspector.Tests.Components;

public class OperationLoadingOverlayTests : TestContext
{
    [Fact]
    public void OperationLoadingOverlay_WhenNotVisible_DoesNotRender()
    {
        // Arrange & Act
        var cut = Render<OperationLoadingOverlay>(parameters => parameters
            .Add(p => p.IsVisible, false));

        // Assert
        Assert.Empty(cut.Markup);
    }

    [Fact]
    public void OperationLoadingOverlay_WhenVisible_RendersWithDefaultTitle()
    {
        // Arrange & Act
        var cut = Render<OperationLoadingOverlay>(parameters => parameters
            .Add(p => p.IsVisible, true));

        // Assert
        Assert.Contains("Processing...", cut.Markup);
        Assert.Contains("spinner-border", cut.Markup);
    }

    [Fact]
    public void OperationLoadingOverlay_WithCustomTitle_RendersCustomTitle()
    {
        // Arrange
        const string customTitle = "Deleting Messages";

        // Act
        var cut = Render<OperationLoadingOverlay>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Title, customTitle));

        // Assert
        Assert.Contains(customTitle, cut.Markup);
    }

    [Fact]
    public void OperationLoadingOverlay_WithMessage_RendersMessage()
    {
        // Arrange
        const string message = "Please wait while we process your request...";

        // Act
        var cut = Render<OperationLoadingOverlay>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Message, message));

        // Assert
        Assert.Contains(message, cut.Markup);
    }

    [Fact]
    public void OperationLoadingOverlay_WithEmptyMessage_DoesNotRenderMessageParagraph()
    {
        // Arrange & Act
        var cut = Render<OperationLoadingOverlay>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Message, string.Empty));

        // Assert
        var paragraphs = cut.FindAll("p");
        Assert.Empty(paragraphs);
    }

    [Fact]
    public void OperationLoadingOverlay_WhenVisible_HasCorrectCssClass()
    {
        // Arrange & Act
        var cut = Render<OperationLoadingOverlay>(parameters => parameters
            .Add(p => p.IsVisible, true));

        // Assert
        Assert.Contains("operation-overlay", cut.Markup);
        Assert.Contains("operation-loading-card", cut.Markup);
    }
}
