namespace FribaScore.Contracts.Requests.Courses;

/// <summary>
/// Represents a request to create a new course.
/// </summary>
/// <param name="Name">The name of the course.</param>
/// <param name="TotalPar">The total par for the course.</param>
/// <param name="TotalLength">The total length of the course in yards or meters.</param>
/// <param name="Holes">The list of holes for the course.</param>
public record CreateCourseRequest(
    string Name,
    int TotalPar,
    int TotalLength,
    List<CreateHoleRequest> Holes);

/// <summary>
/// Represents a hole within a course creation request.
/// </summary>
/// <param name="HoleNumber">The hole number (1-based).</param>
/// <param name="Par">The par value for the hole.</param>
/// <param name="Length">The length of the hole in yards or meters, if available.</param>
public record CreateHoleRequest(
    int HoleNumber,
    int Par,
    int? Length);
