# Column Customization - Visual Guide

## Feature Location

The column customization feature is accessible from the message list table view, which appears after connecting to a Service Bus queue or subscription.

## User Interface Flow

### 1. Message List Table with Settings Button

When viewing messages in a queue or subscription, users will see a **Settings** icon (⚙️) in the top-right corner of the message table:

```
┌─────────────────────────────────────────────────────────────┐
│                                    [Settings Icon ⚙️]        │
├─────────────────────────────────────────────────────────────┤
│ Message ID    │ Originating Endpoint │ Enqueued Time │ ... │
├─────────────────────────────────────────────────────────────┤
│ abc-123...    │ MyService            │ 2025-11-11... │ ... │
│ def-456...    │ OtherService         │ 2025-11-11... │ ... │
└─────────────────────────────────────────────────────────────┘
```

### 2. Column Configuration Modal

Clicking the settings button opens the **Configure Table Columns** modal:

```
┌───────────────────────────────────────────────────────┐
│  🔲 Configure Table Columns                           │
├───────────────────────────────────────────────────────┤
│                                                        │
│  Default Columns                                       │
│  ☑ Message ID                                         │
│  ☑ Originating Endpoint                               │
│  ☑ Enqueued Time                                      │
│  ☑ Delivery                                           │
│                                                        │
│  ─────────────────────────────────────────────        │
│                                                        │
│  Custom Columns                                        │
│  ☑ Message Intent                    [Delete 🗑️]      │
│  ☑ Conversation ID                   [Delete 🗑️]      │
│                                                        │
│  ─────────────────────────────────────────────        │
│                                                        │
│  Add Custom Column                                     │
│  Property Key: [________________________]             │
│  Display Name: [________________________]             │
│  [+ Add Custom Column]                                │
│                                                        │
├───────────────────────────────────────────────────────┤
│  [Reset to Default]         [Cancel]  [Save]         │
└───────────────────────────────────────────────────────┘
```

### 3. After Configuration

The table updates to show only the selected columns:

**Before (Default):**
```
│ Message ID │ Originating Endpoint │ Enqueued Time    │ Delivery │
│ abc-123... │ MyService            │ 2025-11-11 10:00 │    1     │
```

**After (With Custom Columns):**
```
│ Message ID │ Originating Endpoint │ Message Intent │ Enqueued Time    │ Delivery │
│ abc-123... │ MyService            │ Send           │ 2025-11-11 10:00 │    1     │
```

## Step-by-Step Examples

### Example 1: Hiding a Default Column

**Scenario:** User wants to hide the "Originating Endpoint" column

1. Click the Settings icon (⚙️) in the message table
2. In the modal, uncheck "☑ Originating Endpoint"
3. Click "Save"
4. The table refreshes without the "Originating Endpoint" column

**Result:**
```
Before: │ Message ID │ Originating Endpoint │ Enqueued Time │ Delivery │
After:  │ Message ID │ Enqueued Time │ Delivery │
```

### Example 2: Adding a Custom Column for NServiceBus MessageIntent

**Scenario:** NServiceBus user wants to see the message intent

1. Click the Settings icon (⚙️) in the message table
2. In the "Add Custom Column" section:
   - Property Key: `NServiceBus.MessageIntent`
   - Display Name: `Message Intent`
3. Click "Add Custom Column"
4. The new column appears in the "Custom Columns" section
5. Click "Save"

**Result:**
```
│ Message ID │ Originating Endpoint │ Message Intent │ Enqueued Time │
│ abc-123... │ MyService            │ Send           │ 2025-11-11... │
│ def-456... │ OtherService         │ Publish        │ 2025-11-11... │
```

### Example 3: Adding Multiple Custom Columns

**Scenario:** User wants to track conversation flow

1. Add custom column:
   - Property Key: `NServiceBus.ConversationId`
   - Display Name: `Conversation ID`
2. Add another custom column:
   - Property Key: `NServiceBus.RelatedTo`
   - Display Name: `Related Message`
3. Click "Save"

**Result:**
```
│ Message ID │ Conversation ID │ Related Message │ Enqueued Time │
│ abc-123... │ conv-001        │ (not set)       │ 2025-11-11... │
│ def-456... │ conv-001        │ abc-123...      │ 2025-11-11... │
```

### Example 4: Removing a Custom Column

**Scenario:** User no longer needs the "Related Message" column

1. Click the Settings icon (⚙️)
2. Find "Related Message" in the "Custom Columns" section
3. Click the Delete icon (🗑️) next to it
4. Click "Save"

**Result:** The "Related Message" column is removed from the table

### Example 5: Reset to Default

**Scenario:** User wants to restore the original column configuration

1. Click the Settings icon (⚙️)
2. Click "Reset to Default" button
3. All custom columns are removed
4. All default columns are set to visible
5. Click "Save"

**Result:** Table shows the original default columns only

## UI Elements

### Settings Button
- **Location:** Top-right corner of the message table
- **Icon:** ⚙️ (settings/gear icon)
- **Color:** Primary (blue)
- **Size:** Small

### Column Configuration Modal
- **Title:** "🔲 Configure Table Columns"
- **Sections:**
  - Default Columns (system columns)
  - Custom Columns (user-added columns)
  - Add Custom Column (form to add new columns)
- **Actions:**
  - Reset to Default (warning color, left-aligned)
  - Cancel (default color)
  - Save (primary color, filled)

### Column Display
- **Checkboxes:** Toggle visibility
- **Text:** Column display name
- **Delete Button:** Only for custom columns (red/error color)

## Column Types and Display

### System Columns

1. **Message ID**
   - Display: Truncated code with tooltip
   - Max width: 200px with horizontal scroll

2. **Originating Endpoint**
   - Display: Text with tooltip
   - Max width: 300px with horizontal scroll
   - Source: `NServiceBus.OriginatingEndpoint` property

3. **Enqueued Time**
   - Display: Formatted date-time
   - Format: `yyyy-MM-dd HH:mm:ss`

4. **Delivery Count**
   - Display: Chip/badge
   - Color: Warning (orange) if > 0, Secondary (gray) if 0

### Custom Property Columns

- **Display:** Plain text
- **Max width:** 200px with horizontal scroll
- **Value:** Extracted from `MessageInfo.Properties` dictionary
- **Default:** "(not set)" if property doesn't exist

## Persistence

Column configuration is saved automatically and persists across:
- Browser sessions (for Blazor Server)
- Application restarts (for MAUI)
- Different queues/topics (configuration is global)

## Common Property Keys for NServiceBus Users

Reference for common NServiceBus properties to add as custom columns:

| Property Key | Suggested Display Name | Description |
|-------------|----------------------|-------------|
| `NServiceBus.MessageIntent` | Message Intent | Send, Publish, Reply, etc. |
| `NServiceBus.EnclosedMessageTypes` | Message Types | Full type name of the message |
| `NServiceBus.ConversationId` | Conversation ID | Groups related messages |
| `NServiceBus.RelatedTo` | Related To | ID of the message this is related to |
| `NServiceBus.ProcessingEndpoint` | Processing Endpoint | Endpoint that processed the message |
| `NServiceBus.OriginatingMachine` | Originating Machine | Machine that sent the message |
| `NServiceBus.TimeSent` | Time Sent | Original send time |

## Accessibility

- Checkboxes are keyboard accessible
- Modal can be closed with Escape key
- All buttons have proper labels
- Color-blind friendly (no reliance on color alone)
