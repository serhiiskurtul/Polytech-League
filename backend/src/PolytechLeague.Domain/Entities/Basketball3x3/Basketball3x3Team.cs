using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Domain.Entities.Basketball3x3;

public class Basketball3x3Team
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public long EventId { get; set; }
    public Basketball3x3Event? Event { get; set; }
    
    public ICollection<Player>? Players { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<long>? PlayerIds { get; set; }
}
