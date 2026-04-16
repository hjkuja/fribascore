using FribaScore.Application.Models;
using FribaScore.Contracts.Responses;

namespace FribaScore.Application.Mapping;

/// <summary>
/// Provides extension methods for mapping Player entities to their response DTOs.
/// </summary>
public static class PlayerExtensions
{
    /// <summary>
    /// Converts a Player entity to its response DTO.
    /// </summary>
    /// <param name="player">The player entity to convert.</param>
    /// <returns>The player response DTO.</returns>
    public static PlayerResponse ToResponse(this Player player) =>
        new(player.Id, player.Name);
}
