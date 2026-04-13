namespace PolytechLeague.Domain.Entities.Volleyball;

public class VolleyballMatch
{
    public long Id { get; set; }
    public long EventId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public VolleyballEvent? Event { get; set; }

    public long HomeTeamId { get; set; }
    public VolleyballTeam? HomeTeam { get; set; }

    public long AwayTeamId { get; set; }
    public VolleyballTeam? AwayTeam { get; set; }

    public ICollection<Player>? HomeRoster { get; set; }
    public ICollection<Player>? AwayRoster { get; set; }

    public int SetsToWin { get; set; } = 3;

    public int? HomeSetsWon { get; set; }
    public int? AwaySetsWon { get; set; }

    public long? WinnerTeamId { get; set; }
    public VolleyballTeam? WinnerTeam { get; set; }

    public string? Stage { get; set; }

    public ICollection<VolleyballSet>? Sets { get; set; }
    
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<long>? HomeRosterIds { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<long>? AwayRosterIds { get; set; }
}
