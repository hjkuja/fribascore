# Project Context

- **Owner:** @hjkuja
- **Project:** FribaScore — offline-first disc golf scorecard PWA
- **Stack:** React 19, TypeScript, Vite 7, Bun, IndexedDB (idb v8), React Router v7, ESLint 9, Bun test runner + React Testing Library + happy-dom
- **Repo root:** (repo root)
- **Frontend:** ui/
- **Docs:** docs/
- **Created:** 2026-04-03

## Key Architecture Points

- Offline-first: all data in IndexedDB, cloud sync is additive and non-blocking
- Single DB (`fribascore`, version 1) with stores: courses, rounds, players
- Client-side UUIDs for all entity IDs
- Seed data loaded on first run from `ui/src/data/dummyCourses.ts`
- PWA and .NET backend are planned but not yet started
- Retro-futurist design: dark charcoal, amber accents, Bebas Neue + Space Grotesk

## Learnings

<!-- Append new learnings below. -->
- Security sections added to all agent charters (2026-04-03). Scope: XSS/CSP for FE-2, input validation for BE-8, security test cases for QT-3, least-privilege/SHA-pinning for OPS-9, review gate for LD-7.
