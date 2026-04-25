using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Domain.Entities.BeachVolleyball;

public class BeachVolleyballTeam
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public long EventId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public BeachVolleyballEvent? Event { get; set; }

    public ICollection<Player>? Players { get; set; }
    public int Rank { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<long>? PlayerIds { get; set; }
}
