using SBInspector.Shared.Core.Domain;

namespace SBInspector.Shared.Core.Interfaces;

public interface IStorageService
{
    // Connection management
    Task<List<SavedConnection>> GetSavedConnectionsAsync();
    Task SaveConnectionAsync(SavedConnection connection);
    Task DeleteConnectionAsync(string name);
    Task UpdateLastUsedAsync(string name);
    Task RenameConnectionAsync(string oldName, string newName);

    // Template management
    Task<List<MessageTemplate>> GetMessageTemplatesAsync();
    Task SaveMessageTemplateAsync(MessageTemplate template);
    Task DeleteMessageTemplateAsync(string id);
    Task UpdateTemplateLastUsedAsync(string id);

    // User preferences
    Task<UserPreferences> GetUserPreferencesAsync();
    Task SaveUserPreferencesAsync(UserPreferences preferences);
}
