using PolytechLeague.Domain.Entities.Basketball3x3.Enums;

namespace PolytechLeague.Domain.Entities.Basketball3x3;

public class Basketball3x3Event : Event
{
    public Basketball3x3TournamentType? TournamentType { get; set; }

    public ICollection<Basketball3x3Match>? Matches { get; set; }
    public ICollection<Basketball3x3Team>? Teams { get; set; }
    
    public long? FirstPlaceTeamId { get; set; }
    public Basketball3x3Team? FirstPlaceTeam { get; set; }
    public long? SecondPlaceTeamId { get; set; }
    public Basketball3x3Team? SecondPlaceTeam { get; set; }
    public long? ThirdPlaceTeamId { get; set; }
    public Basketball3x3Team? ThirdPlaceTeam { get; set; }
}
