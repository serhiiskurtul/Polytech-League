using System;

namespace PolytechLeague.Domain.Entities;

public class Notification
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public User? User { get; set; }
    public required string Title { get; set; }
    public required string Message { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Link { get; set; }
    public NotificationType Type { get; set; } = NotificationType.System;
}

public enum NotificationType
{
    System,
    Tournament,
    Achievement,
    Match,
    Team
}
