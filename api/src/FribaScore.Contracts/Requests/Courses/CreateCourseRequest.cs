namespace FribaScore.Contracts.Requests.Courses;

public record CreateCourseRequest(
    string Name,
    int TotalPar,
    int TotalLength,
    List<CreateHoleRequest> Holes);

public record CreateHoleRequest(
    int HoleNumber,
    int Par,
    int? Length);
