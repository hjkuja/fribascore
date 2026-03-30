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

The frontend is organised into clear layers:

- **App shell** — routing, layout, and top-level providers
- **Components** — reusable UI pieces, each in its own subfolder with styles and tests
- **Pages** — one file per route, thin wrappers around components
- **Types** — TypeScript interfaces for the core domain entities
- **Utils** — shared logic (e.g. local storage access)
- **Data** — seed/dummy data used in development

## Routing

The app is a single-page application with client-side routing. All pages share a common layout shell that provides navigation.

The main sections of the app are: home, course browsing, round setup, live scoring, round summary, history, and settings. A dev-only admin/debug page is also available in development builds.

## Key Design Decisions

### Offline first, sync later
Data is always written to local storage first. The backend sync is additive and does not block any user interaction. This allows full use of the app without a network connection.

### Single local database
All app data lives in a single local database with separate stores for courses, rounds, and players. All reads and writes are centralised through one utility module — no component accesses storage directly.

### Seed data on first run
On first load, if no courses exist locally, a set of dummy courses is loaded automatically. This ensures the app is usable before any backend is available.

### Dev-only tools
A debug/admin page is only available in development builds. It provides tools for inspecting and resetting local data during development.

### Client-side IDs
All entities are assigned unique IDs on the client at creation time. This supports fully offline creation of rounds and players without any server involvement.
