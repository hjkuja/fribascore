using FribaScore.Contracts.Responses;
using LanguageExt.Common;

namespace FribaScore.Application.Services.Interfaces;

public interface IPlayerService
{
    Task<Result<IEnumerable<PlayerResponse>>> GetAllAsync();
    Task<Result<PlayerResponse>> GetByIdAsync(Guid id);
    Task<Result<PlayerResponse>> CreateAsync(Contracts.Requests.Players.CreatePlayerRequest request);
    Task<Result<bool>> DeleteAsync(Guid id);
}
