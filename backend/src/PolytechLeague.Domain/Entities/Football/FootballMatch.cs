namespace PolytechLeague.Domain.Entities.Football;

public class FootballMatch
{
    public long Id { get; set; }
    public long EventId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public FootballEvent? Event { get; set; }
    
    public long HomeTeamId { get; set; }
    public FootballTeam? HomeTeam { get; set; }
    
    public long AwayTeamId { get; set; }
    public FootballTeam? AwayTeam { get; set; }
    
    public ICollection<Player>? HomeRoster { get; set; }
    public ICollection<Player>? AwayRoster { get; set; }
    
    public int? HomeTeamScore { get; set; }
    public int? AwayTeamScore { get; set; }
    
    public long? WinnerTeamId { get; set; }
    public FootballTeam? WinnerTeam { get; set; }
    
    public string? Stage { get; set; } // e.g., Group Stage, Quarterfinal

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<long>? HomeRosterIds { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<long>? AwayRosterIds { get; set; }

    public virtual ICollection<FootballGoal>? Goals { get; set; }
    public virtual ICollection<FootballCard>? Cards { get; set; }
    
    public long? HomeGoalkeeperId { get; set; }
    public Player? HomeGoalkeeper { get; set; }
    public long? AwayGoalkeeperId { get; set; }
    public Player? AwayGoalkeeper { get; set; }

    public long? HomeCaptainId { get; set; }
    public Player? HomeCaptain { get; set; }
    public long? AwayCaptainId { get; set; }
    public Player? AwayCaptain { get; set; }
}

