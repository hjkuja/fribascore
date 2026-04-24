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
| [`api/`](./api) | ASP.NET Core 10 Minimal API backend (.NET + EF Core + PostgreSQL) |
| [`docs/`](./docs) | Project architecture, API, and development documentation |

## Common Commands

### Frontend (`ui/`)

```bash
cd ui
bun dev              # Start dev server
bun run build        # Type-check + production bundle
bun lint             # Run ESLint
bun run typecheck    # Run TypeScript compiler
bun test             # Run tests
bun preview          # Preview production build locally
```

### Backend (`api/`)

```bash
dotnet restore api/fribascore.slnx
dotnet build api/fribascore.slnx
dotnet test api/fribascore.slnx
dotnet run --project api/src/FribaScore.Api
```

## Documentation

Full project docs live in [`docs/`](./docs/README.md):

- [Architecture](./docs/architecture/overview.md)
- [Getting Started](./docs/development/getting-started.md)
- [Testing](./docs/development/testing.md)
- [Contributing](./docs/development/contributing.md)
- [API Overview](./docs/api/overview.md)
