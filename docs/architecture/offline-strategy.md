# Offline Strategy

FribaScore is built with an offline-first philosophy: all core functionality works without any network connection. This document describes how local data persistence works today and how cloud sync will be layered on top in the future.

## Current State: IndexedDB-First

All app data is stored in the browser's IndexedDB. The database is named `fribascore` (version 1) and has three object stores:

| Store | Key | Description |
|-------|-----|-------------|
| `courses` | `id` | Disc golf courses with hole data |
| `rounds` | `id` | Completed and in-progress rounds |
| `players` | `id` | Locally registered players |

All reads and writes go through `ui/src/utils/db.ts`. Components never access IndexedDB directly.

### Seed data

On first load, if the `courses` store is empty, seed courses are loaded from `src/data/dummyCourses.ts`. This makes the app immediately usable without any backend.

### Score saving

Scores are auto-saved to IndexedDB as the user enters them hole-by-hole. There is no explicit "save" button during a round. This makes the app resilient to accidental browser refreshes or navigating away.

## Planned: Backend Sync

When a user is signed in and has a network connection, local data should be synced to the backend. The sync is entirely optional — the app remains fully functional without it.

### Sync queue

A `syncQueue` object store will be added to IndexedDB to queue operations that haven't been sent to the backend yet. This allows the app to record sync intent while offline and flush the queue when connectivity is restored.

### What gets synced

| Entity | Direction | Notes |
|--------|-----------|-------|
| Rounds | Local → Remote | Completed rounds uploaded when online |
| Players | Bidirectional | Local players pushed; remote players pulled |
| Courses | Remote → Local | Courses are authoritative on the server |

### Conflict resolution

The initial sync strategy is simple: local data wins for rounds (since rounds are user-owned). For courses, remote data wins (the server is authoritative). Player data uses a last-write-wins approach based on `updatedAt` timestamp.

More sophisticated conflict strategies can be introduced as the backend matures.

### Sync triggers

Sync will be triggered:
1. On app load, if the user is authenticated and online
2. When the app regains network connectivity (via the `online` browser event)
3. After a round is completed

### Authentication dependency

Sync requires authentication. Unauthenticated users always operate in fully local mode. Sign-in will be handled by the backend (see [API Overview](../api/overview.md)).
