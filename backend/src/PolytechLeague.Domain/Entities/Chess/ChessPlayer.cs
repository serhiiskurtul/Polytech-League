namespace PolytechLeague.Domain.Entities.Chess;

public class ChessPlayer : Player
{
    public int RatingELO { get; set; }
    public string? FideId { get; set; }
    public string? Title { get; set; }

}
