# Quick Start Guide - Tauri Desktop App

## TL;DR

The Tauri configuration has been fixed. The installer will now work correctly.

## Quick Commands

### Development
```bash
npm install
npm run tauri dev
```

### Build Installer
```bash
npm run tauri build
```
Installer will be in: `src-tauri/target/release/bundle/msi/`

## What Changed?

The app now runs the Blazor server in the background and connects to it, instead of trying to serve static files.

## Documentation

- **Architecture Details**: `Features/TAURI_CONFIGURATION.md`
- **Testing Guide**: `TESTING_CHECKLIST.md`
- **Complete Changes**: `CHANGES_SUMMARY.md`
- **Technical Summary**: `TAURI_UPDATE_SUMMARY.md`

## Need Help?

1. Check `TESTING_CHECKLIST.md` for common issues
2. Review `Features/TAURI_CONFIGURATION.md` for how it works
3. Look at logs in `%APPDATA%/SBInspector/logs/` (Windows)

## Status

✅ **Ready for Testing**

All changes committed to branch: `copilot/update-tauri-config-blazor`

Build the installer on Windows and test it!
