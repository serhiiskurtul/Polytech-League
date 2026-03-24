namespace PolytechLeague.Domain.Entities.Volleyball;

public class VolleyballMatch
{
    public long Id { get; set; }
    public long EventId { get; set; }
    public VolleyballEvent? Event { get; set; }

    public long HomeTeamId { get; set; }
    public VolleyballTeam? HomeTeam { get; set; }

    public long AwayTeamId { get; set; }
    public VolleyballTeam? AwayTeam { get; set; }

    public int SetsToWin { get; set; } = 3;

    public int? HomeSetsWon { get; set; }
    public int? AwaySetsWon { get; set; }

    public long? WinnerTeamId { get; set; }
    public VolleyballTeam? WinnerTeam { get; set; }

    public string? Stage { get; set; }

    public ICollection<VolleyballSet>? Sets { get; set; }
}
