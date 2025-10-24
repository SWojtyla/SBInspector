using System.Text.Json;
using SBInspector.Shared.Core.Domain;
using SBInspector.Shared.Core.Interfaces;

namespace SBInspector.Shared.Infrastructure.Storage;

public class FileStorageService : IStorageService
{
    private readonly string _storageDirectory;
    private const string ConnectionsFileName = "connections.json";
    private const string TemplatesFileName = "templates.json";

    public FileStorageService()
    {
        // Store in user's Desktop folder for easy access
        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        _storageDirectory = Path.Combine(desktopPath, "SBInspector");
        
        // Ensure directory exists
        if (!Directory.Exists(_storageDirectory))
        {
            Directory.CreateDirectory(_storageDirectory);
        }
    }

    // Connection management
    public async Task<List<SavedConnection>> GetSavedConnectionsAsync()
    {
        try
        {
            var filePath = Path.Combine(_storageDirectory, ConnectionsFileName);
            if (!File.Exists(filePath))
            {
                return new List<SavedConnection>();
            }

            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<List<SavedConnection>>(json) ?? new List<SavedConnection>();
        }
        catch
        {
            return new List<SavedConnection>();
        }
    }

    public async Task SaveConnectionAsync(SavedConnection connection)
    {
        var connections = await GetSavedConnectionsAsync();
        
        // Remove existing connection with same name
        connections.RemoveAll(c => c.Name.Equals(connection.Name, StringComparison.OrdinalIgnoreCase));
        
        // Add new connection
        connection.CreatedAt = DateTime.UtcNow;
        connections.Add(connection);
        
        var filePath = Path.Combine(_storageDirectory, ConnectionsFileName);
        var json = JsonSerializer.Serialize(connections, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json);
    }

    public async Task DeleteConnectionAsync(string name)
    {
        var connections = await GetSavedConnectionsAsync();
        connections.RemoveAll(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        
        var filePath = Path.Combine(_storageDirectory, ConnectionsFileName);
        var json = JsonSerializer.Serialize(connections, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json);
    }

    public async Task UpdateLastUsedAsync(string name)
    {
        var connections = await GetSavedConnectionsAsync();
        var connection = connections.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        
        if (connection != null)
        {
            connection.LastUsedAt = DateTime.UtcNow;
            var filePath = Path.Combine(_storageDirectory, ConnectionsFileName);
            var json = JsonSerializer.Serialize(connections, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);
        }
    }

    // Template management
    public async Task<List<MessageTemplate>> GetMessageTemplatesAsync()
    {
        try
        {
            var filePath = Path.Combine(_storageDirectory, TemplatesFileName);
            if (!File.Exists(filePath))
            {
                return new List<MessageTemplate>();
            }

            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<List<MessageTemplate>>(json) ?? new List<MessageTemplate>();
        }
        catch
        {
            return new List<MessageTemplate>();
        }
    }

    public async Task SaveMessageTemplateAsync(MessageTemplate template)
    {
        var templates = await GetMessageTemplatesAsync();
        
        // Remove existing template with same id
        templates.RemoveAll(t => t.Id == template.Id);
        
        // Add new template
        template.CreatedAt = DateTime.UtcNow;
        templates.Add(template);
        
        var filePath = Path.Combine(_storageDirectory, TemplatesFileName);
        var json = JsonSerializer.Serialize(templates, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json);
    }

    public async Task DeleteMessageTemplateAsync(string id)
    {
        var templates = await GetMessageTemplatesAsync();
        templates.RemoveAll(t => t.Id == id);
        
        var filePath = Path.Combine(_storageDirectory, TemplatesFileName);
        var json = JsonSerializer.Serialize(templates, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json);
    }

    public async Task UpdateTemplateLastUsedAsync(string id)
    {
        var templates = await GetMessageTemplatesAsync();
        var template = templates.FirstOrDefault(t => t.Id == id);
        
        if (template != null)
        {
            template.LastUsedAt = DateTime.UtcNow;
            var filePath = Path.Combine(_storageDirectory, TemplatesFileName);
            var json = JsonSerializer.Serialize(templates, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);
        }
    }
}
