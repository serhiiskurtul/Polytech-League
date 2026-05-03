namespace PolytechLeague.Domain.Entities;

public class RegionalRatingManualEntry
{
    public long Id { get; set; }
    public long SeasonId { get; set; }
    public Season? Season { get; set; }
    public long CollegeId { get; set; }
    public College? College { get; set; }
    public int Points { get; set; }
    public string? Rank { get; set; }
}
