using FribaScore.Application.Services.Interfaces;
using FribaScore.Contracts.Requests.Courses;
using FribaScore.Contracts.Responses;

namespace FribaScore.Api.Endpoints.Courses;

/// <summary>
/// Maps course endpoints.
/// </summary>
public static class CourseEndpoints
{
    private const string BasePath = "api/courses";

    /// <summary>
    /// Maps the course endpoints.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    public static void MapCourseEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(BasePath).WithTags("Courses");

        group.MapGet(string.Empty, GetAll)
            .WithName(nameof(GetAll) + "Courses")
            .WithSummary("Returns all courses.")
            .WithDescription("Returns all courses.")
            .Produces<IEnumerable<CourseResponse>>();

        group.MapGet("{id:guid}", GetById)
            .WithName(nameof(GetById) + "Course")
            .WithSummary("Returns a single course by ID.")
            .WithDescription("Returns a single course by ID.")
            .Produces<CourseResponse>()
            .ProducesProblem(404);

        group.MapPost(string.Empty, Create)
            .WithName(nameof(Create) + "Course")
            .WithSummary("Creates a new course.")
            .WithDescription("Creates a new course.")
            .Produces<CourseResponse>(201)
            .ProducesProblem(400)
            .RequireAuthorization();

        group.MapDelete("{id:guid}", Delete)
            .WithName(nameof(Delete) + "Course")
            .WithSummary("Deletes a course by ID.")
            .WithDescription("Deletes a course by ID.")
            .ProducesProblem(404)
            .RequireAuthorization();
    }

    /// <summary>
    /// Returns all courses.
    /// </summary>
    /// <param name="courseService">The course service.</param>
    /// <returns>All available courses.</returns>
    /// <response code="200">The courses were returned successfully.</response>
    private static async Task<IResult> GetAll(ICourseService courseService)
        => (await courseService.GetAllAsync()).Match(
            courses => TypedResults.Ok(courses),
            ex => ex.ToProblemResult());

    /// <summary>
    /// Returns a single course by ID.
    /// </summary>
    /// <param name="id">The course identifier.</param>
    /// <param name="courseService">The course service.</param>
    /// <returns>The requested course.</returns>
    /// <response code="200">The course was returned successfully.</response>
    /// <response code="404">The course was not found.</response>
    private static async Task<IResult> GetById(Guid id, ICourseService courseService)
        => (await courseService.GetByIdAsync(id)).Match(
            course => TypedResults.Ok(course),
            ex => ex.ToProblemResult());

    /// <summary>
    /// Creates a new course.
    /// </summary>
    /// <param name="request">The course creation request.</param>
    /// <param name="courseService">The course service.</param>
    /// <returns>The created course.</returns>
    /// <response code="201">The course was created successfully.</response>
    /// <response code="400">The request payload is invalid.</response>
    private static async Task<IResult> Create(CreateCourseRequest request, ICourseService courseService)
        => (await courseService.CreateAsync(request)).Match(
            course => TypedResults.Created($"{BasePath}/{course.Id}", course),
            ex => ex.ToProblemResult());

    /// <summary>
    /// Deletes a course by ID.
    /// </summary>
    /// <param name="id">The course identifier.</param>
    /// <param name="courseService">The course service.</param>
    /// <returns>An empty response when deletion succeeds.</returns>
    /// <response code="204">The course was deleted successfully.</response>
    /// <response code="404">The course was not found.</response>
    private static async Task<IResult> Delete(Guid id, ICourseService courseService)
        => (await courseService.DeleteAsync(id)).Match(
            _ => TypedResults.NoContent(),
            ex => ex.ToProblemResult());
}

