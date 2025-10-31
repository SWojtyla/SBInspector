# Blazor Component Testing with bUnit

This document describes the bUnit testing approach for Blazor components in the SBInspector project.

## Overview

The SBInspector project uses [bUnit](https://bunit.dev/) for testing Blazor components. bUnit is a testing library for Blazor components that makes it easy to write tests that render components, interact with them, and verify their output.

## Test Organization

Component tests are organized in the `SBInspector.Tests/Components/` directory, with one test file per component:

- `ConfirmationModalTests.cs` - Tests for the ConfirmationModal component
- `OperationLoadingOverlayTests.cs` - Tests for the OperationLoadingOverlay component
- `BackgroundOperationPanelTests.cs` - Tests for the BackgroundOperationPanel component
- `ConnectionFormTests.cs` - Tests for the ConnectionForm component
- `TopicListTableTests.cs` - Tests for the TopicListTable component

## Test Structure

### Simple Component Tests

For simple components with minimal dependencies (e.g., ConfirmationModal, OperationLoadingOverlay), tests follow this pattern:

```csharp
public class SimpleComponentTests : TestContext
{
    [Fact]
    public void Component_Scenario_ExpectedBehavior()
    {
        // Arrange
        var cut = RenderComponent<SimpleComponent>(parameters => parameters
            .Add(p => p.PropertyName, value));

        // Act
        // Interact with the component if needed
        var button = cut.Find("button.some-class");
        button.Click();

        // Assert
        Assert.Contains("expected text", cut.Markup);
    }
}
```

### Complex Component Tests with Dependencies

For components that require service dependencies (e.g., ConnectionForm), tests use FakeItEasy for mocking:

```csharp
public class ComplexComponentTests : TestContext
{
    private readonly IServiceBusService _mockService;

    public ComplexComponentTests()
    {
        _mockService = A.Fake<IServiceBusService>();
        Services.AddSingleton(_mockService);
    }

    [Fact]
    public void Component_WithMockedService_WorksCorrectly()
    {
        // Arrange
        A.CallTo(() => _mockService.SomeMethod())
            .Returns(Task.FromResult(true));

        // Act
        var cut = RenderComponent<ComplexComponent>();

        // Assert
        A.CallTo(() => _mockService.SomeMethod())
            .MustHaveHappenedOnceExactly();
    }
}
```

## Testing Patterns

### Testing Rendering

Test that components render correctly with different parameter combinations:

```csharp
[Fact]
public void Component_WithCustomTitle_RendersCustomTitle()
{
    var cut = RenderComponent<Component>(parameters => parameters
        .Add(p => p.Title, "Custom Title"));

    Assert.Contains("Custom Title", cut.Markup);
}
```

### Testing User Interactions

Test that user interactions trigger the expected behavior:

```csharp
[Fact]
public void Component_ClickButton_InvokesCallback()
{
    var callbackInvoked = false;
    var cut = RenderComponent<Component>(parameters => parameters
        .Add(p => p.OnClick, () => { callbackInvoked = true; }));

    var button = cut.Find("button");
    button.Click();

    Assert.True(callbackInvoked);
}
```

### Testing Input Events

For inputs that use `oninput` event (not `onchange`), use the `Input()` method:

```csharp
[Fact]
public void Component_WithSearchInput_FiltersResults()
{
    var cut = RenderComponent<Component>();
    
    var searchInput = cut.Find("input[placeholder*='Search']");
    searchInput.Input("search term");

    Assert.Contains("filtered result", cut.Markup);
}
```

### Testing Conditional Rendering

Test that components show/hide content based on parameters:

```csharp
[Fact]
public void Component_WhenNotVisible_DoesNotRenderPanel()
{
    var cut = RenderComponent<Component>(parameters => parameters
        .Add(p => p.IsVisible, false));

    var panels = cut.FindAll("div.panel-class");
    Assert.Empty(panels);
}
```

## Running Tests

Run all tests:
```bash
dotnet test
```

Run specific component tests:
```bash
dotnet test --filter "FullyQualifiedName~ComponentNameTests"
```

Run a specific test:
```bash
dotnet test --filter "FullyQualifiedName~ComponentNameTests.TestMethodName"
```

## Test Coverage

Current test coverage includes:

- **ConfirmationModal**: 10 tests covering visibility, custom parameters, button clicks, and callbacks
- **OperationLoadingOverlay**: 6 tests covering visibility, title/message rendering, and CSS classes
- **BackgroundOperationPanel**: 10 tests covering visibility, progress display, cancel functionality
- **ConnectionForm**: 10 tests covering rendering, connection logic, saved connections, and storage integration
- **TopicListTable**: 12 tests covering rendering, filtering, sorting, and status management

Total: **88 tests** (40 service tests + 48 component tests)

## Best Practices

1. **Arrange-Act-Assert Pattern**: Follow the AAA pattern for clear, readable tests
2. **Test Names**: Use descriptive test names that indicate what is being tested and expected outcome
3. **Minimal Mocking**: Only mock what's necessary; use real objects when possible
4. **Test One Thing**: Each test should focus on one specific behavior
5. **Async Operations**: Use `async/await` properly and use `WaitForAssertion()` when needed
6. **Component Lifecycle**: Remember that bUnit components follow Blazor lifecycle rules

## Common Issues and Solutions

### Issue: Event Handler Not Found

**Problem**: `Bunit.MissingEventHandlerException: The element does not have an event handler for the event 'onchange'`

**Solution**: Check if the component uses `oninput` instead of `onchange`. Use `.Input()` instead of `.Change()`:

```csharp
// Wrong
searchInput.Change("value");

// Correct
searchInput.Input("value");
```

### Issue: Cannot Mock Extension Methods

**Problem**: `FakeItEasy.Configuration.FakeConfigurationException: The current proxy generator can not intercept the method... Extension methods can not be intercepted`

**Solution**: Create a stub implementation instead of trying to mock extension methods:

```csharp
private class StubDataProtector : IDataProtector
{
    public byte[] Protect(byte[] plaintext) => plaintext;
    public byte[] Unprotect(byte[] protectedData) => protectedData;
}
```

### Issue: Component Not Rendering as Expected

**Problem**: Component doesn't show expected content after state change

**Solution**: Use `WaitForAssertion()` for async operations:

```csharp
cut.WaitForAssertion(() =>
{
    var element = cut.Find("expected-element");
    Assert.NotNull(element);
});
```

## Further Reading

- [bUnit Documentation](https://bunit.dev/)
- [bUnit GitHub Repository](https://github.com/bUnit-dev/bUnit)
- [Blazor Testing Best Practices](https://docs.microsoft.com/en-us/aspnet/core/blazor/test)
