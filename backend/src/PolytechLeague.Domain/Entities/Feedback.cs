using System;

namespace PolytechLeague.Domain.Entities;

public class Feedback
{
    public long Id { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public long? UserId { get; set; }
    public virtual User? User { get; set; }
    public FeedbackType Type { get; set; } = FeedbackType.Suggestion;
    public bool IsRead { get; set; } = false;
    public string? PhotoUrl { get; set; }
}

public enum FeedbackType
{
    Suggestion,
    Complaint,
    Other
}
