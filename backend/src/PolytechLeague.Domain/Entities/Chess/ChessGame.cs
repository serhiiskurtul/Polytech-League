using PolytechLeague.Domain.Entities.Shared;

namespace PolytechLeague.Domain.Entities.Chess;

public class ChessGame
{
    public long Id { get; set; }
    public long TournamentId { get; set; }
    public ChessEvent? Tournament { get; set; }

    public long WhitePlayerId { get; set; }
    public long BlackPlayerId { get; set; }
    public ChessPlayer? WhitePlayer { get; set; }
    public ChessPlayer? BlackPlayer { get; set; }

    public GameResult Result { get; set; }
    public int Round { get; set; }
    public int Board { get; set; }
}