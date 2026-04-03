# Project Context

- **Owner:** @hjkuja
- **Project:** FribaScore — offline-first disc golf scorecard PWA
- **Stack:** React 19, TypeScript, Vite 7, Bun, IndexedDB (idb v8), React Router v7, Bun test runner + React Testing Library + happy-dom
- **Repo root:** (repo root)
- **Frontend:** ui/
- **Docs:** docs/
- **Created:** 2026-04-03

## Key Testing Points

- Test runner: Bun's built-in (`bun:test`) — NOT Jest or Vitest
- DOM environment: `happy-dom` via `@happy-dom/global-registrator` (preloaded in `test/happydom.ts`)
- React testing: `@testing-library/react` + `@testing-library/jest-dom`
- Preloads in `bunfig.toml`: `test/happydom.ts` then `test/setupTests.ts`
- `afterEach(cleanup)` registered globally
- Co-locate tests: `ComponentName.test.tsx` next to `ComponentName.tsx`
- Wrap routing-dependent components in `<MemoryRouter>` from `react-router-dom`
- E2E: Playwright dir exists at `test/e2e/playwright/` but not configured — excluded from `bun test`
- Run: `bun test` (all), `bun test --watch`, `bun test --coverage`, `bun test src/components/Foo`
- Typecheck: `bun run typecheck` covers both `tsconfig.app.json` and `tsconfig.test.json`

## Learnings

### 2026-04-03: Bun Module Mock Isolation Pattern

**Problem:** When using `mock.module()` in Bun tests, module mocks persist across test files unless properly cleaned up. In branch `squad/10-history-time-of-day`, the new test files `HistoryPage.test.tsx` and `RoundSummary.test.tsx` mocked the `../utils/db` module, causing 7 IndexedDB tests in `db.test.ts` to fail. The mock data leaked into tests that expected real IndexedDB operations.

**Root cause:** Bun's `mock.restore()` alone doesn't fully clear module-level mocks. The module mock registry persists across test files that run in the same process.

**Solution:** Store a reference to the original module before mocking, then explicitly restore it in `afterEach` by re-mocking with the original:

```typescript
describe("HistoryPage", () => {
  let mockGetRounds: ReturnType<typeof mock>;
  let mockGetCourses: ReturnType<typeof mock>;
  let originalDbModule: typeof db;

  beforeEach(() => {
    // Capture original module before any mocking
    originalDbModule = { ...db };
    
    mockGetRounds = mock(() => Promise.resolve([]));
    mockGetCourses = mock(() => Promise.resolve([]));
    
    mock.module("../utils/db", () => ({
      getRounds: mockGetRounds,
      getCourses: mockGetCourses,
    }));
  });

  afterEach(() => {
    // Restore original module implementation
    mock.module("../utils/db", () => originalDbModule);
    mock.restore();
  });
  
  test("some test", async () => {
    // Set test-specific mock behavior
    mockGetRounds.mockImplementation(() => Promise.resolve([mockRound]));
    // ... test logic
  });
});
```

**Key points:**
- Use `mockImplementation()` in individual tests to change behavior, not `mock.module()` repeatedly
- Always pair `mock.module()` in `beforeEach` with restoration in `afterEach`
- The spread operator (`{ ...db }`) captures the original exported functions
- This pattern prevents mock leakage to test files that run later

**Verified:** All 94 tests pass (87 existing + 10 new from HistoryPage/RoundSummary) after applying this pattern.

<!-- Append new learnings below. -->
