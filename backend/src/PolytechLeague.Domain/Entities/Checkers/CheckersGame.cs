using PolytechLeague.Domain.Entities.Shared;

namespace PolytechLeague.Domain.Entities.Checkers;

public class CheckersGame
{
    public long Id { get; set; }
    
    public long TournamentId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public CheckersEvent Event { get; set; } = null!;

    public int Round { get; set; }

    public long? WhitePlayerId { get; set; }
    public long? BlackPlayerId { get; set; }
    public CheckersEventParticipant WhitePlayer { get; set; } = null!;
    public CheckersEventParticipant BlackPlayer { get; set; } = null!;

    public long? WhiteTeamId { get; set; }
    public long? BlackTeamId { get; set; }
    public CheckersTeam? WhiteTeam { get; set; }
    public CheckersTeam? BlackTeam { get; set; }

    public double? WhiteTeamScore { get; set; }
    public double? BlackTeamScore { get; set; }

    public GameResult Result { get; set; }
    
    public DateTime ScheduledAt { get; set; }
    public DateTime? FinishedAt { get; set; }
}
