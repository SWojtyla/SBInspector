using System.Text.Json;
using SBInspector.Shared.Core.Domain;

namespace SBInspector.Shared.Application.Services;

public class ColumnConfigurationService
{
    private readonly string _configFilePath;
    private ColumnConfiguration _configuration;

    public ColumnConfigurationService()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var configDirectory = Path.Combine(appDataPath, "SBInspector");
        
        if (!Directory.Exists(configDirectory))
        {
            Directory.CreateDirectory(configDirectory);
        }
        
        _configFilePath = Path.Combine(configDirectory, "column-config.json");
        _configuration = LoadConfiguration();
    }

    public ColumnConfiguration GetConfiguration()
    {
        return _configuration;
    }

    public async Task SaveConfigurationAsync(ColumnConfiguration configuration)
    {
        _configuration = configuration;
        await SaveConfigurationAsync();
    }

    public List<ColumnDefinition> GetDefaultColumns()
    {
        return new List<ColumnDefinition>
        {
            new ColumnDefinition
            {
                Id = "MessageId",
                DisplayName = "Message ID",
                IsVisible = true,
                Type = ColumnType.MessageId,
                Order = 0,
                IsSystemColumn = true
            },
            new ColumnDefinition
            {
                Id = "OriginatingEndpoint",
                DisplayName = "Originating Endpoint",
                IsVisible = true,
                Type = ColumnType.OriginatingEndpoint,
                Order = 1,
                IsSystemColumn = true
            },
            new ColumnDefinition
            {
                Id = "EnqueuedTime",
                DisplayName = "Enqueued Time",
                IsVisible = true,
                Type = ColumnType.EnqueuedTime,
                Order = 2,
                IsSystemColumn = true
            },
            new ColumnDefinition
            {
                Id = "DeliveryCount",
                DisplayName = "Delivery",
                IsVisible = true,
                Type = ColumnType.DeliveryCount,
                Order = 3,
                IsSystemColumn = true
            }
        };
    }

    public async Task AddCustomColumnAsync(string propertyKey, string displayName)
    {
        var newColumn = new ColumnDefinition
        {
            Id = $"Custom_{propertyKey}",
            DisplayName = displayName,
            IsVisible = true,
            Type = ColumnType.CustomProperty,
            PropertyKey = propertyKey,
            Order = _configuration.Columns.Any() ? _configuration.Columns.Max(c => c.Order) + 1 : 0,
            IsSystemColumn = false
        };

        _configuration.Columns.Add(newColumn);
        await SaveConfigurationAsync();
    }

    public async Task RemoveCustomColumnAsync(string columnId)
    {
        var column = _configuration.Columns.FirstOrDefault(c => c.Id == columnId && !c.IsSystemColumn);
        if (column != null)
        {
            _configuration.Columns.Remove(column);
            await SaveConfigurationAsync();
        }
    }

    public async Task UpdateColumnVisibilityAsync(string columnId, bool isVisible)
    {
        var column = _configuration.Columns.FirstOrDefault(c => c.Id == columnId);
        if (column != null)
        {
            column.IsVisible = isVisible;
            await SaveConfigurationAsync();
        }
    }

    public async Task ReorderColumnsAsync(List<ColumnDefinition> columns)
    {
        for (int i = 0; i < columns.Count; i++)
        {
            var column = _configuration.Columns.FirstOrDefault(c => c.Id == columns[i].Id);
            if (column != null)
            {
                column.Order = i;
            }
        }
        await SaveConfigurationAsync();
    }

    public async Task ResetToDefaultAsync()
    {
        _configuration.Columns = GetDefaultColumns();
        await SaveConfigurationAsync();
    }

    private ColumnConfiguration LoadConfiguration()
    {
        try
        {
            if (File.Exists(_configFilePath))
            {
                var json = File.ReadAllText(_configFilePath);
                var config = JsonSerializer.Deserialize<ColumnConfiguration>(json);
                if (config != null && config.Columns.Any())
                {
                    return config;
                }
            }
        }
        catch
        {
            // If there's any error reading the config, return default
        }
        
        // Return default configuration
        return new ColumnConfiguration { Columns = GetDefaultColumns() };
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
