using System;

namespace PolytechLeague.Domain.Entities
{
    public class UserEventMedia
    {
        public long Id { get; set; }
        public required string Url { get; set; }
        public required string Type { get; set; } // "photo" or "video"
        public string? Comment { get; set; }
        
        public long UserId { get; set; }
        public User? User { get; set; }
        
        public long EventId { get; set; }
        public Event? Event { get; set; }
        
        public ModerationStatus Status { get; set; } = ModerationStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum ModerationStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
