using PolytechLeague.Domain.Entities.Shared;

namespace PolytechLeague.Domain.Entities.Basketball;

public class BasketballEventParticipant
{
    public long EventId { get; set; }
    public BasketballEvent? Event { get; set; }

    public long PlayerId { get; set; }
    public Player? Player { get; set; }

    public RegistrationStatus Status { get; set; } = RegistrationStatus.Approved;
    public string? GroupName { get; set; }
}
