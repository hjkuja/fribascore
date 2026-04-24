# API Overview

Backend API for FribaScore — disc golf scorecard app.

## Status

🟡 **Scaffolded, in progress.** 3-project solution structure is in place, service layer is wired up, and auth endpoints are implemented. Resource APIs remain in progress.

## Tech Stack

| Layer | Technology |
|-------|------------|
| Framework | ASP.NET Core 10 Minimal API |
| Language | C# (.NET 10) |
| Auth | ASP.NET Core Identity — HttpOnly cookie, SameSite=Strict (not JWT) |
| Database | PostgreSQL via EF Core + Npgsql |
| Result pattern | `LanguageExt.Core` — `Result<T>` / `.Match()` |
| OpenAPI | Built-in `AddOpenApi()` + Scalar UI |
| Solution format | SLNX (`api/fribascore.slnx`) |

## Project Structure

```
api/
  Directory.Build.props          — NuGet lock files, shared MSBuild props
  src/
    FribaScore.Api/              — Program.cs, endpoint groups, ApiResults.cs
    FribaScore.Application/      — services, interfaces, AppDbContext, EF migrations, mapping
    FribaScore.Contracts/        — request/response DTOs, custom exceptions (no ASP.NET deps)
  test/
    FribaScore.Api.Tests.Unit/
    FribaScore.Api.Tests.Integration/
```

**Project dependencies:**
- `Api` → `Application` → `Contracts`
- `Api` has no direct EF Core or Npgsql references; those are encapsulated in `Application`.

## Architecture Patterns

**Service layer:** Business logic lives in `Application` services. Services return `Result<T>` (from `LanguageExt.Common`). Failures are expressed as typed exceptions (`NotFoundException`, `BadRequestException`) wrapped inside `Result<T>`.

**Endpoints:** Each resource is a `MapGroup()` route group. Endpoints call the service and use `.Match()` to map `Result<T>` to HTTP responses via `ToProblemResult()` for errors and `TypedResults` for success.

**DTOs:** All request/response types are in `Contracts`. Entity-to-response mapping is done via extension methods in `Application/Mapping/`.

**Validation:** Input validation happens in the service layer before any DB write.

## Endpoints

### Courses — current live routes

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| `GET` | `/api/courses` | Public | List all courses |
| `GET` | `/api/courses/{id}` | Public | Get a single course with hole data |
| `POST` | `/api/courses` | Authenticated | Create a course |
| `DELETE` | `/api/courses/{id}` | Authenticated | Delete a course |

### Players — current live routes

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| `GET` | `/api/players` | Public | List all players |
| `GET` | `/api/players/{id}` | Public | Get a single player |
| `POST` | `/api/players` | Authenticated | Create a player |
| `DELETE` | `/api/players/{id}` | Authenticated | Delete a player |

### Rounds — current live routes

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| `GET` | `/api/rounds` | Public | List all rounds |
| `GET` | `/api/rounds/{id}` | Public | Get a single round |
| `POST` | `/api/rounds` | Authenticated | Create a round |
| `DELETE` | `/api/rounds/{id}` | Authenticated | Delete a round |

> **Current status note:** `POST`/`DELETE` routes opt into authorization today, but the auth endpoints and per-user ownership scoping are not implemented yet. `PUT` endpoints for players are also not implemented in the live code.

### Auth — implemented in issue [#26](https://github.com/hjkuja/fribascore/issues/26)

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| `POST` | `/auth/login` | Public | Validates username + password, returns `200` with current user info and an HttpOnly cookie on success, `401` on failure |
| `POST` | `/auth/logout` | Authenticated | Clears auth cookie and returns `204 No Content` |
| `GET` | `/auth/me` | Authenticated | Returns current user info (`id`, `username`), `401` otherwise |

## Authentication

### Phase 1: ASP.NET Core Identity + HttpOnly Cookies (Implemented)

Uses **ASP.NET Core Identity** with **HttpOnly cookie sessions** — the default authentication mechanism for ASP.NET Core web applications.

**Strategy:**
- Phase 1 scope matches issue #26 exactly: `POST /auth/login`, `POST /auth/logout`, `GET /auth/me`
- User credentials stored securely via ASP.NET Core Identity
- Sessions maintained via persistent HttpOnly cookies (`SameSite=Strict`, secure in production)
- No JWT tokens; tokens are never stored in `localStorage`
- Current non-public create/delete routes require `RequireAuthorization()`
- Passwords are hashed via Identity; no plaintext password storage
- Cookie auth redirects are suppressed for API endpoints so unauthenticated requests return `401` instead of HTML redirects

**Why this approach:**
- Offline-first UX: the browser handles the auth cookie without client-side token storage logic
- Native ASP.NET Core support: minimal code, battle-tested
- Single-app focus: fits FribaScore's current architecture

### Phase 2+: OIDC/SSO Readiness (Future direction)

If FribaScore expands to support multi-app scenarios or federated identity (for example, third-party sign-in or programmatic API access), the auth layer may need an OIDC-capable extension.

**Documented direction:**
- Keep Phase 1 as the committed implementation baseline
- Preserve HttpOnly cookies as the primary web session mechanism
- Re-evaluate whether any OIDC-capable extension is needed only when a concrete Phase 2 use case exists
- Do not treat packages, provider choices, endpoint shapes, or token flows as decided yet

**When this matters:**
- Multi-tenant deployments
- Federated access (e.g., "Sign in with [Partner App]")
- Mobile app companion tokens
- Programmatic API access (external tools, bots)

**Planning guardrail:** Any Phase 2 design should avoid breaking the Phase 1 cookie-based web client and should continue to use the existing Identity-backed user store where practical.

## Local Development

**Prerequisites:** .NET 10 SDK, PostgreSQL running locally, Docker (for integration tests).

**Connection string:** Set in `api/src/FribaScore.Api/appsettings.Development.json` (not committed).

```bash
cd api/src/FribaScore.Api
dotnet run
```

| URL | Description |
|-----|-------------|
| `https://localhost:8443/scalar/v1` | Scalar OpenAPI UI (dev profile) |
| `https://localhost:8443/openapi/v1.json` | Raw OpenAPI JSON |

## Testing

### Integration Tests

Integration tests use **Testcontainers** to run PostgreSQL containers. This provides a production-equivalent test environment without mocking the database layer.

**Requirements:**
- Docker must be running locally
- Testcontainers handles container setup and teardown automatically

**Running integration tests:**
```bash
dotnet test api/test/FribaScore.Api.Tests.Integration
```

The test factory (`AuthApiFactory`, etc.) creates a `WebApplicationFactory<ApiAssemblyMarker>` that:
1. Spins up a PostgreSQL container via Testcontainers
2. Applies all EF Core migrations to create a production schema
3. Provides helper methods to seed test data and create HTTP clients
4. Cleans up the container after tests complete

See `docs/development/testing.md` for more details.

## CI

Separate workflow: `.github/workflows/api.yml`

- Triggered on push/PR to `main` for changes under `api/`
- Runs: `dotnet restore --locked-mode`, `dotnet build`, `dotnet test`
- NuGet lock files are committed; `RestorePackagesWithLockFile=true` is set in `api/Directory.Build.props`
