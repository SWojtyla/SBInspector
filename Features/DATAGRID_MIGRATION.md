# MudDataGrid Migration

## Overview
This document describes the migration from `MudTable` to `MudDataGrid` for the `MessageListTable` component to provide better text wrapping and resizable columns.

## Problem Statement
The original `MudTable` implementation had the following limitations:
- Text overflow created horizontal scrollbars in cells instead of using available whitespace
- No built-in support for column resizing
- Fixed column widths that didn't adapt well to content

## Solution
Migrated from `MudTable` to `MudDataGrid`, which provides:
- ? **Resizable columns** - Users can drag column borders to resize
- ? **Better text wrapping** - Text uses all available cell space with `word-wrap` and `overflow-wrap`
- ? **Virtualization** - Better performance with large datasets
- ? **Fixed header** - Header remains visible when scrolling
- ? **Responsive height** - Grid adapts to viewport size

## Changes Made

### Component: MessageListTable.razor

#### Before (MudTable)
```razor
<MudTable Items="@SortedMessages" Dense="true" Hover="true" Bordered="true" Striped="true">
    <HeaderContent>
        @foreach (var column in VisibleColumns.OrderBy(c => c.Order))
        {
            <MudTh Align="@GetColumnAlignment(column)">
                <MudButton Variant="Variant.Text" OnClick="@(() => HandleSortColumn(GetColumnSortKey(column)))">
                    @column.DisplayName @GetSortIcon(GetColumnSortKey(column))
                </MudButton>
            </MudTh>
        }
        <MudTh Align="Center">Actions</MudTh>
    </HeaderContent>
    <RowTemplate>
        @foreach (var column in VisibleColumns.OrderBy(c => c.Order))
        {
            <MudTd DataLabel="@column.DisplayName" Align="@GetColumnAlignment(column)">
                @RenderColumnValue(context, column)
            </MudTd>
        }
        <MudTd DataLabel="Actions" Align="Center">
            <!-- Action buttons -->
        </MudTd>
    </RowTemplate>
</MudTable>
```

#### After (MudDataGrid)
```razor
<MudDataGrid T="MessageInfo" Items="@SortedMessages" Dense="true" Hover="true" 
             Bordered="true" Striped="true" 
             ColumnResizeMode="ResizeMode.Column"
             SortMode="SortMode.None"
             Virtualize="true"
             FixedHeader="true"
             Height="calc(100vh - 300px)">
    <Columns>
        @foreach (var column in VisibleColumns.OrderBy(c => c.Order))
        {
            <PropertyColumn T="MessageInfo" TProperty="string" 
                           Title="@column.DisplayName" 
                           Sortable="false"
                           Resizable="true"
                           HeaderStyle="@GetHeaderStyle(column)"
                           CellStyle="@GetCellStyle(column)">
                <HeaderTemplate>
                    <MudButton Variant="Variant.Text" OnClick="@(() => HandleSortColumn(GetColumnSortKey(column)))" Style="padding: 0;">
                        @column.DisplayName @GetSortIcon(GetColumnSortKey(column))
                    </MudButton>
                </HeaderTemplate>
                <CellTemplate>
                    @RenderColumnValue(context.Item, column)
                </CellTemplate>
            </PropertyColumn>
        }
        <TemplateColumn T="MessageInfo" Title="Actions" Sortable="false" Resizable="false" 
                       HeaderStyle="text-align: center; width: 250px;" 
                       CellStyle="text-align: center; white-space: nowrap;">
            <CellTemplate>
                <!-- Action buttons -->
            </CellTemplate>
        </TemplateColumn>
    </Columns>
</MudDataGrid>
```

### Key Property Changes

| Feature | MudTable | MudDataGrid |
|---------|----------|-------------|
| Column Definition | `<HeaderContent>` + `<RowTemplate>` | `<Columns>` with `<PropertyColumn>` |
| Resizable Columns | ? Not supported | ? `ColumnResizeMode="ResizeMode.Column"` + `Resizable="true"` |
| Virtualization | ? No | ? `Virtualize="true"` |
| Fixed Header | Manual CSS | ? `FixedHeader="true"` |
| Row Context | `context` | `context.Item` |
| Cell Alignment | `Align` property on `MudTh`/`MudTd` | `HeaderStyle` and `CellStyle` with CSS |

### CSS Style Changes

#### Cell Text Wrapping
Removed fixed widths and horizontal scrolling:
```csharp
// Before
"max-width:200px;overflow-x:auto;display:inline-block;"

// After (via GetCellStyle method)
"text-align: left; word-wrap: break-word; overflow-wrap: break-word;"
```

#### Code Refactoring
Added new helper methods for consistent styling:
```csharp
private string GetHeaderStyle(ColumnDefinition column)
{
    var alignment = column.Type == ColumnType.DeliveryCount ? "center" : "left";
    return $"text-align: {alignment};";
}

private string GetCellStyle(ColumnDefinition column)
{
    var alignment = column.Type == ColumnType.DeliveryCount ? "center" : "left";
    return $"text-align: {alignment}; word-wrap: break-word; overflow-wrap: break-word;";
}
```

## User Experience Improvements

### 1. Resizable Columns ?
Users can now drag the border between column headers to resize columns to their preferred width:
- Hover over column border ? cursor changes to resize indicator
- Click and drag ? column width adjusts dynamically
- Columns maintain their new width during the session

### 2. Better Text Wrapping ?
Text now wraps naturally within the cell:
- **Before**: Long text caused horizontal scrollbars in cells
- **After**: Text wraps to use all available cell width
- Better readability for long MessageIDs, endpoints, and property values

### 3. Fixed Header ?
Header remains visible when scrolling through large message lists:
- Improves navigation and context awareness
- No need to scroll back to top to see column names

### 4. Virtualization ?
Improved performance with large datasets:
- Only visible rows are rendered in the DOM
- Smooth scrolling even with hundreds of messages
- Reduced memory footprint

### 5. Responsive Height ?
Grid height adapts to viewport:
- Height: `calc(100vh - 300px)` allows scrolling within the grid
- Better use of screen real estate
- Works well on different screen sizes

## Column Types Behavior

### System Columns with Text Wrapping
- **Message ID**: Code element with `word-break: break-all` (breaks anywhere for GUIDs)
- **Subject**: Wraps on word boundaries
- **Content Type**: Wraps on word boundaries
- **Originating Endpoint**: Wraps with tooltip showing full value
- **Enqueued Time**: Fixed width, no wrapping needed
- **Scheduled Enqueue Time**: Fixed width, no wrapping needed
- **Sequence Number**: Fixed width, no wrapping needed
- **Delivery Count**: Chip/badge, center-aligned, no wrapping needed

### Custom Property Columns
- Text wraps naturally using `word-wrap: break-word`
- No fixed width constraints
- Resizable by user

### Actions Column
- Fixed width (250px)
- Non-resizable (to prevent accidental resize)
- `white-space: nowrap` to keep buttons in one line

## Breaking Changes
**None** - All existing functionality is preserved:
- Column configuration still works
- Sorting still works
- All event callbacks maintained
- Custom property columns work the same way

## Performance Considerations

### Virtualization Benefits
- Renders only ~20-30 visible rows at a time (depending on row height and viewport)
- Significant memory savings for large datasets (1000+ messages)
- Smooth scrolling experience

### Fixed Header Benefits
- No re-rendering of header on scroll
- Better perceived performance

## Known Limitations

### MudDataGrid Specifics
1. **Column Resize Persistence**: Column widths are not persisted across sessions (would require additional state management)
2. **Minimum Width**: MudDataGrid enforces a minimum column width (~50px)
3. **Browser Compatibility**: Column resizing requires modern browsers (works in all browsers supported by Blazor)

## Future Enhancements

Possible improvements for future versions:
1. **Persist Column Widths**: Save user's column width preferences to local storage
2. **Auto-size Columns**: Add "fit to content" option for columns
3. **Column Reordering**: Allow drag-and-drop to reorder columns (MudDataGrid supports this)
4. **Export Column Layout**: Include column widths in configuration export/import

## Testing Checklist

Manual testing performed:
- ? Build succeeds without errors
- ? Component renders correctly
- ? Column resizing works by dragging borders
- ? Text wraps properly in all column types
- ? Sorting still works correctly
- ? Action buttons work correctly
- ? Column configuration modal still works
- ? Custom property columns render correctly
- ? Fixed header stays visible on scroll
- ? Virtualization works with large datasets
- ? Responsive height adapts to viewport

## Migration Guide for Other Tables

If you want to migrate other tables (QueueListTable, TopicListTable) to MudDataGrid:

1. Replace `<MudTable>` with `<MudDataGrid T="YourType">`
2. Add grid properties:
   ```razor
   ColumnResizeMode="ResizeMode.Column"
   Virtualize="true"
   FixedHeader="true"
   ```
3. Replace `<HeaderContent>` and `<RowTemplate>` with `<Columns>`
4. Define columns using `<PropertyColumn>` or `<TemplateColumn>`
5. Update context references from `context` to `context.Item`
6. Replace `Align` properties with CSS in `HeaderStyle` and `CellStyle`
7. Add text wrapping styles: `word-wrap: break-word; overflow-wrap: break-word;`

## References
- [MudBlazor DataGrid Documentation](https://mudblazor.com/components/datagrid)
- [Column Resizing Documentation](https://mudblazor.com/components/datagrid#resizing)
- [MudBlazor v8.13.0 Release Notes](https://github.com/MudBlazor/MudBlazor/releases/tag/v8.13.0)

---

**Migration Date**: 2025-01-XX  
**MudBlazor Version**: 8.13.0  
**Component**: MessageListTable.razor  
**Status**: ? Complete
