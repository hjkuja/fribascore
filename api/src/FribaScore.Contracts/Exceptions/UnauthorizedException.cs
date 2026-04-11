namespace FribaScore.Contracts.Exceptions;

public class UnauthorizedException(string message = "Authentication is required.")
    : CustomException(
        "Unauthorized",
        message,
        401,
        "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1")
{ }
