using Microsoft.JSInterop;
using SBInspector.Core.Domain;
using SBInspector.Core.Interfaces;

namespace SBInspector.Infrastructure.Storage;

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
            StorageType.FileSystem => new FileStorageService(),
            StorageType.LocalStorage => new LocalStorageService(_jsRuntime),
            _ => new LocalStorageService(_jsRuntime)
        };
    }
}
