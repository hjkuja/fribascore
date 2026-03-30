# Testing this repo

Plan: Add Bun-based tests to fribascore/ui

Goal
- Add Bun's test runner to run unit, component (React) and integration tests using the Bun runtime, keeping Playwright (E2E) separate.
  - Maybe have differenct directories for bun tests, playwright tests (not in scope right now) and some other possible tests
- Provide clear install steps, setup files, scripts and CI notes so the team can opt into Bun testing or revert cleanly.

High-level approach
1. Add Bun dev-time dependencies and minimal polyfills.
2. Create a preload setup that supplies a DOM (HappyDOM) and test matchers before tests run.
3. Update package.json scripts and CI to run bun test, but make this optional until tests pass.

Dependencies to install (recommended with bun)
- bun add -d @testing-library/react @testing-library/jest-dom whatwg-fetch happy-dom msw
  (If already present via npm, bun will use node_modules; bun add makes them explicit for Bun runtime.)

Notes on versions
- Use versions compatible with React 19 and TypeScript 5.x; prefer current stable releases. Bun test supports TS/TSX out of the box.

Setup files
- Preload file (required by bun test): test/preloadBunSetup.ts
  - Purpose: create a global DOM (HappyDOM) before any tests import @testing-library/react.
  - Example content:

    import { Window } from 'happy-dom';
    const win = new Window();
    // copy the most common globals used by RTL
    // @ts-ignore
    globalThis.window = win;
    // @ts-ignore
    globalThis.document = win.document;
    // @ts-ignore
    globalThis.HTMLElement = win.HTMLElement;
    // @ts-ignore
    globalThis.getComputedStyle = win.getComputedStyle;
    // optional: set location, history, etc:
    // @ts-ignore
    globalThis.navigator = win.navigator;

    // Now load remaining test bootstrap
    import './setupTests';

- Keep test/setupTests.ts as the project already has, but ensure it does not assume Vitest globals. setupTests.ts should:
  - import '@testing-library/jest-dom';
  - import 'whatwg-fetch' (if fetch not available);
  - setup MSW server handlers: beforeAll(()=>server.listen()); afterEach(()=>server.resetHandlers()); afterAll(()=>server.close());

Test discovery and naming
- Bun will find *.test.{ts,tsx,js,jsx} and *_test.* and *.spec.*. Keep component tests as Component.test.tsx next to components.
- Suggested layout:
  - src/components/Component/Component.test.tsx (unit/component)
  - src/utils/thing.test.ts (unit)
  - test/mocks/* (msw handlers and server)
  - test/preloadBunSetup.ts and test/setupTests.ts (preload + bootstrap)

package.json scripts
- Add (optional) scripts for teams using Bun:
  "test:bun": "bun test --preload ./test/preloadBunSetup.ts",
  "test:bun:watch": "bun test --watch --preload ./test/preloadBunSetup.ts",
  "test:bun:coverage": "bun test --coverage --preload ./test/preloadBunSetup.ts"

Running locally
- Install deps (if using bun): bun install
- Run once: bun test --preload ./test/preloadBunSetup.ts
- Watch: bun test --watch --preload ./test/preloadBunSetup.ts

CI integration
- On GitHub Actions, install Bun (oven-sh/setup-bun) and run bun install; then run bun test.
- Example CI step:
  - name: Install bun
    uses: oven-sh/setup-bun@v2
  - name: Install deps
    run: bun install
  - name: Run Bun tests
    run: bun test --preload ./test/preloadBunSetup.ts --reporter=junit --reporter-outfile=./bun.xml

Playwright (E2E)
- Keep Playwright separate. Use Playwright test runner for true browser tests.
- Add e2e/ or tests/e2e/ directory and playwright.config.ts. Use webServer to start dev server (bun dev or npm run dev) and run playwright test.

Migration checklist (step-by-step)
1. Create test/preloadBunSetup.ts as above (HappyDOM) and commit to a branch.
2. Run bun test --preload ./test/preloadBunSetup.ts. Fix errors by adjusting setupTests and handlers.
3. Ensure MSW works: server should be the browser variant (setupWorker) or node variant compatible with Bun. For prerendered network mocks, prefer msw/browser where possible.
4. Add package.json scripts and CI step once passing.
5. Optionally set team default to using bun test; otherwise keep vitest for those who prefer it.

Potential pitfalls
- Some Node-only utilities or vitest-specific globals (globals:true) must be replaced or shimmed.
- MSW Node integration might need changes; prefer using browser worker handlers for DOM tests.
- Some test helpers that depend on jsdom quirks may behave differently in HappyDOM — run full suite and verify.

Verification
- Run bun test locally with preload. All component tests should render without ReferenceError: document is not defined.
- Ensure coverage and reporters produce expected outputs.

Rollback
- Revert added files (preloadBunSetup.ts) and package.json scripts. No change to existing vitest config is required.

Notes & references
- Bun docs: https://bun.com/docs/test (UI & DOM testing section) (use HappyDOM + React Testing Library)
- MSW: https://mswjs.io/ (choose setupWorker for browser-mode)

If desired next steps
- Implement preload file and re-run bun test; I can apply and fix compatibility issues on the branch.

Concrete ordered steps (actionable — run in order):

1. Create a feature branch
   - git switch -c feat/bun-tests

2. Install Bun dev dependencies
   - bun add -d @testing-library/react @testing-library/jest-dom whatwg-fetch happy-dom msw

3. Add preload file
   - Create test/preloadBunSetup.ts that establishes a HappyDOM window and copies common globals, then imports './setupTests'
   - Minimal example to include (summary):
     import { Window } from 'happy-dom';
     const win = new Window();
     // @ts-ignore
     globalThis.window = win;
     // @ts-ignore
     globalThis.document = win.document;
     // @ts-ignore
     globalThis.HTMLElement = win.HTMLElement;
     // @ts-ignore
     globalThis.getComputedStyle = win.getComputedStyle;
     // @ts-ignore
     globalThis.navigator = win.navigator;
     import './setupTests';

4. Update test/setupTests.ts (bootstrap)
   - import '@testing-library/jest-dom';
   - import 'whatwg-fetch';
   - Configure MSW for DOM tests using setupWorker (browser):
     import { setupWorker } from 'msw';
     import { handlers } from '../test/mocks/handlers';
     const worker = setupWorker(...handlers);
     beforeAll(() => worker.start());
     afterEach(() => worker.resetHandlers());
     afterAll(() => worker.stop());
   - Avoid Vitest-specific globals (e.g., vi) in this file so Bun can run it.

5. Organize MSW handlers
   - Move browser-oriented handlers to test/mocks/handlers.ts and export `handlers`.
   - If later needed, add test/mocks/server.ts for node-based tests (do not use it for DOM tests).

6. Test discovery and layout
   - Keep component tests next to components as Component.test.tsx
   - Keep utilities tests as *.test.ts

7. Run and iterate locally
   - bun install
   - bun test --preload ./test/preloadBunSetup.ts
   - Fix failing tests by adding shims, adjusting handlers, or adapting tests for HappyDOM differences.

8. Add optional scripts to package.json
   - "test:bun": "bun test --preload ./test/preloadBunSetup.ts"
   - "test:bun:watch": "bun test --watch --preload ./test/preloadBunSetup.ts"
   - "test:bun:coverage": "bun test --coverage --preload ./test/preloadBunSetup.ts"

9. CI integration (GitHub Actions)
   - Use oven-sh/setup-bun@v2 to install Bun, then run:
     bun install
     bun test --preload ./test/preloadBunSetup.ts --reporter=junit --reporter-outfile=./bun.xml
   - Add this as an optional job until the suite is stable.

10. Verify
   - Ensure component tests render without ReferenceError: document is not defined.
   - Confirm coverage/report files are generated as expected (bun.xml, coverage reports).

11. PR & communication
   - Open a PR from feat/bun-tests describing changes, test commands, and any compatibility notes.
   - Request reviewers from the team and document the rollback steps in the PR description.

12. Rollback (if needed)
   - Revert the branch or remove added scripts and files; vitest configuration and tests remain unchanged.

Quick checklist
- [ ] branch created
- [ ] deps installed
- [ ] test/preloadBunSetup.ts added
- [ ] test/setupTests.ts updated
- [ ] MSW handlers adapted
- [ ] local tests passing
- [ ] CI updated

Notes
- Prefer setupWorker (browser) for DOM tests and only use Node server handlers when running true node-based integration tests.
- HappyDOM may behave slightly differently than jsdom; expect a small iteration cycle.

If preferred, push the branch and I can continue iterating on failing tests and CI adjustments.
