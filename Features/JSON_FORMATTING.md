# JSON Formatting Feature

## Overview
The JSON formatting feature provides an easy way to view and format JSON message bodies in a more readable, indented format. This feature is available in both the message details view and the template view.

## How to Use

### In Message Details View
1. Navigate to a queue or subscription and select a message
2. If the message body contains valid JSON (starts with `{` or `[`), a "Format JSON" button will appear
3. Click the "Format JSON" button to display the JSON with indentation
4. Click the "Raw" button to return to the original unformatted view

### In Template View
1. Navigate to the Templates page
2. Click on a template to view its details
3. If the template's message body contains valid JSON, a "Format JSON" button will appear
4. Click the "Format JSON" button to display the JSON with indentation
5. Click the "Raw" button to return to the original unformatted view

## Features

- **Automatic Detection**: The system automatically detects if content is JSON by checking if it starts with `{` or `[`
- **Toggle Formatting**: Easily switch between formatted and raw views with a single button click
- **Error Handling**: If JSON parsing fails, the original content is displayed without errors
- **UTF-8 BOM Support**: Handles JSON with UTF-8 BOM markers (\\uFEFF) correctly
- **Consistent Styling**: The same formatting behavior is available across different views

## Technical Implementation

The feature is implemented using a shared `MessageBodyDisplay` component that:
- Accepts message body text as a parameter
- Uses `System.Text.Json` for JSON parsing and formatting
- Maintains formatting state internally
- Provides a consistent UI across all uses

### Component Usage

```razor
<MessageBodyDisplay MessageBody="@messageBody" Title="Message Body" />
```

### Parameters
- `MessageBody` (string?): The message body content to display
- `Title` (string): The title to display above the message body (default: "Message Body")

## Locations Used

1. **MessageDetailsView.razor** - When viewing a single message from a queue/subscription
2. **TemplateView.razor** - When viewing a saved message template

## Benefits

- **Improved Readability**: Formatted JSON is much easier to read and understand
- **Code Reusability**: Shared component eliminates code duplication
- **Consistency**: Same behavior across different parts of the application
- **User Experience**: Toggle button provides quick access to both formatted and raw views
