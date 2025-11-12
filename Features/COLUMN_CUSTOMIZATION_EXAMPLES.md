# Column Customization Examples

This document provides practical examples of column configurations for different messaging scenarios and frameworks.

## NServiceBus Examples

### Basic NServiceBus Configuration

For a typical NServiceBus implementation, this configuration provides essential message tracking:

**Columns to Add:**
1. Message Intent - `NServiceBus.MessageIntent`
2. Originating Endpoint - (already default)
3. Message Types - `NServiceBus.EnclosedMessageTypes`

**Steps:**
1. Click Settings (⚙️) in the message table
2. Add custom column:
   - Property Key: `NServiceBus.MessageIntent`
   - Display Name: `Intent`
3. Add custom column:
   - Property Key: `NServiceBus.EnclosedMessageTypes`
   - Display Name: `Message Type`
4. Click Save

**Result Table:**
```
│ Message ID │ Intent  │ Message Type              │ Enqueued Time │ Delivery │
│ abc-123... │ Send    │ MyApp.Commands.ProcessOrder│ 2025-11-11... │    1     │
│ def-456... │ Publish │ MyApp.Events.OrderCreated  │ 2025-11-11... │    0     │
```

### Advanced NServiceBus Debugging

For debugging message flow and conversations:

**Columns to Add:**
1. Conversation ID - `NServiceBus.ConversationId`
2. Related To - `NServiceBus.RelatedTo`
3. Processing Endpoint - `NServiceBus.ProcessingEndpoint`
4. Time Sent - `NServiceBus.TimeSent`

**Configuration:**
```json
{
  "Columns": [
    { "DisplayName": "Message ID", "IsVisible": true },
    { "DisplayName": "Conversation ID", "PropertyKey": "NServiceBus.ConversationId" },
    { "DisplayName": "Related To", "PropertyKey": "NServiceBus.RelatedTo" },
    { "DisplayName": "Intent", "PropertyKey": "NServiceBus.MessageIntent" },
    { "DisplayName": "Time Sent", "PropertyKey": "NServiceBus.TimeSent" },
    { "DisplayName": "Delivery", "IsVisible": true }
  ]
}
```

**Result Table:**
```
│ Message ID │ Conversation ID │ Related To │ Intent │ Time Sent    │ Delivery │
│ abc-123... │ conv-001        │ (not set)  │ Send   │ 10:00:01.123 │    0     │
│ def-456... │ conv-001        │ abc-123... │ Reply  │ 10:00:02.456 │    0     │
│ ghi-789... │ conv-001        │ def-456... │ Reply  │ 10:00:03.789 │    1     │
```

### NServiceBus Saga Tracking

For tracking saga-related messages:

**Columns to Add:**
1. Saga Type - `NServiceBus.SagaType`
2. Saga ID - `NServiceBus.SagaId`
3. Originating Saga Type - `NServiceBus.OriginatingSagaType`
4. Originating Saga ID - `NServiceBus.OriginatingSagaId`

**Result Table:**
```
│ Message ID │ Saga Type     │ Saga ID   │ Originating Saga Type │ Delivery │
│ abc-123... │ OrderSaga     │ saga-001  │ CheckoutSaga          │    0     │
│ def-456... │ PaymentSaga   │ saga-002  │ OrderSaga             │    0     │
```

## MassTransit Examples

### Basic MassTransit Configuration

**Columns to Add:**
1. Source Address - `MT-SourceAddress`
2. Destination Address - `MT-DestinationAddress`
3. Message Type - `MT-MessageType`
4. Request ID - `MT-RequestId`

**Result Table:**
```
│ Message ID │ Source Address     │ Message Type    │ Request ID │ Delivery │
│ abc-123... │ rabbitmq://local/q │ OrderCommand    │ req-001    │    0     │
│ def-456... │ rabbitmq://local/q │ OrderCompleted  │ req-001    │    0     │
```

### MassTransit Retry Tracking

**Columns to Add:**
1. Retry Count - `MT-Redelivery-Count`
2. Fault Message - `MT-Fault-Message`
3. Exception Type - `MT-Exception-Type`

**Result Table:**
```
│ Message ID │ Retry Count │ Exception Type      │ Fault Message        │ Delivery │
│ abc-123... │ 2           │ TimeoutException    │ Database timeout     │    3     │
│ def-456... │ 0           │ (not set)           │ (not set)            │    0     │
```

## Azure Service Bus Native Examples

### Basic Azure Service Bus

**Columns to Add:**
1. Session ID - `SessionId`
2. Partition Key - `PartitionKey`
3. Reply To - `ReplyTo`
4. Reply To Session ID - `ReplyToSessionId`

**Result Table:**
```
│ Message ID │ Session ID │ Partition Key │ Reply To      │ Enqueued Time │
│ abc-123... │ session-01 │ partition-A   │ response-queue│ 2025-11-11... │
│ def-456... │ session-02 │ partition-B   │ (not set)     │ 2025-11-11... │
```

### Azure Service Bus Correlation

**Columns to Add:**
1. Correlation ID - `CorrelationId`
2. Label - `Label`
3. To - `To`

**Result Table:**
```
│ Message ID │ Correlation ID │ Label        │ To              │ Delivery │
│ abc-123... │ corr-001       │ OrderProcess │ processing-queue│    0     │
│ def-456... │ corr-001       │ OrderUpdate  │ update-queue    │    0     │
```

## Custom Application Examples

### E-Commerce Order Processing

**Columns to Add:**
1. Order ID - `OrderId`
2. Customer ID - `CustomerId`
3. Order Status - `OrderStatus`
4. Priority - `Priority`

**Result Table:**
```
│ Message ID │ Order ID │ Customer ID │ Order Status │ Priority │ Delivery │
│ abc-123... │ ORD-001  │ CUST-1234   │ Pending      │ High     │    0     │
│ def-456... │ ORD-002  │ CUST-5678   │ Processing   │ Normal   │    0     │
```

### Payment Processing

**Columns to Add:**
1. Transaction ID - `TransactionId`
2. Payment Method - `PaymentMethod`
3. Amount - `Amount`
4. Currency - `Currency`

**Result Table:**
```
│ Message ID │ Transaction ID │ Payment Method │ Amount │ Currency │ Delivery │
│ abc-123... │ TXN-001        │ CreditCard     │ 100.00 │ USD      │    0     │
│ def-456... │ TXN-002        │ PayPal         │ 250.50 │ EUR      │    0     │
```

### Event Sourcing

**Columns to Add:**
1. Aggregate ID - `AggregateId`
2. Aggregate Type - `AggregateType`
3. Event Version - `EventVersion`
4. Event Type - `EventType`

**Result Table:**
```
│ Message ID │ Aggregate ID │ Event Type       │ Version │ Enqueued Time │
│ abc-123... │ AGG-001      │ OrderCreated     │ 1       │ 2025-11-11... │
│ def-456... │ AGG-001      │ OrderUpdated     │ 2       │ 2025-11-11... │
│ ghi-789... │ AGG-001      │ OrderCompleted   │ 3       │ 2025-11-11... │
```

## Performance Monitoring Examples

### Message Processing Metrics

**Columns to Add:**
1. Processing Time MS - `ProcessingTimeMs`
2. Queue Time MS - `QueueTimeMs`
3. Handler Name - `HandlerName`

**Result Table:**
```
│ Message ID │ Handler Name      │ Queue Time │ Processing Time │ Delivery │
│ abc-123... │ OrderHandler      │ 150ms      │ 45ms            │    0     │
│ def-456... │ PaymentHandler    │ 2300ms     │ 890ms           │    2     │
```

## Troubleshooting Examples

### Dead Letter Queue Analysis

When analyzing dead-letter messages, show:

**Columns to Add:**
1. Dead Letter Reason - `DeadLetterReason`
2. Dead Letter Error Description - `DeadLetterErrorDescription`
3. Exception Message - `ExceptionMessage`

**Hide:**
- Originating Endpoint (to make room)
- Subject (usually not relevant for DLQ)

**Result Table:**
```
│ Message ID │ Dead Letter Reason    │ Error Description      │ Delivery │
│ abc-123... │ MaxDeliveryCountExceeded│ Retry limit exceeded │    10    │
│ def-456... │ MessageExpired        │ TTL expired            │    1     │
```

### Scheduled Messages View

For scheduled messages:

**Columns to Add:**
1. Scheduled Enqueue Time - (already available as system column)
2. Time Until Delivery - (calculated custom property if available)

**Show:**
- Message ID
- Scheduled Enqueue Time
- Enqueued Time
- Subject

**Hide:**
- Delivery Count (always 0 for scheduled)
- Originating Endpoint

**Result Table:**
```
│ Message ID │ Subject           │ Enqueued Time │ Scheduled Time │
│ abc-123... │ Send Reminder     │ 2025-11-11... │ 2025-11-15...  │
│ def-456... │ Process Batch     │ 2025-11-11... │ 2025-11-12...  │
```

## Tips for Creating Column Configurations

### 1. Start with Defaults
Begin with the default column configuration and only add what you need.

### 2. Group Related Properties
When adding multiple custom columns, group them logically:
- Identification (IDs, keys)
- Routing (endpoints, addresses)
- Metadata (types, timestamps)
- Business data (customer info, order details)

### 3. Keep It Readable
- Limit visible columns to 5-7 for readability
- Use clear, concise display names
- Hide columns you don't need frequently

### 4. Create Context-Specific Views
While the configuration is global, you can quickly adjust it for different scenarios:
- Normal operations: Basic columns
- Debugging: Add conversation tracking
- Performance analysis: Add timing metrics

### 5. Document Your Custom Properties
Keep a reference of custom property keys you use:
```
Common Properties for MyApp:
- OrderId: Unique order identifier
- CustomerId: Customer identifier  
- Priority: High/Normal/Low
- Version: Message schema version
```

## Exporting and Sharing Configurations

While the feature doesn't currently support export/import, you can manually share configurations:

1. Locate the config file: `%APPDATA%/SBInspector/column-config.json`
2. Copy and share with team members
3. Recipients can place it in their `%APPDATA%/SBInspector/` directory

Example configuration file:
```json
{
  "Columns": [
    {
      "Id": "MessageId",
      "DisplayName": "Message ID",
      "IsVisible": true,
      "Type": "MessageId",
      "PropertyKey": null,
      "Order": 0,
      "IsSystemColumn": true
    },
    {
      "Id": "Custom_NServiceBus.MessageIntent",
      "DisplayName": "Intent",
      "IsVisible": true,
      "Type": "CustomProperty",
      "PropertyKey": "NServiceBus.MessageIntent",
      "Order": 4,
      "IsSystemColumn": false
    }
  ]
}
```
