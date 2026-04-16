using FribaScore.Application.Models;
using FribaScore.Contracts.Responses;

namespace FribaScore.Application.Mapping;

/// <summary>
/// Provides extension methods for mapping Course and Hole entities to their response DTOs.
/// </summary>
public static class CourseExtensions
{
    /// <summary>
    /// Converts a Course entity to its response DTO.
    /// </summary>
    /// <param name="course">The course entity to convert.</param>
    /// <returns>The course response DTO.</returns>
    public static CourseResponse ToResponse(this Course course) =>
        new(course.Id, course.Name, course.TotalPar, course.TotalLength,
            course.Holes.Select(h => h.ToResponse()).ToList());

    /// <summary>
    /// Converts a Hole entity to its response DTO.
    /// </summary>
    /// <param name="hole">The hole entity to convert.</param>
    /// <returns>The hole response DTO.</returns>
    public static HoleResponse ToResponse(this Hole hole) =>
        new(hole.HoleNumber, hole.Par, hole.Length);
}
