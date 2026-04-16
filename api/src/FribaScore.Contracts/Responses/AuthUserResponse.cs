using System.ComponentModel;

namespace FribaScore.Contracts.Responses;

/// <summary>
/// Represents the authenticated user returned by the auth endpoints.
/// </summary>
/// <param name="Id">The unique user identifier.</param>
/// <param name="Username">The username associated with the current session.</param>
public record AuthUserResponse(
    [property: Description("The unique user identifier.")]
    string Id,
    [property: Description("The username associated with the current session.")]
    string Username);
