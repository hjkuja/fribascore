using FribaScore.Application.Database;
using FribaScore.Application.Services;
using FribaScore.Application.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FribaScore.Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            if (connectionString.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
            {
                options.UseSqlite(connectionString);
                return;
            }

            options.UseNpgsql(connectionString);
        });
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IPlayerService, PlayerService>();
        services.AddScoped<IRoundService, RoundService>();
        return services;
    }
}
