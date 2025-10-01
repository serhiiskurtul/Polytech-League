namespace CampusLeague.Domain.Entities.Chess;

public class ChessPlayer : Player
{
    public int RatingELO { get; set; }
    public required string FideId { get; set; }
    public required string Title { get; set; }

}
