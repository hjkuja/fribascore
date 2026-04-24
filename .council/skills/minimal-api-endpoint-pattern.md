# Minimal API endpoint pattern

Use this when adding or refactoring endpoints under `api/src/FribaScore.Api/Endpoints/`.

## Pattern

1. Keep endpoint classes `static` and map routes by resource group.
2. Inject service interfaces into handlers instead of reaching for `DbContext`.
3. Return `TypedResults` and map failures through `ApiResults.ToProblemResult()`.
4. Keep request/response DTOs in `Contracts` and entity mapping in `Application/Mapping`.
5. Make every `WithName()` value globally unique.

## Why

This keeps the HTTP layer thin, preserves OpenAPI inference, and matches the structure already used by the repo.

Source: `.github/copilot-instructions.md`; legacy backend history record deleted during migration; `api/src/FribaScore.Api/Endpoints/Courses/CourseEndpoints.cs`; `api/src/FribaScore.Api/ApiResults.cs`
