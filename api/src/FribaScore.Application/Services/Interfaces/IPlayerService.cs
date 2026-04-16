using FribaScore.Contracts.Responses;
using LanguageExt.Common;

namespace FribaScore.Application.Services.Interfaces;

/// <summary>
/// Provides player-related business operations.
/// </summary>
public interface IPlayerService
{
    /// <summary>
    /// Gets all players.
    /// </summary>
    /// <returns>All known players.</returns>
    Task<Result<IEnumerable<PlayerResponse>>> GetAllAsync();

    /// <summary>
    /// Gets a player by its identifier.
    /// </summary>
    /// <param name="id">The player identifier.</param>
    /// <returns>The matching player when found.</returns>
    Task<Result<PlayerResponse>> GetByIdAsync(Guid id);

    /// <summary>
    /// Creates a new player.
    /// </summary>
    /// <param name="request">The player creation request.</param>
    /// <returns>The created player.</returns>
    Task<Result<PlayerResponse>> CreateAsync(Contracts.Requests.Players.CreatePlayerRequest request);

    /// <summary>
    /// Deletes a player by its identifier.
    /// </summary>
    /// <param name="id">The player identifier.</param>
    /// <returns>A result indicating whether deletion succeeded.</returns>
    Task<Result<bool>> DeleteAsync(Guid id);
}
