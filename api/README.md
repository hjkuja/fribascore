# FribaScore API

ASP.NET Core 10 Web API for FribaScore — disc golf scorecard backend.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL (local or Docker)

## Local Dev Setup

### 1. Start PostgreSQL

```bash
docker run -d \
  --name fribascore-db \
  -e POSTGRES_DB=fribascore \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  postgres:16
```

### 2. Restore packages

```powershell
dotnet restore
```

### 3. Apply EF Core migrations

```powershell
dotnet ef database update --project src/FribaScore.Application --startup-project src/FribaScore.Api
```

### 4. Run the API

```powershell
dotnet run --project src/FribaScore.Api
```

API will be available at `https://localhost:7xxx` (see `src/FribaScore.Api/Properties/launchSettings.json` for exact port).

OpenAPI spec: `https://localhost:7xxx/openapi/v1.json`  
Scalar UI: `https://localhost:7xxx/scalar/v1`

## Project Structure

```
api/
  Directory.Build.props          # NuGet lock file settings
  src/
    FribaScore.Api/
      Endpoints/                 # Minimal API endpoint handlers
        Courses/CourseEndpoints.cs
        Players/PlayerEndpoints.cs
        Rounds/RoundEndpoints.cs
        EndpointExtensions.cs
      Program.cs                 # Top-level statements entry point
    FribaScore.Application/
      Database/AppDbContext.cs   # EF Core + Identity DbContext
      Models/                    # Entity models
  test/
    FribaScore.Api.Tests.Unit/
    FribaScore.Api.Tests.Integration/
```

## Running Tests

```powershell
dotnet test api/test/FribaScore.Api.Tests.Unit
dotnet test api/test/FribaScore.Api.Tests.Integration
```

## Connection String

Override via environment variable or `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=fribascore;Username=postgres;Password=postgres"
  }
}
```
