using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace PolytechLeague.API.Controllers.Common;

[ApiController]
[Route("api/feedback")]
public class FeedbackController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public FeedbackController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> SubmitFeedback([FromBody] FeedbackSubmissionDto dto, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdStr, out long userId)) return Unauthorized();

        var feedback = new Feedback
        {
            Content = dto.Content,
            Type = dto.Type,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        _dbContext.Feedbacks.Add(feedback);
        await _dbContext.SaveChangesAsync(cancellationToken);

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
                f.Type,
                f.CreatedAt,
                f.IsRead,
                f.UserId,
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
}

public class FeedbackSubmissionDto
{
    public required string Content { get; set; }
    public string Type { get; set; } = "Suggestion"; // Suggestion, Complaint, Other
    public bool IsAnonymous { get; set; } = false;
}
