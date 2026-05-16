using System;

namespace PolytechLeague.Domain.Entities
{
    public class ExternalAchievementMedia
    {
        public long Id { get; set; }
        public required string Url { get; set; }
        public required string Type { get; set; } // "photo" or "video"
        public long ExternalAchievementId { get; set; }
        public ExternalAchievement? ExternalAchievement { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
