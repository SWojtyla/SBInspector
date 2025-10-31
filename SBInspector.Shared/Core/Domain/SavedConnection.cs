namespace SBInspector.Shared.Core.Domain;

public class SavedConnection
{
    public string Name { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public bool IsEncrypted { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
}
