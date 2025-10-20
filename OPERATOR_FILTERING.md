# Operator-Based Message Filtering Implementation

## Overview

This document describes the implementation of advanced operator-based filtering for Azure Service Bus messages. The new filtering system extends the previous attribute-only filtering with support for multiple filter fields and comparison operators.

## What Was Implemented

### 1. Filter Fields (FilterField Enum)
Messages can now be filtered on:
- **Application Property**: Custom message attributes (original functionality, enhanced with operators)
- **Enqueued Time**: When the message was enqueued to Service Bus
- **Delivery Count**: Number of times the message has been delivered
- **Sequence Number**: Unique identifier assigned by Service Bus

### 2. Filter Operators (FilterOperator Enum)
The following operators are now supported:

#### For String Fields
- **Contains**: Case-insensitive substring match (default)
- **Equals**: Exact case-insensitive match
- **Not Equals**: Does not match (case-insensitive)
- **Regex**: Regular expression pattern matching

#### For Numeric Fields (Delivery Count, Sequence Number)
- **Equals**: Exact numeric match
- **Not Equals**: Does not equal
- **Greater Than**: Value > specified number
- **Less Than**: Value < specified number
- **Greater Than or Equal**: Value >= specified number
- **Less Than or Equal**: Value <= specified number

#### For Date/Time Fields (Enqueued Time)
- **Equals**: Same date (date only, ignores time)
- **Not Equals**: Different date
- **Greater Than**: After specified date/time
- **Less Than**: Before specified date/time
- **Greater Than or Equal**: On or after specified date/time
- **Less Than or Equal**: On or before specified date/time

### 3. Code Changes

#### Domain Model (`MessageFilter.cs`)
```csharp
public enum FilterOperator
{
    Contains, Equals, NotEquals, GreaterThan, LessThan, 
    GreaterThanOrEqual, LessThanOrEqual, Regex
}

public enum FilterField
{
    ApplicationProperty, EnqueuedTime, DeliveryCount, SequenceNumber
}

public class MessageFilter
{
    public FilterField Field { get; set; } = FilterField.ApplicationProperty;
    public string AttributeName { get; set; } = string.Empty;
    public string AttributeValue { get; set; } = string.Empty;
    public FilterOperator Operator { get; set; } = FilterOperator.Contains;
}
```

#### Service Layer (`MessageFilterService.cs`)
Enhanced the filtering service with:
- Field-specific filtering methods:
  - `MatchesDateTimeField()`: Handles date/time comparisons
  - `MatchesNumericField()`: Handles numeric comparisons (long/int)
  - `MatchesApplicationProperty()`: Handles custom attribute filtering
  - `MatchesValue()`: Handles string value comparisons with operators
- Support for numeric comparison on string properties
- Proper date/time parsing with culture-invariant format
- Fallback mechanisms for invalid regex patterns

#### UI Component (`MessageFilterPanel.razor`)
Completely redesigned the filter panel with:
- **Filter Field Dropdown**: Select which field to filter on
- **Attribute Name Input**: Only shown for Application Property filters
- **Operator Dropdown**: Context-aware operators based on selected field
- **Value Input**: Type changes based on field (text, number, datetime-local)
- Dynamic placeholder text based on field and operator
- Responsive layout that adapts to filter field selection

## Testing Results

All tests passed successfully:

### Test Cases Executed
1. ✅ **Delivery Count > 0**: Correctly filtered messages with delivery count greater than 0
2. ✅ **Enqueued Time > Date**: Correctly filtered messages enqueued after a specific date
3. ✅ **Sequence Number = Value**: Correctly filtered exact sequence number match
4. ✅ **Application Property Contains**: Original functionality preserved
5. ✅ **Multiple Filters (AND logic)**: Correctly applied multiple filters with AND logic
6. ✅ **Numeric Comparison on Properties**: Successfully compared numeric values in application properties

## Key Features

### Backward Compatibility
- The old `UseRegex` property is maintained for backward compatibility (marked as obsolete)
- Existing filters using `UseRegex=true` automatically map to `Operator=Regex`
- Default behavior (Contains) remains unchanged for application properties

### User Experience Improvements
- **Dynamic UI**: Filter options change based on selected field type
- **Input Validation**: Appropriate input types (text, number, datetime) based on field
- **Clear Visual Feedback**: Shows "X of Y messages" count
- **Flexible Filtering**: Can filter on built-in fields or custom properties
- **Multiple Filters**: Support for complex queries using AND logic

### Examples from Testing

```csharp
// Example 1: Find messages with delivery count > 0
var filter = new MessageFilter 
{ 
    Field = FilterField.DeliveryCount, 
    Operator = FilterOperator.GreaterThan, 
    AttributeValue = "0" 
};

// Example 2: Find messages enqueued after Jan 10, 2024
var filter = new MessageFilter 
{ 
    Field = FilterField.EnqueuedTime, 
    Operator = FilterOperator.GreaterThan, 
    AttributeValue = "2024-01-10" 
};

// Example 3: Find messages with sequence number = 200
var filter = new MessageFilter 
{ 
    Field = FilterField.SequenceNumber, 
    Operator = FilterOperator.Equals, 
    AttributeValue = "200" 
};

// Example 4: Multiple filters (AND logic)
var filters = new List<MessageFilter>
{
    new MessageFilter 
    { 
        Field = FilterField.DeliveryCount, 
        Operator = FilterOperator.GreaterThanOrEqual, 
        AttributeValue = "1" 
    },
    new MessageFilter 
    { 
        Field = FilterField.SequenceNumber, 
        Operator = FilterOperator.LessThan, 
        AttributeValue = "300" 
    }
};
// Returns only messages matching BOTH conditions
```

## Documentation

Updated `FILTERING.md` with:
- Complete operator reference
- Field type descriptions
- Usage examples for each operator type
- Multiple filter scenarios
- Technical details about filtering behavior

## Benefits

1. **Enhanced Query Capability**: Users can now filter on system fields (time, counts) in addition to custom properties
2. **Precise Filtering**: Operators like >, <, =, != allow for exact queries
3. **Time-based Filtering**: Find messages in specific time ranges (useful for debugging)
4. **Retry Analysis**: Easily identify messages with high delivery counts
5. **Sequence Tracking**: Filter messages by their Service Bus sequence numbers
6. **Flexible Property Filtering**: Numeric comparison support for custom properties

## Technical Notes

- All filtering is client-side (on loaded messages)
- Date parsing uses `CultureInfo.InvariantCulture` for consistency
- Invalid numeric/date values cause filter to skip that message
- String comparisons are case-insensitive (except regex)
- Regex errors automatically fall back to substring matching
- Filter evaluation uses short-circuit AND logic for efficiency
