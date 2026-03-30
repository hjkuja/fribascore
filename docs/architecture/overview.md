# System Overview

FribaScore is an offline-first disc golf scorecard web app built as a monorepo. This document covers the overall architecture, tech choices, and how the pieces fit together.

## Goals

- **Offline first** — the app must work fully without internet. Data lives in the browser's IndexedDB.
- **Mobile-friendly** — designed for field use on a phone browser.
- **PWA** — installable, fast load, works offline.
- **Optional cloud sync** — rounds and player data can be synced to a backend when the user is online and authenticated.

## Tech Stack

| Layer | Technology |
|-------|------------|
| Framework | React 19 (functional components, hooks) |
| Language | TypeScript (strict mode) |
| Build tool | Vite 7 + Babel React Compiler |
| Routing | React Router DOM v7 |
| Local storage | IndexedDB via `idb` v8 |
| Package manager | Bun |
| Linting | ESLint 9 with TypeScript + React Hooks plugins |
| Testing | Bun test runner + React Testing Library + happy-dom |

## Frontend Structure (`ui/`)

```
ui/
  src/
    app/           # Core app shell (App.tsx, AppLayout.tsx, AppRoutes.tsx)
    components/    # Reusable UI components, one subfolder each
    pages/         # Route-level page components
    types/         # TypeScript interfaces (Course, Hole, Player, Round, ScoreEntry)
    utils/         # Shared utilities (db.ts for IndexedDB access)
    data/          # Seed/dummy data for development
  test/            # Shared test setup and preloads
  public/          # Static assets
```

## Routing

All routes are nested under `AppLayout`, which provides the common shell (navigation, etc.).

| Route | Page | Description |
|-------|------|-------------|
| `/` | Home | Welcome screen |
| `/courses` | Course list | Browse available courses |
| `/courses/:id` | Course details | Hole-by-hole breakdown of a course |
| `/courses/:id/start-round` | Start round | Player selection before a round |
| `/rounds/:id/score` | Round scoring | Hole-by-hole score entry |
| `/rounds/:id/summary` | Round summary | Post-round totals and stats |
| `/history` | History | Past rounds list |
| `/settings` | Settings | Player management, sign-in status, sync status |
| `/__admin` | Admin | Dev-only debug/admin page (only in `import.meta.env.DEV`) |

## Key Design Decisions

### Offline first, sync later
Data is always written to IndexedDB first. The backend sync is additive and does not block any user interaction. This allows full use of the app without a network connection.

### Single IndexedDB database
The database (`fribascore`, version 1) contains three object stores: `courses`, `rounds`, and `players`, each keyed by `id`. All reads and writes go through `src/utils/db.ts`.

### Seed data on first run
On first load, if the `courses` store is empty, dummy courses are loaded from `src/data/dummyCourses.ts`. This ensures the app is usable before any backend is available.

### Dev-only routes
The `/__admin` route is only rendered when `import.meta.env.DEV` is true. It provides tools for inspecting and clearing IndexedDB during development.

### ID generation
All entity IDs are generated with `crypto.randomUUID()` (with a fallback to the `uuid` package) on the client. This supports fully offline creation of rounds and players.
