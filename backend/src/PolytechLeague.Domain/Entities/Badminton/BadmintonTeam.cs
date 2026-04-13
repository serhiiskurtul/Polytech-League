using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Domain.Entities.Badminton;

public class BadmintonTeam
{
    public long Id { get; set; }
    public string? Name { get; set; } // Can be empty, e.g. "Ivanov/Petrov"

    public required long Player1Id { get; set; }
    public Player? Player1 { get; set; }

    public required long Player2Id { get; set; }
    public Player? Player2 { get; set; }
    
    public long? EventId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public BadmintonEvent? Event { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<long>? PlayerIds { get; set; }
}
