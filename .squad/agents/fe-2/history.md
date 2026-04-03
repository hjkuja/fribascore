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

<!-- Append new learnings below. -->
