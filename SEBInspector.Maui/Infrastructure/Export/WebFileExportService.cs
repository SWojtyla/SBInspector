using Microsoft.JSInterop;
using SEBInspector.Maui.Core.Interfaces;

namespace SEBInspector.Maui.Infrastructure.Export;

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
