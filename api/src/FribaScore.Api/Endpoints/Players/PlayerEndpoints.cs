using FribaScore.Application.Services.Interfaces;
using FribaScore.Contracts.Requests.Players;
using FribaScore.Contracts.Responses;

namespace FribaScore.Api.Endpoints.Players;

public static class PlayerEndpoints
{
    private const string BasePath = "api/players";

    public static void MapPlayerEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(BasePath).WithTags("Players");

        group.MapGet(string.Empty, GetAll)
            .WithName(nameof(GetAll) + "Players")
            .WithDescription("Returns all players.")
            .Produces<IEnumerable<PlayerResponse>>();

        group.MapGet("{id:guid}", GetById)
            .WithName(nameof(GetById) + "Player")
            .WithDescription("Returns a single player by ID.")
            .Produces<PlayerResponse>()
            .ProducesProblem(404);

        group.MapPost(string.Empty, Create)
            .WithName(nameof(Create) + "Player")
            .WithDescription("Creates a new player.")
            .Produces<PlayerResponse>(201)
            .ProducesProblem(400)
            .RequireAuthorization();

        group.MapDelete("{id:guid}", Delete)
            .WithName(nameof(Delete) + "Player")
            .WithDescription("Deletes a player by ID.")
            .ProducesProblem(404)
            .RequireAuthorization();
    }

    private static async Task<IResult> GetAll(IPlayerService playerService)
        => (await playerService.GetAllAsync()).Match(
            players => TypedResults.Ok(players),
            ex => ex.ToProblemResult());

    private static async Task<IResult> GetById(Guid id, IPlayerService playerService)
        => (await playerService.GetByIdAsync(id)).Match(
            player => TypedResults.Ok(player),
            ex => ex.ToProblemResult());

    private static async Task<IResult> Create(CreatePlayerRequest request, IPlayerService playerService)
        => (await playerService.CreateAsync(request)).Match(
            player => TypedResults.Created($"{BasePath}/{player.Id}", player),
            ex => ex.ToProblemResult());

    private static async Task<IResult> Delete(Guid id, IPlayerService playerService)
        => (await playerService.DeleteAsync(id)).Match(
            _ => TypedResults.NoContent(),
            ex => ex.ToProblemResult());
}

