# Authentication Strategy

> FribaScore authentication design — concrete Phase 1 plan plus Phase 2 readiness direction.

## Overview

FribaScore uses **ASP.NET Core Identity** for user authentication and session management. The current design prioritizes simplicity and offline-first resilience. Phase 1 is the implementation plan defined by issue #26; Phase 2 is documented only as a readiness path so future expansion can be evaluated without rewriting Phase 1.

---

## Phase 1: Identity + HttpOnly Cookies (Current plan)

### Architecture

| Component | Technology | Role |
|-----------|-----------|------|
| Framework | ASP.NET Core Identity | User registration, credential validation, session lifecycle |
| Storage | PostgreSQL via EF Core | Persists `AspNetUsers`, roles, claims |
| Session carrier | HttpOnly cookies | Server-issued authentication cookie carried automatically by the browser |
| Token storage | **None** | Cookies handle session — no JWT or `localStorage` involvement |

### Phase 1 Endpoint Contract (#26)

| Method | Path | Auth | Contract |
|--------|------|------|----------|
| `POST` | `/auth/login` | Public | Validates username + password, returns `200` with HttpOnly cookie on success, `401` on failure |
| `POST` | `/auth/logout` | Authenticated | Clears auth cookie |
| `GET` | `/auth/me` | Authenticated | Returns current user info (`id`, `username`) if authenticated, `401` otherwise |

### Session Flow

1. **Sign-in**
   - User sends credentials to `POST /auth/login`
   - Identity validates username/password against hashed store
   - ASP.NET Core issues HttpOnly cookie (signed, encrypted) on success
   - Cookie sent to client; client cannot read or manipulate it

2. **Authenticated requests**
   - Browser includes cookie in every request (automatic)
   - ASP.NET Core validates cookie signature and freshness
   - User identity available to endpoint handlers

3. **Sign-out**
   - User calls `POST /auth/logout`
   - ASP.NET Core clears the cookie
   - Client drops the cookie header

### Security Properties

| Property | Guarantee |
|----------|-----------|
| **HttpOnly** | JavaScript cannot access the cookie; XSS cannot steal it |
| **Secure** | Cookie only sent over HTTPS when secure cookies are enabled (required in production) |
| **SameSite=Strict** | Cookie not sent in cross-site requests; prevents CSRF |
| **Encryption** | Cookie contents encrypted — tampering detected on validation |
| **No JWT** | Tokens never stored in `localStorage` where XSS can reach them |
| **Server-side validation** | Session state validated on every request |

### Why This for Phase 1

- **Minimal code:** ASP.NET Core handles 95% of the implementation
- **Battle-tested:** Standard web app pattern since cookies were invented
- **Offline resilience:** The browser handles the auth cookie without client-side token storage logic
- **Single-app focus:** Fits FribaScore's current scope (one Disc Golf scoring app)
- **Mobile-friendly:** Works on all browsers; no local storage workarounds

---

## Phase 2+: Auth Expansion Readiness (Future direction)

Phase 2 is intentionally not a delivery commitment yet. It documents the conditions and constraints for revisiting auth once Phase 1 is live and product needs are clearer.

### When to Revisit

The current HttpOnly cookie model is the right default for a single web application. Revisit the auth architecture if FribaScore evolves to:

- **Multi-tenant SaaS:** Different organizations or clubs manage their own instances
- **Federated identity:** Users can sign in with credentials from a partner app
- **Public API:** External tools (mobile apps, third-party bots) need programmatic access
- **SSO:** Single sign-in across multiple related apps
- **Token-based clients:** Native mobile apps or automation need bearer tokens instead of browser cookies

### Phase 2 Constraints

- **Phase 1 remains the baseline.** Local username/password auth with HttpOnly cookies must keep working.
- **No client-side token storage.** The web app should not move to `localStorage` or similar token handling.
- **Identity stays central.** Reuse the existing user store and account model where practical.
- **Offline-first remains intact.** Online auth features must not make local score entry dependent on network access.

### Planning Guardrail

If those triggers become real, the project can evaluate whether an OIDC-capable extension on top of Identity is warranted. That future planning must not assume specific packages, providers, endpoints, tables, or token flows before the use case is validated.

### Questions Phase 2 Planning Must Answer

- Which concrete use case justifies adding OIDC or token-based access?
- Which external providers or client types actually need support?
- What account-linking rule is acceptable (email match, explicit linking, admin approval)?
- What additional security controls are required for redirect URIs, token lifetime, and client registration?

### Compatibility Goal

The current HttpOnly cookie model should be able to **coexist** with any future auth expansion work:
- Existing users continue using cookies
- New federation scenarios, if added, can introduce tokens separately
- The same Identity-backed user store remains the preferred starting point

---

## Comparison

| Aspect | Phase 1 (Cookies) | Phase 2+ (Future auth expansion) |
|--------|-------------------|-----------------|
| Suitable for | Single web app | Multi-app, federated, programmatic |
| Browser session model | Automatic cookie handling | Must be defined during Phase 2 planning |
| Client complexity | Low | Higher |
| Cross-origin support | Restricted by SameSite | Depends on the chosen token/client model |
| Implementation effort | Low (ASP.NET Core built-in) | Higher |

---

## Decision Record

**Status:** Active (Phase 1)  
**Decided by:** hjkuja, BE-8  
**Date:** 2026-04-09  
**Rationale:** HttpOnly cookies provide the simplest, most secure session model for FribaScore's current single-app scope. Any future auth expansion remains a documented direction only, to be justified by concrete product needs before implementation work starts.

---

## Related Docs

- [API Overview — Authentication section](../api/overview.md#authentication)
- [Architecture Overview — Auth subsection](../architecture/overview.md#auth)
- [Architecture — Authentication (LD-7, tactical implementation guide)](../architecture/auth.md)
- [Backend Sync Spec — Authentication](../specs/backend-sync.md#authentication)
