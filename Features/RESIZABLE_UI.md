# Resizable UI Components

This document describes the UI improvements made to enhance user experience with resizable panels and better text handling in the message table.

## Features Implemented

### 1. Resizable Entity Tree Panel

The entities/queues menu (left sidebar) is now resizable, allowing users to adjust the width to their preference.

#### Implementation Details

- **Drag-to-Resize**: Users can now drag the vertical divider between the entity tree and the main content area to resize the panel
- **Visual Feedback**: The resizer handle changes color on hover to indicate it's interactive
- **Constraints**: The panel width is constrained between 250px (minimum) and 600px (maximum) to ensure usability
- **Responsive**: On smaller screens (< 992px), the resizable functionality is disabled and the layout switches to a stacked vertical layout
- **Collapse State**: The existing collapse functionality is preserved - when collapsed, the panel shows only icons

#### Technical Implementation

**JavaScript (app.js)**:
```javascript
window.initResizablePanel = function (resizerId, leftPanelId, minWidth, maxWidth) {
    // Initializes mouse event handlers for drag-to-resize functionality
}
```

**CSS (app.css)**:
```css
.resizable-container {
    display: flex;
    width: 100%;
}

.resizable-panel-left {
    flex: 0 0 350px;
    min-width: 250px;
    max-width: 600px;
}

.resizer {
    width: 6px;
    background-color: #e0e0e0;
    cursor: col-resize;
}
```

**Component (Home.razor)**:
- Added `IJSRuntime` injection for JavaScript interop
- Initialized resizer in `OnAfterRenderAsync`
- Disposed resizer in `Dispose()` method
- Replaced `MudGrid` layout with flex-based resizable container

### 2. Improved Message Table Text Handling

The message overview table now handles long text strings more effectively by utilizing available whitespace instead of showing horizontal scrollbars.

#### Key Improvements

- **Word Wrapping**: Long strings now wrap within table cells instead of causing horizontal scrollbars
- **Dynamic Width**: Columns dynamically adjust to use available space
- **Tooltips**: All text values display tooltips showing the full content on hover
- **Better Readability**: Text breaks at word boundaries for better readability

#### Implementation Details

**CSS Changes**:
```css
.message-table-responsive {
    table-layout: auto;
    width: 100%;
}

.text-wrap-cell {
    white-space: normal !important;
    word-break: break-word;
}
```

**Component Changes (MessageListTable.razor)**:
- Removed fixed `max-width` constraints from cell content
- Changed from `overflow-x: auto` to `word-break: break-word` for text wrapping
- Added tooltips to all text columns for showing full content
- Applied `white-space: nowrap` only to date/time columns to prevent awkward breaks
- Added minimum widths to table headers to maintain structure

#### Affected Columns
- **Message ID**: Now uses `word-break: break-all` to break at any character
- **Subject**: Uses `word-break: break-word` to break at word boundaries
- **Content Type**: Uses `word-break: break-word` with tooltip
- **Originating Endpoint**: Uses `word-break: break-word` with tooltip
- **Custom Properties**: Uses `word-break: break-word` with tooltip
- **Date/Time Columns**: Remain with `white-space: nowrap` to prevent splitting dates

## User Benefits

1. **More Screen Real Estate**: Users can allocate more or less space to the entity tree based on their needs
2. **Better Text Visibility**: Long message properties are now fully visible without horizontal scrolling
3. **Improved Workflow**: Users can resize panels once and the browser maintains the size within the session
4. **Responsive Design**: The layout adapts gracefully to different screen sizes

## Browser Compatibility

- Modern browsers (Chrome, Edge, Firefox, Safari) - Full support
- The resizable feature requires JavaScript to be enabled
- Falls back gracefully on mobile devices with automatic stacking

## Future Enhancements

Potential improvements for future releases:

1. **Persistent Sizing**: Save panel width preferences to browser localStorage or user settings
2. **Column Resizing**: Add ability to resize individual table columns
3. **Double-Click Reset**: Double-click the resizer to reset to default width
4. **Keyboard Accessibility**: Add keyboard controls for resizing (arrow keys)
5. **Touch Support**: Enhance touch gesture support for mobile devices
