# Tauri Configuration Fix - Quick Reference

## Problem
When creating an MSI installer, Tauri reported: "index.html asset was not found"

## Root Cause
- The app uses **Blazor Server** mode (requires a running .NET server)
- Tauri's `frontendDist` expects **static files** (HTML/CSS/JS)
- No index.html existed in the wwwroot directory

## Solution Applied

### 1. Added index.html
Created `/SBInspector/wwwroot/index.html` that redirects to the local server:
```html
<meta http-equiv="refresh" content="0; url=http://localhost:5000" />
```

### 2. Updated Tauri Configuration
- `frontendDist`: Points to `../dist/wwwroot` (contains index.html and static assets)
- `bundle.resources`: Includes `../dist/*` (all server files)

### 3. Modified Tauri App Startup
The Rust code now:
- Starts the .NET server on `localhost:5000` in production
- Redirects the webview to the server
- Manages the server lifecycle

## Result
✅ The "index.html not found" error is resolved
✅ The app works in both development and production (MSI)
✅ No need to convert to WebAssembly
✅ Full Blazor Server functionality is preserved

## Files Changed
1. `SBInspector/wwwroot/index.html` - New file
2. `src-tauri/src/lib.rs` - Server startup logic
3. `src-tauri/tauri.conf.json` - Resource bundling configuration

## What to Put in frontendDist?
Answer: `../dist/wwwroot` - This directory contains:
- `index.html` (the file Tauri requires)
- CSS files (app.css, bootstrap, etc.)
- Other static assets (favicon, etc.)

The .NET server executable and DLLs are bundled separately as resources.

## Is It Normal the App Works Without index.html?
Yes, in **development mode** it's normal because:
- Development uses `devUrl` which connects directly to your running .NET server
- No static files are needed
- You run `dotnet watch run` separately

However, **production mode** (MSI installer) requires:
- The index.html file (✅ now added)
- The server to be bundled and auto-started (✅ now configured)
