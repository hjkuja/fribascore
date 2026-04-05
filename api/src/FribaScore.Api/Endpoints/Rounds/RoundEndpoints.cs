using FribaScore.Api.Data;
using FribaScore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FribaScore.Api.Endpoints.Rounds;

public static class RoundEndpoints
{
    private const string BasePath = "api/rounds";

    public static void MapRoundEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(BasePath).WithTags("Rounds");

        group.MapGet(string.Empty, GetAll)
            .WithName(nameof(GetAll) + "Rounds")
            .WithDescription("Returns all rounds.");

        group.MapGet("{id:guid}", GetById)
            .WithName(nameof(GetById) + "Round")
            .WithDescription("Returns a single round by ID.");

        group.MapPost(string.Empty, Create)
            .WithName(nameof(Create) + "Round")
            .WithDescription("Creates a new round.")
            .RequireAuthorization();

        group.MapDelete("{id:guid}", Delete)
            .WithName(nameof(Delete) + "Round")
            .WithDescription("Deletes a round by ID.")
            .RequireAuthorization();
    }

    private static async Task<IResult> GetAll(AppDbContext db) =>
        TypedResults.Ok(await db.Rounds.ToListAsync());

    private static async Task<IResult> GetById(Guid id, AppDbContext db)
    {
        var round = await db.Rounds.FindAsync(id);
        return round is null ? TypedResults.NotFound() : TypedResults.Ok(round);
    }

    private static async Task<IResult> Create(Round round, AppDbContext db)
    {
        db.Rounds.Add(round);
        await db.SaveChangesAsync();
        return TypedResults.Created($"{BasePath}/{round.Id}", round);
    }

    private static async Task<IResult> Delete(Guid id, AppDbContext db)
    {
        var round = await db.Rounds.FindAsync(id);
        if (round is null) return TypedResults.NotFound();
        db.Rounds.Remove(round);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }
}
