# Authentication Architecture

**Status:** Phase 1 planning (Phase 1 endpoints in #26)  
**Decision Owner:** LD-7 · Approved by hjkuja  
**Date:** 2026-04-09

---

## Goals

1. **Phase 1** — Simple, secure authentication for MVP: self-hosted app, single-tenant, username/password login
2. **Phase 2 readiness** — Design Phase 1 so it can coexist with future OIDC/SSO work later without breaking existing users
3. **Security first** — HttpOnly cookies (XSS protection), SameSite=Strict (CSRF), hashed passwords (Identity)
4. **Offline sync** — Client-side rounds and players created offline must sync to the authenticated user's backend account post-login

---

## Phase 1: Local Cookie Auth (MVP)

### Technology Stack

- **Identity system:** ASP.NET Core Identity (built-in, battle-tested, PostgreSQL-compatible)
- **Password hashing:** ASP.NET Core Identity password hasher (framework-managed)
- **Session storage:** HttpOnly cookie, `SameSite=Strict`, secure in production
- **API transport:** HTTPS in production; local development should prefer HTTPS as well

### Design

```
User submits (username, password)
         ↓
POST /auth/login (public)
         ↓
Identity validates credentials against the configured user store
         ↓
Set HttpOnly cookie (SameSite=Strict, Secure=true in production)
         ↓
200 OK on success / 401 on failure
```

### Endpoints (Issue #26)

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| `POST` | `/auth/login` | Public | `{ username, password }` → `200` with HttpOnly cookie on success, `401` on failure |
| `POST` | `/auth/logout` | Authenticated | Clears auth cookie |
| `GET` | `/auth/me` | Authenticated | Returns `{ id, username }` if authenticated, `401` otherwise |

### Data Model

Use `IdentityDbContext<AppUser>` as the auth store. Extend `AppUser` only for app-specific fields that Phase 1 actually needs; do not create a parallel `users` table that duplicates Identity data.

### Security Checklist ✓

- [x] HttpOnly flag prevents XSS token theft
- [x] SameSite=Strict prevents CSRF
- [x] Secure cookies are required in production
- [x] Passwords never logged or exposed in errors
- [x] Protected endpoints opt into authorization
- [ ] Login throttling / rate limiting still needs an explicit Phase 1 follow-up
- [x] Session lifetime is enforced by cookie configuration

---

## Phase 2: Auth Expansion Readiness (Future direction)

### Rationale

Phase 1 works for the current MVP. Revisit auth expansion planning only if product needs clearly grow beyond that, for example:
- External identity providers (Google, GitHub, Microsoft, custom LDAP/Okta)
- Multi-tenant SaaS deployment
- Federated identity across multiple apps
- Standards-compliant (OpenID Connect)

That does **not** mean FribaScore is committing to Phase 2 implementation now. It means Phase 1 should avoid boxing us out of a future OIDC-capable design.

### Design Goals

1. **Coexistence** — Phase 1 users keep their local credentials; future auth additions must not break them
2. **Client stability** — The browser app should continue to rely on HttpOnly cookies for its primary session
3. **Minimal migration risk** — Reuse the Identity-backed user store where practical

### Readiness Constraints

- Do not move the web client to bearer tokens or `localStorage`
- Keep local auth viable even if external providers are added later
- Preserve the offline-first user experience for local scoring and sync preparation
- Treat provider selection, account-linking rules, and token surface area as Phase 2 decisions

### Planning Guardrail

If Phase 2 becomes justified, evaluate the need for an OIDC-capable extension on top of Identity during that planning cycle. Do not treat packages, providers, endpoints, database changes, or token flows as committed before that future work is scoped.

### Key Architectural Decision

> **HttpOnly cookies remain the Phase 1 session mechanism and the default expectation for the web client.**
> 
> If Phase 2 happens, it must not quietly redefine the browser client contract established in Phase 1.

---

## Client-Side Integration

### Phase 1

```typescript
// POST /auth/login (browser sends HttpOnly cookie automatically)
const response = await fetch('/auth/login', {
  method: 'POST',
  credentials: 'include', // Include cookies
  body: JSON.stringify({ username, password })
});

if (response.ok) {
  // App can refresh authenticated state after login succeeds
}

// GET /auth/me (check if authenticated, cookie sent automatically)
const me = await fetch('/auth/me', { credentials: 'include' });
```

### Phase 2

No browser-side auth redesign is planned today. Any future auth expansion should be defined in a separate planning issue after Phase 1 is complete.

---

## Offline Sync Context

When a user creates rounds/players offline (in IndexedDB), then later authenticates:

1. **Login:** `POST /auth/login` → user now has `UserId` claim in principal
2. **Sync request:** Client calls `POST /sync/upload` with offline rounds + players
3. **Backend enriches:** Sync service queries `User.Id` from `HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)`
4. **Database insert:** Rounds/players get `created_by_user_id = User.Id` + timestamps

**Architecture:** No changes to sync flow. `UserId` is always available via claims after login.

---

## Testing Strategy

### Unit Tests (Phase 1)

- [ ] `LoginService_ValidPassword_ReturnsUser`
- [ ] `LoginService_InvalidPassword_ThrowsException`
- [ ] `AuthController_Login_SetsCookie`
- [ ] `AuthController_Logout_ClearsCookie`
- [ ] `AuthController_Me_ReturnsClaims`

### Integration Tests (Phase 1)

- [ ] Login endpoint with real Identity store
- [ ] Cookie lifecycle (set → logout → cleared)
- [ ] Unauthorized access to protected endpoints
- [ ] Session timeout scenario

### Phase 2 Planning Checks

- [ ] Confirm there is a real product need for federation, external providers, or token clients
- [ ] Document account-linking and provider-trust rules before any implementation issue is opened
- [ ] Split future implementation work into separate issues only after the planning decision is made

---

## Open Questions / Future Decisions

1. **Rate limiting:** Should login attempts be throttled? (HTTP 429, exponential backoff)
2. **Multi-device sessions:** Should we track active sessions per device, or invalidate all on logout?
3. **Provider trust:** Which external identity sources are acceptable, if any?
4. **Account linking:** Should same-email matching be automatic, explicit, or admin-mediated?

---

## References

- [Issue #26 — Auth endpoints](https://github.com/hjkuja/fribascore/issues/26)
- [ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity)
- [OWASP: Authentication Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Authentication_Cheat_Sheet.html)
