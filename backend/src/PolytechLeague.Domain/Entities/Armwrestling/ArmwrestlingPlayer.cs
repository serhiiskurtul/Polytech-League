using PolytechLeague.Domain.Entities.Armwrestling.Enums;

namespace PolytechLeague.Domain.Entities.Armwrestling;

public class ArmwrestlingPlayer : Player
{
    public float CurrentWeight { get; set; }
    public ArmwrestlingWeightClass PreferredWeightClass { get; set; }
}
