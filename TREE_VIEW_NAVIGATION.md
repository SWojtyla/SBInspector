# Tree View Navigation

This document describes the tree view navigation feature that provides a hierarchical view of Service Bus entities with improved UX for browsing and managing queues, topics, and subscriptions.

## Overview

The tree view navigation replaces the previous tab-based interface with a split-panel layout:
- **Left Panel**: Hierarchical tree view of all entities
- **Right Panel**: Detailed information and actions for the selected entity

## Features

### Hierarchical Tree Structure

The tree view organizes Service Bus entities in a logical hierarchy:

```
Service Bus Entities
├── Queues
│   ├── Queue 1 (with message counts)
│   ├── Queue 2 (with message counts)
│   └── ...
└── Topics
    ├── Topic 1 (with subscriptions)
    │   ├── Subscription 1 (with message counts)
    │   ├── Subscription 2 (with message counts)
    │   └── ...
    └── Topic 2
        └── ...
```

### Key Improvements

#### 1. **Split-Panel Layout**
- **Left Panel (350px)**: Tree navigation with collapsible sections
- **Right Panel**: Entity details and message viewing
- Responsive design that stacks vertically on mobile devices

#### 2. **Tree Navigation**
- **Collapsible Sections**: Click section headers to expand/collapse Queues and Topics
- **Expandable Topics**: Click the chevron icon to load and view subscriptions
- **Visual Selection**: Selected entity is highlighted with blue background
- **Message Count Badges**: Color-coded badges show message counts at a glance
  - Blue: Active messages
  - Yellow: Scheduled messages
  - Red: Dead-letter messages

#### 3. **Entity Details Panel**
- Shows selected entity information on the right
- **For Queues**: Displays three message type cards (Active, Scheduled, Dead-letter)
- **For Topics**: Shows information about the topic
- **For Subscriptions**: Displays two message type cards (Active, Dead-letter)
- Click on message type cards to view messages
- Toggle entity status (Enable/Disable) from the details panel

#### 4. **Operation Loading Feedback**
- **Full-Screen Overlay**: Shows when delete/purge operations are in progress
- **Clear Messaging**: Displays what operation is being performed
- **Visual Spinner**: Animated loading indicator
- **Prevents Interaction**: Blocks UI interaction during operations to prevent conflicts

### Visual Indicators

#### Status Badges
- **Green Badge**: Active entity
- **Gray Badge**: Disabled entity

#### Message Count Badges in Tree
- **Blue Badge**: Active messages count
- **Yellow Badge**: Scheduled messages count
- **Red Badge**: Dead-letter messages count

#### Tree Node States
- **Normal**: Default gray icon and text
- **Hover**: Light gray background
- **Selected**: Blue background with blue icon and left border

### User Interactions

#### Selecting Entities

1. **Select a Queue**:
   - Click on a queue name in the tree
   - Right panel shows queue details with message count cards
   - Click a message type card to view messages

2. **Select a Topic**:
   - Click on a topic name in the tree
   - Right panel shows topic details
   - Click the chevron icon to expand and load subscriptions

3. **Select a Subscription**:
   - Expand a topic first
   - Click on a subscription name
   - Right panel shows subscription details with message count cards
   - Click a message type card to view messages

#### Managing Entity Status

- Click the "Enable" or "Disable" button in the details panel header
- Status change is reflected immediately in the tree view
- Success/error messages appear at the top of the screen

#### Viewing Messages

1. From the details panel, click on a message type card
2. Messages panel opens below showing the message list
3. All existing message operations are available:
   - View message details
   - Delete messages
   - Requeue dead-letter messages
   - Purge all messages
   - Send new messages
   - Reschedule scheduled messages

### Operation Loading States

When performing delete or purge operations:

1. **Full-Screen Overlay Appears**:
   - Semi-transparent dark background
   - White card in the center
   - Large animated spinner
   - Operation title and description

2. **Operation Messages**:
   - **Deleting Message**: "Please wait while the message is being deleted..."
   - **Requeuing Message**: "Please wait while the message is being moved back to the active queue..."
   - **Purging Messages**: "Please wait while all messages are being deleted. This may take a while..."

3. **After Operation**:
   - Overlay disappears
   - Success or error message appears at the top
   - Tree view message counts are automatically refreshed

## Technical Implementation

### New Components

1. **EntityTreeView.razor**
   - Renders the hierarchical tree structure
   - Handles section collapse/expand
   - Manages topic subscription loading
   - Emits events for entity selection

2. **EntityDetailsPanel.razor**
   - Displays selected entity information
   - Shows interactive message type cards
   - Provides status toggle functionality
   - Emits events for message viewing and status changes

3. **OperationLoadingOverlay.razor**
   - Shows full-screen loading state
   - Displays operation-specific messages
   - Blocks user interaction during operations

### CSS Enhancements

New styles added to `app.css`:

- `.split-panel-container`: Main split layout container
- `.split-panel-left`: Left tree panel
- `.split-panel-right`: Right details panel
- `.tree-*`: Tree view styling (sections, nodes, badges)
- `.entity-details-panel`: Details panel styling
- `.message-type-cards`: Interactive card layout
- `.operation-overlay`: Full-screen loading overlay
- Responsive breakpoints for mobile devices

### State Management

The tree view maintains the following state:

- **Selected Entity**: Type (queue/topic/subscription) and name
- **Expanded Sections**: Which tree sections are open
- **Expanded Topics**: Which topics have loaded subscriptions
- **Topic Subscriptions**: Dictionary mapping topic names to subscription lists
- **Loading States**: Which topics are currently loading subscriptions

### Data Refresh

The tree view automatically refreshes entity data after:
- Status toggle operations
- Message delete operations
- Message purge operations
- Message requeue operations

## Responsive Design

### Desktop (> 992px)
- Side-by-side panels
- Tree panel: 350px wide
- Details panel: Fills remaining space
- Three-column message type cards

### Tablet/Mobile (≤ 992px)
- Stacked vertical layout
- Tree panel: Full width, max height 400px
- Details panel: Full width below tree
- Single-column message type cards

## Browser Compatibility

The tree view works on:
- Chrome/Edge (latest)
- Firefox (latest)
- Safari (latest)
- Mobile browsers (iOS Safari, Chrome Mobile)

## Benefits

### For Users

1. **Better Overview**: See all entities at once in a hierarchical structure
2. **Faster Navigation**: No need to switch between tabs
3. **Clearer Context**: Always know which entity is selected
4. **Visual Feedback**: Clear indication when operations are in progress
5. **Efficient Workflow**: Quick access to message viewing from details panel

### For Development

1. **Maintainable Code**: Separate components for tree, details, and loading
2. **Reusable CSS**: Modular styles for tree and panels
3. **Clean Architecture**: Clear separation of concerns
4. **Extensible**: Easy to add new entity types or actions

## Future Enhancements

Potential improvements for the tree view:

1. **Search/Filter**: Add search box to filter entities in tree
2. **Keyboard Navigation**: Arrow keys to navigate tree
3. **Drag and Drop**: Reorder or move items (if applicable)
4. **Context Menus**: Right-click menu for quick actions
5. **Refresh Button**: Manual refresh of tree data
6. **Badges**: Show warning icons for disabled entities
7. **Tooltips**: Show full entity names on hover for long names
8. **Collapse All/Expand All**: Buttons to control all sections at once

## Usage Tips

1. **Keep Topics Collapsed**: Only expand topics when needed to reduce clutter
2. **Use Details Panel**: Click on message type cards for quick access to messages
3. **Watch for Badges**: Color-coded badges help identify queues/subscriptions with issues
4. **Wait for Operations**: The loading overlay ensures operations complete safely
5. **Check Success Messages**: Green alerts confirm successful operations

## Accessibility

The tree view includes:
- Semantic HTML structure
- ARIA labels for screen readers
- Keyboard navigation support
- High contrast text and colors
- Focus visible states
- Clear visual hierarchy

## Performance Considerations

- **Lazy Loading**: Subscriptions are only loaded when topics are expanded
- **Efficient Updates**: Only affected entities are refreshed after operations
- **CSS Transitions**: GPU-accelerated animations for smooth interactions
- **Minimal Re-renders**: Optimized state management reduces unnecessary updates
