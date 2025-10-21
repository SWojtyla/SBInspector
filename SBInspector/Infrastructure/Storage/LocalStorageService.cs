using System.Text.Json;
using Microsoft.JSInterop;
using SBInspector.Core.Domain;
using SBInspector.Core.Interfaces;

namespace SBInspector.Infrastructure.Storage;

public class LocalStorageService : IStorageService
{
    private readonly IJSRuntime _jsRuntime;
    private const string ConnectionsKey = "sbinspector_connections";
    private const string TemplatesKey = "sbinspector_templates";

    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    // Connection management
    public async Task<List<SavedConnection>> GetSavedConnectionsAsync()
    {
        try
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", ConnectionsKey);
            if (string.IsNullOrEmpty(json))
            {
                return new List<SavedConnection>();
            }
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
        
        var json = JsonSerializer.Serialize(connections);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", ConnectionsKey, json);
    }

    public async Task DeleteConnectionAsync(string name)
    {
        var connections = await GetSavedConnectionsAsync();
        connections.RemoveAll(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        
        var json = JsonSerializer.Serialize(connections);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", ConnectionsKey, json);
    }

    public async Task UpdateLastUsedAsync(string name)
    {
        var connections = await GetSavedConnectionsAsync();
        var connection = connections.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        
        if (connection != null)
        {
            connection.LastUsedAt = DateTime.UtcNow;
            var json = JsonSerializer.Serialize(connections);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", ConnectionsKey, json);
        }
    }

    // Template management
    public async Task<List<MessageTemplate>> GetMessageTemplatesAsync()
    {
        try
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TemplatesKey);
            if (string.IsNullOrEmpty(json))
            {
                return new List<MessageTemplate>();
            }
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
        
        var json = JsonSerializer.Serialize(templates);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TemplatesKey, json);
    }

    public async Task DeleteMessageTemplateAsync(string id)
    {
        var templates = await GetMessageTemplatesAsync();
        templates.RemoveAll(t => t.Id == id);
        
        var json = JsonSerializer.Serialize(templates);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TemplatesKey, json);
    }

    public async Task UpdateTemplateLastUsedAsync(string id)
    {
        var templates = await GetMessageTemplatesAsync();
        var template = templates.FirstOrDefault(t => t.Id == id);
        
        if (template != null)
        {
            template.LastUsedAt = DateTime.UtcNow;
            var json = JsonSerializer.Serialize(templates);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TemplatesKey, json);
        }
    }
}
