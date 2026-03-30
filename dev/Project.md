# FribaScore — Project Master Document

## Description

FribaScore is an offline-first Frisbee Golf (disc golf) scorecard web app. Players can browse courses, start a round, enter scores hole-by-hole, and view a round summary — all without an internet connection. Data is persisted locally in IndexedDB and the app is designed to eventually sync with a backend when online and signed in.

The main goal is to have an app with local data if the user doesn't have sufficient internet on the go. The cloud synchronization can be done once the user has internet access.

## Requirements

- Browse and view disc golf courses with hole-by-hole details
- Start a round on any course with up to 6 players
- Record scores per player per hole during a round
- View a round summary with total scores and relative-to-par
- All data must work fully offline (IndexedDB-first)
- Responsive UI suitable for use on a mobile browser in the field
- PWA support: installable, works offline, fast load
- Optional: sync rounds and player data to a backend when online

---

## Tech Stack

| Layer | Technology |
| --- | --- |
| Framework | React 19 (functional components, hooks) |
| Language | TypeScript (strict mode) |
| Build tool | Vite 7 + Babel React Compiler |
| Routing | React Router DOM v7 |
| Local storage | IndexedDB via `idb` v8 |
| Package manager | Bun |
| Linting | ESLint 9 with TypeScript + React Hooks plugins |

---

## Dev Commands

```bash
bun dev          # Start dev server (accessible on local network)
bun run build    # Type-check + production bundle
bun lint         # Run ESLint
bun preview      # Serve the production build locally
```

---

## Architecture Notes

- **`src/app/`** — Core app shell: `App.tsx`, `AppLayout.tsx`, `AppRoutes.tsx`
- **`src/components/`** — Reusable UI components, one subfolder each with `.tsx` and `.css`
- **`src/pages/`** — Route-level page components
- **`src/types/`** — TypeScript interfaces (`Course`, `Hole`, `Player`, `Round`, `ScoreEntry`)
- **`src/utils/db.ts`** — All IndexedDB access (get, save, seed, delete)
- **`src/data/`** — Dummy/seed data used in development
- CSS follows BEM-like naming (e.g. `round-summary__table`)
- Type-only imports use the `import type` keyword
- Dev-only routes (e.g. `/__admin`) are gated by `import.meta.env.DEV`

---

## Core Data Types and Start Round

**Status:** ✅ Complete

- [x] Add `Player` type to `types/`
- [x] Add `Round` type to `types/`
- [x] Add `ScoreEntry` type to `types/`
- [x] Add "Start Round" button/link on CourseDetails
- [x] Create `StartRound` page with player name input (up to 6 players)
- [x] Wire `StartRound` route in `AppRoutes.tsx`

---

## Local Storage (IndexedDB)

**Status:** ✅ Complete

- [x] Install `idb` library
- [x] Create `utils/db.ts` with IndexedDB setup
- [x] `courses` object store with seed logic
- [x] `rounds` object store with get/save
- [x] `players` object store with get/save/add/delete

---

## Round Scoring

**Status:** ✅ Complete

- [x] Create `RoundScoring` page
- [x] Create `ScoreCard` component with hole-by-hole navigation
- [x] Score input per player per hole
- [x] Auto-save scores to IndexedDB on change
- [x] Navigate to summary on round completion
- [x] Extract `HoleScore` as a dedicated sub-component

---

## Remaining Pages and Polish

**Status:** 🔁 In progress

- [x] Round Summary page (totals, relative-to-par, course info)
- [x] Player management UI (in dev Admin panel)
- [x] History page — list past rounds from IndexedDB
- [x] Settings page — player management, sign-in status, sync status (accessible outside dev Admin)
- [ ] PWA manifest (`manifest.json`)
- [ ] Service worker registration (offline caching)
- [ ] Install prompt / "Add to Home Screen" support

---

## Backend Integration

**Status:** ⬜ Not Started

- [ ] `syncQueue` object store for deferred backend sync
- [ ] Set up .NET Web API project in `backend/`
- [ ] Courses endpoint (GET list, GET by id)
- [ ] Players endpoint (CRUD)
- [ ] Rounds endpoint (GET list, POST new round)
- [ ] Authentication (sign-in / sign-out)
- [ ] Frontend sync logic — upload local rounds when online + signed in
- [ ] Use `syncQueue` store to queue failed/offline requests
- [ ] Merge strategy for conflicts between local and remote data
