using FribaScore.Application.Services.Interfaces;
using FribaScore.Contracts.Requests.Players;
using FribaScore.Contracts.Responses;

namespace FribaScore.Api.Endpoints.Players;

/// <summary>
/// Maps player endpoints.
/// </summary>
public static class PlayerEndpoints
{
    private const string BasePath = "api/players";

    /// <summary>
    /// Maps the player endpoints.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    public static void MapPlayerEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(BasePath).WithTags("Players");

        group.MapGet(string.Empty, GetAll)
            .WithName(nameof(GetAll) + "Players")
            .WithSummary("Returns all players.")
            .WithDescription("Returns all players.")
            .Produces<IEnumerable<PlayerResponse>>();

        group.MapGet("{id:guid}", GetById)
            .WithName(nameof(GetById) + "Player")
            .WithSummary("Returns a single player by ID.")
            .WithDescription("Returns a single player by ID.")
            .Produces<PlayerResponse>()
            .ProducesProblem(404);

        group.MapPost(string.Empty, Create)
            .WithName(nameof(Create) + "Player")
            .WithSummary("Creates a new player.")
            .WithDescription("Creates a new player.")
            .Produces<PlayerResponse>(201)
            .ProducesProblem(400)
            .RequireAuthorization();

        group.MapDelete("{id:guid}", Delete)
            .WithName(nameof(Delete) + "Player")
            .WithSummary("Deletes a player by ID.")
            .WithDescription("Deletes a player by ID.")
            .ProducesProblem(404)
            .RequireAuthorization();
    }

    /// <summary>
    /// Returns all players.
    /// </summary>
    /// <param name="playerService">The player service.</param>
    /// <returns>All available players.</returns>
    /// <response code="200">The players were returned successfully.</response>
    private static async Task<IResult> GetAll(IPlayerService playerService)
        => (await playerService.GetAllAsync()).Match(
            players => TypedResults.Ok(players),
            ex => ex.ToProblemResult());

    /// <summary>
    /// Returns a single player by ID.
    /// </summary>
    /// <param name="id">The player identifier.</param>
    /// <param name="playerService">The player service.</param>
    /// <returns>The requested player.</returns>
    /// <response code="200">The player was returned successfully.</response>
    /// <response code="404">The player was not found.</response>
    private static async Task<IResult> GetById(Guid id, IPlayerService playerService)
        => (await playerService.GetByIdAsync(id)).Match(
            player => TypedResults.Ok(player),
            ex => ex.ToProblemResult());

    /// <summary>
    /// Creates a new player.
    /// </summary>
    /// <param name="request">The player creation request.</param>
    /// <param name="playerService">The player service.</param>
    /// <returns>The created player.</returns>
    /// <response code="201">The player was created successfully.</response>
    /// <response code="400">The request payload is invalid.</response>
    private static async Task<IResult> Create(CreatePlayerRequest request, IPlayerService playerService)
        => (await playerService.CreateAsync(request)).Match(
            player => TypedResults.Created($"{BasePath}/{player.Id}", player),
            ex => ex.ToProblemResult());

    /// <summary>
    /// Deletes a player by ID.
    /// </summary>
    /// <param name="id">The player identifier.</param>
    /// <param name="playerService">The player service.</param>
    /// <returns>An empty response when deletion succeeds.</returns>
    /// <response code="204">The player was deleted successfully.</response>
    /// <response code="404">The player was not found.</response>
    private static async Task<IResult> Delete(Guid id, IPlayerService playerService)
        => (await playerService.DeleteAsync(id)).Match(
            _ => TypedResults.NoContent(),
            ex => ex.ToProblemResult());
}

