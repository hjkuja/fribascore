using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FribaScore.Application.Database;

/// <summary>
/// Factory for creating the AppDbContext at design time for EF Core migrations.
/// </summary>
public class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    /// <summary>
    /// Creates a database context for use during design-time operations such as migrations.
    /// </summary>
    /// <param name="args">Command-line arguments (unused).</param>
    /// <returns>A configured AppDbContext instance.</returns>
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Database=fribascore_migrations;Username=postgres;Password=postgres")
            .Options;

        return new AppDbContext(options);
    }
}
