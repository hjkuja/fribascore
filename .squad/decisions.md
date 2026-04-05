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

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
