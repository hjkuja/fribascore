# Squad Decisions

## Active Decisions

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
