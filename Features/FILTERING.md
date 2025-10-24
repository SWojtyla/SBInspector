# Message Filtering

This feature allows you to filter Service Bus messages using multiple criteria and operators. You can filter both on message attributes (application properties) and on built-in message fields like enqueued time, delivery count, and sequence number.

> **New Features!** You can now perform bulk operations on filtered messages:
> - **Delete Filtered Messages**: Delete all messages that match your current filters
> - **Export Filtered Messages**: Download filtered messages as JSON for backup or analysis
> - See [FILTER_BASED_OPERATIONS.md](FILTER_BASED_OPERATIONS.md) for complete documentation

## Filter Fields

You can filter messages by:

### 1. Application Property (Custom Attributes)
Filter on custom application properties/attributes that are attached to messages. You can search by attribute name and/or value.

### 2. Enqueued Time
Filter messages based on when they were enqueued to the Service Bus. Useful for finding messages within specific time ranges.

### 3. Delivery Count
Filter messages based on how many times they've been delivered. Useful for finding messages that have been retried multiple times or are stuck.

### 4. Sequence Number
Filter messages based on their sequence number, which is a unique identifier assigned by Service Bus.

## Filter Operators

Depending on the field type, different operators are available:

### For String Fields (Application Properties with Contains/Equals/NotEquals)
- **Contains**: Case-insensitive substring match (default for application properties)
- **Not Contains**: Case-insensitive substring match - excludes messages that contain the value
- **Equals**: Exact match (case-insensitive)
- **Not Equals**: Does not match (case-insensitive)
- **Regex**: Regular expression pattern matching

### For Numeric Fields (Delivery Count, Sequence Number)
- **Equals**: Exact numeric match
- **Not Equals**: Does not equal
- **Greater Than**: Value is greater than the specified number
- **Less Than**: Value is less than the specified number
- **Greater Than or Equal**: Value is greater than or equal to the specified number
- **Less Than or Equal**: Value is less than or equal to the specified number

### For Date/Time Fields (Enqueued Time)
- **Equals**: Same date (ignores time component)
- **Not Equals**: Different date
- **Greater Than**: After the specified date/time
- **Less Than**: Before the specified date/time
- **Greater Than or Equal**: On or after the specified date/time
- **Less Than or Equal**: On or before the specified date/time

### For Application Properties (Numeric Comparison)
When filtering application properties with numeric operators (>, <, >=, <=), the system will attempt to parse both the property value and filter value as numbers. If both can be parsed as numbers, a numeric comparison is performed.

## How to Use

When viewing messages from a queue or topic subscription, you will see a "Filter Messages" section. Each filter has the following fields:

### 1. Filter Field (Required)
Select what you want to filter on:
- **Application Property**: Filter on custom message attributes
- **Enqueued Time**: Filter on when the message was enqueued
- **Delivery Count**: Filter on retry count
- **Sequence Number**: Filter on the message sequence number

### 2. Attribute Name (Only for Application Property)
When filtering on application properties, optionally specify the exact name of the attribute. If left empty, the filter will search across all attributes.

### 3. Operator (Required)
Select the comparison operator. Available operators depend on the selected field type.

### 4. Value (Required)
Enter the value to compare against. The input type changes based on the field:
- For **Enqueued Time**: Date/time picker
- For **Delivery Count** and **Sequence Number**: Numeric input
- For **Application Property**: Text input

## Multiple Filters

You can add multiple filters by clicking the "Add Filter" button. All filters are combined using AND logic:
- A message must match **all** filters to be displayed
- Empty filters (with no value) are ignored
- You can remove individual filters using the trash icon (except when only one filter remains)
- Use "Clear All" to reset all filters at once

## Examples

### Example 1: Messages Enqueued After a Specific Date
Find messages enqueued after January 15, 2024:
- Filter Field: **Enqueued Time**
- Operator: **Greater Than**
- Value: `2024-01-15 00:00`

### Example 2: Messages with High Delivery Count
Find messages that have been retried 3 or more times:
- Filter Field: **Delivery Count**
- Operator: **Greater Than or Equal**
- Value: `3`

### Example 3: Application Property with Specific Value
Find messages where the `priority` attribute is "high":
- Filter Field: **Application Property**
- Attribute Name: `priority`
- Operator: **Equals**
- Value: `high`

### Example 4: Application Property with Pattern Match
Find messages where `customerId` starts with "CUST" followed by 5 digits:
- Filter Field: **Application Property**
- Attribute Name: `customerId`
- Operator: **Regex**
- Value: `^CUST\d{5}$`

### Example 5: Numeric Comparison on Application Property
Find messages where the `amount` attribute is greater than 100:
- Filter Field: **Application Property**
- Attribute Name: `amount`
- Operator: **Greater Than**
- Value: `100`

### Example 6: Multiple Filters with AND Logic
Find messages enqueued in the last week with high priority:
1. First filter:
   - Filter Field: **Enqueued Time**
   - Operator: **Greater Than**
   - Value: `2024-01-15 00:00`
2. Click "Add Filter"
3. Second filter:
   - Filter Field: **Application Property**
   - Attribute Name: `priority`
   - Operator: **Equals**
   - Value: `high`

Result: Shows only messages that match BOTH conditions.

### Example 7: Range Filter Using Two Filters
Find messages with sequence numbers between 1000 and 2000:
1. First filter:
   - Filter Field: **Sequence Number**
   - Operator: **Greater Than or Equal**
   - Value: `1000`
2. Click "Add Filter"
3. Second filter:
   - Filter Field: **Sequence Number**
   - Operator: **Less Than or Equal**
   - Value: `2000`

## Filtering Behavior

### Date/Time Filtering
- When using **Equals** operator with dates, only the date portion is compared (time is ignored)
- Other operators (>, <, >=, <=) compare the full date and time
- Date format is flexible and will parse most standard formats

### Numeric Filtering
- All numeric values must be valid integers or decimals
- Invalid numeric values will cause the filter to skip the message

### Application Property Filtering
- **Attribute Name Only**: Shows all messages that have that attribute, regardless of value
- **Value Only**: Searches across all attributes and shows messages where any attribute matches
- **Both Name and Value**: Filters on the specific attribute with the specified value

## Technical Details

- Filtering is performed client-side on the messages already loaded (based on page size)
- The filter count shows "Showing X of Y messages" to indicate how many messages match all filters
- Invalid regex patterns automatically fall back to literal substring matching
- All string comparisons (except regex) are case-insensitive
- Filters are applied in the order they are defined, using AND logic
