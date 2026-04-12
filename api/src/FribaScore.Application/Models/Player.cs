namespace FribaScore.Application.Models;

/// <summary>
/// Represents a player in the system.
/// </summary>
public class Player
{
    /// <summary>
    /// Gets or sets the unique identifier for the player.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the name of the player.
    /// </summary>
    public required string Name { get; set; }
}
