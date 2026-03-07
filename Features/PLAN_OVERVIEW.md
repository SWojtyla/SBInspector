# SEBInspector Plan Overview

Status: Draft
Date: 2026-02-24
Owner: plan-expert

## Scope

This overview tracks three coordinated initiatives:

1. .NET 10 migration
2. Code style quality improvements (no feature changes)
3. UI/UX responsiveness review and improvements

## Dependencies

- Blazor Server source is not present in this workspace. This is a dependency for full validation and any shared component updates.
- Dist folder includes a WASM build; confirm ownership and source of truth before changing UI assets.

## Global Risks

- Missing Blazor Server project may hide build or runtime issues in shared components.
- Cross-targeting .NET 10 could surface MAUI or test framework incompatibilities.
- Style-only changes can introduce behavior changes if analyzers or nullable warnings are elevated to errors.
- UI fixes could diverge between MAUI and web if CSS entry points are ambiguous.

## Execution Order (Suggested)

- Phase 1: .NET 10 migration foundations (global.json, central props, target frameworks)
- Phase 2: Code style quality improvements (editorconfig, analyzers, refactors)
- Phase 3: UI/UX responsiveness audit and fixes (layout, CSS entry points)

## Feature Plans

- .NET 10 Migration: Features/NET10_MIGRATION_PLAN.md
- Code Style Quality Improvements: Features/CODE_STYLE_QUALITY_PLAN.md
- UI/UX Responsiveness: Features/UI_UX_RESPONSIVENESS_PLAN.md

## Status Tracking

- [ ] .NET 10 migration plan ready
- [ ] Code style quality plan ready
- [ ] UI/UX responsiveness plan ready
- [ ] Cross-feature dependencies reviewed
- [ ] Validation steps agreed
