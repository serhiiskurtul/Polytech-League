using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Domain.Entities.Checkers;

public class CheckersEventParticipant
{
    public long Id { get; set; }
    public long TournamentId { get; set; }
    public CheckersEvent Event { get; set; } = null!;
    public long PlayerId { get; set; }
    public Player Player { get; set; } = null!; 

    // Поточний рейтинг гравця у шашках на момент початку турніру
    public int StartingRating { get; set; }
    
    // Статистика гравця у цьому конкретному турнірі
    public double Points { get; set; } = 0;
    public int Rank { get; set; } // місце
}
