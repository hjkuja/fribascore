using FribaScore.Application.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FribaScore.Application.Database;

/// <summary>
/// The Entity Framework Core database context for the FribaScore application.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext(options)
{
    /// <summary>
    /// Gets the courses data set.
    /// </summary>
    public DbSet<Course> Courses => Set<Course>();

    /// <summary>
    /// Gets the rounds data set.
    /// </summary>
    public DbSet<Round> Rounds => Set<Round>();

    /// <summary>
    /// Gets the players data set.
    /// </summary>
    public DbSet<Player> Players => Set<Player>();

    /// <summary>
    /// Configures the database model with entity relationships and owned types.
    /// </summary>
    /// <param name="modelBuilder">The model builder for configuration.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Course>().OwnsMany(c => c.Holes, hole =>
        {
            hole.WithOwner().HasForeignKey("CourseId");
            hole.Property<int>("Id");
            hole.HasKey("Id");
        });

        modelBuilder.Entity<Round>().OwnsMany(r => r.Scores, score =>
        {
            score.WithOwner().HasForeignKey("RoundId");
            score.Property<int>("Id");
            score.HasKey("Id");
        });
    }
}
