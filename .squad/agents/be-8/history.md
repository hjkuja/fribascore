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

### 2026-04-05 — Issue #25: API Scaffold

- **.NET version:** 10.0.201 (confirmed installed)
- **Template:** `dotnet new webapi -n FribaScore.Api -o api` — generates top-level statements by default in .NET 10, no flag needed
- **NuGet packages added (all resolved to .NET 10 compatible versions):**
  - `Microsoft.EntityFrameworkCore` 10.0.5
  - `Npgsql.EntityFrameworkCore.PostgreSQL` 10.0.1
  - `Microsoft.AspNetCore.Identity.EntityFrameworkCore` 10.0.5
  - `Microsoft.EntityFrameworkCore.Design` 10.0.5
- **Build fix:** .NET 10 webapi template uses built-in OpenAPI (`AddOpenApi()` / `MapOpenApi()`) — Swashbuckle extension methods (`AddSwaggerGen`, `UseSwagger`, `UseSwaggerUI`) are NOT available by default. Updated Program.cs accordingly.
- **Files created:** `api/Program.cs`, `api/appsettings.json`, `api/Data/AppDbContext.cs`, `api/Models/Course.cs`, `api/Models/Player.cs`, `api/Models/Round.cs`, `api/Controllers/CoursesController.cs`, `api/Controllers/PlayersController.cs`, `api/Controllers/RoundsController.cs`, `api/.gitignore`
- **PR:** https://github.com/hjkuja/fribascore/pull/31
- **Key Pattern:** When scaffolding modern .NET, always check built-in integrations first before adding external packages. ASP.NET Core 10 includes OpenAPI, Identity, EF Core by default.

