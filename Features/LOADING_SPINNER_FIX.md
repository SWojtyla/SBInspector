# Loading Spinner During Entity Loading

## Overview
This feature ensures that a loading spinner is displayed while Service Bus entities (queues and topics) are being loaded, instead of showing a "no entities found" message prematurely.

## User Experience

### Before the Fix
When connecting to Azure Service Bus and loading entities:
1. User clicks "Connect" or "Refresh"
2. The "No entities found" message appears immediately
3. After loading completes, entities appear (if any exist)
4. This created confusion as users see "no entities" even when entities exist

### After the Fix
When connecting to Azure Service Bus and loading entities:
1. User clicks "Connect" or "Refresh"
2. A loading spinner is displayed
3. After loading completes:
   - If entities exist, they are displayed
   - If no entities exist, the "No entities found" message appears
4. Clear feedback that the system is working and loading data

## Technical Implementation

### File Changed
`SBInspector.Shared/Presentation/Components/UI/EntityTreeView.razor`

### Modification
Line 126 was updated to include a check for the `IsLoading` state:

**Before:**
```razor
@if (!Queues.Any() && !Topics.Any())
{
    <MudAlert Severity="Severity.Info">No entities found</MudAlert>
}
```

**After:**
```razor
@if (!IsLoading && !Queues.Any() && !Topics.Any())
{
    <MudAlert Severity="Severity.Info">No entities found</MudAlert>
}
```

### Logic
The "No entities found" message now only displays when ALL of these conditions are true:
1. `!IsLoading` - The component is not currently loading entities
2. `!Queues.Any()` - There are no queues in the namespace
3. `!Topics.Any()` - There are no topics in the namespace

The loading spinner (lines 18-23 of EntityTreeView.razor) is displayed when `IsLoading` is true.

## Testing
Unit tests were added in `SBInspector.Tests/Components/EntityTreeViewTests.cs`:

1. **EntityTreeView_WhenLoading_ShowsSpinner**: Verifies that the spinner is shown when `IsLoading` is true
2. **EntityTreeView_WhenNotLoadingAndNoEntities_ShowsNoEntitiesMessage**: Verifies that the message appears when loading is complete and no entities exist

## Benefits
1. **Better User Experience**: Users receive clear feedback that the system is working
2. **Reduced Confusion**: No more premature "no entities found" messages
3. **Consistent Behavior**: Aligns with expected loading patterns in modern applications
4. **Minimal Code Change**: Single-line modification reduces risk of introducing bugs

## Edge Cases Handled
- **Empty namespaces**: Message appears after loading completes
- **Filtered results**: Different message shown for search filters that return no results
- **Collapsed panel**: No impact on collapsed state behavior
- **Refresh operation**: Spinner shows during refresh just like initial load

## Related Components
This fix is specific to the `EntityTreeView` component, which is used in both:
- **SBInspector** (Blazor Server web application)
- **SEBInspector.Maui** (MAUI desktop application)

The fix applies to both applications since they share the `SBInspector.Shared` components library.
