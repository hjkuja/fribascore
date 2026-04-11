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

### 2026-04-05 — Docs: API Overview Rewrite

- Rewrote `docs/api/overview.md` from placeholder "Not yet started" to an accurate reference doc reflecting the full 3-project scaffold.
- Sections covered: status, tech stack table, project structure with dependency flow, architecture patterns (Result<T>, service layer, DTOs, mapping), full endpoint reference with auth requirements, auth section (cookie-based Identity, issue #26 not yet implemented), local dev setup (connection string, Scalar UI, OpenAPI JSON URLs), CI notes (api.yml, --locked-mode).
- Removed stale content: JWT references, sync queue section, placeholder TBDs.

### 2026-04-05 — Fix: WithName operationId collision in CourseEndpoints

- **Root cause:** `CourseEndpoints.cs` used bare method names (`nameof(GetAll)`, `nameof(GetById)`, etc.) which collide with identically-named methods in other endpoint classes — `WithName()` values must be globally unique across the entire app.
- **Fix:** Appended resource suffix to each: `nameof(GetAll) + "Courses"`, `nameof(GetById) + "Course"`, `nameof(Create) + "Course"`, `nameof(Delete) + "Course"`.
- **Convention confirmed:** Plural suffix for list endpoints (`GetAllCourses`, `GetAllPlayers`), singular for single-resource endpoints (`GetByIdCourse`, `CreateCourse`, `DeleteCourse`). Matches existing `PlayerEndpoints` and `RoundEndpoints` patterns.
- **Build result:** 0 errors, 0 warnings.

### 2026-04-05 — Issue #25: 3-Project Service Layer Split

- **Pattern adopted:** Api / Application / Contracts split modelled on hjkuja/ShouldDo reference repo.
- **Contracts is dep-free:** Status codes hardcoded as ints (404, 400) — avoids referencing `Microsoft.AspNetCore.Http` and keeps Contracts a clean POCO project.
- **ServiceExtensions.cs in Application:** Encapsulates `AddDbContext` + service registrations behind `AddApplicationServices(connectionString)`. Api's Program.cs never needs to import EF or Npgsql directly.
- **Result<T> usage:** `LanguageExt.Common.Result<T>` — wrap success with `new Result<T>(value)`, wrap failure with `new Result<T>(exception)`. `.Match(success, failure)` called at endpoint layer.
- **AppDbContext stays in Application:** Api project has no EF Core or Npgsql NuGet references. Identity DI (`AddEntityFrameworkStores<AppDbContext>`) still works because Application is a project reference (types are transitive).
- **Mapping as extension methods:** `entity.ToResponse()` defined in Application/Mapping/ — clean, no automapper needed at this scale.
- **Input validation in services:** `BadRequestException` thrown inside service (e.g. empty Name) before any DB write — Api layer stays clean.
- **Build result:** All 3 source projects build with 0 errors, 0 warnings.
- **Test project references:** Both test projects (`FribaScore.Api.Tests.Unit`, `FribaScore.Api.Tests.Integration`) updated to reference `FribaScore.Application` (for service interfaces, models, DbContext) and `FribaScore.Contracts` (for DTOs, exceptions). Unit tests do NOT reference `FribaScore.Api` to keep them fast (no ASP.NET hosting overhead). Integration tests reference all three to boot full app via `WebApplicationFactory<Program>`. Build verified: 0 errors, 0 warnings.

### 2026-04-09 — Docs: Authentication Strategy (Phase 1 + OpenIddict Expansion Path)

- **Created:** `docs/specs/authentication.md` — comprehensive spec covering Phase 1 (HttpOnly cookies) and Phase 2+ (OpenIddict OIDC expansion).
- **Enhanced:** `docs/api/overview.md` — expanded Auth section to describe HttpOnly strategy and OpenIddict future path. Now clearly states "no JWT, no localStorage".
- **Enhanced:** `docs/architecture/overview.md` — updated Auth subsection to describe phased approach.
- **Updated:** `.squad/decisions.md` — added formal decision record for Phase 1 + Phase 2+ expansion path.
- **Key pattern:** Phase 1 (HttpOnly cookies) chosen for single-app simplicity and offline resilience. Phase 2+ (OpenIddict) is a non-breaking expansion layer for multi-app/federated scenarios. Same `AppUser` and `AppDbContext` store all auth data — no migration needed.
- **Security properties documented:** HttpOnly, Secure, SameSite=Strict, encryption, server-side validation. Explicit note: cookies survive app restarts without client-side logic (key for offline-first design).

### 2026-04-11 — Docs: Auth revision guardrails

- **Issue alignment:** Phase 1 auth docs should mirror issue #26 closely enough to avoid contract drift. `POST /auth/login`, `POST /auth/logout`, and `GET /auth/me` are the only documented Phase 1 auth endpoints; `/auth/me` returns `id` + `username`.
- **Planning safety:** Future auth docs should stay at the level of triggers, constraints, and open questions. Avoid pre-committing packages, providers, tables, endpoints, or token flows before a separate Phase 2 planning issue exists.
- **Key files:** `docs/api/overview.md`, `docs/architecture/auth.md`, `docs/specs/authentication.md`, `.squad/decisions/inbox/ISSUE-DRAFT-phase2-oidc.md`, `.squad/decisions.md`.
- **Reusable pattern:** For phased documentation, keep the current phase tied to the active issue contract and keep later phases explicitly planning-only.


