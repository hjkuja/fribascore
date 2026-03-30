# Getting Started

This guide covers everything you need to run FribaScore locally for development.

## Prerequisites

- [Bun](https://bun.sh) v1.3.10 or later — used as the package manager, runtime, and test runner
- A modern browser (Chrome or Firefox recommended for IndexedDB DevTools)
- Git

## Setup

```bash
# Clone the repo
git clone https://github.com/your-org/fribascore.git
cd fribascore

# Install dependencies for the frontend
cd ui
bun install
```

## Running the App

```bash
# Start the development server (accessible on local network)
bun dev
```

The dev server starts at `http://localhost:5173` by default and is also accessible on your local network IP (useful for testing on a phone).

## Environment Variables

The following environment variables can be set in a `.env.local` file inside `ui/`:

| Variable | Description | Default |
|----------|-------------|---------|
| `VITE_ALLOWED_HOST` | Additional allowed host for the Vite dev server | — |

> Most development workflows don't require any env vars. `.env.local` is gitignored.

## Build

```bash
# Type-check and produce a production bundle in ui/dist/
bun run build
```

The build output is in `ui/dist/`. You can preview it locally:

```bash
bun preview
```

## Linting

```bash
bun lint
```

Runs ESLint with TypeScript and React Hooks rules. Fix lint errors before committing.

## Type Checking

```bash
bun run typecheck
```

Runs `tsc --noEmit` over both `tsconfig.app.json` and `tsconfig.test.json` without emitting files. Equivalent to what CI runs.

## Project Structure

```
fribascore/           # Monorepo root
  ui/                 # React frontend
    src/
      app/            # App shell (routing, layout)
      components/     # Reusable UI components
      pages/          # Route-level page components
      types/          # TypeScript interfaces
      utils/          # Shared utilities (db.ts for IndexedDB)
      data/           # Seed/dummy data
    test/             # Test setup and shared helpers
    public/           # Static assets
  docs/               # Project documentation (you are here)
  dev/                # Raw scratch notes and planning
```

## Dev-Only Features

When running `bun dev`, an extra route is available at `/__admin`. This page provides tools for inspecting and clearing IndexedDB. It is not included in production builds.

## IndexedDB

All app data lives in the browser's IndexedDB (`fribascore` database). On first run, seed courses are loaded automatically from `src/data/dummyCourses.ts`.

To reset the database during development, use the `/__admin` page or clear site data in your browser's DevTools (Application → Storage → Clear site data).
