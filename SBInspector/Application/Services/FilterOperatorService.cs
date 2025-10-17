using SBInspector.Core.Domain;

namespace SBInspector.Application.Services;

public class FilterOperatorService
{
    public string GetOperatorDisplayName(FilterOperator op)
    {
        return op switch
        {
            FilterOperator.Equals => "= (Equals)",
            FilterOperator.NotEquals => "≠ (Not Equals)",
            FilterOperator.Contains => "Contains",
            FilterOperator.NotContains => "Not Contains",
            FilterOperator.StartsWith => "Starts With",
            FilterOperator.EndsWith => "Ends With",
            FilterOperator.LessThan => "< (Less Than)",
            FilterOperator.LessThanOrEqual => "≤ (Less Than or Equal)",
            FilterOperator.GreaterThan => "> (Greater Than)",
            FilterOperator.GreaterThanOrEqual => "≥ (Greater Than or Equal)",
            FilterOperator.Before => "Before (DateTime)",
            FilterOperator.After => "After (DateTime)",
            FilterOperator.Regex => "Regex Pattern",
            _ => "Contains"
        };
    }

    public IEnumerable<FilterOperator> GetAllOperators()
    {
        return Enum.GetValues<FilterOperator>();
    }
}
