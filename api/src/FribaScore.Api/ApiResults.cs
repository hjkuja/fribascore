using FribaScore.Contracts.Exceptions;

namespace FribaScore.Api;

public static class ApiResults
{
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
