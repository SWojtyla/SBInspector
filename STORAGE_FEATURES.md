# Storage Features

This document describes the connection string and message template storage features implemented in SBInspector.

## Overview

SBInspector now supports persistent storage of connection strings and message templates using browser localStorage. This makes it easy to:
- Switch between multiple Service Bus environments
- Reuse message templates for testing
- Save existing messages as templates for future use

## Saved Connection Strings

### Features

- **Save Connection Strings**: When connecting to a Service Bus namespace, you can optionally save the connection string with a friendly name for later use.
- **Load Saved Connections**: Quickly load previously saved connection strings from a dropdown menu.
- **Delete Connections**: Remove saved connection strings you no longer need.
- **Last Used Tracking**: The application tracks when each connection was last used for easy identification.

### How to Use

1. **Saving a Connection**:
   - Enter your Service Bus connection string
   - Check the "Save this connection for later use" checkbox
   - Enter a friendly name for the connection (e.g., "Development", "Production", "Testing")
   - Click "Connect"
   - If the connection succeeds, it will be saved to localStorage

2. **Loading a Saved Connection**:
   - When on the connection screen, you'll see a "Saved Connections" dropdown
   - Select a connection from the dropdown
   - Click the "Load" button to populate the connection string field
   - Click "Connect" to establish the connection

3. **Deleting a Saved Connection**:
   - Select a connection from the "Saved Connections" dropdown
   - Click the delete (trash) button next to the "Load" button
   - The connection will be removed from localStorage

### Security Note

Connection strings are stored in browser localStorage. While the connection string input field uses a password type for display security, the actual data is stored unencrypted in localStorage. This is appropriate for development and testing environments, but be cautious when using with production credentials. Consider using Azure Key Vault or other secure credential management systems for production scenarios.

## Message Templates

### Features

- **Save Message Templates**: Save message content, subject, content type, and properties as reusable templates.
- **Load Templates**: Quickly populate the send message form with saved templates.
- **Save from Existing Messages**: When viewing message details, save the message as a template with one click.
- **Delete Templates**: Remove templates you no longer need.
- **Last Used Tracking**: Track when each template was last used.

### How to Use

1. **Creating a Template from Scratch**:
   - When sending a new message (not in reschedule mode), fill in the message details
   - Check the "Save as template for later use" checkbox
   - Enter a name for the template
   - Click "Send" to send the message and save the template

2. **Creating a Template from an Existing Message**:
   - View message details by clicking on a message in the messages list
   - In the message details modal, click "Save as Template"
   - Enter a name for the template
   - Click "Save"
   - The template will be saved with the message body, subject, content type, and properties

3. **Using a Saved Template**:
   - Open the send message modal
   - Select a template from the "Message Templates" dropdown
   - Click the "Load" button
   - The form will be populated with the template data
   - Modify as needed and send

4. **Deleting a Template**:
   - Open the send message modal
   - Select a template from the "Message Templates" dropdown
   - Click the delete (trash) button
   - The template will be removed from localStorage

### Template Structure

Each template stores:
- **Name**: Friendly name for the template
- **Message Body**: The content of the message
- **Subject**: Optional message subject
- **Content Type**: MIME type (e.g., application/json, text/plain)
- **Properties**: Dictionary of application properties (key-value pairs)
- **Created At**: Timestamp when the template was created
- **Last Used At**: Timestamp when the template was last loaded

## Data Storage

Both connection strings and message templates are stored in browser localStorage:
- **Connection strings**: Stored under key `sbinspector_connections`
- **Message templates**: Stored under key `sbinspector_templates`

The data is stored as JSON and persists across browser sessions. Clearing browser data or localStorage will remove all saved connections and templates.

## Implementation Details

### Architecture

The storage feature follows the clean architecture pattern used throughout SBInspector:

- **Domain Layer** (`Core/Domain/`):
  - `SavedConnection.cs`: Model for saved connection strings
  - `MessageTemplate.cs`: Model for message templates

- **Interface Layer** (`Core/Interfaces/`):
  - `IStorageService.cs`: Interface defining storage operations

- **Infrastructure Layer** (`Infrastructure/Storage/`):
  - `LocalStorageService.cs`: Implementation using browser localStorage via JSInterop

- **Presentation Layer** (`Presentation/Components/UI/`):
  - `ConnectionForm.razor`: Enhanced with saved connections UI
  - `SendMessageModal.razor`: Enhanced with message templates UI
  - `MessageDetailsModal.razor`: Enhanced with "Save as Template" feature

### Service Registration

The storage service is registered as a scoped service in `Program.cs`:

```csharp
builder.Services.AddScoped<IStorageService, LocalStorageService>();
```

## Future Enhancements

Potential improvements for future versions:
- Export/import of connections and templates
- Encryption of connection strings at rest
- Cloud synchronization of templates across devices
- Template categories/tags for better organization
- Template versioning
- Shared templates for team collaboration
