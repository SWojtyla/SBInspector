using System.Text.Json;
using SBInspector.Core.Domain;

namespace SBInspector.Application.Services;

public class StorageConfigurationService
{
    private StorageConfiguration _configuration;
    private readonly bool _isFileSystemAvailable;

    public StorageConfigurationService()
    {
        // Check if file system is available (won't be in browser WASM)
        _isFileSystemAvailable = TryInitializeFileSystem();
        _configuration = LoadConfiguration();
    }

    private bool TryInitializeFileSystem()
    {
        try
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var configDirectory = Path.Combine(appDataPath, "SBInspector");
            
            if (!Directory.Exists(configDirectory))
            {
                Directory.CreateDirectory(configDirectory);
            }
            
            return true;
        }
        catch
        {
            // File system not available (browser WASM)
            return false;
        }
    }

    public StorageConfiguration GetConfiguration()
    {
        return _configuration;
    }

    public async Task SetStorageTypeAsync(StorageType storageType)
    {
        // Don't allow FileSystem storage if file system is not available
        if (storageType == StorageType.FileSystem && !_isFileSystemAvailable)
        {
            storageType = StorageType.LocalStorage;
        }
        
        _configuration.StorageType = storageType;
        await SaveConfigurationAsync();
    }

    private StorageConfiguration LoadConfiguration()
    {
        if (!_isFileSystemAvailable)
        {
            // In browser WASM, always use LocalStorage
            return new StorageConfiguration { StorageType = StorageType.LocalStorage };
        }
        
        try
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var configFilePath = Path.Combine(appDataPath, "SBInspector", "storage-config.json");
            
            if (File.Exists(configFilePath))
            {
                var json = File.ReadAllText(configFilePath);
                var config = JsonSerializer.Deserialize<StorageConfiguration>(json) ?? new StorageConfiguration();
                
                // Ensure FileSystem type is not used if file system is not available
                if (config.StorageType == StorageType.FileSystem && !_isFileSystemAvailable)
                {
                    config.StorageType = StorageType.LocalStorage;
                }
                
                return config;
            }
        }
        catch
        {
            // If there's any error reading the config, return default
        }
        
        return new StorageConfiguration();
    }

    private async Task SaveConfigurationAsync()
    {
        if (!_isFileSystemAvailable)
        {
            // Can't save to file system in browser WASM
            return;
        }
        
        try
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var configFilePath = Path.Combine(appDataPath, "SBInspector", "storage-config.json");
            var json = JsonSerializer.Serialize(_configuration, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(configFilePath, json);
        }
        catch
        {
            // Silently fail if we can't save configuration
        }
    }
}
