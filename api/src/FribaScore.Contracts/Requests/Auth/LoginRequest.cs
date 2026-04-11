using System.ComponentModel;

namespace FribaScore.Contracts.Requests.Auth;

/// <summary>
/// Represents the credentials required to start an authenticated session.
/// </summary>
/// <param name="Username">The username used to sign in.</param>
/// <param name="Password">The plaintext password supplied for sign-in.</param>
public record LoginRequest(
    [property: Description("The username used to sign in.")]
    string Username,
    [property: Description("The plaintext password supplied for sign-in.")]
    string Password);
