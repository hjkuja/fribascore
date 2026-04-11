using FribaScore.Contracts.Exceptions;

namespace FribaScore.Api;

/// <summary>
/// Converts application exceptions to RFC 7807 problem responses.
/// </summary>
public static class ApiResults
{
    /// <summary>
    /// Converts an exception to an HTTP problem response.
    /// </summary>
    /// <param name="exception">The exception to convert.</param>
    /// <returns>A problem response representing the exception.</returns>
    public static IResult ToProblemResult(this Exception exception)
    {
        if (exception is not CustomException customException)
        {
            return Results.Problem(
                title: "Internal Server Error",
                detail: "An unexpected error occurred.",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        return Results.Problem(
            title: customException.Title,
            detail: customException.Message,
            statusCode: customException.StatusCode,
            type: customException.Type,
            extensions: customException.Errors is not null
                ? new Dictionary<string, object?> { { "errors", customException.Errors } }
                : null);
    }
}
