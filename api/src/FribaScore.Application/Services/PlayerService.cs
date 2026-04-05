using FribaScore.Application.Database;
using FribaScore.Application.Mapping;
using FribaScore.Application.Models;
using FribaScore.Application.Services.Interfaces;
using FribaScore.Contracts.Exceptions;
using FribaScore.Contracts.Requests.Players;
using FribaScore.Contracts.Responses;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;

namespace FribaScore.Application.Services;

public class PlayerService(AppDbContext db) : IPlayerService
{
    public async Task<Result<IEnumerable<PlayerResponse>>> GetAllAsync()
    {
        try
        {
            var players = await db.Players.ToListAsync();
            return new Result<IEnumerable<PlayerResponse>>(players.Select(p => p.ToResponse()));
        }
        catch (Exception ex)
        {
            return new Result<IEnumerable<PlayerResponse>>(ex);
        }
    }

    public async Task<Result<PlayerResponse>> GetByIdAsync(Guid id)
    {
        try
        {
            var player = await db.Players.FindAsync(id);
            if (player is null)
                return new Result<PlayerResponse>(new NotFoundException(nameof(Player)));
            return new Result<PlayerResponse>(player.ToResponse());
        }
        catch (Exception ex)
        {
            return new Result<PlayerResponse>(ex);
        }
    }

    public async Task<Result<PlayerResponse>> CreateAsync(CreatePlayerRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return new Result<PlayerResponse>(new BadRequestException("Player name is required."));

            var player = new Player { Name = request.Name };
            db.Players.Add(player);
            await db.SaveChangesAsync();
            return new Result<PlayerResponse>(player.ToResponse());
        }
        catch (Exception ex)
        {
            return new Result<PlayerResponse>(ex);
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var player = await db.Players.FindAsync(id);
            if (player is null)
                return new Result<bool>(new NotFoundException(nameof(Player)));
            db.Players.Remove(player);
            await db.SaveChangesAsync();
            return new Result<bool>(true);
        }
        catch (Exception ex)
        {
            return new Result<bool>(ex);
        }
    }
}
