using System.Security.Claims;
using FribaScore.Contracts.Requests.Auth;
using FribaScore.Contracts.Responses;
using LanguageExt.Common;

namespace FribaScore.Application.Services.Interfaces;

public interface IAuthService
{
    Task<Result<AuthUserResponse>> LoginAsync(LoginRequest request);
    Task<Result<bool>> LogoutAsync();
    Task<Result<AuthUserResponse>> GetCurrentUserAsync(ClaimsPrincipal principal);
}
