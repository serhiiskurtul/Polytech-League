using PolytechLeague.Domain.Entities.Shared;

namespace PolytechLeague.Domain.Entities.BeachVolleyball;

public class BeachVolleyballEventParticipant
{
    public long EventId { get; set; }
    public BeachVolleyballEvent? Event { get; set; }

    public long PlayerId { get; set; }
    public Player? Player { get; set; }

    public RegistrationStatus Status { get; set; } = RegistrationStatus.Approved;
    public string? GroupName { get; set; }
}
