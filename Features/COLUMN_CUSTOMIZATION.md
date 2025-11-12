# Column Customization Feature

## Overview

The Column Customization feature allows users to configure which columns are displayed in the message list table. This provides flexibility to:
- Show or hide default system columns
- Add custom columns based on message application properties
- Persist column preferences across sessions

## Features

### 1. Default System Columns

The following system columns are available by default:
- **Message ID** - Unique identifier for the message
- **Originating Endpoint** - NServiceBus originating endpoint (extracted from message properties)
- **Enqueued Time** - When the message was enqueued
- **Delivery Count** - Number of delivery attempts

### 2. Additional System Columns

Additional system columns can be shown/hidden:
- **Subject** - Message subject
- **Content Type** - Message content type
- **Sequence Number** - Message sequence number
- **Scheduled Enqueue Time** - Scheduled delivery time (if applicable)
- **State** - Message state (Active, Scheduled, DeadLetter)

### 3. Custom Property Columns

Users can add custom columns based on message application properties:
- Add any message property as a column (e.g., `NServiceBus.MessageIntent`)
- Specify a display name for the column
- Values are extracted from the message properties dictionary

## How to Use

### Accessing Column Configuration

1. Navigate to a queue or subscription with messages
2. In the message list table, click the **Settings** icon (⚙️) in the top-right corner
3. The Column Configuration modal will appear

### Configuring Columns

#### Show/Hide System Columns

1. In the "Default Columns" section, check or uncheck columns to show or hide them
2. All system columns remain in the configuration but can be toggled on/off

#### Add Custom Columns

1. Scroll to the "Add Custom Column" section
2. Enter the **Property Key** (e.g., `NServiceBus.MessageIntent`, `NServiceBus.EnclosedMessageTypes`)
3. Enter a **Display Name** (e.g., "Message Intent", "Message Types")
4. Click **Add Custom Column**
5. The new column will appear in the "Custom Columns" section

#### Remove Custom Columns

1. In the "Custom Columns" section, find the column you want to remove
2. Click the **Delete** icon (🗑️) next to the column
3. The column will be removed from the configuration

#### Reset to Default

1. Click the **Reset to Default** button at the bottom of the modal
2. This will restore the original default column configuration
3. All custom columns will be removed

### Saving Changes

1. Click the **Save** button to apply your changes
2. The modal will close and the table will refresh with your new column configuration
3. Your preferences are automatically saved and will persist across sessions

## Technical Implementation

### Architecture

The feature follows the clean architecture pattern used in the project:

#### Domain Layer
- `ColumnConfiguration.cs` - Domain model for column configuration
- `ColumnDefinition` - Individual column definition with properties
- `ColumnType` enum - Type of column (system vs custom)

#### Application Layer
- `ColumnConfigurationService.cs` - Service for managing column preferences
  - Loads configuration from JSON file
  - Saves configuration to JSON file
  - Provides default configuration
  - Manages custom column CRUD operations

#### Presentation Layer
- `ColumnConfigurationModal.razor` - UI component for column configuration
- `MessageListTable.razor` - Updated to render columns dynamically

### Storage

Column preferences are stored in:
```
%APPDATA%/SBInspector/column-config.json
```

The configuration is persisted as JSON:
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
      "DisplayName": "Message Intent",
      "IsVisible": true,
      "Type": "CustomProperty",
      "PropertyKey": "NServiceBus.MessageIntent",
      "Order": 4,
      "IsSystemColumn": false
    }
  ]
}
```

### Service Registration

The `ColumnConfigurationService` is registered as a singleton in both:
- **Blazor Server** (`Program.cs`)
- **MAUI** (`MauiProgram.cs`)

### Column Rendering

The `MessageListTable` component dynamically renders columns based on:
1. Column visibility (`IsVisible` property)
2. Column order (`Order` property)
3. Column type (determines how to extract and display the value)

For custom property columns, values are extracted from the `MessageInfo.Properties` dictionary using the `PropertyKey`.

## Common Use Cases

### NServiceBus Users

Common NServiceBus properties to add as columns:
- `NServiceBus.MessageIntent` - Message intent (Send, Publish, Reply, etc.)
- `NServiceBus.EnclosedMessageTypes` - Message type information
- `NServiceBus.ConversationId` - Conversation identifier
- `NServiceBus.RelatedTo` - Related message ID
- `NServiceBus.ProcessingEndpoint` - Processing endpoint

### Example Configuration

A typical configuration for NServiceBus users might include:
1. Message ID (visible)
2. Originating Endpoint (visible)
3. Message Intent (custom - visible)
4. Enqueued Time (visible)
5. Delivery Count (visible)
6. Conversation ID (custom - visible)

## Limitations

- Custom columns can only display string representations of property values
- Custom columns use simple text display (no special formatting like chips or tooltips)
- Column reordering is not yet supported via drag-and-drop (order is managed internally)
- Sorting on custom property columns is not yet implemented

## Future Enhancements

Possible future improvements:
- Drag-and-drop column reordering in the UI
- Export/import column configurations
- Column width customization
- Advanced formatting options for custom columns
- Predefined column templates for common message types
- Search/autocomplete for property keys when adding custom columns
