using FribaScore.Api.Data;
using FribaScore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FribaScore.Api.Endpoints.Players;

public static class PlayerEndpoints
{
    private const string BasePath = "api/players";

    public static void MapPlayerEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(BasePath).WithTags("Players");

        group.MapGet(string.Empty, GetAll)
            .WithName(nameof(GetAll) + "Players")
            .WithDescription("Returns all players.");

        group.MapGet("{id:guid}", GetById)
            .WithName(nameof(GetById) + "Player")
            .WithDescription("Returns a single player by ID.");

        group.MapPost(string.Empty, Create)
            .WithName(nameof(Create) + "Player")
            .WithDescription("Creates a new player.")
            .RequireAuthorization();

        group.MapDelete("{id:guid}", Delete)
            .WithName(nameof(Delete) + "Player")
            .WithDescription("Deletes a player by ID.")
            .RequireAuthorization();
    }

    private static async Task<IResult> GetAll(AppDbContext db) =>
        TypedResults.Ok(await db.Players.ToListAsync());

    private static async Task<IResult> GetById(Guid id, AppDbContext db)
    {
        var player = await db.Players.FindAsync(id);
        return player is null ? TypedResults.NotFound() : TypedResults.Ok(player);
    }

    private static async Task<IResult> Create(Player player, AppDbContext db)
    {
        db.Players.Add(player);
        await db.SaveChangesAsync();
        return TypedResults.Created($"{BasePath}/{player.Id}", player);
    }

    private static async Task<IResult> Delete(Guid id, AppDbContext db)
    {
        var player = await db.Players.FindAsync(id);
        if (player is null) return TypedResults.NotFound();
        db.Players.Remove(player);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }
}
