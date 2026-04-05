using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FribaScore.Api.Data;
using FribaScore.Api.Models;

namespace FribaScore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await db.Courses.ToListAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var course = await db.Courses.FindAsync(id);
        return course is null ? NotFound() : Ok(course);
    }
}
