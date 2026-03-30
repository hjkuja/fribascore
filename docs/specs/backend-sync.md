# Backend Sync Spec

This document describes the planned sync strategy between the local IndexedDB store and the FribaScore backend API.

## Overview

FribaScore operates in fully local mode by default. When a user is signed in and connected to the internet, local data is synced with the backend. Sync is additive — it never breaks the offline experience.

## Sync Queue

A sync queue is maintained in local storage. Every data mutation that hasn't been confirmed by the backend is added to this queue. The queue is flushed when sync conditions are met and retried on failure.

## Sync Triggers

Sync is attempted:
1. On app load, if authenticated and online
2. When the browser fires the `online` event
3. After a round is completed (immediate flush attempt)

## Entity Sync Rules

### Rounds
- Direction: **local → remote**
- Rounds are created locally and uploaded when online
- Once synced, a round is not re-uploaded
- Rounds are considered immutable after creation

### Players
- Direction: **bidirectional**
- Local players are pushed to the server; remote players are pulled down
- Conflict resolution: last-write-wins based on modification time
- Deletions are queued and synced

### Courses
- Direction: **remote → local**
- The backend is authoritative for course data
- On sync, courses are refreshed from the API and replace local data
- Local seed data is superseded by real data after first sync

## Authentication

Sync requires an authenticated session. The auth flow is handled by the backend (see [API Overview](../api/overview.md)).

- Unauthenticated users: fully local, no sync attempted
- After sign-in: immediate sync attempt
- After sign-out: sync queue is preserved locally (not cleared), syncs resume on next sign-in

## Error Handling

- Network errors: retry on next sync trigger, increment `attempts` counter
- After 5 failed attempts, the entry is flagged as `failed` and surfaced to the user
- Server errors (4xx): log and skip — do not retry (likely a data issue)
- Auth errors (401): pause sync until re-authenticated

## Future Considerations

- **Merge conflicts:** The current last-write-wins strategy is simple but may not scale. A more robust approach (e.g., CRDTs or server-side merge with user prompt) can be introduced as needed.
- **Partial sync:** Large history syncs may need batching to avoid slow uploads on first sign-in.
- **Optimistic UI:** For near-realtime use cases, optimistic updates with rollback on failure could improve perceived performance.
