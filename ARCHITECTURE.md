# SBInspector Architecture with Tauri

## Application Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Tauri Desktop App                        │
│                                                             │
│  ┌────────────────────────────────────────────────────┐   │
│  │           Webview Window                           │   │
│  │                                                    │   │
│  │  Displays: http://localhost:5000                  │   │
│  │                                                    │   │
│  │  ┌──────────────────────────────────────────┐    │   │
│  │  │  Blazor Server UI                        │    │   │
│  │  │  - Interactive Components                │    │   │
│  │  │  - Service Bus Management                │    │   │
│  │  │  - Message Operations                    │    │   │
│  │  └──────────────────────────────────────────┘    │   │
│  │                                                    │   │
│  └────────────────────────────────────────────────────┘   │
│                           ▲                                 │
│                           │ HTTP                            │
│                           │                                 │
│  ┌────────────────────────────────────────────────────┐   │
│  │   .NET Server Process (SBInspector.exe)            │   │
│  │                                                    │   │
│  │   Listening on: http://localhost:5000             │   │
│  │                                                    │   │
│  │   ┌──────────────────────────────────────────┐   │   │
│  │   │  ASP.NET Core + Blazor Server            │   │   │
│  │   │  - SignalR Connection                    │   │   │
│  │   │  - Interactive Rendering                 │   │   │
│  │   │  - Component State Management            │   │   │
│  │   └──────────────────────────────────────────┘   │   │
│  │                                                    │   │
│  │   ┌──────────────────────────────────────────┐   │   │
│  │   │  Application Services                    │   │   │
│  │   │  - ServiceBusService                     │   │   │
│  │   │  - MessageFilterService                  │   │   │
│  │   │  - StorageService                        │   │   │
│  │   └──────────────────────────────────────────┘   │   │
│  │                          │                         │   │
│  │                          ▼                         │   │
│  │   ┌──────────────────────────────────────────┐   │   │
│  │   │  Azure SDK                               │   │   │
│  │   │  - Azure.Messaging.ServiceBus            │   │   │
│  │   └──────────────────────────────────────────┘   │   │
│  │                                                    │   │
│  └────────────────────────────────────────────────────┘   │
│                                                             │
│  Managed by Tauri Rust Code                                │
│  - Auto-starts server on app launch                        │
│  - Stops server on app close                               │
│                                                             │
└─────────────────────────────────────────────────────────────┘
                           │
                           ▼
                ┌──────────────────────┐
                │  Azure Service Bus   │
                │  - Queues            │
                │  - Topics            │
                │  - Subscriptions     │
                └──────────────────────┘
```

## Startup Sequence

### Development Mode
```
1. Developer runs: dotnet watch run
2. .NET server starts on https://localhost:5000
3. Developer runs: npm run tauri dev
4. Tauri connects to devUrl: https://localhost:5000
5. Developer sees the app with hot reload
```

### Production Mode (MSI)
```
1. User double-clicks SBInspector.exe
2. Tauri startup code executes (src-tauri/src/lib.rs)
3. Tauri extracts bundled resources to temp directory
4. Tauri spawns .NET server process:
   - Executable: SBInspector (from resources)
   - Environment: ASPNETCORE_URLS=http://localhost:5000
5. Wait 2 seconds for server to initialize
6. Tauri webview loads index.html from frontendDist
7. index.html redirects to http://localhost:5000
8. Blazor Server establishes SignalR connection
9. User interacts with the app
10. User closes window → Tauri kills server process
```

## File Structure

```
SBInspector/
├── SBInspector/                  # Blazor Server Application
│   ├── Program.cs                # Server configuration
│   ├── SBInspector.csproj        # Project file
│   ├── wwwroot/                  # Static files
│   │   ├── index.html            # ✨ NEW: Tauri entry point
│   │   ├── app.css
│   │   └── lib/
│   ├── Presentation/
│   │   └── Components/
│   │       ├── App.razor         # Root component
│   │       └── Pages/
│   ├── Application/
│   │   └── Services/
│   ├── Infrastructure/
│   │   └── ServiceBus/
│   └── Core/
│       └── Domain/
│
├── src-tauri/                    # Tauri Application
│   ├── src/
│   │   ├── main.rs
│   │   └── lib.rs                # ✨ UPDATED: Server management
│   ├── tauri.conf.json           # ✨ UPDATED: Resource bundling
│   └── Cargo.toml
│
├── dist/                         # Build Output (generated)
│   ├── SBInspector               # .NET server executable
│   ├── *.dll                     # Dependencies
│   └── wwwroot/
│       ├── index.html            # Copied from source
│       └── *.css                 # Static assets
│
└── Documentation
    ├── IMPLEMENTATION_NOTES.md
    ├── TAURI_BLAZOR_SETUP.md
    └── TAURI_FIX_SUMMARY.md
```

## Communication Flow

```
User Action → Blazor UI Component
              ↓
         SignalR Protocol (over HTTP)
              ↓
         Blazor Server Runtime
              ↓
         Application Services
              ↓
         Azure Service Bus SDK
              ↓
         Azure Service Bus Cloud
```

## Why Sidecar Pattern?

### ✅ Advantages
- Minimal code changes
- Full .NET functionality
- Azure SDK compatibility
- Server-side performance
- Easy to debug

### ❌ Alternative: WebAssembly
- Major refactoring required
- Azure SDK may not work in browser
- Loss of server-side features
- Larger bundle size

## Security Considerations

- Server runs on localhost only
- No external network exposure
- Server process lifetime tied to Tauri window
- Clean process termination on app exit

## Performance

- Fast startup (~2 seconds for server initialization)
- Native desktop performance
- SignalR for real-time updates
- Server-side rendering reduces client load
