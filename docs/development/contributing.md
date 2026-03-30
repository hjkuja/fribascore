# Contributing

Guidelines for contributing to FribaScore. Following these conventions keeps the codebase consistent and makes reviews easier.

## Workflow

1. Create a feature branch from `main`:
   ```bash
   git checkout -b feat/my-feature
   # or: fix/bug-description, docs/topic, chore/task
   ```
2. Make your changes, keeping commits small and focused
3. Run lint, typecheck, and tests before pushing:
   ```bash
   bun lint && bun run typecheck && bun test
   ```
4. Open a pull request against `main`

## Branch Naming

| Prefix | Use for |
|--------|---------|
| `feat/` | New features |
| `fix/` | Bug fixes |
| `docs/` | Documentation changes |
| `chore/` | Tooling, config, maintenance |
| `refactor/` | Code restructuring without behavior change |

## Code Style

### TypeScript
- **Strict mode** is enabled — no implicit `any`, no unused variables/parameters
- Use `import type` for type-only imports:
  ```ts
  import type { Course } from "../../types/course";
  ```
- Use `interface` for object shapes, `type` for unions/aliases
- IDs are generated with `crypto.randomUUID()`

### React Components
- Functional components only — no class components
- Define explicit prop interfaces (e.g., `interface CourseDetailsProps { ... }`)
- PascalCase for component names and their files (`CourseDetails.tsx`)
- One component per file; component and its CSS file live in a subfolder:
  ```
  components/
    CourseDetails/
      CourseDetails.tsx
      CourseDetails.css
      CourseDetails.test.tsx
  ```

### CSS
- BEM-like naming: `block__element--modifier`
  - Example: `course-details__name`, `score-card__hole--active`
- One CSS file per component, imported directly in the component file
- No global styles except in `app/` level files

### Accessibility
- Include `aria-label` on all interactive elements that lack visible text labels
- Use semantic HTML: `<article>`, `<table>`, `<nav>`, `<main>`, `<button>` appropriately
- Tables need `aria-label` or `<caption>`

## File Organization

```
src/
  app/           # App shell only — App.tsx, AppLayout.tsx, AppRoutes.tsx
  components/    # Reusable UI — each in its own subfolder
  pages/         # Route-level pages — one file per route
  types/         # TypeScript interfaces only — no logic
  utils/         # Shared utilities — db.ts, etc.
  data/          # Static seed/dummy data
```

Do not add business logic to `pages/` — extract it into `utils/` or component hooks. Pages should be thin wrappers around components.

## Environment and Dev Tools

- Use `bun` — never `npm` or `yarn`
- Dev-only features (like the `/__admin` route) must be gated with `import.meta.env.DEV`
- Do not commit `.env.local` or any file containing secrets

## Commit Messages

Use the conventional commits format where possible:

```
feat: add round history page
fix: prevent score from being saved as NaN
docs: update getting started guide
chore: upgrade vite to v7
```

## Before Merging

- [ ] `bun lint` passes with no errors
- [ ] `bun run typecheck` passes
- [ ] `bun test` passes
- [ ] `bun run build` succeeds
- [ ] New functionality has at least a basic component test
