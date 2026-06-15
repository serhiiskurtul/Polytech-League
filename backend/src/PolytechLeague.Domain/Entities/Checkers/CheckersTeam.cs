using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Domain.Entities.Checkers;

public class CheckersTeam
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public long EventId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public CheckersEvent? Event { get; set; }

    public ICollection<Player>? Players { get; set; }
    
    public double Points { get; set; }
    public int Rank { get; set; }
    public string? TournamentGroupName { get; set; }
    public bool IsGeneralTableIncluded { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<long>? PlayerIds { get; set; }

    public long? CollegeId { get; set; }
    public College? College { get; set; }
}
