using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto;
using PolytechLeague.Application.Services.Interfaces;

namespace PolytechLeague.API.Controllers.Common;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotificationsController(INotificationService _notificationService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetMyNotifications()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdStr, out long userId)) return Unauthorized();

        var notifications = await _notificationService.GetUserNotificationsAsync(userId);
        return Ok(notifications);
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(long id)
    {
        await _notificationService.MarkAsReadAsync(id);
        return NoContent();
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdStr, out long userId)) return Unauthorized();

        await _notificationService.MarkAllAsReadAsync(userId);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _notificationService.DeleteNotificationAsync(id);
        return NoContent();
    }

    [HttpDelete("clear-read")]
    public async Task<IActionResult> ClearRead()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdStr, out long userId)) return Unauthorized();

        await _notificationService.ClearUserNotificationsAsync(userId);
        return NoContent();
    }
}
