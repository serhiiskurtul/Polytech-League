using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Domain.Entities.Futsal;

public class FutsalTeam
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public long EventId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public FutsalEvent? Event { get; set; }
    
    public ICollection<Player>? Players { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<long>? PlayerIds { get; set; }
}
