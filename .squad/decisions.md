# Squad Decisions

## Active Decisions

### Directive: .NET API Coding Standards (2026-04-05)

**Status:** Active  
**Source:** hjkuja (via Copilot directive)  
**Scope:** All files in `api/`

**Standards:**
- .NET version: 10.x (current LTS)
- Program.cs style: Top-level statements (no Program class wrapper)
- Namespace style: File-scoped namespaces (e.g., `namespace FribaScore.Api.Controllers;`)

**Rationale:** User requirement for consistency and modern best practice adoption.

### CSS Design System Custom Property Naming (FE-2, 2026-04-04)

**Status:** Complete  
**Issue:** #16  
**PR:** #32

Canonical CSS custom property names established in `ui/src/styles/design-system.css`:

| Token | Value | Usage |
|-------|-------|-------|
| `--bg` | `#242424` | Primary background |
| `--bg-deep` | `#1a1a1a` | Deeper/contrast backgrounds |
| `--accent` | `#c77c27` | Amber orange accent — all interactive elements |
| `--accent-dim` | `rgba(199,124,39,0.30)` | Borders, dividers |
| `--accent-glow` | `rgba(199,124,39,0.15)` | Hover glows, shadows |
| `--accent-tint` | `rgba(199,124,39,0.06)` | Badge/tag fills |
| `--text` | `#f0e6d3` | Primary text (warm off-white) |
| `--text-muted` | `#9a8f82` | Secondary text (warm gray) |
| `--glass-bg` | `rgba(255,255,255,0.04)` | Card/panel surfaces |
| `--glass-border` | `rgba(255,255,255,0.08)` | Card/panel borders |

**Key Pattern:** Backward-compatibility alias `--main-accent-color: var(--accent)` prevents cascading failures during migration. New code uses `--accent`.

### API Scaffold Architecture (BE-8, 2026-04-05)

**Status:** Complete  
**Issue:** #25  
**PR:** #31

**Core Decisions:**
- **OpenAPI:** Built-in .NET 10 (not Swashbuckle). Docs at `/openapi/v1.json`. Framework default, reduces dependencies.
- **Auth:** ASP.NET Core Identity with HttpOnly cookie storage.
- **Database:** PostgreSQL with Entity Framework Core.
- **Artifacts:** `.gitignore` excludes `obj/`, `bin/`, `*.user`, `.vs/`.

**Rationale:** Aligned with .NET 10 conventions, team preferences (confirmed 2026-04-05), and minimal external dependencies.

### Authentication Strategy: Phase 1 + Future Auth Readiness (BE-8, 2026-04-09)

**Status:** Active (Phase 1)
**Scope:** All authentication flows in FribaScore

**Phase 1 Decision:**
- **Framework:** ASP.NET Core Identity
- **Session model:** HttpOnly cookies (`SameSite=Strict`, `Secure=Always`)
- **Token storage:** No JWT; no `localStorage`. Cookies only.
- **Rationale:** Simplest, most secure model for single-app use case. Works offline (cookies survive restarts). Battle-tested.

**Phase 2+ Readiness Direction:**
- **When:** Only if multi-app federation, external identity, or token-based access is validated as a real product need
- **Constraint:** Current HttpOnly cookie users must not be broken by later auth expansion work
- **Planning rule:** Packages, providers, endpoints, tables, and token flows are not committed yet
- **Goal:** Keep Phase 1 compatible with future evaluation work without prematurely choosing an implementation path

**Artifacts:**
- `docs/specs/authentication.md` — full strategy doc with phase comparison and security properties
- `docs/api/overview.md#authentication` — implementation details for Phase 1 (includes Phase 2+ callout)
- `docs/architecture/overview.md#auth` — phased auth description

**Related Issue:** #26 (Auth endpoint implementation — not yet started)

### Bun Module Mock Isolation Pattern (QT-3, 2026-04-03)

**Status:** Recommended for team adoption  
**Context:** Testing with Bun test runner and module mocking  
**Decision:** Standardize on module mock restoration pattern to prevent test isolation failures.

**Pattern:** When using `mock.module()` in Bun tests:
1. Capture original module before mocking: `originalModule = { ...moduleToMock }`
2. Call `mock.module()` in `beforeEach`, not per-test
3. Restore original in `afterEach()` before calling `mock.restore()`
4. Use `mockFn.mockImplementation()` to customize per test

**Rationale:** Bun's `mock.module()` creates persistent mocks within a test process. Without restoring the original module, mocks leak across test files, causing isolation failures.

**Evidence:** Fixed 7 test failures in `HistoryPage.test.tsx` and `RoundSummary.test.tsx` (all 94 tests now pass).

**Skills:** Documented in `.squad/skills/bun-mock-isolation/SKILL.md`

### Locale Standardization for DateTime Formatting (FE-2, 2026-04-03)

**Status:** Complete  
**Context:** DateTime formatting inconsistency across environments (Windows/Linux locales)  
**Decision:** Standardize on `'en-GB'` locale with `hour12: false` for all DateTime displays.

```typescript
new Date(date).toLocaleString('en-GB', {
  day: '2-digit',
  month: '2-digit',
  year: 'numeric',
  hour: '2-digit',
  minute: '2-digit',
  hour12: false
})
```

**Output:** Consistent `27/03/2026, 14:32` across all machines.

**Rationale:** Eliminates locale-based variability, enables precise test assertions, 24-hour format reduces ambiguity for same-day rounds.

**Files Changed:** HistoryPage.tsx, RoundSummary.tsx, + tests. All 94 tests passing.

### Model Preference Directive (hjkuja, 2026-04-04)

**Status:** Adopted  
**Context:** Agent selection guidance for team  
**Decision:**
- Default model for all agents: `claude-sonnet-4.6`
- Heavy coder tasks (large multi-file refactors, 500+ line code generation): `gpt-5.3-codex`

**Rationale:** User directive — captured for team memory and consistency.

### Aspire Orchestration: Defer to Phase 2 (LD-7, 2026-04-11)

**Status:** Active  
**Related Issue:** #33

**Decision:**
- Aspire AppHost for local developer orchestration **should not lead** FribaScore Phase 1 backend work.
- Keep sequencing: **#26 auth endpoints → #28 players API → #29 rounds API → #27 courses API** (no Aspire blocker).
- Evaluate Aspire adoption **after** Phase 1 API slice is stable, or sooner if local orchestration pain becomes a concrete drag.

**Rationale:**
- FribaScore is still proving core API contract (auth + user-owned players/rounds + public courses).
- Aspire improves local inner loop but is development orchestration only, not a shipped product feature.
- Current setup (Bun frontend + single API + PostgreSQL) is simple enough to run without AppHost.
- Adding Aspire now introduces more moving parts (DCP engine, containers, discovery wiring) before backend shape is proven.

**When Aspire becomes justified:**
- Dev setup friction is slowing the team (orchestration + logs + health + wiring across 4+ services).
- Consistent local startup, logs, health visibility needed across the stack.
- Multi-service architecture validated (worker, cache, ML, etc.).

**Guardrail if adopted later:**
- Keep the first Aspire pass minimal: AppHost orchestrates **frontend + API + PostgreSQL** only.
- No product-scope expansion; no cloud/deployment assumptions baked in.

### Authentication: Phase 1 + Future OIDC Readiness (LD-7 + BE-8, 2026-04-09)

**Status:** Active  
**Scope:** All authentication flows in FribaScore  
**Related Issues:** #26 (auth endpoints), future Phase 2 planning issue

**Phase 1 Decision (Concrete, Unblocks #26):**
- **Framework:** ASP.NET Core Identity
- **Session model:** HttpOnly cookies (`SameSite=Strict`, `Secure=Always`)
- **Endpoints:** `POST /auth/login`, `POST /auth/logout`, `GET /auth/me`
- **Token storage:** No JWT; no `localStorage`. Cookies only.
- **Database:** PostgreSQL with EF Core
- **Rationale:** Simplest, most secure single-app model. Cookies survive restarts (offline resilience). Battle-tested. Works without network.

**Phase 2+ Readiness Direction (Planning Only):**
- **Candidate approach:** OIDC/SSO via OpenIddict (Google, GitHub, Microsoft, etc.)
- **Constraint:** HttpOnly cookies remain the session mechanism; OIDC is backend-driven; no client-side token flows.
- **Constraint:** Current Phase 1 users must not be broken by later expansion work.
- **Planning rule:** Packages, providers, endpoints, tables, token flows are not committed yet.
- **Goal:** Keep Phase 1 compatible with future evaluation without prematurely choosing Phase 2 implementation.

**Security Properties:**
- XSS: HttpOnly flag prevents JavaScript access to cookies
- CSRF: SameSite=Strict prevents cross-origin form submissions
- Password hashing: bcrypt via ASP.NET Core Identity
- Credential storage: Identity owns all auth data in PostgreSQL (`AspNetUsers`, `AspNetUserLogins`, `AspNetUserTokens`, claims, roles)
- No secrets in client code

**Identity Storage Boundary:**
- **Identity owns:** username/email, password hashes, security stamps, external logins, auth tokens, claims, roles, 2FA state, lockout metadata
- **App-owned only:** `DisplayName`, audit timestamps, app-specific profile fields not related to authentication

**Artifacts:**
- `docs/specs/authentication.md` — full strategy with Phase 1 + Phase 2+ comparison
- `docs/api/overview.md#authentication` — Phase 1 implementation details + Phase 2 readiness note
- `docs/architecture/auth.md` — full design including security properties, testing strategy
- `docs/architecture/overview.md#authentication` — phased description

**Next Steps for Teams:**
- **BE-8:** Implement Phase 1 endpoints (#26); reference `docs/specs/authentication.md` Phase 1 section
- **QT-3:** Test Phase 1 (#26 acceptance criteria); add 8–12 test cases covering login/logout/auth flow
- **Scribe:** File Phase 2 planning issue on GitHub once Phase 1 is approved (separate from #26)

### Aspire Dev Experience: Early Planning, Non-blocking (LD-7, 2026-04-11)

**Status:** Active  
**Context:** Created GitHub issue #33; placed early in ROADMAP.md Dev Experience section

**Decision:**
- Track Aspire as an **early developer-experience/orchestration** item with explicit non-blocking status.
- Aspire improves the local inner loop once UI, API, and local infrastructure all run together.
- **It is not a user-facing feature** and should not be described as one.
- **It should not be a blocker** for auth or core API work.

**ROADMAP Implication:**
- `ROADMAP.md` surfaces Aspire early in its own DX area.
- Backend product work remains centered on auth and core endpoints; Aspire proceeds in parallel when useful.

**Key Learnings (From LD-7 History):**
- Checked `hjkuja/fribascore` open issues; no Aspire/AppHost/dev-orchestration duplicate found.
- Opened **#33** for minimal Aspire AppHost focused on local developer orchestration only.
- Updated `ROADMAP.md` to add new Dev Experience section ahead of Backend API; ordering is visible without implying Aspire must land before auth/core API issues.

### Authentication Docs: Phase Boundary Clarity (LD-7, 2026-04-11)

**Status:** Active  
**Scope:** Documentation consistency and issue hygiene

**Decision:**
- Treat **Phase 1 auth** as the only committed implementation plan.
- Document **Phase 2 auth** as a readiness path only: triggers, constraints, and planning questions — **not** packages/endpoints/schema changes that imply delivery commitment.

**Why:**
- Keeps the project aligned with offline-first and current single-app scope.
- Reduces accidental scope creep in docs and future issue planning.
- Prevents speculative Phase 2 details from being mistaken for approved implementation work.

**Implications:**
- `docs/architecture/auth.md` and `docs/specs/authentication.md` describe Phase 2 at the decision-gate level
- `docs/api/overview.md` and `docs/architecture/overview.md` reference Phase 2 as future direction, not committed build work
- Future Phase 2 issue stays planning-focused (no implementation PRs) until concrete product trigger justifies it

### QT-3 Auth Review Gate: Phase Boundary Approval (QT-3, 2026-04-11)

**Status:** Complete  
**Related:** Phase 1 auth docs, #26, Phase 2 planning issue draft

**Verdict:** ✅ Approved

**Reviewed Files:**
- `docs/specs/authentication.md` — factually safe, phase boundaries clear
- `docs/architecture/auth.md` — full architecture with testing strategy
- `docs/api/overview.md#authentication` — Phase 1 details + Phase 2+ readiness note
- `docs/architecture/overview.md#authentication` — phased description
- `ISSUE-DRAFT-phase2-oidc.md` — explicitly blocked by #26, stays planning-only

**Gate Results:**
- ✅ **Factual safety:** Future auth framed as readiness/planning only; no commits to packages/endpoints before justification
- ✅ **Scope clarity:** Phase 1 tied to #26; `/auth/me` aligns with #26 contract (id + username)
- ✅ **Issue hygiene:** Phase 2 draft is explicitly blocked by #26; keeps packages, endpoints, migrations, PR work out of scope

### Near-Term Backend Work Sequence (LD-7, 2026-04-11)

**Status:** Active  
**Scope:** Short-term sequencing across backend/auth work

**Decision:**
Recommend the next concrete sequence as:
1. **#26** — Auth endpoints
2. **#28** — Players API
3. **#29** — Rounds API
4. **#27** — Courses API

Keep Phase 2 auth planning draft blocked until Phase 1 is complete and a concrete product trigger justifies it.

**Rationale:**
- **Offline-first remains the product backbone.** Current app already works locally; backend work should focus on pieces required for safe authenticated sync.
- **Auth is the trust boundary.** Login/logout/me endpoints establish security model and unblock every protected API.
- **Players before rounds.** Simpler authenticated data flow validates per-user isolation before immutable round payloads.
- **Rounds next.** Core sync-shaped resource but depends on auth and benefits from settled ownership patterns.
- **Courses can wait.** Public, read-only, already seeded locally — less urgent than authenticated resources.
- **Phase 2 auth is not next.** OIDC/SSO draft correctly framed as planning-only. No package/provider/token work until Phase 1 is live and real use case exists.

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
