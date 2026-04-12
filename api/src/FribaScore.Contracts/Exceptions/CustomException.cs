namespace FribaScore.Contracts.Exceptions;

/// <summary>
/// Represents the base class for custom application exceptions.
/// </summary>
public abstract class CustomException(
    string title,
    string message,
    int statusCode,
    string type,
    Dictionary<string, string[]>? errors = null) : Exception(message)
{
    /// <summary>
    /// Gets the title of the exception.
    /// </summary>
    public string Title { get; } = title;

    /// <summary>
    /// Gets the HTTP status code associated with this exception.
    /// </summary>
    public int StatusCode { get; } = statusCode;

    /// <summary>
    /// Gets the exception type or URI identifying the problem.
    /// </summary>
    public string Type { get; } = type;

    /// <summary>
    /// Gets the dictionary of validation errors, if available.
    /// </summary>
    public Dictionary<string, string[]>? Errors { get; } = errors;
}
