using FribaScore.Contracts.Responses;
using Microsoft.AspNetCore.Identity;

namespace FribaScore.Application.Mapping;

public static class IdentityUserExtensions
{
    public static AuthUserResponse ToResponse(this IdentityUser user)
        => new(
            user.Id,
            user.UserName ?? throw new InvalidOperationException("Identity user is missing a username."));
}
