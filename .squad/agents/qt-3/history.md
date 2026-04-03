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

<!-- Append new learnings below. -->
