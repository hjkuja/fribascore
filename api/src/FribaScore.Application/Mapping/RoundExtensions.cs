using FribaScore.Application.Models;
using FribaScore.Contracts.Responses;

namespace FribaScore.Application.Mapping;

/// <summary>
/// Provides extension methods for mapping Round and ScoreEntry entities to their response DTOs.
/// </summary>
public static class RoundExtensions
{
    /// <summary>
    /// Converts a Round entity to its response DTO.
    /// </summary>
    /// <param name="round">The round entity to convert.</param>
    /// <returns>The round response DTO.</returns>
    public static RoundResponse ToResponse(this Round round) =>
        new(round.Id, round.CourseId, round.Date,
            round.Scores.Select(s => s.ToResponse()).ToList());

    /// <summary>
    /// Converts a ScoreEntry entity to its response DTO.
    /// </summary>
    /// <param name="entry">The score entry entity to convert.</param>
    /// <returns>The score entry response DTO.</returns>
    public static ScoreEntryResponse ToResponse(this ScoreEntry entry) =>
        new(entry.PlayerId, entry.HoleNumber, entry.Score);
}
