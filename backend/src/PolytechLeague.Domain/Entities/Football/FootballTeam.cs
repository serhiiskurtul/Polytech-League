using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Domain.Entities.Football;

public class FootballTeam
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public long EventId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public FootballEvent? Event { get; set; }
    
    public ICollection<Player>? Players { get; set; }
    public long? CaptainId { get; set; }
    public Player? Captain { get; set; }
    public long? GoalkeeperId { get; set; }
    public Player? Goalkeeper { get; set; }
    public int Points { get; set; }
    public int Rank { get; set; }
    
    public string? TournamentGroupName { get; set; }
    public bool IsGeneralTableIncluded { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<long>? PlayerIds { get; set; }

    public long? CollegeId { get; set; }
    public College? College { get; set; }
}
