using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Domain.Entities.Chess;

public class ChessEventParticipant
{
    public long TournamentId { get; set; }
    public ChessEvent? Tournament { get; set; }

    public long PlayerId { get; set; }
    public Player? Player { get; set; }

    public double Points { get; set; } = 0;
    public int Rank { get; set; } //місце
    public string? GroupName { get; set; }
}
