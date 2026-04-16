using FribaScore.Contracts.Responses;
using LanguageExt.Common;

namespace FribaScore.Application.Services.Interfaces;

/// <summary>
/// Provides course-related business operations.
/// </summary>
public interface ICourseService
{
    /// <summary>
    /// Gets all courses.
    /// </summary>
    /// <returns>All known courses.</returns>
    Task<Result<IEnumerable<CourseResponse>>> GetAllAsync();

    /// <summary>
    /// Gets a course by its identifier.
    /// </summary>
    /// <param name="id">The course identifier.</param>
    /// <returns>The matching course when found.</returns>
    Task<Result<CourseResponse>> GetByIdAsync(Guid id);

    /// <summary>
    /// Creates a new course.
    /// </summary>
    /// <param name="request">The course creation request.</param>
    /// <returns>The created course.</returns>
    Task<Result<CourseResponse>> CreateAsync(Contracts.Requests.Courses.CreateCourseRequest request);

    /// <summary>
    /// Deletes a course by its identifier.
    /// </summary>
    /// <param name="id">The course identifier.</param>
    /// <returns>A result indicating whether deletion succeeded.</returns>
    Task<Result<bool>> DeleteAsync(Guid id);
}
