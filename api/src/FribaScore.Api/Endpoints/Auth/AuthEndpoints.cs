using System.Security.Claims;
using FribaScore.Application.Services.Interfaces;
using FribaScore.Contracts.Requests.Auth;
using FribaScore.Contracts.Responses;

namespace FribaScore.Api.Endpoints.Auth;

public static class AuthEndpoints
{
    private const string BasePath = "auth";

    public static void MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(BasePath).WithTags("Auth");

        group.MapPost("login", Login)
            .WithName(nameof(Login) + "Auth")
            .WithDescription("Validates credentials and starts an authenticated session.")
            .Produces<AuthUserResponse>()
            .ProducesProblem(400)
            .ProducesProblem(401);

        group.MapPost("logout", Logout)
            .WithName(nameof(Logout) + "Auth")
            .WithDescription("Clears the current authentication cookie.")
            .Produces(204)
            .ProducesProblem(401)
            .RequireAuthorization();

        group.MapGet("me", Me)
            .WithName(nameof(Me) + "Auth")
            .WithDescription("Returns the current authenticated user.")
            .Produces<AuthUserResponse>()
            .ProducesProblem(401)
            .RequireAuthorization();
    }

    private static async Task<IResult> Login(LoginRequest request, IAuthService authService)
        => (await authService.LoginAsync(request)).Match(
            user => TypedResults.Ok(user),
            ex => ex.ToProblemResult());

    private static async Task<IResult> Logout(IAuthService authService)
        => (await authService.LogoutAsync()).Match(
            _ => TypedResults.NoContent(),
            ex => ex.ToProblemResult());

    private static async Task<IResult> Me(ClaimsPrincipal user, IAuthService authService)
        => (await authService.GetCurrentUserAsync(user)).Match(
            currentUser => TypedResults.Ok(currentUser),
            ex => ex.ToProblemResult());
}
