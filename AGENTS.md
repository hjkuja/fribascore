# AGENTS.md

**FribaScore** is an offline-first disc golf (frisbee golf) scorecard web app. Players browse courses, start a round, enter scores hole-by-hole, and view a summary — all without a network connection. Cloud sync is a future phase and must never block offline use.

This repo is a **monorepo** plus a project-specific **council** system for routing and memory. Read both.

| Area | Where |
|------|-------|
| Frontend | `ui/` — React 19 + TypeScript (strict) + Vite 8 + Bun |
| Backend | `api/` — ASP.NET Core 10 Minimal API + EF Core 10 + Npgsql + PostgreSQL |
| Documentation | `docs/` — canonical reference (architecture, specs, dev, API) |
| Planning/scratch | `dev/` — raw notes, **not** canonical; prefer `docs/` |
| Council system | `.council/` + `.github/agents/` — AI-team routing & memory |

> **Docs vs code:** When `docs/`, `.council/memory/*`, and live code disagree, prefer live code + `docs/api/` for backend. Several root-level docs still reflect a phase where the backend was only planned.

---

## 1. Council System (how work is routed and remembered)

FribaScore uses a multi-agent "council". If you are acting as an agent (or routing on behalf of one), follow these rules.

### Roles
| Agent | ID | Owns |
|-------|----|------|
| Council | `council.agent.md` | Orchestrator — routes work, decomposes cross-domain tasks, handoffs |
| Lead/Architect | `LD-7` | Architecture decisions, decomposition, auth/storage/contract boundaries, CI/workflow changes, doc-vs-code conflicts |
| Backend | `BE-8` | IndexedDB, shared data models, API/auth/business logic, EF Core, PostgreSQL, integration contracts |
| Frontend | `FE-2` | React UI, CSS, routing, accessibility, responsive/mobile UX, PWA client UX |
| Tester | `QT-3` | Test strategy, regression risk, edge cases, integration coverage. **Does not write production code.** |
| Scribe | `scribe` | Merges inbox decisions into durable memory. **Run only when the human explicitly asks.** |

Direct agent entrypoints: `.github/agents/{council,ld-7,be-8,fe-2,qt-3,scribe}.agent.md`. Charters: `.council/charters/{ld-7,be-8,fe-2,qt-3,scribe}.md`.

### Routing
1. Single-domain work → go straight to the specialist.
   - UI/navigation/styling/a11y → **FE-2**
   - storage (IndexedDB), API, auth, business logic, contracts → **BE-8**
   - test design / edge cases → **QT-3**
2. Cross-domain work, or anything touching **auth, storage boundaries, dependencies, CI/workflows, or documented architecture** → **LD-7** first.
3. Overlaps: UI+UI tests → FE-2 leads, QT-3 supports. API/IndexedDB/auth+tests → BE-8 leads, QT-3 supports.
4. **Escalate to the human** when: product scope or UX behavior is ambiguous; a breaking change has multiple valid directions; code and docs disagree and the source of truth is unclear; or a new dependency/runtime changes the system boundary.
5. Run agents in parallel only when tracks are independent; otherwise go sequential with a tight handoff.
6. Every handoff includes: goal + done state; touched files/dirs; relevant routes/services/stores/tests; constraints from decisions+architecture memory; open questions/risks.

### Before starting (every agent)
Read, in order:
1. `.council/orchestrator.md` (if routing)
2. `.council/memory/decisions.md`
3. `.council/memory/architecture.md`
4. `.council/routing.md`
5. `.council/memory/history/{role}.md` (role's own history)

### Memory / decision logging (do not skip)
- New **decisions** → append to `.council/memory/inbox/{role}.md`:
  `[YYYY-MM-DD] [DECISION] what | why | alternatives rejected`
- Durable **learnings** → append to `.council/memory/history/{role}.md`:
  `[YYYY-MM-DD] [LEARNING] one line, actionable`
- Scribe merges inbox → `decisions.md` (dedupe, chronological, clear inbox, report what was merged) — only on explicit request.

> GitHub "squad" workflows (`.github/workflows/squad-*.yml`) are a separate automation layer that triages/assigns GitHub issues via `squad:` labels and a `.squad/team.md` (or `.ai-team/team.md`) roster. They are not the same as the council but share the same role set.

---

## 2. Frontend (`ui/`)

**Stack:** React 19, TypeScript (strict, ES2022, `react-jsx`), Vite 8, React Router 7, **Bun 1.3.10**, `idb` 8, `uuid` 13.
**Testing:** Bun's `bun:test`, `@testing-library/react`, `@testing-library/jest-dom`, `happy-dom`.

### Commands (run from `ui/`, use `bun` — never `npm`/`yarn`)
```bash
bun dev              # dev server (port 5173)
bun run build        # type-check + Vite production bundle
bun lint             # ESLint
bun run typecheck    # tsc (app + test configs)
bun test             # unit/component tests (happy-dom)
bun test --watch     # watch mode
bun test --coverage  # coverage
bun preview          # serve the build
```
E2E dir `test/e2e/playwright/` exists but is not configured and is excluded from `bun test` (`ui/bunfig.toml` → `pathIgnorePatterns`).

### File organization
```
ui/src/
  app/          # shell only: App.tsx, AppLayout.tsx, AppRoutes.tsx
  components/   # reusable UI — one subfolder per component
  pages/        # route-level pages — thin wrappers, one per route
  styles/       # shared styles, design-system.css
  types/        # TypeScript interfaces only — no logic
  utils/        # shared logic — db.ts (the storage gateway)
  data/         # static seed data — dummyCourses.ts
ui/test/        # happydom.ts, setupTests.ts, e2e/playwright/
```
- **Components live in their own subfolder** with collocated files:
  ```
  src/components/CourseDetails/
    CourseDetails.tsx
    CourseDetails.css
    CourseDetails.test.tsx
  ```
- **Pages are thin.** Don't put business logic in `pages/` — extract it into `utils/` or component hooks.
- One component per file; its CSS is imported directly in the component file.

### React & TypeScript style
- Functional components only; explicit prop interfaces (e.g. `interface CourseDetailsProps {...}`).
- PascalCase component names and filenames (`CourseDetails.tsx`).
- Use `import type {...}` for type-only imports.
- `interface` for object shapes, `type` for unions/aliases.
- No implicit `any`; no unused locals/parameters (strict mode).

### Styling
- **BEM-like** naming: `block__element--modifier` (e.g. `course-details__name`, `score-card__hole--active`).
- One CSS file per component; no global styles except `app/` level.
- Design tokens live in `ui/src/styles/design-system.css` (`--bg`, `--text`, `--accent`, `--glass-bg`, …). **`--accent` is canonical**; `--main-accent-color` is a compatibility alias only during migration.

### Accessibility
- `aria-label` on all interactive elements lacking visible text.
- Semantic HTML (`<article>`, `<table>`, `<nav>`, `<main>`, `<button>`).
- Tables need `aria-label` or `<caption>`.

### Data & IDs
- **IndexedDB is the single source of local truth.** All reads/writes go through `ui/src/utils/db.ts` — never touch IndexedDB from components.
  - DB: `fribascore` (version 1), object stores: `courses`, `rounds`, `players` (each keyed by `id`).
  - Exports: `getCourses`, `saveRound`, `getRounds`, `savePlayer`, `getPlayers`, `addPlayer`, `deletePlayer`, `clearStore`, `clearAllData`, `deleteDatabase`, `seedDummyCourses`.
- **Seed dummy courses only when the `courses` store is empty** (`src/data/dummyCourses.ts`).
- Generate IDs with `crypto.randomUUID()` (fallback in `addPlayer`).
- Client env vars are `VITE_`-prefixed.
- Dev-only features (e.g. the `/__admin` route) **must** be gated behind `import.meta.env.DEV`.
- Round timestamps use explicit `toLocaleString('en-GB', { hour12: false, ... })` — don't rely on locale.

### Routes
`/` · `/courses` · `/courses/:id` · `/courses/:id/start-round` · `/rounds/:id/score` · `/rounds/:id/summary` · `/history` · `/settings` · `/__admin` (dev only).

### Testing (frontend)
- Co-locate with the source: `src/components/<Name>/<Name>.test.tsx`, `src/data/dummyCourses.test.ts`.
- Import from `bun:test`: `describe, test, expect, beforeEach, afterEach`.
- Use `@testing-library/react` + jest-dom matchers; wrap routing-dependent components in `<MemoryRouter>`.
- Mock data inline; avoid importing production `db.ts` in tests.
- Setup: `bunfig.toml` preloads `test/happydom.ts` then `test/setupTests.ts`; `afterEach(cleanup)` is registered globally.

---

## 3. Backend (`api/`)

**Stack:** ASP.NET Core 10 Minimal API, C#, EF Core 10 + Npgsql, ASP.NET Core Identity, `LanguageExt.Core`, Scalar UI. PostgreSQL.
**Solution entry point:** `api/fribascore.slnx`.

### Commands
```bash
dotnet restore api/fribascore.slnx
dotnet build   api/fribascore.slnx
dotnet test    api/fribascore.slnx
dotnet run --project api/src/FribaScore.Api
```
PostgreSQL via Docker:
```bash
docker run -d --name fribascore-db -e POSTGRES_DB=fribascore \
  -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 postgres:16
```
API: `https://localhost:8443` / `http://localhost:8080`.
- OpenAPI: `/openapi/v1.json` · Scalar UI: `/scalar/v1`.
- **No committed EF Core migration yet** — schema work is in progress (see issue #37 for rename to `FribaDbContext`).

### Project structure (3-project split)
```
api/
  src/
    FribaScore.Api/            # HTTP boundary — endpoints, Program.cs, ApiResults.cs
    FribaScore.Application/    # Services, AppDbContext, Mapping, Models
    FribaScore.Contracts/      # DTOs + exceptions — framework-light, no refs
  test/
    FribaScore.Api.Tests.Unit/
    FribaScore.Api.Tests.Integration/
```
**Dependency rule (must never invert):** `Api` → `Application` → `Contracts`. `Contracts` has no project references.

### Core patterns
- **Endpoints** are `static` classes in `Endpoints/<Resource>/<Resource>Endpoints.cs`; they `MapGroup()`, inject **service interfaces** (never `DbContext` directly), return `TypedResults` (`Results<Ok<T>, ProblemHttpResult>`), and map failures via `ApiResults.ToProblemResult()`.
- **Services** implement interfaces in `Application/Services/Interfaces/` (e.g. `ICourseService`); return `Result<T>` from `LanguageExt.Common`.
- **Mapping**: `ToResponse()` extension methods on entities (in `Application/Mapping`) — not in services.
- **DTOs** are `record` types in `Contracts`: `Requests/` (`CreateCourseRequest`…) and `Responses/` (`CourseResponse`…).
- **Exceptions** hierarchy in `Contracts/Exceptions/`: base `CustomException` (carries HTTP status) → `NotFoundException` (404), `BadRequestException` (400).
- **Auth:** cookie-based ASP.NET Core Identity with **HttpOnly** sessions (`SameSite=Strict`). **Never** JWT, never `localStorage` tokens for the web client without a new decision.
- **DI:** register in `Application/ServiceExtensions` (`services.AddApplicationServices()`); call it from `Program.cs`.
- **Naming:** `CourseEndpoints`/`PlayerEndpoints`/`RoundEndpoints`; `ICourseService`/`IPlayerService`/`IRoundService`; `FribaScore.Api.Tests.Unit` / `.Integration`.
- **`.NET standards`:** .NET 10; `Program.cs` uses top-level statements (no `Program` class); file-scoped namespaces (`namespace FribaScore.Api.Endpoints;`).

### Backend testing
- Unit + integration in `api/test/`. CI runs `dotnet build`/`dotnet test` (Release) against `api/fribascore.slnx`.
- **Upgrade to xUnit v3** is pending (issue #36) — currently xUnit v2.
- **NuGet lockfiles** are enforced via `api/Directory.Build.props` (`RestorePackagesWithLockFile=true`): after adding a package, run `dotnet restore` and commit the updated `packages.lock.json`. CI restores in `--locked-mode`.
- `Minimal API` `WithName()` values must be **globally unique** across the app.

---

## 4. CI

Two independent workflows (path-filtered):
- **UI CI** (`.github/workflows/ui-ci.yml`): on `ui/**` — `bun ci` → `bun lint` → `bun typecheck` → `bun test` → `bun run build` → upload `ui/dist/`.
- **API CI** (`.github/workflows/api-ci.yml`): on `api/**` — `dotnet restore --locked-mode` → `dotnet build -c Release` → `dotnet test -c Release`.
- **Squad workflows** (`squad-*.yml`): issue triage/assignment via `squad:` labels + `.squad/team.md`.

CI runs on push/PR to `main`. Use `dotnet` `10.x` and Bun in CI; match local tool versions.

---

## 5. Conventions & Workflow

### Branches
| Prefix | Use |
|--------|-----|
| `feat/` | new features |
| `fix/` | bug fixes |
| `docs/` | documentation |
| `chore/` | tooling/config/maintenance |
| `refactor/` | restructuring, no behavior change |

### Commits (conventional commits)
```
feat: add round history page
fix: prevent score from being saved as NaN
docs: update getting started guide
chore: upgrade vite to v8
```

### Before pushing / opening a PR
- [ ] `bun lint` passes (no errors)
- [ ] `bun run typecheck` passes
- [ ] `bun test` passes
- [ ] `bun run build` succeeds
- [ ] For backend: `dotnet build` + `dotnet test` on `api/fribascore.slnx`
- [ ] New functionality has at least a basic component test

### Don'ts
- Don't use `npm`/`yarn` — use `bun`.
- Don't touch IndexedDB outside `ui/src/utils/db.ts`.
- Don't add JWT / `localStorage` auth.
- Don't put business logic in `ui/src/pages/`.
- Don't commit `.env.local`, secrets, or any token.
- Don't introduce a new dependency or change an auth/storage/CI boundary without routing through **LD-7**.
- Keep `/__admin` and similar dev tooling behind `import.meta.env.DEV`.

### Roadmap
`ROADMAP.md` tracks progress (14/45 done). When picking an item, create a GitHub issue, link it in the **Issue** column, and bump the progress bar `%` (`round(done/total*100)`).
