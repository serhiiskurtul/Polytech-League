namespace PolytechLeague.Domain.Entities.Football;

public enum FootballCardType
{
    Yellow = 1,
    Red = 2
}

public class FootballCard
{
    public long Id { get; set; }
    public long MatchId { get; set; }
    public virtual FootballMatch? Match { get; set; }
    
    public long PlayerId { get; set; }
    public virtual Player? Player { get; set; }
    
    public FootballCardType Type { get; set; }
}
