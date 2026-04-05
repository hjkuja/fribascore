namespace FribaScore.Contracts.Requests.Rounds;

public record CreateRoundRequest(
    Guid CourseId,
    List<CreateScoreEntryRequest> Scores);

public record CreateScoreEntryRequest(
    Guid PlayerId,
    int HoleNumber,
    int Score);
