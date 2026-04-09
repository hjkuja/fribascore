using FribaScore.Application.Models;
using FribaScore.Contracts.Responses;

namespace FribaScore.Application.Mapping;

public static class RoundExtensions
{
    public static RoundResponse ToResponse(this Round round) =>
        new(round.Id, round.CourseId, round.Date,
            round.Scores.Select(s => s.ToResponse()).ToList());

    public static ScoreEntryResponse ToResponse(this ScoreEntry entry) =>
        new(entry.PlayerId, entry.HoleNumber, entry.Score);
}
