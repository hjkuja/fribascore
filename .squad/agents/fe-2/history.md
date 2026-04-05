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

### Issue #16 — CSS Design System (2026-04-04)

**Status:** ✅ Complete
**PR:** #32

**Files Created:**
- `ui/src/styles/design-system.css` — Full CSS custom property palette implementing the retro-futurist design system from `docs/specs/ui-design.md`

**Files Modified:**
- `ui/index.html` — Added Google Fonts preconnect + stylesheet (Bebas Neue + Space Grotesk)
- `ui/src/main.tsx` — Imports `design-system.css` before `index.css` for global availability
- `ui/src/index.css` — Updated `:root` to reference `var(--font-body)`, `var(--text)`, `var(--bg)` instead of hardcoded values; removed duplicated `--main-accent-color`
- `ui/src/app/AppLayout.css` — `#242424` → `var(--bg)` for header and side-menu backgrounds
- `ui/src/components/HoleScore/HoleScore.css` — `var(--main-accent-color)` → `var(--accent)`
- `ui/src/components/PlayerSelectModal/PlayerSelectModal.css` — `#1a1a1a` → `var(--bg-deep)`, all `var(--main-accent-color)` → `var(--accent)`
- `ui/src/components/PlayersManagement/PlayersManagement.css` — `#242424` → `var(--bg)`
- `ui/src/pages/RoundSummary.css` — `#ddd` border → `var(--glass-border)`, `#242424` → `var(--bg)`

**Design System Covers:**
- Colour palette: `--bg`, `--bg-deep`, `--accent`, `--accent-dim`, `--accent-glow`, `--accent-tint`, `--text`, `--text-muted`, `--glass-bg`, `--glass-border`
- Typography: `--font-display` (Bebas Neue), `--font-body` (Space Grotesk), type scale variables
- Spacing: `--max-width`, section/hero padding, card/feature gap tokens
- Border radius: `--radius-card` (10px), `--radius-button` (3px sharp), `--radius-badge`, `--radius-input`
- Shadows/Effects: `--shadow-card-hover`, `--shadow-btn-primary`, `--shadow-btn-outline`
- Transitions: `--transition-card`, `--transition-btn`, `--transition-base`
- Nav: `--nav-height`, `--nav-bg`
- Keyframes: `fadeIn`, `float`, `pulse-ring`, `blink`
- Background textures: `body::before` amber grid, `body::after` film-grain
- Utility classes: `.card`, `.btn-primary`, `.btn-outline`, `.badge`, `.section-label`, `.section-title`, `.retro-divider`, `.orb`
- Backward-compat alias: `--main-accent-color: var(--accent)`

**Key Pattern:** When replacing the old `--main-accent-color`, define it as an alias in the design system rather than removing it, to avoid cascading breakage across existing components. Then update components to use `--accent` as time allows.
### Dark Theme Enforcement — Remove Light Mode Overrides (2026-04-03)

**Status:** ✅ Complete  
**Branch:** squad/16-css-design-system

**Problem:** The app had `@media (prefers-color-scheme: light)` blocks in five CSS files that overrode the dark design system when the OS was set to light mode. Additionally, raw `rgba()` and hex values in several components bypassed the design token system.

**Files fixed:**
- `ui/src/index.css` — removed light mode block, changed `color-scheme: light dark` → `color-scheme: dark`
- `ui/src/app/AppLayout.css` — removed `#ffffff` header/sidebar override block
- `ui/src/components/CourseList/CourseList.css` — replaced raw rgba values with `--glass-bg`, `--glass-border`, `--accent-tint`, `--accent-dim`, `--text-muted`; removed light mode block
- `ui/src/pages/HistoryPage.css` — replaced raw rgba borders with `--glass-border`; removed light mode block
- `ui/src/components/PlayerSelectModal/PlayerSelectModal.css` — removed entire light mode override block (dark values already correct)
- `ui/src/components/PlayersManagement/PlayersManagement.css` — replaced hardcoded `#df0000`/`#ff8888` with `--accent`/`--accent-dim`

**Welcome redesign:**
Replaced placeholder text with a proper app home screen: large Bebas Neue display title, amber tagline, `.btn-primary` CTA to /courses, secondary link to /history. Mobile-first layout centered vertically in viewport.

**Key Learning:** Setting `color-scheme: dark` on `:root` (not `light dark`) is the correct signal for a dark-only app — it tells browsers to use dark scrollbars, form controls, and default styles without needing any `prefers-color-scheme` overrides.

