namespace FribaScore.Contracts.Responses;

/// <summary>
/// Represents a course in the response from the API.
/// </summary>
/// <param name="Id">The unique identifier of the course.</param>
/// <param name="Name">The name of the course.</param>
/// <param name="TotalPar">The total par for the course.</param>
/// <param name="TotalLength">The total length of the course in yards or meters.</param>
/// <param name="Holes">The list of holes for the course.</param>
public record CourseResponse(
    Guid Id,
    string Name,
    int TotalPar,
    int TotalLength,
    List<HoleResponse> Holes);

/// <summary>
/// Represents a hole in the course response.
/// </summary>
/// <param name="HoleNumber">The hole number (1-based).</param>
/// <param name="Par">The par value for the hole.</param>
/// <param name="Length">The length of the hole in yards or meters, if available.</param>
public record HoleResponse(
    int HoleNumber,
    int Par,
    int? Length);
