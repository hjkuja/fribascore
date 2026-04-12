namespace FribaScore.Contracts.Requests.Rounds;

/// <summary>
/// Represents a request to create a new round.
/// </summary>
/// <param name="CourseId">The unique identifier of the course for this round.</param>
/// <param name="Scores">The list of score entries for players in this round.</param>
public record CreateRoundRequest(
    Guid CourseId,
    List<CreateScoreEntryRequest> Scores);

/// <summary>
/// Represents a score entry within a round creation request.
/// </summary>
/// <param name="PlayerId">The unique identifier of the player.</param>
/// <param name="HoleNumber">The hole number for the score (1-based).</param>
/// <param name="Score">The number of strokes for this hole.</param>
public record CreateScoreEntryRequest(
    Guid PlayerId,
    int HoleNumber,
    int Score);
