# Routing

This file is human-editable. Use it as the default routing table when a task does not explicitly name an agent.

## Per-domain ownership

- `FE-2` owns `ui/src/app/`, `ui/src/pages/`, `ui/src/components/`, `ui/src/styles/`, navigation, accessibility, and PWA-facing client UX.
  Source: `ui/src/app/AppRoutes.tsx`; `ui/src/app/AppLayout.tsx`; `ui/src/styles/design-system.css`; `docs/development/contributing.md`

- `BE-8` owns the browser data layer in `ui/src/utils/db.ts`, domain models in `ui/src/types/`, seed data in `ui/src/data/`, backend code under `api/src/`, auth behavior, and any client/server contract changes.
  Source: `ui/src/utils/db.ts`; `docs/architecture/overview.md`; `api/src/FribaScore.Api/Program.cs`; `api/src/FribaScore.Application/ServiceExtensions.cs`

- `QT-3` owns test strategy, edge cases, regression coverage, and test files under `ui/**/*.test.tsx`, `ui/test/`, and `api/test/`.
  Source: `docs/development/testing.md`; `ui/bunfig.toml`; `api/test/FribaScore.Api.Tests.Unit/FribaScore.Api.Tests.Unit.csproj`; `api/test/FribaScore.Api.Tests.Integration/FribaScore.Api.Tests.Integration.csproj`

- `LD-7` owns architecture decisions, cross-domain decomposition, CI/workflow changes, documentation that changes system boundaries, and conflicts between docs and code.
  Source: `docs/architecture/overview.md`; `.github/workflows/ci.yml`; `.github/workflows/api.yml`; `.github/copilot-instructions.md`

## Overlap rules

- UI implementation plus UI tests: `FE-2` leads, `QT-3` supports.
- API, IndexedDB, auth, or shared model changes plus tests: `BE-8` leads, `QT-3` supports.
- Tasks spanning UI and API contracts, offline-sync behavior, auth flows, or route/data ownership: `LD-7` decomposes first, then hands work to specialists.
- Documentation that only reflects an implemented domain change belongs to the owning specialist. Documentation that redefines boundaries or future direction belongs to `LD-7`.
- Because this lean team has no dedicated ops role, `.github/workflows/**` and other automation changes route to `LD-7` first.
  Source: legacy routing record deleted during migration; current lean role set in `.council/charters/*`; live workflows in `.github/workflows/*.yml`

## Escalation triggers

- a new dependency, framework, or major runtime change
- auth/session/cookie/token behavior changes
- IndexedDB store changes, database schema changes, or breaking API contract changes
- CI permission changes or new workflow automation
- disagreement between the codebase and the docs
- ambiguous product scope, naming, or UX behavior

## Current repo-specific notes

- `/__admin` is development-only and should stay behind `import.meta.env.DEV`.
  Source: `ui/src/app/AppRoutes.tsx`

- Backend build and test commands should treat `api/fribascore.slnx` as the current solution entry point.
  Source: `api/fribascore.slnx`; `.github/workflows/api.yml`
