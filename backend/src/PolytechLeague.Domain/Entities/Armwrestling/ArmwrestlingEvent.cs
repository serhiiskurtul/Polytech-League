using PolytechLeague.Domain.Entities.Armwrestling.Enums;

namespace PolytechLeague.Domain.Entities.Armwrestling;

public class ArmwrestlingEvent : Event
{
    public ArmwrestlingHand Hand { get; set; }
    public ArmwrestlingWeightClass WeightClass { get; set; }
    public ArmwrestlingCategory Category { get; set; }
    public string? TimeControl { get; set; }
    public int ParticipantsCount { get; set; }
    
    public ICollection<ArmwrestlingMatch>? Matches { get; set; }
}
