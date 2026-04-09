namespace FribaScore.Contracts.Responses;

public record RoundResponse(
    Guid Id,
    Guid CourseId,
    DateTime Date,
    List<ScoreEntryResponse> Scores);

public record ScoreEntryResponse(
    Guid PlayerId,
    int HoleNumber,
    int Score);
