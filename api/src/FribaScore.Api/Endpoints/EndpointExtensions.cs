using FribaScore.Api.Endpoints.Auth;
using FribaScore.Api.Endpoints.Courses;
using FribaScore.Api.Endpoints.Players;
using FribaScore.Api.Endpoints.Rounds;

namespace FribaScore.Api.Endpoints;

/// <summary>
/// Maps all API endpoint groups for the application.
/// </summary>
public static class EndpointExtensions
{
    /// <summary>
    /// Maps all endpoint groups to the route builder.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <returns>The same route builder for chaining.</returns>
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapAuthEndpoints();
        endpoints.MapCourseEndpoints();
        endpoints.MapPlayerEndpoints();
        endpoints.MapRoundEndpoints();
        return endpoints;
    }
}
