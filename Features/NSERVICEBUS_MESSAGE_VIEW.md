# NServiceBus Message Details View

## Overview

This feature adds a specialized view for NServiceBus messages in the Message Details page. When viewing a message that contains NServiceBus-specific application properties, users can toggle between a standard message view and an NServiceBus-optimized view that highlights the most important NServiceBus properties.

## User Experience

### Toggle Switch
- A toggle switch labeled "NServiceBus View" appears in the message details header **only** when viewing a message that contains NServiceBus properties
- The toggle state is persisted across sessions using the storage service (LocalStorage for web, file storage for MAUI)
- When enabled, subsequent messages will automatically open in NServiceBus view mode

### NServiceBus View Layout
When enabled, the NServiceBus view displays key properties prominently:

1. **Message Intent** - Shown with color-coded chip:
   - Send: Primary (blue)
   - Publish: Success (green)
   - Subscribe: Info (cyan)
   - Reply: Secondary (grey)

2. **Originating Endpoint** - Source system/service

3. **Message Types** (Enclosed Message Types) - Full type names of the message

4. **Correlation ID** - For message correlation tracking

5. **Conversation ID** - For conversation tracking

6. **NServiceBus Message ID** - Internal NServiceBus message identifier

7. **Time Sent** - When the message was sent by NServiceBus

8. **Originating Machine** - Machine that sent the message

9. **Reply To Address** - Where replies should be sent

10. **NServiceBus Version** - Version of NServiceBus used

11. **Content Type** - NServiceBus content type

12. **Additional NServiceBus Properties** - Any other properties with "NServiceBus." prefix

### Standard View
When disabled or viewing non-NServiceBus messages, the standard message details view is shown:
- Service Bus Message ID
- Subject
- Content Type
- Delivery Count
- Enqueued Time
- Scheduled Enqueue Time (if applicable)
- Sequence Number
- Application Properties (expandable panel)
- Message Body

## Technical Implementation

### Components Created

1. **UserPreferences Domain Model** (`Core/Domain/UserPreferences.cs`)
   - Stores user preference for NServiceBus view mode
   - Properties:
     - `UseNServiceBusView` (bool) - Toggle state

2. **NServiceBusMessageHelper** (`Application/Services/NServiceBusMessageHelper.cs`)
   - Static helper class for NServiceBus message detection and property extraction
   - Methods:
     - `IsNServiceBusMessage(MessageInfo)` - Detects if a message contains NServiceBus properties
     - `GetNServiceBusProperties(MessageInfo)` - Extracts all NServiceBus properties
     - `GetNServiceBusProperty(MessageInfo, string)` - Gets a specific NServiceBus property

3. **NServiceBusMessageView Component** (`Presentation/Components/UI/NServiceBusMessageView.razor`)
   - Displays NServiceBus properties in an organized, easy-to-read layout
   - Color-codes message intent for quick identification
   - Shows properties in priority order
   - Handles any additional NServiceBus properties not explicitly listed

4. **Updated MessageDetailsView Component** (`Presentation/Components/UI/MessageDetailsView.razor`)
   - Added toggle switch for NServiceBus view (only visible for NServiceBus messages)
   - Loads user preference on component initialization
   - Saves preference when toggle is changed
   - Conditionally renders NServiceBus view or standard view

### Storage Service Updates

Extended `IStorageService` interface with preference management:
```csharp
Task<UserPreferences> GetUserPreferencesAsync();
Task SaveUserPreferencesAsync(UserPreferences preferences);
```

Implemented in both storage services:
- **LocalStorageService** - Uses browser localStorage with key `sbinspector_preferences`
- **FileStorageService** - Uses JSON file `preferences.json` in storage directory

## Detection Logic

A message is identified as an NServiceBus message if it contains any application properties with the following prefixes:
- `NServiceBus.` (standard NServiceBus properties)
- `$.diagnostics.` (NServiceBus diagnostic headers)

## Benefits

1. **Improved Developer Experience**: NServiceBus developers can quickly identify key message properties without expanding application properties panel
2. **Visual Clarity**: Color-coded message intent provides immediate context
3. **Persistent Preference**: Users don't need to toggle the view every time they open a message
4. **Non-Intrusive**: Toggle only appears for NServiceBus messages, keeping the UI clean for other message types
5. **Comprehensive Coverage**: Displays all standard NServiceBus properties plus any custom ones

## Usage Example

1. Connect to a Service Bus queue/topic that contains NServiceBus messages
2. View a message in the message details view
3. Notice the "NServiceBus View" toggle switch in the header
4. Toggle it ON to see NServiceBus-optimized layout
5. The preference is saved automatically
6. View another NServiceBus message - it will automatically open in NServiceBus view
7. Toggle OFF to return to standard view

## Future Enhancements

Potential improvements for future versions:
- Add filters specifically for NServiceBus properties (e.g., filter by MessageIntent)
- Support for sagas-specific NServiceBus properties
- Visualization of message flow using CorrelationId and ConversationId
- Export NServiceBus properties separately
- Search across NServiceBus properties
