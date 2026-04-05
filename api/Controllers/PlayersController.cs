using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FribaScore.Api.Data;
using FribaScore.Api.Models;

namespace FribaScore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayersController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await db.Players.ToListAsync());

    [HttpPost]
    public async Task<IActionResult> Create(Player player)
    {
        player.Id = Guid.NewGuid();
        db.Players.Add(player);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = player.Id }, player);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var player = await db.Players.FindAsync(id);
        if (player is null) return NotFound();
        db.Players.Remove(player);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
