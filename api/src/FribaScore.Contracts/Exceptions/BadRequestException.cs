namespace FribaScore.Contracts.Exceptions;

public class BadRequestException(string message, Dictionary<string, string[]>? errors = null)
    : CustomException(
        "Bad Request",
        message,
        400,
        "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
        errors)
{ }
