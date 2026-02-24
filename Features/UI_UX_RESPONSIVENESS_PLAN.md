# Feature Plan: UI/UX Responsiveness Review and Improvements

Status: Draft
Date: 2026-02-24
Owner: plan-expert

## Overview

Audit and improve UI responsiveness across shared components and MAUI/web surfaces without altering feature behavior. Focus on layout issues and CSS entry points.

## Goals

- [ ] Improve responsiveness of split panels and cards
- [ ] Fix flexbox overflow issues
- [ ] Clarify CSS entry points
- [ ] Verify impact across MAUI and web assets where applicable

## Findings Applied

- Flex layout missing min-width: 0
- Split-panel fixed widths and min-height
- Fixed min widths in cards
- CSS entry points ambiguous
- dist/ contains WASM build
- Blazor Server source missing

## Implementation Tasks

### Phase 1: Audit

- [ ] Identify primary CSS entry points and ownership
- [ ] Inventory layouts with fixed widths or min heights
- [ ] Catalog flex containers missing min-width: 0 on children
- [ ] Determine whether dist/ is source or build output

### Phase 2: Layout Fixes

- [ ] Add min-width: 0 to flex items to prevent overflow
- [ ] Replace fixed split-panel widths with responsive sizing
- [ ] Remove hard min widths in cards where safe
- [ ] Ensure consistent CSS imports across entry points

### Phase 3: Validation

- [ ] Manual responsive checks at common breakpoints
- [ ] Verify MAUI and web UI consistency
- [ ] Confirm no changes to behavior or feature flow

## Risks

- Unknown source of CSS entry points could cause changes to be overwritten.
- Missing Blazor Server source prevents full verification.
- dist/ assets may be auto-generated; changes should target source files only.

## Dependencies

- Blazor Server source not present; cannot validate shared components there.

## Status Tracking

- [ ] Plan approved
- [ ] CSS entry points confirmed
- [ ] Flex overflow fixes applied
- [ ] Split-panel responsiveness applied
- [ ] Card sizing adjusted
- [ ] Responsive validation complete
