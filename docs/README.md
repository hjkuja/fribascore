# FribaScore Documentation

Welcome to the FribaScore project docs. This directory contains the canonical technical documentation for the project.

> **Note:** The `dev/` directory at the repo root contains raw scratch notes and planning documents. `docs/` is the clean, maintained reference.

## Contents

### Architecture
- [System Overview](./architecture/overview.md) — tech stack, component structure, design decisions
- [Offline Strategy](./architecture/offline-strategy.md) — IndexedDB-first data layer and future sync design

### Specifications
- [Requirements](./specs/requirements.md) — functional and non-functional requirements
- [UI Design Guidelines](./specs/ui-design.md) — colour palette, typography, components, animation
- [PWA Spec](./specs/pwa.md) — installability, offline caching, service worker
- [Backend Sync Spec](./specs/backend-sync.md) — sync strategy between local and remote data

### Development
- [Getting Started](./development/getting-started.md) — setup, dev server, build, environment
- [Testing](./development/testing.md) — test runner, writing tests, CI
- [Contributing](./development/contributing.md) — code style, conventions, PR workflow

### API
- [API Overview](./api/overview.md) — planned .NET backend API design

## Project at a Glance

FribaScore is an offline-first disc golf (frisbee golf) scorecard web app. Players can browse courses, start a round, enter scores hole-by-hole, and view a round summary — all without an internet connection. Cloud sync is planned for a future phase.

**Monorepo layout:**

| Directory | Description |
|-----------|-------------|
| `ui/` | React 19 frontend (Vite + TypeScript + Bun) |
| `dev/` | Raw planning and scratch notes (not canonical) |
| `docs/` | This directory — maintained technical documentation |
| `backend/` | _(planned)_ .NET Web API |
