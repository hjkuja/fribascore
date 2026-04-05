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

### 2026-04-05 — Issue #25: Full API scaffold (restructure + Minimal API)

- **Structure:** api/ restructured to `api/src/FribaScore.Api/` + `api/test/`. git mv used for history preservation; prior session had already committed the moves.
- **Minimal API pattern:** Replaced MVC Controllers with `MapGroup()` per resource, `TypedResults` for strongly-typed responses, `WithName()` + `WithDescription()` on every endpoint, `RequireAuthorization()` on mutating endpoints.
- **WithName uniqueness:** `WithName()` values must be globally unique across all endpoint groups — solved by appending resource suffix (e.g. `nameof(GetAll) + "Players"`) to avoid duplicate route name conflicts.
- **New packages:** `LanguageExt.Core` 4.4.9 (Result pattern for service layer), `Scalar.AspNetCore` 2.9.0 (OpenAPI UI replacing raw `/openapi/v1.json`).
- **Directory.Build.props:** Placed at `api/` root (NOT inside `src/`) to apply `RestorePackagesWithLockFile=true` to all three projects. `RestoreLockedMode` only applies in CI.
- **fribascore.slnx:** SLNX format (no GUIDs) replaces old `fribascore.sln`. Organized into `/src/` and `/test/` solution folders.
- **Test projects:** `dotnet new xunit --framework net10.0`. Integration test project also gets `Microsoft.AspNetCore.Mvc.Testing` package + project reference to main API.
- **Build result:** All 3 projects build with 0 errors, 0 warnings.
- **Key Pattern:** For Minimal API endpoint naming, always use unique `WithName()` strings globally — the simplest approach is appending the resource name to the method name.

### 2026-04-05 — Issue #25: 3-Project Service Layer Split

- **Pattern adopted:** Api / Application / Contracts split modelled on hjkuja/ShouldDo reference repo.
- **Contracts is dep-free:** Status codes hardcoded as ints (404, 400) — avoids referencing `Microsoft.AspNetCore.Http` and keeps Contracts a clean POCO project.
- **ServiceExtensions.cs in Application:** Encapsulates `AddDbContext` + service registrations behind `AddApplicationServices(connectionString)`. Api's Program.cs never needs to import EF or Npgsql directly.
- **Result<T> usage:** `LanguageExt.Common.Result<T>` — wrap success with `new Result<T>(value)`, wrap failure with `new Result<T>(exception)`. `.Match(success, failure)` called at endpoint layer.
- **AppDbContext stays in Application:** Api project has no EF Core or Npgsql NuGet references. Identity DI (`AddEntityFrameworkStores<AppDbContext>`) still works because Application is a project reference (types are transitive).
- **Mapping as extension methods:** `entity.ToResponse()` defined in Application/Mapping/ — clean, no automapper needed at this scale.
- **Input validation in services:** `BadRequestException` thrown inside service (e.g. empty Name) before any DB write — Api layer stays clean.
- **Build result:** All 3 source projects build with 0 errors, 0 warnings.
- **Open item for QT-3:** Both test projects (`FribaScore.Api.Tests.Unit`, `FribaScore.Api.Tests.Integration`) reference `FribaScore.Api` which no longer contains `AppDbContext` or entity models. Integration tests that mock/inject `AppDbContext` directly need project references updated to `FribaScore.Application`. Unit tests mocking endpoints need service interfaces from `FribaScore.Application` and DTOs from `FribaScore.Contracts`.


