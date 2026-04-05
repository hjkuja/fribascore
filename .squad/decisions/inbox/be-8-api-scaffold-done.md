# BE-8: API Scaffold Restructure Complete

**Date:** 2026-04-05  
**Agent:** BE-8 (Backend Dev)  
**Issue:** #25 — Full API scaffold  
**Branch:** squad/25-api-scaffold

## Decision

The API scaffold restructure is complete. The following architectural decisions were made and should be recorded:

### Minimal API over MVC Controllers

All MVC Controllers (`CoursesController`, `PlayersController`, `RoundsController`) have been removed. Replaced with Minimal API endpoint classes using `MapGroup()`, `TypedResults`, and `RequireAuthorization()`. This aligns with .NET 10 conventions and keeps the codebase lean.

### Solution Format: SLNX

`fribascore.sln` (GUID-based) is replaced by `fribascore.slnx` (XML, no GUIDs). Easier to read and maintain in version control.

### NuGet Lockfiles Committed

`Directory.Build.props` at `api/` root enables `RestorePackagesWithLockFile=true` for all projects. All three `packages.lock.json` files are committed. `RestoreLockedMode` activates only in CI.

### Packages Added

- `LanguageExt.Core` 4.4.9 — for Result/Option patterns in service layer (future)
- `Scalar.AspNetCore` 2.9.0 — OpenAPI UI (replaces raw JSON endpoint)

### Test Project Layout

- `api/test/FribaScore.Api.Tests.Unit/` — unit tests (xUnit)
- `api/test/FribaScore.Api.Tests.Integration/` — integration tests (xUnit + `Microsoft.AspNetCore.Mvc.Testing`)

**Status:** Ready for PR review.
