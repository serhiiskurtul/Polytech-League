using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Domain.Entities.Armwrestling;

public class ArmwrestlingEventParticipant
{
    public long TournamentId { get; set; }
    public ArmwrestlingEvent? Tournament { get; set; }

    public long PlayerId { get; set; }
    public Player? Player { get; set; }

    public double Weight { get; set; }
    public int Rank { get; set; }
}
