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
- Auth technology & storage reviewed with hjkuja (2026-04-04). Recommendation: ASP.NET Core Identity (framework approach; PostgreSQL compat; offline sync pattern tested) + HttpOnly cookies (XSS protection via HttpOnly flag; SameSite=Strict for CSRF; automatic token attachment; secondary option: sessionStorage if HttpOnly blocked; never localStorage for tokens). Decision record written to `.squad/decisions/inbox/ld-7-auth-storage-decision.md` pending confirmation. This unblocks backend API work.
- Security sections added to all agent charters (2026-04-03). Scope: XSS/CSP for FE-2, input validation for BE-8, security test cases for QT-3, least-privilege/SHA-pinning for OPS-9, review gate for LD-7.
- Plan assessment completed (2026-04-03). ROADMAP has 29 pending items across 6 areas. UI/Design and PWA are ready to start. Backend API blocked on auth+DB tech decision. Cloud Sync blocked on Backend decisions plus needs queue schema and JWT storage strategy. Testing area needs a concrete coverage list before QT-3 can act. `docs/architecture/offline-strategy.md` referenced but does not exist — should be created before sync work begins. Manifest colours in `pwa.md` contradict the design spec and must be corrected before PWA implementation.
- GitHub issues created for ROADMAP (2026-04-04). Labels `ui-design` (#C77C27) and `backend` (#0075CA) created. 15 issues opened: UI/Design #16–#24 (CSS design system, Google Fonts, background effects, glassmorphism card, button system, nav bar, home hero, start round, round scoring), Backend #25–#29 (scaffold, auth endpoints, courses API, players API, rounds API), Other #30 (app icon/favicon). ROADMAP.md updated with issue links in all Issue columns. PWA skipped per Hannu's directive. Cloud Sync and Testing skipped (blocked/no spec). Committed as `roadmap: link GitHub issues to ROADMAP items`.
