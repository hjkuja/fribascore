<!-- Frontend appends project-specific learnings here. Never cleared. -->

[2026-04-03] [LEARNING] Use `toLocaleString('en-GB', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit', hour12: false })` for round timestamps so output stays stable across Windows and Linux.
Source: legacy decisions record deleted during migration; legacy frontend history record deleted during migration; `ui/src/pages/HistoryPage.tsx`; `ui/src/pages/RoundSummary.tsx`

[2026-04-04] [LEARNING] Prefer tokens from `ui/src/styles/design-system.css`; keep `--main-accent-color` only as a compatibility alias and write new UI against `--accent`.
Source: legacy decisions record deleted during migration; legacy frontend history record deleted during migration; `ui/src/styles/design-system.css`

[2026-04-21] [LEARNING] Keep route files thin and push reusable UI into component folders with colocated `.tsx`, `.css`, and `.test.tsx` files.
Source: `docs/development/contributing.md`; `ui/src/app/AppRoutes.tsx`
