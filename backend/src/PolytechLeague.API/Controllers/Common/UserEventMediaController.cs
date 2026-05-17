using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace PolytechLeague.API.Controllers.Common;

[ApiController]
[Route("api/user-event-media")]
public class UserEventMediaController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IFileService _fileService;
    private readonly INotificationService _notificationService;

    public UserEventMediaController(ApplicationDbContext dbContext, IFileService fileService, INotificationService notificationService)
    {
        _dbContext = dbContext;
        _fileService = fileService;
        _notificationService = notificationService;
    }

    [Authorize]
    [HttpPost("{eventId}")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> SubmitMedia(long eventId, IFormFile file, [FromForm] string? comment, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdStr, out long userId)) return Unauthorized();

        var @event = await _dbContext.Events.FindAsync(new object[] { eventId }, cancellationToken);
        if (@event == null) return NotFound("Event not found");

        if (file == null || file.Length == 0) return BadRequest("File is required");

        // Determine folder based on file type
        string type = file.ContentType.StartsWith("video/") ? "video" : "photo";
        string folder = type == "video" ? "events/user-videos" : "events/user-photos";
        
        var url = await _fileService.SaveFileAsync(file, folder, cancellationToken);
        
        var media = new UserEventMedia
        {
            Url = url,
            Type = type,
            Comment = comment,
            UserId = userId,
            EventId = eventId,
            Status = ModerationStatus.Pending
        };

        _dbContext.UserEventMedias.Add(media);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _notificationService.SendNotificationToAdminsAsync(
            "Новий медіа матеріал на модерацію",
            $"Користувач завантажив медіа до змагання {@event.Name}",
            NotificationType.Tournament,
            $"/tournaments/{eventId}"
        );

        return Ok(new { message = "Медіа матеріал відправлено на модерацію", id = media.Id });
    }

    [HttpGet("{eventId}")]
    public async Task<IActionResult> GetApprovedMedia(long eventId, CancellationToken cancellationToken)
    {
        var media = await _dbContext.UserEventMedias
            .Where(m => m.EventId == eventId && m.Status == ModerationStatus.Approved)
            .Include(m => m.User)
            .Select(m => new
            {
                m.Id,
                m.Url,
                m.Type,
                m.Comment,
                m.CreatedAt,
                AuthorName = m.User != null ? $"{m.User.FirstName} {m.User.LastName}" : "Unknown"
            })
            .ToListAsync(cancellationToken);

        return Ok(media);
    }

    [Authorize(Policy = "Admin")]
    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingMedia(CancellationToken cancellationToken)
    {
        var media = await _dbContext.UserEventMedias
            .Where(m => m.Status == ModerationStatus.Pending)
            .Include(m => m.User)
            .Include(m => m.Event)
            .Select(m => new
            {
                m.Id,
                m.Url,
                m.Type,
                m.Comment,
                m.CreatedAt,
                m.EventId,
                EventName = m.Event != null ? m.Event.Name : "Unknown",
                AuthorName = m.User != null ? $"{m.User.FirstName} {m.User.LastName}" : "Unknown"
            })
            .ToListAsync(cancellationToken);

        return Ok(media);
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(long id, [FromBody] UpdateMediaStatusDto dto, CancellationToken cancellationToken)
    {
        var media = await _dbContext.UserEventMedias.FindAsync(new object[] { id }, cancellationToken);
        if (media == null) return NotFound();

        media.Status = dto.Status;
        await _dbContext.SaveChangesAsync(cancellationToken);

        var @event = await _dbContext.Events.FindAsync(new object[] { media.EventId }, cancellationToken);
        var eventName = @event?.Name ?? "змагання";
        var statusStr = dto.Status == ModerationStatus.Approved ? "схвалено" : "відхилено";

        await _notificationService.SendNotificationToUserAsync(
            media.UserId,
            "Статус модерації медіа",
            $"Ваш медіа матеріал для {eventName} було {statusStr}",
            NotificationType.Tournament,
            $"/tournaments/{media.EventId}"
        );

        return NoContent();
    }
}

public class UpdateMediaStatusDto
{
    public ModerationStatus Status { get; set; }
}
