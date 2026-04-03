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

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
