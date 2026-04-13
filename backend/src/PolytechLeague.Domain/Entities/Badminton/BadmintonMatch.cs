using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Domain.Entities.Badminton;

public class BadmintonMatch
{
    public long Id { get; set; }
    
    public long TournamentId { get; set; }
    public BadmintonEvent? Tournament { get; set; }

    public long? Player1Id { get; set; }
    public Player? Player1 { get; set; }

    public long? Player2Id { get; set; }
    public Player? Player2 { get; set; }

    public long? Team1Id { get; set; }
    public BadmintonTeam? Team1 { get; set; }

    public long? Team2Id { get; set; }
    public BadmintonTeam? Team2 { get; set; }

    public ICollection<Player>? HomeRoster { get; set; }
    public ICollection<Player>? AwayRoster { get; set; }

    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }

    public long? WinnerPlayerId { get; set; }
    public Player? WinnerPlayer { get; set; }
    
    public long? WinnerTeamId { get; set; }
    public BadmintonTeam? WinnerTeam { get; set; }

    public string? Stage { get; set; }
    public int CourtNumber { get; set; }
    public int SetsToWin { get; set; } = 2; // Usually best of 3

    public ICollection<BadmintonSet>? Sets { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<long>? HomeRosterIds { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<long>? AwayRosterIds { get; set; }
}
