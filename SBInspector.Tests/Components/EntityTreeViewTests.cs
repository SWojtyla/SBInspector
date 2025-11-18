using Bunit;
using SBInspector.Shared.Core.Domain;
using SBInspector.Shared.Presentation.Components.UI;

namespace SBInspector.Tests.Components;

public class EntityTreeViewTests : TestContext
{
    [Fact]
    public void EntityTreeView_WhenLoading_ShowsSpinner()
    {
        // Arrange & Act
        var cut = RenderComponent<EntityTreeView>(parameters => parameters
            .Add(p => p.IsLoading, true)
            .Add(p => p.Queues, new List<EntityInfo>())
            .Add(p => p.Topics, new List<EntityInfo>()));

        // Assert
        Assert.Contains("mud-progress-circular", cut.Markup);
        Assert.DoesNotContain("No entities found", cut.Markup);
    }

    [Fact]
    public void EntityTreeView_WhenNotLoadingAndNoEntities_ShowsNoEntitiesMessage()
    {
        // Arrange & Act
        var cut = RenderComponent<EntityTreeView>(parameters => parameters
            .Add(p => p.IsLoading, false)
            .Add(p => p.Queues, new List<EntityInfo>())
            .Add(p => p.Topics, new List<EntityInfo>()));

        // Assert
        Assert.Contains("No entities found", cut.Markup);
        Assert.DoesNotContain("mud-progress-circular", cut.Markup);
    }
}
