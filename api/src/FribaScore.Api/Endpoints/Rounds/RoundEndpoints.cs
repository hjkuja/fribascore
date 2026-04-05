using FribaScore.Application.Services.Interfaces;
using FribaScore.Contracts.Requests.Rounds;
using FribaScore.Contracts.Responses;

namespace FribaScore.Api.Endpoints.Rounds;

public static class RoundEndpoints
{
    private const string BasePath = "api/rounds";

    public static void MapRoundEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(BasePath).WithTags("Rounds");

        group.MapGet(string.Empty, GetAll)
            .WithName(nameof(GetAll) + "Rounds")
            .WithDescription("Returns all rounds.")
            .Produces<IEnumerable<RoundResponse>>();

        group.MapGet("{id:guid}", GetById)
            .WithName(nameof(GetById) + "Round")
            .WithDescription("Returns a single round by ID.")
            .Produces<RoundResponse>()
            .ProducesProblem(404);

        group.MapPost(string.Empty, Create)
            .WithName(nameof(Create) + "Round")
            .WithDescription("Creates a new round.")
            .Produces<RoundResponse>(201)
            .RequireAuthorization();

        group.MapDelete("{id:guid}", Delete)
            .WithName(nameof(Delete) + "Round")
            .WithDescription("Deletes a round by ID.")
            .ProducesProblem(404)
            .RequireAuthorization();
    }

    private static async Task<IResult> GetAll(IRoundService roundService)
        => (await roundService.GetAllAsync()).Match(
            rounds => TypedResults.Ok(rounds),
            ex => ex.ToProblemResult());

    private static async Task<IResult> GetById(Guid id, IRoundService roundService)
        => (await roundService.GetByIdAsync(id)).Match(
            round => TypedResults.Ok(round),
            ex => ex.ToProblemResult());

    private static async Task<IResult> Create(CreateRoundRequest request, IRoundService roundService)
        => (await roundService.CreateAsync(request)).Match(
            round => TypedResults.Created($"{BasePath}/{round.Id}", round),
            ex => ex.ToProblemResult());

    private static async Task<IResult> Delete(Guid id, IRoundService roundService)
        => (await roundService.DeleteAsync(id)).Match(
            _ => TypedResults.NoContent(),
            ex => ex.ToProblemResult());
}

