# Decisions

---
[2026-04-21] [DECISION]
What: The web app remains offline-first: browser data lives in IndexedDB and `ui/src/utils/db.ts` is the single storage gateway for `courses`, `rounds`, and `players`.
Why: The app must work on a disc golf course without a network connection, and centralized storage prevents components from drifting into incompatible local-data behavior.
Alternatives rejected: Remote-first writes and direct component-level access to IndexedDB.
Source: `docs/architecture/overview.md`; `ui/src/utils/db.ts`; `docs/development/contributing.md`
---

---
[2026-04-04] [DECISION]
What: Frontend styling is anchored on a shared design-token system in `ui/src/styles/design-system.css`, with `--accent` as the canonical accent token and `--main-accent-color` kept only as a compatibility alias during migration.
Why: Shared tokens keep the retro-futurist visual system consistent while avoiding cascading breakage in older components.
Alternatives rejected: Hardcoded colors per component and immediate removal of the legacy accent alias.
Source: legacy decisions record deleted during migration; legacy frontend history record deleted during migration; `ui/src/styles/design-system.css`
---

---
[2026-04-05] [DECISION]
What: The backend stays split into `FribaScore.Api`, `FribaScore.Application`, and `FribaScore.Contracts`, with endpoints calling services instead of `DbContext` directly.
Why: The split keeps HTTP concerns, business logic, and shared contracts separate, and keeps `Contracts` framework-light.
Alternatives rejected: A single-project backend and direct endpoint access to EF Core.
Source: legacy backend history record deleted during migration; `api/src/FribaScore.Api/Program.cs`; `api/src/FribaScore.Application/ServiceExtensions.cs`; `docs/api/overview.md`
---

---
[2026-04-05] [DECISION]
What: Backend API documentation uses built-in ASP.NET Core OpenAPI plus Scalar UI instead of Swashbuckle.
Why: The current stack already supports OpenAPI and browser exploration without adding a larger dependency surface.
Alternatives rejected: Swashbuckle-first API docs.
Source: legacy decisions record deleted during migration; legacy backend history record deleted during migration; `api/src/FribaScore.Api/FribaScore.Api.csproj`; `api/src/FribaScore.Api/Program.cs`
---

---
[2026-04-09] [DECISION]
What: Phase 1 web authentication uses ASP.NET Core Identity with HttpOnly cookie sessions; the browser client should not switch to JWT or `localStorage` token storage without a new decision.
Why: Cookie-based auth matches the current single-app architecture, keeps credentials out of browser storage APIs, and preserves the documented web-client contract.
Alternatives rejected: JWT-based browser auth and token storage in `localStorage`.
Source: legacy decisions record deleted during migration; `docs/architecture/auth.md`; `docs/api/overview.md`; `api/src/FribaScore.Api/Program.cs`
---

---
[2026-04-03] [DECISION]
What: Round timestamps shown in the UI use `toLocaleString('en-GB', { hour12: false, ... })` for stable date/time output.
Why: Explicit formatting prevents Windows/Linux locale drift in both product UI and tests.
Alternatives rejected: Locale-dependent `toLocaleString(undefined, ...)` output.
Source: legacy decisions record deleted during migration; legacy frontend history record deleted during migration; `ui/src/pages/HistoryPage.tsx`; `ui/src/pages/RoundSummary.tsx`
---

---
[2026-04-11] [DECISION]
What: Aspire/AppHost work is optional developer-experience follow-up and must not block the current auth/players/rounds/courses API slice.
Why: The repo is still stabilizing its core API and local orchestration is not a product requirement.
Alternatives rejected: Treating Aspire as a prerequisite for the current backend phase.
Source: legacy decisions record deleted during migration; legacy lead history record deleted during migration; `ROADMAP.md`
---

