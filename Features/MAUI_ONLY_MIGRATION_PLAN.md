# MAUI-Only Migration Plan

## Goals

- Remove the Blazor Server app and shared Razor class library projects.
- Move shared app logic, components, and assets into the MAUI project.
- Keep tests, but make them MAUI-focused.
- Update solution, project references, namespaces/usings, and docs.
- Build at the end and capture results.

## Checklist

- [x] Inventory shared code/assets and MAUI usage.
- [x] Move SBInspector.Shared code and assets into SEBInspector.Maui.
- [x] Update namespaces/usings to MAUI-only root.
- [x] Update MAUI entry points and static asset references.
- [x] Remove SBInspector and SBInspector.Shared projects from solution.
- [x] Update SBInspector.Tests references and namespaces.
- [x] Update README and Features docs for MAUI-only layout.
- [ ] Build solution and record results.

## Status

- Date: 2026-02-23
- Owner: GitHub Copilot
- Notes: Migration steps completed; build/test validation pending. Build failed with NU1201 because SBInspector.Tests targeted net9.0 while referencing SEBInspector.Maui (net9.0-windows10.0.19041). Updated SBInspector.Tests to target net9.0-windows10.0.19041.0; re-run build/test. Current build errors: missing MudBlazor/DataProtection references and related types, Application namespace conflict, ColumnDefinition ambiguity. Fixes applied: added MudBlazor/DataProtection packages, ensured MudBlazor/ColumnDefinition alias imports, qualified App base class, disambiguated ColumnDefinition, replaced `IMudDialogInstance` with `MudDialogInstance`, updated `MessageDetailsModal` to use `DialogContent`/`DialogActions`, qualified `Application.Current` usages in MAUI services, and aliased `MudBlazor.Colors` to resolve `Colors` ambiguity. Another build error found: MainPage.xaml still referenced SBInspector.Shared `Routes`. Fixed by switching XAML namespace to MAUI `Presentation.Components` and updating the RootComponent to `Routes`. Fix applied to prevent duplicate appicon resizetizer errors in tests by removing custom wwwroot appicon metadata in the MAUI project. Resizetizer duplicate icon fix: replaced conditional `TargetFrameworks` concatenation with a single `TargetFramework` in the MAUI project. Test cleanup: added global usings for Xunit/Bunit and removed Blazor-only folder picker tests, including deleting the FolderPickerService test file.
- Build/Test: Not run in this change (per request).

## Next Steps

- Run `dotnet build -f net9.0-windows10.0.19041.0` in SEBInspector.Maui.
- Run `dotnet test SBInspector.Tests` from the repo root.
- Update this plan with results (pass/fail and any errors).
