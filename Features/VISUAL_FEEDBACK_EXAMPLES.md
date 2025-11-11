# Visual Feedback Examples

This document provides visual descriptions of the feedback users will see when performing actions in SBInspector.

## New Visual Feedback Added in This PR

### 1. Template Operations in Send Message Modal

#### Before This PR
- User clicks "Load Template" → No feedback until operation completes
- User clicks "Delete" → No feedback until operation completes
- Users might click multiple times thinking nothing happened

#### After This PR

**Load Template Button:**
```
[Normal State]
┌─────────────────────┐
│  📥 Load Template   │  ← Blue filled button
└─────────────────────┘

[Loading State - Added in this PR]
┌─────────────────────┐
│  ◐ Loading...       │  ← Spinner animating, button disabled, grey
└─────────────────────┘
```

**Delete Template Button:**
```
[Normal State]
┌─────────────────────┐
│  🗑️  Delete          │  ← Red outlined button
└─────────────────────┘

[Deleting State - Added in this PR]
┌─────────────────────┐
│  ◐ Deleting...      │  ← Spinner animating, button disabled, grey
└─────────────────────┘
```

### 2. Send Message Operation

#### Before This PR
- User fills form in modal and clicks "Send"
- Modal shows spinner and closes
- **Then nothing!** User doesn't know if message is being sent
- Message list eventually updates but no indication in between

#### After This PR

**Flow:**
```
1. User clicks "Send" in modal
   ┌──────────────────────────────┐
   │  ◐ Sending...                │  ← Modal shows spinner
   └──────────────────────────────┘

2. Modal closes, overlay appears (NEW!)
   ┌────────────────────────────────────────┐
   │                                        │
   │        ◐                               │
   │    Sending Message                     │
   │  Please wait while the message is      │
   │  being sent...                         │
   │                                        │
   └────────────────────────────────────────┘
   
3. Success message appears at top
   ┌────────────────────────────────────────┐
   │ ✓ Message sent successfully.           │
   └────────────────────────────────────────┘
```

### 3. Export Operations

#### Before This PR
- User clicks export button
- **Nothing visible happens** until file save dialog appears
- On slow networks/large exports, seemed broken

#### After This PR

**Export Single Message:**
```
User clicks export button on a message row
   ↓
┌────────────────────────────────────────┐
│                                        │
│        ◐                               │
│    Exporting Message                   │
│  Please wait while the message is      │
│  being exported...                     │
│                                        │
└────────────────────────────────────────┘
   ↓
File save dialog appears OR success message
```

**Export Filtered Messages:**
```
User clicks "Export Filtered" button
   ↓
┌────────────────────────────────────────┐
│                                        │
│        ◐                               │
│    Exporting Messages                  │
│  Please wait while messages are        │
│  being exported...                     │
│                                        │
└────────────────────────────────────────┘
   ↓
Success: "Successfully exported 15 message(s) to filename.json"
```

## Existing Visual Feedback (Already Working)

### Connection Form
```
[Normal State]
┌─────────────────┐
│   Connect       │
└─────────────────┘

[Connecting State]
┌─────────────────┐
│ ◐ Connecting... │  ← Spinner + disabled
└─────────────────┘
```

### Send Button in Modal
```
[Normal State]
┌─────────────────┐
│  ✉️  Send        │
└─────────────────┘

[Sending State]
┌─────────────────┐
│ ◐ Sending...    │  ← Spinner + disabled
└─────────────────┘
```

### Refresh Entities
```
Entity Tree with spinner while loading:
┌──────────────────────────┐
│  🌳 Entities    🔄       │
│                          │
│      ◐                   │ ← Spinner shown while loading
│   Loading...             │
└──────────────────────────┘

Once loaded:
┌──────────────────────────┐
│  🌳 Entities    🔄       │
│                          │
│  📦 Queues (3)           │
│    ├─ queue-1            │
│    ├─ queue-2            │
│    └─ queue-3            │
│  📨 Topics (2)           │
│    ├─ topic-1            │
│    └─ topic-2            │
└──────────────────────────┘
```

### Message Operations (Delete, Requeue, Move)
```
User clicks delete/requeue/move button
   ↓
Confirmation dialog appears
   ↓
User confirms
   ↓
┌────────────────────────────────────────┐
│                                        │
│        ◐                               │
│    [Operation Name]                    │
│  Please wait while the message is      │
│  being [action]...                     │
│                                        │
└────────────────────────────────────────┘
   ↓
Success message + message removed from list
```

### Purge/Delete Filtered Operations
```
User confirms purge/delete filtered
   ↓
Background panel appears at bottom:
┌────────────────────────────────────────┐
│  Deleting Messages                  ❌ │
│                                        │
│  Deleting active messages from queue1  │
│                                        │
│  ████████████░░░░░░░░░░  45/100        │
│                                        │
│  [Cancel]                              │
└────────────────────────────────────────┘
   ↓
Progress updates in real-time
   ↓
Success: "Successfully deleted 100 message(s)"
```

### Load More Messages
```
[Normal State]
┌──────────────────────────┐
│  ⬇️  Load More           │
└──────────────────────────┘

[Loading State]
┌──────────────────────────┐
│  ◐ Loading...            │  ← Spinner + disabled
└──────────────────────────┘
```

## Visual Feedback Patterns Summary

### 1. Button Spinner (In-Place Feedback)
- **When to use**: Quick operations (< 3 seconds)
- **Visual**: Small spinner replaces button content
- **Examples**: Load Template, Delete Template, Connect, Send in modal

### 2. Loading Overlay (Blocking Feedback)
- **When to use**: Operations that modify data
- **Visual**: Full-screen overlay with large spinner and message
- **Examples**: Send message, Export, Delete, Requeue, Move to Dead-Letter

### 3. Background Panel (Progress Feedback)
- **When to use**: Long operations with progress reporting
- **Visual**: Panel at bottom with progress bar and cancel button
- **Examples**: Purge, Delete Filtered

### 4. Loading State (Non-Blocking Feedback)
- **When to use**: Loading/refreshing data
- **Visual**: Spinner in the data area, controls disabled
- **Examples**: Refresh entities, Refresh messages, Load subscriptions

## User Benefits

✅ **Clarity**: Users always know their action was received  
✅ **Confidence**: No more wondering "did it work?"  
✅ **Control**: Can see progress and cancel long operations  
✅ **Prevention**: Disabled buttons prevent accidental duplicates  
✅ **Consistency**: Same patterns across all features  

## Testing the Visual Feedback

To see the new visual feedback:

1. **Template Operations**:
   - Open any queue/topic messages
   - Click "Send New"
   - Select a template → Click "Load Template" (see spinner)
   - Click "Delete" on a template (see spinner)

2. **Send Message**:
   - Fill in message details
   - Click "Send"
   - Watch for modal spinner, then overlay after modal closes (NEW!)

3. **Export Operations**:
   - Right-click any message → Export (see overlay with "Exporting Message")
   - Filter some messages → Click "Export Filtered" (see overlay with "Exporting Messages")

All visual feedback uses smooth animations and clear messaging to provide the best user experience.
