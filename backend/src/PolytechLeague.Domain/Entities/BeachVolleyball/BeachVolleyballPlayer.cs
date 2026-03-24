using PolytechLeague.Domain.Entities.BeachVolleyball.Enums;

namespace PolytechLeague.Domain.Entities.BeachVolleyball;

public class BeachVolleyballPlayer : Player
{
    public long? TeamId { get; set; }
    public BeachVolleyballTeam? Team { get; set; }
    public int? ShirtNumber { get; set; }
}
