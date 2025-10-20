namespace SBInspector.Core.Domain;

public enum FilterOperator
{
    Contains,           // For strings (default)
    Equals,            // For strings, numbers, dates
    NotEquals,         // For strings, numbers, dates
    GreaterThan,       // For numbers, dates
    LessThan,          // For numbers, dates
    GreaterThanOrEqual, // For numbers, dates
    LessThanOrEqual,   // For numbers, dates
    Regex              // For strings (regex pattern)
}

public enum FilterField
{
    ApplicationProperty, // Filter on application properties (custom attributes)
    EnqueuedTime,       // Filter on enqueued time
    DeliveryCount,      // Filter on delivery count
    SequenceNumber      // Filter on sequence number
}

public class MessageFilter
{
    public FilterField Field { get; set; } = FilterField.ApplicationProperty;
    public string AttributeName { get; set; } = string.Empty;
    public string AttributeValue { get; set; } = string.Empty;
    public FilterOperator Operator { get; set; } = FilterOperator.Contains;
    
    // For backward compatibility
    [Obsolete("Use Operator property with FilterOperator.Regex instead")]
    public bool UseRegex
    {
        get => Operator == FilterOperator.Regex;
        set => Operator = value ? FilterOperator.Regex : FilterOperator.Contains;
    }
}
