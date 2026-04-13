namespace PolytechLeague.Domain.Entities.BeachVolleyball;

public class BeachVolleyballMatch
{
    public long Id { get; set; }
    public long EventId { get; set; }
    public BeachVolleyballEvent? Event { get; set; }

    public long HomeTeamId { get; set; }
    public BeachVolleyballTeam? HomeTeam { get; set; }

    public long AwayTeamId { get; set; }
    public BeachVolleyballTeam? AwayTeam { get; set; }

    public ICollection<Player>? HomeRoster { get; set; }
    public ICollection<Player>? AwayRoster { get; set; }

    public int SetsToWin { get; set; } = 2;

    public int? HomeSetsWon { get; set; }
    public int? AwaySetsWon { get; set; }

    public long? WinnerTeamId { get; set; }
    public BeachVolleyballTeam? WinnerTeam { get; set; }

    public string? Stage { get; set; }

    public ICollection<BeachVolleyballSet>? Sets { get; set; }
    
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<long>? HomeRosterIds { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<long>? AwayRosterIds { get; set; }
}
