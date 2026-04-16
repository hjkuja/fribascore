using FribaScore.Application.Services.Interfaces;
using FribaScore.Contracts.Requests.Rounds;
using FribaScore.Contracts.Responses;

namespace FribaScore.Api.Endpoints.Rounds;

/// <summary>
/// Maps round endpoints.
/// </summary>
public static class RoundEndpoints
{
    private const string BasePath = "api/rounds";

    /// <summary>
    /// Maps the round endpoints.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    public static void MapRoundEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(BasePath).WithTags("Rounds");

        group.MapGet(string.Empty, GetAll)
            .WithName(nameof(GetAll) + "Rounds")
            .WithSummary("Returns all rounds.")
            .WithDescription("Returns all rounds.")
            .Produces<IEnumerable<RoundResponse>>();

        group.MapGet("{id:guid}", GetById)
            .WithName(nameof(GetById) + "Round")
            .WithSummary("Returns a single round by ID.")
            .WithDescription("Returns a single round by ID.")
            .Produces<RoundResponse>()
            .ProducesProblem(404);

        group.MapPost(string.Empty, Create)
            .WithName(nameof(Create) + "Round")
            .WithSummary("Creates a new round.")
            .WithDescription("Creates a new round.")
            .Produces<RoundResponse>(201)
            .RequireAuthorization();

        group.MapDelete("{id:guid}", Delete)
            .WithName(nameof(Delete) + "Round")
            .WithSummary("Deletes a round by ID.")
            .WithDescription("Deletes a round by ID.")
            .ProducesProblem(404)
            .RequireAuthorization();
    }

    /// <summary>
    /// Returns all rounds.
    /// </summary>
    /// <param name="roundService">The round service.</param>
    /// <returns>All available rounds.</returns>
    /// <response code="200">The rounds were returned successfully.</response>
    private static async Task<IResult> GetAll(IRoundService roundService)
        => (await roundService.GetAllAsync()).Match(
            rounds => TypedResults.Ok(rounds),
            ex => ex.ToProblemResult());

    /// <summary>
    /// Returns a single round by ID.
    /// </summary>
    /// <param name="id">The round identifier.</param>
    /// <param name="roundService">The round service.</param>
    /// <returns>The requested round.</returns>
    /// <response code="200">The round was returned successfully.</response>
    /// <response code="404">The round was not found.</response>
    private static async Task<IResult> GetById(Guid id, IRoundService roundService)
        => (await roundService.GetByIdAsync(id)).Match(
            round => TypedResults.Ok(round),
            ex => ex.ToProblemResult());

    /// <summary>
    /// Creates a new round.
    /// </summary>
    /// <param name="request">The round creation request.</param>
    /// <param name="roundService">The round service.</param>
    /// <returns>The created round.</returns>
    /// <response code="201">The round was created successfully.</response>
    private static async Task<IResult> Create(CreateRoundRequest request, IRoundService roundService)
        => (await roundService.CreateAsync(request)).Match(
            round => TypedResults.Created($"{BasePath}/{round.Id}", round),
            ex => ex.ToProblemResult());

    /// <summary>
    /// Deletes a round by ID.
    /// </summary>
    /// <param name="id">The round identifier.</param>
    /// <param name="roundService">The round service.</param>
    /// <returns>An empty response when deletion succeeds.</returns>
    /// <response code="204">The round was deleted successfully.</response>
    /// <response code="404">The round was not found.</response>
    private static async Task<IResult> Delete(Guid id, IRoundService roundService)
        => (await roundService.DeleteAsync(id)).Match(
            _ => TypedResults.NoContent(),
            ex => ex.ToProblemResult());
}

