namespace FribaScore.Contracts.Exceptions;

/// <summary>
/// Represents a failure caused by a missing or invalid authenticated user context.
/// </summary>
public class UnauthorizedException(string message = "Authentication is required.")
    : CustomException(
        "Unauthorized",
        message,
        401,
        "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1")
{ }
