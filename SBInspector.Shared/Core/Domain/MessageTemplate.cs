namespace SBInspector.Shared.Core.Domain;

public class MessageTemplate
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string MessageBody { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public string? ContentType { get; set; }
    public Dictionary<string, object>? Properties { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
}
