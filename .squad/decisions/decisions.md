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
