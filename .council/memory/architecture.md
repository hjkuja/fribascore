# Architecture

## Stack

- Frontend runtime: React 19.2, TypeScript strict mode, Vite 8, React Router 7, Bun 1.3.10, `idb` 8, `uuid` 13.
  Source: `ui/package.json`; `ui/tsconfig.app.json`

- Frontend testing: Bun test, React Testing Library, `@testing-library/jest-dom`, and `happy-dom` preloaded from `ui/bunfig.toml`.
  Source: `ui/package.json`; `ui/bunfig.toml`; `docs/development/testing.md`

- Backend runtime: ASP.NET Core Minimal API on .NET 10, C#, EF Core 10 + Npgsql, ASP.NET Core Identity, `LanguageExt.Core`, and Scalar UI.
  Source: `api/src/FribaScore.Api/FribaScore.Api.csproj`; `api/src/FribaScore.Application/FribaScore.Application.csproj`; `api/src/FribaScore.Api/Program.cs`

- CI surfaces: one workflow for `ui/` and one for `api/`.
  Source: `.github/workflows/ci.yml`; `.github/workflows/api.yml`

## Structure

- Monorepo layout: `ui/` for the frontend, `api/` for the backend, and `docs/` for reference material.
  Source: `README.md`; `docs/architecture/overview.md`; `api/README.md`

- Frontend layers: `src/app` for the shell and routes, `src/pages` for route-level pages, `src/components/<Name>/` for reusable UI, `src/types` for domain types, `src/utils` for shared logic, and `src/data` for seed data.
  Source: `docs/development/contributing.md`; `ui/src/app/AppRoutes.tsx`

- Backend layers: `FribaScore.Api` is the HTTP boundary, `FribaScore.Application` owns services/DbContext/mapping, `FribaScore.Contracts` holds DTOs and exceptions, and `api/test` holds unit and integration tests.
  Source: `docs/api/overview.md`; `api/src/FribaScore.Api/Program.cs`; `api/src/FribaScore.Application/ServiceExtensions.cs`; `api/src/FribaScore.Contracts/FribaScore.Contracts.csproj`

- Current backend solution entry point: `api/fribascore.slnx`.
  Source: `api/fribascore.slnx`; `.github/workflows/api.yml`

## Key patterns

- Local data is written and read through `ui/src/utils/db.ts`, which owns the `courses`, `rounds`, and `players` object stores in IndexedDB.
  Source: `ui/src/utils/db.ts`; `docs/architecture/overview.md`

- Dummy course data is seeded only when the local `courses` store is empty.
  Source: `ui/src/utils/db.ts`

- React pages are thin route files; reusable UI is colocated with CSS and tests in component subfolders.
  Source: `docs/development/contributing.md`; `ui/src/app/AppRoutes.tsx`

- Client-only debug tooling stays behind `import.meta.env.DEV`, including the `/__admin` route.
  Source: `ui/src/app/AppRoutes.tsx`; `ui/src/app/AppLayout.tsx`

- Minimal API endpoints are grouped by resource, use service interfaces, return `TypedResults`, and map failures through `ApiResults.ToProblemResult()`.
  Source: `api/src/FribaScore.Api/Endpoints/Courses/CourseEndpoints.cs`; `api/src/FribaScore.Api/ApiResults.cs`; `api/src/FribaScore.Application/Services/PlayerService.cs`

- DTOs live in `Contracts`, while entity-to-response mapping lives in `Application/Mapping`.
  Source: `docs/api/overview.md`; `api/src/FribaScore.Application/ServiceExtensions.cs`

- Browser auth is cookie-based; do not introduce JWT or `localStorage` token flows for the web client without an explicit new decision.
  Source: `docs/architecture/auth.md`; `api/src/FribaScore.Api/Program.cs`

- Round history and round summaries use explicit `en-GB` timestamp formatting.
  Source: `ui/src/pages/HistoryPage.tsx`; `ui/src/pages/RoundSummary.tsx`

## Integration points

- Browser IndexedDB database: `fribascore` version 1 with `courses`, `rounds`, and `players`.
  Source: `ui/src/utils/db.ts`

- Local PostgreSQL backing the ASP.NET Core app through EF Core + Npgsql.
  Source: `api/src/FribaScore.Application/FribaScore.Application.csproj`; `api/README.md`

- OpenAPI served from the API plus Scalar UI in development.
  Source: `api/src/FribaScore.Api/Program.cs`; `api/README.md`

- GitHub Actions workflows for frontend and backend validation.
  Source: `.github/workflows/ci.yml`; `.github/workflows/api.yml`

## Known constraints or non-obvious rules

- Use `bun`, not `npm` or `yarn`, for frontend install/build/test commands.
  Source: `ui/package.json`; `docs/development/contributing.md`; `.github/copilot-instructions.md`

- NuGet lockfiles are enabled via `api/Directory.Build.props`.
  Source: `api/Directory.Build.props`

- Minimal API `WithName()` values need to be globally unique.
  Source: legacy backend history record deleted during migration; `api/src/FribaScore.Api/Endpoints/Courses/CourseEndpoints.cs`

- The live repo uses `api/fribascore.slnx`; older docs that mention a root-level solution path are stale.
  Source: `api/fribascore.slnx`; `.github/workflows/api.yml`; `.github/copilot-instructions.md`
  <!-- Human review: align older docs and helper instructions with the current solution path. -->

- Some top-level docs still reflect an earlier phase where the backend was only planned.
  Source: `README.md`; `docs/api/overview.md`; `api/src/FribaScore.Api/Program.cs`
  <!-- Human review: prefer live code and the API docs over the root README when they disagree. -->
