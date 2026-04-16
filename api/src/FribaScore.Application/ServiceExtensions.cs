using FribaScore.Application.Database;
using FribaScore.Application.Services;
using FribaScore.Application.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FribaScore.Application;

/// <summary>
/// Provides extension methods for registering Application layer services in the dependency injection container.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Registers Application layer services including the database context and service implementations.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="connectionString">The connection string for the PostgreSQL database.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IPlayerService, PlayerService>();
        services.AddScoped<IRoundService, RoundService>();
        return services;
    }
}
