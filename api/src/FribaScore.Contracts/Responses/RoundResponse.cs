namespace FribaScore.Contracts.Responses;

/// <summary>
/// Represents a round in the response from the API.
/// </summary>
/// <param name="Id">The unique identifier of the round.</param>
/// <param name="CourseId">The unique identifier of the course played in this round.</param>
/// <param name="Date">The date and time when the round was played.</param>
/// <param name="Scores">The list of score entries from all players in this round.</param>
public record RoundResponse(
    Guid Id,
    Guid CourseId,
    DateTime Date,
    List<ScoreEntryResponse> Scores);

/// <summary>
/// Represents a score entry in a round response.
/// </summary>
/// <param name="PlayerId">The unique identifier of the player.</param>
/// <param name="HoleNumber">The hole number for the score (1-based).</param>
/// <param name="Score">The number of strokes for this hole.</param>
public record ScoreEntryResponse(
    Guid PlayerId,
    int HoleNumber,
    int Score);
