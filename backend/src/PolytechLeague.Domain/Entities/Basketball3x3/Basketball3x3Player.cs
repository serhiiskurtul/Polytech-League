using PolytechLeague.Domain.Entities.Basketball3x3.Enums;

namespace PolytechLeague.Domain.Entities.Basketball3x3;

public class Basketball3x3Player : Player
{
    public long? TeamId { get; set; }
    public Basketball3x3Team? Team { get; set; }
    
    public int? ShirtNumber { get; set; }
}
