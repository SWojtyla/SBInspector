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

            return MatchesValueWithOperator(attributeValue?.ToString() ?? string.Empty, filter.AttributeValue, filter.Operator, filter.UseRegex);
        }
        else if (!string.IsNullOrWhiteSpace(filter.AttributeValue))
        {
            // Only value specified, search across all attributes
            foreach (var prop in message.Properties)
            {
                var valueStr = prop.Value?.ToString() ?? string.Empty;
                if (MatchesValueWithOperator(valueStr, filter.AttributeValue, filter.Operator, filter.UseRegex))
                {
                    return true;
                }
            }
            return false;
        }

        return true;
    }

    private bool MatchesValueWithOperator(string value, string filterValue, FilterOperator op, bool useRegexLegacy)
    {
        // Handle legacy UseRegex flag
        if (useRegexLegacy && op != FilterOperator.Regex)
        {
            op = FilterOperator.Regex;
        }

        switch (op)
        {
            case FilterOperator.Equals:
                return string.Equals(value, filterValue, StringComparison.OrdinalIgnoreCase);

            case FilterOperator.NotEquals:
                return !string.Equals(value, filterValue, StringComparison.OrdinalIgnoreCase);

            case FilterOperator.Contains:
                return value.Contains(filterValue, StringComparison.OrdinalIgnoreCase);

            case FilterOperator.NotContains:
                return !value.Contains(filterValue, StringComparison.OrdinalIgnoreCase);

            case FilterOperator.StartsWith:
                return value.StartsWith(filterValue, StringComparison.OrdinalIgnoreCase);

            case FilterOperator.EndsWith:
                return value.EndsWith(filterValue, StringComparison.OrdinalIgnoreCase);

            case FilterOperator.LessThan:
                return CompareValues(value, filterValue, result => result < 0);

            case FilterOperator.LessThanOrEqual:
                return CompareValues(value, filterValue, result => result <= 0);

            case FilterOperator.GreaterThan:
                return CompareValues(value, filterValue, result => result > 0);

            case FilterOperator.GreaterThanOrEqual:
                return CompareValues(value, filterValue, result => result >= 0);

            case FilterOperator.Before:
                return CompareDates(value, filterValue, (a, b) => a < b);

            case FilterOperator.After:
                return CompareDates(value, filterValue, (a, b) => a > b);

            case FilterOperator.Regex:
                try
                {
                    return System.Text.RegularExpressions.Regex.IsMatch(value, filterValue);
                }
                catch
                {
                    // Invalid regex, fall back to contains
                    return value.Contains(filterValue, StringComparison.OrdinalIgnoreCase);
                }

            default:
                return value.Contains(filterValue, StringComparison.OrdinalIgnoreCase);
        }
    }

    private bool CompareValues(string value, string filterValue, Func<int, bool> comparison)
    {
        // Try numeric comparison first
        if (decimal.TryParse(value, out var numValue) && decimal.TryParse(filterValue, out var numFilterValue))
        {
            return comparison(numValue.CompareTo(numFilterValue));
        }

        // Try datetime comparison
        if (DateTime.TryParse(value, out var dateValue) && DateTime.TryParse(filterValue, out var dateFilterValue))
        {
            return comparison(dateValue.CompareTo(dateFilterValue));
        }

        // Fall back to string comparison
        return comparison(string.Compare(value, filterValue, StringComparison.OrdinalIgnoreCase));
    }

    private bool CompareDates(string value, string filterValue, Func<DateTime, DateTime, bool> comparison)
    {
        if (DateTime.TryParse(value, out var dateValue) && DateTime.TryParse(filterValue, out var dateFilterValue))
        {
            return comparison(dateValue, dateFilterValue);
        }

        // If not valid dates, always return false
        return false;
    }
}
