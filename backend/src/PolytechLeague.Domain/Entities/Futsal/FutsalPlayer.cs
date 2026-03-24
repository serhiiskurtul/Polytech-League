using PolytechLeague.Domain.Entities.Futsal.Enums;

namespace PolytechLeague.Domain.Entities.Futsal;

public class FutsalPlayer : Player
{
    public long? TeamId { get; set; }
    public FutsalTeam? Team { get; set; }
    
    public FutsalPosition Position { get; set; }
    public int? ShirtNumber { get; set; }
}
