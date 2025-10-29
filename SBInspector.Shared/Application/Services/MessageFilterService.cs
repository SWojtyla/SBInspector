using System.Globalization;
using System.Text.RegularExpressions;
using SBInspector.Shared.Core.Domain;

namespace SBInspector.Shared.Application.Services;

public class MessageFilterService
{
    public IEnumerable<MessageInfo> ApplyFilters(IEnumerable<MessageInfo> messages, List<MessageFilter> filters)
    {
        if (filters == null || !filters.Any(f => f.IsEnabled && (!string.IsNullOrWhiteSpace(f.AttributeName) || !string.IsNullOrWhiteSpace(f.AttributeValue))))
        {
            return messages;
        }

        return messages.Where(message => filters.All(filter => !filter.IsEnabled || MatchesFilter(message, filter)));
    }

    private bool MatchesFilter(MessageInfo message, MessageFilter filter)
    {
        // Skip empty filters
        if (string.IsNullOrWhiteSpace(filter.AttributeName) && string.IsNullOrWhiteSpace(filter.AttributeValue))
        {
            return true;
        }

        // Handle different filter fields
        switch (filter.Field)
        {
            case FilterField.EnqueuedTime:
                return MatchesDateTimeField(message.EnqueuedTime, filter);
            
            case FilterField.DeliveryCount:
                return MatchesNumericField(message.DeliveryCount, filter);
            
            case FilterField.SequenceNumber:
                return MatchesNumericField(message.SequenceNumber, filter);
            
            case FilterField.ApplicationProperty:
            default:
                return MatchesApplicationProperty(message, filter);
        }
    }

    private bool MatchesApplicationProperty(MessageInfo message, MessageFilter filter)
    {
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

            return MatchesValue(attributeValue?.ToString() ?? string.Empty, filter.AttributeValue, filter.Operator);
        }
        else if (!string.IsNullOrWhiteSpace(filter.AttributeValue))
        {
            // Only value specified, search across all attributes
            foreach (var prop in message.Properties)
            {
                var valueStr = prop.Value?.ToString() ?? string.Empty;
                if (MatchesValue(valueStr, filter.AttributeValue, filter.Operator))
                {
                    return true;
                }
            }
            return false;
        }

        return true;
    }

    private bool MatchesDateTimeField(DateTime fieldValue, MessageFilter filter)
    {
        if (string.IsNullOrWhiteSpace(filter.AttributeValue))
        {
            return true;
        }

        if (!DateTime.TryParse(filter.AttributeValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out var filterDateTime))
        {
            return false;
        }

        return filter.Operator switch
        {
            FilterOperator.Equals => fieldValue.Date == filterDateTime.Date,
            FilterOperator.NotEquals => fieldValue.Date != filterDateTime.Date,
            FilterOperator.GreaterThan => fieldValue > filterDateTime,
            FilterOperator.LessThan => fieldValue < filterDateTime,
            FilterOperator.GreaterThanOrEqual => fieldValue >= filterDateTime,
            FilterOperator.LessThanOrEqual => fieldValue <= filterDateTime,
            _ => false
        };
    }

    private bool MatchesNumericField(long fieldValue, MessageFilter filter)
    {
        if (string.IsNullOrWhiteSpace(filter.AttributeValue))
        {
            return true;
        }

        if (!long.TryParse(filter.AttributeValue, out var filterNumeric))
        {
            return false;
        }

        return filter.Operator switch
        {
            FilterOperator.Equals => fieldValue == filterNumeric,
            FilterOperator.NotEquals => fieldValue != filterNumeric,
            FilterOperator.GreaterThan => fieldValue > filterNumeric,
            FilterOperator.LessThan => fieldValue < filterNumeric,
            FilterOperator.GreaterThanOrEqual => fieldValue >= filterNumeric,
            FilterOperator.LessThanOrEqual => fieldValue <= filterNumeric,
            _ => false
        };
    }

    private bool MatchesValue(string value, string filterValue, FilterOperator op)
    {
        return op switch
        {
            FilterOperator.Contains => value.Contains(filterValue, StringComparison.OrdinalIgnoreCase),
            FilterOperator.NotContains => !value.Contains(filterValue, StringComparison.OrdinalIgnoreCase),
            FilterOperator.Equals => value.Equals(filterValue, StringComparison.OrdinalIgnoreCase),
            FilterOperator.NotEquals => !value.Equals(filterValue, StringComparison.OrdinalIgnoreCase),
            FilterOperator.Regex => MatchesRegex(value, filterValue),
            // For strings, numeric comparisons try to parse as numbers first
            FilterOperator.GreaterThan => TryNumericComparison(value, filterValue, (a, b) => a > b),
            FilterOperator.LessThan => TryNumericComparison(value, filterValue, (a, b) => a < b),
            FilterOperator.GreaterThanOrEqual => TryNumericComparison(value, filterValue, (a, b) => a >= b),
            FilterOperator.LessThanOrEqual => TryNumericComparison(value, filterValue, (a, b) => a <= b),
            _ => false
        };
    }

    private bool MatchesRegex(string value, string pattern)
    {
        try
        {
            return Regex.IsMatch(value, pattern);
        }
        catch
        {
            // Invalid regex, fall back to literal match
            return value.Contains(pattern, StringComparison.OrdinalIgnoreCase);
        }
    }

    private bool TryNumericComparison(string value, string filterValue, Func<double, double, bool> comparison)
    {
        if (double.TryParse(value, out var numValue) && double.TryParse(filterValue, out var numFilter))
        {
            return comparison(numValue, numFilter);
        }
        return false;
    }
}
