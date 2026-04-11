using System.Net.Http;
using FribaScore.Api;
using FribaScore.Application.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace FribaScore.Api.Tests.Integration.Infrastructure;

/// <summary>
/// Creates a test host for auth integration tests.
/// </summary>
public sealed class AuthApiFactory : WebApplicationFactory<ApiAssemblyMarker>
{
    // Keep the connection open for the lifetime of the factory so the in-memory
    // SQLite database survives across DI scopes and HTTP requests.
    private readonly SqliteConnection connection = new("Data Source=:memory:");

    /// <summary>
    /// Initializes a new integration test factory instance.
    /// </summary>
    public AuthApiFactory()
    {
        connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging => logging.ClearProviders());
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<AppDbContext>();
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
            services.AddSingleton(connection);
            services.AddDbContext<AppDbContext>((serviceProvider, options) =>
                options.UseSqlite(serviceProvider.GetRequiredService<SqliteConnection>()));
        });
    }

    /// <summary>
    /// Creates an HTTPS test client with cookie handling enabled.
    /// </summary>
    /// <returns>A configured test HTTP client.</returns>
    public HttpClient CreateHttpsClient()
    {
        return CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost"),
            HandleCookies = true
        });
    }

    /// <summary>
    /// Ensures that the SQLite test database schema exists.
    /// </summary>
    public async Task InitializeDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    /// <summary>
    /// Seeds a user for authentication tests.
    /// </summary>
    /// <param name="username">The username to create.</param>
    /// <param name="password">The password to hash and store with Identity.</param>
    /// <returns>The created identity user.</returns>
    public async Task<IdentityUser> SeedUserAsync(string username, string password)
    {
        await InitializeDatabaseAsync();

        using var scope = Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var user = new IdentityUser
        {
            UserName = username,
            Email = $"{username}@qt-3.local"
        };

        var createResult = await userManager.CreateAsync(user, password);
        Assert.True(
            createResult.Succeeded,
            $"Failed to seed auth user: {string.Join(", ", createResult.Errors.Select(error => error.Description))}");

        return user;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            connection.Dispose();
        }
    }
}
