using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Entities.Shared;

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
    public string? GroupName { get; set; }
    public RegistrationStatus Status { get; set; } = RegistrationStatus.Approved;
}
