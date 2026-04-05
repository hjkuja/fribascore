using FribaScore.Api.Data;
using FribaScore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FribaScore.Api.Endpoints.Courses;

public static class CourseEndpoints
{
    private const string BasePath = "api/courses";

    public static void MapCourseEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(BasePath).WithTags("Courses");

        group.MapGet(string.Empty, GetAll)
            .WithName(nameof(GetAll))
            .WithDescription("Returns all courses.");

        group.MapGet("{id:guid}", GetById)
            .WithName(nameof(GetById))
            .WithDescription("Returns a single course by ID.");

        group.MapPost(string.Empty, Create)
            .WithName(nameof(Create))
            .WithDescription("Creates a new course.")
            .RequireAuthorization();

        group.MapDelete("{id:guid}", Delete)
            .WithName(nameof(Delete))
            .WithDescription("Deletes a course by ID.")
            .RequireAuthorization();
    }

    private static async Task<IResult> GetAll(AppDbContext db) =>
        TypedResults.Ok(await db.Courses.ToListAsync());

    private static async Task<IResult> GetById(Guid id, AppDbContext db)
    {
        var course = await db.Courses.FindAsync(id);
        return course is null ? TypedResults.NotFound() : TypedResults.Ok(course);
    }

    private static async Task<IResult> Create(Course course, AppDbContext db)
    {
        db.Courses.Add(course);
        await db.SaveChangesAsync();
        return TypedResults.Created($"{BasePath}/{course.Id}", course);
    }

    private static async Task<IResult> Delete(Guid id, AppDbContext db)
    {
        var course = await db.Courses.FindAsync(id);
        if (course is null) return TypedResults.NotFound();
        db.Courses.Remove(course);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }
}
