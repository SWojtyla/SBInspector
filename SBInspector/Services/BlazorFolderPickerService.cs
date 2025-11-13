using Microsoft.JSInterop;
using SBInspector.Shared.Core.Interfaces;

namespace SBInspector.Services;

/// <summary>
/// Blazor Server implementation of folder picker using JavaScript File System Access API
/// </summary>
public class BlazorFolderPickerService : IFolderPickerService
{
    private readonly IJSRuntime _jsRuntime;

    public BlazorFolderPickerService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<string?> PickFolderAsync()
    {
        try
        {
            var result = await _jsRuntime.InvokeAsync<string?>("pickFolder");
            return result;
        }
        catch (Exception)
        {
            // File System Access API might not be supported or user cancelled
            return null;
        }
    }
}
