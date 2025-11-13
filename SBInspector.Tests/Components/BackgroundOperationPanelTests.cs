using Bunit;
using SBInspector.Shared.Presentation.Components.UI;

namespace SBInspector.Tests.Components;

public class BackgroundOperationPanelTests : TestContext
{
    [Fact]
    public void BackgroundOperationPanel_WhenNotVisible_DoesNotRenderPanel()
    {
        // Arrange & Act
        var cut = Render<BackgroundOperationPanel>(parameters => parameters
            .Add(p => p.IsVisible, false));

        // Assert - The panel div should not be rendered (CSS is always rendered)
        var panelDivs = cut.FindAll("div.background-operation-panel");
        Assert.Empty(panelDivs);
    }

    [Fact]
    public void BackgroundOperationPanel_WhenVisible_RendersWithDefaultTitle()
    {
        // Arrange & Act
        var cut = Render<BackgroundOperationPanel>(parameters => parameters
            .Add(p => p.IsVisible, true));

        // Assert
        Assert.Contains("Processing...", cut.Markup);
        Assert.Contains("bi-arrow-repeat", cut.Markup);
    }

    [Fact]
    public void BackgroundOperationPanel_WithCustomTitle_RendersCustomTitle()
    {
        // Arrange
        const string customTitle = "Purging Messages";

        // Act
        var cut = Render<BackgroundOperationPanel>(parameters => parameters
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
        var cut = Render<BackgroundOperationPanel>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Message, message));

        // Assert
        Assert.Contains(message, cut.Markup);
    }

    [Fact]
    public void BackgroundOperationPanel_WithEmptyMessage_DoesNotRenderMessageParagraph()
    {
        // Arrange & Act
        var cut = Render<BackgroundOperationPanel>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Message, string.Empty));

        // Assert
        var messageParagraphs = cut.FindAll("p.text-muted");
        Assert.Empty(messageParagraphs);
    }

    [Fact]
    public void BackgroundOperationPanel_WithProgress_RendersProgressBar()
    {
        // Arrange
        const int progress = 42;

        // Act
        var cut = Render<BackgroundOperationPanel>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Progress, progress));

        // Assert
        Assert.Contains("progress-bar", cut.Markup);
        Assert.Contains("42 deleted", cut.Markup);
    }

    [Fact]
    public void BackgroundOperationPanel_WithZeroProgress_DoesNotRenderProgressBar()
    {
        // Arrange & Act
        var cut = Render<BackgroundOperationPanel>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Progress, 0));

        // Assert
        var progressBars = cut.FindAll(".progress");
        Assert.Empty(progressBars);
    }

    [Fact]
    public void BackgroundOperationPanel_ClickCancel_InvokesOnCancel()
    {
        // Arrange
        var cancelCalled = false;
        var cut = Render<BackgroundOperationPanel>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.OnCancel, () => { cancelCalled = true; }));

        // Act
        var cancelButton = cut.Find("button.btn-outline-danger");
        cancelButton.Click();

        // Assert
        Assert.True(cancelCalled);
    }

    [Fact]
    public void BackgroundOperationPanel_WhenVisible_HasCorrectCssClasses()
    {
        // Arrange & Act
        var cut = Render<BackgroundOperationPanel>(parameters => parameters
            .Add(p => p.IsVisible, true));

        // Assert
        Assert.Contains("background-operation-panel", cut.Markup);
        Assert.Contains("background-operation-card", cut.Markup);
    }

    [Fact]
    public void BackgroundOperationPanel_HasCancelButton_WithCorrectAttributes()
    {
        // Arrange & Act
        var cut = Render<BackgroundOperationPanel>(parameters => parameters
            .Add(p => p.IsVisible, true));

        // Assert
        var cancelButton = cut.Find("button.btn-outline-danger");
        Assert.NotNull(cancelButton);
        Assert.Contains("Cancel", cancelButton.TextContent);
        Assert.Contains("bi-x-circle", cut.Markup);
    }
}
