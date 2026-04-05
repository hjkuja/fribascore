using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FribaScore.Api.Data;
using FribaScore.Api.Models;

namespace FribaScore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoundsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await db.Rounds.ToListAsync());

    [HttpPost]
    public async Task<IActionResult> Create(Round round)
    {
        round.Id = Guid.NewGuid();
        round.Date = DateTime.UtcNow;
        db.Rounds.Add(round);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = round.Id }, round);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var round = await db.Rounds.FindAsync(id);
        return round is null ? NotFound() : Ok(round);
    }
}
