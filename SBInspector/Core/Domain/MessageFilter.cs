namespace SBInspector.Core.Domain;

public class MessageFilter
{
    public string AttributeName { get; set; } = string.Empty;
    public string AttributeValue { get; set; } = string.Empty;
    public bool UseRegex { get; set; } = false;
}
