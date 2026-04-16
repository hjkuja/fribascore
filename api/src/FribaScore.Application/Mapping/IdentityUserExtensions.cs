using FribaScore.Contracts.Responses;
using Microsoft.AspNetCore.Identity;

namespace FribaScore.Application.Mapping;

/// <summary>
/// Maps ASP.NET Core Identity users to FribaScore auth DTOs.
/// </summary>
public static class IdentityUserExtensions
{
    /// <summary>
    /// Converts an <see cref="IdentityUser" /> to an <see cref="AuthUserResponse" />.
    /// </summary>
    /// <param name="user">The identity user to convert.</param>
    /// <returns>The auth response payload for the user.</returns>
    public static AuthUserResponse ToResponse(this IdentityUser user)
        => new(
            user.Id,
            user.UserName ?? throw new InvalidOperationException("Identity user is missing a username."));
}
