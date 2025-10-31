using Microsoft.JSInterop;
using SBInspector.Shared.Core.Domain;
using SBInspector.Shared.Core.Interfaces;

namespace SBInspector.Shared.Infrastructure.Storage;

public class StorageServiceFactory
{
    private readonly IJSRuntime _jsRuntime;
    private readonly StorageConfiguration _configuration;

    public StorageServiceFactory(IJSRuntime jsRuntime, StorageConfiguration configuration)
    {
        _jsRuntime = jsRuntime;
        _configuration = configuration;
    }

    public IStorageService CreateStorageService()
    {
        return _configuration.StorageType switch
        {
            StorageType.FileSystem => new FileStorageService(
                null, // Use default storage directory
                string.IsNullOrWhiteSpace(_configuration.ExportPath) ? null : _configuration.ExportPath,
                string.IsNullOrWhiteSpace(_configuration.TemplatePath) ? null : _configuration.TemplatePath
            ),
            StorageType.LocalStorage => new LocalStorageService(_jsRuntime),
            _ => new LocalStorageService(_jsRuntime)
        };
    }
}
