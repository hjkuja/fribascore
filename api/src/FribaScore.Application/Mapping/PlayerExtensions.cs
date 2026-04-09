using FribaScore.Application.Models;
using FribaScore.Contracts.Responses;

namespace FribaScore.Application.Mapping;

public static class PlayerExtensions
{
    public static PlayerResponse ToResponse(this Player player) =>
        new(player.Id, player.Name);
}
