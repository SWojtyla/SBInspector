# UI Flow: Message Details View

## Before (Modal Approach)

```
┌─────────────────────────────────────────────────────────┐
│ Message List Panel                                      │
│ ┌─────────────────────────────────────────────────────┐ │
│ │ ┌───────┬──────────┬──────────┬─────────┬─────────┐ │ │
│ │ │Msg ID │ Subject  │ Enqueued │ Delivery│ Actions │ │ │
│ │ ├───────┼──────────┼──────────┼─────────┼─────────┤ │ │
│ │ │ abc123│ Test Msg │ 10:00 AM │    0    │ [View]  │ │ │
│ │ │ def456│ Another  │ 10:05 AM │    1    │ [View]  │ │ │
│ │ └───────┴──────────┴──────────┴─────────┴─────────┘ │ │
│ └─────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────┘
                        ↓ Click [View]
┌─────────────────────────────────────────────────────────┐
│ ╔═══════════════════════════════════════════════════╗   │
│ ║ Message Details Modal        [X Close]            ║   │
│ ║ ┌───────────────────────────────────────────────┐ ║   │
│ ║ │ Message ID: abc123                            │ ║   │
│ ║ │ Subject: Test Message                         │ ║   │
│ ║ │ ... (details obscure the list below)          │ ║   │
│ ║ └───────────────────────────────────────────────┘ ║   │
│ ║                             [Close] [Save Tmpl]   ║   │
│ ╚═══════════════════════════════════════════════════╝   │
│ (Message List visible but grayed out behind modal)      │
└─────────────────────────────────────────────────────────┘
```

## After (In-Place Approach)

```
┌─────────────────────────────────────────────────────────┐
│ Message List Panel                                      │
│ ┌─────────────────────────────────────────────────────┐ │
│ │ ┌───────┬──────────┬──────────┬─────────┬─────────┐ │ │
│ │ │Msg ID │ Subject  │ Enqueued │ Delivery│ Actions │ │ │
│ │ ├───────┼──────────┼──────────┼─────────┼─────────┤ │ │
│ │ │ abc123│ Test Msg │ 10:00 AM │    0    │ [View]  │ │ │
│ │ │ def456│ Another  │ 10:05 AM │    1    │ [View]  │ │ │
│ │ └───────┴──────────┴──────────┴─────────┴─────────┘ │ │
│ └─────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────┘
                        ↓ Click [View]
┌─────────────────────────────────────────────────────────┐
│ Message Details View                                    │
│ ┌─────────────────────────────────────────────────────┐ │
│ │ [← Back to Messages]                                │ │
│ ├─────────────────────────────────────────────────────┤ │
│ │ Message ID: abc123                                  │ │
│ │ Subject: Test Message                               │ │
│ │ Content Type: application/json                      │ │
│ │ Delivery Count: 0                                   │ │
│ │ Enqueued Time: 2024-01-15 10:00:00                 │ │
│ │                                                     │ │
│ │ Application Properties:                             │ │
│ │ ┌─────────────┬─────────────────────────────────┐  │ │
│ │ │ Key         │ Value                           │  │ │
│ │ ├─────────────┼─────────────────────────────────┤  │ │
│ │ │ correlationId│ 12345                          │  │ │
│ │ └─────────────┴─────────────────────────────────┘  │ │
│ │                                                     │ │
│ │ Message Body:                                       │ │
│ │ ┌─────────────────────────────────────────────────┐ │ │
│ │ │ { "name": "test", "value": 123 }                │ │ │
│ │ └─────────────────────────────────────────────────┘ │ │
│ ├─────────────────────────────────────────────────────┤ │
│ │ [← Back to Messages]        [Save as Template]     │ │
│ └─────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────┘
                        ↓ Click [← Back to Messages]
┌─────────────────────────────────────────────────────────┐
│ Message List Panel (returns to list)                   │
│ ┌─────────────────────────────────────────────────────┐ │
│ │ ┌───────┬──────────┬──────────┬─────────┬─────────┐ │ │
│ │ │Msg ID │ Subject  │ Enqueued │ Delivery│ Actions │ │ │
│ │ ├───────┼──────────┼──────────┼─────────┼─────────┤ │ │
│ │ │ abc123│ Test Msg │ 10:00 AM │    0    │ [View]  │ │ │
│ │ │ def456│ Another  │ 10:05 AM │    1    │ [View]  │ │ │
│ │ └───────┴──────────┴──────────┴─────────┴─────────┘ │ │
│ └─────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────┘
```

## Key Improvements

### Navigation
- **Before**: Click X or Close button to dismiss modal
- **After**: Click prominent "Back to Messages" button (available at top and bottom)

### Screen Usage
- **Before**: Modal overlays the list, using limited space
- **After**: Full panel dedicated to message details

### User Experience
- **Before**: Modal can feel intrusive and harder to read on small screens
- **After**: Natural flow with clear navigation path

### Accessibility
- **Before**: Modal requires managing focus and escape key handling
- **After**: Standard component navigation with clear visual hierarchy

## Component State Flow

```
MessagesPanel Component
│
├─ State: SelectedMessage = null
│  └─→ Render: Message List Table
│       └─→ User clicks message or [View] button
│            └─→ HandleViewDetails(message)
│                 └─→ SelectedMessage = message
│                      └─→ StateHasChanged()
│
├─ State: SelectedMessage = MessageInfo
│  └─→ Render: MessageDetailsView
│       └─→ User clicks [Back to Messages]
│            └─→ HandleBackToList()
│                 └─→ SelectedMessage = null
│                      └─→ StateHasChanged()
│
└─ State: SelectedMessage = null
   └─→ Render: Message List Table (cycle repeats)
```
