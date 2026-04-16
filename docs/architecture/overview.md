# System Overview

FribaScore is an offline-first disc golf scorecard web app built as a monorepo. This document covers the overall architecture, tech choices, and how the pieces fit together.

## Goals

- **Offline first** — the app must work fully without internet. Data lives in the browser's IndexedDB.
- **Mobile-friendly** — designed for field use on a phone browser.
- **PWA** — installable, fast load, works offline.
- **Optional cloud sync** — rounds and player data can be synced to a backend when the user is online and authenticated.

## Tech Stack

| Layer | Technology |
|-------|------------|
| Frontend framework | React 19 (functional components, hooks) |
| Frontend language | TypeScript (strict mode) |
| Build tool | Vite 7 + Babel React Compiler |
| Routing | React Router DOM v7 |
| Local storage | IndexedDB via `idb` v8 |
| Package manager | Bun |
| Linting | ESLint 9 with TypeScript + React Hooks plugins |
| Frontend testing | Bun test runner + React Testing Library + happy-dom |
| Backend framework | ASP.NET Core 10 (Minimal APIs) |
| Backend language | C# (file-scoped namespaces, top-level statements) |
| Database | PostgreSQL via EF Core + Npgsql |
| Auth | ASP.NET Core Identity — HttpOnly cookie sessions |
| OpenAPI | Built-in .NET `AddOpenApi()` + Scalar UI |
| Backend CI | GitHub Actions (`.github/workflows/api.yml`) |

## Monorepo Layout

The repo is a monorepo containing both the frontend and backend.

```
fribascore/
  ui/                       # React frontend (Bun + Vite)
  api/                      # ASP.NET Core backend
    src/
      FribaScore.Api/
      FribaScore.Application/
      FribaScore.Contracts/
    test/
      FribaScore.Api.Tests.Unit/
      FribaScore.Api.Tests.Integration/
  docs/                     # Architecture docs
  .github/
    workflows/
      ci.yml                # Frontend CI
      api.yml               # Backend CI
  fribascore.slnx           # .NET solution file (includes all 5 projects)
```

Frontend work lives entirely under `ui/`. Backend work lives entirely under `api/`. Shared docs live under `docs/`.

## Frontend Structure (`ui/`)

The frontend is organised into clear layers:

- **App shell** — routing, layout, and top-level providers
- **Components** — reusable UI pieces, each in its own subfolder with styles and tests
- **Pages** — one file per route, thin wrappers around components
- **Types** — TypeScript interfaces for the core domain entities
- **Utils** — shared logic (e.g. local storage access)
- **Data** — seed/dummy data used in development

## Routing

The app is a single-page application with client-side routing. All pages share a common layout shell that provides navigation.

The main sections of the app are: home, course browsing, round setup, live scoring, round summary, history, and settings. A dev-only admin/debug page is also available in development builds.

## Key Design Decisions

### Offline first, sync later
Data is always written to local storage first. The backend sync is additive and does not block any user interaction. This allows full use of the app without a network connection.

### Single local database
All app data lives in a single local database with separate stores for courses, rounds, and players. All reads and writes are centralised through one utility module — no component accesses storage directly.

### Seed data on first run
On first load, if no courses exist locally, a set of dummy courses is loaded automatically. This ensures the app is usable before any backend is available.

### Dev-only tools
A debug/admin page is only available in development builds. It provides tools for inspecting and resetting local data during development.

### Client-side IDs
All entities are assigned unique IDs on the client at creation time. This supports fully offline creation of rounds and players without any server involvement.

---

## Backend

### Project Structure

The backend is a 3-project ASP.NET Core 10 solution under `api/`:

| Project | Role |
|---------|------|
| `FribaScore.Api` | HTTP boundary — Minimal API endpoints, middleware, `Program.cs` |
| `FribaScore.Application` | Business logic — services, EF Core `DbContext`, DI registration |
| `FribaScore.Contracts` | Shared types — DTOs (`record`s), custom exceptions. No framework deps |

**Dependency rule:** `Api` → `Application` → `Contracts`. `Contracts` has zero project references.

### Service Layer Pattern

Endpoints inject service interfaces — never `DbContext` directly:

```
ICourseService  ←  CourseEndpoints
IPlayerService  ←  PlayerEndpoints
IRoundService   ←  RoundEndpoints
```

Service interfaces are defined in `Application/Services/Interfaces/`. Implementations live alongside them in `Application/Services/`. DI wiring is encapsulated in `ServiceExtensions` — `Program.cs` calls `services.AddApplicationServices()`.

### Result Pattern

Services return `Result<T>` from `LanguageExt.Common`. Endpoint handlers use `.Match()` with `ToProblemResult()`:

```csharp
return (await courseService.GetAllAsync())
    .Match(ok => TypedResults.Ok(ok), ex => ex.ToProblemResult());
```

`ToProblemResult(this Exception)` is an extension method in `ApiResults.cs` in the Api project. It maps `CustomException` subclasses to the correct HTTP problem response.

### Auth

**Phase 1 (current plan):** ASP.NET Core Identity with **HttpOnly cookie authentication** (`SameSite=Strict`, secure cookies in production). Issue #26 defines the Phase 1 auth surface: `POST /auth/login`, `POST /auth/logout`, and `GET /auth/me`. JWT is explicitly not used. Tokens are never stored in `localStorage`, and `/auth/me` returns the authenticated user's `id` and `username`.

**Phase 2+ (readiness path):** If FribaScore later needs multi-app federation, external identity providers, or token-based access, the Phase 1 design leaves room to evaluate a future auth expansion. That remains a planning direction only; packages, providers, endpoints, and flows are intentionally undecided.

### Database

PostgreSQL via EF Core + Npgsql. `AppDbContext` lives in `FribaScore.Application`. NuGet lockfiles are enforced via `RestorePackagesWithLockFile=true` in `Directory.Build.props`.

**Integration tests:** Use Testcontainers to spin up PostgreSQL containers automatically. Tests run migrations and provide a production-equivalent schema for testing. Requires Docker to be available locally.

### OpenAPI

Built-in .NET `AddOpenApi()` (no Swashbuckle). Spec available at `/openapi/v1.json`. Scalar UI served for browser exploration. Endpoint handlers use `TypedResults` with `Results<Ok<T>, ProblemHttpResult>` return types so the framework can infer response schemas.

### Solution File

`fribascore.slnx` at the repo root includes all 5 projects (3 app + 2 test). Use this for all `dotnet build` and `dotnet test` invocations.
