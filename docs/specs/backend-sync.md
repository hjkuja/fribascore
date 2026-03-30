# Backend Sync Spec

This document describes the planned sync strategy between the local IndexedDB store and the FribaScore backend API.

## Overview

FribaScore operates in fully local mode by default. When a user is signed in and connected to the internet, local data is synced with the backend. Sync is additive — it never breaks the offline experience.

## Sync Queue

A `syncQueue` object store will be added to IndexedDB. Every data mutation (creating a round, updating a player) that hasn't been confirmed by the backend is added to this queue.

### Queue entry shape

```ts
{
  id: string;           // UUID
  operation: "create" | "update" | "delete";
  entity: "round" | "player";
  entityId: string;
  payload: object;      // The data to send
  createdAt: string;    // ISO timestamp
  attempts: number;     // Retry count
}
```

## Sync Triggers

Sync is attempted:
1. On app load, if authenticated and online
2. When the browser fires the `online` event
3. After a round is completed (immediate flush attempt)

## Entity Sync Rules

### Rounds
- Direction: **local → remote**
- Rounds are created locally and uploaded when online
- Once a round is synced, its `syncedAt` field is set; it is not re-uploaded
- Rounds are never modified after creation (immutable)

### Players
- Direction: **bidirectional**
- Local players are pushed to the server; remote players are pulled down
- Conflict resolution: last-write-wins based on an `updatedAt` timestamp
- Player deletions are queued and synced as a `delete` operation

### Courses
- Direction: **remote → local**
- The backend is authoritative for course data
- On sync, courses are refreshed from the API and upserted into the local `courses` store
- Local seed data (`dummyCourses.ts`) is replaced by real data after first sync

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
