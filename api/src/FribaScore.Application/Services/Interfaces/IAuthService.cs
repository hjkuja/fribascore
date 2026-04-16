using System.Security.Claims;
using FribaScore.Contracts.Requests.Auth;
using FribaScore.Contracts.Responses;
using LanguageExt.Common;

namespace FribaScore.Application.Services.Interfaces;

/// <summary>
/// Provides authentication operations for logging users in, logging them out, and resolving the current user.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Validates credentials and starts an authenticated session.
    /// </summary>
    /// <param name="request">The login request to validate.</param>
    /// <returns>The authenticated user when sign-in succeeds.</returns>
    Task<Result<AuthUserResponse>> LoginAsync(LoginRequest request);

    /// <summary>
    /// Ends the current authenticated session.
    /// </summary>
    /// <returns>A result indicating whether sign-out completed successfully.</returns>
    Task<Result<bool>> LogoutAsync();

    /// <summary>
    /// Resolves the currently authenticated user from the request principal.
    /// </summary>
    /// <param name="principal">The current request principal.</param>
    /// <returns>The current authenticated user.</returns>
    Task<Result<AuthUserResponse>> GetCurrentUserAsync(ClaimsPrincipal principal);
}
