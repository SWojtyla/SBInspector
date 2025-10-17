namespace SBInspector.Core.Domain;

public class EntityInfo
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Queue or Topic
    public long ActiveMessageCount { get; set; }
    public long ScheduledMessageCount { get; set; }
    public long DeadLetterMessageCount { get; set; }
}
