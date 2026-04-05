namespace FribaScore.Contracts.Exceptions;

public abstract class CustomException(
    string title,
    string message,
    int statusCode,
    string type,
    Dictionary<string, string[]>? errors = null) : Exception(message)
{
    public string Title { get; } = title;
    public int StatusCode { get; } = statusCode;
    public string Type { get; } = type;
    public Dictionary<string, string[]>? Errors { get; } = errors;
}
