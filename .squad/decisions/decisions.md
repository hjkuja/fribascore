# Team Decisions

## Database Technology (2026-04-05)

**Agent:** hjkuja (via Copilot)

**Decision:** PostgreSQL

**Rationale:** User explicit choice for FribaScore backend.

**Status:** Adopted

---

## Authentication Technology & Storage (2026-04-05)

**Agent:** hjkuja (confirmed), recommended by LD-7

**Decision 1 — Auth Framework:** ASP.NET Core Identity

**Rationale:**
- Framework-integrated, battle-tested, handles password hashing, token lifecycle, and account lockout
- Seamless integration with PostgreSQL via Entity Framework Core
- No custom JWT implementation needed
- Production-ready for offline-first PWA sync pattern

**Decision 2 — JWT Client-Side Storage:** HttpOnly cookie with SameSite=Strict

**Rationale:**
- Automatic token attachment to API requests (essential for PWA sync)
- Not readable by JavaScript (XSS protection via HttpOnly flag)
- CSRF risk mitigated by SameSite=Strict
- Native support in ASP.NET Core auth middleware

**Status:** Adopted

**Next Steps:**
- BE-8 documents ASP.NET Identity setup in ROADMAP
- Backend API skeleton includes HttpOnly cookie configuration

---

## DateTime Formatting Standard (2026-04-03)

**Agent:** FE-2  
**Issue:** #10  
**Status:** Adopted

Use `toLocaleString()` with explicit options for displaying round dates where user disambiguation is needed:

```typescript
new Date(date).toLocaleString(undefined, {
  day: '2-digit',
  month: '2-digit',
  year: 'numeric',
  hour: '2-digit',
  minute: '2-digit'
})
```

**Rationale:**
- Users play multiple rounds on the same day and need to distinguish them by time
- `toLocaleDateString()` strips time information entirely
- Explicit options ensure consistent formatting across locales
- The underlying `Round.date` field already contains full timestamps

**Impact:**
- Files changed: `ui/src/pages/HistoryPage.tsx`, `ui/src/pages/RoundSummary.tsx` (PR #14)
- Pattern applies to any future date-time displays requiring disambiguation
- Display-only modification; no breaking changes

---

## Testing Pages with IndexedDB Dependencies (2026-04-03)

**Agent:** QT-3  
**Issue:** #10  
**Status:** Adopted

Use Bun's `mock.module()` to mock the entire db module at import level for page tests:

```typescript
import * as db from "../utils/db";

beforeEach(() => {
  mock.restore();
});

test("my test", async () => {
  mock.module("../utils/db", () => ({
    getRounds: mock(() => Promise.resolve([mockRound])),
    getCourses: mock(() => Promise.resolve([mockCourse])),
  }));
  
  // render and assert...
});
```

**For pages with routing (useParams):**
- Use `<MemoryRouter initialEntries={["/rounds/round-1/summary"]}>` with `<Routes>` and `<Route>` to set route params properly

**Rationale:**
- Avoids dependency on real IndexedDB in tests
- Keeps tests fast and deterministic
- Allows testing edge cases with known data
- Follows "mock data inline" principle from test guidelines

**Impact:**
- Applied to: HistoryPage.test.tsx, RoundSummary.test.tsx (10 tests, all passing)
- Pattern ready for adoption across all future page-level tests
- Prevents IndexedDB pollution in test suite

---

## Locale-Agnostic Date/Time Test Assertions (2026-04-03)

**Agent:** QT-3  
**Issue:** #10  
**Status:** Adopted

Use regex patterns that match time components across different locale formats instead of asserting specific digit values.

```typescript
// ❌ Locale-dependent (fails on different locales)
expect(dateElement.textContent).toMatch(/14/);

// ✅ Locale-agnostic (works across locales)
expect(dateElement.textContent).toMatch(/\d{2}[.:]\d{2}/);
```

The pattern `/\d{2}[.:]\d{2}/` matches both colon (`:`) and period (`.`) separators used in different locales.

**Rationale:**
- CI runs on Linux (en-US locale) while development happens on Windows (e.g., Finnish locale)
- Different locales format times differently (`14:32` vs `14.32`)
- Locale-agnostic patterns remain representative of real user behavior
- Avoids mocking `toLocaleString()` or forcing explicit locales in production

**Impact:**
- Fixed 6 test assertions across HistoryPage.test.tsx and RoundSummary.test.tsx
- CI green on squad/10-history-time-of-day
- Pattern reusable for all future date/time test assertions

**Alternatives Considered:**
- Mock `toLocaleString()` (reduces test representativeness)
- Force explicit locale in code (impacts user experience)
- Check for specific formatted string (locale-dependent)

---

## API Test Project Naming — Split Unit/Integration (2026-04-05)

**Agent:** LD-7, QT-3 (recommendation)  
**Status:** Approved

Use split test projects: **`FribaScore.Api.Tests.Unit`** and **`FribaScore.Api.Tests.Integration`**

**Rationale:**
- Standard .NET convention (`{RootNamespace}.Tests.{Category}`)
- Unit tests (milliseconds, mocked) in fast CI feedback loop; integration tests (seconds, real infrastructure) run separately
- Allows developers to run quick unit tests during development, full integration suite in CI
- Aligns with FribaScore architecture: EF Core + controllers + planned auth = integration tests inevitable
- Cost of refactoring later (csproj files, namespaces, solution references) higher than setting up today

**Implementation:**
```
api/test/
  FribaScore.Api.Tests.Unit/
    FribaScore.Api.Tests.Unit.csproj
  FribaScore.Api.Tests.Integration/
    FribaScore.Api.Tests.Integration.csproj
```

Both projects reference `api/src/FribaScore.Api.csproj` and are in `fribascore.slnx` solution.

---

## Solution Format: SLNX (2026-04-05)

**Agent:** BE-8  
**Status:** Adopted

Replace `fribascore.sln` (GUID-based) with `fribascore.slnx` (XML format, no GUIDs).

**Benefits:**
- Human-readable, version control-friendly
- No GUID collisions or merge conflicts on GUID changes
- Easier to maintain large multi-project solutions

**Implementation:**
- Single solution file at repo root: `fribascore.slnx`
- Organized into `/src/` and `/test/` solution folders
- All project references use relative paths

---

## Minimal API Over MVC Controllers (2026-04-05)

**Agent:** BE-8  
**Status:** Adopted

All MVC Controllers (`CoursesController`, `PlayersController`, `RoundsController`) removed. Replaced with Minimal API endpoints using `MapGroup()`, `TypedResults`, and `RequireAuthorization()`.

**Patterns:**
- `MapGroup(routePrefix)` for resource grouping
- `TypedResults` for strongly-typed HTTP responses (not `Results`)
- `WithName(uniqueName)` + `WithDescription(desc)` on every endpoint
- Unique `WithName()` values across all endpoint groups (append resource name to method name to avoid collisions)
- `RequireAuthorization()` on all POST/DELETE endpoints

**Benefits:**
- Cleaner, more functional approach
- Automatic OpenAPI schema inference with typed returns
- Aligns with .NET 10 conventions

---

## 3-Project Service Layer Architecture (2026-04-05)

**Agent:** BE-8  
**Branch:** `squad/25-api-scaffold`  
**Status:** Implemented, build verified (0 errors, 0 warnings)

Split `FribaScore.Api` into three clean-dependency projects following hjkuja/ShouldDo reference pattern:

| Project | Role | Key contents |
|---------|------|-------------|
| `FribaScore.Contracts` | Shared contract types | Request/response DTOs (records), `CustomException` hierarchy |
| `FribaScore.Application` | Business logic + data access | `AppDbContext`, entity models, service interfaces + implementations, mapping extensions, `ServiceExtensions` |
| `FribaScore.Api` | HTTP boundary only | Endpoint route registration, `ApiResults.ToProblemResult`, `Program.cs` |

### Key architectural decisions

**Contracts is dependency-free:**
- HTTP status codes hardcoded as `int` literals (`404`, `400`) in exception constructors
- Avoids referencing `Microsoft.AspNetCore.Http`, keeps it a pure POCO library
- Safe to reference from any layer including future mobile/WASM clients

**ServiceExtensions encapsulates Application DI:**
- `FribaScore.Application.ServiceExtensions.AddApplicationServices(connectionString)` registers `AppDbContext` + all three scoped service implementations
- `Program.cs` in Api calls this single method — Api has no direct EF Core or Npgsql NuGet references

**Result<T> at service boundary:**
- Services return `Result<T>` from `LanguageExt.Common`
- Failure paths use `NotFoundException` / `BadRequestException` from Contracts
- Endpoints call `.Match(success => TypedResults.X, ex => ex.ToProblemResult())`

**Input validation in services:**
- Minimal validation (empty name checks) in service implementations, not endpoints
- Keeps Api layer dumb; full FluentValidation can be added to Application later

**Entity models stay in Application:**
- `Course`, `Player`, `Round` (EF entity classes) in `FribaScore.Application.Models`
- Clients never see entity shapes — they get Response DTOs from `FribaScore.Contracts.Responses`

---

## NuGet Lockfiles Committed (2026-04-05)

**Agent:** BE-8, OPS-9  
**Status:** Implemented

All .NET API projects use NuGet lockfile for reproducible restores.

**Implementation:**
- `Directory.Build.props` at `api/` root sets `RestorePackagesWithLockFile=true` for all projects
- `RestoreLockedMode=true` activates only in CI (environment variable)
- All three `packages.lock.json` files committed to source control

**CI Configuration:**
- `actions/setup-dotnet` with `cache: 'nuget'` and `cache-dependency-path: '**/packages.lock.json'`
- `dotnet restore --locked-mode` ensures deterministic dependency resolution

**Benefits:**
- Reproducible builds across environments
- Free NuGet caching via GitHub Actions
- Prevents accidental version drifts in CI/CD

---

## API CI Workflow Created (2026-04-05)

**Agent:** OPS-9  
**Branch:** `squad/25-api-scaffold`  
**File:** `.github/workflows/api.yml`  
**Status:** Done

Dedicated CI pipeline for .NET API.

**Trigger scope:** `api/**` and `.github/workflows/api.yml` path filters. Does not interfere with `ci.yml` (UI pipeline).

**Pipeline:**
- Dotnet version: `10.x`
- NuGet caching via `setup-dotnet` with `cache-dependency-path: '**/packages.lock.json'`
- Restore mode: `--locked-mode` (belt-and-suspenders with `Directory.Build.props`)
- Solution file: `fribascore.slnx` at repo root
- Step order: restore → build `--no-restore` → test `--no-build` (fail-fast)
- Security: All actions pinned to full commit SHA; `permissions: contents: read`

---

## Dark-Only Design Theme (2026-04-03)

**Agent:** FE-2  
**Branch:** squad/16-css-design-system  
**Status:** Adopted

FribaScore is a dark-only app — no light mode support.

**Decision:**
- Set `color-scheme: dark` on `:root` in `index.css`
- Remove all `@media (prefers-color-scheme: light)` blocks from component CSS
- All colours use design tokens from `design-system.css` — no raw hex or rgba values

**Rationale:**
- Retro-futurist design (dark charcoal + amber) only makes sense as dark theme
- Users on light-mode OS will see dark theme intentionally (disc golf is outdoors; dark screen easier to read in bright sun with brightness up)
- Reduces maintenance burden; no parallel light theme needed

**Affected Files:** `index.css`, `AppLayout.css`, `CourseList.css`, `HistoryPage.css`, `PlayerSelectModal.css`, `PlayersManagement.css`

---

## NuGet Lockfile Directive (2026-04-05)

**Author:** hjkuja (via Copilot)  
**Status:** Approved

All .NET projects in `api/` must use NuGet lockfile for reproducible restores.

**Implementation:**
- Set `RestorePackagesWithLockFile=true` in `Directory.Build.props`
- CI workflow uses `setup-dotnet` with `cache: nuget` and `cache-dependency-path: **/packages.lock.json`
- Set `RestoreLockedMode=true` in CI environment

**Rationale:**
1. Reproducible Restores — lockfile ensures deterministic dependency resolution across environments
2. Free NuGet Caching — GitHub Actions setup-dotnet provides automatic caching when lockfile is configured
3. CI Reliability — locked mode prevents accidental version drifts in CI/CD pipelines

---

## Test Project References to Application and Contracts (2026-04-05)

**Author:** QT-3  
**Branch:** squad/25-api-scaffold  
**Status:** Implemented

After BE-8 restructured the API into Contracts/Application/Api split, both test projects needed project reference updates.

**Decision:**

**Unit tests** (`FribaScore.Api.Tests.Unit`) reference:
- `FribaScore.Application` — to mock/test service interfaces and business logic
- `FribaScore.Contracts` — to construct DTOs and use custom exceptions in assertions

**Integration tests** (`FribaScore.Api.Tests.Integration`) reference:
- `FribaScore.Api` (already present) — for `WebApplicationFactory<Program>`
- `FribaScore.Application` — for any direct service or DbContext access in test setup
- `FribaScore.Contracts` — for DTOs in HTTP request/response assertions

**Rationale:**
- Unit tests should not reference `Api` (avoids pulling in ASP.NET hosting infrastructure, keeps tests fast)
- Integration tests need all three because `WebApplicationFactory` boots the full app

**Verification:**
`dotnet build fribascore.slnx` — Build succeeded (0 errors, 0 warnings)
