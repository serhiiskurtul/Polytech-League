using PolytechLeague.Domain.Entities.Badminton.Enums;

namespace PolytechLeague.Domain.Entities.Badminton;

public class BadmintonEvent : Event
{
    public BadmintonTournamentType? TournamentType { get; set; }

    public ICollection<BadmintonMatch>? Matches { get; set; }
    public ICollection<BadmintonEventParticipant>? Participants { get; set; }
    public ICollection<BadmintonTeam>? Teams { get; set; }

    public long? FirstPlaceTeamId { get; set; }
    public BadmintonTeam? FirstPlaceTeam { get; set; }
    public long? SecondPlaceTeamId { get; set; }
    public BadmintonTeam? SecondPlaceTeam { get; set; }
    public long? ThirdPlaceTeamId { get; set; }
    public BadmintonTeam? ThirdPlaceTeam { get; set; }
}
