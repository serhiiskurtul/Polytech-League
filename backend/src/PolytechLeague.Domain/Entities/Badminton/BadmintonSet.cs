namespace PolytechLeague.Domain.Entities.Badminton;

public class BadmintonSet
{
    public long Id { get; set; }
    
    public long MatchId { get; set; }
    public BadmintonMatch? Match { get; set; }

    public int SetNumber { get; set; }
    public int Player1Score { get; set; }
    public int Player2Score { get; set; }
}
