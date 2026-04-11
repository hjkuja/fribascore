using FribaScore.Api.Endpoints.Auth;
using FribaScore.Api.Endpoints.Courses;
using FribaScore.Api.Endpoints.Players;
using FribaScore.Api.Endpoints.Rounds;

namespace FribaScore.Api.Endpoints;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapAuthEndpoints();
        endpoints.MapCourseEndpoints();
        endpoints.MapPlayerEndpoints();
        endpoints.MapRoundEndpoints();
        return endpoints;
    }
}
