namespace PolytechLeague.Domain.Entities.BeachVolleyball;

public class BeachVolleyballSet
{
    public long Id { get; set; }

    public long MatchId { get; set; }
    public BeachVolleyballMatch? Match { get; set; }

    public int SetNumber { get; set; }
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
}
