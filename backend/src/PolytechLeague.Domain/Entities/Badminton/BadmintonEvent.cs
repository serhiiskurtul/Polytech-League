using PolytechLeague.Domain.Entities.Badminton.Enums;

namespace PolytechLeague.Domain.Entities.Badminton;

public class BadmintonEvent : Event
{
    public BadmintonTournamentType TournamentType { get; set; }
    public ICollection<BadmintonMatch>? Matches { get; set; }
    public ICollection<BadmintonEventParticipant>? Participants { get; set; }
}
