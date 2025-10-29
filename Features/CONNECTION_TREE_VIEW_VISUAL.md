# Connection Tree View - Visual Layout

## Left Navigation Menu Structure

```
┌─────────────────────────────────────┐
│   SEBInspector                      │ ← App Title Bar
├─────────────────────────────────────┤
│                                     │
│  🔌 CONNECTIONS                     │ ← Section Header
│  ┌─────────────────────────────────┤
│  │ 🔌 Dev Environment          ●   │ ← Active Connection (highlighted blue)
│  │                                 │    with green pulsing status dot
│  ├─────────────────────────────────┤
│  │ 🔌 Production                   │ ← Saved Connection (clickable)
│  │                                 │
│  └─────────────────────────────────┤
│                                     │
├─────────────────────────────────────┤ ← Border Separator
│                                     │
│  🏠 Service Bus Inspector           │ ← Main Navigation Link
│                                     │
│  📄 Message Templates               │ ← Main Navigation Link
│                                     │
└─────────────────────────────────────┘
```

## Connection States

### No Connections Saved
```
┌─────────────────────────────────────┐
│  🔌 CONNECTIONS                     │
│  ┌─────────────────────────────────┤
│  │  No saved connections           │ ← Empty state message
│  └─────────────────────────────────┤
└─────────────────────────────────────┘
```

### Multiple Connections (One Active)
```
┌─────────────────────────────────────┐
│  🔌 CONNECTIONS                     │
│  ┌─────────────────────────────────┤
│  │ 🔌 Production              ●   │ ← Active (blue background + green dot)
│  ├─────────────────────────────────┤
│  │ 🔌 Dev Environment              │ ← Inactive (hover shows light bg)
│  ├─────────────────────────────────┤
│  │ 🔌 Staging                      │ ← Inactive
│  ├─────────────────────────────────┤
│  │ 🔌 Test                         │ ← Inactive
│  └─────────────────────────────────┤
└─────────────────────────────────────┘
```

### Loading State
```
┌─────────────────────────────────────┐
│  🔌 CONNECTIONS                     │
│  ┌─────────────────────────────────┤
│  │         ⟳                       │ ← Loading spinner
│  └─────────────────────────────────┤
└─────────────────────────────────────┘
```

## Interaction Behaviors

### Click on Inactive Connection
```
Before:                          After:
┌──────────────────┐            ┌──────────────────┐
│ 🔌 Production    │            │ 🔌 Production  ● │ ← Now connected
└──────────────────┘            └──────────────────┘
                                 (blue background)

Result: Connects to that Service Bus namespace
```

### Click on Active Connection
```
Before:                          After:
┌──────────────────┐            ┌──────────────────┐
│ 🔌 Production  ● │            │ 🔌 Production    │ ← Disconnected
└──────────────────┘            └──────────────────┘
 (blue background)               (normal appearance)

Result: Disconnects from Service Bus
```

### Hover Effect
```
Normal State:                   Hover State:
┌──────────────────┐            ┌──────────────────┐
│ 🔌 Dev Env       │   →→→     │ 🔌 Dev Env       │
└──────────────────┘            └──────────────────┘
(gray text)                     (white text + light bg)
```

## Color Palette

- **Background (Normal)**: Transparent
- **Background (Hover)**: rgba(255, 255, 255, 0.1) - Light transparent white
- **Background (Active)**: rgba(0, 123, 255, 0.25) - Transparent blue
- **Text (Normal)**: #d7d7d7 - Light gray
- **Text (Hover/Active)**: #ffffff - White
- **Status Indicator**: #28a745 - Green with pulsing animation
- **Border Separator**: Secondary border color

## Component Hierarchy

```
MainLayout.razor
└── NavMenu.razor
    └── ConnectionTreeView.razor
        ├── Connection 1 (clickable div)
        ├── Connection 2 (clickable div)
        └── Connection N (clickable div)
```

## Data Flow

```
User Clicks Connection
        ↓
ConnectionTreeView raises OnConnect event
        ↓
NavMenu forwards to OnConnectionSelected
        ↓
MainLayout.HandleConnectionSelected
        ↓
ServiceBusService.ConnectAsync
        ↓
ConnectionStateService.CurrentConnectionString updated
        ↓
ConnectionTreeView receives state change event
        ↓
UI updates to show active state
```

## Key Features Illustrated

1. **Visual Hierarchy**: Clear separation between connections section and main navigation
2. **Active State**: Blue background + green pulsing dot makes it obvious which connection is active
3. **Hover Feedback**: Light background on hover provides immediate visual feedback
4. **Icon Consistency**: Plug icon (🔌/bi-plug) used for all connections
5. **Sorting**: Most recently used connections appear at the top
6. **Tooltips**: Hover text explains the action ("Click to connect" or "Click to disconnect")
