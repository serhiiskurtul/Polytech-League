namespace PolytechLeague.Domain.Entities.Basketball;

public class BasketballMatch
{
    public long Id { get; set; }
    public long EventId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public BasketballEvent? Event { get; set; }
    
    public long HomeTeamId { get; set; }
    public BasketballTeam? HomeTeam { get; set; }
    
    public long AwayTeamId { get; set; }
    public BasketballTeam? AwayTeam { get; set; }
    
    public ICollection<Player>? HomeRoster { get; set; }
    public ICollection<Player>? AwayRoster { get; set; }
    
    public int? HomeTeamScore { get; set; }
    public int? AwayTeamScore { get; set; }
    
    public long? WinnerTeamId { get; set; }
    public BasketballTeam? WinnerTeam { get; set; }
    
    public string? Stage { get; set; } // e.g., Group Stage, Quarterfinal

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<long>? HomeRosterIds { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<long>? AwayRosterIds { get; set; }
}
