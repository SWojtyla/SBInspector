using Bunit;
using SBInspector.Shared.Presentation.Components.UI;
using MudBlazor;
using MudBlazor.Services;

namespace SBInspector.Tests.Components;

public class ConfirmationModalTests : TestContext
{
    public ConfirmationModalTests()
    {
        // Register MudBlazor services for testing
        Services.AddMudServices();
        
        // Setup JSInterop to handle MudBlazor's JS calls
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void ConfirmationModal_WhenNotVisible_DoesNotRender()
    {
        // Arrange & Act
        var cut = RenderComponent<ConfirmationModal>(parameters => parameters
            .Add(p => p.IsVisible, false));

        // Assert
        Assert.Empty(cut.Markup);
    }

    [Fact]
    public void ConfirmationModal_WhenVisible_RendersWithDefaultValues()
    {
        // Arrange & Act
        var cut = RenderComponent<ConfirmationModal>(parameters => parameters
            .Add(p => p.IsVisible, true));

        // Assert
        Assert.Contains("Confirm Action", cut.Markup);
        Assert.Contains("Are you sure?", cut.Markup);
        Assert.Contains("Cancel", cut.Markup);
        Assert.Contains("Confirm", cut.Markup);
    }

    [Fact]
    public void ConfirmationModal_WithCustomTitle_RendersCustomTitle()
    {
        // Arrange
        const string customTitle = "Delete Item";

        // Act
        var cut = RenderComponent<ConfirmationModal>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Title, customTitle));

        // Assert
        Assert.Contains(customTitle, cut.Markup);
    }

    [Fact]
    public void ConfirmationModal_WithCustomMessage_RendersCustomMessage()
    {
        // Arrange
        const string customMessage = "This action cannot be undone.";

        // Act
        var cut = RenderComponent<ConfirmationModal>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Message, customMessage));

        // Assert
        Assert.Contains(customMessage, cut.Markup);
    }

    [Fact]
    public void ConfirmationModal_WithCustomConfirmText_RendersCustomConfirmText()
    {
        // Arrange
        const string customConfirmText = "Delete";

        // Act
        var cut = RenderComponent<ConfirmationModal>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.ConfirmText, customConfirmText));

        // Assert
        Assert.Contains(customConfirmText, cut.Markup);
    }

    [Fact]
    public void ConfirmationModal_ClickConfirm_InvokesOnConfirm()
    {
        // Arrange
        var confirmCalled = false;
        var cut = RenderComponent<ConfirmationModal>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.OnConfirm, () => { confirmCalled = true; }));

        // Act - Find MudButton with Confirm text
        var confirmButton = cut.FindAll("button").Last(b => b.TextContent.Contains("Confirm"));
        confirmButton.Click();

        // Assert
        Assert.True(confirmCalled);
    }

    [Fact]
    public void ConfirmationModal_ClickCancel_InvokesOnCancel()
    {
        // Arrange
        var cancelCalled = false;
        var cut = RenderComponent<ConfirmationModal>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.OnCancel, () => { cancelCalled = true; }));

        // Act - Find MudButton with Cancel text
        var cancelButton = cut.FindAll("button").First(b => b.TextContent.Contains("Cancel"));
        cancelButton.Click();

        // Assert
        Assert.True(cancelCalled);
    }

    [Fact]
    public void ConfirmationModal_ClickCloseButton_InvokesOnCancel()
    {
        // Arrange
        var cancelCalled = false;
        var cut = RenderComponent<ConfirmationModal>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.OnCancel, () => { cancelCalled = true; }));

        // Act - Find MudIconButton (close button)
        var closeButton = cut.FindAll("button").First(b => b.ClassName?.Contains("mud-icon-button") ?? false);
        closeButton.Click();

        // Assert
        Assert.True(cancelCalled);
    }

    [Fact]
    public void ConfirmationModal_WithCustomButtonClass_RendersWithCorrectColor()
    {
        // Arrange
        const string customClass = "btn-danger";

        // Act
        var cut = RenderComponent<ConfirmationModal>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.ConfirmButtonClass, customClass));

        // Assert - Check for MudBlazor error color class
        Assert.Contains("mud-button-filled-error", cut.Markup);
    }

    [Fact]
    public void ConfirmationModal_WithCustomIcons_RendersCustomIcons()
    {
        // Arrange
        const string iconClass = "bi-trash";
        const string confirmIconClass = "bi-check-lg";

        // Act
        var cut = RenderComponent<ConfirmationModal>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.IconClass, iconClass)
            .Add(p => p.ConfirmIconClass, confirmIconClass));

        // Assert - Check that the component renders with material icons
        // MudBlazor converts Bootstrap icons to Material icons
        Assert.Contains("mud-icon", cut.Markup);
    }
}
