# Testing

FribaScore uses Bun's built-in test runner with React Testing Library for frontend component tests, and xUnit with Testcontainers for backend integration tests. This document covers how to run tests, how the test setup works, and how to write new tests.

## Running Tests

All frontend commands are run from the `ui/` directory.

### Frontend Tests

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

### Backend Tests

All backend commands are run from the repository root.

```bash
# Run all backend tests (unit + integration)
dotnet test fribascore.slnx

# Run only unit tests
dotnet test fribascore.slnx -p:ExcludeIntegrationTests=true

# Run only integration tests (requires Docker)
dotnet test api/test/FribaScore.Api.Tests.Integration

# Run tests with coverage
dotnet test fribascore.slnx --collect:"XPlat Code Coverage"
```

**Note:** Integration tests require Docker to be running locally. The Testcontainers library automatically manages PostgreSQL containers.

## Test Stack

| Tool | Role | Scope |
|------|------|-------|
| `bun:test` | Test runner | Frontend unit + component |
| `@testing-library/react` | React component rendering and querying | Frontend |
| `@testing-library/jest-dom` | Extended matchers (`toBeInTheDocument`, etc.) | Frontend |
| `happy-dom` | DOM environment (via `@happy-dom/global-registrator`) | Frontend |
| `xUnit` | Test runner | Backend unit + integration |
| `Testcontainers.PostgreSql` | PostgreSQL container lifecycle management | Backend integration |
| `WebApplicationFactory` | Test host for API endpoints | Backend integration |

## How the Setup Works

### Frontend Setup

Tests are configured via `bunfig.toml` at the `ui/` root, which preloads two setup files before every test file:

1. **`test/happydom.ts`** — Registers `happy-dom` as the global DOM environment using `GlobalRegistrator.register()`. This runs before any test file imports React or Testing Library.

2. **`test/setupTests.ts`** — Imports `@testing-library/jest-dom` to add extended matchers, and registers `afterEach(cleanup)` globally so React trees are unmounted between tests.

You do not need to import or call either of these in your own test files.

## Backend Integration Tests

Integration tests for the API (`FribaScore.Api.Tests.Integration`) use **Testcontainers to manage PostgreSQL containers**. This approach provides a production-equivalent schema for testing without maintaining separate migration paths or mock databases.

### How it Works

1. **Container lifecycle:** Testcontainers automatically downloads and runs a PostgreSQL image when tests start, and tears it down when tests complete.
2. **Schema setup:** The `AuthApiFactory` (and other test factories) calls `dbContext.Database.MigrateAsync()` to apply all EF Core migrations to the container's PostgreSQL instance.
3. **Test isolation:** Each test class gets its own container instance, ensuring tests do not interfere with each other.
4. **Docker requirement:** Tests require Docker to be available locally (`docker info` must succeed). GitHub Actions `ubuntu-latest` runners include Docker out of the box.

### Writing Integration Tests

Integration tests inherit from xUnit's `[Fact]` or `[Theory]` and use `WebApplicationFactory<ApiAssemblyMarker>` to create an in-process test host. The test factory can override the production `DbContext` to swap in a Testcontainers-managed PostgreSQL instance.

Example:
```csharp
[Fact]
public async Task Login_WithValidCredentials_ReturnsOk()
{
    using var factory = new AuthApiFactory();
    using var client = factory.CreateHttpsClient();
    
    var user = await factory.SeedUserAsync("testuser", "Str0ng!Pass");
    var response = await client.PostAsJsonAsync("/auth/login", new { username = "testuser", password = "Str0ng!Pass" });
    
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
}
```

The factory handles database initialization and cleanup; you only need to seed test data and make assertions.

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

### Frontend CI

Tests run automatically on every push and pull request to `main` via GitHub Actions (`.github/workflows/ci.yml`). The CI pipeline runs:

1. `bun ci` — install dependencies
2. `bun lint` — ESLint
3. `bun run typecheck` — TypeScript
4. `bun run build` — production bundle
5. `bun test` — all unit and component tests

> E2E tests (Playwright) are excluded from the standard `bun test` run and are not yet wired into CI.

### Backend CI

Backend tests run automatically on every push and pull request to `main` via GitHub Actions (`.github/workflows/api.yml`). The CI pipeline runs on `ubuntu-latest` (which includes Docker) and executes:

1. `dotnet restore --locked-mode` — restore NuGet packages from lock files
2. `dotnet build fribascore.slnx --no-restore -c Release` — build all projects
3. `dotnet test fribascore.slnx --no-build -c Release` — run all unit and integration tests (including Testcontainers tests against PostgreSQL containers)

Docker is available on `ubuntu-latest` runners, so integration tests work without additional setup.
