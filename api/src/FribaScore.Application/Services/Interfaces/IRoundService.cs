using FribaScore.Contracts.Responses;
using LanguageExt.Common;

namespace FribaScore.Application.Services.Interfaces;

/// <summary>
/// Provides round-related business operations.
/// </summary>
public interface IRoundService
{
    /// <summary>
    /// Gets all rounds.
    /// </summary>
    /// <returns>All known rounds.</returns>
    Task<Result<IEnumerable<RoundResponse>>> GetAllAsync();

    /// <summary>
    /// Gets a round by its identifier.
    /// </summary>
    /// <param name="id">The round identifier.</param>
    /// <returns>The matching round when found.</returns>
    Task<Result<RoundResponse>> GetByIdAsync(Guid id);

    /// <summary>
    /// Creates a new round.
    /// </summary>
    /// <param name="request">The round creation request.</param>
    /// <returns>The created round.</returns>
    Task<Result<RoundResponse>> CreateAsync(Contracts.Requests.Rounds.CreateRoundRequest request);

    /// <summary>
    /// Deletes a round by its identifier.
    /// </summary>
    /// <param name="id">The round identifier.</param>
    /// <returns>A result indicating whether deletion succeeded.</returns>
    Task<Result<bool>> DeleteAsync(Guid id);
}
