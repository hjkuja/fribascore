namespace FribaScore.Contracts.Exceptions;

public class NotFoundException(string resource) : CustomException(
    "Not Found",
    $"{resource} was not found.",
    404,
    "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4")
{ }
