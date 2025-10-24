using System.Text.Json;
using SBInspector.Shared.Core.Domain;

namespace SBInspector.Shared.Application.Services;

public class StorageConfigurationService
{
    private readonly string _configFilePath;
    private StorageConfiguration _configuration;

    public StorageConfigurationService()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var configDirectory = Path.Combine(appDataPath, "SBInspector");
        
        if (!Directory.Exists(configDirectory))
        {
            Directory.CreateDirectory(configDirectory);
        }
        
        _configFilePath = Path.Combine(configDirectory, "storage-config.json");
        _configuration = LoadConfiguration();
    }

    public StorageConfiguration GetConfiguration()
    {
        return _configuration;
    }

    public async Task SetStorageTypeAsync(StorageType storageType)
    {
        _configuration.StorageType = storageType;
        await SaveConfigurationAsync();
    }

    private StorageConfiguration LoadConfiguration()
    {
        try
        {
            if (File.Exists(_configFilePath))
            {
                var json = File.ReadAllText(_configFilePath);
                return JsonSerializer.Deserialize<StorageConfiguration>(json) ?? new StorageConfiguration();
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
        try
        {
            var json = JsonSerializer.Serialize(_configuration, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_configFilePath, json);
        }
        catch
        {
            // Silently fail if we can't save configuration
        }
    }
}
