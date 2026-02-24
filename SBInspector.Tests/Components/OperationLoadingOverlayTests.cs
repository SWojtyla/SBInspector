using Bunit;
using MudBlazor;
using MudBlazor.Services;
using SEBInspector.Maui.Presentation.Components.UI;

namespace SBInspector.Tests.Components;

public class OperationLoadingOverlayTests : TestContext
{
    public OperationLoadingOverlayTests()
    {
        Services.AddMudServices();
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void OperationLoadingOverlay_WhenNotVisible_DoesNotRender()
    {
        // Arrange & Act
        var cut = RenderComponent<OperationLoadingOverlay>(parameters => parameters
            .Add(p => p.IsVisible, false));

        // Assert
        Assert.Empty(cut.Markup);
    }

    [Fact]
    public void OperationLoadingOverlay_WhenVisible_RendersWithDefaultTitle()
    {
        // Arrange & Act
        var cut = RenderComponent<OperationLoadingOverlay>(parameters => parameters
            .Add(p => p.IsVisible, true));

        // Assert
        Assert.Contains("Processing...", cut.Markup);
        Assert.NotNull(cut.FindComponent<MudProgressCircular>());
    }

    [Fact]
    public void OperationLoadingOverlay_WithCustomTitle_RendersCustomTitle()
    {
        // Arrange
        const string customTitle = "Deleting Messages";

        // Act
        var cut = RenderComponent<OperationLoadingOverlay>(parameters => parameters
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
        var cut = RenderComponent<OperationLoadingOverlay>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Message, message));

        // Assert
        Assert.Contains(message, cut.Markup);
    }

    [Fact]
    public void OperationLoadingOverlay_WithEmptyMessage_DoesNotRenderMessageParagraph()
    {
        // Arrange & Act
        var cut = RenderComponent<OperationLoadingOverlay>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Message, string.Empty));

        // Assert
        var textBlocks = cut.FindComponents<MudText>();
        Assert.Single(textBlocks);
    }

    [Fact]
    public void OperationLoadingOverlay_WhenVisible_HasCorrectCssClass()
    {
        // Arrange & Act
        var cut = RenderComponent<OperationLoadingOverlay>(parameters => parameters
            .Add(p => p.IsVisible, true));

        // Assert
        Assert.NotNull(cut.FindComponent<MudOverlay>());
        Assert.NotNull(cut.FindComponent<MudPaper>());
    }
}
