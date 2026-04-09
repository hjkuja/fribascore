using FribaScore.Contracts.Responses;
using LanguageExt.Common;

namespace FribaScore.Application.Services.Interfaces;

public interface ICourseService
{
    Task<Result<IEnumerable<CourseResponse>>> GetAllAsync();
    Task<Result<CourseResponse>> GetByIdAsync(Guid id);
    Task<Result<CourseResponse>> CreateAsync(Contracts.Requests.Courses.CreateCourseRequest request);
    Task<Result<bool>> DeleteAsync(Guid id);
}
