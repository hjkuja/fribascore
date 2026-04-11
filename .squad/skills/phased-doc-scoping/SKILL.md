---
name: "phased-doc-scoping"
description: "Keep current-phase docs aligned to the active issue while documenting later phases as constraints and decision gates only"
domain: "documentation"
confidence: "high"
source: "earned"
---

## Context
Use this when documentation covers a current implementation phase plus a later possible phase. It prevents the current contract from drifting and prevents future planning notes from reading like approved implementation.

## Patterns
- Mirror the active issue's endpoints, payload expectations, and success/failure behavior closely in Phase 1 docs.
- State future-phase constraints, triggers, and open questions without committing packages, tables, endpoints, or flows.
- Keep future work in a separate issue draft that is explicitly blocked by the current implementation issue.
- Prefer phrases like "evaluate if needed" and "planning direction only" over candidate-specific promises.

## Examples
- `docs\api\overview.md` — Phase 1 auth scope matches issue #26 exactly.
- `docs\architecture\auth.md` — future auth expansion documented as readiness constraints, not implementation detail.
- `.squad\decisions\inbox\ISSUE-DRAFT-phase2-oidc.md` — future issue kept separate from #26 and planning-oriented.

## Anti-Patterns
- Letting `/auth/me` response fields drift away from the active issue.
- Documenting speculative packages or database changes as if already approved.
- Mixing future planning work into the current implementation issue.
