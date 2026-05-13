namespace PolytechLeague.Domain.Entities.Football;

public class FootballGoal
{
    public long Id { get; set; }
    public long MatchId { get; set; }
    public virtual FootballMatch? Match { get; set; }
    
    public long PlayerId { get; set; }
    public virtual Player? Player { get; set; }
    
    public int GoalsCount { get; set; }
}
