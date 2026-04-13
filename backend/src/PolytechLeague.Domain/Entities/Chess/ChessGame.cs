using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Entities.Shared;

namespace PolytechLeague.Domain.Entities.Chess;

public class ChessGame
{
    public long Id { get; set; }
    public long TournamentId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public ChessEvent? Tournament { get; set; }

    public long WhitePlayerId { get; set; }
    public long BlackPlayerId { get; set; }
    public Player? WhitePlayer { get; set; }
    public Player? BlackPlayer { get; set; }

    public GameResult Result { get; set; }
    public int Round { get; set; }
    public int Board { get; set; }
}