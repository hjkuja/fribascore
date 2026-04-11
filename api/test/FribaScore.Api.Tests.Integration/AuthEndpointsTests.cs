using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FribaScore.Api.Tests.Integration.Infrastructure;

namespace FribaScore.Api.Tests.Integration;

public sealed class AuthEndpointsTests : IClassFixture<PostgresDatabaseFixture>
{
    private readonly PostgresDatabaseFixture fixture;

    public AuthEndpointsTests(PostgresDatabaseFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkAndSetsStrictHttpOnlyCookie()
    {
        using var factory = new AuthApiFactory(fixture.ConnectionString);
        using var client = factory.CreateHttpsClient();

        var username = $"qt3-{Guid.NewGuid():N}";
        const string password = "Str0ng!Pass";
        await factory.SeedUserAsync(username, password);

        var response = await client.PostAsJsonAsync("/auth/login", new { username, password });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var cookieHeader = GetRequiredAuthCookie(response);
        Assert.Contains("HttpOnly", cookieHeader, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("SameSite=Strict", cookieHeader, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorizedAndDoesNotSetCookie()
    {
        using var factory = new AuthApiFactory(fixture.ConnectionString);
        using var client = factory.CreateHttpsClient();

        var username = $"qt3-{Guid.NewGuid():N}";
        const string password = "Str0ng!Pass";
        await factory.SeedUserAsync(username, password);

        var response = await client.PostAsJsonAsync("/auth/login", new { username, password = "Wr0ng!Pass" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.False(response.Headers.TryGetValues("Set-Cookie", out _));
    }

    [Fact]
    public async Task Login_WithBlankCredentials_ReturnsBadRequestWithValidationErrors()
    {
        using var factory = new AuthApiFactory(fixture.ConnectionString);
        using var client = factory.CreateHttpsClient();

        var response = await client.PostAsJsonAsync("/auth/login", new { username = "   ", password = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var problem = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var errors = problem.RootElement.GetProperty("errors");

        Assert.Contains("username", errors.EnumerateObject().Select(property => property.Name));
        Assert.Contains("password", errors.EnumerateObject().Select(property => property.Name));
    }

    [Fact]
    public async Task Logout_WhenAnonymous_ReturnsUnauthorized()
    {
        using var factory = new AuthApiFactory(fixture.ConnectionString);
        using var client = factory.CreateHttpsClient();

        var response = await client.PostAsync("/auth/logout", content: null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Logout_WhenAuthenticated_ExpiresTheAuthCookie()
    {
        using var factory = new AuthApiFactory(fixture.ConnectionString);
        using var client = factory.CreateHttpsClient();

        var username = $"qt3-{Guid.NewGuid():N}";
        const string password = "Str0ng!Pass";
        await factory.SeedUserAsync(username, password);

        var loginResponse = await client.PostAsJsonAsync("/auth/login", new { username, password });
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginCookie = GetRequiredAuthCookie(loginResponse);
        var cookieName = loginCookie[..loginCookie.IndexOf('=')];

        var logoutResponse = await client.PostAsync("/auth/logout", content: null);

        Assert.Equal(HttpStatusCode.NoContent, logoutResponse.StatusCode);

        Assert.True(logoutResponse.Headers.TryGetValues("Set-Cookie", out var cookieHeaders));

        var clearedCookie = cookieHeaders
            .SingleOrDefault(value => value.StartsWith($"{cookieName}=", StringComparison.OrdinalIgnoreCase));

        Assert.NotNull(clearedCookie);
        Assert.True(
            clearedCookie.Contains("expires=", StringComparison.OrdinalIgnoreCase)
            || clearedCookie.Contains("max-age=0", StringComparison.OrdinalIgnoreCase),
            "Expected logout to clear the auth cookie.");
    }

    [Fact]
    public async Task Me_WhenAnonymous_ReturnsUnauthorized()
    {
        using var factory = new AuthApiFactory(fixture.ConnectionString);
        using var client = factory.CreateHttpsClient();

        var response = await client.GetAsync("/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Me_WhenAuthenticated_ReturnsCurrentUserIdAndUsernameOnly()
    {
        using var factory = new AuthApiFactory(fixture.ConnectionString);
        using var client = factory.CreateHttpsClient();

        var username = $"qt3-{Guid.NewGuid():N}";
        const string password = "Str0ng!Pass";
        var user = await factory.SeedUserAsync(username, password);

        var loginResponse = await client.PostAsJsonAsync("/auth/login", new { username, password });
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var meResponse = await client.GetAsync("/auth/me");
        Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);

        using var payload = JsonDocument.Parse(await meResponse.Content.ReadAsStringAsync());
        var root = payload.RootElement;

        var propertyNames = root.EnumerateObject()
            .Select(property => property.Name)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(["id", "username"], propertyNames);
        Assert.Equal(user.Id, root.GetProperty("id").GetString());
        Assert.Equal(username, root.GetProperty("username").GetString());
        Assert.True(Guid.TryParse(root.GetProperty("id").GetString(), out _));
    }

    private static string GetRequiredAuthCookie(HttpResponseMessage response)
    {
        Assert.True(response.Headers.TryGetValues("Set-Cookie", out var cookieHeaders));

        var authCookie = cookieHeaders.SingleOrDefault(header => header.Contains("HttpOnly", StringComparison.OrdinalIgnoreCase));
        Assert.NotNull(authCookie);
        return authCookie;
    }
}

