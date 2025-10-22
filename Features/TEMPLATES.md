# Message Template Management

## Overview

The Message Template Management feature provides a dedicated interface for managing reusable message templates in SBInspector. Users can create, view, edit, and delete message templates that can be used when sending messages to Azure Service Bus queues and topics.

## Features

### Template Management Page

Access the template management page via the navigation menu under "Message Templates" or by navigating to `/templates`.

The page displays:
- A table listing all saved templates with their metadata
- Action buttons for each template (View, Edit, Delete)
- A "New Template" button to create new templates
- Total template count

### Template Properties

Each template contains the following properties:

- **Name** (required): A unique, descriptive name for the template
- **Message Body** (required): The content/payload of the message
- **Subject** (optional): The message subject
- **Content Type** (optional): MIME type of the message (defaults to "text/plain")
- **Application Properties** (optional): Custom key-value pairs in format `key=value` (one per line)
- **Created**: Timestamp when the template was created
- **Last Used**: Timestamp when the template was last used (from Send Message modal)

### CRUD Operations

#### Create Template

1. Click the "New Template" button on the Templates page
2. Fill in the template details in the modal dialog:
   - Enter a template name
   - Enter the message body
   - Optionally add subject, content type, and application properties
3. Click "Save" to create the template

#### View Template

1. Click the "View" button (eye icon) for any template in the list
2. A read-only modal displays all template details including:
   - Template name
   - Message body (formatted)
   - Subject and content type
   - Application properties
   - Created and last used timestamps
3. Click "Edit" from the view modal to switch to edit mode
4. Click "Close" to return to the template list

#### Edit Template

1. Click the "Edit" button (pencil icon) for any template in the list
2. Modify the template details in the modal dialog
3. Click "Save" to update the template
4. Click "Cancel" to discard changes

#### Delete Template

1. Click the "Delete" button (trash icon) for any template in the list
2. Confirm the deletion in the confirmation dialog
3. The template will be permanently deleted

## Using Templates with Send Message

Templates created here can be used when sending messages:

1. Open the Send Message modal (from queue or topic details)
2. Select a template from the dropdown
3. Click "Load" to populate the form with template data
4. Modify as needed and send

Templates can also be created directly from the Send Message modal by checking "Save as template for later use".

## Storage

Templates are persisted using the configured storage service:
- **Browser Local Storage**: Stored in the browser's localStorage (web deployments)
- **File System**: Stored in `SBInspector/templates.json` on the desktop (Tauri/desktop deployments)

The storage location can be configured in the Storage Settings panel on the home page.

## Technical Implementation

### Components

- **Templates.razor**: Main page component located at `/templates`
- **MessageTemplateEditorModal.razor**: Reusable modal for creating/editing templates
- **ConfirmationModal.razor**: Shared confirmation dialog for delete operations

### Services

Template operations use the `IStorageService` interface:
- `GetMessageTemplatesAsync()`: Retrieve all templates
- `SaveMessageTemplateAsync(template)`: Create or update a template
- `DeleteMessageTemplateAsync(id)`: Delete a template by ID
- `UpdateTemplateLastUsedAsync(id)`: Update the last used timestamp

### Domain Model

```csharp
public class MessageTemplate
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string MessageBody { get; set; }
    public string? Subject { get; set; }
    public string? ContentType { get; set; }
    public Dictionary<string, object>? Properties { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
}
```

## Best Practices

1. **Naming**: Use descriptive template names that clearly indicate the message purpose
2. **Content Type**: Set appropriate content type (e.g., `application/json` for JSON payloads)
3. **Properties**: Use application properties for message metadata and routing information
4. **Organization**: Regularly clean up unused templates to keep the list manageable
5. **Testing**: Test templates in a development environment before using in production

## Examples

### JSON Order Confirmation Template

```
Name: Order Confirmation
Message Body: {"orderId": "12345", "status": "confirmed", "total": 99.99}
Subject: Order Confirmed
Content Type: application/json
Properties:
  environment=production
  version=1.0
```

### Plain Text Notification Template

```
Name: System Notification
Message Body: System maintenance scheduled for tonight at 11 PM UTC
Subject: System Maintenance Notice
Content Type: text/plain
Properties:
  priority=high
  category=maintenance
```

### XML Data Transfer Template

```
Name: Customer Data Export
Message Body: <customer><id>123</id><name>John Doe</name></customer>
Subject: Customer Export
Content Type: application/xml
Properties:
  export-type=customer
  format=xml
```
