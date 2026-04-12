namespace FribaScore.Application.Models;

/// <summary>
/// Represents a round of golf with score entries for each player and hole.
/// </summary>
public class Round
{
    /// <summary>
    /// Gets or sets the unique identifier for the round.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the course identifier for this round.
    /// </summary>
    public Guid CourseId { get; set; }

    /// <summary>
    /// Gets or sets the date and time when this round was played.
    /// </summary>
    public DateTime Date { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the collection of score entries for this round.
    /// </summary>
    public List<ScoreEntry> Scores { get; set; } = [];
}

/// <summary>
/// Represents a single score entry for a player on a specific hole.
/// </summary>
public class ScoreEntry
{
    /// <summary>
    /// Gets or sets the player identifier.
    /// </summary>
    public Guid PlayerId { get; set; }

    /// <summary>
    /// Gets or sets the hole number.
    /// </summary>
    public int HoleNumber { get; set; }

    /// <summary>
    /// Gets or sets the score achieved on this hole.
    /// </summary>
    public int Score { get; set; }
}
