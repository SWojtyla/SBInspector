using System.Text.Json;
using SBInspector.Shared.Core.Domain;
using SBInspector.Shared.Core.Interfaces;

namespace SBInspector.Shared.Infrastructure.Storage;

public class FileStorageService : IStorageService
{
    private readonly string _storageDirectory;
    private readonly string _exportDirectory;
    private readonly string _templateDirectory;
    private const string ConnectionsFileName = "connections.json";
    private const string TemplatesFileName = "templates.json";

    public FileStorageService(string? customStorageDir = null, string? customExportDir = null, string? customTemplateDir = null)
    {
        // Use custom directory or default to Desktop
        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        _storageDirectory = customStorageDir ?? Path.Combine(desktopPath, "SBInspector");
        
        // Set export and template directories
        _exportDirectory = customExportDir ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SBInspector", "Exports");
        _templateDirectory = customTemplateDir ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SBInspector", "Templates");
        
        // Ensure directories exist
        EnsureDirectoryExists(_storageDirectory);
        EnsureDirectoryExists(_exportDirectory);
        EnsureDirectoryExists(_templateDirectory);
    }

    private void EnsureDirectoryExists(string directory)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public string GetExportDirectory() => _exportDirectory;
    public string GetTemplateDirectory() => _templateDirectory;

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

    public async Task RenameConnectionAsync(string oldName, string newName)
    {
        var connections = await GetSavedConnectionsAsync();
        var connection = connections.FirstOrDefault(c => c.Name.Equals(oldName, StringComparison.OrdinalIgnoreCase));
        
        if (connection != null)
        {
            connection.Name = newName;
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
            var filePath = Path.Combine(_templateDirectory, TemplatesFileName);
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
        
        var filePath = Path.Combine(_templateDirectory, TemplatesFileName);
        var json = JsonSerializer.Serialize(templates, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json);
    }

    public async Task DeleteMessageTemplateAsync(string id)
    {
        var templates = await GetMessageTemplatesAsync();
        templates.RemoveAll(t => t.Id == id);
        
        var filePath = Path.Combine(_templateDirectory, TemplatesFileName);
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
            var filePath = Path.Combine(_templateDirectory, TemplatesFileName);
            var json = JsonSerializer.Serialize(templates, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);
        }
    }
}
