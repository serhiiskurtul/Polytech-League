using System;

namespace PolytechLeague.Domain.Entities
{
    public class HallOfFameManualEntry
    {
        public long Id { get; set; }
        public required string Sport { get; set; } // e.g., "football", "volleyball"
        public required string SeasonName { get; set; } // e.g., "2022-2023" or "2015"
        public required string TeamName { get; set; }
        public string? PhotoUrl { get; set; } // Optional photo for the team
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
