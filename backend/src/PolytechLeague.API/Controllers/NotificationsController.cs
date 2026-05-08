using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Dto;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Persistence.Database;
using System.Security.Claims;

namespace PolytechLeague.API.Controllers.Common;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public NotificationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe([FromBody] PushSubscriptionDto dto)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var existingSubscription = await _context.PushSubscriptions
            .FirstOrDefaultAsync(s => s.Endpoint == dto.Endpoint && s.UserId == userId);

        if (existingSubscription == null)
        {
            var subscription = new PushSubscription
            {
                UserId = userId,
                Endpoint = dto.Endpoint,
                P256DH = dto.Keys.P256dh,
                Auth = dto.Keys.Auth
            };

            _context.PushSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();
        }

        return Ok(new { message = "Subscription successful" });
    }
}
