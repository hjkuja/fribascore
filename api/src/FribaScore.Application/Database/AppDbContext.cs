using FribaScore.Application.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FribaScore.Application.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Round> Rounds => Set<Round>();
    public DbSet<Player> Players => Set<Player>();

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
