<!-- Backend appends project-specific learnings here. Never cleared. -->

[2026-04-05] [LEARNING] Keep backend work split across `FribaScore.Api`, `FribaScore.Application`, and `FribaScore.Contracts`; endpoints call services and `Contracts` stays framework-light.
Source: legacy backend history record deleted during migration; `api/src/FribaScore.Api/Program.cs`; `api/src/FribaScore.Application/ServiceExtensions.cs`; `docs/api/overview.md`

[2026-04-05] [LEARNING] Minimal API `WithName()` values must be globally unique; append a resource suffix such as `GetAllCourses`.
Source: legacy backend history record deleted during migration; `api/src/FribaScore.Api/Endpoints/Courses/CourseEndpoints.cs`

[2026-04-05] [LEARNING] Prefer built-in ASP.NET Core OpenAPI plus Scalar UI before adding Swagger-specific dependencies.
Source: legacy backend history record deleted during migration; `api/src/FribaScore.Api/FribaScore.Api.csproj`; `api/src/FribaScore.Api/Program.cs`

[2026-04-09] [LEARNING] Keep browser auth cookie-based with ASP.NET Core Identity and avoid JWT or `localStorage` token flows for the web client.
Source: legacy decisions record deleted during migration; `docs/architecture/auth.md`; `api/src/FribaScore.Api/Program.cs`
