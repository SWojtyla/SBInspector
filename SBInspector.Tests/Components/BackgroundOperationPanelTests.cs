using Bunit;
using MudBlazor;
using MudBlazor.Services;
using SEBInspector.Maui.Presentation.Components.UI;
using System.Linq;

namespace SEBInspector.Tests.Components;

public class BackgroundOperationPanelTests : TestContext
{
    public BackgroundOperationPanelTests()
    {
        Services.AddMudServices();
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void BackgroundOperationPanel_WhenNotVisible_DoesNotRenderPanel()
    {
        // Arrange & Act
        var cut = RenderComponent<BackgroundOperationPanel>(parameters => parameters
            .Add(p => p.IsVisible, false));

        // Assert
        Assert.Empty(cut.Markup);
    }

    [Fact]
    public void BackgroundOperationPanel_WhenVisible_RendersWithDefaultTitle()
    {
        // Arrange & Act
        var cut = RenderComponent<BackgroundOperationPanel>(parameters => parameters
            .Add(p => p.IsVisible, true));

        // Assert
        Assert.Contains("Processing...", cut.Markup);
        Assert.Contains("mud-animate-spin", cut.Markup);
    }

    [Fact]
    public void BackgroundOperationPanel_WithCustomTitle_RendersCustomTitle()
    {
        // Arrange
        const string customTitle = "Purging Messages";

        // Act
        var cut = RenderComponent<BackgroundOperationPanel>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Title, customTitle));

        // Assert
        Assert.Contains(customTitle, cut.Markup);
    }

    [Fact]
    public void BackgroundOperationPanel_WithMessage_RendersMessage()
    {
        // Arrange
        const string message = "Deleting messages from queue...";

        // Act
        var cut = RenderComponent<BackgroundOperationPanel>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Message, message));

        // Assert
        Assert.Contains(message, cut.Markup);
    }

    [Fact]
    public void BackgroundOperationPanel_WithEmptyMessage_DoesNotRenderMessageParagraph()
    {
        // Arrange & Act
        var cut = RenderComponent<BackgroundOperationPanel>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Message, string.Empty));

        // Assert
        var textBlocks = cut.FindComponents<MudText>();
        Assert.Single(textBlocks);
    }

    [Fact]
    public void BackgroundOperationPanel_WithProgress_RendersProgressBar()
    {
        // Arrange
        const int progress = 42;

        // Act
        var cut = RenderComponent<BackgroundOperationPanel>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Progress, progress));

        // Assert
        Assert.NotNull(cut.FindComponent<MudProgressLinear>());
        Assert.Contains("42 deleted", cut.Markup);
    }

    [Fact]
    public void BackgroundOperationPanel_WithZeroProgress_DoesNotRenderProgressBar()
    {
        // Arrange & Act
        var cut = RenderComponent<BackgroundOperationPanel>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Progress, 0));

        // Assert
        var progressBars = cut.FindComponents<MudProgressLinear>();
        Assert.Empty(progressBars);
    }

    [Fact]
    public void BackgroundOperationPanel_ClickCancel_InvokesOnCancel()
    {
        // Arrange
        var cancelCalled = false;
        var cut = RenderComponent<BackgroundOperationPanel>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.OnCancel, () => { cancelCalled = true; }));

        // Act
        var cancelButton = cut.FindAll("button")
            .First(button => button.TextContent.Contains("Cancel"));
        cancelButton.Click();

        // Assert
        Assert.True(cancelCalled);
    }

    [Fact]
    public void BackgroundOperationPanel_WhenVisible_HasCorrectCssClasses()
    {
        // Arrange & Act
        var cut = RenderComponent<BackgroundOperationPanel>(parameters => parameters
            .Add(p => p.IsVisible, true));

        // Assert
        Assert.NotNull(cut.FindComponent<MudPaper>());
        Assert.NotNull(cut.FindComponent<MudCard>());
    }

    [Fact]
    public void BackgroundOperationPanel_HasCancelButton_WithCorrectAttributes()
    {
        // Arrange & Act
        var cut = RenderComponent<BackgroundOperationPanel>(parameters => parameters
            .Add(p => p.IsVisible, true));

        // Assert
        var cancelButton = cut.FindAll("button")
            .First(button => button.TextContent.Contains("Cancel"));
        Assert.Equal("Cancel operation", cancelButton.GetAttribute("title"));
    }
}
