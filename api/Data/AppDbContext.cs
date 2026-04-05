using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FribaScore.Api.Models;

namespace FribaScore.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Round> Rounds => Set<Round>();
    public DbSet<Player> Players => Set<Player>();
}
