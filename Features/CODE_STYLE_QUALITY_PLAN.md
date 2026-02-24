# Feature Plan: Code Style Quality Improvements (No Feature Changes)

Status: Draft
Date: 2026-02-24
Owner: plan-expert

## Overview

Improve code style and quality without changing behavior. Focus on analyzers, nullability consistency, and eliminating silent error patterns.

## Goals

- [ ] Centralize style rules in .editorconfig
- [ ] Reduce code quality risks without feature changes
- [ ] Align nullable usage across projects
- [ ] Improve diagnostics for error handling patterns

## Findings Applied

- Mixed namespace style
- Silent catches
- Sync-over-async
- String literals for message state
- Inconsistent nullability

## Implementation Tasks

### Phase 1: Standards and Tooling

- [ ] Add or update .editorconfig with consistent style rules
- [ ] Centralize LangVersion, Nullable, AnalysisLevel in central props
- [ ] Decide namespace style (file-scoped vs block) and enforce

### Phase 2: Code Hygiene Pass

- [ ] Replace silent catches with logging or explicit handling
- [ ] Identify sync-over-async and convert to async paths where safe
- [ ] Replace string literals for message state with enums or constants
- [ ] Fix inconsistent nullability annotations and suppressions

### Phase 3: Validation

- [ ] Build with analyzers enabled
- [ ] Run tests
- [ ] Ensure no behavior changes (spot check critical flows)

## Risks

- Style changes can create large diffs; merge conflicts risk.
- Analyzer upgrades may require incremental adoption to avoid blocking builds.
- Converting sync-over-async may affect timing or exception flow.

## Dependencies

- Blazor Server source not present; cannot validate shared components there.

## Status Tracking

- [ ] Plan approved
- [ ] .editorconfig in place
- [ ] Namespace style aligned
- [ ] Silent catches addressed
- [ ] Sync-over-async addressed
- [ ] Message state literals addressed
- [ ] Nullability aligned
- [ ] Build validated
- [ ] Tests validated
