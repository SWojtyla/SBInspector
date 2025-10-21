using SBInspector.Core.Domain;

namespace SBInspector.Core.Interfaces;

public interface IStorageService
{
    // Connection management
    Task<List<SavedConnection>> GetSavedConnectionsAsync();
    Task SaveConnectionAsync(SavedConnection connection);
    Task DeleteConnectionAsync(string name);
    Task UpdateLastUsedAsync(string name);

    // Template management
    Task<List<MessageTemplate>> GetMessageTemplatesAsync();
    Task SaveMessageTemplateAsync(MessageTemplate template);
    Task DeleteMessageTemplateAsync(string id);
    Task UpdateTemplateLastUsedAsync(string id);
}
