using PolytechLeague.Domain.Entities.Volleyball.Enums;

namespace PolytechLeague.Domain.Entities.Volleyball;

public class VolleyballPlayer : Player
{
    public long? TeamId { get; set; }
    public VolleyballTeam? Team { get; set; }

    public VolleyballPosition Position { get; set; }
    public int? ShirtNumber { get; set; }
}
