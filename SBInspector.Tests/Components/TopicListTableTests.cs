using Bunit;
using SBInspector.Shared.Core.Domain;
using SBInspector.Shared.Presentation.Components.UI;

namespace SBInspector.Tests.Components;

public class TopicListTableTests : TestContext
{
    [Fact]
    public void TopicListTable_WithNoTopics_ShowsWarningMessage()
    {
        // Arrange & Act
        var cut = Render<TopicListTable>(parameters => parameters
            .Add(p => p.Topics, new List<EntityInfo>()));

        // Assert
        Assert.Contains("No topics found in this namespace", cut.Markup);
        Assert.Contains("alert-warning", cut.Markup);
    }

    [Fact]
    public void TopicListTable_WithTopics_RendersTable()
    {
        // Arrange
        var topics = new List<EntityInfo>
        {
            new EntityInfo { Name = "test-topic-1", Status = "Active" },
            new EntityInfo { Name = "test-topic-2", Status = "Disabled" }
        };

        // Act
        var cut = Render<TopicListTable>(parameters => parameters
            .Add(p => p.Topics, topics));

        // Assert
        Assert.Contains("Topics (2)", cut.Markup);
        Assert.Contains("test-topic-1", cut.Markup);
        Assert.Contains("test-topic-2", cut.Markup);
    }

    [Fact]
    public void TopicListTable_WithActiveTopics_ShowsDisableButton()
    {
        // Arrange
        var topics = new List<EntityInfo>
        {
            new EntityInfo { Name = "active-topic", Status = "Active" }
        };

        // Act
        var cut = Render<TopicListTable>(parameters => parameters
            .Add(p => p.Topics, topics));

        // Assert
        Assert.Contains("Disable", cut.Markup);
        Assert.Contains("bi-pause-circle", cut.Markup);
    }

    [Fact]
    public void TopicListTable_WithDisabledTopics_ShowsEnableButton()
    {
        // Arrange
        var topics = new List<EntityInfo>
        {
            new EntityInfo { Name = "disabled-topic", Status = "Disabled" }
        };

        // Act
        var cut = Render<TopicListTable>(parameters => parameters
            .Add(p => p.Topics, topics));

        // Assert
        Assert.Contains("Enable", cut.Markup);
        Assert.Contains("bi-play-circle", cut.Markup);
    }

    [Fact]
    public void TopicListTable_WithTopics_ShowsViewSubscriptionsButton()
    {
        // Arrange
        var topics = new List<EntityInfo>
        {
            new EntityInfo { Name = "test-topic", Status = "Active" }
        };

        // Act
        var cut = Render<TopicListTable>(parameters => parameters
            .Add(p => p.Topics, topics));

        // Assert
        Assert.Contains("View Subscriptions", cut.Markup);
        Assert.Contains("bi-list", cut.Markup);
    }

    [Fact]
    public void TopicListTable_ClickToggleStatus_InvokesCallback()
    {
        // Arrange
        var capturedTopicName = string.Empty;
        var topics = new List<EntityInfo>
        {
            new EntityInfo { Name = "test-topic", Status = "Active" }
        };

        var cut = Render<TopicListTable>(parameters => parameters
            .Add(p => p.Topics, topics)
            .Add(p => p.OnToggleStatus, (string name) => { capturedTopicName = name; }));

        // Act
        var toggleButton = cut.Find("button.btn-secondary");
        toggleButton.Click();

        // Assert
        Assert.Equal("test-topic", capturedTopicName);
    }

    [Fact]
    public void TopicListTable_ClickViewSubscriptions_InvokesCallback()
    {
        // Arrange
        var capturedTopicName = string.Empty;
        var topics = new List<EntityInfo>
        {
            new EntityInfo { Name = "test-topic", Status = "Active" }
        };

        var cut = Render<TopicListTable>(parameters => parameters
            .Add(p => p.Topics, topics)
            .Add(p => p.OnViewSubscriptions, (string name) => { capturedTopicName = name; }));

        // Act
        var viewButton = cut.Find("button.btn-info");
        viewButton.Click();

        // Assert
        Assert.Equal("test-topic", capturedTopicName);
    }

    [Fact]
    public void TopicListTable_WithSearchTerm_FiltersTopics()
    {
        // Arrange
        var topics = new List<EntityInfo>
        {
            new EntityInfo { Name = "production-topic", Status = "Active" },
            new EntityInfo { Name = "development-topic", Status = "Active" },
            new EntityInfo { Name = "test-topic", Status = "Active" }
        };

        var cut = Render<TopicListTable>(parameters => parameters
            .Add(p => p.Topics, topics));

        // Act
        var searchInput = cut.Find("input[placeholder*='Search']");
        searchInput.Input("production");

        // Assert
        Assert.Contains("production-topic", cut.Markup);
        Assert.DoesNotContain("development-topic", cut.Markup);
        Assert.DoesNotContain("test-topic", cut.Markup);
        Assert.Contains("Topics (1)", cut.Markup);
    }

    [Fact]
    public void TopicListTable_WithNoMatchingSearch_ShowsInfoMessage()
    {
        // Arrange
        var topics = new List<EntityInfo>
        {
            new EntityInfo { Name = "test-topic", Status = "Active" }
        };

        var cut = Render<TopicListTable>(parameters => parameters
            .Add(p => p.Topics, topics));

        // Act
        var searchInput = cut.Find("input[placeholder*='Search']");
        searchInput.Input("nonexistent");

        // Assert
        Assert.Contains("No topics match your search criteria", cut.Markup);
        Assert.Contains("alert-info", cut.Markup);
    }

    [Fact]
    public void TopicListTable_ClickSortHeader_TogglesSortOrder()
    {
        // Arrange
        var topics = new List<EntityInfo>
        {
            new EntityInfo { Name = "b-topic", Status = "Active" },
            new EntityInfo { Name = "a-topic", Status = "Active" },
            new EntityInfo { Name = "c-topic", Status = "Active" }
        };

        var cut = Render<TopicListTable>(parameters => parameters
            .Add(p => p.Topics, topics));

        // Initially should show ascending order (▲)
        Assert.Contains("▲", cut.Markup);

        // Act - click to change sort order
        var sortHeader = cut.Find("th.sortable-header");
        sortHeader.Click();

        // Assert - should now show descending order (▼)
        Assert.Contains("▼", cut.Markup);
    }

    [Fact]
    public void TopicListTable_WithTopics_DisplaysCorrectStatusBadges()
    {
        // Arrange
        var topics = new List<EntityInfo>
        {
            new EntityInfo { Name = "active-topic", Status = "Active" },
            new EntityInfo { Name = "disabled-topic", Status = "Disabled" }
        };

        // Act
        var cut = Render<TopicListTable>(parameters => parameters
            .Add(p => p.Topics, topics));

        // Assert
        Assert.Contains("bg-success", cut.Markup); // Active status
        Assert.Contains("bg-secondary", cut.Markup); // Disabled status
    }

    [Fact]
    public void TopicListTable_WithTopics_ShowsSearchBox()
    {
        // Arrange
        var topics = new List<EntityInfo>
        {
            new EntityInfo { Name = "test-topic", Status = "Active" }
        };

        // Act
        var cut = Render<TopicListTable>(parameters => parameters
            .Add(p => p.Topics, topics));

        // Assert
        var searchInput = cut.Find("input[placeholder*='Search']");
        Assert.NotNull(searchInput);
    }
}
