using PolytechLeague.Domain.Entities.Shared;

namespace PolytechLeague.Domain.Entities.Football;

public class FootballEventParticipant
{
    public long EventId { get; set; }
    public FootballEvent? Event { get; set; }

    public long PlayerId { get; set; }
    public Player? Player { get; set; }

    public RegistrationStatus Status { get; set; } = RegistrationStatus.Approved;
    public string? GroupName { get; set; }
}
