using System;

namespace PolytechLeague.Domain.Entities
{
    public class ExternalAchievement
    {
        public long Id { get; set; }
        public required string StudentName { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public string? PhotoUrl { get; set; }
        public string? ExternalLink { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
