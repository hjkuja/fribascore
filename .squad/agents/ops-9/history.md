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
- **2025-01:** Removed `go:*` label system (yes/no/needs-research) from both `sync-squad-labels.yml` and `squad-triage.yml`. Removed auto-application of `go:needs-research` during triage. Decision: Type/priority labels cover intent without extra friction. Changes applied to both `.github/workflows/` and `.squad/templates/workflows/` copies. Stale labels may exist on GitHub but will age out naturally.

- **2026-04:** Created `.github/workflows/api.yml` on branch `squad/25-api-scaffold`. Separate from `ci.yml`; triggers on `api/**` and `.github/workflows/api.yml` paths only. Uses `actions/setup-dotnet@67a3573c9a986a3f9c594539f4ab511d57bb3ce9` (#v4) with `cache: 'nuget'` and `cache-dependency-path: '**/packages.lock.json'`. Steps: restore `--locked-mode` → build `--no-restore -c Release` → test `--no-build -c Release`, all against `fribascore.slnx` at repo root. All third-party actions pinned to full commit SHA. `permissions: contents: read` (least privilege).

<!-- Append new learnings below. -->
