using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Entities.Shared;

namespace PolytechLeague.Domain.Entities.Armwrestling;

public class ArmwrestlingEventParticipant
{
    public long TournamentId { get; set; }
    public ArmwrestlingEvent? Tournament { get; set; }

    public long PlayerId { get; set; }
    public Player? Player { get; set; }

    public double Weight { get; set; }
    public string? GroupName { get; set; }
    public int Rank { get; set; }
    public RegistrationStatus Status { get; set; } = RegistrationStatus.Approved;
    public long? CollegeId { get; set; }
    public College? College { get; set; }
}
