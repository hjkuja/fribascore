# API Overview

Backend API for FribaScore — disc golf scorecard app.

## Status

🟡 **Scaffolded, in progress.** 3-project solution structure is in place, all endpoints are mapped, service layer is wired up. Auth (issue #26) is not yet implemented.

## Tech Stack

| Layer | Technology |
|-------|------------|
| Framework | ASP.NET Core 10 Minimal API |
| Language | C# (.NET 10) |
| Auth | ASP.NET Core Identity — HttpOnly cookie, SameSite=Strict (not JWT) |
| Database | PostgreSQL via EF Core + Npgsql |
| Result pattern | `LanguageExt.Core` — `Result<T>` / `.Match()` |
| OpenAPI | Built-in `AddOpenApi()` + Scalar UI |
| Solution format | SLNX (`fribascore.slnx`) |

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

### Courses — public

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/courses` | List all courses |
| `GET` | `/courses/{id}` | Get a single course with hole data |

### Players — auth required

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/players` | List players for the authenticated user |
| `POST` | `/players` | Create a player |
| `PUT` | `/players/{id}` | Update a player |
| `DELETE` | `/players/{id}` | Delete a player |

### Rounds — auth required

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/rounds` | List rounds for the authenticated user |
| `POST` | `/rounds` | Create a round (immutable after creation — no PUT) |

### Auth — issue [#26](https://github.com/hjkuja/fribascore/issues/26), not yet implemented

| Method | Path | Description |
|--------|------|-------------|
| `POST` | `/auth/login` | Sign in — sets HttpOnly cookie |
| `POST` | `/auth/logout` | Sign out — clears cookie |
| `GET` | `/auth/me` | Get current authenticated user |

## Authentication

Uses **ASP.NET Core Identity** with **HttpOnly cookie sessions** (no JWT). Cookie is `SameSite=Strict`. All non-public endpoints use `RequireAuthorization()`.

## Local Development

**Prerequisites:** .NET 10 SDK, PostgreSQL running locally.

**Connection string:** Set in `api/src/FribaScore.Api/appsettings.Development.json` (not committed).

```bash
cd api/src/FribaScore.Api
dotnet run
```

| URL | Description |
|-----|-------------|
| `https://localhost:5001/scalar` | Scalar OpenAPI UI (dev only) |
| `https://localhost:5001/openapi/v1.json` | Raw OpenAPI JSON |

## CI

Separate workflow: `.github/workflows/api.yml`

- Triggered on push/PR to `main` for changes under `api/`
- Runs: `dotnet restore --locked-mode`, `dotnet build`, `dotnet test`
- NuGet lock files are committed; `RestorePackagesWithLockFile=true` is set in `api/Directory.Build.props`
