namespace PolytechLeague.Domain.Entities.Volleyball;

public class VolleyballSet
{
    public long Id { get; set; }

    public long MatchId { get; set; }
    public VolleyballMatch? Match { get; set; }

    public int SetNumber { get; set; }
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
}
