using System.Security.Claims;
using FribaScore.Application.Services.Interfaces;
using FribaScore.Contracts.Requests.Auth;
using FribaScore.Contracts.Responses;

namespace FribaScore.Api.Endpoints.Auth;

/// <summary>
/// Maps authentication endpoints for logging in, logging out, and resolving the current user.
/// </summary>
public static class AuthEndpoints
{
    private const string BasePath = "auth";

    /// <summary>
    /// Maps the authentication endpoints.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    public static void MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(BasePath).WithTags("Auth");

        group.MapPost("login", Login)
            .WithName(nameof(Login) + "Auth")
            .WithSummary("Authenticates a user and issues an auth cookie.")
            .WithDescription("Validates credentials and starts an authenticated session.")
            .Produces<AuthUserResponse>()
            .ProducesProblem(400)
            .ProducesProblem(401);

        group.MapPost("logout", Logout)
            .WithName(nameof(Logout) + "Auth")
            .WithSummary("Clears the current authentication cookie.")
            .WithDescription("Clears the current authentication cookie.")
            .Produces(204)
            .ProducesProblem(401)
            .RequireAuthorization();

        group.MapGet("me", Me)
            .WithName(nameof(Me) + "Auth")
            .WithSummary("Returns the current authenticated user.")
            .WithDescription("Returns the current authenticated user.")
            .Produces<AuthUserResponse>()
            .ProducesProblem(401)
            .RequireAuthorization();
    }

    /// <summary>
    /// Authenticates a user and issues an auth cookie.
    /// </summary>
    /// <param name="request">The credentials to validate.</param>
    /// <param name="authService">The auth service handling sign-in.</param>
    /// <returns>The authenticated user when the credentials are valid.</returns>
    /// <response code="200">The user was authenticated successfully.</response>
    /// <response code="400">The request payload is invalid.</response>
    /// <response code="401">The supplied credentials are invalid.</response>
    private static async Task<IResult> Login(LoginRequest request, IAuthService authService)
        => (await authService.LoginAsync(request)).Match(
            user => TypedResults.Ok(user),
            ex => ex.ToProblemResult());

    /// <summary>
    /// Clears the current authentication cookie.
    /// </summary>
    /// <param name="authService">The auth service handling sign-out.</param>
    /// <returns>An empty response when sign-out succeeds.</returns>
    /// <response code="204">The session was cleared successfully.</response>
    /// <response code="401">The caller is not authenticated.</response>
    private static async Task<IResult> Logout(IAuthService authService)
        => (await authService.LogoutAsync()).Match(
            _ => TypedResults.NoContent(),
            ex => ex.ToProblemResult());

    /// <summary>
    /// Returns the currently authenticated user.
    /// </summary>
    /// <param name="user">The current request principal.</param>
    /// <param name="authService">The auth service used to resolve the current user.</param>
    /// <returns>The current authenticated user.</returns>
    /// <response code="200">The current user was resolved successfully.</response>
    /// <response code="401">The caller is not authenticated.</response>
    private static async Task<IResult> Me(ClaimsPrincipal user, IAuthService authService)
        => (await authService.GetCurrentUserAsync(user)).Match(
            currentUser => TypedResults.Ok(currentUser),
            ex => ex.ToProblemResult());
}
