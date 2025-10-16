namespace CampusLeague.Domain.Entities.Chess;

public class ChessEventParticipant
{
    public long TournamentId { get; set; }
    public ChessEvent? Tournament { get; set; }

    public long PlayerId { get; set; }
    public ChessPlayer? Player { get; set; }

    public double Points { get; set; }
    public int Rank { get; set; } //місце
}
