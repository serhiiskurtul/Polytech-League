using PolytechLeague.Domain.Entities;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Dto;
using PolytechLeague.Application.Services.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PolytechLeague.API.Controllers.Common;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotificationsController(
    INotificationService _notificationService,
    ApplicationDbContext _context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetMyNotifications()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdStr, out long userId)) return Unauthorized();

        var notifications = await _notificationService.GetUserNotificationsAsync(userId);
        return Ok(notifications);
    }

    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe([FromBody] PushSubscriptionDto dto)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdStr, out var userId)) return Unauthorized();

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
        }
        else
        {
            existingSubscription.P256DH = dto.Keys.P256dh;
            existingSubscription.Auth = dto.Keys.Auth;
            _context.PushSubscriptions.Update(existingSubscription);
        }

        await _context.SaveChangesAsync();

        return Ok(new { message = "Subscription successful" });
    }

    [HttpGet("test-push")]
    public async Task<IActionResult> TestPush()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdStr, out long userId)) return Unauthorized();

        await _notificationService.SendNotificationToUserAsync(
            userId,
            "Тестове сповіщення",
            "Якщо ви це бачите, значить пуш-сповіщення працюють!",
            NotificationType.System,
            "/"
        );

        return Ok(new { message = "Test notification sent. Check your device." });
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
