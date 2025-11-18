using SBInspector.Shared.Core.Domain;

namespace SBInspector.Shared.Application.Services;

public static class NServiceBusMessageHelper
{
    private static readonly string[] NServiceBusPropertyPrefixes = new[]
    {
        "NServiceBus.",
        "$.diagnostics."  // NServiceBus diagnostic headers
    };

    public static bool IsNServiceBusMessage(MessageInfo message)
    {
        if (message?.Properties == null || !message.Properties.Any())
        {
            return false;
        }

        return message.Properties.Keys.Any(key => 
            NServiceBusPropertyPrefixes.Any(prefix => 
                key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)));
    }

    public static Dictionary<string, object> GetNServiceBusProperties(MessageInfo message)
    {
        if (message?.Properties == null)
        {
            return new Dictionary<string, object>();
        }

        return message.Properties
            .Where(kvp => NServiceBusPropertyPrefixes.Any(prefix => 
                kvp.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    public static object? GetNServiceBusProperty(MessageInfo message, string propertyName)
    {
        if (message?.Properties == null)
        {
            return null;
        }

        message.Properties.TryGetValue(propertyName, out var value);
        return value;
    }
}
