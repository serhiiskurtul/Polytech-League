using PolytechLeague.Domain.Entities.Shared;

namespace PolytechLeague.Domain.Entities.Checkers;

public class CheckersGame
{
    public long Id { get; set; }
    
    public long TournamentId { get; set; }
    public CheckersEvent Event { get; set; } = null!;

    public int Round { get; set; }

    public long WhitePlayerId { get; set; }
    public CheckersEventParticipant WhitePlayer { get; set; } = null!;

    public long BlackPlayerId { get; set; }
    public CheckersEventParticipant BlackPlayer { get; set; } = null!;

    public GameResult Result { get; set; }
    
    // Формат запису партії (наприклад, нотація PDN - Portable Draughts Notation)
    //public string? PdnRecord { get; set; }

    public DateTime ScheduledAt { get; set; }
    public DateTime? FinishedAt { get; set; }
}
