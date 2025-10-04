namespace CampusLeague.Domain.Entities.Chess;

public class ChessEventParticipant
{
    public long TournamentId { get; set; }
    public required ChessEvent Tournament { get; set; }

    public long PlayerId { get; set; }
    public required ChessPlayer Player { get; set; }

    public double Points { get; set; }
    public int Rank { get; set; } //місце
}
