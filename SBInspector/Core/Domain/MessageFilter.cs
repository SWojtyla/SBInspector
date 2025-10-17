namespace SBInspector.Core.Domain;

public class MessageFilter
{
    public string AttributeName { get; set; } = string.Empty;
    public FilterOperator Operator { get; set; } = FilterOperator.Contains;
    public string AttributeValue { get; set; } = string.Empty;
    
    // Keep UseRegex for backward compatibility, but deprecate it in favor of Operator.Regex
    [Obsolete("Use Operator property with FilterOperator.Regex instead")]
    public bool UseRegex { get; set; } = false;
}
