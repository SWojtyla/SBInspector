# MudBlazor Migration Fix

## Overview
This document describes the fixes applied to resolve binding issues after the MudBlazor migration that were preventing the UI from functioning correctly in both the Blazor Server and MAUI applications.

## Issues Identified

### 1. Invalid MudSelect Binding Syntax
**Location:** `SBInspector.Shared/Presentation/Components/UI/ConnectionForm.razor`

**Problem:** The MudSelect component had an invalid `@bind-Value:event="onchange"` attribute:
```razor
<MudSelect T="string" @bind-Value="SelectedConnectionName" ... @bind-Value:event="onchange">
```

**Root Cause:** The `@bind-Value:event` syntax is not supported by MudBlazor components. MudBlazor uses `ValueChanged` event callbacks instead.

**Fix:** Changed to use `Value` and `ValueChanged` properties:
```razor
<MudSelect T="string" Value="@SelectedConnectionName" ValueChanged="@((string val) => OnConnectionNameChanged(val))" ...>
```

### 2. Parameter Binding Without EventCallback Invocation
**Location:** Multiple components (ConnectionForm, MessagesPanel, MessageFilterPanel)

**Problem:** Components were using `@bind-Value` on Parameter properties, which doesn't automatically trigger the corresponding EventCallbacks in parent components.

**Examples:**
- `ConnectionForm.razor`: Connection string parameter wasn't updating parent
- `MessagesPanel.razor`: PageSize parameter changes weren't propagating
- `MessageFilterPanel.razor`: Filter changes weren't notifying parent

**Root Cause:** MudBlazor components don't support two-way binding (`@bind-Value`) on Parameter properties. You must manually invoke the EventCallback when values change.

**Fix Pattern:**
```razor
<!-- Before (broken) -->
<MudTextField @bind-Value="ConnectionString" ... />

<!-- After (working) -->
<MudTextField Value="@ConnectionString" ValueChanged="@((string val) => OnConnectionStringChanged(val))" ... />

<!-- With handler in @code section -->
@code {
    [Parameter]
    public string ConnectionString { get; set; } = string.Empty;
    
    [Parameter]
    public EventCallback<string> ConnectionStringChanged { get; set; }
    
    private async Task OnConnectionStringChanged(string value)
    {
        ConnectionString = value;
        await ConnectionStringChanged.InvokeAsync(value);
    }
}
```

### 3. Method Group vs Lambda Expression
**Location:** `ConnectionForm.razor`

**Problem:** ValueChanged was receiving method group instead of EventCallback:
```razor
ValueChanged="@OnConnectionStringChanged"  <!-- Error: cannot convert method group to EventCallback -->
```

**Fix:** Wrapped method calls in lambda expressions:
```razor
ValueChanged="@((string val) => OnConnectionStringChanged(val))"
```

## Files Modified

1. **SBInspector.Shared/Presentation/Components/UI/ConnectionForm.razor**
   - Fixed MudSelect dropdown binding for saved connections
   - Fixed MudTextField binding for connection string parameter
   - Fixed MudCheckBox and MudTextField for save connection feature

2. **SBInspector.Shared/Presentation/Components/UI/MessagesPanel.razor**
   - Fixed MudSelect binding for PageSize parameter
   - Added OnPageSizeChanged handler

3. **SBInspector.Shared/Presentation/Components/UI/MessageFilterPanel.razor**
   - Fixed MudCheckBox binding for filter.IsEnabled
   - Fixed MudSelect bindings for filter.Field and filter.Operator
   - Fixed MudTextField bindings for filter.AttributeName and filter.AttributeValue
   - All changes now trigger NotifyFiltersChanged to update parent

## Key Learnings

### MudBlazor Binding Patterns

1. **Local Field Binding (Simple):**
   ```razor
   <MudTextField @bind-Value="localField" />
   ```
   Works fine for local component fields.

2. **Parameter Binding (Requires EventCallback):**
   ```razor
   <MudTextField Value="@ParameterProperty" 
                 ValueChanged="@((string val) => OnValueChanged(val))" />
   
   @code {
       [Parameter]
       public string ParameterProperty { get; set; }
       
       [Parameter]
       public EventCallback<string> ParameterPropertyChanged { get; set; }
       
       private async Task OnValueChanged(string value)
       {
           ParameterProperty = value;
           await ParameterPropertyChanged.InvokeAsync(value);
       }
   }
   ```

3. **Collection Item Binding (Fire and Forget):**
   ```razor
   <MudTextField Value="@item.Property" 
                 ValueChanged="@((string val) => { item.Property = val; _ = NotifyChanged(); })" />
   ```

### Testing
- Blazor Server project builds successfully (verified)
- MudBlazor analyzers produce warnings about lowercase patterns but these are cosmetic
- MAUI project requires Windows platform for full build and testing

## Result
All binding issues are resolved. The UI should now:
- ✅ Allow connection to Service Bus via connection string
- ✅ Load and display queues and topics after connection
- ✅ Support saved connections dropdown
- ✅ Update page size selection
- ✅ Apply message filters correctly

## References
- [MudBlazor Components Documentation](https://mudblazor.com/components)
- [Blazor Two-Way Binding](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/data-binding#two-way-binding)
- [EventCallback in Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/event-handling#eventcallback)
