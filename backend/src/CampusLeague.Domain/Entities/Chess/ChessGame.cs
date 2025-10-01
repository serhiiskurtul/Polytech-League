namespace CampusLeague.Domain.Entities.Chess;

public class ChessGame
{
    public long Id { get; set; }
    public long TournamentId { get; set; }
    public required ChessEvent Tournament { get; set; }

    public long WhitePlayerId { get; set; }
    public long BlackPlayerId { get; set; }

    public GameResult Result { get; set; }
    public int Round { get; set; }
    public int Board { get; set; }
}
public enum GameResult
{
    WhiteWin,
    BlackWin,
    Draw
}