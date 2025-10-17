# Message Attribute Filtering

This feature allows you to filter Service Bus messages based on their application properties (message attributes).

## How to Use

When viewing messages from a queue or topic subscription, you will see a "Filter by Message Attributes" section with three input fields:

### 1. Attribute Name (Optional)
- Enter the exact name of the message attribute/property you want to filter on
- Example: `customerId`, `orderType`, `region`
- If left empty, the filter will search across all attributes

### 2. Attribute Value (Optional)
- Enter the value you want to search for
- When "Use Regex Expression" is unchecked, performs a case-insensitive substring match
- When "Use Regex Expression" is checked, treats the value as a regular expression pattern
- Example literal values: `12345`, `premium`, `US-East`
- Example regex patterns: `^[A-Z]{2}-`, `\d{5}`, `(premium|gold)`

### 3. Use Regex Expression (Checkbox)
- **Unchecked (default)**: Performs literal substring matching (case-insensitive)
- **Checked**: Treats the attribute value as a regular expression pattern

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

### Example 1: Literal Match (Case-Insensitive)
Filter messages where the `region` attribute contains "east":
- Attribute Name: `region`
- Attribute Value: `east`
- Use Regex: ☐ (unchecked)
- Matches: "East", "EAST", "east", "Northeast", etc.

### Example 2: Exact Match with Regex
Filter messages where the `orderType` is exactly "PREMIUM":
- Attribute Name: `orderType`
- Attribute Value: `^PREMIUM$`
- Use Regex: ☑ (checked)
- Matches: Only "PREMIUM" (exact match)

### Example 3: Pattern Match with Regex
Filter messages where `customerId` starts with "CUST" followed by 5 digits:
- Attribute Name: `customerId`
- Attribute Value: `^CUST\d{5}$`
- Use Regex: ☑ (checked)
- Matches: "CUST12345", "CUST99999", etc.

### Example 4: Multiple Options with Regex
Filter messages where `status` is either "pending" or "processing":
- Attribute Name: `status`
- Attribute Value: `^(pending|processing)$`
- Use Regex: ☑ (checked)
- Matches: "pending" or "processing"

## Notes

- Filtering is performed client-side on the messages already loaded (up to 100 messages)
- The filter count shows "Showing X of Y messages" to indicate how many messages match the filter
- Use the "Clear Filters" button to reset all filter fields
- If a regex pattern is invalid, the filter automatically falls back to literal substring matching
- All literal matches are case-insensitive for better usability
