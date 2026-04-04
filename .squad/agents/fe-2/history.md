# Project Context

- **Owner:** @hjkuja
- **Project:** FribaScore — offline-first disc golf scorecard PWA
- **Stack:** React 19, TypeScript, Vite 7, Bun, IndexedDB (idb v8), React Router v7, ESLint 9, Bun test runner + React Testing Library + happy-dom
- **Repo root:** (repo root)
- **Frontend:** ui/
- **Docs:** docs/
- **Created:** 2026-04-03

## Key Frontend Points

- Components in `ui/src/components/{Name}/` — each has `.tsx` + `.css` + `.test.tsx`
- Pages in `ui/src/pages/` — thin wrappers around components
- Design system: dark charcoal bg (`--bg: #242424`), amber accent (`--accent: #c77c27`)
- Fonts: `Bebas Neue` (display/headings) + `Space Grotesk` (body/UI), loaded from Google Fonts
- BEM-like class naming: `component-name__element`
- No hardcoded hex — always use CSS custom properties
- PWA not yet started: manifest, service worker, install prompt all pending
- Key routes: `/`, `/courses`, `/courses/:id`, `/courses/:id/start-round`, `/rounds/:id/score`, `/rounds/:id/summary`, `/history`, `/settings`, `/__admin` (dev only)

## Learnings

### Issue #10 — DateTime Display Fix (2026-04-03)

**Status:** ✅ Complete  
**PR:** #14 (open)

Fixed date-time display in HistoryPage and RoundSummary by replacing `toLocaleDateString()` with `toLocaleString()`. Uses explicit format options to ensure consistent locale-aware formatting:

```typescript
new Date(date).toLocaleString(undefined, {
  day: '2-digit',
  month: '2-digit',
  year: 'numeric',
  hour: '2-digit',
  minute: '2-digit'
})
```

Allows users to distinguish rounds played on same day by displaying both date and time. No breaking changes; display-only modification.

### Locale Standardization — DateTime Formatting (2026-04-03)

**Status:** ✅ Complete

**Problem:** HistoryPage.tsx and RoundSummary.tsx used `toLocaleString(undefined, {...})` which picks up the OS system locale. This caused inconsistent output between Finnish Windows (e.g., `27.3.2026, 14:32`) and Linux CI environments.

**Solution:** Standardized on `'en-GB'` locale with `hour12: false` for 24-hour format:
```typescript
new Date(round.date).toLocaleString('en-GB', {
  day: '2-digit', month: '2-digit', year: 'numeric',
  hour: '2-digit', minute: '2-digit',
  hour12: false
})
```

**Output:** Consistent `27/03/2026, 14:32` format everywhere.

**Test Updates:** Updated locale-sensitive assertions in both test files to expect en-GB format instead of locale-variable regex patterns.

**Key Learning:** Always specify an explicit locale when locale-sensitive formatting affects test assertions or CI/local reproducibility. Use a neutral locale like 'en-GB' for consistent cross-platform behavior.

<!-- Append new learnings below. -->
