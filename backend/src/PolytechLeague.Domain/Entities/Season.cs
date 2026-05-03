namespace PolytechLeague.Domain.Entities;

public class Season
{
    public long Id { get; set; }
    public required string Name { get; set; } // e.g. "2023-2024"
    public bool IsCurrent { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? SpartakiadNumber { get; set; }
    
    public string? NationalRatingSports { get; set; } // JSON list of active sports for national rating
    public string? RegionalRatingSports { get; set; } // JSON list of active sports for regional rating

    public ICollection<Event>? Events { get; set; }
}

