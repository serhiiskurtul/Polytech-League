using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Domain.Entities.Volleyball;

public class VolleyballTeam
{
    public long Id { get; set; }
    public required string Name { get; set; }

    public ICollection<Player>? Players { get; set; }
}
