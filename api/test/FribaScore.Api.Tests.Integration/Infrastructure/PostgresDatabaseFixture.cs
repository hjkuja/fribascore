using Testcontainers.PostgreSql;

namespace FribaScore.Api.Tests.Integration.Infrastructure;

/// <summary>
/// xUnit class fixture that manages a PostgreSQL Testcontainer and a shared <see cref="AuthApiFactory"/>
/// for the duration of a test class. The container starts once, the schema is migrated once, the test
/// host is built once, and everything is torn down after all tests complete.
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

    /// <summary>
    /// Gets the shared <see cref="AuthApiFactory"/> backed by this fixture's container.
    /// Tests should create a new <see cref="System.Net.Http.HttpClient"/> per test for cookie isolation,
    /// but reuse this factory so the ASP.NET Core test host is only built once.
    /// </summary>
    public AuthApiFactory Factory { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await container.StartAsync();
        Factory = new AuthApiFactory(ConnectionString);
        await Factory.InitializeDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        await Factory.DisposeAsync();
        await container.DisposeAsync();
    }
}
