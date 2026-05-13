using PolytechLeague.Domain.Entities.Football.Enums;

namespace PolytechLeague.Domain.Entities.Football;

public class FootballEvent : Event
{
    public FootballTournamentType? TournamentType { get; set; }

    public ICollection<FootballMatch>? Matches { get; set; }
    public ICollection<FootballEventParticipant>? Participants { get; set; }
    public ICollection<FootballTeam>? Teams { get; set; }
    
    public long? FirstPlaceTeamId { get; set; }
    public FootballTeam? FirstPlaceTeam { get; set; }
    public long? SecondPlaceTeamId { get; set; }
    public FootballTeam? SecondPlaceTeam { get; set; }
    public long? ThirdPlaceTeamId { get; set; }
    public FootballTeam? ThirdPlaceTeam { get; set; }
}
