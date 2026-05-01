using PolytechLeague.Domain.Entities.Shared;

namespace PolytechLeague.Domain.Entities.Basketball3x3;

public class Basketball3x3EventParticipant
{
    public long EventId { get; set; }
    public Basketball3x3Event? Event { get; set; }

    public long PlayerId { get; set; }
    public Player? Player { get; set; }

    public RegistrationStatus Status { get; set; } = RegistrationStatus.Approved;
    public string? GroupName { get; set; }
}
