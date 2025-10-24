# Template Modal Binding Fix

## Overview
Fixed the binding issues with the Message Template Editor Modal in Blazor Server mode that prevented edit and delete operations from working correctly.

## Problem
In Blazor Server mode with InteractiveServer render mode, the MessageTemplateEditorModal component was not properly updating its state when attempting to edit or delete templates. The component parameters were missing two-way binding support.

## Solution
Added two-way binding support to the MessageTemplateEditorModal component by:

1. **Added EventCallback Parameters**: Added `IsVisibleChanged`, `IsEditModeChanged`, and `TemplateChanged` EventCallback parameters to support two-way binding.

2. **Updated Parent Components**: Modified both the Shared and Blazor Server Templates.razor pages to use proper two-way binding with handler methods.

3. **Handler Methods**: Created explicit handler methods (`HandleIsVisibleChanged`, `HandleIsEditModeChanged`, `HandleTemplateChanged`) in the Blazor Server Templates.razor to properly handle state changes.

## Files Modified
- `SBInspector.Shared/Presentation/Components/UI/MessageTemplateEditorModal.razor` - Added EventCallback parameters
- `SBInspector.Shared/Presentation/Components/Pages/Templates.razor` - Updated to use `@bind-` syntax
- `SBInspector/Components/Pages/Templates.razor` - Updated with explicit handler methods for InteractiveServer mode

## Technical Details

### MessageTemplateEditorModal.razor Changes
```csharp
[Parameter]
public bool IsVisible { get; set; }

[Parameter]
public EventCallback<bool> IsVisibleChanged { get; set; }

[Parameter]
public bool IsEditMode { get; set; }

[Parameter]
public EventCallback<bool> IsEditModeChanged { get; set; }

[Parameter]
public MessageTemplate? Template { get; set; }

[Parameter]
public EventCallback<MessageTemplate?> TemplateChanged { get; set; }
```

### Templates.razor (Blazor Server) Changes
```csharp
<MessageTemplateEditorModal
    IsVisible="@isEditorVisible"
    IsEditMode="@isEditMode"
    Template="@selectedTemplate"
    IsVisibleChanged="@HandleIsVisibleChanged"
    IsEditModeChanged="@HandleIsEditModeChanged"
    TemplateChanged="@HandleTemplateChanged"
    OnClose="@HandleEditorClose"
    OnSave="@HandleTemplateSave" />
```

## Testing
The fix can be tested by:
1. Navigating to the Message Templates page
2. Creating a new template
3. Attempting to edit the template - the modal should now properly load the template data
4. Attempting to delete the template - the confirmation should work correctly

## Related Issues
This fix addresses the binding issues in Blazor Server mode specifically. The MAUI and standard Blazor implementations continue to work as expected.
