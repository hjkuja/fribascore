---
name: "bun-mock-isolation"
description: "Ensure module mocks don't persist across test files in Bun test runner by properly restoring original implementations"
domain: "testing"
confidence: "high"
source: "earned"
tools:
  - name: "bun:test"
    description: "Bun's built-in test runner with mock API"
    when: "When writing unit tests in Bun and need to mock imported modules"
---

## Context

Bun's `mock.module()` creates module-level mocks that persist across all test files within the same test process. Calling `mock.restore()` alone does **not** clear module mocks. When test files mock modules without properly restoring the original implementation, subsequent test files receive the mocked versions instead of the real module, causing test isolation failures.

This pattern applies when:
- Using Bun's test runner with module mocks
- Testing code that imports real module implementations
- Running the full test suite (not individual test files)
- Multiple test files mock the same module

## Patterns

### The Correct Module Mock Pattern

```typescript
import { describe, test, expect, mock, beforeEach, afterEach } from "bun:test";
import * as moduleToMock from "../path/to/module";

describe("ComponentUnderTest", () => {
  let mockFunctionA: ReturnType<typeof mock>;
  let mockFunctionB: ReturnType<typeof mock>;
  let originalModule: typeof moduleToMock;

  beforeEach(() => {
    // 1. ALWAYS capture the original module BEFORE mocking
    originalModule = { ...moduleToMock };
    
    // 2. Create individual mock functions with default implementations
    mockFunctionA = mock(() => Promise.resolve([]));
    mockFunctionB = mock(() => Promise.resolve({}));
    
    // 3. Apply the module mock ONCE per describe block (in beforeEach)
    mock.module("../path/to/module", () => ({
      functionA: mockFunctionA,
      functionB: mockFunctionB,
    }));
  });

  afterEach(() => {
    // 4. ALWAYS restore the original module implementation in afterEach
    mock.module("../path/to/module", () => originalModule);
    // 5. Then call mock.restore()
    mock.restore();
  });

  test("should do something", async () => {
    // Customize mock behavior per test using mockImplementation
    const testData = [{ id: "1", name: "Test Course" }];
    mockFunctionA.mockImplementation(() => Promise.resolve(testData));
    
    // ... test assertions
  });
});
```

### Step-by-Step Breakdown

1. **Capture original module:** Store the real module implementation before mocking it
   ```typescript
   originalModule = { ...moduleToMock };
   ```

2. **Create individual mocks:** Use `mock()` to create function mocks with sensible defaults
   ```typescript
   mockFunctionA = mock(() => Promise.resolve([]));
   ```

3. **Mock once in beforeEach:** Call `mock.module()` in the setup phase, not in each test
   ```typescript
   mock.module("../path/to/module", () => ({
     functionA: mockFunctionA,
     functionB: mockFunctionB,
   }));
   ```

4. **Restore in afterEach:** Always restore the original module
   ```typescript
   mock.module("../path/to/module", () => originalModule);
   mock.restore();
   ```

5. **Customize per test:** Use `mockImplementation()` to change behavior for individual tests
   ```typescript
   mockFunctionA.mockImplementation(() => Promise.resolve(customData));
   ```

## Examples

### Real-World Fix: Database Mock in Page Tests

**Before (broken):**
```typescript
describe("HistoryPage", () => {
  beforeEach(() => {
    mock.restore(); // WRONG: doesn't clear module mocks
  });

  test("displays time of day", async () => {
    mock.module("../utils/db", () => ({
      getRounds: mock(() => Promise.resolve([mockRound])),
      getCourses: mock(() => Promise.resolve([mockCourse])),
    }));
    // This mock PERSISTS even after test ends, affecting other test files!
  });
});
```

**After (fixed):**
```typescript
describe("HistoryPage", () => {
  let mockGetRounds: ReturnType<typeof mock>;
  let mockGetCourses: ReturnType<typeof mock>;
  let originalDbModule: typeof db;

  beforeEach(() => {
    // Capture the real module
    originalDbModule = { ...db };
    
    // Create mocks
    mockGetRounds = mock(() => Promise.resolve([]));
    mockGetCourses = mock(() => Promise.resolve([]));
    
    // Mock once
    mock.module("../utils/db", () => ({
      getRounds: mockGetRounds,
      getCourses: mockGetCourses,
    }));
  });

  afterEach(() => {
    // Restore the real module
    mock.module("../utils/db", () => originalDbModule);
    mock.restore();
  });

  test("displays time of day", async () => {
    // Customize mocks for this specific test
    mockGetRounds.mockImplementation(() => Promise.resolve([mockRound]));
    mockGetCourses.mockImplementation(() => Promise.resolve([mockCourse]));
    
    // ... assertions
  });
});
```

**Result:** All tests in the suite pass; no cross-file contamination.

## Anti-Patterns

❌ **Don't call `mock.module()` in every test:**
```typescript
test("test 1", () => {
  mock.module("../utils/db", () => ({ getRounds: ... }));
  // Creates a new persistent mock
});
test("test 2", () => {
  mock.module("../utils/db", () => ({ getRounds: ... }));
  // Doesn't clean up the previous mock; both now exist!
});
```

❌ **Don't rely on `mock.restore()` alone:**
```typescript
afterEach(() => {
  mock.restore(); // Does NOT clear module mocks!
  // Original module still replaced in subsequent test files
});
```

❌ **Don't use a reference instead of spread:**
```typescript
originalModule = moduleToMock; // WRONG: only stores reference, not functions
// When moduleToMock is mutated, originalModule is also affected
```

❌ **Don't forget to import with namespace:**
```typescript
import { functionA, functionB } from "../module"; // Can't capture full module
originalModule = { functionA, functionB }; // Incomplete; other exports missing
```

## When to Use

Apply this pattern whenever:
- Writing tests in Bun that mock imported modules
- Using `mock.module()` to replace a module's exports
- Running multiple test files that might import the same module
- You need module mocks to NOT persist across test files
- Tests check the behavior of code that imports real modules

## References

- **Issue:** #10 (History page time of day display)
- **Branch:** `squad/10-history-time-of-day`
- **Files Fixed:** `ui/src/pages/HistoryPage.test.tsx`, `ui/src/pages/RoundSummary.test.tsx`
- **Outcome:** 7 test failures resolved; all 94 tests pass
- **Bun Documentation:** https://bun.sh/docs/api/test#mocking

---

**Team Standard:** This pattern is the recommended approach for all module mocking in Bun tests at fribascore.
