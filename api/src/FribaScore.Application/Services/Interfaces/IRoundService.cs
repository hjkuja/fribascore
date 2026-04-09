using FribaScore.Contracts.Responses;
using LanguageExt.Common;

namespace FribaScore.Application.Services.Interfaces;

public interface IRoundService
{
    Task<Result<IEnumerable<RoundResponse>>> GetAllAsync();
    Task<Result<RoundResponse>> GetByIdAsync(Guid id);
    Task<Result<RoundResponse>> CreateAsync(Contracts.Requests.Rounds.CreateRoundRequest request);
    Task<Result<bool>> DeleteAsync(Guid id);
}
