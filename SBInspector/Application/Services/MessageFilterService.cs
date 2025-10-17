using System.Text.RegularExpressions;
using SBInspector.Core.Domain;

namespace SBInspector.Application.Services;

public class MessageFilterService
{
    public IEnumerable<MessageInfo> ApplyFilters(IEnumerable<MessageInfo> messages, List<MessageFilter> filters)
    {
        if (filters == null || !filters.Any(f => !string.IsNullOrWhiteSpace(f.AttributeName) || !string.IsNullOrWhiteSpace(f.AttributeValue)))
        {
            return messages;
        }

        return messages.Where(message => filters.All(filter => MatchesFilter(message, filter)));
    }

    private bool MatchesFilter(MessageInfo message, MessageFilter filter)
    {
        // Skip empty filters
        if (string.IsNullOrWhiteSpace(filter.AttributeName) && string.IsNullOrWhiteSpace(filter.AttributeValue))
        {
            return true;
        }

        // If attribute name is specified, find that specific attribute
        if (!string.IsNullOrWhiteSpace(filter.AttributeName))
        {
            if (!message.Properties.TryGetValue(filter.AttributeName, out var attributeValue))
            {
                return false; // Attribute doesn't exist in this message
            }

            // If no value filter specified, just check attribute existence
            if (string.IsNullOrWhiteSpace(filter.AttributeValue))
            {
                return true;
            }

            return MatchesValue(attributeValue?.ToString() ?? string.Empty, filter.AttributeValue, filter.UseRegex);
        }
        else if (!string.IsNullOrWhiteSpace(filter.AttributeValue))
        {
            // Only value specified, search across all attributes
            foreach (var prop in message.Properties)
            {
                var valueStr = prop.Value?.ToString() ?? string.Empty;
                if (MatchesValue(valueStr, filter.AttributeValue, filter.UseRegex))
                {
                    return true;
                }
            }
            return false;
        }

        return true;
    }

    private bool MatchesValue(string value, string filterValue, bool useRegex)
    {
        if (useRegex)
        {
            try
            {
                return Regex.IsMatch(value, filterValue);
            }
            catch
            {
                // Invalid regex, fall back to literal match
                return value.Contains(filterValue, StringComparison.OrdinalIgnoreCase);
            }
        }
        else
        {
            return value.Contains(filterValue, StringComparison.OrdinalIgnoreCase);
        }
    }
}
