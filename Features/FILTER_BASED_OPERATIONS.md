# Filter-Based Message Operations

This feature extends the message filtering capabilities with powerful bulk operations for managing messages that match specific filter criteria.

## New Features

### 1. Not Contains Filter Operator

A new filter operator has been added to help exclude messages with specific attribute values.

#### How to Use

When setting up a filter on **Application Property**:

1. Select **Filter Field**: Application Property
2. Enter the **Attribute Name** (optional)
3. Select **Operator**: "Not Contains"
4. Enter the **Value** to exclude

#### Example Use Cases

- **Exclude test messages**: Filter out messages where `environment` does not contain "test"
- **Exclude specific customers**: Filter out messages where `customerId` does not contain "DEMO"
- **Exclude old formats**: Filter out messages where `messageVersion` does not contain "1.0"

#### Behavior

- Case-insensitive substring matching (like the Contains operator, but inverted)
- Can be combined with other filters using AND logic
- Works across all message attributes when Attribute Name is left empty

---

## 2. Delete Filtered Messages

Delete only the messages that match your current filter criteria, without affecting other messages in the queue or subscription.

### How to Use

1. Apply one or more filters to narrow down the messages you want to delete
2. Click the **"Delete Filtered"** button (yellow/warning color)
3. Review the confirmation message showing which messages will be deleted
4. Type **"DELETE"** to confirm the operation
5. The operation will:
   - Show progress as messages are deleted
   - Update the message list after completion
   - Refresh entity counts in the tree view
   - Can be cancelled at any time

### Safety Features

- **Confirmation Required**: You must type "DELETE" to confirm the operation
- **Filter Validation**: If no filters are applied, an error message suggests using "Purge All" instead
- **Progress Reporting**: Real-time count of deleted messages
- **Cancellable**: Stop the operation at any time using the Cancel button
- **Non-destructive to Filter**: Your filters remain active after deletion

### Use Cases

#### Delete High-Retry Messages
Find and remove messages that have been retried too many times:
```
Filter 1:
- Field: Delivery Count
- Operator: Greater Than or Equal
- Value: 5
```

#### Delete Old Messages
Remove messages older than a specific date:
```
Filter 1:
- Field: Enqueued Time
- Operator: Less Than
- Value: 2024-01-01
```

#### Delete Messages from Specific Source
Remove messages from a particular application or source:
```
Filter 1:
- Field: Application Property
- Attribute Name: source
- Operator: Equals
- Value: legacy-system
```

#### Complex Multi-Filter Deletion
Delete only messages that match multiple criteria:
```
Filter 1:
- Field: Application Property
- Attribute Name: status
- Operator: Equals
- Value: processed

Filter 2:
- Field: Enqueued Time
- Operator: Less Than
- Value: 2024-10-01

Filter 3:
- Field: Delivery Count
- Operator: Equals
- Value: 1
```

### Technical Details

The delete filtered operation:
1. **Peeks** at messages in batches of 100
2. **Applies filters** client-side to identify matching messages
3. **Receives and deletes** messages in ReceiveAndDelete mode
4. Only messages that pass all filters are deleted
5. Continues until no more messages match the filters
6. Reports progress every 100 messages

---

## 3. Export Filtered Messages

Download messages that match your current filters as a JSON file for backup, analysis, or auditing purposes.

### How to Use

1. Apply one or more filters to select the messages you want to export
2. Click the **"Export Filtered"** button (blue/info color)
3. The filtered messages will be downloaded as a JSON file
4. The filename will be: `{entity}_{messagetype}_messages_{timestamp}.json`

### Example Filenames

- `myqueue_active_messages_20241024_153045.json`
- `mytopic_mysubscription_deadletter_messages_20241024_153045.json`
- `testqueue_scheduled_messages_20241024_153045.json`

### Export Format

The exported JSON file contains an array of message objects with all properties:

```json
[
  {
    "messageId": "abc-123",
    "subject": "Order Created",
    "contentType": "application/json",
    "body": "{\"orderId\":\"12345\"}",
    "enqueuedTime": "2024-10-24T15:30:00Z",
    "scheduledEnqueueTime": null,
    "sequenceNumber": 12345,
    "deliveryCount": 1,
    "state": "Active",
    "properties": {
      "customerId": "CUST001",
      "priority": "high"
    }
  }
]
```

### Use Cases

#### Backup Before Deletion
Export messages before deleting them for safety:
1. Apply filters to identify messages to delete
2. Click "Export Filtered" to backup the messages
3. Click "Delete Filtered" to remove them

#### Message Analysis
Export messages for offline analysis:
- Export failed messages for debugging
- Export high-priority messages for review
- Export messages with specific attributes for audit trails

#### Data Migration
Export messages in one format for import into another system:
1. Filter messages by date range or criteria
2. Export to JSON
3. Transform and import into target system

#### Compliance and Auditing
Export messages for compliance reporting:
- Export all messages from a specific date range
- Export messages for specific customers
- Export messages with specific transaction types

### Features

- **Formatted JSON**: Indented and human-readable
- **Complete Data**: Includes all message properties and metadata
- **Automatic Download**: File downloads directly to your browser's download folder
- **No Server Storage**: File is created client-side in the browser
- **Works with Any Filter**: Export supports all filter types and combinations

---

## Filter Combinations

All three features work seamlessly with the existing filter system, supporting:

- **Multiple Filters**: Combine multiple filters with AND logic
- **All Filter Types**: Application Property, Enqueued Time, Delivery Count, Sequence Number
- **All Operators**: Contains, Not Contains, Equals, Not Equals, Greater Than, Less Than, Regex, etc.

### Example Workflow

1. **Identify**: Apply filters to find problematic messages
   - Example: Messages with delivery count > 3 from a specific source
2. **Backup**: Export filtered messages for audit trail
3. **Clean**: Delete filtered messages to clear the queue
4. **Verify**: Refresh to confirm deletion

---

## Tips and Best Practices

### For Delete Filtered

1. **Test Filters First**: Apply filters and verify the message count before deleting
2. **Export Before Delete**: Always export messages before deleting them (if needed for audit)
3. **Start Small**: Test with narrow filters first before running large deletions
4. **Monitor Progress**: Watch the progress counter to ensure operation is proceeding
5. **Check Results**: Refresh messages after deletion to verify correct messages were removed

### For Export Filtered

1. **Use Descriptive Filters**: Set up filters that clearly identify what you're exporting
2. **Check Message Count**: Verify the filtered count matches expectations before exporting
3. **Organize Exports**: Use consistent naming by entity and timestamp
4. **Large Exports**: For very large exports, consider filtering by date ranges to create smaller files

### For Not Contains Filter

1. **Combine with Other Operators**: Use Not Contains with Equals, Greater Than, etc. for precise filtering
2. **Test Pattern**: Verify your Not Contains pattern matches your expectations
3. **Case Sensitivity**: Remember that Not Contains is case-insensitive

---

## Limitations

1. **Client-Side Filtering**: Messages must be loaded into the browser before filtering applies
2. **Page Size Limit**: Delete and export operations work on messages within the current page size
3. **Performance**: Large batch operations may take time depending on message count
4. **Browser Limits**: Very large JSON exports (thousands of messages) may impact browser performance

---

## Troubleshooting

### "No filters applied" Error
- **Cause**: Clicked "Delete Filtered" without setting up any filters
- **Solution**: Apply at least one filter, or use "Purge All" to delete all messages

### "No messages matched the filters" Error
- **Cause**: The filters are too restrictive or there are no matching messages
- **Solution**: Adjust filters or verify messages exist in the queue/subscription

### Export Downloads Empty File
- **Cause**: No messages matched the current filters
- **Solution**: Check filter criteria and message list before exporting

### Delete Operation Seems Slow
- **Cause**: Large number of matching messages
- **Solution**: This is normal; monitor progress counter for status

---

## See Also

- [FILTERING.md](FILTERING.md) - Complete guide to message filtering
- [OPERATOR_FILTERING.md](OPERATOR_FILTERING.md) - Details on all filter operators
- [MESSAGE_CRUD.md](MESSAGE_CRUD.md) - Other message operations (single delete, requeue, etc.)
