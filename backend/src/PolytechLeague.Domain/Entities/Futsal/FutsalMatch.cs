namespace PolytechLeague.Domain.Entities.Futsal;

public class FutsalMatch
{
    public long Id { get; set; }
    public long EventId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public FutsalEvent? Event { get; set; }
    
    public long HomeTeamId { get; set; }
    public FutsalTeam? HomeTeam { get; set; }
    
    public long AwayTeamId { get; set; }
    public FutsalTeam? AwayTeam { get; set; }
    
    public ICollection<Player>? HomeRoster { get; set; }
    public ICollection<Player>? AwayRoster { get; set; }
    
    public int? HomeTeamScore { get; set; }
    public int? AwayTeamScore { get; set; }
    
    public long? WinnerTeamId { get; set; }
    public FutsalTeam? WinnerTeam { get; set; }
    
    public string? Stage { get; set; } // e.g., Group Stage, Quarterfinal

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<long>? HomeRosterIds { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<long>? AwayRosterIds { get; set; }

    public virtual ICollection<FutsalGoal>? Goals { get; set; }
}
