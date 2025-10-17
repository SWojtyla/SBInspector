namespace SBInspector.Models;

public class MessageInfo
{
    public string MessageId { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime EnqueuedTime { get; set; }
    public DateTime? ScheduledEnqueueTime { get; set; }
    public long SequenceNumber { get; set; }
    public int DeliveryCount { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new();
    public string State { get; set; } = string.Empty; // Active, Scheduled, DeadLetter
}
