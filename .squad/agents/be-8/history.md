# Project Context

- **Owner:** @hjkuja
- **Project:** FribaScore — offline-first disc golf scorecard PWA
- **Stack:** React 19, TypeScript, Vite 7, Bun, IndexedDB (idb v8), React Router v7. Planned backend: ASP.NET Core Web API (C#).
- **Repo root:** (repo root)
- **Frontend:** ui/
- **Docs:** docs/
- **Created:** 2026-04-03

## Key Data Layer Points

- DB file: `ui/src/utils/db.ts` — single gateway for all storage access
- DB name: `fribascore`, version 1
- Stores: `courses`, `rounds`, `players` — all keyed by `id`
- Key functions: `getCourses`, `saveRound`, `getRounds`, `savePlayer`, `getPlayers`, `addPlayer`, `deletePlayer`, `clearStore`, `clearAllData`, `deleteDatabase`
- Types in `ui/src/types/`: `course.ts` (Course, Hole), `player.ts` (Player), `round.ts` (Round), `scoreEntry.ts` (ScoreEntry)
- IDs: `crypto.randomUUID()` with `uuid` package fallback
- Seed data: `ui/src/data/dummyCourses.ts` — loaded on first run if no courses exist
- Backend: planned ASP.NET Core API in `api/` — NOT yet scaffolded
- Auth: **ASP.NET Core Identity** with **HttpOnly cookie** storage (confirmed 2026-04-05)
- Database: **PostgreSQL** with Entity Framework Core (confirmed 2026-04-05)

## Learnings

<!-- Append new learnings below. -->
