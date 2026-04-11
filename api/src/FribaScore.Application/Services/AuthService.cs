using System.Security.Claims;
using FribaScore.Application.Mapping;
using FribaScore.Application.Services.Interfaces;
using FribaScore.Contracts.Exceptions;
using FribaScore.Contracts.Requests.Auth;
using FribaScore.Contracts.Responses;
using LanguageExt.Common;
using Microsoft.AspNetCore.Identity;

namespace FribaScore.Application.Services;

/// <summary>
/// Implements ASP.NET Core Identity-backed authentication operations for the API.
/// </summary>
public class AuthService(
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager) : IAuthService
{
    /// <inheritdoc />
    public async Task<Result<AuthUserResponse>> LoginAsync(LoginRequest request)
    {
        try
        {
            var errors = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(request.Username))
            {
                errors["username"] = ["Username is required."];
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                errors["password"] = ["Password is required."];
            }

            if (errors.Count > 0)
            {
                return new Result<AuthUserResponse>(new BadRequestException("Login request is invalid.", errors));
            }

            var username = request.Username.Trim();
            var user = await userManager.FindByNameAsync(username);
            if (user is null)
            {
                return new Result<AuthUserResponse>(new UnauthorizedException("Invalid username or password."));
            }

            var signInResult = await signInManager.PasswordSignInAsync(
                user,
                request.Password,
                isPersistent: true,
                lockoutOnFailure: false);

            if (!signInResult.Succeeded)
            {
                return new Result<AuthUserResponse>(new UnauthorizedException("Invalid username or password."));
            }

            return new Result<AuthUserResponse>(user.ToResponse());
        }
        catch (Exception ex)
        {
            return new Result<AuthUserResponse>(ex);
        }
    }

    /// <inheritdoc />
    public async Task<Result<bool>> LogoutAsync()
    {
        try
        {
            await signInManager.SignOutAsync();
            return new Result<bool>(true);
        }
        catch (Exception ex)
        {
            return new Result<bool>(ex);
        }
    }

    /// <inheritdoc />
    public async Task<Result<AuthUserResponse>> GetCurrentUserAsync(ClaimsPrincipal principal)
    {
        try
        {
            if (principal.Identity?.IsAuthenticated != true)
            {
                return new Result<AuthUserResponse>(new UnauthorizedException());
            }

            var user = await userManager.GetUserAsync(principal);
            if (user is null)
            {
                return new Result<AuthUserResponse>(new UnauthorizedException());
            }

            return new Result<AuthUserResponse>(user.ToResponse());
        }
        catch (Exception ex)
        {
            return new Result<AuthUserResponse>(ex);
        }
    }
}
