using FribaScore.Application.Services.Interfaces;
using FribaScore.Contracts.Requests.Courses;
using FribaScore.Contracts.Responses;

namespace FribaScore.Api.Endpoints.Courses;

public static class CourseEndpoints
{
    private const string BasePath = "api/courses";

    public static void MapCourseEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(BasePath).WithTags("Courses");

        group.MapGet(string.Empty, GetAll)
            .WithName(nameof(GetAll))
            .WithDescription("Returns all courses.")
            .Produces<IEnumerable<CourseResponse>>();

        group.MapGet("{id:guid}", GetById)
            .WithName(nameof(GetById))
            .WithDescription("Returns a single course by ID.")
            .Produces<CourseResponse>()
            .ProducesProblem(404);

        group.MapPost(string.Empty, Create)
            .WithName(nameof(Create))
            .WithDescription("Creates a new course.")
            .Produces<CourseResponse>(201)
            .ProducesProblem(400)
            .RequireAuthorization();

        group.MapDelete("{id:guid}", Delete)
            .WithName(nameof(Delete))
            .WithDescription("Deletes a course by ID.")
            .ProducesProblem(404)
            .RequireAuthorization();
    }

    private static async Task<IResult> GetAll(ICourseService courseService)
        => (await courseService.GetAllAsync()).Match(
            courses => TypedResults.Ok(courses),
            ex => ex.ToProblemResult());

    private static async Task<IResult> GetById(Guid id, ICourseService courseService)
        => (await courseService.GetByIdAsync(id)).Match(
            course => TypedResults.Ok(course),
            ex => ex.ToProblemResult());

    private static async Task<IResult> Create(CreateCourseRequest request, ICourseService courseService)
        => (await courseService.CreateAsync(request)).Match(
            course => TypedResults.Created($"{BasePath}/{course.Id}", course),
            ex => ex.ToProblemResult());

    private static async Task<IResult> Delete(Guid id, ICourseService courseService)
        => (await courseService.DeleteAsync(id)).Match(
            _ => TypedResults.NoContent(),
            ex => ex.ToProblemResult());
}

