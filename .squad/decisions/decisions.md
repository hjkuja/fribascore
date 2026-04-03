# Team Decisions

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
