using Microsoft.JSInterop;
using SBInspector.Shared.Core.Interfaces;

namespace SBInspector.Shared.Infrastructure.Export;

public class WebFileExportService : IFileExportService
{
    private readonly IJSRuntime _jsRuntime;

    public WebFileExportService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<bool> ExportToFileAsync(string filename, string content, string contentType = "application/json")
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("downloadFile", filename, content);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
