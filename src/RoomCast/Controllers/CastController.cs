using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomCast.Data;

[ApiController]
[Route("api/cast")]
public class CastController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CastController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("assigned-screen")]
    public async Task<IActionResult> GetAssignedScreen()
    {
        var cast = await _context.CastingAssignments.FirstAsync(c => c.Id == 1);

        return Ok(new
        {
            screenId = cast.CurrentScreenId
        });
    }
}
