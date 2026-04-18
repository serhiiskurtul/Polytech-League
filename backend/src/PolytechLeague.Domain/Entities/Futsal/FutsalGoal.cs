namespace PolytechLeague.Domain.Entities.Futsal;

public class FutsalGoal
{
    public long Id { get; set; }
    public long MatchId { get; set; }
    public virtual FutsalMatch? Match { get; set; }
    
    public long PlayerId { get; set; }
    public virtual Player? Player { get; set; }
    
    public int GoalsCount { get; set; }
}
