using PolytechLeague.Domain.Entities.Shared;

namespace PolytechLeague.Domain.Entities.Athletics;

public class AthleticsEventParticipant
{
    public long AthleticsEventId { get; set; }
    public AthleticsEvent? AthleticsEvent { get; set; }

    public long PlayerId { get; set; }
    public Player? Player { get; set; }

    public string? GroupName { get; set; }
    public string? PlayerFirstName { get; set; }
    public string? PlayerLastName { get; set; }
    public RegistrationStatus Status { get; set; } = RegistrationStatus.Approved;
    public long? CollegeId { get; set; }
    public College? College { get; set; }
}
