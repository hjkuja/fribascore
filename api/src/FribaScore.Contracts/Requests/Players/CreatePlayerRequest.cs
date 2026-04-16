namespace FribaScore.Contracts.Requests.Players;

/// <summary>
/// Represents a request to create a new player.
/// </summary>
/// <param name="Name">The name of the player.</param>
public record CreatePlayerRequest(string Name);
