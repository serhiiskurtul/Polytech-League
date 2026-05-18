using System;

namespace PolytechLeague.Domain.Entities;

public class Feedback
{
    public long Id { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public long? UserId { get; set; }
    public virtual User? User { get; set; }
    public string Type { get; set; } = "Suggestion"; // Suggestion, Complaint, Other
    public bool IsRead { get; set; } = false;
}
