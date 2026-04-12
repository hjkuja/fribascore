namespace FribaScore.Contracts.Responses;

/// <summary>
/// Represents a player in the response from the API.
/// </summary>
/// <param name="Id">The unique identifier of the player.</param>
/// <param name="Name">The name of the player.</param>
public record PlayerResponse(Guid Id, string Name);
