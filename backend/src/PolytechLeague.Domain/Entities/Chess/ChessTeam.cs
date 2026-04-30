using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Domain.Entities.Chess;

public class ChessTeam
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public long EventId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public ChessEvent? Event { get; set; }

    public ICollection<Player>? Players { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<long>? PlayerIds { get; set; }

    public long? CollegeId { get; set; }
    public College? College { get; set; }
}
