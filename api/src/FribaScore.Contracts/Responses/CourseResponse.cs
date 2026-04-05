namespace FribaScore.Contracts.Responses;

public record CourseResponse(
    Guid Id,
    string Name,
    int TotalPar,
    int TotalLength,
    List<HoleResponse> Holes);

public record HoleResponse(
    int HoleNumber,
    int Par,
    int? Length);
