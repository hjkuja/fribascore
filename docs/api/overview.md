# API Overview

This document describes the planned .NET backend API for FribaScore.

## Status

⬜ Not yet started. The backend will be scaffolded in a `backend/` directory at the monorepo root.

## Technology

| Layer | Technology |
|-------|------------|
| Framework | ASP.NET Core Web API |
| Language | C# |
| Auth | To be decided (JWT or ASP.NET Core Identity) |
| Database | To be decided |

## Base URL

- Development: `https://localhost:5001`
- Production: TBD

The frontend reads the API base URL from a `VITE_API_URL` environment variable.

## Authentication

All endpoints except `GET /courses` require authentication. Auth is handled via bearer tokens (JWT).

| Endpoint | Description |
|----------|-------------|
| `POST /auth/login` | Sign in and receive a JWT |
| `POST /auth/logout` | Invalidate the current session |
| `GET /auth/me` | Get the currently authenticated user |

## Courses

Courses are managed server-side. The frontend syncs them down on connect.

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/courses` | List all courses |
| `GET` | `/courses/{id}` | Get a single course with full hole data |

## Players

Players are user-owned and synced bidirectionally.

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/players` | List all players for the authenticated user |
| `POST` | `/players` | Create a new player |
| `PUT` | `/players/{id}` | Update a player |
| `DELETE` | `/players/{id}` | Delete a player |

## Rounds

Rounds are created locally and uploaded when online.

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/rounds` | List rounds for the authenticated user |
| `POST` | `/rounds` | Upload a completed round |

> Rounds are considered immutable once created. There is no `PUT /rounds/{id}`.

## Sync

The frontend uses a sync queue to batch and retry failed operations. The backend does not have a dedicated sync endpoint — operations are sent as individual requests against the entity endpoints above.

See [Backend Sync Spec](../specs/backend-sync.md) for the full sync strategy.

## Error Responses

All error responses follow this shape:

```json
{
  "error": "string",
  "details": "string (optional)"
}
```

Standard HTTP status codes are used:

| Code | Meaning |
|------|---------|
| 200 | OK |
| 201 | Created |
| 400 | Bad request (validation error) |
| 401 | Unauthorized |
| 404 | Not found |
| 500 | Server error |
