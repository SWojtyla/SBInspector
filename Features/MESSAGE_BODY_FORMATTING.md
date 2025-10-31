# Message Body Formatting

## Overview

This feature enhances the message viewing experience in the Message Details Modal by providing better formatting and display options for message bodies.

## Features

### 1. Expandable Message Body View

Users can now expand and collapse the message body display area:
- **Default view**: 300px height
- **Expanded view**: 600px height
- Toggle button with visual icons (expand/collapse)

### 2. JSON Formatting

When viewing JSON messages, users can format the JSON for better readability:
- Automatically detects JSON content (by content-type or message structure)
- Format JSON button appears when JSON content is detected
- Formats JSON with proper indentation for easy reading
- Toggle between formatted and raw JSON views

### 3. Improved Text Display

- **Word wrapping**: Long lines now wrap instead of requiring horizontal scrolling
- **Vertical layout**: Message body displays vertically making it easier to read
- **Pre-formatted text**: Preserves original formatting while being responsive

## How to Use

### Viewing a Message

1. Click on a message in the message list to open the Message Details Modal
2. Scroll to the "Message Body" section

### Expanding the View

1. Click the **Expand** button to increase the display area from 300px to 600px
2. Click the **Collapse** button to return to the default size

### Formatting JSON

1. If the message contains JSON content, a **Format JSON** button will appear
2. Click **Format JSON** to display the JSON with proper indentation
3. Click **Raw** to view the original unformatted message

## Technical Implementation

### Detection Logic

JSON content is automatically detected by:
1. Checking the message's `ContentType` header for "json"
2. Analyzing the message body structure (starts with `{` or `[` and ends with `}` or `]`)

### Formatting

- Uses `System.Text.Json.JsonDocument` for parsing
- Uses `System.Text.Json.JsonSerializer` with `WriteIndented = true` for formatting
- Gracefully falls back to raw display if JSON parsing fails

### Display Enhancements

- CSS properties:
  - `white-space: pre-wrap` - Preserves whitespace and wraps text
  - `word-wrap: break-word` - Breaks long words if necessary
  - Dynamic `max-height` based on expand/collapse state

## UI Components Modified

- **MessageDetailsModal.razor**: Enhanced message body display section
  - Added expand/collapse functionality
  - Added JSON formatting feature
  - Improved text wrapping and display

## Benefits

1. **Better Readability**: Vertical display with proper wrapping eliminates horizontal scrolling
2. **Space Efficiency**: Expandable view lets users control screen real estate
3. **Developer Friendly**: JSON formatting makes debugging and analysis easier
4. **Automatic Detection**: No manual configuration needed - JSON content is detected automatically

## Future Enhancements

Potential future improvements could include:
- XML formatting support
- Copy formatted text to clipboard
- Syntax highlighting for JSON/XML
- Adjustable height slider instead of just expand/collapse
- Full-screen view option
