# FribaScore

An offline-first disc golf (frisbee golf) scorecard web app. Browse courses, start a round, enter scores hole-by-hole, and view a summary — all without an internet connection. Cloud sync is planned for a future phase.

## Quick Start

```bash
cd ui
bun install
bun dev        # http://localhost:5173
```

## Packages

| Directory | Description |
|-----------|-------------|
| [`ui/`](./ui) | React 19 frontend (Vite + TypeScript + Bun) |
| `backend/` | _(planned)_ .NET Web API |

## Common Commands

All commands are run from the `ui/` directory.

```bash
bun dev              # Start dev server
bun run build        # Type-check + production bundle
bun lint             # Run ESLint
bun run typecheck    # Run TypeScript compiler
bun test             # Run tests
bun preview          # Preview production build locally
```

## Documentation

Full project docs live in [`docs/`](./docs/README.md):

- [Architecture](./docs/architecture/overview.md)
- [Getting Started](./docs/development/getting-started.md)
- [Testing](./docs/development/testing.md)
- [Contributing](./docs/development/contributing.md)
- [API Overview](./docs/api/overview.md)
