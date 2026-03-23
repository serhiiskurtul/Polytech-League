namespace PolytechLeague.Domain.Entities.TableTennis;

public class TableTennisSet
{
    public long Id { get; set; }
    
    public long MatchId { get; set; }
    public TableTennisMatch? Match { get; set; }

    public int SetNumber { get; set; }
    public int Player1Score { get; set; }
    public int Player2Score { get; set; }
}
