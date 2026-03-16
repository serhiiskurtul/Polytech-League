namespace PolytechLeague.Domain.Entities.Checkers;

public class CheckersPlayer : Player
{
    public int Rating { get; set; }
    public required string Title { get; set; } // Candidate Master, Master of Sports, etc.
}
