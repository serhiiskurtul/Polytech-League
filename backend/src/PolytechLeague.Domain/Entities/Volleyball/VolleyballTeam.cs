namespace PolytechLeague.Domain.Entities.Volleyball;

public class VolleyballTeam
{
    public long Id { get; set; }
    public required string Name { get; set; }

    public ICollection<VolleyballPlayer>? Players { get; set; }
}
