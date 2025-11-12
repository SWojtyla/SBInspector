namespace SBInspector.Shared.Core.Domain;

public class ColumnConfiguration
{
    public List<ColumnDefinition> Columns { get; set; } = new();
}

public class ColumnDefinition
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsVisible { get; set; } = true;
    public ColumnType Type { get; set; }
    public string? PropertyKey { get; set; } // For custom property columns
    public int Order { get; set; }
    public bool IsSystemColumn { get; set; } // Cannot be deleted, only hidden
}

public enum ColumnType
{
    MessageId,
    Subject,
    ContentType,
    EnqueuedTime,
    ScheduledEnqueueTime,
    SequenceNumber,
    DeliveryCount,
    State,
    OriginatingEndpoint,
    CustomProperty
}
