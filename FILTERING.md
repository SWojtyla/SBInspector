# Message Attribute Filtering

This feature allows you to filter Service Bus messages based on their application properties (message attributes). You can add multiple filters that work together using AND logic - messages must match all filters to be displayed.

## How to Use

When viewing messages from a queue or topic subscription, you will see a "Filter by Message Attributes" section. You can add multiple filters by clicking the "Add Filter" button. Each filter has three input fields:

### 1. Attribute Name (Optional)
- Enter the exact name of the message attribute/property you want to filter on
- Example: `customerId`, `orderType`, `region`, `TimeSent`
- If left empty, the filter will search across all attributes

### 2. Operator (Required)
- Select the comparison operator to use for filtering
- Available operators:
  - **Contains** (default): Case-insensitive substring match
  - **= (Equals)**: Exact match (case-insensitive)
  - **≠ (Not Equals)**: Value does not match (case-insensitive)
  - **Not Contains**: Value does not contain the substring
  - **Starts With**: Value starts with the specified string
  - **Ends With**: Value ends with the specified string
  - **< (Less Than)**: Numeric, date, or string comparison
  - **≤ (Less Than or Equal)**: Numeric, date, or string comparison
  - **> (Greater Than)**: Numeric, date, or string comparison
  - **≥ (Greater Than or Equal)**: Numeric, date, or string comparison
  - **Before (DateTime)**: Date/time comparison (value < filterValue)
  - **After (DateTime)**: Date/time comparison (value > filterValue)
  - **Regex Pattern**: Treats the value as a regular expression pattern

### 3. Value (Optional)
- Enter the value you want to compare against
- For text operators (Contains, Equals, etc.), enter text strings
- For numeric operators (<, >, etc.), enter numeric values - will auto-detect if comparison is numeric or date-based
- For date operators (Before, After), enter date/time values in a parseable format (e.g., `2025-10-09 14:23:53`)
- For regex operator, enter a valid regular expression pattern
- Example values: `12345`, `premium`, `2025-10-09 14:23:53`, `^[A-Z]{2}-`

## Multiple Filters

You can add multiple filters by clicking the "Add Filter" button. All filters are combined using AND logic:
- A message must match **all** filters to be displayed
- Empty filters (with no name or value) are ignored
- You can remove individual filters using the "Remove" button next to each filter (except the last one)
- Use "Clear All Filters" to reset all filters at once

### Example: Multiple Filters with AND Logic
To find messages where both `region` is "east" AND `customerType` is "premium":
1. First filter: Attribute Name: `region`, Attribute Value: `east`
2. Click "Add Filter"
3. Second filter: Attribute Name: `customerType`, Attribute Value: `premium`
Result: Shows only messages that have BOTH attributes matching the specified values.

## Filtering Behavior

### Attribute Name Only
If you specify only an attribute name (leaving value empty), the filter will show all messages that have that attribute, regardless of its value.

Example:
- Attribute Name: `customerId`
- Attribute Value: (empty)
- Result: Shows all messages that have a `customerId` attribute

### Attribute Value Only
If you specify only a value (leaving name empty), the filter will search across all attributes in each message and show messages where any attribute value matches.

Example:
- Attribute Name: (empty)
- Attribute Value: `premium`
- Result: Shows messages where any attribute contains "premium"

### Both Attribute Name and Value
If you specify both, the filter will look for the specific attribute and match its value.

Example:
- Attribute Name: `subscriptionType`
- Attribute Value: `premium`
- Result: Shows messages where the `subscriptionType` attribute contains "premium"

## Examples

### Example 1: Contains Match (Case-Insensitive)
Filter messages where the `region` attribute contains "east":
- Attribute Name: `region`
- Operator: `Contains`
- Value: `east`
- Matches: "East", "EAST", "east", "Northeast", etc.

### Example 2: Exact Match
Filter messages where the `orderType` is exactly "PREMIUM":
- Attribute Name: `orderType`
- Operator: `= (Equals)`
- Value: `PREMIUM`
- Matches: Only "PREMIUM" (case-insensitive)

### Example 3: Date Comparison
Filter messages sent after a specific date/time:
- Attribute Name: `TimeSent`
- Operator: `After (DateTime)`
- Value: `2025-10-09 14:23:53`
- Matches: Any message where TimeSent is after October 9, 2025 at 14:23:53

### Example 4: Numeric Comparison
Filter messages where the `quantity` is less than or equal to 100:
- Attribute Name: `quantity`
- Operator: `≤ (Less Than or Equal)`
- Value: `100`
- Matches: Any numeric value ≤ 100

### Example 5: Pattern Match with Regex
Filter messages where `customerId` starts with "CUST" followed by 5 digits:
- Attribute Name: `customerId`
- Operator: `Regex Pattern`
- Value: `^CUST\d{5}$`
- Matches: "CUST12345", "CUST99999", etc.

### Example 6: Multiple Options with Regex
Filter messages where `status` is either "pending" or "processing":
- Attribute Name: `status`
- Operator: `Regex Pattern`
- Value: `^(pending|processing)$`
- Matches: "pending" or "processing"

### Example 7: Not Contains
Filter messages where the `category` does not contain "test":
- Attribute Name: `category`
- Operator: `Not Contains`
- Value: `test`
- Matches: Any value that doesn't contain "test"

## Notes

- Filtering is performed client-side on the messages already loaded (up to 100 messages)
- The filter count shows "Showing X of Y messages" to indicate how many messages match the filter
- Use the "Clear All Filters" button to reset all filter fields
- If a regex pattern is invalid, the filter automatically falls back to literal substring matching (Contains)
- Most text-based operators (Contains, Equals, Starts With, Ends With) are case-insensitive for better usability
- Numeric and date comparisons automatically detect the type and perform appropriate comparisons
- Date comparisons support various date/time formats - the filter will parse the value to a DateTime before comparing
