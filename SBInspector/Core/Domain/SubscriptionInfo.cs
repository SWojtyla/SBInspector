namespace SBInspector.Core.Domain;

public class SubscriptionInfo
{
    public string Name { get; set; } = string.Empty;
    public long ActiveMessageCount { get; set; }
    public long ScheduledMessageCount { get; set; }
    public long DeadLetterMessageCount { get; set; }
}
