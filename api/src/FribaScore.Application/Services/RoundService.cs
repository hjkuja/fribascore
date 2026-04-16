using FribaScore.Application.Database;
using FribaScore.Application.Mapping;
using FribaScore.Application.Models;
using FribaScore.Application.Services.Interfaces;
using FribaScore.Contracts.Exceptions;
using FribaScore.Contracts.Requests.Rounds;
using FribaScore.Contracts.Responses;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;

namespace FribaScore.Application.Services;

/// <summary>
/// Implements round-related business operations.
/// </summary>
public class RoundService(AppDbContext db) : IRoundService
{
    /// <inheritdoc />
    public async Task<Result<IEnumerable<RoundResponse>>> GetAllAsync()
    {
        try
        {
            var rounds = await db.Rounds.ToListAsync();
            return new Result<IEnumerable<RoundResponse>>(rounds.Select(r => r.ToResponse()));
        }
        catch (Exception ex)
        {
            return new Result<IEnumerable<RoundResponse>>(ex);
        }
    }

    /// <inheritdoc />
    public async Task<Result<RoundResponse>> GetByIdAsync(Guid id)
    {
        try
        {
            var round = await db.Rounds.FindAsync(id);
            if (round is null)
                return new Result<RoundResponse>(new NotFoundException(nameof(Round)));
            return new Result<RoundResponse>(round.ToResponse());
        }
        catch (Exception ex)
        {
            return new Result<RoundResponse>(ex);
        }
    }

    /// <inheritdoc />
    public async Task<Result<RoundResponse>> CreateAsync(CreateRoundRequest request)
    {
        try
        {
            var round = new Round
            {
                CourseId = request.CourseId,
                Scores = request.Scores.Select(s => new ScoreEntry
                {
                    PlayerId = s.PlayerId,
                    HoleNumber = s.HoleNumber,
                    Score = s.Score
                }).ToList()
            };

            db.Rounds.Add(round);
            await db.SaveChangesAsync();
            return new Result<RoundResponse>(round.ToResponse());
        }
        catch (Exception ex)
        {
            return new Result<RoundResponse>(ex);
        }
    }

    /// <inheritdoc />
    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var round = await db.Rounds.FindAsync(id);
            if (round is null)
                return new Result<bool>(new NotFoundException(nameof(Round)));
            db.Rounds.Remove(round);
            await db.SaveChangesAsync();
            return new Result<bool>(true);
        }
        catch (Exception ex)
        {
            return new Result<bool>(ex);
        }
    }
}
