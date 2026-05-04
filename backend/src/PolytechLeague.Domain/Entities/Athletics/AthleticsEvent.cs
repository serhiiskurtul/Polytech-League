using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Domain.Entities.Athletics;

public class AthleticsEvent : Event
{
    public ICollection<AthleticsResult> Results { get; set; } = new List<AthleticsResult>();
    public ICollection<AthleticsEventParticipant> Participants { get; set; } = new List<AthleticsEventParticipant>();
}
