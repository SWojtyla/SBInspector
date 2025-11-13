using SBInspector.Services;
using Microsoft.JSInterop;
using FakeItEasy;

namespace SBInspector.Tests.Services;

public class FolderPickerServiceTests
{
    [Fact]
    public async Task BlazorFolderPickerService_PickFolderAsync_ReturnsPath()
    {
        // Arrange
        var mockJsRuntime = A.Fake<IJSRuntime>();
        var expectedPath = "/test/path";
        A.CallTo(() => mockJsRuntime.InvokeAsync<string?>("pickFolder", A<object[]>._))
            .Returns(ValueTask.FromResult<string?>(expectedPath));

        var service = new BlazorFolderPickerService(mockJsRuntime);

        // Act
        var result = await service.PickFolderAsync();

        // Assert
        Assert.Equal(expectedPath, result);
    }

    [Fact]
    public async Task BlazorFolderPickerService_PickFolderAsync_WhenCancelled_ReturnsNull()
    {
        // Arrange
        var mockJsRuntime = A.Fake<IJSRuntime>();
        A.CallTo(() => mockJsRuntime.InvokeAsync<string?>("pickFolder", A<object[]>._))
            .Returns(ValueTask.FromResult<string?>(null));

        var service = new BlazorFolderPickerService(mockJsRuntime);

        // Act
        var result = await service.PickFolderAsync();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task BlazorFolderPickerService_PickFolderAsync_WhenExceptionThrown_ReturnsNull()
    {
        // Arrange
        var mockJsRuntime = A.Fake<IJSRuntime>();
        A.CallTo(() => mockJsRuntime.InvokeAsync<string?>("pickFolder", A<object[]>._))
            .Throws<Exception>();

        var service = new BlazorFolderPickerService(mockJsRuntime);

        // Act
        var result = await service.PickFolderAsync();

        // Assert
        Assert.Null(result);
    }
}
