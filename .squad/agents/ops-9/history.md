# OPS-9 — History

## Core Context

- **Project:** FribaScore — offline-first disc golf scorecard PWA
- **Owner:** @hjkuja
- **Stack:** React 19, TypeScript, Vite 7, Bun, IndexedDB (idb v8), React Router v7
- **Test stack:** Bun test runner, React Testing Library, happy-dom
- **Build output:** `dist/` (Vite)
- **CI:** `.github/workflows/ci.yml` — lint → typecheck → build → artifact upload
- **Squad workflows:** `squad-heartbeat.yml`, `squad-triage.yml`, `squad-issue-assign.yml`, `sync-squad-labels.yml`

## Learnings

- **2025-01:** Removed `schedule:` trigger from `squad-heartbeat.yml` (both `.github/workflows/` and template copy). Ralph now event-driven only (`issues`, `pull_request`, `workflow_dispatch`). Saves ~1,440 cron runs/month that polled with no work. Only location in repo with `schedule:` or `cron:` triggers — removal complete.

<!-- Append new learnings below. -->
