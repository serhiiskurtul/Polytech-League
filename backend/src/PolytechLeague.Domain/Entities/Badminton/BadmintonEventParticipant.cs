using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Entities.Shared;

namespace PolytechLeague.Domain.Entities.Badminton;

public class BadmintonEventParticipant
{
    public long EventId { get; set; }
    public BadmintonEvent? Event { get; set; }

    public long PlayerId { get; set; }
    public Player? Player { get; set; }

    public double Points { get; set; } = 0;
    public int Rank { get; set; }
    public string? GroupName { get; set; }
    public RegistrationStatus Status { get; set; } = RegistrationStatus.Approved;
}
