using Testcontainers.PostgreSql;

namespace FribaScore.Api.Tests.Integration.Infrastructure;

/// <summary>
/// xUnit class fixture that manages a PostgreSQL Testcontainer for the duration of a test class.
/// The container starts once, the schema is migrated once, and the container stops after all tests complete.
/// </summary>
public sealed class PostgresDatabaseFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer container = new PostgreSqlBuilder("postgres:18.3-alpine3.23")
        .WithDatabase("fribascore_test")
        .WithUsername("fribascore")
        .WithPassword("testpassword")
        .Build();

    /// <summary>
    /// Gets the PostgreSQL connection string for this fixture's container.
    /// </summary>
    public string ConnectionString => container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await container.StartAsync();

        using var factory = new AuthApiFactory(ConnectionString);
        await factory.InitializeDatabaseAsync();
    }

    public async Task DisposeAsync() => await container.DisposeAsync();
}
