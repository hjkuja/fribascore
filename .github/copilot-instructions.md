# Project Guidelines

## Code Style

- **TypeScript Configuration**: Strict mode enabled with no unused locals/parameters (`tsconfig.app.json`). Target ES2022, JSX transform to `react-jsx`.
- **Linting**: ESLint with recommended JS/TS rules, React hooks, and Vite refresh plugins (`eslint.config.js`). Run `bun lint` to check.
- **Component Patterns**: Functional components with explicit prop interfaces (e.g., `CourseDetailsProps` in `CourseDetails.tsx`). PascalCase naming for components and files.
- **Styling**: Separate CSS files per component (e.g., `CourseDetails.css` imported in `CourseDetails.tsx`). Use semantic class names like `course-details__name`.
- **Accessibility**: Include ARIA labels on interactive elements (e.g., `aria-label` on tables and articles in `CourseDetails.tsx`).

## Architecture

- **Framework**: React 19 functional components with hooks. Entry point via `main.tsx` with `StrictMode` and `BrowserRouter`.
- **Build Tool**: Vite with React plugin and Babel for React Compiler (`vite.config.ts`).
- **Routing**: React Router DOM for nested routes under `AppLayout` (`AppRoutes.tsx`). Pages in `src/pages/`, components in `src/components/`. Key routes:
  - `/` — Home page
  - `/courses` — Course list
  - `/courses/:id` — Course details
  - `/courses/:id/start-round` — Start a new round
  - `/rounds/:id/score` — Scoring during an active round
  - `/rounds/:id/summary` — Post-round summary
  - `/history` — Round history
  - `/settings` — App settings
  - `/__admin` — Admin/debug page (only rendered in `import.meta.env.DEV` mode)
- **Data Types**: TypeScript interfaces in `src/types/`:
  - `Course` and `Hole` in `course.ts` (id, name, holes array, totalPar, totalLength)
  - `Player` in `player.ts` (id, name)
  - `Round` in `round.ts` (id, courseId, date, players, scores)
  - `ScoreEntry` in `scoreEntry.ts` (playerId, holeNumber, score)
- **Data Persistence**: IndexedDB via the `idb` library (`src/utils/db.ts`). The database (`fribascore`, version 1) has three object stores: `courses`, `rounds`, and `players`, each keyed by `id`. Functions include `getCourses`, `saveRound`, `getRounds`, `savePlayer`, `getPlayers`, `addPlayer`, `deletePlayer`, `clearStore`, `clearAllData`, and `deleteDatabase`. Seed data loaded from `src/data/dummyCourses.ts` on first run.
- **Component Structure**: Reusable components in subfolders with `.tsx` and `.css` (e.g., `CourseList/CourseList.tsx`). Components include `CourseDetails`, `CourseList`, `CourseNotFound`, `HoleScore`, `PlayersManagement`, `ScoreCard`, and `Welcome`.
- **IDs**: Use `crypto.randomUUID()` (with fallback to `uuid` package) for generating entity IDs.

## Build and Test

- **Development**: `bun dev` starts Vite dev server with host binding (`package.json`).
- **Build**: `bun run build` compiles TypeScript and bundles with Vite (`package.json`).
- **Linting**: `bun lint` runs ESLint on the project (`package.json`).
  - Use this to find potential errors.
- **Typecheck**: `bun x tsc --noEmit` runs the TypeScript compiler without emitting files. The test config is checked separately via `tsconfig.test.json`: `bun run typecheck` covers both.
- **Preview**: `bun preview` serves built app locally (`package.json`).
- **Testing**: Bun's built-in test runner with `@testing-library/react` and `happy-dom` for DOM simulation. See the **Testing** section below for details.
- **CI**: GitHub Actions workflow (`.github/workflows/ci.yml`) runs on pushes and pull requests to `main`: installs dependencies (`bun ci`), lints, typechecks, builds, and uploads the `dist/` folder as an artifact.

## Testing

### Framework and Tools

- **Test runner**: Bun's built-in test runner (`bun:test`). No Jest or Vitest.
- **DOM environment**: `happy-dom` via `@happy-dom/global-registrator` (preloaded in `test/happydom.ts`).
- **React testing**: `@testing-library/react` with `@testing-library/jest-dom` matchers (setup in `test/setupTests.ts`).
- **Preloads**: `bunfig.toml` auto-preloads `test/happydom.ts` then `test/setupTests.ts` before every test file. `afterEach(cleanup)` is registered globally.
- **TypeScript**: Tests use `tsconfig.test.json` (extends `tsconfig.app.json`, includes `bun-types`).
- **E2E**: Playwright directory exists at `test/e2e/playwright/` but is not yet configured. E2E paths are excluded from `bun test` via `bunfig.toml`.

### Running Tests

```bash
bun test                  # run all unit/component tests once
bun test --watch          # re-run on file changes
bun test --coverage       # run with coverage report
bun test src/components/CourseDetails  # run tests in a specific path
```

### File Organization

- **Unit/component tests**: Co-located with the source file in the same directory, named `<ComponentName>.test.tsx` (e.g., `src/components/CourseDetails/CourseDetails.test.tsx`).
- **Data/utility tests**: Co-located in the same folder as the module (e.g., `src/data/dummyCourses.test.ts`).
- **E2E tests**: Go under `test/e2e/playwright/` and are excluded from `bun test`.
- **Test helpers/setup**: Shared setup files live in `test/` (e.g., `test/happydom.ts`, `test/setupTests.ts`).

### Writing Tests

- Import test primitives from `bun:test`: `import { describe, test, expect, beforeEach, afterEach } from "bun:test"`.
- Use `@testing-library/react` (`render`, `screen`, etc.) and `@testing-library/jest-dom` matchers (`toBeDefined`, `toBeInTheDocument`, etc.).
- Wrap components that need routing in `<MemoryRouter>` from `react-router-dom`.
- Mock data inline in the test file; avoid importing from production DB utilities.

```tsx
import { describe, test, expect } from "bun:test";
import { render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { MyComponent } from "./MyComponent";

describe("MyComponent", () => {
  test("renders expected content", () => {
    render(<MemoryRouter><MyComponent /></MemoryRouter>);
    expect(screen.getByText("Hello")).toBeDefined();
  });
});
```

## Project Conventions

- **File Organization**: `src/app/` for core app logic (`App.tsx`, `AppRoutes.tsx`), `src/components/` for UI components, `src/pages/` for route handlers, `src/types/` for type definitions, `src/utils/` for shared utilities (e.g., `db.ts`), `src/data/` for static seed data.
- **Imports**: Use `type` keyword for type-only imports (e.g., `import type { Course }` in `CourseDetails.tsx`).
- **Naming**: Components and types in PascalCase (e.g., `CourseDetails`, `Hole`). CSS classes use BEM-like naming (e.g., `course-details__name`).
- **Environment Variables**: Prefix with `VITE_` for client-side (e.g., `VITE_ALLOWED_HOST` in `vite.config.ts`).

## Integration Points

- **Server Configuration**: Vite dev server allows specific hosts including localhost and configurable extras via `ALLOWED_HOST` or `VITE_ALLOWED_HOST` env vars (`vite.config.ts`). Useful for development in different environments.
- **Package Manager**: Uses Bun (`bun@1.3.10`). Always use `bun` commands instead of `npm` or `yarn`.