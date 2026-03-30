# Testing

FribaScore uses Bun's built-in test runner with React Testing Library for component tests. This document covers how to run tests, how the test setup works, and how to write new tests.

## Running Tests

All commands are run from the `ui/` directory.

```bash
# Run all tests once
bun test

# Watch mode — re-run on file changes
bun test --watch

# With coverage report
bun test --coverage

# Run tests in a specific path
bun test src/components/CourseDetails
```

## Test Stack

| Tool | Role |
|------|------|
| `bun:test` | Test runner (no Jest or Vitest) |
| `@testing-library/react` | React component rendering and querying |
| `@testing-library/jest-dom` | Extended matchers (`toBeInTheDocument`, etc.) |
| `happy-dom` | DOM environment (via `@happy-dom/global-registrator`) |

## How the Setup Works

Tests are configured via `bunfig.toml` at the `ui/` root, which preloads two setup files before every test file:

1. **`test/happydom.ts`** — Registers `happy-dom` as the global DOM environment using `GlobalRegistrator.register()`. This runs before any test file imports React or Testing Library.

2. **`test/setupTests.ts`** — Imports `@testing-library/jest-dom` to add extended matchers, and registers `afterEach(cleanup)` globally so React trees are unmounted between tests.

You do not need to import or call either of these in your own test files.

## File Conventions

| Test type | Location | Naming |
|-----------|----------|--------|
| Component tests | Next to the component | `ComponentName.test.tsx` |
| Utility tests | Next to the utility | `utilityName.test.ts` |
| Shared test helpers | `test/` directory | Any `.ts` file |
| E2E tests | `test/e2e/playwright/` | `.spec.ts` (excluded from `bun test`) |

## Writing Tests

Import test primitives from `bun:test`:

```tsx
import { describe, test, expect } from "bun:test";
import { render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { MyComponent } from "./MyComponent";

describe("MyComponent", () => {
  test("renders expected content", () => {
    render(
      <MemoryRouter>
        <MyComponent />
      </MemoryRouter>
    );
    expect(screen.getByText("Hello")).toBeDefined();
  });
});
```

### Key rules

- **Always wrap components that use routing** in `<MemoryRouter>` from `react-router-dom`
- **Mock data inline** in the test file — do not import from `db.ts` or other production utilities
- **Use `screen` queries** (`getByText`, `getByRole`, etc.) rather than `container.querySelector`
- Prefer `toBeDefined()` over `toBeInTheDocument()` for simple presence checks when either works

### Mocking

Bun's test runner supports module mocking with `mock()` from `bun:test`. For IndexedDB-dependent code, either mock `db.ts` entirely or test components with inline prop data.

```ts
import { mock } from "bun:test";

mock.module("../../utils/db", () => ({
  getCourses: async () => [{ id: "1", name: "Test Course", holes: [], totalPar: 36 }],
}));
```

## TypeScript for Tests

Tests use `tsconfig.test.json`, which extends `tsconfig.app.json` and adds `bun-types`. Run the full typecheck (app + tests) with:

```bash
bun run typecheck
```

## CI

Tests run automatically on every push and pull request to `main` via GitHub Actions (`.github/workflows/ci.yml`). The CI pipeline runs:

1. `bun ci` — install dependencies
2. `bun lint` — ESLint
3. `bun run typecheck` — TypeScript
4. `bun run build` — production bundle
5. `bun test` — all unit and component tests

> E2E tests (Playwright) are excluded from the standard `bun test` run and are not yet wired into CI.
