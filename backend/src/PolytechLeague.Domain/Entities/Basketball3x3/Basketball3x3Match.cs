namespace PolytechLeague.Domain.Entities.Basketball3x3;

public class Basketball3x3Match
{
    public long Id { get; set; }
    public long EventId { get; set; }
    public Basketball3x3Event? Event { get; set; }
    
    public long HomeTeamId { get; set; }
    public Basketball3x3Team? HomeTeam { get; set; }
    
    public long AwayTeamId { get; set; }
    public Basketball3x3Team? AwayTeam { get; set; }
    
    public ICollection<Player>? HomeRoster { get; set; }
    public ICollection<Player>? AwayRoster { get; set; }
    
    public int? HomeTeamScore { get; set; }
    public int? AwayTeamScore { get; set; }
    
    public long? WinnerTeamId { get; set; }
    public Basketball3x3Team? WinnerTeam { get; set; }
    
    public string? Stage { get; set; } // e.g., Group Stage, Quarterfinal

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<long>? HomeRosterIds { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<long>? AwayRosterIds { get; set; }
}
