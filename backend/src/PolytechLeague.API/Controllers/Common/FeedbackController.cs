using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using PolytechLeague.Application.Services.Interfaces;

namespace PolytechLeague.API.Controllers.Common;

[ApiController]
[Route("api/feedback")]
public class FeedbackController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IFileService _fileService;
    private readonly INotificationService _notificationService;

    public FeedbackController(ApplicationDbContext dbContext, IFileService fileService, INotificationService notificationService)
    {
        _dbContext = dbContext;
        _fileService = fileService;
        _notificationService = notificationService;
    }

    [Authorize]
    [HttpPost]
    [DisableRequestSizeLimit]
    [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = 2147483648)]
    public async Task<IActionResult> SubmitFeedback([FromForm] FeedbackSubmissionFormDto dto, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdStr, out long userId)) return Unauthorized();

        string? photoUrl = null;
        if (dto.File != null && dto.File.Length > 0)
        {
            photoUrl = await _fileService.SaveFileAsync(dto.File, "feedback", cancellationToken);
        }

        var feedback = new Feedback
        {
            Content = dto.Content,
            Type = dto.Type,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            IsRead = false,
            PhotoUrl = photoUrl
        };

        _dbContext.Feedbacks.Add(feedback);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var user = await _dbContext.Users.FindAsync(new object[] { userId }, cancellationToken);
        var senderName = user != null ? $"{user.FirstName} {user.LastName}" : "Користувач";

        string title = dto.Type switch
        {
            FeedbackType.Complaint => "Нова скарга",
            FeedbackType.Suggestion => "Нова пропозиція",
            _ => "Новий відгук"
        };

        string contentPreview = dto.Content.Length > 60
            ? dto.Content[..60] + "..."
            : dto.Content;

        string message = $"Від: {senderName}. Текст: {contentPreview}";

        await _notificationService.SendNotificationToAdminsAsync(
            title,
            message,
            NotificationType.System,
            "/admin/feedback"
        );

        return Ok(new { message = "Дякуємо за ваш відгук!" });
    }

    [Authorize(Policy = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetFeedbacks(CancellationToken cancellationToken)
    {
        var feedbacks = await _dbContext.Feedbacks
            .Include(f => f.User)
            .OrderByDescending(f => f.CreatedAt)
            .Select(f => new
            {
                f.Id,
                f.Content,
                Type = f.Type.ToString(),
                f.CreatedAt,
                f.IsRead,
                f.UserId,
                f.PhotoUrl,
                AuthorName = f.User != null ? $"{f.User.FirstName} {f.User.LastName}" : "Анонімно"
            })
            .ToListAsync(cancellationToken);

        return Ok(feedbacks);
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(long id, CancellationToken cancellationToken)
    {
        var feedback = await _dbContext.Feedbacks.FindAsync(new object[] { id }, cancellationToken);
        if (feedback == null) return NotFound();

        feedback.IsRead = true;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFeedback(long id, CancellationToken cancellationToken)
    {
        var feedback = await _dbContext.Feedbacks.FindAsync(new object[] { id }, cancellationToken);
        if (feedback == null) return NotFound();

        if (!string.IsNullOrEmpty(feedback.PhotoUrl))
        {
            try
            {
                _fileService.DeleteFile(feedback.PhotoUrl);
            }
            catch { /* ignore if file not found */ }
        }

        _dbContext.Feedbacks.Remove(feedback);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}

public class FeedbackSubmissionFormDto
{
    public required string Content { get; set; }
    public FeedbackType Type { get; set; } = FeedbackType.Suggestion;
    public IFormFile? File { get; set; }
}
