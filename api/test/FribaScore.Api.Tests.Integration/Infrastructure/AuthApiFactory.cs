using FribaScore.Api;
using FribaScore.Application.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FribaScore.Api.Tests.Integration.Infrastructure;

/// <summary>
/// Creates a test host for auth integration tests backed by a real PostgreSQL database.
/// The existing <see cref="Microsoft.EntityFrameworkCore.DbContextOptions{TContext}"/> registration is replaced
/// in <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/> so the Testcontainers
/// connection string is used instead of the one from appsettings.json.
/// </summary>
public sealed class AuthApiFactory : WebApplicationFactory<ApiAssemblyMarker>
{
    private readonly string connectionString;

    /// <summary>
    /// Initializes a new integration test factory instance.
    /// </summary>
    /// <param name="connectionString">
    /// PostgreSQL connection string, typically from a <see cref="PostgresDatabaseFixture"/>.
    /// </param>
    public AuthApiFactory(string connectionString)
    {
        this.connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging => logging.ClearProviders());
        builder.ConfigureServices(services =>
        {
            // Replace the appsettings.json-backed DbContext options with the Testcontainers instance.
            // ConfigureAppConfiguration is not reliable here because Program.cs reads the connection
            // string during WebApplicationBuilder construction before overrides can take effect.
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        });
    }

    /// <summary>
    /// Creates an HTTPS test client with cookie handling enabled.
    /// </summary>
    public HttpClient CreateHttpsClient() =>
        CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost"),
            HandleCookies = true
        });

    /// <summary>
    /// Applies pending EF Core migrations so the test database schema is up to date.
    /// Safe to call multiple times — MigrateAsync is idempotent.
    /// </summary>
    public async Task InitializeDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    /// <summary>
    /// Seeds a user for authentication tests.
    /// </summary>
    public async Task<IdentityUser> SeedUserAsync(string username, string password)
    {
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
            $"Failed to seed auth user: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");

        return user;
    }
}

