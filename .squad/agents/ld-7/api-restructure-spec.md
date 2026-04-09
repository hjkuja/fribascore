# API Restructure & SLNX Migration Spec

**Branch:** `squad/25-api-scaffold`  
**Owner:** BE-8 (implementation), LD-7 (authored)  
**Status:** Approved for implementation

---

## 1. Target Directory Layout

### Repo root (after)
```
fribascore/
  fribascore.slnx          ← new (replaces fribascore.sln)
  api/
    .gitignore             ← stays here (covers full api/ tree)
    src/
      Controllers/
        CoursesController.cs
        PlayersController.cs
        RoundsController.cs
      Data/
        AppDbContext.cs
      Models/
        Course.cs
        Player.cs
        Round.cs
      Properties/
        launchSettings.json
      appsettings.Development.json
      appsettings.json
      FribaScore.Api.csproj
      FribaScore.Api.http
      Program.cs
    test/
      FribaScore.Api.Tests/
        FribaScore.Api.Tests.csproj   ← new
  ui/
  src/
  docs/
  ...
```

---

## 2. SLNX Format Spec

### Create: `fribascore.slnx`

```xml
<Solution>
  <Folder Name="/api/">
    <Project Path="api/src/FribaScore.Api.csproj" />
    <Project Path="api/test/FribaScore.Api.Tests/FribaScore.Api.Tests.csproj" />
  </Folder>
</Solution>
```

**Notes:**
- Paths are relative to the `.slnx` file (repo root).
- Forward slashes only — cross-platform safe.
- No GUIDs, no configuration blocks — tooling infers them.
- Solution folders (`<Folder>`) are virtual (same as old "api" solution folder).
- `dotnet build fribascore.slnx` and `dotnet test` both understand this format (.NET 10 SDK required).

### Delete: `fribascore.sln`

The old `.sln` is replaced entirely. Do not keep both.

### Tooling note

The `.slnx` format requires .NET 9+ SDK. Since the project already targets `net10.0`, this is satisfied. The CI runner must have .NET 10 SDK installed when a BE CI step is added.

---

## 3. File Moves Required

All moves are `git mv` to preserve history.

| Source | Destination |
|--------|-------------|
| `api/FribaScore.Api.csproj` | `api/src/FribaScore.Api.csproj` |
| `api/Program.cs` | `api/src/Program.cs` |
| `api/appsettings.json` | `api/src/appsettings.json` |
| `api/appsettings.Development.json` | `api/src/appsettings.Development.json` |
| `api/FribaScore.Api.http` | `api/src/FribaScore.Api.http` |
| `api/Controllers/` (whole dir) | `api/src/Controllers/` |
| `api/Data/` (whole dir) | `api/src/Data/` |
| `api/Models/` (whole dir) | `api/src/Models/` |
| `api/Properties/` (whole dir) | `api/src/Properties/` |

**Do NOT move:**
- `api/.gitignore` — leave at `api/` root so it continues to cover the full `api/` subtree.
- `api/test/` — already exists as an empty directory; will receive the new test project.

### Git commands

```bash
git mv api/FribaScore.Api.csproj api/src/FribaScore.Api.csproj
git mv api/Program.cs api/src/Program.cs
git mv api/appsettings.json api/src/appsettings.json
git mv api/appsettings.Development.json api/src/appsettings.Development.json
git mv api/FribaScore.Api.http api/src/FribaScore.Api.http
git mv api/Controllers api/src/Controllers
git mv api/Data api/src/Data
git mv api/Models api/src/Models
git mv api/Properties api/src/Properties
```

> `api/src/` does not exist yet — create it first with `mkdir api/src` (or it will be implicitly created by the first `git mv`; behaviour varies by shell). Safe to `mkdir -p api/src` before running the moves.

---

## 4. Test Project Setup

### Location
`api/test/FribaScore.Api.Tests/FribaScore.Api.Tests.csproj`

### Create via CLI (recommended)

```bash
dotnet new xunit -n FribaScore.Api.Tests -o api/test/FribaScore.Api.Tests --framework net10.0
```

Then add the project reference manually (the CLI template won't add it):

```bash
dotnet add api/test/FribaScore.Api.Tests/FribaScore.Api.Tests.csproj \
  reference api/src/FribaScore.Api.csproj
```

### Expected `FribaScore.Api.Tests.csproj` content

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
    <PackageReference Include="xunit" Version="2.9.3" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="coverlet.collector" Version="6.0.2">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\FribaScore.Api.csproj" />
  </ItemGroup>

</Project>
```

> **Version note:** Pin to whatever `dotnet new xunit` scaffolds for .NET 10 — the versions above are approximate. Accept what the template emits; just ensure the `ProjectReference` path is correct.

### Add to solution

```bash
dotnet sln fribascore.slnx add api/test/FribaScore.Api.Tests/FribaScore.Api.Tests.csproj
```

> If `dotnet sln` doesn't support `.slnx` on the installed SDK version, edit the XML directly — it's just adding a `<Project Path="..." />` element inside the `/api/` folder.

---

## 5. Build Verification Steps

Run these from the **repo root** after all moves and file creation:

```bash
# 1. Full solution build
dotnet build fribascore.slnx

# 2. Test project build in isolation
dotnet build api/test/FribaScore.Api.Tests/FribaScore.Api.Tests.csproj

# 3. Run tests (empty suite is fine at this stage — just verify runner works)
dotnet test api/test/FribaScore.Api.Tests/FribaScore.Api.Tests.csproj

# 4. Confirm API project still builds standalone
dotnet build api/src/FribaScore.Api.csproj
```

All four commands must exit 0. A passing but empty test run (`0 passed, 0 failed`) is acceptable — test authoring is BE-8's next task.

---

## 6. Gotchas / Risks

### CI workflow (`ci.yml`) — no changes needed
The existing CI at `.github/workflows/ci.yml` only runs the `ui/` build. It has **no references** to `.sln`, `api/`, or any dotnet commands. No CI edits are required for this migration.

When BE CI steps are added later, use `fribascore.slnx` not `fribascore.sln`.

### `api/.gitignore` scope
The current `api/.gitignore` lives at the `api/` root. After the restructure it will still correctly cover `api/src/` and `api/test/` because gitignore rules cascade down. No change needed.

### Relative paths inside the csproj
`FribaScore.Api.csproj` itself has no `<ProjectReference>` entries (it's the only app project currently). After the move, its internal paths (if any `<Content>`, `<None>`, etc. reference relative files) should still be valid because all referenced files move together into `api/src/`.

Verify by checking for any hardcoded `../` paths inside the csproj after the move:
```bash
grep -r "\.\." api/src/FribaScore.Api.csproj
```

### `launchSettings.json`
`api/Properties/launchSettings.json` moves to `api/src/Properties/launchSettings.json`. ASP.NET tooling discovers this by convention relative to the csproj, so no path edits needed inside the file.

### `FribaScore.Api.http`
REST Client / HTTP REPL files are convention-based and don't need internal path updates. Just confirm it moves to `api/src/`.

### `.slnx` support in older tooling
Visual Studio 2022 17.12+ and JetBrains Rider 2024.3+ support `.slnx`. If a team member is on an older IDE, they'll see an unrecognised format warning. The `dotnet` CLI itself supports it from .NET 9 SDK onward.

### Branch note
All work is on `squad/25-api-scaffold`. The old `fribascore.sln` should be deleted in the **same commit** as `fribascore.slnx` creation to keep the branch coherent.
