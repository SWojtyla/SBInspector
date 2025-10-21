# MudBlazor Integration Guide

## Overview

This document describes the integration of MudBlazor components into the SBInspector application, replacing Bootstrap components while maintaining the same functionality and user experience.

## What is MudBlazor?

MudBlazor is a comprehensive Material Design component library for Blazor applications. It provides a rich set of pre-built components that follow Google's Material Design guidelines, offering a modern and consistent UI/UX.

## Changes Made

### 1. Package Installation

Added MudBlazor NuGet package (version 8.13.0) to the project:

```bash
dotnet add package MudBlazor
```

### 2. Service Registration

Updated `Program.cs` to register MudBlazor services:

```csharp
using MudBlazor.Services;
builder.Services.AddMudServices();
```

### 3. Layout Updates

#### App.razor
- Replaced Bootstrap CSS with MudBlazor CSS and fonts
- Added MudBlazor JavaScript
- Removed Bootstrap Icons (MudBlazor includes Material Icons)

#### MainLayout.razor
- Replaced custom layout with MudBlazor layout components:
  - `MudThemeProvider` - Provides theming
  - `MudPopoverProvider` - Manages popovers
  - `MudDialogProvider` - Manages dialogs
  - `MudSnackbarProvider` - Provides snackbar notifications
  - `MudLayout` - Main layout container
  - `MudAppBar` - Top application bar
  - `MudDrawer` - Side navigation drawer
  - `MudMainContent` - Main content area

#### NavMenu.razor
- Replaced Bootstrap navigation with `MudNavMenu` and `MudNavLink`

### 4. Component Conversions

#### ConnectionForm.razor
- **Card** → `MudCard` with `MudCardContent`
- **Form inputs** → `MudTextField`, `MudSelect`
- **Buttons** → `MudButton` with variants and icons
- **Alerts** → `MudAlert` with severity levels
- **Checkboxes** → `MudCheckBox`

#### MessageFilterPanel.razor
- **Card panels** → `MudPaper` and `MudCard`
- **Form controls** → `MudSelect`, `MudTextField`
- **Grid layout** → `MudGrid` with `MudItem`
- **Icons** → MudBlazor Material Icons
- Resolved naming conflict between `SBInspector.Core.Domain.FilterOperator` and `MudBlazor.FilterOperator` using type alias

#### ConfirmationModal.razor
- **Bootstrap modal** → `MudDialog` with `TitleContent`, `DialogContent`, and `DialogActions`
- **Buttons** → `MudButton` with colors and variants
- Mapped Bootstrap icon classes to Material Icons

#### OperationLoadingOverlay.razor
- **Custom overlay** → `MudOverlay` with `MudPaper`
- **Bootstrap spinner** → `MudProgressCircular`

#### BackgroundOperationPanel.razor
- **Custom styled panel** → `MudPaper` with `MudStack` layout
- **Progress bar** → `MudProgressLinear`
- **Buttons** → `MudIconButton`

#### Home.razor (Partial)
- **Heading** → `MudText` with typography
- **Buttons** → `MudButton` with variants and icons
- **Alerts** → `MudAlert` with severity and close functionality

### 5. Key Differences and Patterns

#### Icon Mapping
Bootstrap Icons → Material Icons:
- `bi-trash` → `Icons.Material.Filled.Delete`
- `bi-check-lg` → `Icons.Material.Filled.Check`
- `bi-exclamation-triangle` → `Icons.Material.Filled.Warning`
- `bi-arrow-counterclockwise` → `Icons.Material.Filled.Refresh`
- `bi-funnel` → `Icons.Material.Filled.FilterList`

#### Button Classes to MudBlazor Colors
- `btn-primary` → `Color.Primary`
- `btn-danger` → `Color.Error`
- `btn-success` → `Color.Success`
- `btn-warning` → `Color.Warning`

#### Two-way Binding Pattern
When using `ValueChanged` with MudBlazor components, cannot use `@bind-Value`:

```csharp
// Instead of: @bind-Value="filter.Field" ValueChanged="..."
// Use:
Value="filter.Field" 
ValueChanged="@(async (FilterField val) => { filter.Field = val; await NotifyFiltersChanged(); })"
```

## Remaining Components to Convert

The following components still use Bootstrap and need to be converted to MudBlazor:

### Large Table/List Components
1. **EntityTreeView.razor** - Tree view for queues, topics, and subscriptions
2. **MessageListTable.razor** - Data table for messages with sorting
3. **QueueListTable.razor** - List of queues
4. **TopicListTable.razor** - List of topics  
5. **SubscriptionListPanel.razor** - List of subscriptions

### Large Modal/Form Components
6. **MessageDetailsModal.razor** - Modal showing message details with JSON formatting
7. **SendMessageModal.razor** - Complex form modal for sending messages with properties

### Display Panels
8. **EntityDetailsPanel.razor** - Details panel showing queue/topic/subscription info
9. **MessagesPanel.razor** - Main panel for displaying messages with toolbar

## Conversion Guidelines for Remaining Components

### For Table Components
Replace with `MudTable<T>` or `MudDataGrid<T>`:
- Use `MudTable` for simple tables
- Use `MudDataGrid` for advanced features (sorting, filtering, pagination)
- Define columns using `<PropertyColumn>` or `<TemplateColumn>`

Example pattern:
```razor
<MudTable Items="@messages" Hover="true" Striped="true" Dense="true">
    <HeaderContent>
        <MudTh>Message ID</MudTh>
        <MudTh>Subject</MudTh>
        <MudTh>Actions</MudTh>
    </HeaderContent>
    <RowTemplate>
        <MudTd>@context.MessageId</MudTd>
        <MudTd>@context.Subject</MudTd>
        <MudTd>
            <MudIconButton Icon="@Icons.Material.Filled.Visibility" />
        </MudTd>
    </RowTemplate>
</MudTable>
```

### For Modal Components
Use `MudDialog` structure:
```razor
@if (IsVisible)
{
    <MudDialog>
        <TitleContent>...</TitleContent>
        <DialogContent>...</DialogContent>
        <DialogActions>...</DialogActions>
    </MudDialog>
}
```

### For Tree View
Use `MudTreeView<T>`:
```razor
<MudTreeView Items="@items">
    <ItemTemplate>
        <MudTreeViewItem Value="@context" Icon="@GetIcon(context)">
            @context.Name
        </MudTreeViewItem>
    </ItemTemplate>
</MudTreeView>
```

## Theme Customization

MudBlazor uses Material Design with a default purple/blue theme. The theme can be customized in `MainLayout.razor` or `App.razor`:

```razor
<MudThemeProvider Theme="@customTheme" />

@code {
    private MudTheme customTheme = new MudTheme()
    {
        Palette = new PaletteLight()
        {
            Primary = "#1b6ec2",
            Secondary = "#26b050",
            // ... other colors
        }
    };
}
```

## Benefits of MudBlazor

1. **Modern UI** - Material Design provides a contemporary, professional look
2. **Rich Component Library** - Comprehensive set of components for all common UI needs
3. **Responsive** - Built-in responsive behavior for mobile and desktop
4. **Accessibility** - ARIA attributes and keyboard navigation support
5. **Performance** - Optimized for Blazor's rendering model
6. **Active Development** - Regular updates and bug fixes
7. **Documentation** - Excellent documentation with live examples

## Testing

After converting each component:
1. Build the project to check for compilation errors
2. Run the application and test the component functionality
3. Verify that the UI looks correct and is responsive
4. Test all interactive features (buttons, forms, dialogs)
5. Check browser console for any JavaScript errors

## References

- [MudBlazor Official Documentation](https://mudblazor.com/)
- [MudBlazor GitHub Repository](https://github.com/MudBlazor/MudBlazor)
- [Material Design Guidelines](https://material.io/design)
