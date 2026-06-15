using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Domain.Entities.Badminton;

public class BadmintonTeam
{
    public long Id { get; set; }
    public string? Name { get; set; }

    public long? Player1Id { get; set; }
    public Player? Player1 { get; set; }

    public long? Player2Id { get; set; }
    public Player? Player2 { get; set; }
    
    public long? EventId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public BadmintonEvent? Event { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<long>? PlayerIds { get; set; }

    public double Points { get; set; }
    public int Rank { get; set; }
    public string? TournamentGroupName { get; set; }
    public bool IsGeneralTableIncluded { get; set; }

    public long? CollegeId { get; set; }
    public College? College { get; set; }
}
