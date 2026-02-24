# Feature Plan: .NET 10 Migration

Status: Draft
Date: 2026-02-24
Owner: plan-expert

## Overview

Migrate SBInspector projects to .NET 10 while keeping behavior unchanged. This includes MAUI and test projects currently targeting net9.0-windows10.0.19041.0.

## Goals

- [ ] Update all applicable projects to net10.0 (and net10.0-windows10.0.19041.0 where required)
- [ ] Add global.json to pin SDK
- [ ] Centralize LangVersion, Nullable, and AnalysisLevel
- [ ] Ensure build and tests pass on updated framework

## Findings Applied

- MAUI and tests currently target net9.0-windows10.0.19041.0; update to net10.0.
- Add global.json and central props for LangVersion/Nullable/AnalysisLevel.

## Implementation Tasks

### Phase 1: Inventory and Decisions

- [ ] Confirm which projects should target net10.0 vs net10.0-windows10.0.19041.0
- [ ] Confirm if any multi-targeting is required

### Phase 2: SDK and Project Updates

- [ ] Add global.json to pin .NET 10 SDK
- [ ] Add central props (e.g., Directory.Build.props) for LangVersion, Nullable, AnalysisLevel
- [ ] Update target frameworks in MAUI project(s)
- [ ] Update target frameworks in test project(s)

### Phase 3: Validation

- [ ] Build solution with .NET 10 SDK
- [ ] Run tests
- [ ] Review warnings or analyzer changes introduced by new SDK

## Risks

- .NET 10 may introduce analyzer behavior changes causing warnings or errors.
- MAUI platform-specific constraints may require additional changes for net10.0-windows.
- Missing Blazor Server project limits validation coverage.

## Dependencies

- Blazor Server source not present; must be validated in its own repository or location.

## Status Tracking

- [ ] Plan approved
- [ ] Target frameworks updated
- [ ] global.json added
- [ ] Central props added
- [ ] Build validated
- [ ] Tests validated
